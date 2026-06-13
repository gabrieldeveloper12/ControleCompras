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
    options.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
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

        // Garante que a tabela de histórico de migrações existe e está populada com as migrações anteriores
        // para evitar que o EF Core tente recriar tabelas que já existem em produção (Postgres)
        try
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                context.Database.OpenConnection();
                
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                        ""MigrationId"" varchar(150) NOT NULL,
                        ""ProductVersion"" varchar(32) NOT NULL,
                        CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                    );";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20260527173704_InitialCreate', '10.0.8')
                    ON CONFLICT DO NOTHING;";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                    VALUES ('20260529195900_AddUsuarioEntity', '10.0.8')
                    ON CONFLICT DO NOTHING;";
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            // Silencia erro para fallback seguro no Migrate()
        }

        // Aplica todas as migrações pendentes no banco de dados (SQLite local ou Postgres no Render)
        context.Database.Migrate();
        // Remove old error file if migrations succeed
        if (File.Exists("migration_error.txt"))
        {
            try { File.Delete("migration_error.txt"); } catch {}
        }
    }
    catch (Exception ex)
    {
        try { File.WriteAllText("migration_error.txt", ex.ToString()); } catch {}
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
app.MapGet("/health", () => Results.Ok("healthy"));
app.MapGet("/api/debug-db", async (ComprasDbContext context) =>
{
    try
    {
        var pending = await context.Database.GetPendingMigrationsAsync();
        var applied = await context.Database.GetAppliedMigrationsAsync();
        string? migrationError = null;
        if (File.Exists("migration_error.txt"))
        {
            migrationError = await File.ReadAllTextAsync("migration_error.txt");
        }
        return Results.Ok(new
        {
            Provider = context.Database.ProviderName,
            Pending = pending,
            Applied = applied,
            MigrationError = migrationError
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.ToString());
    }
});

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
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 10)
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

    if (mes.HasValue && ano.HasValue)
    {
        var dataInicioMes = new DateTime(ano.Value, mes.Value, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFimMes = dataInicioMes.AddMonths(1);
        query = query.Where(c => c.Data >= dataInicioMes && c.Data < dataFimMes);
    }
    else if (ano.HasValue)
    {
        var dataInicioAno = new DateTime(ano.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFimAno = dataInicioAno.AddYears(1);
        query = query.Where(c => c.Data >= dataInicioAno && c.Data < dataFimAno);
    }
    else if (mes.HasValue)
    {
        query = query.Where(c => c.Data.Month == mes.Value);
    }

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

// ==========================================
// ENDPOINTS DE DESPESAS FIXAS
// ==========================================

// Listar despesas fixas
app.MapGet("/api/despesas-fixas", async (ClaimsPrincipal user, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var despesas = await db.DespesasFixas
        .Include(df => df.Categoria)
        .Where(df => df.UsuarioId == userId && df.Ativa)
        .OrderBy(df => df.DiaVencimento)
        .ToListAsync();
    return Results.Ok(despesas);
}).RequireAuthorization();

// Criar nova despesa fixa
app.MapPost("/api/despesas-fixas", async (ClaimsPrincipal user, CriarDespesaFixaRequest req, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    if (string.IsNullOrWhiteSpace(req.Descricao) || req.Descricao.Length < 3)
        return Results.BadRequest("Descrição deve ter pelo menos 3 caracteres.");
    if (req.Valor <= 0)
        return Results.BadRequest("Valor deve ser maior que zero.");
    if (req.DiaVencimento < 1 || req.DiaVencimento > 31)
        return Results.BadRequest("Dia de vencimento deve ser entre 1 e 31.");

    var despesa = new DespesaFixa
    {
        Descricao = req.Descricao,
        Valor = req.Valor,
        DiaVencimento = req.DiaVencimento,
        CategoriaId = req.CategoriaId,
        UsuarioId = userId,
        Ativa = true,
        DataCriacao = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
    };

    db.DespesasFixas.Add(despesa);
    await db.SaveChangesAsync();

    await db.Entry(despesa).Reference(df => df.Categoria).LoadAsync();
    return Results.Created($"/api/despesas-fixas/{despesa.Id}", despesa);
}).RequireAuthorization();

// Editar despesa fixa
app.MapPut("/api/despesas-fixas/{id:int}", async (ClaimsPrincipal user, int id, CriarDespesaFixaRequest req, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var despesa = await db.DespesasFixas.FirstOrDefaultAsync(df => df.Id == id && df.UsuarioId == userId);
    if (despesa is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(req.Descricao) || req.Descricao.Length < 3)
        return Results.BadRequest("Descrição deve ter pelo menos 3 caracteres.");
    if (req.Valor <= 0)
        return Results.BadRequest("Valor deve ser maior que zero.");
    if (req.DiaVencimento < 1 || req.DiaVencimento > 31)
        return Results.BadRequest("Dia de vencimento deve ser entre 1 e 31.");

    despesa.Descricao = req.Descricao;
    despesa.Valor = req.Valor;
    despesa.DiaVencimento = req.DiaVencimento;
    despesa.CategoriaId = req.CategoriaId;

    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// Excluir (inativar) despesa fixa
app.MapDelete("/api/despesas-fixas/{id:int}", async (ClaimsPrincipal user, int id, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var despesa = await db.DespesasFixas.FirstOrDefaultAsync(df => df.Id == id && df.UsuarioId == userId);
    if (despesa is null) return Results.NotFound();

    despesa.Ativa = false;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

// Listar pagamentos mensais
app.MapGet("/api/despesas-fixas/pagamentos", async (ClaimsPrincipal user, int mes, int ano, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // Despesas fixas ativas criadas até o mês/ano solicitado (simplificado para o dia 28 para evitar erros de borda)
    var despesasAtivas = await db.DespesasFixas
        .Include(df => df.Categoria)
        .Where(df => df.UsuarioId == userId && df.Ativa && df.DataCriacao <= DateTime.SpecifyKind(new DateTime(ano, mes, 28), DateTimeKind.Utc))
        .ToListAsync();

    var pagamentos = await db.PagamentosDespesasFixas
        .Include(p => p.CompraGerada)
        .Where(p => p.DespesaFixa!.UsuarioId == userId && p.Mes == mes && p.Ano == ano)
        .ToListAsync();

    var result = despesasAtivas.Select(df =>
    {
        var pagamento = pagamentos.FirstOrDefault(p => p.DespesaFixaId == df.Id);
        return new
        {
            DespesaFixaId = df.Id,
            Descricao = df.Descricao,
            ValorOriginal = df.Valor,
            DiaVencimento = df.DiaVencimento,
            Categoria = df.Categoria,
            Pago = pagamento != null && pagamento.Pago,
            DataPagamento = pagamento?.DataPagamento,
            ValorPago = pagamento != null ? pagamento.ValorPago : df.Valor,
            CompraGeradaId = pagamento?.CompraGeradaId
        };
    }).OrderBy(x => x.DiaVencimento).ToList();

    return Results.Ok(result);
}).RequireAuthorization();

// Alternar status de pagamento (Marcar Pago / Estornar)
app.MapPost("/api/despesas-fixas/pagamentos/toggle", async (ClaimsPrincipal user, TogglePagamentoRequest req, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var despesa = await db.DespesasFixas.FirstOrDefaultAsync(df => df.Id == req.DespesaFixaId && df.UsuarioId == userId);
    if (despesa is null) return Results.BadRequest("Despesa fixa não encontrada.");

    var pagamento = await db.PagamentosDespesasFixas
        .Include(p => p.CompraGerada)
        .FirstOrDefaultAsync(p => p.DespesaFixaId == req.DespesaFixaId && p.Mes == req.Mes && p.Ano == req.Ano);

    if (pagamento != null && pagamento.Pago)
    {
        // Estornar pagamento (deletar compra automática e registro de pagamento)
        if (pagamento.CompraGeradaId.HasValue)
        {
            var compra = await db.Compras.FirstOrDefaultAsync(c => c.Id == pagamento.CompraGeradaId.Value && c.UsuarioId == userId);
            if (compra != null)
            {
                db.Compras.Remove(compra);
            }
        }
        db.PagamentosDespesasFixas.Remove(pagamento);
        await db.SaveChangesAsync();
        return Results.Ok(new { pago = false });
    }
    else
    {
        // Marcar como pago
        var dataPagamento = req.DataPagamento ?? DateTime.UtcNow;
        
        // 1. Criar Compra automática
        var compra = new Compra
        {
            Descricao = $"{despesa.Descricao} (Fixo)",
            Valor = req.ValorPago ?? despesa.Valor,
            Data = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc),
            CategoriaId = despesa.CategoriaId,
            UsuarioId = userId,
            FormaPagamento = "pix",
            TotalParcelas = null,
            NumeroParcela = null,
            GrupoParcelaId = null
        };
        db.Compras.Add(compra);
        await db.SaveChangesAsync(); // gera o Id da Compra

        // 2. Criar registro de PagamentoDespesaFixa
        var novoPagamento = new PagamentoDespesaFixa
        {
            DespesaFixaId = despesa.Id,
            Mes = req.Mes,
            Ano = req.Ano,
            Pago = true,
            DataPagamento = DateTime.SpecifyKind(dataPagamento, DateTimeKind.Utc),
            ValorPago = req.ValorPago ?? despesa.Valor,
            CompraGeradaId = compra.Id
        };
        db.PagamentosDespesasFixas.Add(novoPagamento);
        await db.SaveChangesAsync();

        // 3. Vincular a compra de volta ao pagamento
        compra.PagamentoDespesaFixaId = novoPagamento.Id;
        await db.SaveChangesAsync();

        return Results.Ok(new { pago = true, compraId = compra.Id });
    }
}).RequireAuthorization();

// Obter próximos vencimentos do mês corrente
app.MapGet("/api/despesas-fixas/proximos-vencimentos", async (ClaimsPrincipal user, ComprasDbContext db) =>
{
    var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var agora = DateTime.UtcNow;
    var mes = agora.Month;
    var ano = agora.Year;

    var despesasAtivas = await db.DespesasFixas
        .Include(df => df.Categoria)
        .Where(df => df.UsuarioId == userId && df.Ativa)
        .ToListAsync();

    var pagamentosEfetuadosIds = await db.PagamentosDespesasFixas
        .Where(p => p.DespesaFixa!.UsuarioId == userId && p.Mes == mes && p.Ano == ano && p.Pago)
        .Select(p => p.DespesaFixaId)
        .ToListAsync();

    var proximosVencimentos = despesasAtivas
        .Where(df => !pagamentosEfetuadosIds.Contains(df.Id))
        .Select(df => new
        {
            df.Id,
            df.Descricao,
            df.Valor,
            df.DiaVencimento,
            Categoria = df.Categoria
        })
        .OrderBy(df => df.DiaVencimento)
        .ToList();

    return Results.Ok(proximosVencimentos);
}).RequireAuthorization();

app.Run();

public record LoginRequest(string Email, string Password);
public record RegisterRequest(string Email, string Password, string Nome);
public record CriarCompraRequest(string Descricao, decimal ValorTotal, DateTime Data, int CategoriaId, string? FormaPagamento, int? TotalParcelas);
public record CriarDespesaFixaRequest(string Descricao, decimal Valor, int DiaVencimento, int CategoriaId);
public record TogglePagamentoRequest(int DespesaFixaId, int Mes, int Ano, DateTime? DataPagamento, decimal? ValorPago);

