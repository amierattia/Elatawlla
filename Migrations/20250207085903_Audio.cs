using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWork.Migrations
{
    /// <inheritdoc />
    public partial class Audio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioPath",
                table: "LessonParagraphs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioPath",
                table: "LessonParagraphs");
        }
    }
}
