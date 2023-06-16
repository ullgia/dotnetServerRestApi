using Microsoft.EntityFrameworkCore;

namespace ServerRest.DataBase;

/*
 * Per aggiungere nuove migrazioni basta fare tasto destro sul progetto, apri console e scrivere:
 * dotnet ef migrations add <nome migrazione>
 * le migrazioni verranno salvate in Migrations in funzione al tipo di database definito sotto
 */
public class AppDbContext: DbContext
{
    public AppDbContext():base(new DbContextOptions<AppDbContext>())
    {
        
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }




    public DbSet<UserTable> UserTable { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite("Data Source=DataBase.db");
    }
}

