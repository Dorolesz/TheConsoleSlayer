using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TheConsoleSlayer
{
    public enum ItemType { Ammo, BFGCell, Door, LevelExit, Medikit, ToxicWaste, Wall }

    internal class GameItem
    {

        private Position position;
        private ConsoleSprite sprite;
        private ItemType type;
        private double fillingRatio;
        private bool available;

        public Position Position
        {
            get { return position; }
        }

        public ConsoleSprite Sprite
        {
            get { return sprite; }
        }

        public ItemType Type
        {
            get { return type; }
        }

        public double FillingRatio
        {
            get { return fillingRatio; }
        }

        public bool Available
        {
            get { return available; }
        }

        public GameItem(int x, int y, ItemType type)
        {
            position = new Position(x, y);
            this.type = type;
            available = true;

            SetInitialProperties();
        }

        private void SetInitialProperties()
        {
            switch (type)
            {
                case ItemType.Ammo:
                    sprite = new ConsoleSprite(ConsoleColor.DarkRed, ConsoleColor.Yellow, 'A');
                    fillingRatio = 0.0;
                    break;

                case ItemType.BFGCell:
                    sprite = new ConsoleSprite(ConsoleColor.Green, ConsoleColor.White, 'B');
                    fillingRatio = 0.0;
                    break;

                case ItemType.Door:
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Yellow, '/');
                    fillingRatio = 1.0;
                    break;

                case ItemType.LevelExit:
                    sprite = new ConsoleSprite(ConsoleColor.Blue, ConsoleColor.Black, 'E');
                    fillingRatio = 1.0;
                    break;

                case ItemType.Medikit:
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Red, '+');
                    fillingRatio = 0.0;
                    break;

                case ItemType.ToxicWaste:
                    sprite = new ConsoleSprite(ConsoleColor.DarkGreen, ConsoleColor.Green, ':');
                    fillingRatio = 0.0;
                    break;


                case ItemType.Wall:
                    sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Gray, '#');
                    fillingRatio = 1.0;
                    break;
            }
        }

        public void Interact()
        {
            switch (type)
            {
                case ItemType.Ammo:
                case ItemType.BFGCell:
                case ItemType.Medikit:
                    available = false;
                    break;

                case ItemType.Door:
                    if (fillingRatio == 1.0)
                    {
                        fillingRatio = 0.0;
                        sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.Yellow, '/');
                    }
                    else
                    {
                        fillingRatio = 1.0;
                        sprite = new ConsoleSprite(ConsoleColor.Gray, ConsoleColor.DarkYellow, '/');
                    }
                    break;
            }
        }
    }
}
