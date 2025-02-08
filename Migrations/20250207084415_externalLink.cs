using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWork.Migrations
{
    /// <inheritdoc />
    public partial class externalLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLink",
                table: "LessonParagraphs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLink",
                table: "LessonParagraphs");
        }
    }
}
