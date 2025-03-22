using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Items.Models;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            bool scaredAway = false;
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
                    messageHandler.Write($"Choose an action: (A)ttack, Use (S)pecial Ability, (U)se Item, (R) - Scare away");
                    messageHandler.Write("Character (I)nfo, (E)nemy info");
                    var action = messageHandler.Read().ToLower();

                    switch (action)
                    {
                        case "a":
                            Attack(character, monsters[0], dice, charDamageDie);
                            break;
                        case "s":
                            UseSpecialAbility(character, monsters[0], dice, ref healCooldown, charDamageDie);
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
                            if (TryToScare(character, dice, monsters[0]))
                            {
                                scaredAway = true;
                                monsters[0].HitPoints = 0;
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
                    HandleMonsterDeath(character, monsters[0], scaredAway);
                    monsters.RemoveAt(0);
                    if (monsters.Count < 1)
                    {
                        monstersAreAlive = false;
                    }
                    else
                    {
                        messageHandler.Write("Another enemy comes!");
                    }
                }

                if (healCooldown)
                {
                    healCooldown = false;
                }
            }
        }

        private void Attack(Character character, Monster monster, Dice dice, List<Die> charDamageDie)
        {
            messageHandler.WriteRead($"Hit a key to attack the {monster.MonsterType}!");
            var attackToHitMonster = dice.RollDice(new List<Die> { Die.D20 });
            messageHandler.WriteRead($"You rolled a {attackToHitMonster} and the {monster.MonsterType} ArmorClass is {monster.ArmorClass}!");

            if (attackToHitMonster >= monster.ArmorClass)
            {
                messageHandler.WriteRead($"Hit a key to roll for damage!");
                var RollAtack = dice.RollDice(charDamageDie);
                messageHandler.WriteRead($"You rolled {RollAtack}");
                var damage = RollAtack * character.Attack.BonusDamage;
                messageHandler.WriteRead($"You hit the {monster.MonsterType} for {damage} damage!");
                monster.HitPoints -= damage;
            }
            else
            {
                messageHandler.WriteRead($"You missed the {monster.MonsterType}!");
            }
        }

        private void UseSpecialAbility(Character character, Monster monster, Dice dice, ref bool healCooldown,List<Die> charDamageDie)
        {
            var d100 = new List<Die> { Die.D100 };
            switch (character.Class)
            {
                case CharacterClass.Thief:
                    if (dice.RollDice(d100) >= 70)
                    {
                        messageHandler.WriteRead($"You attempt to steal armor from the {monster.MonsterType}!");
                        if (monster.ArmorClass >= 2)
                        {
                            monster.ArmorClass -= 2;
                        }
                        else
                        {
                            monster.ArmorClass = 1;
                        }
                        messageHandler.WriteRead($"The {monster.MonsterType}'s armor class is now {monster.ArmorClass}.");
                    }
                    else
                    {
                        messageHandler.WriteRead($"You attempted to steal armor from the {monster.MonsterType}! But you failed");
                    }
                    break;
                case CharacterClass.MagicUser:
                    if (!character.HasUsedSpell)
                    {
                        var bonusDamage = 0;
                        messageHandler.WriteRead($"You cast a powerful spell on the {monster.MonsterType}!");
                        if (character.WeaponEquipped[0].MagicValue != null)
                        {
                            bonusDamage = character.WeaponEquipped[0].MagicValue;
                            
                        }
                        else
                        {
                            bonusDamage = 1;
                        }
                        var damage = dice.RollDice(new List<Die> { Die.D20 }) * bonusDamage;
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
                case CharacterClass.Fighter:
                    var roll = dice.RollDice(new List<Die> { Die.D100 });
                    messageHandler.Write($"You rolled {roll} and needed 80");
                    messageHandler.Read();
                    if (roll >= 80)
                    {
                        messageHandler.WriteRead($"You channel the power of vril and prepare a powerful attack on the {monster.MonsterType}!");
                        var attackRoll = dice.RollDice(new List<Die> { Die.D20 });
                        messageHandler.WriteRead($"You rolled {attackRoll} and the monster´s ArmorClass is {monster.ArmorClass}");
                        if (attackRoll >= monster.ArmorClass)
                        {
                            var damage = dice.RollDice(charDamageDie) * character.Attack.BonusDamage * 2;
                            messageHandler.WriteRead($"You hit the {monster.MonsterType} for {damage} damage!");
                            monster.HitPoints -= damage;
                        }
                        else
                        {
                            messageHandler.WriteRead($"You missed the {monster.MonsterType}!");
                        }
                    }
                    else
                    {
                        messageHandler.Write("Your vril is too weak!");
                    }
                    
                    break;
                default:
                    messageHandler.Write("You have no special ability.");
                    break;
            }
        }

        private void UseItem(Character character)
        {
            var itemChoice = 0;
            messageHandler.Clear();
            ShowInventory(character);
            messageHandler.Write("Choose an item to use:");
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
                        itemChoice = characterIndex;
                        RunLol = false;
                    }
                    else
                    {
                        messageHandler.Write("Invalid choice. Please try again.");
                        RunLol = true;
                    }
                }
            }

            character.HitPoints += character.Inventory[itemChoice].HealthValue;
            character.Inventory.RemoveAt(itemChoice);
            messageHandler.Clear();
            messageHandler.Write("You used the item!");
            messageHandler.Read();
            messageHandler.Clear();

        }

        private bool TryToScare(Character character, Dice dice, Monster monster)
        {
            var bonus = monster.MonsterStrenght;
            var runAwayRoll = dice.RollDice(new List<Die> { Die.D60 }) * character.Abilities.Strength;
            var monsterBar = bonus * 100;
            if (runAwayRoll > monsterBar)
            {
                messageHandler.Write($"You successfully scared the {monster.MonsterType} away!");
                return true;
            }
            else
            {
                messageHandler.Write($"You failed to scare the {monster.MonsterType} away!");
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

        private void HandleMonsterDeath(Character character, Monster monster, bool scaredAway)
        {
            if (scaredAway)
            {
                if (monster.Gold > 0 || (monster.Inventory != null && monster.Inventory.Count > 0) || (monster.Armors != null && monster.Armors.Count > 0) || (monster.Weapons != null && monster.Weapons.Count > 0))
                {
                    messageHandler.Write($"The {monster.MonsterType} left something behind!");
                }
            }
            else
            {
                messageHandler.Write($"The {monster.MonsterType} is dead!");
            }
            

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

            if (monster.Armors != null && monster.Armors.Count > 0)
            {
                messageHandler.Write($"You found the following armor on the {monster.MonsterType}:");
                foreach (var item in monster.Armors)
                {
                    messageHandler.Write(item.Description);
                }
                character.Armors.AddRange(monster.Armors);
            }

            if (monster.Weapons != null && monster.Weapons.Count > 0)
            {
                messageHandler.Write($"You found the following weapons on the {monster.MonsterType}:");
                foreach (var item in monster.Weapons)
                {
                    messageHandler.Write(item.Description);
                }
                character.Weapons.AddRange(monster.Weapons);
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
