using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items.Models
{
    public class Weapon
    {
        public WeaponType Name;
        public string Description;
        public int ObjectiveNumber;
        public int GoldValue;
        public int DamageValue;
        public int MagicValue;

        public enum WeaponType
        {
            Sword,
            MagicStaff,
            Dagger,
            Rapier
        }
    }
}
