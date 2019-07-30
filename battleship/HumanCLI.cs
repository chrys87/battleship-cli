using System;

namespace battleship
{
	public class HumanCLI
	{
		private Players HumanPlayer;
		private AISystem AutoTurn;
		private bool AutoSetShips;
		private string RawCommand;
		private string Command;
		private string [] Parameters;
		private Games CurrGame;

		public HumanCLI (Players NewHumanPlayer, Games NewCurrGame){
			this.HumanPlayer = NewHumanPlayer;
			this.ClearCommand();
			this.AutoTurn = new AISystem (this.HumanPlayer);
			this.CurrGame = NewCurrGame;

		}

		private void DoAutoTurnsPlaceShips(Players Enemy, int PlayerNo){
			if (this.AutoSetShips && !this.AutoTurn.GetAllShipsInPlace ()) {
				this.AutoTurn.PlaceAShip ();
			}
			this.AutoSetShips = !this.AutoTurn.GetAllShipsInPlace ();
		}

		public void DoHumanTurn (Players Enemy, int PlayerNo){
			Console.WriteLine ("It is player " + PlayerNo.ToString ()+ "'s turn.");

			if (!this.AutoSetShips) {
				this.PrintLastShoot (false);
				this.InputParseCommand (Enemy); // here can this.AutoSetShips can be set thats the reson
			} // here is no else 
			if (this.AutoSetShips) {
				Console.WriteLine ("Place a Ship...");
				this.DoAutoTurnsPlaceShips (Enemy, PlayerNo);
			}
		}

		public void SetRawCommand(string NewRawCommand){
			this.ClearCommand();
			this.RawCommand = NewRawCommand.Trim ();
			this.RawCommand = System.Text.RegularExpressions.Regex.Replace(this.RawCommand,@"\s+"," ");
			this.RawCommand = this.RawCommand.ToLower();
		}

		public void ClearCommand(){
			this.RawCommand = null;
			this.Command = null;
			this.Parameters = null;


		}

		private bool CreateCommand(){
			if (this.RawCommand != "") { 
				if (this.RawCommand.IndexOf (" ") == -1) {
					this.Command = this.RawCommand;
					return true;
				} else {
					this.Command = this.RawCommand.Substring (0, RawCommand.IndexOf (" "));
					return true;
				}
			} else {
				return false;
			}
		}

		public void InputParseCommand(Players Enemy){
			do {
				do {
					this.SetRawCommand (Console.ReadLine ());
				} while (!this.ParseCommand());
			} while(this.RunCommand (Enemy));
		}

		public bool ParseCommand(){
			if (!this.CreateCommand ())
				return false;
			string [] tmpParameters = RawCommand.Split (' ');
			this.Parameters = new string[tmpParameters.Length - 1];
			for (int i = 0; i < this.Parameters.Length; i++) {
				this.Parameters [i] = tmpParameters[i + 1];
			}

			return true;
		}

		public bool RunCommand(Players Enemy){

			Maps MyShipMap = this.HumanPlayer.GetMyShipMap ();
			Maps AttackMap = this.HumanPlayer.GetAttackMap();

			// set position of ships
			if (Command == "set") {
				if (Parameters.Length != 4) {
					this.ShowError ("You have to input 4 parameters.");
					return true;
				}
				int[] IntArg = new int[Parameters.Length];
				for(int i = 0; i < Parameters.Length;i++){
					if (!Int32.TryParse (this.Parameters [i], out IntArg [i])) {
						this.ShowError ("The parameters have to be numeric.");
						return true;
					}
				}

				if (!MyShipMap.IsCoordinateValid (IntArg [0], 0, this.HumanPlayer.GetNoOfShips())) {
					this.ShowError ("Ship ID " + IntArg [0] + " is not valid (0 to " + (this.HumanPlayer.GetNoOfShips() - 1).ToString()+").");
					return true;
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [1], 0, MyShipMap.GetMapLenght ())) {
					this.ShowError ("Coordinate " + IntArg [1] + " is not valid.");
					return true;
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [2], 0, MyShipMap.GetMapHight ())) {
					this.ShowError ("Coordinate " + IntArg [2] + " is not valid.");
					return true;
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [3], 0, 2)) {
					this.ShowError ("alignment  " + IntArg [3] + " is not valid. 0 = horizontal 1 = vertical.");
					return true;
				}

