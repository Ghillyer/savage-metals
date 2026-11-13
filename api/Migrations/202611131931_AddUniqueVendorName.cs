using FluentMigrator;

namespace SavageMetals.Api.Migrations;

[Migration(202611131931)]
public class AddUniqueVendorName : Migration
{
    public override void Up()
    {
        Create.Index("IX_Vendors_Name")
            .OnTable("Vendors")
            .OnColumn("Name").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Index("IX_Vendors_Name").OnTable("Vendors");
    }
}