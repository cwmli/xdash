using System;

namespace Fly_Away
{
#if WINDOWS || XBOX
    static class mainProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (FlyAway game = new FlyAway())
            {
                game.Run();
            }
        }
    }
#endif
}

