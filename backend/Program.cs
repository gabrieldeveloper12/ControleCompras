using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext (PostgreSQL para produção, SQLite para local)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ComprasDbContext>(options =>
{
    if (connectionString != null && (connectionString.Contains("Host=") || connectionString.Contains("Server=") || connectionString.Contains("Database=")))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Configuração de CORS para permitir chamadas do front-end Vue 3
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registrar OpenAPI para documentação da API
builder.Services.AddOpenApi();

// Configuração de Autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "UmaChaveSuperSecretaParaOControleDeComprasSeguro123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ControleCompras";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Rodar migrações e inicializar banco de dados de categorias
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ComprasDbContext>();
        
        // Garante que o banco e as tabelas estejam criados (funciona tanto para SQLite quanto Postgres)
        // Usamos IRelationalDatabaseCreator para criar as tabelas mesmo se a tabela de histórico de migrações já existir
        var databaseCreator = context.Database.GetService<Microsoft.EntityFrameworkCore.Storage.IRelationalDatabaseCreator>();
        try
        {
            databaseCreator.CreateTables();
        }
        catch
        {
            // Se as tabelas já existirem, ignoramos o erro e prosseguimos
        }

        // Aplica todas as migrações pendentes no banco de dados (SQLite local ou Postgres no Render)
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao rodar as migrações ou ao realizar o Seed do banco.");
    }
}

// Usar CORS, Autenticação e OpenAPI no pipeline
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Controle de Compras API está online e banco de dados configurado!");

// ==========================================
// AUTH ENDPOINTS
// ==========================================
app.MapPost("/api/auth/register", async (RegisterRequest req, ComprasDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
        return Results.BadRequest("Email e Senha são obrigatórios.");

    if (await db.Usuarios.AnyAsync(u => u.Email == req.Email))
        return Results.BadRequest("Email já em uso.");

    var user = new Usuario
    {
        Email = req.Email,
        Nome = string.IsNullOrWhiteSpace(req.Nome) ? req.Email : req.Nome,
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Password)
    };
    
    db.Usuarios.Add(user);
    await db.SaveChangesAsync();
    
    // Create standard categories for the new user
    db.Categorias.AddRange(
        new Categoria { Nome = "Mercado", Icone = "🛒", CorHex = "#4CAF50", UsuarioId = user.Id },
        new Categoria { Nome = "Farmácia", Icone = "💊", CorHex = "#F44336", UsuarioId = user.Id },
        new Categoria { Nome = "Uber", Icone = "🚗", CorHex = "#2196F3", UsuarioId = user.Id },
        new Categoria { Nome = "Outros", Icone = "📦", CorHex = "#9E9E9E", UsuarioId = user.Id }
    );
    await db.SaveChangesAsync();

    return Results.Ok(new { success = true });
});

app.MapPost("/api/auth/login", async (LoginRequest req, ComprasDbContext db) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == req.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.SenhaHash))
        return Results.BadRequest("Credenciais inválidas.");

    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(jwtKey);
    var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nome)
        }),
        Expires = DateTime.UtcNow.AddHours(8),
        Issuer = jwtIssuer,
        Audience = jwtIssuer,
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new 
    { 
        token = tokenHandler.WriteToken(token),
        user = new { id = user.Id, email = user.Email, nome = user.Nome }
    });
});

// ==========================================
// ENDPOINTS DE CATEGORIAS
// ==========================================

// Listar categorias
app.MapGet("/api/categorias", async (ClaimsPrincipal user, ComprasDbContext db) => {
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    return await db.Categorias.Where(c => c.UsuarioId == userId).ToListAsync();
}).RequireAuthorization();

// Criar nova categoria
app.MapPost("/api/categorias", async (ClaimsPrincipal user, Categoria categoria, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    if (string.IsNullOrWhiteSpace(categoria.Nome))
        return Results.BadRequest("O nome da categoria é obrigatório.");

    var existe = await db.Categorias.AnyAsync(c => c.UsuarioId == userId && c.Nome.ToLower() == categoria.Nome.ToLower());
    if (existe)
        return Results.BadRequest("Já existe uma categoria com este nome.");

    categoria.UsuarioId = userId;
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync();
    return Results.Created($"/api/categorias/{categoria.Id}", categoria);
}).RequireAuthorization();

