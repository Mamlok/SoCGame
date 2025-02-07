using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items.Models
{
    public class Armor
    {
        public ArmorType Name;
        public string Description;
        public int ObjectiveNumber;
        public int GoldValue;
        public int ArmorValue;

        public enum ArmorType
        {
            Heavy,
            Medium,
            Robe,
            Light
        }
    }
}
