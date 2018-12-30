using Draughts.Access.Models;

namespace Draughts.Access.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Draughts.Access.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
        }
    }
}