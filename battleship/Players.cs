using System;

namespace battleship
{

	public class Players
	{
		private Maps MyShipMap;
		private Maps AttackMap;
		private Ships [] MyShips;
		private int NoOfShips;
		private bool IsHuman;
		private int[,] LastEnemyShoot;
		private AISystem AI;
		private HumanCLI Human;
		private Games CurrGame;
		private bool SoundEffects;

		public Players (Games NewCurrGame){
			this.MyShipMap  = new Maps();
			this.AttackMap  = new Maps();
			this.NoOfShips = 5;
			this.MyShips = new Ships[this.NoOfShips]; 
			this.LastEnemyShoot = new int[1,2];
			this.LastEnemyShoot [0, 0] = -1;
			this.LastEnemyShoot [0, 1] = -1;
			this.CurrGame = NewCurrGame;
			this.CreateShips ();
		}

		public void SetIsHuman(bool NewIsHuman){
			this.IsHuman = NewIsHuman;
			SoundEffects = this.IsHuman;
			if (this.IsHuman) {
				this.Human = new HumanCLI (this, this.CurrGame);
				this.AI = null;

			} else {
				this.AI = new AISystem (this);
				this.Human = null;
				this.Human = new HumanCLI (this,this.CurrGame); // debug AI 
			}
		}

		public int GetNoOfShips(){
			return NoOfShips;
		}

		public bool DoPlayerTurn(int PlayerNo, Players Enemy){
			this.CurrGame.SetSoundEffects (this.SoundEffects);
			if (this.IsHuman)
				DoHumanPlayerTurn(PlayerNo, Enemy);
			else
				DoAIPlayerTurn(PlayerNo, Enemy);
			Console.Clear ();

			return this.IsPlayerVictory (Enemy);
		}

		public void DoAIPlayerTurn(int PlayerNo, Players Enemy){
			this.AI.DoAITurn (Enemy,PlayerNo);
			//this.Human.PrintMaps (3, false, true); // debug AI
			//Console.ReadLine (); // debug AI
		}

		public void DoHumanPlayerTurn(int PlayerNo, Players Enemy){
			this.Human.DoHumanTurn (Enemy, PlayerNo);
		}

		public bool IsPlayerVictory(Players Enemy){
			bool Victory = true;
			Ships [] EnemyFleet = Enemy.GetPlayerShips();
			if (EnemyFleet == null)
				return false;
			foreach (Ships EnemyShip in EnemyFleet)
				Victory = Victory && EnemyShip.GetIsSunken();
			return Victory;
		}

		public Ships [] GetPlayerShips(){
			return this.MyShips;
		}

		public Maps GetMyShipMap(){
			return this.MyShipMap;
		}

		public Maps GetAttackMap(){
			return this.AttackMap;
		}

		public void SetLastShoot(int x, int y){
			this.LastEnemyShoot [0, 0] = x;
			this.LastEnemyShoot [0, 1] = y;

		}

		public int[,] GetLastShoot(){
			return this.LastEnemyShoot;
		}

		public int Attack(int x, int y, Players Enemy){
			Maps EnemyMap = Enemy.GetMyShipMap ();
			if (!EnemyMap.IsCoordinateValid (x, 0, EnemyMap.GetMapLenght ())) {
				return -1;
			}
			if (!EnemyMap.IsCoordinateValid (y, 0, EnemyMap.GetMapHight ())) {
				return -1;
			}
			this.CurrGame.PlaySound (0,false);
			Enemy.SetLastShoot (x, y);
			Ships DestinationShip = Enemy.GetShipOnMapPos (x, y);

			if (DestinationShip != null) {
				DestinationShip.DetectImpact (x, y, EnemyMap, this.AttackMap);
				if (DestinationShip.GetIsSunken ()) {
					this.CurrGame.PlaySound (4,false);
					return 2;
				}
				this.CurrGame.PlaySound (1,false);
				return 1;
			} else {
				this.AttackMap.SetMapField (x, y, 'W');
				EnemyMap.SetMapField (x, y, 'W');
				this.CurrGame.PlaySound (2,false);
				return 0;
			}
			return 0;
		}

		public Ships GetShipOnMapPos(int x, int y){
			char MapAgenda = this.MyShipMap.GetMapField(x,y);
			if (MapAgenda == '~')
				return null;
			if (MapAgenda == 'W')
				return null;
			if (MapAgenda == '+')
				return null;
			foreach (Ships CurrShip in this.MyShips) {
				if (MapAgenda == CurrShip.GetShipAgenda()) {
					if (CurrShip.MatchCoordinates (x, y))
						return CurrShip;
				}
			}
			return null;
		}

		private void CreateShips(){
			this.MyShips [0] = new Ships (2, "Gunboat", 'G',0);
			this.MyShips [1] = new Ships (3, "Destroyer", 'D',1);
			this.MyShips [2] = new Ships (3, "Frigate", 'F',2);
			this.MyShips [3] = new Ships (4, "Battleship", 'B',3);
			this.MyShips [4] = new Ships (5, "Air Carrier", 'A',4);
		}

		public void PrintShipList(bool PrintCoordinates){
			foreach (Ships CurrShip in this.MyShips) {
				CurrShip.PrintShipDetails (PrintCoordinates);
			}

		}

		public bool AllShipsPlaced(){
			bool AllPlaced = true;
			for (int i = 0; i < this.NoOfShips && AllPlaced; i++) {
				AllPlaced = AllPlaced && this.MyShips [i].IsOnMap ();
			}
			return AllPlaced;
		}

		public bool SetShipByID(int x, int y, int NewOrientation, int ID){
			foreach (Ships CurrShip in this.MyShips) {
				if (CurrShip.GetID() == ID) {
					return CurrShip.SetShipPosition (x, y, NewOrientation, this.MyShipMap);
				}
			}
			return false;
		}
	}
}

