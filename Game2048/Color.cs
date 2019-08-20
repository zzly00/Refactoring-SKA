using System;
using System.Collections.Generic;

namespace Game2048
{
    public class Color
    {
        private readonly Dictionary<ulong, ConsoleColor> _color;

        public Color()
        {
            _color = new Dictionary<ulong, ConsoleColor>
            {
                { 0, ConsoleColor.DarkGray },
                { 2, ConsoleColor.Cyan },
                { 4, ConsoleColor.Magenta },
                { 8, ConsoleColor.Red },
                { 16, ConsoleColor.Green },
                { 32, ConsoleColor.Yellow },
                { 64, ConsoleColor.Yellow },
                { 128, ConsoleColor.DarkCyan },
                { 256, ConsoleColor.Cyan },
                { 512, ConsoleColor.DarkMagenta },
                { 1024, ConsoleColor.Magenta }
            };
        }

        public ConsoleColor GetColor(ulong number)
        {
            return _color[number];
        }

        public bool ContainsKey(ulong number)
        {
            return _color.ContainsKey(number);
        }

        public class ColorOutput : IDisposable
        {
            public ColorOutput(ConsoleColor fg, ConsoleColor bg = ConsoleColor.Black)
            {
                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
            }

            public void Dispose()
            {
                Console.ResetColor();
            }
        }
    }
}