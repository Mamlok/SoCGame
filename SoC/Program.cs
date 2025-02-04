﻿


using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Game;
using SoC.Utilities;
using SoC.Utilities.Interfaces;
using System.IO;
using System.Net.Http.Headers;

namespace SoC
{
    public class Program
    {
        private static readonly AdventureService adventureService = new AdventureService();
        private static readonly ConsoleMessageHandler consoleMessageHandler = new ConsoleMessageHandler();
        private static readonly CharacterService characterService = new CharacterService(consoleMessageHandler);
        private static readonly CharakterInfo charakterInfo = new CharakterInfo(consoleMessageHandler, characterService);
        private static readonly CombatService combatService = new CombatService(consoleMessageHandler, charakterInfo);
        private static readonly Tavern tavern = new Tavern(consoleMessageHandler, characterService, charakterInfo);
        private static readonly LevelUp levelUp = new LevelUp(characterService, consoleMessageHandler);
        private static GameService gameService = new GameService(adventureService, characterService, consoleMessageHandler, combatService, tavern, charakterInfo, levelUp);
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            MainMenu();
        }

        public static void MakeTitle()
        {
            consoleMessageHandler.Clear();
            consoleMessageHandler.Write(@"  __   __           _____                     ");
            consoleMessageHandler.Write(@" |  \\/  |        / ____|                     ");
            consoleMessageHandler.Write(@" | \\  / |_   _  | |  __  __ _ _ __ ___   ___ ");
            consoleMessageHandler.Write(@" | |\\/| | | | | | | |_ |/ _` | '_ ` _ \\/ _\\");
            consoleMessageHandler.Write(@" | |  | | |_| | | |__| | (_| | | | | | |  __/");
            consoleMessageHandler.Write(@" |_|  |_|\__, |  \_____|\__,_|_| |_| |_|\___|");
            consoleMessageHandler.Write(@"          __/ |                              ");
            consoleMessageHandler.Write("         |___/                               ");
            consoleMessageHandler.Write("----------------------------------------------");
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
            consoleMessageHandler.Write("(C)reate a new character");
        }

        private static void CreateACharacter()
        {
            characterService.CreateCharacter();
        }


    }
}
