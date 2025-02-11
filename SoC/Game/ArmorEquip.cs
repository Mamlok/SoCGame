using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Items.Interfaces;
using SoC.Items.Models;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class ArmorEquip : IArmorEquip
    {
        private readonly IWeaponService weaponService;
        private readonly ICharakterService charakterService;
        private readonly IMessageHandler messageHandler;

        public ArmorEquip(IWeaponService weaponService, ICharakterService charakterService, IMessageHandler messageHandler)
        {
            this.weaponService = weaponService;
            this.charakterService = charakterService;
            this.messageHandler = messageHandler;
        }
        public void ArmorEquipMethod(Character character)
        {
            var playerChoice = 0;
            messageHandler.Clear();
            if (character.ArmorEquipped.Count == 0 || character.ArmorEquipped == null)
            {
                messageHandler.Write("You dont have anything equiped.");
            }
            else
            {
                messageHandler.Write($"You have this armor equiped now :");
                messageHandler.Write(character.ArmorEquipped[0].Description);
            }
            messageHandler.Write("*********************************");
            messageHandler.Write("Choose a armor to equip :");
            ShowCharakterArmor(character);
            var RunLol = true;
            while (RunLol)
            {
                var character1 = messageHandler.Read();
                if (string.IsNullOrWhiteSpace(character1))
                {
                    messageHandler.Write("Invalid choice. Please try again.");
                    RunLol = true;
                }
                else
                {
                    if (int.TryParse(character1, out int characterIndex))
                    {
                        playerChoice = characterIndex;
                        RunLol = false;
                    }
                    else
                    {
                        messageHandler.Write("Invalid choice. Please try again.");
                        RunLol = true;
                    }
                }
            }
            character.ArmorEquipped.Clear();
            character.ArmorEquipped.Add(character.Armors[playerChoice]);
            messageHandler.Clear();
            character.ArmorClass = character.ArmorEquipped[0].ArmorValue;
            messageHandler.Write("You equiped your armor successfully.");
            messageHandler.Write("[ENTER to END]");
            messageHandler.Read();
            charakterService.SaveCharacter(character);
            messageHandler.Clear();
        }

        private void ShowCharakterArmor(Character character)
        {
            var counter = 0;
            foreach (var item in character.Armors)
            {
                messageHandler.Write("*********************************");
                messageHandler.Write($"Name: ({counter}). {item.Description}");
                messageHandler.Write($"Value: {item.GoldValue} Gold");
                messageHandler.Write($"ArmorClass: {item.ArmorValue}");
                counter++;
            }
            messageHandler.Write("*********************************");
        }
    }
}
