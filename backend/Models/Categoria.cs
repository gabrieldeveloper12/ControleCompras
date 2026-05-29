using System.Text.Json.Serialization;

namespace backend.Models;

public class Categoria
{
    public int Id { get; set; }
    public required string Nome { get; set; } // "Mercado", "Farmácia", "Uber", "Outros"
    public string? Icone { get; set; } // Emoji ou classe de ícone CSS
    public string? CorHex { get; set; } // Cor hexadecimal (ex: #FF5733)

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [JsonIgnore]
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
