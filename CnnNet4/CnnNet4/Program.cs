using System;

namespace CnnNet4
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameCnnNet game = new GameCnnNet())
            {
                game.Run();
            }
        }
    }
#endif
}

