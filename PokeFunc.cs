// Daryl Lim Yong Rui 213321J

using System;
using System.Collections.Generic;

namespace PokemonPocket
{
	public class MyMethods
	{

		public static string Dividers(char symbol, int n)
		{
			return new string(symbol, n);
		}
		public static void DisplayAll(List<Pokemon> list, bool isLongForm = false)
		{
			if (list.Count == 0)
			{
				Console.WriteLine("You do not own any Pokemons, try adding Pokemons.");
			}
			else
			{

				for (int i = 0; i < list.Count; i++)
				{
					if (isLongForm)
					{
						Console.WriteLine($"{Dividers('-', 30)} \nName: {list[i].Name}\nHP: {list[i].Hp}" +
						$"\nExp: {list[i].Exp}\nSkill: {list[i].Skill}\n{Dividers('-', 30)}");
					}
					else
					{
						Console.WriteLine($"{Dividers('-', 30)} \n{i + 1}. {list[i].Name} | Hp: {list[i].Hp}/{list[i].BaseHp} | Attack: {(list[i].LAttack + list[i].HAttack) / 2}");
					}
				}

			}
		}
	}
}