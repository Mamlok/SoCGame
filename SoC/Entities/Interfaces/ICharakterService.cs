using SoC.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Entities.Interfaces
{
    public interface ICharakterService
    {
        public Character LoadInitialCharacter();

    }
}
