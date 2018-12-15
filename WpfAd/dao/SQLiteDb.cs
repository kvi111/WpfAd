using System.Data.Entity;
using WpfAd.model;

namespace WpfAd.dao
{
    public class SQLiteDb : DbContext
    {
        public SQLiteDb() : base("dbConn")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<SQLiteDb, Configuration>());
        }

        public DbSet<Ad> Ads { set; get; }
        public DbSet<Dm> Dms { set; get; }
        public DbSet<Category> Categories { set; get; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    modelBuilder.Configurations.AddFromAssembly(typeof(SQLiteDb).Assembly);
        //}
    }

    //public class Configuration : DbMigrationsConfiguration<SQLiteDb>
    //{
    //    public Configuration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //        AutomaticMigrationDataLossAllowed = true;
    //        SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
    //    }
    //    protected override void Seed(SQLiteDb context)
    //    {

    //    }
    //}
}
