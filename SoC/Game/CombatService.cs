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
    public class CombatService : ICombatService
    {
        private readonly IMessageHandler messageHandler;

        public CombatService(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        public void RunCombat(ref Character character, List<Monster> monsters)
        {
            var monsterDescriptions = "You face off against :";
            foreach (var monster in monsters)
            {
                monsterDescriptions += $"{monster.MonsterType} ";
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
                    messageHandler.Write("Choose an action: (A)ttack, Use (S)pecial Ability, Use (I)tem, (R)un Away");
                    var action = messageHandler.Read().ToLower();

                    switch (action)
                    {
                        case "a":
                            Attack(character, monsters[0], dice, charDamageDie);
                            break;
                        case "s":
                            UseSpecialAbility(character, monsters[0], dice, ref healCooldown);
                            break;
                        case "i":
                            UseItem(character);
                            break;
                        case "r":
                            if (TryToRunAway(character, dice))
                            {
                                return; //dodělat
                            }
                            break;
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
            messageHandler.WriteRead($"You rolled a {attackToHitMonster} to hit the {monster.ArmorClass}!");

            if (attackToHitMonster >= monster.ArmorClass)
            {
                var damage = dice.RollDice(charDamageDie);
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
            //itemy
            messageHandler.Write("You used an item.");
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
                var damage = dice.RollDice(new List<Die> { (Die)monster.Attack.BaseDie });
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


    }
}
