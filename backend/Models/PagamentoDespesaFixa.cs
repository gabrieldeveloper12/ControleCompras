using System.Text.Json.Serialization;

namespace backend.Models;

public class PagamentoDespesaFixa
{
    public int Id { get; set; }
    public int DespesaFixaId { get; set; }
    public DespesaFixa? DespesaFixa { get; set; }

    public int Mes { get; set; } // 1 a 12
    public int Ano { get; set; }
    public bool Pago { get; set; }
    public DateTime? DataPagamento { get; set; }
    public decimal ValorPago { get; set; }

    // Chave estrangeira e navegação para a compra associada gerada
    public int? CompraGeradaId { get; set; }
    public Compra? CompraGerada { get; set; }
}
