using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HtmlConverter.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jobs",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    status = table.Column<int>(type: "INTEGER", nullable: true),
                    html_file_name = table.Column<string>(type: "TEXT", nullable: false),
                    html_contents = table.Column<string>(type: "TEXT", nullable: false),
                    pdf_contents = table.Column<byte[]>(type: "BLOB", nullable: true),
                    created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    finished = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs");
        }
    }
}
