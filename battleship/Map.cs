using System;

namespace battleship
{
	public class Maps
	{
		private int MapLenght;
		private int MapHight;
		private char [,] Map;

		public Maps (){
			this.MapLenght = 10;
			this.MapHight = 10;
			this.Map = new char [MapLenght, MapHight];
			this.ClearMap ();
		}

		public int GetMapLenght(){
			return this.MapLenght;
		}

		public int GetMapHight(){
			return this.MapHight;
		}

		public char GetMapField(int x, int y){
			if (x < MapLenght && y < MapHight)
				return this.Map [x, y];
			return (char) 0 ;
		}

		public bool SetMapField(int x, int y, char value){
			if (!this.IsCoordinateValid(x,0,this.GetMapLenght()))
				return false;
			if (!this.IsCoordinateValid(y,0,this.GetMapHight()))
				return false;
			this.Map [x, y] = value;
			return true;
		}

		public void ClearMap(){
			for (int i = 0; i < MapHight; i++)
				for (int j = 0; j < MapLenght; j++)
					this.SetMapField (j, i, '~');
		}

		public void PrintMap(bool PrinHorizontaltHelpLines, bool PrintVerticaltHelpLines){
			if (PrintVerticaltHelpLines)
				Console.Write (" ");
			Console.Write (" ");
			for (int i = 0; i < MapLenght; i++) {
				if (PrintVerticaltHelpLines)
					Console.Write ("|");
				Console.Write (i.ToString ());
			}
			if (PrintVerticaltHelpLines)
				Console.Write("|");
			Console.WriteLine ();
			if (PrinHorizontaltHelpLines)
				Console.WriteLine("-----------------------");
			for (int i = 0; i < MapHight; i++) {
				if (PrintVerticaltHelpLines)
					Console.Write ("|");
				Console.Write (i.ToString());
				for (int j = 0; j < MapLenght; j++){
					if (PrintVerticaltHelpLines)
						Console.Write ("|");
					Console.Write (this.GetMapField (j, i));
				}
				if (PrintVerticaltHelpLines)
					Console.Write ("|");
				Console.WriteLine ();
				if (PrinHorizontaltHelpLines)
					Console.WriteLine("-----------------------");

			}
		}

		public bool IsCoordinateValid(int Coordinate, int MinCoordinate, int MaxCoordinate){
			if (Coordinate < MinCoordinate)
				return false;
			if (Coordinate >= MaxCoordinate)
				return false;
			return true;
		}
	}
}

