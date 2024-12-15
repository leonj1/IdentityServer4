using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServer.Data.Migrations.IdentityServer.PersistedGrantDb
{
    public partial class InitialIdentityServerPersistedGrantDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This method is now empty as the logic has been moved to a separate file.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The Down method remains unchanged.
            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "PersistedGrants");
        }
    }
}
