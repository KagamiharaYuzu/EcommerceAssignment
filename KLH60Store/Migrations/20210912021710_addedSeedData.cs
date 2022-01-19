using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KLH60Store.Migrations
{
    public partial class addedSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CartItem",
                columns: new[] { "CartItemId", "CartId", "Price", "ProductId", "Quantity", "ShoppingCartCartId" },
                values: new object[,]
                {
                    { 1, 1, 105.35m, 7, 1, null },
                    { 2, 1, 17.97m, 13, 3, null },
                    { 3, 1, 89.99m, 22, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "CustomerId", "CreditCard", "Email", "FirstName", "LastName", "PhoneNumber", "Province" },
                values: new object[,]
                {
                    { 1, "5299014012611141", "an@email.com", "Goose", "Man", "8197734608", "QC" },
                    { 2, "5384861194865788", "dat@boi.com", "Dat", "Boi", "8193034847", "ON" },
                    { 3, "5130403899507394", "kk@gmail.com", "Kana", "Kamiko", "8196734682", "QC" }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "CustomerId", "DateCreated", "DateFulfiled", "OrderCustId", "Taxes", "Total" },
                values: new object[] { 1, null, new DateTime(2020, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 28.5m, 218.5m });

            migrationBuilder.InsertData(
                table: "OrderItem",
                columns: new[] { "OrderItemId", "OrderId", "Price", "ProductId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 85.25m, 3, 1 },
                    { 2, 1, 67.85m, 6, 1 },
                    { 3, 1, 77.99m, 19, 1 }
                });

            migrationBuilder.InsertData(
                table: "ShoppingCart",
                columns: new[] { "CartId", "CartCustId", "DateCreated" },
                values: new object[] { 1, 2, new DateTime(2019, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "CartItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "CartItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CartItem",
                keyColumn: "CartItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "CustomerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "CustomerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderItem",
                keyColumn: "OrderItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItem",
                keyColumn: "OrderItemId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItem",
                keyColumn: "OrderItemId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ShoppingCart",
                keyColumn: "CartId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "CustomerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1);
        }
    }
}
