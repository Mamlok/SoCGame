using SoC.Items.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Entities.Model
{
    public class Character
    {
        public string Name;
        public int Level;
        public int Eddies;
        public string Background;
        public int InventoryWeight;
        public List<string> AdventurePlayed;
        public bool IsAlive;
        public int ArmorClass;
        public List<IItem> Inverntory;
        public int HitPoints;


    }

    public class Abilities
    {
        public int Strenght;
        public int Hax;
        public int Speed;
    }
}
