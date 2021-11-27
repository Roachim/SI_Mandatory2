using System;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Task.Run(async () => await Client.MakeRequestAsync()).Wait();
            }
        }
    }
}
