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
    public class EquipWeapon : IEquipWeapon
    {
        private readonly IWeaponService weaponService;
        private readonly ICharakterService charakterService;
        private readonly IMessageHandler messageHandler;

        public EquipWeapon(IWeaponService weaponService, ICharakterService charakterService, IMessageHandler messageHandler)
        {
            this.weaponService = weaponService;
            this.charakterService = charakterService;
            this.messageHandler = messageHandler;
        }
        public void EquipWeaponMethod(Character character)
        {
            messageHandler.Clear();
            if (character.WeaponEquipped.Count == 0 || character.WeaponEquipped == null)
            {
                messageHandler.Write("You dont have anything equiped.");
            }
            else
            {
                messageHandler.Write($"You have this weapon equiped now :");
                messageHandler.Write(character.WeaponEquipped[0].Description);
            }
            messageHandler.Write("*********************************");
            messageHandler.Write("Choose a weapon to equip :");
            ShowCharakterWeapons(character);
            var playerChoice = Convert.ToInt32(messageHandler.Read());
            character.WeaponEquipped.Clear();
            character.WeaponEquipped.Add(character.Weapons[playerChoice]);
            messageHandler.Clear();
            character.Attack.BonusDamage = character.WeaponEquipped[0].DamageValue;
            messageHandler.Write("You equiped your weapon successfully.");
            messageHandler.Write("[ENTER to END]");
            messageHandler.Read();
            charakterService.SaveCharacter(character);
            messageHandler.Clear();
        }

        private void ShowCharakterWeapons(Character character)
        {
            var counter = 0;
            foreach (var item in character.Weapons)
            {
                messageHandler.Write("*********************************");
                messageHandler.Write($"Name: ({counter}). {item.Description}");
                messageHandler.Write($"Value: {item.GoldValue} Gold");
                messageHandler.Write($"Damage: {item.DamageValue}");
                counter++;
            }
            messageHandler.Write("*********************************");
        }
    }
}
