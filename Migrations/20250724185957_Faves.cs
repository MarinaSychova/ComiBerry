using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComiBerry.Migrations
{
    /// <inheritdoc />
    public partial class Faves : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faves",
                columns: table => new
                {
                    FaveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SeriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faves", x => x.FaveId);
                    table.ForeignKey(
                        name: "FK_Faves_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Faves_Series_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "Series",
                        principalColumn: "SeriesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Faves_SeriesId",
                table: "Faves",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Faves_UserId",
                table: "Faves",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faves");
        }
    }
}
