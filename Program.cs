using System;
using System.Threading;
using Mindmagma.Curses;

namespace Curses_and_Bullets
{
    class Program
    {
        private static IntPtr Screen;

        private static int BoxWidth = 64;
        
        private static int BoxHeight = 48;

        private static readonly Random rng = new Random();

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
            string[] BackScreen = StarScreenInit();

            while (NCurses.GetChar() == -1)
            {   
                frames++;
                NCurses.Clear();
                StarScreen(BackScreen, lines, cols);
                Box(lines, cols);
                NCurses.MoveAddString(lines/2 - 1, cols/2 - message.Length / 2, message);
                NCurses.MoveAddString(0,0, frames.ToString());
                NCurses.Refresh();
                NCurses.Nap(50);
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

        private static string[] StarScreenInit()
        {
            
            string[] sArray = new string[BoxHeight];
            for(int i = 0; i < BoxHeight; i++)
            {
                char[] charArray = new char [BoxWidth];
                for(int j = 0; j < BoxWidth; j++)
                {
                    int rngCheck = rng.Next(50);
                    if(rngCheck == 1)
                    {
                        charArray[j] = '%';
                    }
                    else 
                    {
                        charArray[j] = ' ';
                    }
                }
                sArray[i] = new string(charArray); 
            }

            return sArray;
        }

        private static void StarScreen(string[] backScreen, int l, int c)
        {
            int leftBound = c/2 - BoxWidth/2;
            int topBound = l/2 - BoxHeight/2;
            
            for(int i = 0; i < BoxHeight; i++)
            {
                NCurses.MoveAddString(topBound + i, leftBound, backScreen[i]);
            }
            string bufstr = backScreen[BoxHeight - 1];
            for(int i = BoxHeight - 1; i > 0; i--)
            {
                backScreen[i] = backScreen[i - 1];
            }
            backScreen[0] = bufstr;
        }

    }
}
