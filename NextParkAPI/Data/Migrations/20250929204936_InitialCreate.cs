using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextParkAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_NEXTPARK_MOTO",
                columns: table => new
                {
                    ID_MOTO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NR_PLACA = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NM_MODELO = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ST_MOTO = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    ID_VAGA = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NEXTPARK_MOTO", x => x.ID_MOTO);
                });

            migrationBuilder.CreateTable(
                name: "TB_NEXTPARK_VAGA",
                columns: table => new
                {
                    ID_VAGA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AREA_VAGA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ST_VAGA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_PATIO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NEXTPARK_VAGA", x => x.ID_VAGA);
                });

            migrationBuilder.CreateTable(
                name: "TB_NEXTPARK_MANUTENCAO",
                columns: table => new
                {
                    ID_MANUTENCAO = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DS_MANUTENCAO = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DT_INICIO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DT_FIM = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ID_MOTO = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NEXTPARK_MANUTENCAO", x => x.ID_MANUTENCAO);
                    table.ForeignKey(
                        name: "FK_TB_NEXTPARK_MANUTENCAO_TB_NEXTPARK_MOTO_ID_MOTO",
                        column: x => x.ID_MOTO,
                        principalTable: "TB_NEXTPARK_MOTO",
                        principalColumn: "ID_MOTO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_NEXTPARK_MANUTENCAO_ID_MOTO",
                table: "TB_NEXTPARK_MANUTENCAO",
                column: "ID_MOTO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_MANUTENCAO");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_VAGA");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_MOTO");
        }
    }
}
