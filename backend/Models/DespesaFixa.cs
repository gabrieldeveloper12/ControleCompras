using System.Text.Json.Serialization;

namespace backend.Models;

public class DespesaFixa
{
    public int Id { get; set; }
    public required string Descricao { get; set; }
    public decimal Valor { get; set; }
    public int DiaVencimento { get; set; } // 1 a 31
    public bool Ativa { get; set; } = true;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [JsonIgnore]
    public ICollection<PagamentoDespesaFixa> Pagamentos { get; set; } = new List<PagamentoDespesaFixa>();
}
