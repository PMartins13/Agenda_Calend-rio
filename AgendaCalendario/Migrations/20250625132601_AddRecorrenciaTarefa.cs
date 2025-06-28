using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaCalendario.Migrations
{
    /// <inheritdoc />
    public partial class AddRecorrenciaTarefa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFimRecorrencia",
                table: "Tarefas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Recorrencia",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFimRecorrencia",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Recorrencia",
                table: "Tarefas");
        }
    }
}
