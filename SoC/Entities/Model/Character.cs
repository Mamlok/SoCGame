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
        public int XP = 0;
        public Abilities Abilities = new Abilities();
        public string Background;
        public int InventoryWeight = 0;
        public List<Guid> AdventurePlayed = new List<Guid>();
        public CharacterClass Class;
        public string CauseOfDeath = "";
        public string DiedInAdventure = "";
    }
    

    public class Abilities
    {
        public int Strength;
        public int Dexterity;
        public int Intelligence;
        public int Wisdom;
    }

    public enum CharacterClass
    {
        Fighter,
        Thief,
        MagicUser,
        Healer
    }
}
