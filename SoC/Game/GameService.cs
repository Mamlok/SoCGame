using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Adventures.Interfaces;
using SoC.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class GameService
    {
        private IAdventureService adventureService;
        private ICharakterService charakterService;
        public GameService(IAdventureService AdventureService, ICharakterService CharakterService) 
        {
            adventureService = AdventureService;
            charakterService = CharakterService;
        }
        public void StartGame() 
        {
            var initialAdventure = adventureService.GetInitialAdventure();
            var initialCharacter = charakterService.LoadInitialCharacter();

            Console.WriteLine($"Adventure : {initialAdventure.Title}");
            Console.WriteLine($"Adventure : {initialAdventure.Description}");

            Console.WriteLine($"Character name : {initialCharacter.Name}");
            Console.WriteLine($"Level : {initialCharacter.Level}");
        }
    }
}
