using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items.Interfaces
{
    public interface IArmorService
    {
        public List<Armor> GetArmor();
    }
}
