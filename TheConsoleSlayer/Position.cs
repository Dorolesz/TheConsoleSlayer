using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class Position
    {
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }


        public static Position Add(Position p_1, Position p_2)
        {
            return new Position(p_1.X + p_2.X, p_1.Y + p_2.Y);
        }


        //Euklideszi távolság képlete ( d=gyökAlatt(x_2 - x_1)^2 + (y_2 - y_1)^2 )
        public static double Distance(Position p_1, Position p_2)
        {
            int dx = p_2.X - p_1.X;
            int dy = p_2.Y - p_1.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

}
