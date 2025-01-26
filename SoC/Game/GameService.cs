using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Adventures.Interfaces;
using SoC.Adventures.Models;
using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Items.Models;
using SoC.Utilities.Interfaces;
using System.Security.AccessControl;

namespace SoC.Game
{
    public class GameService : IGameService
    {
        private IAdventureService adventureService;
        private ICharakterService characterService;
        private IMessageHandler messageHandler;

        private Character character;
        private Adventure gameAdventure;

        public GameService(IAdventureService AdventureService, ICharakterService CharakterService, IMessageHandler MessageHandler)
        {
            adventureService = AdventureService;
            characterService = CharakterService;
            messageHandler = MessageHandler; 
        }
        public bool StartGame(Adventure adventure = null)
        {
            try
            {
                gameAdventure = adventure;
                if (gameAdventure == null)
                {
                    gameAdventure = adventureService.GetInitialAdventure();
                }

                CreateTitleBanner(gameAdventure.Title);
                CreateDescription(gameAdventure);

                var charactersInRange = characterService.GetCharactersInRange(gameAdventure.MinLevel, gameAdventure.MaxLevel);

                if (charactersInRange.Count == 0)
                {
                    messageHandler.Write("no enough level");
                    return false;
                }
                else
                {
                    messageHandler.Write("pick character:");
                    var characterCount = 0;
                    foreach (var character in charactersInRange)
                    {
                        messageHandler.Write($"{characterCount}. {character.Name} Level - {character.Level} Class: {character.Class}");
                        characterCount++;
                    }
                }
                character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(messageHandler.Read())].Name);

