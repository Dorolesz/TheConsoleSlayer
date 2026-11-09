using static TheConsoleSlayer.GameItem;

namespace TheConsoleSlayer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();

            game.Items.Add(new GameItem(3, 3, ItemType.Door));
            game.Items.Add(new GameItem(5, 3, ItemType.Medikit));
            game.Items.Add(new GameItem(7, 3, ItemType.Ammo));
            game.Items.Add(new GameItem(9, 3, ItemType.Wall));

            Demon demon = new Demon(5, 5, DemonType.Zombieman);
            Demon demon2 = new Demon(5, 5, DemonType.Imp);
            Demon demon3 = new Demon(5, 5, DemonType.Mancubus);

            Console.WriteLine(demon.Health);
            Console.WriteLine(demon.FillingRatio);
            Console.WriteLine(demon.SightRange);
            Console.WriteLine(demon.AttackRange);

            game.Run();
        }
    }
}
