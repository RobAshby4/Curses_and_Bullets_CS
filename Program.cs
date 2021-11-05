using System;
using Mindmagma.Curses;

namespace Curses_and_Bullets
{
    class Program
    {
        private static IntPtr Screen;

        private static int BoxWidth = 64;
        
        private static int BoxHeight = 48;

        private static readonly Random rng = new Random();

        private struct Planet 
        {
            public Planet(int x, int y, string pstr){
                X = x;
                Y = y;
                planet = pstr;
            }
            public int X  {set; get;}
            public int Y {set; get;}
            public string planet {set; get;}
        }

        static void Main(string[] args)
        {
            Screen = NCurses.InitScreen();

            try
            {
                Loop();
            }
            finally
            {
                NCurses.SetCursor(1);
                NCurses.EndWin();
            }
        }

        private static void Loop()
        {
            string message = "Hello Curses!";
            int lines = NCurses.Lines;
            int cols = NCurses.Columns;
            int frames = 0;            

            NCurses.NoDelay(Screen, true);
            NCurses.NoEcho();
            NCurses.SetCursor(0);
            Planet[] bg = BackgroundInit(lines, cols);

            while (NCurses.GetChar() == -1)
            {   
                frames++;
                NCurses.Clear();
                Background(bg, lines, cols);
                Box(lines, cols);
                NCurses.MoveAddString(lines/2 - 1, cols/2 - message.Length / 2, message);
                NCurses.MoveAddString(0,0, frames.ToString());
                NCurses.Nap(50);
                NCurses.Refresh();
            }
        }

        private static void Box(int l, int c)
        {
            string topBottom = "################################################################";
            int leftBound = c/2 - BoxWidth/2;
            int topBound = l/2 - BoxHeight/2;
            NCurses.MoveAddString(topBound, leftBound, topBottom);
            NCurses.MoveAddString(topBound + BoxHeight, leftBound, topBottom);
            for(int i = 1; i < BoxHeight; i++)
            {
                NCurses.MoveAddChar(topBound + i, leftBound, '#');
                NCurses.MoveAddChar(topBound + i, leftBound + BoxWidth - 1, '#');
            }
        }

        private static Planet[] BackgroundInit(int l, int c)
        {
            int PlanetNum = 4;
            Planet[] planets = new Planet[PlanetNum];
            for(int i = 0; i < PlanetNum; i++)
            {
                planets[i] = PlanetGenerator(l, c);
            }
            return planets;
        }
        
        private static Planet PlanetGenerator(int l, int c)
        {
            string planet1 = "( 0 )";
            string planet2 = "-@-";
            string planet3 = "=%=";
            string planet4 = "/0/";

            Planet planet;

            int rngNum = rng.Next(4);
            switch(rngNum)
            {
                case < 1:
                    planet = new Planet(0, 0, planet1);
                    break;
                
                case < 2:
                    planet = new Planet(0, 0, planet2);
                    break;
                
                case < 3:
                    planet = new Planet(0, 0, planet3);
                    break;
                
                case < 4:
                    planet = new Planet(0, 0, planet4);
                    break;

                default:
                    planet = new Planet(0, 0, planet4);
                    break;
            }
            int rngX = rng.Next(BoxWidth) - 4;
            int rngY = rng.Next(BoxHeight) - 1;
            planet.X = c/2 - BoxWidth/2 + 1 + (rngX);
            planet.Y = l/2 - BoxHeight/2 + (rngY);
            return planet;
        }

        private static void Background(Planet[] pArray, int l, int c)
        {
            for(int i = 0; i < pArray.Length; i++)
            {
                NCurses.MoveAddString(pArray[i].Y, pArray[i].X, pArray[i].planet);
                pArray[i].Y++;

                if(pArray[i].Y > l/2 + BoxHeight/2)
                {
                    pArray[i] = PlanetGenerator(l, c);
                    pArray[i].Y = l/2 - BoxHeight/2;
                }
            }
        }
    }
}
