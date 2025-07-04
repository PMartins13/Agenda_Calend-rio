﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaCalendario.Migrations
{
    /// <inheritdoc />
    public partial class AddPerfilUtilizadorToUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PerfilUtilizador",
                table: "Utilizadores",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerfilUtilizador",
                table: "Utilizadores");
        }
    }
}
