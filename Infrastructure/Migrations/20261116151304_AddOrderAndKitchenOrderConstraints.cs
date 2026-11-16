using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndKitchenOrderConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_KitchenOrder_Status",
                table: "KitchenOrders",
                sql: "[Status] IN ('Pending', 'Sent', 'InPreparation', 'Ready', 'Delivered', 'Cancelled')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_KitchenOrder_Status",
                table: "KitchenOrders");
        }
    }
}
