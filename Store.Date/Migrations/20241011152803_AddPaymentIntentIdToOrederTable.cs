﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Date.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentIntentIdToOrederTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentIntentId",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentIntentId",
                table: "Order");
        }
    }
}