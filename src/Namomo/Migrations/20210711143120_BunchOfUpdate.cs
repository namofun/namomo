using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SatelliteSite.Migrations
{
    [DbContext(typeof(DefaultContext))]
    [Migration("20210711143120_BunchOfUpdate")]
    public partial class BunchOfUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEligible",
                table: "TenantCategory",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ContestEvents",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);

            migrationBuilder.InsertData(
                table: "Configurations",
                columns: new[] { "Name", "Category", "Description", "DisplayPriority", "Public", "Type", "Value" },
                values: new object[] { "contest_last_rating_change_time", "Contest", "Last rating update time.", 0, false, "datetime", "null" });

            migrationBuilder.UpdateData(
                table: "TenantCategory",
                keyColumn: "Id",
                keyValue: -3,
                column: "IsEligible",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Configurations",
                keyColumn: "Name",
                keyValue: "contest_last_rating_change_time");

            migrationBuilder.DropColumn(
                name: "IsEligible",
                table: "TenantCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ContestEvents",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
