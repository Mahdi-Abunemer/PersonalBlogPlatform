using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalBlogPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnsureNonNullNavCollectionsInPostAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("58abd188-a359-4443-9644-a926b699b305"),
                column: "ConcurrencyStamp",
                value: "05688aaa-cca6-46ec-a431-0688d284591b");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("58abd188-a359-4443-9644-a926b699b305"),
                column: "ConcurrencyStamp",
                value: "7b4ab232-ed6f-428e-a495-1350e7a66aee");
        }
    }
}
