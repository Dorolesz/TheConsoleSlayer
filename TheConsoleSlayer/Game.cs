using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TheConsoleSlayer
{
    internal class Game
    {
        private List<GameItem> items;
        private List<Demon> demons;
        private ConsoleRenderer renderer;
        private GameLogic gameLogic;
        private Stopwatch stopwatchLogic;
        private Stopwatch stopwatchRenderer;

        private static Random RNG = new Random();

        public List<GameItem> Items
        {
            get { return items; }
        }
        public List<Demon> Demons
        {
            get { return demons; }
        }

        public Player Player { get; private set; }

        public bool Exited { get; set; }


        public Game()
        {
            Player = new Player(0, 0);

            items = new List<GameItem>();
            demons = new List<Demon>();

            renderer = new ConsoleRenderer(this);
            gameLogic = new GameLogic(this);

            stopwatchLogic = new Stopwatch();
            stopwatchRenderer = new Stopwatch();

            //példa
            demons.Add(new Demon(10, 5, DemonType.Zombieman));
            demons.Add(new Demon(15, 8, DemonType.Imp));
            demons.Add(new Demon(20, 10, DemonType.Mancubus));

            Exited = false;
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
                        gameLogic.Move(Player, Position.Add(Player.Position, new Position(0, -1)));
                        break;

                    case ConsoleKey.DownArrow:
                        gameLogic.Move(Player, Position.Add(Player.Position, new Position(0, 1)));
                        break;

                    case ConsoleKey.LeftArrow:
                        gameLogic.Move(Player, Position.Add(Player.Position, new Position(-1, 0)));
                        break;

                    case ConsoleKey.RightArrow:
                        gameLogic.Move(Player, Position.Add(Player.Position, new Position(1, 0)));
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

                    case ConsoleKey.F:
                        gameLogic.PlayerAttackLogic();
                        break;
                }
            }
        }


        public void Run()
        {
            stopwatchLogic.Start();
            stopwatchRenderer.Start();

            while (!Exited && Player.Alive)
            {
                if (stopwatchLogic.ElapsedMilliseconds > 500)
                {
                    gameLogic.UpdateGameState();
                    stopwatchLogic.Restart();
                }
                if (stopwatchRenderer.ElapsedMilliseconds > 25)
                {
                    renderer.RenderGame();
                    stopwatchRenderer.Restart();
                }

                UserAction();
            }

            Console.ResetColor();
            Console.CursorVisible = true;
        }
    }
}