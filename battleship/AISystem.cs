using System;

namespace battleship
{
	public class AISystem
	{
		private int [,] LastShotsXYPos;
		private int[] WasImpact;
		private int HistoryLenght;
		private int LastPlacedShip;
		private bool AllShipsInPlace;
		private Players AIPlayer;
		private Random RandomGen;
		private bool LastImpactKillsShip;

		public AISystem (Players NewAIPlayer){
			this.AIPlayer = NewAIPlayer;
			this.RandomGen = new Random ();
			Maps MyShipMap = this.AIPlayer.GetMyShipMap ();
			for (int i = 0; i < 100; i++) // make shure its random
				this.RandomGen.Next (0, MyShipMap.GetMapLenght () - 1);
			this.HistoryLenght = 10;
			this.LastPlacedShip = 0;
			this.AllShipsInPlace = false;
			this.LastShotsXYPos = new int[HistoryLenght, 2];
			for (int i = 0; i < this.HistoryLenght; i++) {
				this.LastShotsXYPos[i,0] = new int();
				this.LastShotsXYPos[i,1] = new int();
			}

			WasImpact = new int[HistoryLenght];
			this.LastImpactKillsShip = false;
		}

		private void NewHistoryEntry(int x, int y, int NewWasImpact){
			for (int i = this.HistoryLenght -1 ; i > 0; i--) {
				this.WasImpact [i] = this.WasImpact [i - 1];
				this.LastShotsXYPos[i,0] = this.LastShotsXYPos[i - 1,0];
				this.LastShotsXYPos[i,1] = this.LastShotsXYPos[i - 1,1];
			}
			this.WasImpact [0] = NewWasImpact;
			this.LastShotsXYPos[0,0] = x;
			this.LastShotsXYPos[0,1] = y;
		}

		private void SetLastImpactKillsShip(bool NewLastImpactKillsShip){
			this.LastImpactKillsShip = NewLastImpactKillsShip;
		}

		private void ClearHistory(){
			for (int i = 0; i < this.HistoryLenght; i++) {
				this.LastShotsXYPos [i, 0] = -1;
				this.LastShotsXYPos [i, 1] = -1;
				this.WasImpact [i] = 0;
			}
			this.LastImpactKillsShip = false;
		}

		public bool DoAITurn(Players Enemy,int PlayerNo){
			Console.WriteLine("Computer ai player "+ PlayerNo.ToString()+" is on turn.");
			if (!this.AllShipsInPlace) {
				this.PlaceAShip ();
			} else {
				this.AllocateAttacZone(Enemy);
			}
			return true;
		}

		public bool PlaceAShip(){
			int x = 0;
			int y = 0;
			int newOrientation = 0;
			Maps MyShipMap = this.AIPlayer.GetMyShipMap ();
			for (int i = 0; i < 30; i++) // make shure its random
				this.RandomGen.Next (0, MyShipMap.GetMapLenght () - 1);
			bool ShipIsPlaced = false;
			while(!ShipIsPlaced && !this.AllShipsInPlace ){
				x = this.RandomGen.Next(0, MyShipMap.GetMapLenght() - 1);
				y = this.RandomGen.Next(0, MyShipMap.GetMapHight() - 1);
				for (int i = 0; i < 25; i++)
					newOrientation = this.RandomGen.Next(0,2);

				ShipIsPlaced = this.AIPlayer.SetShipByID (x, y, newOrientation, this.LastPlacedShip);
				if (ShipIsPlaced)
					this.LastPlacedShip ++;
				this.AllShipsInPlace = this.AIPlayer.AllShipsPlaced ();
			};
			return ShipIsPlaced;
		}

		private void AllocateAttacZone( Players Enemy){
			int x = 0;
			int y = 0;			
			Maps AttackMap = this.AIPlayer.GetAttackMap ();

			if (CalculatePossibleImpact (out x, out y)) {
			} else {
				do {
					x = this.RandomGen.Next (0, AttackMap.GetMapLenght () - 1);
					y = this.RandomGen.Next (0, AttackMap.GetMapHight () - 1);
				} while(AttackMap.GetMapField (x, y) != '~');
			}
			int AttackState = this.AIPlayer.Attack (x, y, Enemy);
			this.SetLastImpactKillsShip (false);
			if (AttackState < 2) {
				this.NewHistoryEntry (x, y, AttackState);
			} else {
				this.SetLastImpactKillsShip (true);
				this.ClearHistory ();
			}
		}

