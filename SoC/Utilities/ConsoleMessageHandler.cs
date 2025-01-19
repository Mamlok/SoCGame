using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Utilities
{
    public class ConsoleMessageHandler : IMessageHandler
    {
        public string Read()
        {
            return Console.ReadLine();
        }

        public void Write(string message = "", bool withLine = true)
        {
            if (withLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
        }

        public void Clear()
        {
            Console.Clear();
        }

    }
}
