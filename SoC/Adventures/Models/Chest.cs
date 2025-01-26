using SoC.Items.Interfaces;
using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures.Models
{
    public class Chest
    {
        public Lock Lock;
        public Trap Trap;
        public List<Item> Treasure;
        public int Gold;
    }
}
