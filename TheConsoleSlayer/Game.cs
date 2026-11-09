using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class Game
    {
        private List<GameItem> items;
        private List<Demon> demons;


        private static Random RNG = new Random();

        public List<GameItem> Items
        {
            get { return items; }
        }
        public List<Demon> Demons
        {
            get { return demons; }
        }

        private Player player;

        public Player Player { get; private set; }

        public bool Exited { get; set; }


        public Game()
        {
            Player = new Player(0, 0);
            items = new List<GameItem>();
            demons = new List<Demon>();

            demons.Add(new Demon(10, 5, DemonType.Zombieman));
            demons.Add(new Demon(15, 8, DemonType.Imp));
            demons.Add(new Demon(20, 10, DemonType.Mancubus));

            Exited = false;
        }

        private void RenderSingleSprite(Position position, ConsoleSprite sprite)
        {
            if (position.X >= 0 && position.X < Console.WindowWidth &&
                position.Y >= 0 && position.Y < Console.WindowHeight)
            {
                Console.SetCursorPosition(position.X, position.Y);

                Console.BackgroundColor = sprite.Backgroung;
                Console.ForegroundColor = sprite.Foregroung;

                Console.Write(sprite.Glyph);
            }
        }

        private void RenderGame()
        {
            Console.CursorVisible = false;
            Console.ResetColor();
            Console.Clear();

            foreach (GameItem item in Items)
            {
                RenderSingleSprite(item.Position, item.Sprite);
            }

            foreach (Demon demon in Demons)
            {
                RenderSingleSprite(demon.Position, demon.Sprite);
            }

            RenderSingleSprite(Player.Position, Player.Sprite);
        }

        private void UserAction()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo pressed = Console.ReadKey(true);

                switch (pressed.Key)
                {
                    case ConsoleKey.Escape:
                        Exited = true;
                        break;

                    case ConsoleKey.UpArrow:
                        Move(Player, Position.Add(Player.Position, new Position(0, -1)));
                        break;

                    case ConsoleKey.DownArrow:
                        Move(Player, Position.Add(Player.Position, new Position(0, 1)));
                        break;

                    case ConsoleKey.LeftArrow:
                        Move(Player, Position.Add(Player.Position, new Position(-1, 0)));
                        break;

                    case ConsoleKey.RightArrow:
                        Move(Player, Position.Add(Player.Position, new Position(1, 0)));
                        break;

                    case ConsoleKey.D:
                        foreach (GameItem item in Items)
                        {
                            if (item.Position.X == Player.Position.X &&
                                item.Position.Y == Player.Position.Y)
                            {
                                item.Interact();
                            }
                        }
                        break;
                }
            }
        }

        private void CleanUpGameItems()
        {
            List<GameItem> itemsToRemove = new List<GameItem>();

            foreach (GameItem item in Items)
            {
                if (!item.Available)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (GameItem item in itemsToRemove)
            {
                Items.Remove(item);
            }
        }

        private List<GameItem> GetGameItemsWithinDistance(Position position, double distance)
        {
            List<GameItem> result = new List<GameItem>();

            foreach (GameItem item in Items)
            {
                if (Position.Distance(position, item.Position) <= distance)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private double GetToFillingRatio(Position position)
        {
            double total = 0.0;

            List<GameItem> items = GetGameItemsWithinDistance(position, 0);
            foreach (GameItem item in items)
            {
                total += item.FillingRatio;
            }

            return total;
        }

        private void Move(Player player, Position targetPosition)
        {
            double totalFillingRatio = GetToFillingRatio(targetPosition) + player.FillingRatio;

            if (totalFillingRatio <= 1.0)
            {
                player.Position = targetPosition;
            }
        }

        //Demon mozgas


        private void Move(Demon demon, Position targetPosition)
        {
            double totalFillingRatio = GetToFillingRatio(targetPosition) + demon.FillingRatio;

            if (totalFillingRatio <= 1.0)
            {
                demon.Position = targetPosition;
            }
        }


        private void DemonMoveLogic(Demon demon)
        {
            int x = demon.Position.X + RNG.Next(-1, 2);
            int y = demon.Position.Y + RNG.Next(-1, 2);

            Position targetPosition = new Position(x, y);

            Move(demon, targetPosition);
        }

        private void UpdateDemons()
        {
            foreach (Demon demon in Demons)
            {
                demon.UpdateState(Player);

                switch (demon.State)
                {
                    case DemonStateType.Move:
                        DemonMoveLogic(demon);
                        break;

                    case DemonStateType.Attack:
                        // A támadást később valósítjuk meg.
                        break;

                    case DemonStateType.Idle:
                        // A démon jelenleg nem csinál semmit.
                        break;
                }
            }
        }

        private void CleanUpDemons()
        {
            List<Demon> deadDemons = new List<Demon>();

            foreach (Demon demon in Demons)
            {
                if (!demon.Alive)
                {
                    deadDemons.Add(demon);
                }
            }

            foreach (Demon demon in deadDemons)
            {
                Demons.Remove(demon);
            }
        }

        //Támadás

        private List<Demon> GetDemonsWithinDistance(Position position, double distance)
        {
            List<Demon> result = new List<Demon>();

            foreach (Demon demon in Demons)
            {
                if (Position.Distance(position, demon.Position) <= distance)
                {
                    result.Add(demon);
                }
            }
            return result;
        }

        public void PlayerAttackLogic()
        {
            if (Player.Ammo > 0)
            {
                Player.Shoot();

                List<Demon> targets =
                    GetDemonsWithinDistance(Player.Position, Player.SightRange);

                foreach (Demon demon in targets)
                {
                    double distance = Position.Distance(Player.Position, demon.Position);

                    int damage = 2 * RNG.Next(35, 106) / (1 + (int)distance);

                    demon.TakeDamage(damage);

                    if (!demon.Alive)
                    {
                        switch (demon.Type)
                        {
                            case DemonType.Zombieman:
                                Player.AddCombatPoints(1);
                                break;

                            case DemonType.Imp:
                                Player.AddCombatPoints(3);
                                break;

                            case DemonType.Mancubus:
                                Player.AddCombatPoints(10);
                                break;
                        }
                    }
                }
            }
        }

        public void Run()
        {
            while (!Exited)
            {
                UpdateDemons();
                CleanUpDemons();

                RenderGame();
                UserAction();
                CleanUpGameItems();

                Thread.Sleep(25);
            }

            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}
