using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaCalendario.Migrations
{
    /// <inheritdoc />
    public partial class EmailConfirmacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoConfirmacao",
                table: "Utilizadores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmado",
                table: "Utilizadores",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoConfirmacao",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "EmailConfirmado",
                table: "Utilizadores");
        }
    }
}
