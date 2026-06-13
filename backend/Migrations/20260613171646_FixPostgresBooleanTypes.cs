using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class FixPostgresBooleanTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" DROP DEFAULT;");
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" TYPE boolean USING (CASE WHEN \"Ativa\" = 0 THEN false ELSE true END);");
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" SET DEFAULT true;");

                migrationBuilder.Sql("ALTER TABLE \"PagamentosDespesasFixas\" ALTER COLUMN \"Pago\" TYPE boolean USING (CASE WHEN \"Pago\" = 0 THEN false ELSE true END);");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" DROP DEFAULT;");
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" TYPE integer USING (CASE WHEN \"Ativa\" = true THEN 1 ELSE 0 END);");
                migrationBuilder.Sql("ALTER TABLE \"DespesasFixas\" ALTER COLUMN \"Ativa\" SET DEFAULT 1;");

                migrationBuilder.Sql("ALTER TABLE \"PagamentosDespesasFixas\" ALTER COLUMN \"Pago\" TYPE integer USING (CASE WHEN \"Pago\" = true THEN 1 ELSE 0 END);");
            }
        }
    }
}
