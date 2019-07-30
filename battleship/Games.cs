using System;
using System.Media;
using System.Diagnostics;
using System.Threading;

namespace battleship
{
	public class Games
	{
		private int CurrentPlayer = 0;
		private int NextPlayer = 0;
		private Players [] Player;
		private bool Finished = false;
		private int AISelection = -1;
		private volatile bool IsPlayBackgroundSound;
		private Thread BackgroundSoundThread;
		private bool PlaySoundEffects;

		public Games (){
			this.Player = new Players[2];
			this.Player [0] = new Players (this);
			this.Player [1] = new Players (this);
			this.Player [0].SetIsHuman (true);
			this.Player [1].SetIsHuman (true);
			PlaySoundEffects = true;
			StartBackgroundMusic ();
		}

		public void StartBackgroundMusic(){
			BackgroundSoundThread = new Thread (new ThreadStart(PlayBackgroundMusic	));
			this.IsPlayBackgroundSound = true;
			this.BackgroundSoundThread.Start ();
		}

		public void StopBackgroundMusic(){
			this.IsPlayBackgroundSound = false;
			this.BackgroundSoundThread.Abort ();
			this.BackgroundSoundThread = null;
		}

		public void PlayBackgroundMusic(){
			while (true) {
				if (!this.IsPlayBackgroundSound) {
					return;
				}else {
					this.PlaySound (3, true);
				}
			}
		}

		public void IputAISelection(){
			Console.WriteLine ("Select an enemy:");
			Console.WriteLine ("0 Computer ai");
			Console.WriteLine("1 Human");

			while (!Int32.TryParse (Console.ReadLine (), out this.AISelection) || (this.AISelection < 0 || this.AISelection > 1)) {
				Console.WriteLine ("Invalid Input.");
			}

			if (this.AISelection == 1) {
				Console.WriteLine("You select an human enemy.");
				Player [1].SetIsHuman (true);
			} else {
				Console.WriteLine("You select an computer ai enemy");
				Player [1].SetIsHuman (false);
			}
			Console.WriteLine("Press Return to continue.");
			Console.ReadLine ();
			Console.Clear();
		}

		public void Run(){
			while (!this.Finished){

				switch(this.CurrentPlayer){
				case 0:
					this.NextPlayer = 1;
					Finished = Player[this.CurrentPlayer].DoPlayerTurn(CurrentPlayer,Player [this.NextPlayer]);
					if (!Finished) {
						this.CurrentPlayer = this.NextPlayer;
					}
					break;
				case 1:
					this.NextPlayer = 0;
					Finished = Player [this.CurrentPlayer].DoPlayerTurn (this.CurrentPlayer, Player [this.NextPlayer]);
					if (!Finished) {
						this.CurrentPlayer = this.NextPlayer;
					}
					break;
				}
			}

			Console.WriteLine ("Player " + CurrentPlayer.ToString() + " wins!");
			Console.WriteLine("Congratulations!!!");
			Console.WriteLine ("Press Return to exit.");
			Console.ReadLine ();
		}

		public void SetSoundEffects(bool NewSoundEffects){
			this.PlaySoundEffects = NewSoundEffects;
		}

		public void PlaySound(int SoundID, bool Wait){
			if (SoundID != 3) //backgroundsound
			if (!this.PlaySoundEffects)
				return;
			string SoundFile = "";
			switch (SoundID) {
			case(0):
				SoundFile = "./battleship-sounds/cannon.ogg";
				break;
			case(1):
				SoundFile = "./battleship-sounds/cannon-hit.ogg";
				break;
			case(2):
				SoundFile = "./battleship-sounds/cannon-miss.ogg";
				break;
			case(3):
				SoundFile = "./battleship-sounds/ocean.ogg";
				break;
			case(4):
				SoundFile = "./battleship-sounds/ship-destroyed.ogg";
				break;
			default:
				break;
			};
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.WorkingDirectory = "./";

			proc.StartInfo.FileName = "/bin/playsound";
			proc.StartInfo.Arguments = SoundFile;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardOutput = true;
			proc.StartInfo.RedirectStandardError = true;
			proc.StartInfo.RedirectStandardInput = true;
			proc.Start ();
			if (Wait) 
				proc.WaitForExit();
			//(new SoundPlayer(@"/home/chrys/Projekte/battleship-ger/battleship/bin/Debug/battleship-sounds/cannon.wav")).PlaySync();

			//SoundPlayer SP = new SoundPlayer();
			//SP.SoundLocation = "/home/chrys/Projekte/battleship-ger/battleship/bin/Debug/battleship-sounds/cannon.wav";
			//SP.PlaySync();

		}
	}
}

