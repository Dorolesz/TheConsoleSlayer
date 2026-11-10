using System;
using System.Collections.Generic;
using System.Text;

namespace TheConsoleSlayer
{
    internal class ConsoleRenderer
    {
        private Game game;

        public ConsoleRenderer(Game game)
        {
            this.game = game;
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

        public void RenderGame()
        {
            Console.CursorVisible = false;
            Console.ResetColor();
            Console.Clear();

            foreach (GameItem item in game.Items)
            {
                double distance = Position.Distance(
                game.Player.Position,
                item.Position
            );

                if (distance <= game.Player.SightRange)
                {
                    RenderSingleSprite(item.Position, item.Sprite);
                }
            }

            foreach (Demon demon in game.Demons)
            {
                double distance = Position.Distance(
                game.Player.Position,
                demon.Position
            );

                if (distance <= game.Player.SightRange)
                {
                    RenderSingleSprite(demon.Position, demon.Sprite);
                }
            }

            RenderSingleSprite(game.Player.Position, game.Player.Sprite);

            RenderHUD();
        }

        private void RenderHUD()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;

            string hud = $" HP: {game.Player.Health}/{game.Player.MaxHealth} " +
                 $"| Ammo: {game.Player.Ammo}/{game.Player.MaxAmmo} " +
                 $"| BFG: {game.Player.BFGCells}/{game.Player.MaxBFGCells} " +
                 $"| XP: {game.Player.CombatPoints} ";

            Console.Write(hud.PadRight(Console.WindowWidth));
            Console.ResetColor();
        }
    }
}
