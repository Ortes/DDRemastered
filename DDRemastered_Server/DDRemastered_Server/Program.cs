using System;
using System.Threading;

namespace DDRemastered_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Thread(new Server(65174).start).Start();
        }
    }
}
