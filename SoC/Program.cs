


using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Game;
using SoC.Utilities;
using System.IO;
using System.Net.Http.Headers;

namespace SoC
{
    public class Program
    {
        private static readonly AdventureService adventureService = new AdventureService();
        private static readonly ConsoleMessageHandler consoleMessageHandler = new ConsoleMessageHandler();
        private static readonly CharacterService characterService = new CharacterService(consoleMessageHandler);
        private static readonly CombatService combatService = new CombatService(consoleMessageHandler);
        private static GameService gameService = new GameService(adventureService, characterService, consoleMessageHandler, combatService);
        static void Main(string[] args)
        {
            MainMenu();
        }

        private static void MakeTitle()
        {
            consoleMessageHandler.Write(@"  __   __           _____                     ");
            consoleMessageHandler.Write(@" |  \\/  |        / ____|                     ");
            consoleMessageHandler.Write(@" | \\  / |_   _  | |  __  __ _ _ __ ___   ___ ");
            consoleMessageHandler.Write(@" | |\\/| | | | | | | |_ |/ _` | '_ ` _ \\/ _\\");
            consoleMessageHandler.Write(@" | |  | | |_| | | |__| | (_| | | | | | |  __/");
            consoleMessageHandler.Write(@" |_|  |_|\__, |  \_____|\__,_|_| |_| |_|\___|");
            consoleMessageHandler.Write(@"          __/ |                              ");
            consoleMessageHandler.Write("         |___/                               \n\n");
        }


        public static void MainMenu()
        {
            bool MainMenuInputValid = false;
            while (!MainMenuInputValid) 
            {
                MakeTitle();
                MainMenuChoice();
                switch (Console.ReadLine().ToLower())
                {
                    case "s":
                        gameService.StartGame();
                        MainMenuInputValid = true;
                        break;
                    case "l":
                        LoadGame();
                        MainMenuInputValid = true;
                        break;
                    case "c":
                        characterService.CreateCharacter();
                        MainMenuInputValid = true;
                        break;
                    default:
                        consoleMessageHandler.Write("Please enter a valid option.");
                        Thread.Sleep(1000);
                        MainMenuInputValid = false;
                        consoleMessageHandler.Clear();
                        continue;
                }

            }
        }

        private static void MainMenuChoice()
        {
            consoleMessageHandler.Write("(S)tart a new game");
            consoleMessageHandler.Write("(L)oad a game");
            consoleMessageHandler.Write("(C)reate a new character");
        }

        private static void CreateACharacter()
        {
            characterService.CreateCharacter();
        }

        private static void LoadGame()
        {
            consoleMessageHandler.Write("load");
        }

    }
}
