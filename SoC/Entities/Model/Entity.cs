using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Entities.Model
{
    public abstract class Entity
    {
        public int HitPoints = 0;
        public Attack Attack;
        public int Gold;
        public int Level = 1;
        public bool IsAlive = true;
        public int ArmorClass;
        public List<Item> Inventory;
        public List<Weapon> Weapons;
    }

    public class Attack
    {
        public int BaseDie;
        public int BonusDamage;
    }
}
