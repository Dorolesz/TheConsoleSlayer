using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class ConsoleSprite
    {
        public ConsoleSprite(ConsoleColor backgroung, ConsoleColor foregroung, char glyph)
        {
            Backgroung = backgroung;
            Foregroung = foregroung;
            Glyph = glyph;
        }

        public ConsoleColor Backgroung { get; }
        public ConsoleColor Foregroung { get; }
        public char Glyph { get; }


    }
}
