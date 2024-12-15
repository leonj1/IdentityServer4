using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlServer.Migrations.PersistedGrantDb
{
    public partial class Grants : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "PersistedGrants");
        }
    }
}
