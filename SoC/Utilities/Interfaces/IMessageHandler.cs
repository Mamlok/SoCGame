using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Utilities.Interfaces
{
    public interface IMessageHandler
    {
        public void Write(string message = "", bool withLine = true);

        public string Read();
        //clear screen
        public void Clear();

    }
}
