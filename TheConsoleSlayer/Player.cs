using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class Player
    {
        public Position Position { get; set; }
        public ConsoleSprite Sprite { get; }
        private double fillingRatio;
        private int health;
        private int ammo;
        private bool alive;
        private int combatPoints;
        private int sightRange;

        public Player(int x, int y)
        {
            Position = new Position(x, y);

            Sprite = new ConsoleSprite
                (
                ConsoleColor.Black,
                ConsoleColor.Green,
                'O'
                );

            fillingRatio = 0.4;

            health = 100;
            ammo = 10;
            alive = true;
            combatPoints = 0;
            sightRange = 8;
        }

        public double FillingRatio
        {
            get { return fillingRatio; }
        }

        public int Health
        {
            get { return health; }
            set
            {
                if (value >= 0 && value <= MaxHealt)
                {
                    health = value;
                }
            }
        }

        public int Ammo
        {
            get { return ammo; }
            set
            {
                if (value >= 0 && value <= MaxAmmo)
                {
                    ammo = value;
                }
            }
        }

        public bool Alive
        {
            get { return alive; }
        }
        public int CombatPoints
        {
            get { return combatPoints; }
        }

        public int MaxHealt
        {
            get { return 100 + CombatPoints / 10; }
        }

        public int MaxAmmo
        {
            get { return 10 + CombatPoints / 50; }
        }

        public int SightRange
        {
            get { return sightRange; }
        }

        public void Shoot()
        {
            if (ammo > 0)
            {
                Ammo--;
            }
        }

        public void AddCombatPoints(int points)
        {
            if (points > 0)
            {
                combatPoints += points;
            }
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
            {
                Health -= damage;

                if (Health == 0)
                {
                    alive = false;
                }
            }
        }
    }
}