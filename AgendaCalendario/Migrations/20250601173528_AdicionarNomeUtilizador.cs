using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaCalendario.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNomeUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Utilizadores",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Utilizadores");
        }
    }
}
