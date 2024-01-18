using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanHang.Migrations.SqliteMigrations
{
    public partial class Orders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailRom_Color",
                table: "DetailColor");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailRom_Product",
                table: "DetailColor");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.CreateTable(
                name: "InfoOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentMethod = table.Column<bool>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    District = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Total = table.Column<float>(type: "REAL", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Price = table.Column<double>(type: "REAL", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: true),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: false),
                    InfoOrderId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK__OrderItem__Info",
                        column: x => x.InfoOrderId,
                        principalTable: "InfoOrder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_InfoOrderId",
                table: "OrderItem",
                column: "InfoOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailColor_Color",
                table: "DetailColor",
                column: "IdColor",
                principalTable: "Color",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailColor_Product",
                table: "DetailColor",
                column: "IdProduct",
                principalTable: "Product",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailColor_Color",
                table: "DetailColor");

            migrationBuilder.DropForeignKey(
                name: "FK_DetailColor_Product",
                table: "DetailColor");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "InfoOrder");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", precision: 1, nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAmount = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ProductPrice = table.Column<double>(type: "REAL", nullable: true),
                    Qty = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderId",
                table: "OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailRom_Color",
                table: "DetailColor",
                column: "IdColor",
                principalTable: "Color",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailRom_Product",
                table: "DetailColor",
                column: "IdProduct",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
