using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures.Models
{
    public class Lock
    {
        public bool Locked;
        public bool Attempted = false;
        public int KeyNumber; 
    }
}
