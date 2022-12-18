using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyBudget.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class BudgetBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Budgets",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Budgets");
        }
    }
}
