using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaCalendario.Migrations
{
    /// <inheritdoc />
    public partial class RelacaoMuitosParaMuitos_TarefasCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TarefasCategorias",
                columns: table => new
                {
                    TarefaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TarefasCategorias", x => new { x.TarefaId, x.CategoriaId });
                    table.ForeignKey(
                        name: "FK_TarefasCategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TarefasCategorias_Tarefas_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "Tarefas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TarefasCategorias_CategoriaId",
                table: "TarefasCategorias",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TarefasCategorias");
        }
    }
}
