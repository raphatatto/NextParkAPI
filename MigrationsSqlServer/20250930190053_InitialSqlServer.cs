using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextParkAPI.MigrationsSqlServer
{
    /// <inheritdoc />
    public partial class InitialSqlServer : Migration
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
                name: "TB_NEXTPARK_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "int", nullable: false),
                    NR_EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NEXTPARK_USUARIO", x => x.ID_USUARIO);
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

            migrationBuilder.CreateTable(
                name: "TB_NEXTPARK_LOGIN",
                columns: table => new
                {
                    ID_LOGIN = table.Column<int>(type: "int", nullable: false),
                    ID_USUARIO = table.Column<int>(type: "int", nullable: false),
                    NR_EMAIL = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DS_SENHA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_NEXTPARK_LOGIN", x => x.ID_LOGIN);
                    table.ForeignKey(
                        name: "FK_TB_NEXTPARK_LOGIN_TB_NEXTPARK_USUARIO_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "TB_NEXTPARK_USUARIO",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_NEXTPARK_LOGIN_ID_USUARIO",
                table: "TB_NEXTPARK_LOGIN",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_TB_NEXTPARK_LOGIN_NR_EMAIL",
                table: "TB_NEXTPARK_LOGIN",
                column: "NR_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_NEXTPARK_MANUTENCAO_ID_MOTO",
                table: "TB_NEXTPARK_MANUTENCAO",
                column: "ID_MOTO");

            migrationBuilder.CreateIndex(
                name: "IX_TB_NEXTPARK_USUARIO_NR_EMAIL",
                table: "TB_NEXTPARK_USUARIO",
                column: "NR_EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_LOGIN");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_MANUTENCAO");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_VAGA");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_USUARIO");

            migrationBuilder.DropTable(
                name: "TB_NEXTPARK_MOTO");
        }
    }
}
