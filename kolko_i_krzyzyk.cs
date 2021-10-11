using System;
using System.Collections.Generic;
using System.Linq;

namespace kolko_i_krzyzyk
{
	class Program
	{
		static void Main(string[] args)
		{
			var w_grze = true;

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("-----------------------");
			Console.WriteLine("No siema!");
			Console.WriteLine("-----------------------\n");
			Console.ResetColor();

			while (w_grze)
			{
				Console.WriteLine("Co robimy?");
				Console.WriteLine("1. Gramy!!");
				Console.WriteLine("2. Spierdalaj\n");

				Console.Write("Poproszę o numerek: ");

				var choice = akcja_gracza("[12]");

				switch (choice)
				{
					case "1":
						Grajmy();
						Console.Clear();
						break;
					case "2":
						w_grze = false;
						break;
				}
			}
		}

		private static string akcja_gracza(string szablon = null)
		{
			var input = Console.ReadLine();
			input = input.Trim();

			if (szablon != null && !System.Text.RegularExpressions.Regex.IsMatch(input, szablon))
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("\"" + input + "\" is not valid.\n");
				Console.ResetColor();
				return null;
			}

			return input;
		}

		private static void Grajmy()
		{
			string wybor_liczby_wierszy = null;
			while (wybor_liczby_wierszy == null)
			{
				Console.Write("Na ile wierszy gramy? (3, 4, 5) ");
				wybor_liczby_wierszy = akcja_gracza("[345]");
			}
			var rozmiar_planszy = (int)Math.Pow(int.Parse(wybor_liczby_wierszy), 2);
			var plansza = new string[rozmiar_planszy];

			var turn = "X";
			while (true)
			{
				Console.Clear();

				var gracz_win = Kto_wygral(plansza);
				if (gracz_win != null)
				{
					pokaz_wynik(gracz_win[0] + " to miszcz tej gry!", plansza);
					break;
				}
				if (czy_pelna_plansza(plansza))
				{
					pokaz_wynik("Remis...", plansza);
					break;
				}

				Console.WriteLine("Postaw swoj(e) " + turn + ":");

				rysuj_plansze(plansza);

				Console.WriteLine("\n steruj strzalkami a potem <enter>");

				var LokalizacjaXO = Lokalizacja_XO(plansza);
				plansza[LokalizacjaXO] = turn;

				turn = turn == "X" ? "O" : "X";
			}
		}

		private static void pokaz_wynik(string wiadomosc, string[] plansza)
		{
			Console.WriteLine();
			rysuj_plansze(plansza);

			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine(wiadomosc);
			Console.ResetColor();

			Console.Write("\nCiskaj cokolwiek...");
			Console.CursorVisible = false;
			Console.ReadKey();
			Console.CursorVisible = true;
		}

		private static int Lokalizacja_XO(string[] plansza)
		{
			int liczba_wierszy = (int)Math.Sqrt(plansza.Length);

			int wiersz = 0, kolumna = 0;

			for (int i = 0; i < plansza.Length; i++)
			{
				if (plansza[i] == null)
				{
					wiersz = i / liczba_wierszy;
					kolumna = i % liczba_wierszy;
					break;
				}
			}

			while (true)
			{
				Console.SetCursorPosition(kolumna * 4 + 2, wiersz * 4 + 3);
				var strzalka = Console.ReadKey();
				Console.SetCursorPosition(kolumna * 4 + 2, wiersz * 4 + 3);
				Console.Write(plansza[wiersz * liczba_wierszy + kolumna] ?? " ");

				switch (strzalka.Key)
				{
					case ConsoleKey.LeftArrow:
						if (kolumna > 0)
							kolumna--;
						break;
					case ConsoleKey.RightArrow:
						if (kolumna + 1 < liczba_wierszy)
							kolumna++;
						break;
					case ConsoleKey.UpArrow:
						if (wiersz > 0)
							wiersz--;
						break;
					case ConsoleKey.DownArrow:
						if (wiersz + 1 < liczba_wierszy)
							wiersz++;
						break;
					case ConsoleKey.Spacebar:
					case ConsoleKey.Enter:
						if (plansza[wiersz * liczba_wierszy + kolumna] == null)
							return wiersz * liczba_wierszy + kolumna;
						break;
				}
			}
		}

