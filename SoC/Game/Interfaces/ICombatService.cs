using SoC.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game.Interfaces
{
    public interface ICombatService
    {
        void RunCombat(ref Character character, List<Monster> monsters);
    }
}