				if (!this.HumanPlayer.SetShipByID (IntArg [1], IntArg [2], IntArg [3], IntArg [0])) {
					Console.WriteLine ("The ship is already placed or can not placed here.");
					return true;
				}
				Console.WriteLine ("Setting of the ship is successfull.");
				Console.ReadLine ();
				return false;

			}
			if (Command == "afield") {
				if (Parameters.Length != 2) {
					this.ShowError ("You have to input 2 parameters.");
					return true;
				}
				int[] IntArg = new int[Parameters.Length];
				for (int i = 0; i < Parameters.Length; i++) {
					if (!Int32.TryParse (this.Parameters [i], out IntArg [i])) {
						this.ShowError ("The parameters have to be numeric.");
						return true;
					}
				}

				if (!AttackMap.IsCoordinateValid (IntArg [0], 0, AttackMap.GetMapLenght ())) {
					this.ShowError ("Coordinate " + IntArg [0] + " is not valid.");
					return true;
				}
				if (!AttackMap.IsCoordinateValid (IntArg [1], 0, AttackMap.GetMapHight ())) {
					this.ShowError ("Coordinate " + IntArg [1] + " is not valid.");
					return true;
				}

				Console.WriteLine( AttackMap.GetMapField(IntArg[0],IntArg[1]));
				return true;
			}
			if (Command == "sfield") {
				if (Parameters.Length != 2) {
					this.ShowError ("You have to input 2 parameters.");
					return true;
				}
				int[] IntArg = new int[Parameters.Length];
				for (int i = 0; i < Parameters.Length; i++) {
					if (!Int32.TryParse (this.Parameters [i], out IntArg [i])) {
						this.ShowError ("You have to input integers as parameters.");
						return true;
					}
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [0], 0, MyShipMap.GetMapLenght ())) {
					this.ShowError ("Coordinate " + IntArg [0] + " is not valid.");
					return true;
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [1], 0, MyShipMap.GetMapHight ())) {
					this.ShowError ("Coordinate " + IntArg [1] + " is not valid.");
					return true;
				}
				Console.WriteLine( MyShipMap.GetMapField(IntArg[0],IntArg[1]));
				return true;
			}

			if (Command == "skip") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				return false;
			}
			if (this.Command == "bsound"){
				if (Parameters.Length != 1) {
					this.ShowError ("You have to input 1 parameters.");
					return true;
				}
				int IsPlayBackground;
				while (!Int32.TryParse (Console.ReadLine (), out IsPlayBackground) || (IsPlayBackground < 0 || IsPlayBackground > 1)) {
					Console.WriteLine ("You have to be numeric 0/1.");
					return true;
				}
				if (IsPlayBackground==0){
					this.CurrGame.StopBackgroundMusic ();
				}
				else{
					this.CurrGame.StartBackgroundMusic ();
				}
				return true;
			}

			// print list of ships
			if (Command == "ship") {
				if (Parameters.Length > 1) {
					this.ShowError ("You can input until 1 parameters.");
					return true;
				}
				bool PrintDetails = true;
				if (Parameters.Length == 1) {
					if (!Boolean.TryParse (Parameters [0], out PrintDetails))
						PrintDetails = true;
				}
				this.HumanPlayer.PrintShipList (PrintDetails);
				return true;
			}

			if (Command == "autoset") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				if (this.HumanPlayer.AllShipsPlaced ()) {
					this.ShowError ("All ships already in place.");
					return true;
				}
				this.AutoSetShips = true;
				return false;
			}

			if (Command == "attack") {
				if (!ErrorAllShipsHasToBeSet())
					return true;
				if (Parameters.Length != 2) {
					this.ShowError ("You can input until 2 parameters.");
					return true;
				}
				int[] IntArg = new int[Parameters.Length];
				for (int i = 0; i < Parameters.Length; i++) {
					if (!Int32.TryParse (this.Parameters [i], out IntArg [i])) {
						this.ShowError ("The parameters have to be numeric.");
						return true;
					}
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [0], 0, MyShipMap.GetMapLenght ())) {
					this.ShowError ("Coordinate " + IntArg [0] + " is not valid.");
					return true;
				}
				if (!MyShipMap.IsCoordinateValid (IntArg [1], 0, MyShipMap.GetMapHight ())) {
					this.ShowError ("Coordinate " + IntArg [1] + " is not valid.");
					return true;
				}
				switch (this.HumanPlayer.Attack (IntArg [0], IntArg [1], Enemy)) {
				case(0):
					Console.WriteLine ("Missed!");
					break;
				case(1):
					Console.WriteLine ("Hit!");
					break;
				case(2):
					Console.WriteLine ("Hit and sunken!");
					break;
				case(-1):
					Console.WriteLine ("Attack is on ivalid area.");
					return true;
					break;
				}
				Console.ReadLine ();
				return false;
			}

			if (Command == "hilfe" || Command == "help" || Command == "?") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				this.PrintHelp();
				return true;
			}
			if (Command == "shelling") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				this.PrintLastShoot(true);
				return true;
			}
			if (Command == "aturn") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				this.PrintAttackList(AttackMap);
				return true;
			}
			if (Command == "sturn") {
				if (Parameters.Length > 0) {
					this.ShowError ("You have to input no parameters.");
					return true;
				}
				this.PrintAttackList(MyShipMap);
				return true;
			}
			if (Command == "map") {
				if (Parameters.Length > 3){
					this.ShowError ("You can input until 3 parameters.");
					return true;
				}
				bool [] boolPrintHelpLines = new bool[2];
				int ShowMode = 3;
				int [] intPrintHelpLines = new int[2];
				boolPrintHelpLines[0] = false;
				boolPrintHelpLines[1] = true;
				if (this.Parameters.Length > 0) {
					if (!Int32.TryParse (this.Parameters [0], out ShowMode)) {
						this.ShowError ("The first parameter have to be numeric.");
						return true;
					}
				}
				if (Parameters.Length > 1) {
					for (int i = 1; i < Parameters.Length; i++) {
						if (Int32.TryParse (this.Parameters [i], out intPrintHelpLines [i - 1])) {
							if ((intPrintHelpLines[i -1] == 0) || (intPrintHelpLines[i -1] == 1)) {
								boolPrintHelpLines [i - 1] = Convert.ToBoolean (intPrintHelpLines[i-1]);
							} else {
								this.ShowError ("You have to input 0/1.");
								return true;
							}
						} else {
							if (!Boolean.TryParse (this.Parameters [i], out boolPrintHelpLines [i - 1])) {
								this.ShowError ("You have to input 0/1 or true/false.");
								return true;
							}
						}
					}
				}
				this.PrintMaps(ShowMode,boolPrintHelpLines[0],boolPrintHelpLines[1] );
				return true;
			}

			this.ShowError ("");
			return true; //once again
		}

		public void PrintLastShoot(bool ShowMissedAttack){
			Maps MyShipMap = this.HumanPlayer.GetMyShipMap ();
			int[,] LastShoot = this.HumanPlayer.GetLastShoot ();
			if (ShowMissedAttack) {
				if (LastShoot [0, 0] == -1 || LastShoot [0, 1] == -1) {
					Console.WriteLine ("No shelling until now.");
				} else {
					Console.WriteLine ("x=" + LastShoot [0, 0].ToString () + " y=" + LastShoot [0, 1].ToString () + " Feldwert=" + MyShipMap.GetMapField (LastShoot [0, 0], LastShoot [0, 1]));
				}
			} else {
				if (LastShoot [0, 0] == -1 || LastShoot [0, 1] == -1)
					return;
				if (MyShipMap.GetMapField(LastShoot [0, 0],LastShoot [0, 1])!= 'W'&&MyShipMap.GetMapField(LastShoot [0, 0],LastShoot [0, 1])!= '~'){
					Console.WriteLine ("You were hitten on  x=" + LastShoot [0, 0].ToString () + " y=" + LastShoot [0, 1].ToString () + " Fieldvalue=" + MyShipMap.GetMapField (LastShoot [0, 0], LastShoot [0, 1])+".");
				}
			}
		}

		private void ShowError(string ErrorMsg){
			if (ErrorMsg == "") {
				Console.WriteLine ("Command " + this.Command + " not found or missspelled.");
			} else {
				Console.WriteLine (ErrorMsg);
			}
		}

		public bool ErrorAllShipsHasToBeSet(){
			if (!this.HumanPlayer.AllShipsPlaced()) {
				this.ShowError("You have to playe all ships first.");
				return false;
			}
			return true;
		}

		public void PrintMaps(int Mode, bool PrintHorizontalHelpLines, bool PrintVerticaltHelpLines){
			Maps MyShipMap;
			Maps AttackMap;
			switch (Mode) {
			case 0:
				Console.WriteLine("Shipmap:");
				MyShipMap = this.HumanPlayer.GetMyShipMap ();
				MyShipMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				break;
			case 1:
				Console.WriteLine ("Attackmap:");
				AttackMap = this.HumanPlayer.GetAttackMap ();
				AttackMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				break;
			case 3:
				Console.WriteLine ("Attackmap:");
				AttackMap = this.HumanPlayer.GetAttackMap ();
				AttackMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				Console.WriteLine ();
				Console.WriteLine("Shipmap:");
				MyShipMap = this.HumanPlayer.GetMyShipMap ();
				MyShipMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				break;
			default:
				Console.WriteLine ("Attackmap:");
				AttackMap = this.HumanPlayer.GetAttackMap ();
				AttackMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				Console.WriteLine ();
				Console.WriteLine("Shipmap:");
				MyShipMap = this.HumanPlayer.GetMyShipMap ();
				MyShipMap.PrintMap (PrintHorizontalHelpLines, PrintVerticaltHelpLines);
				break;
			} 
		}

		public void PrintAttackList(Maps ForMap){
			Console.WriteLine ("x,y");
			for (int j = 0; j < ForMap.GetMapHight (); j++) {
				for (int i = 0; i < ForMap.GetMapLenght (); i++) {
					if (ForMap.GetMapField (i, j) == 'W' || ForMap.GetMapField (i, j) == '+' || ForMap.GetMapField (i, j) == 'X') {
						Console.WriteLine(i.ToString()+","+j.ToString()+ " Fieldvalue="+ForMap.GetMapField (i, j).ToString());
					};	

				}
			}

		}

		public void PrintHelp(){
			Console.WriteLine("All commands are lowercase.");
			Console.WriteLine("The generally parameters:");
			Console.WriteLine("<XPosition>: numeric(0-9) Is the width of the map.");
			Console.WriteLine("<YPosition>: numeric(0-9) Is the height of the map.");
			Console.WriteLine("<ShipID>: numeric(0-4).");
			Console.WriteLine("<alignment>: 0 = horizontal 1 = vertikal.");
			Console.WriteLine("Commands:");
			Console.WriteLine ("set <ShipID> <XPosition> <YPosition> <alignment>: set the ship <ShipID> to the coordinates <XPosition> <XPosition> with the alignment <alignment>.");
			Console.WriteLine ("autoset: Place the ships automatically. One Ship per turn.");
			Console.WriteLine("help: This helpscreen.");
			Console.WriteLine("skip: Skip your turn.");
			Console.WriteLine ("shelling: show the enemys last counterattack.");
			Console.WriteLine("map: Show shipmap and attackmap.");
			Console.WriteLine("map <Mode> <Vertical Helperline> <Horizontale Helperline>: Show an map in given Mode turned on/off Helperlines.");
			Console.WriteLine("start parameters of map");
			Console.WriteLine("<Mode>: numeric (0-2) 0 = print shipmap and attackmap (default of map without parameters) 1= print attackmap 2= print shipmap.");
			Console.WriteLine("<Vertical Helperline>: Zahl(0-1) Zeige Vertikale Hilfslinien in Ascii Art auf der Karte.");
			Console.WriteLine("<Horizontale Helperline>: Zahl(0-1) Zeige Horizontale Hilfslinien in Ascii Art auf der Karte.");
			Console.WriteLine("ende parameters of map");
			Console.WriteLine("attack XPosition YPosition: do an shot to XPosition YPosition.");
			Console.WriteLine("ships: Show an list of shiips with state an position.");
			Console.WriteLine("aturn: show an list of your last shots.");
			Console.WriteLine("sturn: show an list of the enemy last shots.");
			Console.WriteLine ("afield XPosition YPosition: Show the content of the field XPosition YPosition on the attackmap. See Agenda.");
			Console.WriteLine ("sfield XPosition YPosition: Show the content of the field XPosition YPosition on the shipmap. See Agenda.");
			Console.WriteLine("Agenda of the Map:");			
			Console.WriteLine("~ = Water; Not attacked yet.");
			Console.WriteLine("W = Water; Already attacked here.");
			Console.WriteLine("X = Sunken ship.");
			Console.WriteLine("+ = Hit (not sunken).");
			Console.WriteLine("G = Gunboat.");
			Console.WriteLine("D = Destroyer.");
			Console.WriteLine("F = Frigate.");
			Console.WriteLine("B = Battleship.");
			Console.WriteLine("A = Air Carrier.");
			Console.WriteLine ("Gamesettings");
			Console.WriteLine ("bsound <IsSound>: turns the backgroundsound on <IsSound>=1 or off <IsSound>=0");

		}
	}
}