		private static void rysuj_plansze(string[] plansza)
		{
			var liczba_wierszy = (int)Math.Sqrt(plansza.Length);

			Console.WriteLine();

			for (int row = 0; row < liczba_wierszy; row++)
			{
				if (row != 0)
					Console.WriteLine(" " + string.Join("|", Enumerable.Repeat("---", liczba_wierszy)));

				Console.Write(" " + string.Join("|", Enumerable.Repeat("   ", liczba_wierszy)) + "\n ");

				for (int col = 0; col < liczba_wierszy; col++)
				{
					if (col != 0)
						Console.Write("|");
					var space = plansza[row * liczba_wierszy + col] ?? " ";
					if (space.Length > 1)
						Console.ForegroundColor = ConsoleColor.DarkGreen;
					Console.Write(" " + space[0] + " ");
					Console.ResetColor();
				}

				Console.WriteLine("\n " + string.Join("|", Enumerable.Repeat("   ", liczba_wierszy)));
			}

			Console.WriteLine();
		}

		private static bool czy_pelna_plansza(IEnumerable<string> plansza)
		{
			return plansza.All(space => space != null);
		}

		private static string Kto_wygral(string[] plansza)
		{
			var liczba_wierszy = (int)Math.Sqrt(plansza.Length);

			// Check rows
			for (int row = 0; row < liczba_wierszy; row++)
			{
				if (plansza[row * liczba_wierszy] != null)
				{
					bool trzy_w_rzedzie = true;
					for (int col = 1; col < liczba_wierszy && trzy_w_rzedzie; col++)
					{
						if (plansza[row * liczba_wierszy + col] != plansza[row * liczba_wierszy])
							trzy_w_rzedzie = false;
					}
					if (trzy_w_rzedzie)
					{
						// Put an indicator in the plansza to know which ones are part of the tic tac toe
						for (int col = 0; col < liczba_wierszy; col++)
							plansza[row * liczba_wierszy + col] += "!";
						return plansza[row * liczba_wierszy];
					}
				}
			}

			// Check columns
			for (int col = 0; col < liczba_wierszy; col++)
			{
				if (plansza[col] != null)
				{
					bool trzy_w_rzedzie = true;
					for (int row = 1; row < liczba_wierszy && trzy_w_rzedzie; row++)
					{
						if (plansza[row * liczba_wierszy + col] != plansza[col])
							trzy_w_rzedzie = false;
					}
					if (trzy_w_rzedzie)
					{
						// Put an indicator in the plansza to know which ones are part of the tic tac toe
						for (int row = 0; row < liczba_wierszy; row++)
							plansza[row * liczba_wierszy + col] += "!";
						return plansza[col];
					}
				}
			}

			// Check top left -> bottom right diagonal
			if (plansza[0] != null)
			{
				bool trzy_w_rzedzie = true;
				for (int row = 1; row < liczba_wierszy && trzy_w_rzedzie; row++)
				{
					if (plansza[row * liczba_wierszy + row] != plansza[0])
						trzy_w_rzedzie = false;
				}
				if (trzy_w_rzedzie)
				{
					// Put an indicator in the plansza to know which ones are part of the tic tac toe
					for (int row = 0; row < liczba_wierszy; row++)
						plansza[row * liczba_wierszy + row] += "!";
					return plansza[0];
				}
			}

			// Check top right -> bottom left diagonal
			if (plansza[liczba_wierszy - 1] != null)
			{
				bool trzy_w_rzedzie = true;
				for (int row = 1; row < liczba_wierszy && trzy_w_rzedzie; row++)
				{
					if (plansza[row * liczba_wierszy + (liczba_wierszy - 1 - row)] != plansza[liczba_wierszy - 1])
						trzy_w_rzedzie = false;
				}
				if (trzy_w_rzedzie)
				{
					// Put an indicator in the plansza to know which ones are part of the tic tac toe
					for (int row = 0; row < liczba_wierszy; row++)
						plansza[row * liczba_wierszy + (liczba_wierszy - 1 - row)] += "!";
					return plansza[liczba_wierszy - 1];
				}
			}

			return null;
		}
	}
}