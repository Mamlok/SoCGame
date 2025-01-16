


using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Game;
using System.IO;
using System.Net.Http.Headers;

namespace SoC
{
    public class Program
    {
        private static GameService gameService = new GameService();
        static void Main(string[] args)
        {
            MakeTitle();
            MainMenu();
        }

        private static void MakeTitle()
        {
            Console.WriteLine(@"  __   __           _____                     ");
            Console.WriteLine(@" |  \\/  |        / ____|                     ");
            Console.WriteLine(@" | \\  / |_   _  | |  __  __ _ _ __ ___   ___ ");
            Console.WriteLine(@" | |\\/| | | | | | | |_ |/ _` | '_ ` _ \\/ _\\");
            Console.WriteLine(@" | |  | | |_| | | |__| | (_| | | | | | |  __/");
            Console.WriteLine(@" |_|  |_|\__, |  \_____|\__,_|_| |_| |_|\___|");
            Console.WriteLine(@"          __/ |                              ");
            Console.WriteLine("         |___/                               \n\n");
        }


        private static void MainMenu()
        {
            MainMenuChoice();
            bool MainMenuInputValid = false;
            while (!MainMenuInputValid) 
            {
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
                        CreateACharacter();
                        MainMenuInputValid = true;
                        break;
                    default:
                        Console.WriteLine("Not the right one!!!");
                        MainMenuInputValid = false;
                        MainMenuChoice();
                        break;
                }

            }
        }

        private static void MainMenuChoice()
        {
            Console.WriteLine("(S)tart a new game");
            Console.WriteLine("(L)oad a game");
            Console.WriteLine("(C)reate a new character");
        }

        private static void CreateACharacter()
        {
            Console.WriteLine("create");
        }

        private static void LoadGame()
        {
            Console.WriteLine("load");
        }

    }
}
