using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WMPLib;
using System.Threading;

namespace TheConsoleSlayer
{
    enum SoundEffectType
    {
        BFG,
        Door,
        ItemPickup,
        Pain,
        PlayerDeath,
        Shotgun
    }

    internal class Game
    {
        private List<GameItem> items;
        private List<Demon> demons;
        private ConsoleRenderer renderer;
        private GameLogic gameLogic;
        private Stopwatch stopwatchLogic;
        private Stopwatch stopwatchRenderer;

        private static Random RNG = new Random();

        private WindowsMediaPlayer MusicPlayer;
        private WindowsMediaPlayer SfxPlayer;


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

            MusicPlayer = new WindowsMediaPlayer();
            SfxPlayer = new WindowsMediaPlayer();

            /**
             * //példa
            demons.Add(new Demon(10, 5, DemonType.Zombieman));
            demons.Add(new Demon(15, 8, DemonType.Imp));
            demons.Add(new Demon(20, 10, DemonType.Mancubus));
             */


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
                        gameLogic.PlayerDirectInteractLogic();
                        break;

                    case ConsoleKey.F:
                        gameLogic.PlayerAttackLogic();
                        break;

                    case ConsoleKey.S:
                        gameLogic.PlayerBFGAttackLogic();
                        break;
                }
            }

            gameLogic.PlayerIndirectInteractionLogic();
        }

        public void LoadMapFromPlainText(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            string[] dimensions = lines[0].Split(',');

            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);

            for (int y = 0; y < rows; y++)
            {
                string line = lines[y + 1];

                for (int x = 0; x < columns; x++)
                {
                    char symbol = line[x];

                    switch (symbol)
                    {
                        case 'A':
                            items.Add(new GameItem(x, y, ItemType.Ammo));
                            break;

                        case 'B':
                            items.Add(new GameItem(x, y, ItemType.BFGCell));
                            break;

                        case 'D':
                            items.Add(new GameItem(x, y, ItemType.Door));
                            break;

                        case 'E':
                            items.Add(new GameItem(x, y, ItemType.LevelExit));
                            break;

                        case 'M':
                            items.Add(new GameItem(x, y, ItemType.Medikit));
                            break;

                        case 'T':
                            items.Add(new GameItem(x, y, ItemType.ToxicWaste));
                            break;

                        case 'W':
                            items.Add(new GameItem(x, y, ItemType.Wall));
                            break;

                        case 'z':
                            demons.Add(new Demon(x, y, DemonType.Zombieman));
                            break;

                        case 'i':
                            demons.Add(new Demon(x, y, DemonType.Imp));
                            break;

                        case 'm':
                            demons.Add(new Demon(x, y, DemonType.Mancubus));
                            break;

                        case 'p':
                            Player = new Player(x, y);
                            break;

                        case '_':
                            //üres
                            break;
                    }
                }
            }
        }

        public void PlaySoundEffect(SoundEffectType effectType)
        {
            switch (effectType)
            {
                case SoundEffectType.BFG:
                    SfxPlayer.URL = "sound/bfg.mp3";
                    break;

                case SoundEffectType.Door:
                    SfxPlayer.URL = "sound/door.mp3";
                    break;

                case SoundEffectType.ItemPickup:
                    SfxPlayer.URL = "sound/item_pickup.mp3";
                    break;

                case SoundEffectType.Pain:
                    SfxPlayer.URL = "sound/pain.mp3";
                    break;

                case SoundEffectType.PlayerDeath:
                    SfxPlayer.URL = "sound/player_death.mp3";
                    break;

                case SoundEffectType.Shotgun:
                    SfxPlayer.URL = "sound/shotgun.mp3";
                    break;
            }

            SfxPlayer.controls.play();
        }
        
        public void PlayMusic(string filePath)
        {
            MusicPlayer.URL = filePath;
            MusicPlayer.controls.play();
        }

        public void Run()
        {
            PlayMusic("sound/music.mp3");

            stopwatchLogic.Start();
            stopwatchRenderer.Start();

            while (!Exited && Player.Alive)
            {
                if (stopwatchLogic.ElapsedMilliseconds > 500)
                {
                     long deltaTime = stopwatchLogic.ElapsedMilliseconds;

                    gameLogic.UpdateGameState(deltaTime);

                    stopwatchLogic.Restart();
                }
                if (stopwatchRenderer.ElapsedMilliseconds > 25)
                {
                    renderer.RenderGame();
                    stopwatchRenderer.Restart();
                }

                UserAction();
            }

            if (Exited)
            {
                PlayMusic("sound/victory.mp3");
                Console.WriteLine("YOU WIN!");

            }
            else if (!Player.Alive)
            {
                Console.Clear();
                Console.WriteLine("YOU DIED!");

                PlaySoundEffect(SoundEffectType.PlayerDeath);

                Thread.Sleep(3000);
            }

            Console.ResetColor();
            Console.CursorVisible = true;

            Console.ReadLine();
        }
    }
}