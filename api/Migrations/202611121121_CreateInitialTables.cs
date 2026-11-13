using FluentMigrator;

namespace SavageMetals.Api.Migrations;

[Migration(202611121121)]
public class CreateInitialTables : Migration
{
    public override void Up()
    {
        Create.Table("Products")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("PurchasePrice").AsDecimal(18, 4).Nullable()
            .WithColumn("SalesPrice").AsDecimal(18, 4).Nullable()
            .WithColumn("IsScrap").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("Vendors")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Type").AsInt32().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("Locations")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(200).NotNullable().Unique()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("FirebaseUid").AsString(200).NotNullable().Unique()
            .WithColumn("Email").AsString(200).NotNullable().Unique()
            .WithColumn("DisplayName").AsString(200).NotNullable()
            .WithColumn("Role").AsInt32().NotNullable()
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);

        Create.Table("PurchaseOrders")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("PoNumber").AsString(200).NotNullable().Unique()
            .WithColumn("VendorId").AsInt32().NotNullable().Indexed().ForeignKey("FK_PurchaseOrders_Vendors", "Vendors", "Id")
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable()
            .WithColumn("ReceivedAtUtc").AsDateTime2().Nullable()
            .WithColumn("CreatedByUserId").AsInt32().NotNullable().ForeignKey("FK_PurchaseOrders_Users", "Users", "Id");

        Create.Table("PurchaseOrderLines")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("PurchaseOrderId").AsInt32().NotNullable().Indexed().ForeignKey("FK_PurchaseOrderLines_PurchaseOrders", "PurchaseOrders", "Id")
            .WithColumn("ProductId").AsInt32().NotNullable().ForeignKey("FK_PurchaseOrderLines_Products", "Products", "Id")
            .WithColumn("Grade").AsInt32().NotNullable()
            .WithColumn("Weight").AsDecimal(18, 4).NotNullable()
            .WithColumn("UnitPrice").AsDecimal(18, 4).NotNullable();

        Create.Table("InventoryItems")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("TagNumber").AsString(200).NotNullable().Unique()
            .WithColumn("ProductId").AsInt32().NotNullable().Indexed().ForeignKey("FK_InventoryItems_Products", "Products", "Id")
            .WithColumn("LocationId").AsInt32().NotNullable().Indexed().ForeignKey("FK_InventoryItems_Locations", "Locations", "Id")
            .WithColumn("Weight").AsDecimal(18, 4).NotNullable()
            .WithColumn("TotalCost").AsDecimal(18, 4).NotNullable()
            .WithColumn("Status").AsInt32().NotNullable()
            .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable()
            .WithColumn("PurchaseOrderLineId").AsInt32().Nullable().ForeignKey("FK_InventoryItems_PurchaseOrderLines", "PurchaseOrderLines", "Id")
            .WithColumn("SourceWorkOrderId").AsInt32().Nullable();

        Create.Table("WorkOrders")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("WorkOrderNumber").AsString(200).NotNullable().Unique()
            .WithColumn("InputItemId").AsInt32().NotNullable().Unique().ForeignKey("FK_WorkOrders_InventoryItems", "InventoryItems", "Id")
            .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable()
            .WithColumn("CreatedByUserId").AsInt32().NotNullable().ForeignKey("FK_WorkOrders_Users", "Users", "Id");

        Create.ForeignKey("FK_InventoryItems_WorkOrders")
            .FromTable("InventoryItems").ForeignColumn("SourceWorkOrderId")
            .ToTable("WorkOrders").PrimaryColumn("Id");

        Execute.Sql("ALTER TABLE InventoryItems ADD CONSTRAINT CK_InventoryItems_SingleSource CHECK ((PurchaseOrderLineId IS NOT NULL AND SourceWorkOrderId IS NULL) OR (PurchaseOrderLineId IS NULL AND SourceWorkOrderId IS NOT NULL))");

        Create.Table("SalesOrders")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("SoNumber").AsString(200).NotNullable().Unique()
            .WithColumn("CustomerId").AsInt32().NotNullable().Indexed().ForeignKey("FK_SalesOrders_Vendors", "Vendors", "Id")
            .WithColumn("ContainerNumber").AsString(200).NotNullable()
            .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable()
            .WithColumn("CreatedByUserId").AsInt32().NotNullable().ForeignKey("FK_SalesOrders_Users", "Users", "Id");

        Create.Table("SalesOrderLines")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("SalesOrderId").AsInt32().NotNullable().Indexed().ForeignKey("FK_SalesOrderLines_SalesOrders", "SalesOrders", "Id")
            .WithColumn("InventoryItemId").AsInt32().NotNullable().Unique().ForeignKey("FK_SalesOrderLines_InventoryItems", "InventoryItems", "Id")
            .WithColumn("UnitPrice").AsDecimal(18, 4).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("SalesOrderLines");
        Delete.Table("SalesOrders");
        Delete.ForeignKey("FK_InventoryItems_WorkOrders").OnTable("InventoryItems");
        Delete.Table("WorkOrders");
        Delete.Table("InventoryItems");
        Delete.Table("PurchaseOrderLines");
        Delete.Table("PurchaseOrders");
        Delete.Table("Users");
        Delete.Table("Locations");
        Delete.Table("Vendors");
        Delete.Table("Products");
    }
}