using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAngular.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletedToTodoItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "TodoItems",
                newName: "Completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Completed",
                table: "TodoItems",
                newName: "IsCompleted");
        }
    }
}
