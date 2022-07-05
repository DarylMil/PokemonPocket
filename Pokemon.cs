// Daryl Lim Yong Rui 213321J

using System;
using System.Collections.Generic;

namespace PokemonPocket
{
	public class PokemonMaster
	{
		public string Name { get; set; }
		public int NoToEvolve { get; set; }
		public string EvolveTo { get; set; }

		public PokemonMaster(string name, int noToEvolve, string evolveTo)
		{
			this.Name = name;
			this.NoToEvolve = noToEvolve;
			this.EvolveTo = evolveTo;
		}
	}
	public abstract class Pokemon
	{
		public int Id { get; private set; }

		public string Name { get;protected set; }
		public int BaseHp{get; set;}
		public int Hp	{ get; set; }
		public int Exp { get; set; }
		public string Skill	{ get; protected set; }

		public int LAttack { get; protected set; }
		public int HAttack { get; protected set; }

		protected Pokemon(string name, string skill)
		{
			this.Name = name;
			this.Skill = skill;
		}

		public List<Pokemon> StartBattle(List<Type> allPokemonTypes)
		{
			Random rdn = new Random();
			// Find a random index 
			var theRandomIndex = rdn.Next(1, allPokemonTypes.Count);

			var selectedPokeType = allPokemonTypes[theRandomIndex];
			// activator is a class in .NET which can create new objects with the help of a type
			var compPokemon = (Pokemon)Activator.CreateInstance(selectedPokeType);

			Console.WriteLine($"Starting to battle {compPokemon.Name} | "
			+ $"Hp: {compPokemon.Hp}/{compPokemon.BaseHp} | Attack: {(compPokemon.LAttack + compPokemon.HAttack) / 2}");
			return this.Attack(compPokemon);
		}
		private List<Pokemon> Attack(Pokemon compPokemon)
		{
			bool foughtTillEnd = false;
			List<Pokemon> pokemonToReturn = new List<Pokemon>(){this};

			while(this.Hp > 0 && compPokemon.Hp > 0)
			{
				int move = 1;
				foughtTillEnd = false;
				Console.Write($"{MyMethods.Dividers('-',30)}\n1.Flee from battle \n2.Use {this.Skill}\nWhat will you do?: ");
				string option = Console.ReadLine();
				Random rdn = new Random();

				if (option == "1")
				{
					move -= 1;
					if(rdn.Next(0,2) == 0)
					{
						Console.WriteLine("Successfully flee from battle.");
						break;
					}
					else
					{
						Console.WriteLine("Fail to flee from battle.");
					}
				}
				else if(option =="2")
				{
					foughtTillEnd = true;
					if (move==1)
					{
						int damage = rdn.Next(this.LAttack,this.HAttack);
						int balanceCompPokemonHp = compPokemon.Hp - damage;
						compPokemon.Hp = Math.Max(balanceCompPokemonHp,0);
						Console.WriteLine($"{this.Name} attacks enemy {compPokemon.Name} with {this.Skill} for {damage} damage."
						+ $" Enemy {compPokemon.Name} | Hp: {compPokemon.Hp}/{compPokemon.BaseHp}");
					}

					int compDamage = rdn.Next(compPokemon.LAttack,compPokemon.HAttack);
					int balanceHp = this.Hp - compDamage;
					this.Hp = Math.Max(balanceHp, 0);
					Console.WriteLine($"Enemy {compPokemon.Name} attacks {this.Name} with {compPokemon.Skill} for {compDamage} damage."
					+ $" {this.Name} | Hp: {this.Hp}/{this.BaseHp}");
				}
				else{
					Console.WriteLine("Invalid Input. Try Again.");
				}
			}
			// Print battle results only if battle lasted till the end.

			if(foughtTillEnd){
				string battleResults = "";
				if(this.Hp > compPokemon.Hp)
				{
					battleResults = $"You win the battle!. You gain the enemy {compPokemon.Name}";
					pokemonToReturn.Add(compPokemon);
				}
				else if (this.Hp < compPokemon.Hp)
				{
					battleResults = "You lost the battle!.";
				}
				else 
				{
					battleResults = "Battle was a draw!";
				}
				Console.WriteLine(battleResults);
			}
			// return your pokemon stats after battle for saving to db
			return pokemonToReturn;
		}
	}

	public class Pikachu : Pokemon
	{
		public Pikachu() : base("Pikachu", "Lightning Bolt")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(10, 15);
			this.HAttack = rdn.Next(15, 20);
			this.Hp = rdn.Next(60,90);
			this.BaseHp = this.Hp;
		}
	}
	public class Eevee : Pokemon
	{
		public Eevee() : base("Eevee", "Run Away")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(3, 18);
			this.HAttack = rdn.Next(18, 27);
			this.Hp = rdn.Next(70,85);
			this.BaseHp = this.Hp;
		}
	}
	public class Charmander : Pokemon
	{
		public Charmander() : base("Charmander", "Solar Power")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(8, 17);
			this.HAttack = rdn.Next(17, 25);
			this.Hp = rdn.Next(50,100);
			this.BaseHp = this.Hp;
		}
	}
	public class Raichu : Pokemon
	{
		public Raichu() : base("Raichu", "Thunder")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(18, 25);
			this.HAttack = rdn.Next(25, 30);
			this.Hp = rdn.Next(60, 140);
			this.BaseHp = this.Hp;
		}
	}

	public class Charmeleon : Pokemon
	{

		public Charmeleon() : base("Charmeleon", "Fire Blast")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(20, 27);
			this.HAttack = rdn.Next(27, 33);
			this.Hp = rdn.Next(70, 130);
			this.BaseHp = this.Hp;
		}
	}
	public class Flareon : Pokemon
	{

		public Flareon() : base("Flareon", "Flame Thrower")
		{
			Random rdn = new Random();
			this.LAttack = rdn.Next(15, 23);
			this.HAttack = rdn.Next(23, 34);
			this.Hp = rdn.Next(60, 120);
			this.BaseHp = this.Hp;
		}
	}
	public class ViewModel
	{
		public string Name;
		public int PokeCount;
	} 
}

