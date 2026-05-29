using System.Text.Json.Serialization;

namespace backend.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    
    [JsonIgnore]
    public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
}
