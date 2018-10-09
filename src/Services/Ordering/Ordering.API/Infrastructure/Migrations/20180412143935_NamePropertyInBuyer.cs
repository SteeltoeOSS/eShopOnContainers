using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class NamePropertyInBuyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_orders_OrderId",
                table: "orderItems");                       

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "buyers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_orders_OrderId",
                table: "orderItems",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_orders_OrderId",
                table: "orderItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "buyers");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "orders",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_orders_OrderId",
                table: "orderItems",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
