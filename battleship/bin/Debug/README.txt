For running battleship you need:
- mono framework 
- sdl_sound. 

Maybe its also working on .net (but its not tested yet).

On arch:
sudo pacman -S mono sdl_sound
On fedora:
sudo yum install mono* SDL_sound

Start:
if you have mono in your path variable you can just start it with 
./battleship.bin
if you dont have mono in your path variable
mono battleship.bin
in the containing folder.


todo:
can build an shipyart
can repair ships
can move ships
special attack per ship

if you wanna help ->
mail@chrys.de
or 
chrys87@web.de


After Starting the game you can select an computer ai enemy or an human.
The game is turn based.
with the command 
help
you will see all the possible commands.

first you have to set all the sips. after that you can attack.
the coordinates are just numeric for example 1 1 is the secod field in the second row (starting with 0

here the helpscreen:

All commands are lowercase.
The generally parameters:
<XPosition>: numeric(0-9) Is the width of the map.
<YPosition>: numeric(0-9) Is the height of the map.
<ShipID>: numeric(0-4).
<alignment>: 0 = horizontal 1 = vertikal.
Commands:
set <ShipID> <XPosition> <YPosition> <alignment>: set the ship <ShipID> to the coordinates <XPosition> <XPosition> with the alignment <alignment>.
autoset: Set the Ships automatically. One ship per turn.
help: This helpscreen.
skip: Skip your turn.
shelling: show the enemys last counterattack.
map: Show shipmap and attackmap.
map <Mode> <Vertical Helperline> <Horizontale Helperline>: Show an map in given Mode turned on/off Helperlines.
start parameters of map
<Mode>: numeric (0-2) 0 = print shipmap and attackmap (default of map without parameters) 1= print attackmap 2= print shipmap.
<Vertical Helperline>: Zahl(0-1) Zeige Vertikale Hilfslinien in Ascii Art auf der Karte.
<Horizontale Helperline>: Zahl(0-1) Zeige Horizontale Hilfslinien in Ascii Art auf der Karte.
ende parameters of map
attack XPosition YPosition: do an shot to XPosition YPosition.
ships: Show an list of shiips with state an position.
aturn: show an list of your last shots.
sturn: show an list of the enemy last shots.
afield XPosition YPosition: Show the content of the field XPosition YPosition on the attackmap. See Agenda.
sfield XPosition YPosition: Show the content of the field XPosition YPosition on the shipmap. See Agenda.
Agenda of the Map:
~ = Water; Not attacked yet.
W = Water; Already attacked here.
X = Sunken ship.
+ = Hit (not sunken).
G = Gunboat.
D = Destroyer.
F = Frigate.
B = Battleship.
A = Air Carrier.
