using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUserPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"),
                column: "PasswordHash",
                value: "$2a$11$bq03k3pdNQVI7wxW4ozA4u8HGCQKFLNA9DEYrzVecbmx.0gv40ShW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("8a5edca7-8f80-4ed6-8b91-01863782b51b"),
                column: "PasswordHash",
                value: "$2a$11$bq03k3pdNQVI7wxW4ozA4u8HGCQKFLNA9DEYrzVecbmx.0gv40ShW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"),
                column: "PasswordHash",
                value: "$2a$11$bq03k3pdNQVI7wxW4ozA4u8HGCQKFLNA9DEYrzVecbmx.0gv40ShW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"),
                column: "PasswordHash",
                value: "$2a$11$bq03k3pdNQVI7wxW4ozA4u8HGCQKFLNA9DEYrzVecbmx.0gv40ShW");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d"),
                column: "PasswordHash",
                value: "$2a$11$bq03k3pdNQVI7wxW4ozA4u8HGCQKFLNA9DEYrzVecbmx.0gv40ShW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"),
                column: "PasswordHash",
                value: "development-password-hash");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("8a5edca7-8f80-4ed6-8b91-01863782b51b"),
                column: "PasswordHash",
                value: "development-password-hash");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"),
                column: "PasswordHash",
                value: "development-password-hash");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"),
                column: "PasswordHash",
                value: "development-password-hash");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d"),
                column: "PasswordHash",
                value: "development-password-hash");
        }
    }
}
