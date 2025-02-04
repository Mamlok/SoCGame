using SoC.Entities;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class LevelUp : ILevelUp
    {
        private readonly CharacterService characterService;
        private readonly IMessageHandler messageHandler;


        public LevelUp(CharacterService characterService, IMessageHandler messageHandler)
        {
            this.characterService = characterService;
            this.messageHandler = messageHandler;
        }
        public void LevelUpCharacter(Character charakter)
        {
            var isRunning = true;
            while (isRunning)
            {
                LevelUpTextFirst(charakter);
                var playerChoice = Console.ReadLine().ToLower();
                switch (playerChoice)
                {
                    case "s":
                        charakter.Abilities.Strength++;
                        isRunning = false;
                        charakter.XP -= charakter.Level - 250;
                        charakter.Level++;
                        break;
                    case "d":
                        charakter.Abilities.Dexterity++;
                        isRunning = false;
                        charakter.XP -= charakter.Level - 250;
                        charakter.Level++;
                        break;
                    case "i":
                        charakter.Abilities.Intelligence++;
                        isRunning = false;
                        charakter.XP -= charakter.Level - 250;
                        charakter.Level++;
                        break;
                    case "w":
                        charakter.Abilities.Wisdom++;
                        isRunning = false;
                        charakter.XP -= charakter.Level - 250;
                        charakter.Level++;
                        break;
                    case "b":
                        isRunning = false;
                        break;
                    default:
                        messageHandler.Write("Invalid option, please try again.");
                        isRunning = true;
                        break;
                }
            }
            LevelUpTextSecond(charakter);
        }

        private void WriteAbilities(Abilities abilities)
        {
            messageHandler.Write("****************** ABILITIES *****************");
            Console.ForegroundColor = ConsoleColor.Red;
            messageHandler.Write($"Strength:                                    {abilities.Strength}");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            messageHandler.Write($"Dexterity:                                   {abilities.Dexterity}");
            Console.ForegroundColor = ConsoleColor.Blue;
            messageHandler.Write($"Intelligence:                                {abilities.Intelligence}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            messageHandler.Write($"Wisdom:                                      {abilities.Wisdom}");
            Console.ForegroundColor = ConsoleColor.White;
            messageHandler.Write("**********************************************");
        }

        private void LevelUpTextFirst(Character charakter)
        {
            messageHandler.Clear();
            messageHandler.Write("*************************************************");
            messageHandler.Write("*     CONGRATULATIONS! YOU HAVE LEVELED UP!     *");
            messageHandler.Write("*************************************************");
            WriteAbilities(charakter.Abilities);
            messageHandler.Write("What do you want to level up:");
            messageHandler.Write("(S)trength");
            messageHandler.Write("(I)exterity");
            messageHandler.Write("(I)ntelligence");
            messageHandler.Write("(W)isdomh");
            messageHandler.Write("(B)ack");
        }

        private void LevelUpTextSecond(Character charakter)
        {
            messageHandler.Clear();
            messageHandler.Write("*************************************************************");
            messageHandler.Write("*               ABILITIES SUCCESSFULLY ADDED!               *");
            messageHandler.Write("*************************************************************");
            WriteAbilities(charakter.Abilities);
            messageHandler.Write("[ENTER TO END]");
            messageHandler.Read();
            messageHandler.Clear();
        }
    }
}
