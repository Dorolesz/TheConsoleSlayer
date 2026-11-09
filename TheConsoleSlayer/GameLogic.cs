using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class GameLogic
    {
        private Game game;
        private static Random RNG = new Random();

        public GameLogic(Game game)
        {
            this.game = game;
        }

        public void CleanUpGameItems()
        {
            List<GameItem> itemsToRemove = new List<GameItem>();

            foreach (GameItem item in game.Items)
            {
                if (!item.Available)
                {
                    itemsToRemove.Add(item);
                }
            }

            foreach (GameItem item in itemsToRemove)
            {
                game.Items.Remove(item);
            }
        }

        public void CleanUpDemons()
        {
            List<Demon> deadDemons = new List<Demon>();

            foreach (Demon demon in game.Demons)
            {
                if (!demon.Alive)
                {
                    deadDemons.Add(demon);
                }
            }

            foreach (Demon demon in deadDemons)
            {
                game.Demons.Remove(demon);
            }
        }

        private void DemonMoveLogic(Demon demon)
        {
            int x = demon.Position.X + RNG.Next(-1, 2);
            int y = demon.Position.Y + RNG.Next(-1, 2);

            Position targetPosition = new Position(x, y);

            Move(demon, targetPosition);
        }

        //Démonok támadása
        private void DemonAttackLogic(Demon demon)
        {
            double distance = Position.Distance(
                demon.Position,
                game.Player.Position
            );

            int damage = (int)(2 * RNG.Next(35, 106) / (1 + distance));

            game.Player.TakeDamage(damage);
        }

        private List<GameItem> GetGameItemsWithinDistance(Position position, double distance)
        {
            List<GameItem> result = new List<GameItem>();

            foreach (GameItem item in game.Items)
            {
                if (Position.Distance(position, item.Position) <= distance)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        private List<Demon> GetDemonsWithinDistance(Position position, double distance)
        {
            List<Demon> result = new List<Demon>();

            foreach (Demon demon in game.Demons)
            {
                if (Position.Distance(position, demon.Position) <= distance)
                {
                    result.Add(demon);
                }
            }
            return result;
        }

        private double GetTotalFillingRatio(Position position)
        {
            double total = 0.0;

            List<GameItem> items = GetGameItemsWithinDistance(position, 0);
            foreach (GameItem item in items)
            {
                total += item.FillingRatio;
            }

            return total;
        }

        //Jatekos mozgas
        public void Move(Player player, Position targetPosition)
        {
            double totalFillingRatio = GetTotalFillingRatio(targetPosition) + player.FillingRatio;

            if (totalFillingRatio <= 1.0)
            {
                player.Position = targetPosition;
            }
        }

        //Demon mozgas
        public void Move(Demon demon, Position targetPosition)
        {
            double totalFillingRatio = GetTotalFillingRatio(targetPosition) + demon.FillingRatio;

            if (totalFillingRatio <= 1.0)
            {
                demon.Position = targetPosition;
            }
        }

        public void PlayerAttackLogic()
        {
            if (game.Player.Ammo > 0)
            {
                game.Player.Shoot();

                List<Demon> targets =
                    GetDemonsWithinDistance(game.Player.Position, game.Player.SightRange);

                foreach (Demon demon in targets)
                {
                    double distance = Position.Distance(game.Player.Position, demon.Position);

                    int damage = 2 * RNG.Next(35, 106) / (1 + (int)distance);

                    demon.TakeDamage(damage);

                    bool wasAlive = demon.Alive;

                    demon.TakeDamage(damage);

                    if (wasAlive && !demon.Alive)
                    {
                        switch (demon.Type)
                        {
                            case DemonType.Zombieman:
                                game.Player.AddCombatPoints(1);
                                break;

                            case DemonType.Imp:
                                game.Player.AddCombatPoints(3);
                                break;

                            case DemonType.Mancubus:
                                game.Player.AddCombatPoints(10);
                                break;
                        }
                    }
                }
            }
        }

        public void UpdateDemons()
        {
            foreach (Demon demon in game.Demons)
            {
                demon.UpdateState(game.Player);

                switch (demon.State)
                {
                    case DemonStateType.Move:
                        DemonMoveLogic(demon);
                        break;

                    case DemonStateType.Attack:
                        DemonAttackLogic(demon);
                        break;

                    case DemonStateType.Idle:
                        // A démon jelenleg nem csinál semmit.
                        break;
                }
            }
        }

        public void UpdateGameState()
        {
            UpdateDemons();
            CleanUpGameItems();
            CleanUpDemons();
        }
    }
}
