using System;

namespace Game2048
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new Game();
            game.Run();
        }
    }

    internal class Game
    {
        public ulong Score { get; private set; }
        public Board Board;
        private readonly int _nRows;
        private readonly int _nCols;
        public Color Color;

        public Game()
        {
            Board = new Board(4,4);
            _nRows = Board.NRows;
            _nCols = Board.NCols;
            Score = 0;
            Color = new Color();
        }

        public void Run()
        {
            var hasUpdated = true;
            do
            {
                if (hasUpdated)
                {
                    Board.PutNewValue();
                }

                Display();

                if (IsDead())
                {
                    using (new Color.ColorOutput(ConsoleColor.Red))
                    {
                        Console.WriteLine("YOU ARE DEAD!!!");
                        break;
                    }
                }

                Console.WriteLine("Use arrow keys to move the tiles. Press Ctrl-C to exit.");
                var input = Console.ReadKey(true); // BLOCKING TO WAIT FOR INPUT
                Console.WriteLine(input.Key.ToString());

                hasUpdated = InputHandler(input);
            }
            while (true); // use CTRL-C to break out of loop

            Console.WriteLine("Press any key to quit...");
            Console.Read();
        }

        private bool InputHandler(ConsoleKeyInfo input)
        {
            bool hasUpdated;
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    hasUpdated = Update(Direction.Up);
                    break;

                case ConsoleKey.DownArrow:
                    hasUpdated = Update(Direction.Down);
                    break;

                case ConsoleKey.LeftArrow:
                    hasUpdated = Update(Direction.Left);
                    break;

                case ConsoleKey.RightArrow:
                    hasUpdated = Update(Direction.Right);
                    break;

                default:
                    hasUpdated = false;
                    break;
            }

            return hasUpdated;
        }

        private bool Update(Direction dir)
        {
            ulong score;
            var isUpdated = Board.MoveHandler(Board, dir, out score);
            Score += score;
            return isUpdated;
        }

        private bool IsDead()
        {
            ulong score;
            foreach (var dir in new[] { Direction.Down, Direction.Up, Direction.Left, Direction.Right })
            {
                var clone = (Board)Board.Clone();
                if (Board.MoveHandler(clone, dir, out score))
                {
                    return false;
                }
            }
            return true;
        }

        private void Display()
        {
            Console.Clear();
            Console.WriteLine();
            for (var i = 0; i < _nRows; i++)
            {
                for (var j = 0; j < _nCols; j++)
                {
                    var numberColor = Color.ContainsKey(Board.GetBoardNumber(i, j)) ? Color.GetColor(Board.GetBoardNumber(i, j)) : ConsoleColor.Red;
                    using (new Color.ColorOutput(numberColor))
                    {
                        Console.Write($"{Board.GetBoardNumber(i, j),6}");
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Score: {0}", Score);
            Console.WriteLine();
        }
    }
}