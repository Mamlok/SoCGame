﻿using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Items.Models;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class CombatService : ICombatService
    {
        private readonly IMessageHandler messageHandler;
        private readonly ICharakterInfo charakterInfo;
        private readonly ICharakterService charakterService;

        public CombatService(IMessageHandler messageHandler,ICharakterInfo charakterInfo)
        {
            this.messageHandler = messageHandler;
            this.charakterInfo = charakterInfo;
        }

        public void RunCombat(ref Character character, List<Monster> monsters)
        {
            var monsterDescriptions = "You face off against : ";
            var monsterCount = monsters.Count;
            foreach (var monster in monsters)
            {
                monsterDescriptions += $"{monster.MonsterType} ";
                if (monsterCount > 1)
                {
                    monsterDescriptions += "and ";
                    monsterCount--;
                }
            }
            messageHandler.Write(monsterDescriptions);

            var dice = new Dice();
            var d20 = new List<Die> { Die.D20 };
            var charDamageDie = new List<Die> { (Die)character.Attack.BaseDie };
            
            messageHandler.WriteRead("Hit a Key to roll for initiative!");

            var charInitiative = dice.RollDice(d20);
            var monsterInitiative = dice.RollDice(d20);

            messageHandler.WriteRead($"You rolled a {charInitiative} and the monster/s rolled {monsterInitiative}");

            while (charInitiative == monsterInitiative)
            {
                messageHandler.Write("That is a tie lets roll again!");

                charInitiative = dice.RollDice(d20);
                monsterInitiative = dice.RollDice(d20);

                messageHandler.WriteRead($"You rolled a {charInitiative} and the monster/s rolled {monsterInitiative}");
            }

            bool monstersAreAlive = true;
            bool characterTurn = charInitiative > monsterInitiative;
            bool armorStolen = false;
            bool healCooldown = false;

            while (character.IsAlive && monstersAreAlive)
            {
                if (characterTurn)
                {
                    messageHandler.Write("Choose an action: (A)ttack, Use (S)pecial Ability, (U)se Item, (R)un Away");
                    messageHandler.Write("Character (I)nfo, (E)nemy info");
                    var action = messageHandler.Read().ToLower();

                    switch (action)
                    {
                        case "a":
                            Attack(character, monsters[0], dice, charDamageDie);
                            break;
                        case "s":
                            UseSpecialAbility(character, monsters[0], dice, ref healCooldown);
                            break;
                        case "u":
                            if (character.Inventory.Count == 0)
                            {  
                                messageHandler.Write("Your inventory is empty");
                                messageHandler.Read();
                                continue;
                            }
                            else
                            {
                                UseItem(character);
                                break;
                            }
                        case "r":
                            if (TryToRunAway(character, dice))
                            {
                                return; //dodělat
                            }
                            break;
                        case "i":
                            charakterInfo.ShowCharakterInfo(character);
                            continue;
                        case "e":
                            MonsterInfo(monsters[0]);
                            continue;
                        default:
                            messageHandler.Write("Invalid action. Try again.");
                            messageHandler.Clear();
                            continue;
                    }

                    characterTurn = false;
                }
                else
                {
                    MonsterAttack(character, monsters[0], dice);
                    characterTurn = true;
                }

                if (monsters[0].HitPoints < 1)
                {
                    HandleMonsterDeath(character, monsters[0]);
                    monsters.RemoveAt(0);
                    if (monsters.Count < 1)
                    {
                        monstersAreAlive = false;
                    }
                }

                if (healCooldown)
                {
                    healCooldown = false;
                }
                if (armorStolen)
                {
                    armorStolen = false;
                }
            }
        }

        private void Attack(Character character, Monster monster, Dice dice, List<Die> charDamageDie)
        {
            messageHandler.WriteRead($"Hit a key to attack the {monster.MonsterType}!");
            var attackToHitMonster = dice.RollDice(new List<Die> { Die.D20 });
            messageHandler.WriteRead($"You rolled a {attackToHitMonster} to hit the {monster.ArmorClass} ArmorClass of the {monster.MonsterType}!");

            if (attackToHitMonster >= monster.ArmorClass)
            {
                var damage = dice.RollDice(charDamageDie) * character.Attack.BonusDamage;
                messageHandler.WriteRead($"You hit the {monster.MonsterType} for {damage} damage!");
                monster.HitPoints -= damage;
            }
            else
            {
                messageHandler.WriteRead($"You missed the {monster.MonsterType}!");
            }
        }

        private void UseSpecialAbility(Character character, Monster monster, Dice dice, ref bool healCooldown)
        {
            var d2 = new List<Die> { Die.D2 };
            switch (character.Class)
            {
                case CharacterClass.Thief:
                    if (dice.RollDice(d2) == 1)
                    {
                        messageHandler.WriteRead($"You attempt to steal armor from the {monster.MonsterType}!");
                        monster.ArmorClass -= 2;
                        messageHandler.WriteRead($"The {monster.MonsterType}'s armor class is now {monster.ArmorClass} for one turn.");
                    }
                    else
                    {
                        messageHandler.WriteRead($"You attempted to steal armor from the {monster.MonsterType}! But you failed");
                    }
                    break;
                case CharacterClass.MagicUser:
                    if (!character.HasUsedSpell)
                    {
                        messageHandler.WriteRead($"You cast a powerful spell on the {monster.MonsterType}!");
                        var damage = dice.RollDice(new List<Die> { Die.D20 }) * 2;
                        messageHandler.WriteRead($"You hit the {monster.MonsterType} for {damage} damage!");
                        monster.HitPoints -= damage;
                        character.HasUsedSpell = true;
                    }
                    else
                    {
                        messageHandler.Write("You have already used your powerful spell in this adventure.");
                    }
                    break;
                case CharacterClass.Healer:
                    if (!healCooldown)
                    {
                        messageHandler.WriteRead("You heal yourself!");
                        var healAmount = dice.RollDice(new List<Die> { Die.D4 });
                        character.HitPoints += healAmount;
                        messageHandler.WriteRead($"You healed yourself for {healAmount} hit points!");
                        healCooldown = true;
                    }
                    else
                    {
                        messageHandler.Write("Your healing ability is on cooldown.");
                    }
                    break;
                default:
                    messageHandler.Write("You have no special ability.");
                    break;
            }
        }

        private void UseItem(Character character)
        {
            messageHandler.Clear();
            ShowInventory(character);
            messageHandler.Write("Choose an item to use:");
            var itemChoice = Convert.ToInt32(messageHandler.Read());
            character.HitPoints += character.Inventory[itemChoice].HealthValue;
            character.Inventory.RemoveAt(itemChoice);
            messageHandler.Clear();
            messageHandler.Write("You used the item!");
            messageHandler.Read();
            messageHandler.Clear();

        }

        private bool TryToRunAway(Character character, Dice dice)
        {
            var runAwayRoll = dice.RollDice(new List<Die> { Die.D20 });
            if (runAwayRoll > 10)
            {
                messageHandler.Write("You successfully ran away!");
                return true;
            }
            else
            {
                messageHandler.Write("You failed to run away!");
                return false;
            }
        }

        private void MonsterAttack(Character character, Monster monster, Dice dice)
        {
            messageHandler.WriteRead($"The {monster.MonsterType} attacks you!");
            var attackToHitCharacter = dice.RollDice(new List<Die> { Die.D20 });
            messageHandler.WriteRead($"The {monster.MonsterType} rolled a {attackToHitCharacter} and your ArmorClass is {character.ArmorClass}!");

            if (attackToHitCharacter >= character.ArmorClass)
            {
                messageHandler.WriteRead($"The {monster.MonsterType} hit you!");
                var damage = dice.RollDice(new List<Die> { (Die)monster.Attack.BaseDie }) * monster.Attack.BonusDamage;
                messageHandler.WriteRead($"The {monster.MonsterType} hit you for {damage} damage!");
                character.HitPoints -= damage;
                if (character.HitPoints < 1)
                {
                    messageHandler.Write($"You are dead! at the hands of a vicious {monster.MonsterType}");
                    character.CauseOfDeath = $"Killed by a {monster.MonsterType}";
                    character.IsAlive = false;
                }
            }
            else
            {
                messageHandler.WriteRead($"The {monster.MonsterType} missed you!");
            }
        }

        private void HandleMonsterDeath(Character character, Monster monster)
        {
            messageHandler.Write($"The {monster.MonsterType} is dead!");

            if (monster.Gold > 0)
            {
                character.Gold += monster.Gold;
                messageHandler.Write($"You found {monster.Gold} gold on the {monster.MonsterType}!");
            }

            if (monster.Inventory != null && monster.Inventory.Count > 0)
            {
                messageHandler.Write($"You found the following items on the {monster.MonsterType}:");
                foreach (var item in monster.Inventory)
                {
                    messageHandler.Write(item.Description);
                }
                character.Inventory.AddRange(monster.Inventory);
            }
        }

        private void MonsterInfo(Monster monster)
        {
            messageHandler.Clear();
            messageHandler.Write($"[{monster.MonsterType.ToUpper()}]");
            messageHandler.Write($"HP: {monster.HitPoints}");
            messageHandler.Write($"Armor Class: {monster.ArmorClass}");
            messageHandler.Write($"Gold: {monster.Gold}");
            if (monster.Inventory != null)
            {
                messageHandler.Write("Inventory:");
                foreach (var item in monster.Inventory)
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
            messageHandler.Write("[ENTER to END]");
            messageHandler.Read();
            messageHandler.Clear();
        }

        private void ShowInventory(Character character)
        {
            messageHandler.Clear();
            messageHandler.Write("[INVENTORY]");
            var counter = 0;
            foreach (var item in character.Inventory)
            {
                if (item.Name == ItemType.Food)
                {
                    messageHandler.Write("*********************************");
                    messageHandler.Write($"Name: ({counter}). {item.Description}");
                    messageHandler.Write($"Value: {item.GoldValue} Gold");
                    if (item.HealthValue > 0)
                    {
                        messageHandler.Write($"Healing value: {item.HealthValue} HP");
                    }
                    counter++;
                }

            }
            messageHandler.Write("*********************************");
            
        }
    }
}
