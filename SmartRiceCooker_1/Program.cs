using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SmartRiceCooker_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 40);
            Random random = new Random();
            ConsoleColor[] Color = { ConsoleColor.Blue, ConsoleColor.Cyan,
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green,
                ConsoleColor.Magenta, ConsoleColor.Gray };

            while (true)
            {
                Console.Clear();
                for(int i = 0; i < 30; i++)
                {
                    Console.ForegroundColor = Color[random.Next(7)];
                    Console.SetCursorPosition(random.Next(100), random.Next(40));
                    Console.Write("Hello World!");
                }
                Thread.Sleep(200);
            }
        }
    }
}
