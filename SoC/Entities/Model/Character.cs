using SoC.Items.Interfaces;
using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Entities.Model
{
    public class Character : Entity
    {
        public string Name;
        public int Level;
        public int Gold;
        public Abilities Abilities;
        public string Background;
        public int InventoryWeight;
        public List<string> AdventurePlayed;
        public bool IsAlive;
        public int ArmorClass;
        public List<Item> Inventory;
        public CharacterClass Class;


    }

    public class Abilities
    {
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;
        public int Charisma;
    }

    public enum CharacterClass
    {
        Fighter,
        Thief,
        MagicUser,
        Healer
    }
}
