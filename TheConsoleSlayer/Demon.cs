using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    public enum DemonType
    {
        Zombieman,
        Imp,
        Mancubus
    }

    public enum DemonStateType
    {
        Idle,
        Move,
        Attack
    }

    internal class Demon
    {
        private Position position;
        private ConsoleSprite sprite;
        private DemonType type;
        private double fillingRatio;
        private int health;
        private bool alive;
        private int sightRange;
        private int attackRange;
        private DemonStateType state;

        public Demon(Position position, ConsoleSprite sprite, DemonType type, double fillingRatio, int health, bool alive, int sightRange, int attackRange, DemonStateType state)
        {
            this.position = position;
            this.sprite = sprite;
            this.type = type;
            this.fillingRatio = fillingRatio;
            this.health = health;
            this.alive = alive;
            this.sightRange = sightRange;
            this.attackRange = attackRange;
            this.state = DemonStateType.Idle;
        }

        public Position Position
        {
            get { return position; }
            set { position = value; }
        }

        public ConsoleSprite Sprite
        {
            get { return sprite; }
        }

        public DemonType Type
        {
            get { return type; }
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
                if (value >= 0)
                {
                    health = value;
                }
            }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int SightRange
        {
            get { return sightRange; }
        }

        public int AttackRange
        {
            get { return attackRange; }
        }

        public DemonStateType State
        {
            get { return state; }
            set { state = value; }
        }

        public Demon(int x, int y, DemonType type)
        {
            position = new Position(x, y);
            this.type = type;
            alive = true;

            SetInitialPropertirs();
        }

        private void SetInitialPropertirs()
        {
            switch (type)
            {
                case DemonType.Zombieman:
                    fillingRatio = 0.4;
                    health = 20;
                    sightRange = 3;
                    attackRange = 1;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Green, 'o');
                    break;

                case DemonType.Imp:
                    fillingRatio = 0.4;
                    health = 60;
                    sightRange = 6;
                    attackRange = 3;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.Red, 'o');
                    break;

                case DemonType.Mancubus:
                    fillingRatio = 0.96;
                    health = 600;
                    sightRange = 9;
                    attackRange = 6;
                    sprite = new ConsoleSprite(ConsoleColor.Black, ConsoleColor.DarkRed, 'O');
                    break;
            }
        }
        public void UpdateState(Player player)
        {
            double distance = Position.Distance(this.Position, player.Position);

            if (distance < AttackRange)
            {
                State = DemonStateType.Attack;
            }
            else if (distance < SightRange)
            {
                State = DemonStateType.Move;
            }
            else
            {
                State = DemonStateType.Idle;
            }
        }

        public void TakeDamage(int damage)
        {
            if (damage > 0)
            {
                Health = Math.Max(0, Health - damage);

                if (Health == 0)
                {
                    alive = false;
                }
            }
        }

    }

}
