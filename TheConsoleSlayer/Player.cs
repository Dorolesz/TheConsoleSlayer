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
        public int BFGCells { get; private set; }

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
                if (value >= 0 && value <= MaxHealth)
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

        public int MaxHealth
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

        public int MaxBFGCells
        {
            get { return 3; }
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
            if (damage > 0 && alive)
            {
                health = Math.Max(0, health - damage);

                if (health == 0)
                {
                    alive = false;
                }
            }
        }

        public void PickUpAmmo(int amount)
        {
            Ammo = Math.Min(Ammo + amount, MaxAmmo);
        }

        public void PickUpHealth(int amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
        }

        public void PickUpBFGCell(int amount)
        {
            BFGCells = Math.Min(BFGCells + amount, MaxBFGCells);
        }

        public void ShootBFG()
        {
            if (BFGCells > 0)
            {
                BFGCells--;
            }
        }
    }
}