		private bool CalculatePossibleImpact(out int x,out int y){
			x = 0;
			y = 0;
			if (this.LastImpactKillsShip) {
				return false;
			}
			int LastImpact = this.FindImpact (0);
			if (LastImpact == -1)
				return false;
			if (CalculateNewPositionFor (0,LastImpact, out x, out y)) {
				return true;
			}
			LastImpact = this.FindImpact (1);
			if (LastImpact == -1)
				return false;
			if (CalculateNewPositionFor (1,LastImpact, out x, out y)) {
				return true;
			}

			return false;
		}

		private bool CalculateNewPositionFor(int WhichImpact1,int Impact, out int x, out int y){
			int[] DoDecisions = new int[4];
			bool found = true;
			int RndNo = 0;
			for (int i = 0; i < 3; i++) {
				while (found) {
					found = false;
					RndNo = RandomGen.Next (0, 4);
					for (int j = 0; j < i; j++) {
						found = DoDecisions [j] == RndNo;
						if (found)
							break;
					}
				}
				if (!found)
					DoDecisions [i] = RndNo;
			}

			DoDecisions [3] = 6 - DoDecisions [0] - DoDecisions [1] - DoDecisions [2];

			int FirstImpact = this.FindImpact (0);
			int LastImpact = this.FindImpact (1);
			if (LastImpact != FirstImpact) {
				if (this.LastShotsXYPos [LastImpact, 0] == this.LastShotsXYPos [FirstImpact, 0]) {

					DoDecisions [0] = RandomGen.Next (2, 4);
					DoDecisions [1] = 5 - DoDecisions [0];
					DoDecisions [2] = RandomGen.Next (0, 2);
					DoDecisions [3] = 1 - DoDecisions [2];
				}

				if (this.LastShotsXYPos [LastImpact, 1] == this.LastShotsXYPos [FirstImpact, 1]) {
					DoDecisions [0] = RandomGen.Next (0, 2);
					DoDecisions [1] = 1 - DoDecisions [0];
					DoDecisions [2] = RandomGen.Next (2, 4);
					DoDecisions [3] = 5 - DoDecisions [2];
				}
			};
			Maps AttackMap = this.AIPlayer.GetAttackMap ();
			for (int i = 0; i < 4; i++) {
				switch (DoDecisions [i]) {
				case(0):
					if (this.LastShotsXYPos [Impact, 0] + 1 < AttackMap.GetMapLenght ()) {
						if (AttackMap.GetMapField (
							this.LastShotsXYPos [Impact, 0] + 1,
							this.LastShotsXYPos [Impact, 1]) == '~') {
							x = this.LastShotsXYPos [Impact, 0] + 1;
							y = this.LastShotsXYPos [Impact, 1];
							return true;
						}
					}
					break;
				case(1):
					if (this.LastShotsXYPos [Impact, 0] - 1 >= 0) {
						if (AttackMap.GetMapField (
							this.LastShotsXYPos [Impact, 0] - 1,
							this.LastShotsXYPos [Impact, 1]) == '~') {
							x = this.LastShotsXYPos [Impact, 0] - 1;
							y = this.LastShotsXYPos [Impact, 1];
							return true;
						}
					}
					break;
				case(2):
					if (this.LastShotsXYPos [Impact, 1] - 1 < AttackMap.GetMapHight ()) {
						if (AttackMap.GetMapField (
							this.LastShotsXYPos [Impact, 0],
							this.LastShotsXYPos [Impact, 1] + 1) == '~') {
							x = this.LastShotsXYPos [Impact, 0];
							y = this.LastShotsXYPos [Impact, 1] + 1;
							return true;
						}
					}
					break;
				case(3):
					if (this.LastShotsXYPos [Impact, 1] - 1 >= 0) {
						if (AttackMap.GetMapField (
							this.LastShotsXYPos [Impact, 0],
							this.LastShotsXYPos [Impact, 1] - 1) == '~') {
							x = this.LastShotsXYPos [Impact, 0];
							y = this.LastShotsXYPos [Impact, 1] - 1;
							return true;
						}
					}
					break;
				}
			}
			x = 0;
			y = 0;
			return false;
		}

		public bool GetAllShipsInPlace(){
			return this.AllShipsInPlace;
		}

		private int FindImpact(int Which){
			if (Which == 0) { // First
				for (int i = 0; i < this.HistoryLenght; i++) {
					if (this.WasImpact [i] == 1)
						return i;
				}
			} else {
				for (int i = this.HistoryLenght -1; i >= 0; i--) {
					if (this.WasImpact [i] == 1)
						return i;
				}
			}
			return -1;
		}

	}
}

