namespace backend.Models;

public class Compra
{
    public int Id { get; set; }
    public required string Descricao { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }

    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public string? FormaPagamento { get; set; }
    public int? TotalParcelas { get; set; }
    public int? NumeroParcela { get; set; }
    public Guid? GrupoParcelaId { get; set; }
}
