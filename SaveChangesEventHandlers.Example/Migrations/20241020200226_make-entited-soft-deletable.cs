using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveChangesEventHandlers.Example.Migrations
{
    public partial class makeentitedsoftdeletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "Emails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSoftDeleted",
                table: "Contacts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "IsSoftDeleted",
                table: "Contacts");
        }
    }
}
