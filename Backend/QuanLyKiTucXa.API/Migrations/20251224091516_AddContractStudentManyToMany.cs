using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKiTucXa.API.Migrations
{
    /// <inheritdoc />
    public partial class AddContractStudentManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Students_StudentId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_StudentId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Contracts");

            migrationBuilder.CreateTable(
                name: "ContractStudents",
                columns: table => new
                {
                    ContractId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractStudents", x => new { x.ContractId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ContractStudents_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractStudents_StudentId",
                table: "ContractStudents",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractStudents");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Contracts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StudentId",
                table: "Contracts",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Students_StudentId",
                table: "Contracts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
