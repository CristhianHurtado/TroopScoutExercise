using Microsoft.EntityFrameworkCore.Migrations;

namespace Test2.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetScoutTimestampOnUpdate
                    AFTER UPDATE ON Scouts
                    BEGIN
                        UPDATE Scouts
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetScoutTimestampOnInsert
                    AFTER INSERT ON scouts
                    BEGIN
                        UPDATE Scouts
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE VIEW AchievementSummaryVM as
                    Select p.ID, p.FirstName, p.MiddleName, p.LastName, Count(*) as NumberOfPerformances, Count(a.ID) as TotalPerformances,
                        Sum(a.FeePaid) as TotalExtraFees, Max(a.FeePaid) as MaximumFeeCharged, Min(a.FeePaid) as MinimumFeeCharged,
                        Avg(a.Feepaid) as AverageFeeCharged 
                    From Scouts p Join Achivements a
                    on p.ID = a.MusicianID
                    Group by p.ID, p.FirstName, p.MiddleName, p.LastName 	
                ");
        }
    }
}
