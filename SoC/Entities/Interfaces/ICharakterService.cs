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
        public Character LoadCharacter(string name);

        public List<Character> GetCharactersInRange();

        public bool SaveCharacter(Character character);

        public void CreateCharacter();

    }
}
