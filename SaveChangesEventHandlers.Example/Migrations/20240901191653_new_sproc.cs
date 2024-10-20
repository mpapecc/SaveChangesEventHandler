using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveChangesEventHandlers.Example.Migrations
{
    public partial class new_sproc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE DeleteContact
                    @Id uniqueidentifier
                AS
                BEGIN
                    delete Contacts where Id = @Id
                END;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
