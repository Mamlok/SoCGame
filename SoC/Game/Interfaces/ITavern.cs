using SoC.Entities.Model;
using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game.Interfaces
{
    public interface ITavern
    {
        public void TavernMenu(Character character, int adventureNumber);

        public void TavernShop(Character character, int adventureNumber, List<Item> Items, List<Weapon> Weapons, List<Armor> Armors);

        public void NPCInteraction(Character character, int adventureNumber);

    }
}
