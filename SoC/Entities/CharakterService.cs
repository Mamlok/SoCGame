using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Entities
{
    public class CharacterService : ICharakterService
    {
        public Character LoadInitialCharacter()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var initialCharacter = new Character();

            if (File.Exists($"{basePath}\\Nettspend.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var InitialJsonFile = directory.GetFiles("Nettspend.json");

                using (StreamReader fi = File.OpenText(InitialJsonFile[0].FullName))
                {
                    initialCharacter = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                }
            }
            return initialCharacter;
        }
    }
}
