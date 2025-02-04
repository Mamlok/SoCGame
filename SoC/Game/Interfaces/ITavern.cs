using SoC.Entities.Model;
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

        public void TavernShop(Character character, int adventureNumber);

        public void NPCInteraction(Character character, int adventureNumber);

    }
}
