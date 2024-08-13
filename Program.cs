namespace ShoppingList;

class Program
{
	// initialize stock information
	static readonly Dictionary<string, double> stock = new(StringComparer.OrdinalIgnoreCase)
	{
		{"Apple", 1.00}, {"Banana", 1.50}, {"Candy", 3.00}, {"Chips", 6.35}, {"Soda", 2.00},
		{"Cereal", 3.99}, {"Pizza", 4.65}, {"Cheese", 6.66}, {"Milk", 4.49}, {"Hotdogs", 12.99}
	};

	static void Main(string[] args)
	{
		// intialize cart
		List<string> cart = new();

		// main loop
		do
		{
			// list and prompt
			ListItems(stock, false);
			Console.WriteLine("\nPlease select an item to purchase");
			Console.WriteLine("Enter either the name or index of the item");

			// select item
			string item = GetItem();
			if (item.Length is 0)
				break;

			// add it to the cart
			cart.Add(item);
		}
		while (PromptYesNo(true, "Would you like to add another item?"));

		Console.WriteLine(" ");

		// get data for cart items. type IEnumerable<KeyValuePair<string, double>>
		var purchased = from item in cart select new KeyValuePair<string, double>(item, stock[item]);
		purchased = purchased.OrderBy(static pair => pair.Value);

		// list purchases with sum
		ListItems(purchased, true);

		// print most and least expensive items
		KeyValuePair<string, double> highest = purchased.MaxBy(static pair => pair.Value);
		Console.WriteLine(string.Format("Most expensive item: {0} @ {1:C}", highest.Key, highest.Value));
		KeyValuePair<string, double> lowest = purchased.MinBy(static pair => pair.Value);
		Console.WriteLine(string.Format("Least expensive item: {0} @ {1:C}", lowest.Key, lowest.Value));

		// goodbye
		Console.WriteLine("Thank you for your purchase!");
		Console.WriteLine("Press any key to exit");
		Console.ReadKey();
	}

	static void ListItems(IEnumerable<KeyValuePair<string, double>> items, bool showSum)
	{
		// automatic column sizing
		int columnName = (from item in items select item.Key).Append("Item").Max(static s => s.Length);

		string format = $"{{0, -{columnName}}}    {{1, -6:C}}";
		if (!showSum)
			format = "{2, -3}    " + format;

		// print header text with divider and spacing
		string header = string.Format(format, "Item", "Price", "#");

		Console.WriteLine();
		Console.WriteLine(header);
		Console.WriteLine(new string('\x2500', header.Length));
		Console.WriteLine();

		// print entries
		int i = 1;
		foreach ((string name, double price) in items)
		{
			Console.WriteLine(string.Format(format, name, price, i));
			i++;
		}

		if (showSum)
		{
			// print sum with divider and spacing
			double sum = items.Sum(static item => item.Value);
			Console.WriteLine();
			Console.WriteLine(new string('\x2500', header.Length));
			Console.WriteLine(string.Format($"Sum: {{0, {header.Length - 5}:C}}", sum));
		}

		Console.WriteLine();
	}

	static string GetItem()
	{
		while(true)
		{
			// null input, canceled by user
			if (Console.ReadLine() is not string line)
				return "";

			line = line.Trim();

			// try to get item by number
			if (int.TryParse(line, out int index) && index > 0)
			{
				var pair = stock.ElementAtOrDefault(index - 1);
				if (pair.Key != null)
					return pair.Key;
			}

			// get item by name
			if (stock.ContainsKey(line))
				return line;

			// no match
			Console.WriteLine("That item is not in stock. Try something else.");
		}
	}

	static bool PromptYesNo(bool allowEscape, string message)
	{
		Console.WriteLine(message + " [Y/N]");

		while (true)
		{
			// get keystroke
			char key = Console.ReadKey().KeyChar;

			// deletes echoed keystroke from output
			Console.Write("\b\\\b");

			// process keystroke
			switch (key)
			{
				// yes
				case 'y':
				case 'Y':
					return true;

				// no
				case 'n':
				case 'N':
					return false;

				// escape key
				case '\x1b':
					if (allowEscape)
						return false;
					break;
			}
		}
	}
}
