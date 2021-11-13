using System;
using System.Collections.Generic;
using Mindmagma.Curses;

namespace Curses_and_Bullets
{
    public class Bullet
    {
        public Bullet(int x, int y, char bstr, int bt, bool powned)
        {
            X = x;
            Y = y;
            bmodel = bstr;
            btype = bt;
            removeb = false;
            powner = powned;
        }

        public int X  {set; get;}
        public int Y {set; get;}
        public char bmodel {set; get;}
        public int btype {set; get;}
        public bool removeb {set; get;}
        public bool powner {set; get;}
    }

    class Program
    {
        private static IntPtr Screen;
        
        private int frames = 0;

        private static int BoxWidth = 64;
        
        private static int BoxHeight = 48;

        private int LeftBound = NCurses.Columns/2 - BoxWidth/2;

        private int TopBound = NCurses.Lines/2 - BoxHeight/2;
        
        private string[] BoxString = new string[50];

        public int Ammo = 49;

        private static readonly Random rng = new Random();

        private static readonly short[] color_table = {
            CursesColor.BLACK,
            CursesColor.RED,
            CursesColor.BLUE,
            CursesColor.GREEN,
            CursesColor.CYAN,
            CursesColor.RED,
            CursesColor.MAGENTA,
            CursesColor.YELLOW,
            CursesColor.WHITE
        };

        private struct Planet 
        {
            public Planet(int x, int y, string pstr)
	    {
                X = x;
                Y = y;
                planet = pstr;
            }
            public int X  {set; get;}
            public int Y {set; get;}
            public string planet {set; get;}
        }
        
        private struct Player 
        {
            public Player(int x, int y, string pstr)
	    {
                X = x;
                Y = y;
                pmodel = pstr;
                shooting = false;
                btype = 1;
                bfired = 0;
            }
            public int X  {set; get;}
            public int Y {set; get;}
            public string pmodel {set; get;}
            public bool shooting {set; get;}
            public int btype {set; get;}
            public int bfired {set; get;}
        }


        private Player PC = new Player(0, 0, "/|\\");

        public List<Bullet> bullets = new List<Bullet>();

        static void Main(string[] args)
        {
            Screen = NCurses.InitScreen();
            
            if(NCurses.Lines < 50 || NCurses.Columns < 80){
                NCurses.EndWin();
                Console.WriteLine("Terminal must be at least 50L and 80C");
                System.Environment.Exit(1);
            }

            try
            {
                Program loop = new Program();
                loop.Loop();
            }
            finally
            {
                NCurses.SetCursor(1);
                NCurses.EndWin();
            }
        }

        private void Loop()
        {
            BoxString[0] = "################################################################\n";
            BoxString[49] = "################################################################\n";
            for(int i = 1; i < 49; i++)
            {
                BoxString[i] = "#                                                              #\n" ;
            }

            if(NCurses.HasColors())
            {
                NCurses.StartColor();
                for (short i = 1; i < 9; i++)
                {
                    NCurses.InitPair(i, color_table[i], CursesColor.BLACK);
                }
            }

            Console.TreatControlCAsInput = true;
            int lines = NCurses.Lines;
            int cols = NCurses.Columns;          
            int quit = 0;

            NCurses.NoDelay(Screen, true);
            NCurses.NoEcho();
            NCurses.SetCursor(0);
            Planet[] bg = BackgroundInit(lines, cols);
            PC.X = cols/2;
            PC.Y = lines/2;
            ConsoleKeyInfo cki;
            while (quit != -1)
            {   
                frames++;
                NCurses.Clear();
                Box(lines, cols);
                Background(bg, lines, cols);
                NCurses.MoveAddString(0,0, frames.ToString());
                NCurses.MoveAddString(1,0, PC.shooting.ToString());
                NCurses.MoveAddString(2,0, PC.bfired.ToString());
                NCurses.MoveAddString(3,0, Ammo.ToString());
                NCurses.Nap(50);
                if(Console.KeyAvailable == true)
                {
                    cki = Console.ReadKey(true);
                    quit = PMovement(cki);                
                }
                NCurses.MoveAddString(PC.Y, PC.X, PC.pmodel);
                RenderBullets();
                RenderAmmo();
                NCurses.Refresh();
            }
        }

