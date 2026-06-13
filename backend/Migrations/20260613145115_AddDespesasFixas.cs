using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDespesasFixas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PagamentoDespesaFixaId",
                table: "Compras",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DespesasFixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Valor = table.Column<double>(type: "REAL", nullable: false),
                    DiaVencimento = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativa = table.Column<bool>(nullable: false, defaultValue: true),
                    DataCriacao = table.Column<DateTime>(nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespesasFixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespesasFixas_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DespesasFixas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagamentosDespesasFixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DespesaFixaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Pago = table.Column<bool>(nullable: false),
                    DataPagamento = table.Column<DateTime>(nullable: true),
                    ValorPago = table.Column<double>(type: "REAL", nullable: false),
                    CompraGeradaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosDespesasFixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosDespesasFixas_DespesasFixas_DespesaFixaId",
                        column: x => x.DespesaFixaId,
                        principalTable: "DespesasFixas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compras_PagamentoDespesaFixaId",
                table: "Compras",
                column: "PagamentoDespesaFixaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DespesasFixas_CategoriaId",
                table: "DespesasFixas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespesasFixas_UsuarioId",
                table: "DespesasFixas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosDespesasFixas_DespesaFixaId",
                table: "PagamentosDespesasFixas",
                column: "DespesaFixaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Compras_PagamentosDespesasFixas_PagamentoDespesaFixaId",
                table: "Compras",
                column: "PagamentoDespesaFixaId",
                principalTable: "PagamentosDespesasFixas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Compras_PagamentosDespesasFixas_PagamentoDespesaFixaId",
                table: "Compras");

            migrationBuilder.DropTable(
                name: "PagamentosDespesasFixas");

            migrationBuilder.DropTable(
                name: "DespesasFixas");

            migrationBuilder.DropIndex(
                name: "IX_Compras_PagamentoDespesaFixaId",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "PagamentoDespesaFixaId",
                table: "Compras");
        }
    }
}
