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

        private void DemonMoveLogic(Demon demon, long deltaTime)
        {
            int x = demon.Position.X + RNG.Next(-1, 2);
            int y = demon.Position.Y + RNG.Next(-1, 2);

            Position targetPosition = new Position(x, y);

            Move(demon, targetPosition, deltaTime);
        }

        //Démonok támadása
        private void DemonAttackLogic(Demon demon, long deltaTime)
        {
            double distance = Position.Distance(
                demon.Position,
                game.Player.Position
            );

            double damage = (int)(2 * RNG.Next(35, 106) / (1 + distance)) * (deltaTime / 1000.0);

            game.Player.TakeDamage((int)damage);
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
        public void Move(Demon demon, Position targetPosition, long deltaTime)
        {
            double pMove = (demon.Speed / 100.0) * (deltaTime / 1000.0);

            double randomValue = RNG.NextDouble();

            if (randomValue < pMove)
            {
                double totalFillingRatio = GetTotalFillingRatio(targetPosition) + demon.FillingRatio;

                if (totalFillingRatio <= 1.0)
                {
                    demon.Position = targetPosition;
                }
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

        public void UpdateDemons(long deltaTime)
        {
            foreach (Demon demon in game.Demons)
            {
                demon.UpdateState(game.Player);

                switch (demon.State)
                {
                    case DemonStateType.Move:
                        DemonMoveLogic(demon, deltaTime);
                        break;

                    case DemonStateType.Attack:
                        DemonAttackLogic(demon, deltaTime);
                        break;

                    case DemonStateType.Idle:
                        // A démon jelenleg nem csinál semmit.
                        break;
                }

                DemonIndirectInteractionLogic(demon);
            }

        }
        public void PlayerDirectInteractLogic()
        {
            List<GameItem> itemsInRange = GetGameItemsWithinDistance(game.Player.Position, 1);
            foreach (GameItem item in itemsInRange)
            {
                switch (item.Type)
                {
                    case ItemType.Door:
                        item.Interact();
                        game.PlaySoundEffect(SoundEffectType.Door);
                        break;

                    case ItemType.LevelExit:
                        item.Interact();
                        game.PlaySoundEffect(SoundEffectType.Door);
                        game.Exited = true;
                        break;
                }

            }
        }

        public void PlayerIndirectInteractionLogic()
        {
            List<GameItem> items = GetGameItemsWithinDistance(game.Player.Position, 0);

            foreach (GameItem item in items)
            {
                switch (item.Type)
                {
                    case ItemType.Ammo:
                        item.Interact();
                        game.Player.PickUpAmmo(5);
                        game.PlaySoundEffect(SoundEffectType.ItemPickup);
                        break;

                    case ItemType.Medikit:
                        item.Interact();
                        game.Player.PickUpHealth(25);
                        game.PlaySoundEffect(SoundEffectType.ItemPickup);
                        break;

                    case ItemType.ToxicWaste:
                        item.Interact();
                        game.Player.TakeDamage(5);
                        game.PlaySoundEffect(SoundEffectType.Pain);
                        break;

                    case ItemType.BFGCell:
                        if (game.Player.BFGCells < game.Player.MaxBFGCells)
                        {
                            item.Interact();
                            game.Player.PickUpBFGCell(1);
                            game.PlaySoundEffect(SoundEffectType.ItemPickup);
                        }
                        break;
                }
            }

            game.PlaySoundEffect(SoundEffectType.Door);
        }

        private void DemonIndirectInteractionLogic(Demon demon)
        {
            List<GameItem> items = GetGameItemsWithinDistance(demon.Position, 0);

            foreach (GameItem item in items)
            {
                if (item.Type == ItemType.ToxicWaste)
                {
                    demon.TakeDamage(5);
                }
            }

            game.PlaySoundEffect(SoundEffectType.Door);
        }

        public void PlayerBFGAttackLogic()
        {
            if (game.Player.BFGCells > 0)
            {
                game.Player.ShootBFG();

                game.PlaySoundEffect(SoundEffectType.BFG);

                List<Demon> targets =
                    GetDemonsWithinDistance(
                        game.Player.Position,
                        game.Player.SightRange
                    );

                foreach (Demon demon in targets)
                {
                    bool wasAlive = demon.Alive;

                    int damage = RNG.Next(100, 801);

                    demon.TakeDamage(damage);

                    if (wasAlive && !demon.Alive)
                    {
                        game.Player.AddCombatPoints(1);
                    }
                }
            }
        }

        public void UpdateGameState(long deltaTime)
        {
            UpdateDemons(deltaTime);
            CleanUpGameItems();
            CleanUpDemons();
        }
    }
}