        private void RenderAmmo()
        {
            NCurses.MoveAddString(TopBound, LeftBound - 6, "Ammo");
            for (int i = 0; i < Ammo ; i++)
            {
                NCurses.MoveAddString(TopBound + i + 1, LeftBound - 5, "[*]");
            }
        }

        private void RenderBullets()
        {
            if(PC.bfired >= 3)
            {
                PC.shooting = false;
                PC.bfired = 0;
            }
            if(Ammo <= 0)
            {
                PC.shooting = false;
                Ammo = 0;
                PC.bfired = 0;
            } 
            if(PC.shooting == true && Ammo > 0 && PC.bfired < 3)
            {
                bullets.Add(new Bullet(PC.X + 1, PC.Y - 1, '*', 1, true));
                PC.bfired++;
                Ammo--;   
            }
            else if(Ammo < 49 && frames % 4 == 0)
            {
                Ammo++;
            }
            NCurses.AttributeOn(NCurses.ColorPair(7));
            foreach(var aBullet in bullets)
            {
                NCurses.MoveAddChar(aBullet.Y, aBullet.X, aBullet.bmodel);
                switch(aBullet.btype)
                {
                    case 1:
                        if(aBullet.Y - 1 > TopBound)
                            aBullet.Y -= 1;
                        else{
                            aBullet.removeb = true;
                        }
                        break;

                    default:
                        break;
                }
            }
            bullets.RemoveAll(b => b.removeb == true);
            NCurses.AttributeOff(NCurses.ColorPair(7));
        }

        private int PMovement(ConsoleKeyInfo cki)
        {
            switch(cki.Key.ToString()){
                case "UpArrow":
                    if(PC.Y - 1 != TopBound)
                        PC.Y -= 1;
                    return 0;
                
                case "DownArrow":
                    if(PC.Y + 1 != TopBound + 49)
                        PC.Y += 1;
                    return 0;
                
                case "LeftArrow":
                    if(PC.X  - 2 > LeftBound)
                        PC.X -= 2;
                    return 0;
                
                case "RightArrow":
                    if(PC.X  + 2 < LeftBound + 61)
                        PC.X += 2;
                    return 0;
                
                case "Q":
                    return -1;
                
                case "Z":
                    if(Ammo > 2)
                    {
                        PC.shooting = true;
                        PC.bfired = 0;
                    }
                    return 0;
                
                default:
                    return 0;
            }
        }

        private void Box(int l, int c)
        {
            for(int i = 0; i < 50; i++)
            {
                NCurses.MoveAddString(TopBound + i, LeftBound, BoxString[i]);
            }
        }

        private Planet[] BackgroundInit(int l, int c)
        {
            int PlanetNum = 4;
            Planet[] planets = new Planet[PlanetNum];
            for(int i = 0; i < PlanetNum; i++)
            {
                planets[i] = PlanetGenerator(l, c);
            }
            return planets;
        }
        
        private Planet PlanetGenerator(int l, int c)
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
            int rngX = rng.Next(BoxWidth - 5);
            int rngY = rng.Next(BoxHeight - 1);
            planet.X = LeftBound + 1 + (rngX);
            planet.Y = TopBound + 1 + (rngY);
            return planet;
        }
        private void Background(Planet[] pArray, int l, int c)
        {
            for(int i = 0; i < pArray.Length; i++)
            {
                NCurses.AttributeOn(NCurses.ColorPair(5));
                NCurses.MoveAddString(pArray[i].Y, pArray[i].X, pArray[i].planet);
                NCurses.AttributeOff(NCurses.ColorPair(6));
                if(frames % 3 == 0)
                {
                    pArray[i].Y++;
                }

                if(pArray[i].Y > l/2 + BoxHeight/2)
                {
                    pArray[i] = PlanetGenerator(l, c);
                    pArray[i].Y = TopBound + 1;
                }
            }
        }
    }
}
