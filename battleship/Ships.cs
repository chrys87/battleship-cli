using System;

namespace battleship
{
	public class Ships
	{
		private int ID;
		private String ShipType;
		private bool IsSunken;
		private char ShipAgenda;
		private int [,] XYPos;
		private int Lenght;
		private int Orientation;
		private bool [] Life;

		public Ships (int NewLenght, String NewShipType,
			char NewShipAgenda,int NewID){
			this.Lenght = NewLenght;
			this.XYPos = new int[this.Lenght, 2];
			this.ShipType = NewShipType;
			this.ShipAgenda = NewShipAgenda;
			this.Life = new bool[this.Lenght];
			for (int i = 0; i < this.Lenght; i++)
				this.Life [i] = false;
			this.IsSunken = false;
			this.Orientation = -1;
			this.ID = NewID;
		}

		public int GetID(){
			return this.ID;
		}

		public bool SetShipPosition(int x, int y, int NewOrientation, Maps MyShipMap){
			// Orientation = 0 Horizontal
			// Orientation = 1 Vertikal
			if (!MyShipMap.IsCoordinateValid(x,0,MyShipMap.GetMapLenght()))
				return false;
			if (!MyShipMap.IsCoordinateValid(y,0,MyShipMap.GetMapHight()))
				return false;
			if (!(NewOrientation == 0) && !(NewOrientation == 1))
				return false;
			if (this.IsOnMap())
				return false;
			switch (NewOrientation) {
			case 0: 
				if ((this.Lenght + x) > MyShipMap.GetMapLenght ()) {
					return false;
				}
				for (int i = 0; i < this.Lenght; i++) {
					if (MyShipMap.GetMapField(x+i,y) != '~' ){
						return false;
					}
				}
				for (int i = 0; i < this.Lenght; i++) {
					XYPos [i, 0] = x + i;
					XYPos [i, 1] = y;
				}
				break;
			case 1:
				if ((this.Lenght + y) > MyShipMap.GetMapHight ())
					return false;
				for (int i = 0; i < this.Lenght; i++) {
					if (MyShipMap.GetMapField (x, y + i) != '~') {
						return false;
					}
				}
				for (int i = 0; i < this.Lenght; i++) {
					XYPos [i, 0] = x;
					XYPos [i, 1] = y + i;
				}
				break;
			default:
				return false;

			}
			this.Orientation = NewOrientation;
			SetAgendaOnMap(MyShipMap, false);
			return true;
		}

		public int[,] GetShipPosition(){
			return this.XYPos;
		}

		public void PrintShipDetails(bool PrintCoordinates){
			if (this.Lenght > 0) {

				if (this.Orientation >= 0) {
					Console.WriteLine (this.ID.ToString() +" " + this.ShipType + "(" + this.ShipAgenda +"," +this.Lenght.ToString()+")");
					if (this.IsSunken)
						Console.WriteLine ("The ship is sunken.");
					if (PrintCoordinates) {
						Console.WriteLine ("x,y");
						for (int i = 0; i < this.Lenght; i++) {
							Console.Write (XYPos [i, 0].ToString () + "," + XYPos [i, 1].ToString ());
							if (this.Life [i])
								Console.Write (" hit.");
							Console.WriteLine ();
						}
					}
				} else {
					Console.WriteLine (this.ID.ToString()+" " + this.ShipType + "(" + this.ShipAgenda +"," +this.Lenght.ToString()+ ")" + " wurde noch nicht gesetzt");
				}
			}
		}

		public void SetAgendaOnMap(Maps ShipMap, bool Cover){
			char NewMapFieldContent = '~';
			if (this.IsSunken)
				NewMapFieldContent = 'X';
			for (int i = 0; i < this.Lenght; i++) {
				if (!this.IsSunken) {
					if (this.Life [i]) {
						NewMapFieldContent = '+';
					} else {
						if (!Cover) {
							NewMapFieldContent = this.ShipAgenda;
						} else {
							NewMapFieldContent = '~';
						}
					}
				}
				ShipMap.SetMapField (XYPos [i, 0], XYPos [i, 1], NewMapFieldContent);
			}

		}

		public char GetShipAgenda(){
			return this.ShipAgenda;
		}

		public bool GetIsSunken(){
			return this.IsSunken;
		}

		public void ModifyHealth( int PartOfShip,bool Value, Maps MyShipMap, Maps AttackMap){
			if (this.IsSunken)
				return;
			if (this.Orientation < 0)
				return;
			if (this.Lenght < PartOfShip)
				return;
			this.Life [PartOfShip] = Value;

			this.IsSunken = true;
			for (int i = 0; i < this.Lenght && this.IsSunken; i++)
				this.IsSunken = (this.IsSunken && this.Life[i]);
			this.SetAgendaOnMap (MyShipMap, false);
			this.SetAgendaOnMap (AttackMap, true);
		}

		public bool DetectImpact(int x, int y, Maps ShipMap, Maps AttackMap){
			if (this.Lenght <= 0)
				return false;
			if (this.IsSunken)
				return false;
			for (int i = 0; i < this.Lenght; i++) {
				if (this.XYPos [i, 0] == x && this.XYPos [i, 1] == y) {
					this.ModifyHealth (i, true, ShipMap, AttackMap);
					return true;
				}
			}
			return false;
		}

		public bool MatchCoordinates(int x, int y){
			for (int i = 0; i < this.Lenght; i++) {
				if ((XYPos [i, 0] == x) && (XYPos [i, 1] == y)) {
					return true;
				}
			}
			return false;
		}

		public bool IsOnMap(){
			return (this.Orientation >= 0);
		}
	}
}

