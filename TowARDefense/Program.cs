using System;

namespace TowARDefense
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TowARDefense game = new TowARDefense())
            {
                game.Run();
            }
        }
    }
}

