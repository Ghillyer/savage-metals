using FluentMigrator;

namespace SavageMetals.Api.Migrations;

[Migration(202611131956)]
public class SeedDemoUsers : Migration
{
    private const int PurchaserRole = 0;
    private const int AdminRole = 1;

    private const string AdminFirebaseUid = "QQS5KYdSLDYzXQ2Lhkp4AaSJB2k1";
    private const string PurchaserFirebaseUid = "gc6ViJ35mpSKwJYMI8XEPvVrfWv2";

    public override void Up()
    {
        Insert.IntoTable("Users")
            .Row(new
            {
                FirebaseUid = AdminFirebaseUid,
                Email = "admin@example.com",
                DisplayName = "Demo Admin",
                Role = AdminRole,
                IsActive = true
            })
            .Row(new
            {
                FirebaseUid = PurchaserFirebaseUid,
                Email = "purchaser@example.com",
                DisplayName = "Demo Purchaser",
                Role = PurchaserRole,
                IsActive = true
            });
    }

    public override void Down()
    {
        Delete.FromTable("Users")
            .Row(new { FirebaseUid = AdminFirebaseUid })
            .Row(new { FirebaseUid = PurchaserFirebaseUid });
    }
}