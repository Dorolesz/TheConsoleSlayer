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
                RenderSingleSprite(item.Position, item.Sprite);
            }

            foreach (Demon demon in game.Demons)
            {
                RenderSingleSprite(demon.Position, demon.Sprite);
            }

            RenderSingleSprite(game.Player.Position, game.Player.Sprite);
        }

    }
}
