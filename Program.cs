// Daryl Lim Yong Rui 213321J

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PokemonPocket
{
	class Program
	{
		static void Main(string[] args)
		{
			//PokemonMaster list for checking pokemon evolution availability.    
			List<PokemonMaster> pokemonMasters = new List<PokemonMaster>(){
						new PokemonMaster("Pikachu", 2, "Raichu"),
						new PokemonMaster("Eevee", 3, "Flareon"),
						new PokemonMaster("Charmander", 1, "Charmeleon")
						};

			//Use "Environment.Exit(0);" if you want to implement an exit of the console program
			//Start your assignment 1 requirements below.

			var context = new PokemonContext();
			var runningAssembly = Assembly.GetExecutingAssembly();

			// all classes have a "Type" which exposes information about the class
			var pokemonType = typeof(Pokemon);

			// go through all types in our project and locate those who inherit our pokemon class
			List<Type> allPokemonTypes = runningAssembly.GetTypes().Where(t => !t.IsInterface && pokemonType.IsAssignableFrom(t)).ToList();

			string option = "";
			while (true)
			{
				Console.Write(
@$"{MyMethods.Dividers('*', 30)}
Welcome to Pokemon Pocket App
{MyMethods.Dividers('*', 30)}
(1). Add pokemon to my pocket
(2). List pokemon in my pocket
(3). Check if I can evolve pokemon
(4). Evolve pokemon
(5). Battle random pokemons to win pokemons
(6). Heal a pokemon
(7). Delete a pokemon
Please only enter [1,2,3,4,5,6,7] or Q to quit: ");
				option = Console.ReadLine().ToLower();
				if (option == "1")
				{
					int hp = 0;
					string pokeName = null;

					Console.Write("Enter Pokemon's Name: ");
					string name = Console.ReadLine().ToLower().Trim();

					PokemonMaster pokeFound = pokemonMasters.FirstOrDefault(x => x.Name.ToLower() == name);
					pokeName = pokeFound != null ? pokeFound.Name.ToLower() : null;

					if (pokeName != null)
					{
						while (true)
						{
							Console.Write("Enter Pokemon's HP: ");
							if (Int32.TryParse(Console.ReadLine(), out hp) && hp >= 0)
							{
								break;
							}
							else
							{
								Console.WriteLine("HP must be positive numbers. Try again.");
							}
						}
						while (true)
						{
							Console.Write("Enter Pokemon's Exp: ");
							if (Int32.TryParse(Console.ReadLine(), out int exp) && exp >= 0)
							{
								using (var transaction = context.Database.BeginTransaction())
								{
									try
									{
										Type pokeType = allPokemonTypes.First(t => t.Name.ToLower() == pokeName);
										Pokemon newPokemon = (Pokemon)Activator.CreateInstance(pokeType);
										newPokemon.Hp = hp;
										newPokemon.BaseHp = hp;
										newPokemon.Exp = exp;
										context.Add(newPokemon);
										context.SaveChanges();
										transaction.Commit();
										Console.WriteLine($"{newPokemon.Name} has been added.");
										break;
									}
									catch (Exception)
									{
										transaction.Rollback();
										Console.WriteLine("Error occured. Please try again.");
									}
								}

							}
							else
							{
								Console.WriteLine("Exp must be positive numbers . Try again.");
							}
						}
					}
					else
					{
						Console.WriteLine("Pokemon not found.");
					}
				}
				else if (option == "2")
				{
					List<Pokemon> pokemons = context.Pokemons.OrderBy(x => x.Hp).ToList();
					MyMethods.DisplayAll(pokemons, true);
				}
				else if (option == "3")
				{

					List<ViewModel> myPokemons = context.Pokemons.GroupBy(x => x.Name)
					.Select(x => new ViewModel { Name = x.Key, PokeCount = x.Count() }).ToList();

					int evolutionChecker = 0;
					foreach (var view in myPokemons)
					{
						PokemonMaster pokeMaster = pokemonMasters.Find(x => x.Name == view.Name);
						if (pokeMaster != null)
						{
							int numberOfPossibleEvol = view.PokeCount / pokeMaster.NoToEvolve;
							evolutionChecker += numberOfPossibleEvol;
							for (int i = numberOfPossibleEvol; i > 0; i--)
							{
								Console.WriteLine($"{view.Name} --> {pokeMaster.EvolveTo}");
							}
						}
					}
					if (evolutionChecker == 0)
					{
						Console.WriteLine("You do not have any Pokemons eligible for evolution.");
					}
				}
				else if (option == "4")
				{
					Console.WriteLine("Please note all eligible pokemons will be evolved simultaneously.");
					Console.Write("Press Y to continue and anything else to cancel: ");
					string cont = Console.ReadLine().ToLower();

					if (cont == "y")
					{
						List<ViewModel> myPokemons = context.Pokemons.GroupBy(x => x.Name)
						.Select(x => new ViewModel { Name = x.Key, PokeCount = x.Count() }).ToList();

						int evolutionChecker = 0;
						foreach (ViewModel view in myPokemons)
						{
							string pokemonToEvolve = view.Name;
							PokemonMaster pokeMaster = pokemonMasters.Find(x => x.Name == pokemonToEvolve);
							if (pokeMaster != null)
							{
								int numberOfPossibleEvol = view.PokeCount / pokeMaster.NoToEvolve;

								//Begin Transaction only if number of possible evol is not 0
								if (numberOfPossibleEvol == 0) continue;
								evolutionChecker = 1;
								using (var transaction = context.Database.BeginTransaction())
								{
									try
									{
										//Delete the existing Pokemon
										context.Pokemons.RemoveRange(context.Pokemons.Where(x => x.Name == pokemonToEvolve).Take(numberOfPossibleEvol * pokeMaster.NoToEvolve).ToList());

										// Create new pokemon
										List<Pokemon> evolvedPokemons = new List<Pokemon>();
										for (int i = numberOfPossibleEvol; i > 0; i--)
										{
											Type pokeType = allPokemonTypes.First(t => t.Name.ToLower() == pokeMaster.EvolveTo.ToLower());
											Pokemon evolvedPokemon = (Pokemon)Activator.CreateInstance(pokeType);
											evolvedPokemons.Add(evolvedPokemon);
											Console.WriteLine($"Evolving {pokemonToEvolve}.");
										}
										context.Pokemons.AddRange(evolvedPokemons);
										context.SaveChanges();
										transaction.Commit();
										Console.WriteLine($"Successfully evolved {pokemonToEvolve} to {pokeMaster.EvolveTo} X {numberOfPossibleEvol}.");
									}
									catch (Exception)
									{
										transaction.Rollback();
										Console.WriteLine("Error occured. Please try again.");
									}
								}
							}
						}
						if (evolutionChecker == 0)
						{
							Console.WriteLine("You do not have any Pokemons eligible for evolution.");
						}

					}

				}
				else if (option == "5")
				{
					// battle random pokemon to win that pokemon
					List<Pokemon> pokemons = context.Pokemons.OrderByDescending(x => (x.LAttack + x.HAttack) / 2).ToList();
					MyMethods.DisplayAll(pokemons);

					while (pokemons.Count > 0)
					{
						Console.Write($"{MyMethods.Dividers('-', 30)}\nChoose your pokemon to start battling or [-1] to cancel: ");
						if (Int32.TryParse(Console.ReadLine(), out int pokeChoice) && pokeChoice <= pokemons.Count && pokeChoice >= 1)
						{
							pokeChoice -= 1;
							Pokemon pokemonToBattle = pokemons[pokeChoice];
							if (pokemonToBattle.Hp > 0)
							{
								Console.WriteLine($"{MyMethods.Dividers('-', 30)}\nChosen {pokemonToBattle.Name} | Hp: {pokemonToBattle.Hp}/{pokemonToBattle.BaseHp} | Attack: {(pokemonToBattle.LAttack + pokemonToBattle.HAttack) / 2}");
								List<Pokemon> pokemonToSave = pokemonToBattle.StartBattle(allPokemonTypes);
								using (var transaction = context.Database.BeginTransaction())
								{
									try
									{
										// save your existing pokemon but save new for pokemon won
										foreach (var poke in pokemonToSave)
										{
											Pokemon foundPoke = context.Pokemons.FirstOrDefault(x => x.Id == poke.Id);
											if (foundPoke != null)
											{
												foundPoke.Hp = poke.Hp;
											}
											else
											{
												context.Add(poke);
											}
										}
										context.SaveChanges();
										transaction.Commit();
									}
									catch (Exception)
									{
										transaction.Rollback();
										Console.WriteLine("Error occured. Please try again.");
									}
								}
								break;
							}
							else
							{
								Console.WriteLine($"{pokemonToBattle.Name} has 0 hp and it's unable to fight. Choose another pokemon.");
							}
						}
						else if (pokeChoice == -1)
						{
							break;
						}
						else
						{
							Console.WriteLine("Invalid input. Try again.");
						}
					}
				}
				else if (option == "6")
				{
					// Heal your pokemon using potions.
					List<Pokemon> pokemons = context.Pokemons.OrderBy(x => x.Hp).ToList();
					MyMethods.DisplayAll(pokemons);
					while (pokemons.Count > 0)
					{
						Console.Write($"{MyMethods.Dividers('-', 30)}\nChoose your pokemon to heal it back to health or [-1] to cancel: ");
						if (Int32.TryParse(Console.ReadLine(), out int pokeChoice) && pokeChoice <= pokemons.Count && pokeChoice >= 1)
						{
							pokeChoice -= 1;
							Pokemon pokemonToHeal = pokemons[pokeChoice];
							if (pokemonToHeal.Hp != pokemonToHeal.BaseHp)
							{
								using (var transaction = context.Database.BeginTransaction())
								{
									try
									{
										pokemonToHeal.Hp = pokemonToHeal.BaseHp;
										context.SaveChanges();
										transaction.Commit();
										Console.WriteLine($"Successfully healed {pokemonToHeal.Name}.");
									}
									catch (Exception)
									{
										transaction.Rollback();
										Console.WriteLine("Error occured. Please try again.");
									}
								}
							}
							else
							{
								Console.WriteLine($"{pokemonToHeal.Name}'s Hp is already full.");
							}
							break;
						}
						else if (pokeChoice == -1)
						{
							break;
						}
						else
						{
							Console.WriteLine("Invalid input. Try again.");
						}
					}
				}
				else if (option == "7")
				{
					// Delete a pokemon
					List<Pokemon> pokemons = context.Pokemons.OrderBy(x => x.Hp).ToList();
					MyMethods.DisplayAll(pokemons);
					while (pokemons.Count > 0)
					{
						Console.Write($"{MyMethods.Dividers('-', 30)}\nChoose your pokemon to delete or [-1] to cancel: ");
						if (Int32.TryParse(Console.ReadLine(), out int pokeChoice) && pokeChoice <= pokemons.Count && pokeChoice >= 1)
						{
							pokeChoice -= 1;
							Pokemon pokemonToDelete = pokemons[pokeChoice];
							using (var transaction = context.Database.BeginTransaction())
							{
								try
								{
									context.Remove(pokemonToDelete);
									context.SaveChanges();
									transaction.Commit();
									Console.WriteLine($"Successfully deleted {pokemonToDelete.Name}");
								}
								catch (Exception)
								{
									transaction.Rollback();
									Console.WriteLine("Error occured. Please try again.");
								}
							}
							break;
						}
						else if (pokeChoice == -1)
						{
							break;
						}
						else
						{
							Console.WriteLine("Invalid input. Try again.");
						}
					}
				}
				else if (option == "q")
				{
					Environment.Exit(0);
				}
				else
				{
					Console.WriteLine("Invalid Input");
				}
			}
		}
	}
}