                var rooms = gameAdventure.Rooms;
                RoomProcessor(rooms[0]);


            }
            catch (Exception ex)
            {

                messageHandler.Write($"Something went wrong {ex.Message}");
            }
            return true;

        }

        private void RoomProcessor(Room room)
        {
            RoomDescription(room);
            RoomOptions(room);

        }

        private void RoomOptions(Room room)
        {
            WriteRoomOptions(room);

            var playerDecision = messageHandler.Read().ToLower();
            var exitRoom = false;

            while (exitRoom == false)
            {
                switch (playerDecision)
                {
                    case "l":
                    case "c":
                        messageHandler.Clear();
                        CheckForTrap(room);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        messageHandler.Clear();
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
                        }
                        else
                        {
                            messageHandler.Write("There is no chest");
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
                        }
                        break;
                    case "n":
                    case "s":
                    case "e":
                    case "w":
                        messageHandler.Clear();
                        var wallLocation = CompassDirection.North;
                        if (playerDecision == "s") wallLocation = CompassDirection.South;
                        else if (playerDecision == "e") wallLocation = CompassDirection.East;
                        else if (playerDecision == "w") wallLocation = CompassDirection.West;
                        if (room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation) != null)
                        {
                            ExitRoom(room, wallLocation);
                        }
                        else
                        {
                            messageHandler.Write("\n Something went wrong there is a wall \n");
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
                        }
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Clear();
                        Console.WriteLine("Please enter a valid option.");
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    default:
                        messageHandler.Clear();
                        Console.WriteLine("Please enter a valid option.");
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                }
            }
        }

        private void WriteRoomOptions(Room room)
        {
            messageHandler.Write("What would you like to do?");
            messageHandler.Write("----------------------------");
            messageHandler.Write("(L)ook for traps");
            if (room.Chest != null)
            {
                messageHandler.Write("(O)pen the chest");
                messageHandler.Write("(C)heck the chest for traps");
            }
            messageHandler.Write("Use an exit:");
            foreach (var exit in room.Exits)
            {
                messageHandler.Write($"({exit.WallLocation.ToString().Substring(0, 1)}){exit.WallLocation.ToString().Substring(1)}");
            }
           
        }

        private void ExitRoom(Room room, CompassDirection wallLocation)
        {
            if (room.Trap != null && room.Trap.TrippedOrDisarmed == false)
            {
                ProcessTrapMessagesAndDamage(room.Trap);
                room.Trap.TrippedOrDisarmed = true;
                //IF NOT DEAD - keep going.
            }

            var exit = room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation);

            if (exit == null)
            {
                throw new Exception("this room doesnt have that exception");
            }

            var newRoom = gameAdventure.Rooms.FirstOrDefault(x => x.RoomNumber == exit.LeadsToRoomNumber);

            if (newRoom == null)
            {
                throw new Exception("The room that this previous room was supposed to lead too does not exist!?  Dragons?  Or maybe a bad author!!!");
            }

            if ((exit.Lock == null || !exit.Lock.Locked) || TryUnlock(exit.Lock))
            {
                RoomProcessor(newRoom);
            }
            else
            {
                RoomProcessor(room);
            }
        }

        private void OpenChest(Chest chest)
        {
            if (chest.Lock == null || !chest.Lock.Locked)
            {
                if (chest.Trap != null && !chest.Trap.TrippedOrDisarmed)
                {
                    ProcessTrapMessagesAndDamage(chest.Trap);
                    chest.Trap.TrippedOrDisarmed = true;
                }
                else
                {
                    messageHandler.Write("You open the chest..");
                    if (chest.Gold > 0)
                    {
                        character.Gold += chest.Gold;
                        messageHandler.Write($"Woot! You find {chest.Gold} gold! Your total gold is now {character.Gold}\n");
                        chest.Gold = 0;
                    }

                    if (chest.Treasure != null && chest.Treasure.Count > 0)
                    {
                        messageHandler.Write($"You find {chest.Treasure.Count} items in this chest!  And they are:");

                        foreach (var item in chest.Treasure)
                        {
                            messageHandler.Write(item.Name.ToString());
                        }
                        messageHandler.Write("\n");

                        character.Inventory.AddRange(chest.Treasure);
                        chest.Treasure = new List<Item>();
                        return;
                    }

                    if (chest.Gold == 0 && (chest.Treasure == null || chest.Treasure.Count == 0))
                    {
                        messageHandler.Write("The chest is empty... \n");
                    }
                }
            }
            else
            {
                if (TryUnlock(chest.Lock))
                {
                    OpenChest(chest);
                }
            }
        }
        private void CheckForTrap(Room room)
        {
            if (room.Trap != null)
            {
                if (room.Trap.TrippedOrDisarmed)
                {
                    messageHandler.Write("You had already found this trap or tripped it");
                    return;
                }
                if (room.Trap.SearchedFor)
                {
                    messageHandler.Write("You had already searched for it ");
                    return;
                }

                var trapBonus = 0 + character.Abilities.Intelligence;
                if (character.Class == CharacterClass.Thief)
                {
                    trapBonus += 2;
                }

                var dice = new Dice();
                var findTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

                if (findTrapRoll < 12)
                {
                    messageHandler.Write("You find no traps");
                    room.Trap.SearchedFor = true;
                    return;
                }
                messageHandler.Write("You found the trap and are forced to disarm it");
                var disarmTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) +trapBonus;

                if (disarmTrapRoll < 11)
                {
                    ProcessTrapMessagesAndDamage(room.Trap);

                }
                else
                {
                    messageHandler.Write("Trap disarmed");
                }
                room.Trap.TrippedOrDisarmed = true;
                return;

            }          

            messageHandler.Write("You find no traps");
            return;
        }

        private void ProcessTrapMessagesAndDamage(Trap trap)
        {
            var dice = new Dice();

            messageHandler.Write($"U tripped a {trap.TrapType.ToString()} trap!");
            var trapDamage = dice.RollDice(new List<Die>() { trap.DamageDie });
            character.HitPoints -= trapDamage;
            var hitPoints = character.HitPoints;
            messageHandler.Write($"You were damaged for {trapDamage} HP. You now have {hitPoints} HP");
            if (hitPoints < 1)
            {
                messageHandler.Write("U dead");
            }
            messageHandler.Read();
        }

        private void RoomDescription(Room room)
        {
            messageHandler.Clear();
            messageHandler.Write("---------------------------");

            messageHandler.Write($"{room.RoomNumber} {room.Description}");
            if (room.Exits.Count == 1)
            {
                messageHandler.Write($"Therer is an exit on the {room.Exits[0].WallLocation} wall");
            }
            else
            {
                var exitDescription = "";
                foreach (var exit in room.Exits)
                {
                    exitDescription += $"{exit.WallLocation},";
                }

                messageHandler.Write($"There are exits on the {exitDescription.Remove(exitDescription.Length - 1)} walls.");
            }

            if (room.Chest != null)
            {
                messageHandler.Write("There is a chest in this room");
            }
        }

        private void CreateDescription(Adventure adventure)
        {
            messageHandler.Write($"\n{adventure.Description}");
            messageHandler.Write($"\nLevels: {adventure.MinLevel} - {adventure.MaxLevel}");
            messageHandler.Write($"\nCompletion Rewards: {adventure.CompleteEddiesReward} gold & {adventure.CompleteXpReward} XP");
            messageHandler.Write();
        }

        private void CreateTitleBanner(string title)
        {
            messageHandler.Clear();
            messageHandler.Write();
            
            for (int i = 0; i <= title.Length + 1; i++)
            {
                messageHandler.Write("*", false);
                if (i == title.Length + 1)
                {
                    messageHandler.Write("\n", false);
                }
            }
            messageHandler.Write($"│{title}│");
            for (int i = 0; i <= title.Length + 1; i++)
            {
                messageHandler.Write("*", false);
                if (i == title.Length + 1)
                {
                    messageHandler.Write("\n", false);
                }
            }
        }

        private bool TryUnlock(Lock theLock)
        {
            if (!theLock.Locked) return true;

            var hasOptions = true;
            var dice = new Dice();

            while (hasOptions)
            {
                if (!theLock.Attempted)
                {
                    messageHandler.Write("Locked!  Would you like to attempt to unlock it? \n" +
                        "K)ey L)ockpick B)ash or W)alk away");
                    var playerDecision = messageHandler.Read().ToLower();
                    switch (playerDecision)
                    {
                        case "k":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLock.KeyNumber) != null)
                            {
                                messageHandler.WriteRead("You have the right key!  It unlocks the lock! \n");
                                theLock.Locked = false;
                                return true;
                            }
                            else
                            {
                                messageHandler.Write("You do not have a key for this chest \n");
                                break;
                            }
                        case "l":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Lockpicks) == null)
                            {
                                messageHandler.Write("You don't have lockpicks! \n");
                                break;
                            }
                            else
                            {
                                var lockpickBonus = 0 + character.Abilities.Dexterity;
                                if (character.Class == CharacterClass.Thief)
                                {
                                    lockpickBonus += 2;
                                }
                                var pickRoll = (dice.RollDice(new List<Die> { Die.D20 }) + lockpickBonus);
                                if (pickRoll > 12)
                                {
                                    messageHandler.WriteRead($"Youe dextrous hands click that lock open! \n" +
                                    $"Your lockpick roll was {pickRoll} and you needed 12! \n");
                                    theLock.Locked = false;
                                    theLock.Attempted = true;
                                    return true;
                                }
                                messageHandler.WriteRead($"Snap! The lock doesnt budge! \n" +
                                $"Your lockpick roll was {pickRoll} and you needed 12! \n");
                                theLock.Attempted = true;
                                break;
                            }
                        case "b":
                            var bashBonus = 0 + character.Abilities.Strength;
                            if (character.Class == CharacterClass.Fighter)
                            {
                                bashBonus += 2;
                            }
                            var bashRoll = (dice.RollDice(new List<Die> { Die.D20 }) + bashBonus);
                            if (bashRoll > 16)
                            {
                                messageHandler.WriteRead($"You muster your strength and BASH that silly lock into submission! \n" +
                                    $"Your bash roll was {bashRoll} and you needed 16! \n");
                                theLock.Locked = false;
                                theLock.Attempted = true;
                                return true;
                            }
                            messageHandler.WriteRead($"Ouch! The lock doesnt budge! \n" +
                                $"Your bash roll was {bashRoll} and you needed 16! \n");
                            theLock.Attempted = true;
                            break;

                        default:
                            return false;
                    }
                }
                else
                {
                    if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLock.KeyNumber) != null)
                    {
                        messageHandler.WriteRead("You've tried bashing or picking to no avail BUT you have the right key!  Unlocked! \n");
                        theLock.Locked = false;
                        return true;
                    }
                    else
                    {
                        messageHandler.WriteRead("You cannot try to bash or pick this lock again and you do not currently have a key! \n");
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
