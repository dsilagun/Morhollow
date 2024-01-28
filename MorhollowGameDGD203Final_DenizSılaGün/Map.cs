using System;
using System.Numerics;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;

public class Map
{
	private Game _theGame;

	private Vector2 _coordinates;

	private int[] _widthBoundaries;
	private int[] _heightBoundaries;

	private Location[] _locations;


	public Map(Game game, int width, int height)
	{
		_theGame = game;

		// Setting the width boundaries
		int widthBoundary = (width - 1) / 2;

        _widthBoundaries = new int[2];
        _widthBoundaries[0] = -widthBoundary;
		_widthBoundaries[1] = widthBoundary;

		// Setting the height boundaries
        int heightBoundary = (height - 1) / 2;

        _heightBoundaries = new int[2];
		_heightBoundaries[0] = -heightBoundary;
		_heightBoundaries[1] = heightBoundary;

		// Setting starting coordinates
        _coordinates = new Vector2(0,0);

		GenerateLocations();

        // Display result
         Console.WriteLine("Creating World...");
        //Console.WriteLine($"Created map with size {width}x{height}");
    }

    #region Coordinates

    public Vector2 GetCoordinates()
	{
		return _coordinates;
	}

	public void SetCoordinates(Vector2 newCoordinates)
	{
		_coordinates = newCoordinates;
	}

	#endregion

	#region Movement

	public void MovePlayer(int x, int y)
	{
        int newXCoordinate = (int)_coordinates[0] + x;
        int newYCoordinate = (int)_coordinates[1] + y;

        if (!CanMoveTo(newXCoordinate, newYCoordinate))
        {
            Console.WriteLine("You can't go that way");
            return;
        }

		_coordinates[0] = newXCoordinate;
        _coordinates[1] = newYCoordinate;
        //_coordinates = new Vector2(newXCoordinate, newYCoordinate);

        CheckForLocation(_coordinates);
    }

	private bool CanMoveTo(int x, int y)
	{
		return !(x < _widthBoundaries[0] || x > _widthBoundaries[1] || y < _heightBoundaries[0] || y > _heightBoundaries[1]);
	}

	#endregion

	#region Locations

