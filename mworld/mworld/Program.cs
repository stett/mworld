using System;

namespace mworld
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MWorld game = new MWorld())
            {
                game.Run();
            }
        }
    }
#endif
}

