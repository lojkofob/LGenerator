using System;

namespace L_generator
{
#if WINDOWS || XBOX
    static class A
    {
        static public Game1 _g = new Game1();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
           _g.Run();
        }
    }
#endif
}