	private void GenerateLocations()
	{
        _locations = new Location[6];
        //Zenithar
        Vector2 zenitharLocation = new Vector2(0, 0);
		List<Item> zenitharItems = new List<Item>();
		zenitharItems.Add(Item.Coin);
        Location zenithar = new Location("Zenithar", LocationType.Forest, zenitharLocation, zenitharItems);
        _locations[0] = zenithar;
        //Crow’s Perch
        Vector2 crowperchLocation = new Vector2(-2, 2);
		List<Item> crowperchtems = new List<Item>();
		crowperchtems.Add(Item.SymbolsPaper);
        Location CrowPerch = new Location("Crow’s Perch", LocationType.Forest, crowperchLocation, crowperchtems);
        _locations[1] = CrowPerch;
        // Faerie Mound
        Vector2 faerieMLocation = new Vector2(1, -2);
		List<Item> FaerieMItems = new List<Item>();
		FaerieMItems.Add(Item.MagicalPowder);
        Location Faerie = new Location("Faerie Mound", LocationType.Forest, faerieMLocation, FaerieMItems);
        _locations[2] = Faerie;
        //First Puzzle
        Vector2 FirstPLocation = new Vector2(-2, 1);
        Location FirstPuzzle = new Location("First Puzzle", LocationType.Cave, FirstPLocation);
        _locations[3] = FirstPuzzle;


        Vector2 morleyLocation = new Vector2(1, 1);
        Location morley = new Location("Zenithar", LocationType.Forest, morleyLocation);
        _locations[4] = morley;


        Vector2 secondCombatLocation = new Vector2(-1, -2);
        Location secondCombat = new Location("Faerie Mound", LocationType.Forest, secondCombatLocation);
        _locations[5] = secondCombat;

        
    }
	public void Cave(string name)
	{
        Console.WriteLine("You find yourself in a somber cavern. A mysterious Sentinel stands before you.");
        Console.WriteLine("Sentinel: Halt! Who goes there?");
        Console.WriteLine("Sentinel: I see that you gathered the items I desired. Well done. Now...");
        Console.WriteLine($"Sentinel: Speak your name, brave adventurer, {name}.");


        Console.WriteLine("Sentinel: Before you proceed, you must answer this riddle:");
        Console.WriteLine("Sentinel: In shadows deep, I whisper low,");
        Console.WriteLine("Sentinel: A secret held, the ancients know.");
        Console.WriteLine("Sentinel: A flicker bright, a spark untold,");
        Console.WriteLine("Sentinel: This I am, a spirit bold.");
        Console.WriteLine("Sentinel: Within the flesh, I make my home,");
        Console.WriteLine("Sentinel: Yet bound to wander, free to roam.");
        Console.WriteLine("Sentinel: What am I, this essence whole,");
        Console.WriteLine("Sentinel: A treasure deep, the essence...");

        string playerAnswer = Console.ReadLine();

        if (playerAnswer.ToLower() == "soul")
        {
            Console.WriteLine("Sentinel: Well done! You may proceed.");

            RestartApp(Process.GetCurrentProcess().Id, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
        }
        else
        {
            Console.WriteLine("Sentinel: Incorrect! You shall not pass until you solve the riddle.");


            Console.WriteLine("Sentinel: The Sentinel's gaze darkens, and you feel a sudden chill.");
			
            
            return;
        }
        static void RestartApp(int pid, string applicationName)
        {
            // Wait for the process to terminate
            Process process = null;
            try
            {
                process = Process.GetProcessById(pid);
                process.WaitForExit(1000);
            }
            catch (ArgumentException ex)
            {
                // ArgumentException to indicate that the 
                // process doesn't exist?   LAME!!
            }
            Process.Start(applicationName, "");
        }
    }
    public void CheckForLocation(Vector2 coordinates)
	{
        Console.WriteLine($"Thou are presently positioned upon {_coordinates[0]},{_coordinates[1]}");

        if (IsOnLocation(_coordinates, out Location location))
        {
            if (location.Type == LocationType.Cave)
			{
				Console.WriteLine("Make ready to solve the riddle!");
				Cave(this._theGame.Player.Name);


            } else
			{
				Console.WriteLine($"You are in {location.Name} {location.Type}");

				if (HasItem(location))
				{
                    Console.WriteLine($"An item called { location.ItemsOnLocation[0]} here. Write a direction to go to the next location.");
				}
			}
        }
    }

	private bool IsOnLocation(Vector2 coords, out Location foundLocation)
	{
		try
		{

            for (int i = 0; i < _locations.Length; i++)
            {
				if (_locations[i].Coordinates == coords)
				{
					foundLocation = _locations[i];
					return true;
				}
            }

        }
		catch (Exception)
		{
			Console.WriteLine("You canst not proceed in that direction.");
        }
        foundLocation = null;
        return false;
    }

	private bool HasItem(Location location)
	{
		return location.ItemsOnLocation.Count != 0;

		// ---- THE LONG FORM ----
		//if (location.ItemsOnLocation.Count == 0)
		//{
		//	return false;
		//} else
		//{
		//	return true;
		//}
	}

    public void TakeItem(Location location)
	{

	}

	public void TakeItem(Player player, Vector2 coordinates)
	{
		if (IsOnLocation(coordinates, out Location location))
		{
			if (HasItem(location))
			{
				Item itemOnLocation = location.ItemsOnLocation[0];

				player.TakeItem(itemOnLocation);
				location.RemoveItem(itemOnLocation);
				if(itemOnLocation.ToString() == "MagicalPowder")
				{
					Console.WriteLine("You took the Magical Powder. It is going to be useful in the future.");
				}
				else
                {
                    Console.WriteLine($"You took the {itemOnLocation}");
                }

				return;
			}
		}

		Console.WriteLine("There is nothing to take here!");
	}

	public void RemoveItemFromLocation(Item item)
	{
		for (int i = 0; i < _locations.Length; i++)
		{
			if (_locations[i].ItemsOnLocation.Contains(item))
			{
				_locations[i].RemoveItem(item);
			}
		}
	}

	#endregion
}