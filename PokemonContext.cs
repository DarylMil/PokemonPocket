// Daryl Lim Yong Rui 213321J

using Microsoft.EntityFrameworkCore;

namespace PokemonPocket {
	public class PokemonContext:DbContext
	{
		public DbSet<Pokemon> Pokemons {get;set;}

		 protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            optionsBuilder.UseSqlite("Data Source=Pokemon.db");
        }

			protected override void OnModelCreating(ModelBuilder builder)
			{	
					builder.Entity<Pikachu>().HasDiscriminator(m=>m.Name).HasValue("Pikachu");
					builder.Entity<Eevee>().HasDiscriminator(m=>m.Name).HasValue("Eevee");
					builder.Entity<Charmander>().HasDiscriminator(m=>m.Name).HasValue("Charmander");
					builder.Entity<Raichu>().HasDiscriminator(m=>m.Name).HasValue("Raichu");
					builder.Entity<Flareon>().HasDiscriminator(m=>m.Name).HasValue("Flareon");
					builder.Entity<Charmeleon>().HasDiscriminator(m=>m.Name).HasValue("Charmeleon");

					base.OnModelCreating(builder);
			}
	}
}