using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Adventures.Interfaces;
using SoC.Adventures.Models;
using SoC.Entities;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
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
                        CheckForTrap(room);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                        }
                        else
                        {
                            messageHandler.Write("There is no chest");
                        }
                        break;
                    case "n":
                    case "s":
                    case "e":
                    case "w":
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
                        }
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        Console.WriteLine("Please enter a valid option.");
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    default:
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
                ProcessTrapMessagesAndDamage(room);
            }

            var exit = room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation);

            if (exit == null) 
            {
                throw new Exception("this room doesnt have that exception");
            }

            var NewRoom = gameAdventure.Rooms.FirstOrDefault(x => x.RoomNumber == exit.LeadsToRoomNumber);

            if (NewRoom == null)
            {
                throw new Exception("Room does not exist");
            }

            RoomProcessor(NewRoom);
        }

        private void OpenChest(Chest chest)
        {
            throw new NotImplementedException();
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
                    ProcessTrapMessagesAndDamage(room);

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

        private void ProcessTrapMessagesAndDamage(Room room)
        {
            var dice = new Dice();

            messageHandler.Write($"U tripped a {room.Trap.TrapType.ToString()} trap!");
            var trapDamage = dice.RollDice(new List<Die>() { room.Trap.DamageDie });
            var hitPoints = character.HitPoints - trapDamage;
            messageHandler.Write($"You were damaged for {trapDamage} HP. You now have {hitPoints} HP");
            if (hitPoints < 1)
            {
                messageHandler.Write("U dead");
            }
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
    }
}
