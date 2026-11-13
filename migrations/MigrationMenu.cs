using FluentMigrator.Runner;

namespace SavageMetals.Migrations;

public class MigrationMenu(IMigrationRunner migrationRunner)
{
    public void Run()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Savage Metals Migrations ===");
            Console.WriteLine("1. Migrate up");
            Console.WriteLine("2. Migrate down");
            Console.WriteLine("0. Exit");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine()?.Trim();

            if (choice == "0")
            {
                return;
            }

            Execute(choice);
        }
    }

    public void Execute(string? choice)
    {
        try
        {
            switch (choice)
            {
                case "1":
                    migrationRunner.MigrateUp();
                    break;
                case "2":
                    migrationRunner.Rollback(1);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
        }
    }
}