using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Adventures.Interfaces;
using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class GameService : IGameService
    {
        private IAdventureService adventureService;
        private ICharakterService characterService;
        private Character character;

        public GameService(IAdventureService AdventureService, ICharakterService CharakterService)
        {
            adventureService = AdventureService;
            characterService = CharakterService;
        }
        public bool StartGame(Adventure adventure = null)
        {

            try
            {
                if (adventure == null)
                {
                    adventure = adventureService.GetInitialAdventure();
                }
                var initialAdventure = adventureService.GetInitialAdventure();

                Console.Clear();
                Console.WriteLine();
                //Banner
                for (int i = 0; i <= adventure.Title.Length + 1; i++)
                {
                    Console.Write("*");
                    if (i == adventure.Title.Length + 1)
                    {
                        Console.Write("\n");
                    }
                }
                Console.WriteLine($"│{adventure.Title}│");
                for (int i = 0; i <= adventure.Title.Length + 1; i++)
                {
                    Console.Write("*");
                    if (i == adventure.Title.Length + 1)
                    {
                        Console.Write("\n");
                    }
                }

                Console.WriteLine($"\n{adventure.Description}");

                var charactersInRange = characterService.GetCharactersInRange(adventure.MinLevel, adventure.MaxLevel);

                if (charactersInRange.Count == 0)
                {
                    Console.WriteLine("no enough level");
                    return false;
                }
                else
                {
                    Console.WriteLine("pick character:");
                    var characterCount = 0;
                    foreach (var character in charactersInRange)
                    {
                        Console.WriteLine($"{characterCount}. {character.Name} Level - {character.Level} Class: {character.Class}");
                        characterCount++;
                    }
                }
                character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(Console.ReadLine())].Name);

                Monster myMonster = new Monster(); //dont need


            }
            catch (Exception ex)
            {

                Console.WriteLine($"Something went wrong {ex.Message}");
            }
            return true;

        }
    }
}
