using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixPostgresDateTimeTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"DataCriacao\" TYPE timestamp with time zone USING (\"DataCriacao\"::timestamp with time zone);");
                migrationBuilder.Sql("ALTER TABLE \"PagamentosDespesasFixas\" ALTER COLUMN \"DataPagamento\" TYPE timestamp with time zone USING (\"DataPagamento\"::timestamp with time zone);");
                migrationBuilder.Sql("ALTER TABLE \"Compras\" ALTER COLUMN \"Data\" TYPE timestamp with time zone USING (\"Data\"::timestamp with time zone);");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"DataCriacao\" TYPE text;");
                migrationBuilder.Sql("ALTER TABLE \"PagamentosDespesasFixas\" ALTER COLUMN \"DataPagamento\" TYPE text;");
                migrationBuilder.Sql("ALTER TABLE \"Compras\" ALTER COLUMN \"Data\" TYPE text;");
            }
        }
    }
}