// Atualizar categoria
app.MapPut("/api/categorias/{id:int}", async (ClaimsPrincipal user, int id, Categoria inputCat, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
    if (categoria is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(inputCat.Nome))
        return Results.BadRequest("O nome da categoria é obrigatório.");

    var existeDuplicado = await db.Categorias.AnyAsync(c => c.UsuarioId == userId && c.Id != id && c.Nome.ToLower() == inputCat.Nome.ToLower());
    if (existeDuplicado)
        return Results.BadRequest("Já existe outra categoria com este nome.");

    categoria.Nome = inputCat.Nome;
    categoria.Icone = inputCat.Icone;
    categoria.CorHex = inputCat.CorHex;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// Excluir categoria (bloqueia se houver compras vinculadas)
app.MapDelete("/api/categorias/{id:int}", async (ClaimsPrincipal user, int id, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var categoria = await db.Categorias.Include(c => c.Compras).FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
    if (categoria is null) return Results.NotFound();

    if (categoria.Compras.Any())
        return Results.BadRequest("Não é possível excluir uma categoria que possui compras associadas.");

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// ==========================================
// ENDPOINTS DE COMPRAS
// ==========================================

// Listar compras com filtros de data e mês/ano
app.MapGet("/api/compras", async (
    ClaimsPrincipal user,
    int? mes, 
    int? ano, 
    DateTime? inicio, 
    DateTime? fim, 
    ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var query = db.Compras.Include(c => c.Categoria).Where(c => c.UsuarioId == userId).AsQueryable();

    Console.WriteLine($"[API] GetCompras: inicio={inicio}, fim={fim}, mes={mes}, ano={ano}");

    if (inicio.HasValue)
    {
        var dataInicio = DateTime.SpecifyKind(inicio.Value.Date, DateTimeKind.Utc);
        query = query.Where(c => c.Data >= dataInicio);
    }

    if (fim.HasValue)
    {
        var dataFim = DateTime.SpecifyKind(fim.Value.Date.AddDays(1), DateTimeKind.Utc);
        query = query.Where(c => c.Data < dataFim);
    }

    if (mes.HasValue)
        query = query.Where(c => c.Data.Month == mes.Value);

    if (ano.HasValue)
        query = query.Where(c => c.Data.Year == ano.Value);

    var resultado = await query.OrderByDescending(c => c.Data).ToListAsync();
    return Results.Ok(resultado);
}).RequireAuthorization();

// Cadastrar nova compra
app.MapPost("/api/compras", async (ClaimsPrincipal user, CriarCompraRequest req, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    if (string.IsNullOrWhiteSpace(req.Descricao))
        return Results.BadRequest("A descrição da compra é obrigatória.");

    if (req.ValorTotal <= 0)
        return Results.BadRequest("O valor da compra deve ser maior que zero.");

    var categoriaExiste = await db.Categorias.AnyAsync(c => c.Id == req.CategoriaId && c.UsuarioId == userId);
    if (!categoriaExiste)
        return Results.BadRequest("A categoria informada não existe ou não pertence a você.");

    var dataBase = DateTime.SpecifyKind(req.Data, DateTimeKind.Utc);

    if (req.FormaPagamento == "cartao_parcelado" && req.TotalParcelas.HasValue && req.TotalParcelas.Value >= 2)
    {
        int totalParcelas = req.TotalParcelas.Value;
        if (totalParcelas > 12)
            return Results.BadRequest("O número máximo de parcelas é 12.");

        decimal valorBase = Math.Floor((req.ValorTotal / totalParcelas) * 100) / 100;
        decimal valorExtra = req.ValorTotal - (valorBase * totalParcelas);
        Guid grupoId = Guid.NewGuid();
        var comprasCriadas = new List<Compra>();

        for (int i = 1; i <= totalParcelas; i++)
        {
            decimal valorParcela = (i == 1) ? valorBase + valorExtra : valorBase;
            DateTime dataParcela = dataBase.AddMonths(i - 1);

            var compra = new Compra
            {
                Descricao = req.Descricao,
                Valor = valorParcela,
                Data = DateTime.SpecifyKind(dataParcela, DateTimeKind.Utc),
                CategoriaId = req.CategoriaId,
                UsuarioId = userId,
                FormaPagamento = req.FormaPagamento,
                TotalParcelas = totalParcelas,
                NumeroParcela = i,
                GrupoParcelaId = grupoId
            };

            db.Compras.Add(compra);
            comprasCriadas.Add(compra);
        }

        await db.SaveChangesAsync();

        foreach (var c in comprasCriadas)
        {
            await db.Entry(c).Reference(compra => compra.Categoria).LoadAsync();
        }

        return Results.Created("/api/compras", comprasCriadas);
    }
    else
    {
        var compra = new Compra
        {
            Descricao = req.Descricao,
            Valor = req.ValorTotal,
            Data = dataBase,
            CategoriaId = req.CategoriaId,
            UsuarioId = userId,
            FormaPagamento = req.FormaPagamento,
            TotalParcelas = null,
            NumeroParcela = null,
            GrupoParcelaId = null
        };

        db.Compras.Add(compra);
        await db.SaveChangesAsync();

        await db.Entry(compra).Reference(c => c.Categoria).LoadAsync();

        return Results.Created($"/api/compras/{compra.Id}", compra);
    }
}).RequireAuthorization();

// Atualizar compra existente
app.MapPut("/api/compras/{id:int}", async (ClaimsPrincipal user, int id, Compra inputCompra, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var compra = await db.Compras.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
    if (compra is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(inputCompra.Descricao))
        return Results.BadRequest("A descrição da compra é obrigatória.");

    if (inputCompra.Valor <= 0)
        return Results.BadRequest("O valor da compra deve ser maior que zero.");

    var categoriaExiste = await db.Categorias.AnyAsync(c => c.Id == inputCompra.CategoriaId && c.UsuarioId == userId);
    if (!categoriaExiste)
        return Results.BadRequest("A categoria informada não existe ou não pertence a você.");

    compra.Descricao = inputCompra.Descricao;
    compra.Valor = inputCompra.Valor;
    compra.Data = DateTime.SpecifyKind(inputCompra.Data, DateTimeKind.Utc);
    compra.CategoriaId = inputCompra.CategoriaId;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// Excluir compra
app.MapDelete("/api/compras/{id:int}", async (ClaimsPrincipal user, int id, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var compra = await db.Compras.FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == userId);
    if (compra is null) return Results.NotFound();

    if (compra.GrupoParcelaId.HasValue)
    {
        var comprasDoGrupo = await db.Compras
            .Where(c => c.GrupoParcelaId == compra.GrupoParcelaId && c.UsuarioId == userId)
            .ToListAsync();
        
        db.Compras.RemoveRange(comprasDoGrupo);
    }
    else
    {
        db.Compras.Remove(compra);
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Nome);
public record CriarCompraRequest(string Descricao, decimal ValorTotal, DateTime Data, int CategoriaId, string? FormaPagamento, int? TotalParcelas);
