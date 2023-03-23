using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Test2.Models;

namespace Test2.Data
{
    public static class ScoutTroopInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            ScoutTroopContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<ScoutTroopContext>();

            try
            {
                //Delete the database
                //context.Database.EnsureDeleted();
                //Create/Refresh the database
                context.Database.Migrate();

                if (!context.Troops.Any())
                {
                    context.Troops.AddRange(
                     new Troop
                     {
                         TroopName = "Welland Wambats",
                         TroopNumber = "W515",
                         TroopBudget = 1500.00d
                     },
                     new Troop
                     {
                         TroopName = "Port Paddlers",
                         TroopNumber = "P517",
                         TroopBudget = 2500.00d
                     },
                    new Troop
                    {
                        TroopName = "St. Kits Kittens",
                        TroopNumber = "C510",
                        TroopBudget = 2100.00d
                    });
                    context.SaveChanges();
                }
                if (!context.Scouts.Any())
                {
                    context.Scouts.AddRange(
                    new Scout
                    {
                        FirstName = "Fred",
                        LastName = "Flintstone",
                        FeePaid = 15.00d,
                        DOB = DateTime.Parse("2008-09-01"),
                        TroopID = context.Troops.FirstOrDefault(d => d.TroopNumber == "C510").ID
                    },
                    new Scout
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        FeePaid = 15.00d,
                        DOB = DateTime.Parse("2005-01-01"),
                        TroopID = context.Troops.FirstOrDefault(d => d.TroopNumber == "C510").ID
                    },
                    new Scout
                    {
                        FirstName = "Wilma",
                        LastName = "Flintstone",
                        FeePaid = 15.00d,
                        DOB = DateTime.Parse("2014-04-23"),
                        TroopID = context.Troops.FirstOrDefault(d => d.TroopNumber == "P517").ID
                    },
                    new Scout
                    {
                        FirstName = "Barney",
                        LastName = "Rubble",
                        FeePaid = 25.00d,
                        DOB = DateTime.Parse("2014-02-22"),
                        TroopID = context.Troops.FirstOrDefault(d => d.TroopNumber == "W515").ID
                    });
                    context.SaveChanges();
                    //To randomly generate data.  Using a seed value so we get the same data each time.
                    Random random = new(0);

                    //Create a collection of the primary keys
                    int[] scoutIDs = context.Scouts.Select(a => a.ID).ToArray();
                    int[] stats = { 555, 26, 104, 92, 2 };//for HL, BV, LS, CE, EO

                    if (!context.Achievements.Any())
                    {
                        //Create 5 seasons
                        for (int i = 2018; i < 2023; i++)
                        {
                            foreach (int s in scoutIDs)
                            {
                                Achievement a = new Achievement
                                {
                                    Year = i,
                                    HL = stats[0],
                                    BV = stats[1],
                                    LS = stats[2],
                                    CE = stats[3],
                                    EO = stats[4],
                                    ScoutID = s
                                };
                                context.Achievements.Add(a);
                                //new stats
                                stats[0] = random.Next(200, 1000);
                                stats[1] = random.Next(stats[0] / 4);
                                stats[2] = random.Next(stats[1] + 10, stats[0]);
                                stats[3] = random.Next(stats[2]);
                                stats[4] = random.Next(50);
                            }
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
