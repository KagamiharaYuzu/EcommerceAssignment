using Microsoft.EntityFrameworkCore.Migrations;

namespace KLH60Store.Migrations
{
    public partial class ModifiedForeignKeyToUseTheCorrectOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_ShoppingCart_ShoppingCartCartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CustomerId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_ShoppingCartCartId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShoppingCartCartId",
                table: "CartItem");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderCustId",
                table: "Order",
                column: "OrderCustId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_ShoppingCart_CartId",
                table: "CartItem",
                column: "CartId",
                principalTable: "ShoppingCart",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_OrderCustId",
                table: "Order",
                column: "OrderCustId",
                principalTable: "Customer",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_ShoppingCart_CartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer_OrderCustId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderCustId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartCartId",
                table: "CartItem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ShoppingCartCartId",
                table: "CartItem",
                column: "ShoppingCartCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_ShoppingCart_ShoppingCartCartId",
                table: "CartItem",
                column: "ShoppingCartCartId",
                principalTable: "ShoppingCart",
                principalColumn: "CartId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "Customer",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
