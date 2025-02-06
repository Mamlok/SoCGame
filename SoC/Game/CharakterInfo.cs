using SoC.Entities.Interfaces;
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
    public class CharakterInfo : ICharakterInfo
    {
        private readonly IMessageHandler messageHandler;
        private readonly ICharakterService charakterService;
        private readonly IEquipWeapon equipWeapon;

        public CharakterInfo(IMessageHandler messageHandler, ICharakterService charakterService, IEquipWeapon equipWeapon)
        {
            this.messageHandler = messageHandler;
            this.charakterService = charakterService;
            this.equipWeapon = equipWeapon;
        }
        public void ShowCharakterInfo(Character character)
        {

            bool isRunning = true;
            while (isRunning)
            {
                ShowCharakterInfoBanner(character);
                var playerChoice = Console.ReadLine().ToLower();

                switch (playerChoice)
                {
                    case "i":
                        messageHandler.Clear();
                        ShowCharakterInventory(character);
                        ShowCharakterWeapons(character);
                        isRunning = true;
                        break;
                    case "e":
                        messageHandler.Clear();
                        equipWeapon.EquipWeaponMethod(character);
                        isRunning = true;
                        break;
                    case "b":
                        messageHandler.Clear();
                        isRunning = false;
                        break;
                    default:
                        messageHandler.Clear();
                        messageHandler.Write("Invalid option, please try again.");
                        isRunning = true;
                        break;
                }
            }
            
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

        private void ShowCharakterInventory(Character character)
        {
            messageHandler.Clear();
            messageHandler.Write("[INVENTORY]");
            if (character.Inventory.Count == 0)
            {
                messageHandler.Write("Your inventory is empty....");
            }
            else
            {
                
                foreach (var item in character.Inventory)
                {
                    messageHandler.Write("*********************************");
                    messageHandler.Write($"Name: {item.Description}");
                    messageHandler.Write($"Value: {item.GoldValue} Gold");
                    if (item.HealthValue > 0)
                    {
                        messageHandler.Write($"Healing value: {item.HealthValue} HP");
                    }
                }
                messageHandler.Write("*********************************");
            }
        }

        private void ShowCharakterInfoBanner(Character character)
        {
            messageHandler.Clear();
            messageHandler.Write("************************************************************");
            messageHandler.Write("                   HERE IS YOUR CHARACTER                   ");
            messageHandler.Write($"Name: {character.Name}");
            messageHandler.Write($"Class: {character.Class}");
            messageHandler.Write($"Background: {character.Background.ToString()}");
            messageHandler.Write($"HP: {character.HitPoints}");
            messageHandler.Write($"Level: {character.Level}");
            messageHandler.Write($"XP: {character.XP}");
            messageHandler.Write($"Gold: {character.Gold}");
            if (character.WeaponEquipped.Count > 0)
            {
                messageHandler.Write($"Weapon equiped: {character.WeaponEquipped[0].Description}");
            }
            else
            {
                messageHandler.Write("Weapon equiped: NONE");
            }
            WriteAbilities(character.Abilities);
            messageHandler.Write("************************************************************");
            messageHandler.Write("See character (I)nventory");
            messageHandler.Write("(E)quip weapon");
            messageHandler.Write("(B)ack");
        }

        private void ShowCharakterWeapons(Character character)
        {
            messageHandler.Write("[WEAPONS]");
            if (character.Weapons.Count == 0)
            {
                messageHandler.Write("Your weapons list is empty....");
            }
            else
            {
                var counter = 0;
                foreach (var item in character.Weapons)
                {
                    messageHandler.Write("*********************************");
                    messageHandler.Write($"Name:  {item.Description}");
                    messageHandler.Write($"Value: {item.GoldValue} Gold");
                    messageHandler.Write($"Damage: {item.DamageValue}");
                    counter++;
                }
                messageHandler.Write("*********************************");
            }
            messageHandler.Write("[ENTER to END]");
            messageHandler.Read();
        }
    }
}
