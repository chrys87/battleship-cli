﻿using System;
using System.Media;

namespace battleship
{
	class MainClass
	{

		public static void Main (string[] args){

			Games CurrGame = new Games ();
			CurrGame.IputAISelection ();
			CurrGame.Run ();

		}

	}
}