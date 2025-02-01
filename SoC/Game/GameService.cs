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
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Threading;

namespace SoC.Game
{
    public class GameService : IGameService
    {
        private IAdventureService adventureService;
        private ICharakterService characterService;
        private IMessageHandler messageHandler;
        private ICombatService combatService;

        private Character character;
        private Adventure gameAdventure;
        private bool gameWon = false;
        private string gameWinningDescription;

        public GameService(IAdventureService AdventureService, ICharakterService CharakterService, IMessageHandler MessageHandler, ICombatService CombatService)
        {
            adventureService = AdventureService;
            characterService = CharakterService;
            messageHandler = MessageHandler;
            combatService = CombatService;
        }
        public bool StartGame(Adventure adventure = null)
        {
            try
            {
                /*
                gameAdventure = adventure;
                if (gameAdventure == null)
                {
                    gameAdventure = adventureService.GetAdventure();
                }

                CreateTitleBanner(gameAdventure.Title);
                CreateDescription(gameAdventure);
                */
                messageHandler.Clear();
                var charactersInRange = characterService.GetCharactersInRange();

                if (charactersInRange.Count == 0)
                {
                    messageHandler.Write("Sorry, you dont have any characters.");
                    messageHandler.Write("(C)reate a new character");
                    messageHandler.Write("(R)eturn to main menu");
                    var playerDecision = messageHandler.Read().ToLower();
                    bool running = true;
                    while (running)
                    {
                        switch (playerDecision)
                        {
                            case "c":
                                messageHandler.Clear();
                                running = false;
                                characterService.CreateCharacter();
                                break;
                            case "r":
                                running = false;
                                messageHandler.Clear();
                                Program.MainMenu();
                                break;
                            default:
                                Console.WriteLine("Please enter a valid option.");
                                playerDecision = messageHandler.Read().ToLower();
                                running = true;
                                break;
                        }
                    }   

                }
                else
                {
                    messageHandler.Write("Pick character:");
                    var characterCount = 0;
                    foreach (var character in charactersInRange)
                    {
                        messageHandler.Write($"{characterCount}. {character.Name} Level - {character.Level} Class: {character.Class}");
                        characterCount++;
                    }
                }
                character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(messageHandler.Read())].Name);

                if (character.PlayedIntro == false)
                {
                    PlayIntro(character);
                    
                }
                //var rooms = gameAdventure.Rooms;
                //RoomProcessor(rooms[0]);


            }
            catch (Exception ex)
            {

                messageHandler.Write($"Something went wrong {ex.Message}");
            }
            return true;

        }

        private void PlayIntro(Character character)
        {
            Program.MakeTitle();
            messageHandler.Write("------------------------------------------------------------");
            messageHandler.Write("\t**BLACKWOOD FOREST – MIDNIGHT**");
            messageHandler.Write("\tA thick fog clings to the ground, swallowing sound.");
            messageHandler.Write("\tDistant howls pierce the air—closer than before.");
            messageHandler.Write("\tThe trees whisper as a chill runs down your spine.");
            messageHandler.Write("\tSomething is wrong here... very wrong.");
            messageHandler.Write("------------------------------------------------------------");
            messageHandler.Write(" ");
            messageHandler.Write("\t[Press ENTER to Begin Your Journey]");
            messageHandler.Read();
            messageHandler.Clear();
            if (character.Background == CharacterBackground.Noble)
            {
                messageHandler.Write("The halls of your family’s estate were grand, adorned with banners bearing your house's");
                messageHandler.Write("crest, the scent of polished wood mingling with the faint aroma of aged wine. Yet,");
                messageHandler.Write("beneath the luxury, you felt trapped—bound by duty, suffocated by the weight of ");
                messageHandler.Write("expectation. A life of politics, alliances, and preordained destiny was all you’d known.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("When your elder sibling inherited the title of lord, you thought you’d find peace in your");
                messageHandler.Write("reduced responsibilities. But the scheming of courtiers and whispers of betrayal grew");
                messageHandler.Write("unbearable. One fateful evening, you made your choice: you donned a simple cloak,");
                messageHandler.Write("packed your sword, and left your family behind. The road ahead was uncertain, but at");
                messageHandler.Write("least it was your own.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("Now, after weeks of wandering and dwindling coin, you find yourself stumbling through a");
                messageHandler.Write("dense forest, your noble boots muddied, your stomach growling. You crest a hill and");
                messageHandler.Write("spot a small village nestled in a valley below. Lanterns flicker in the misty evening air,");
                messageHandler.Write("and your heart lifts slightly. Perhaps here, among simpler folk, you might find refuge—");
                messageHandler.Write("and perhaps a new purpose.");
                messageHandler.Read();
                messageHandler.Clear();
            }
            else if (character.Background == CharacterBackground.Outcast)
            {
                messageHandler.Write("The memories of your past are vivid, sharp as blades: the betrayal that tore your world");
                messageHandler.Write("apart, the faces of those who turned their backs on you, and the cold realization that you");
                messageHandler.Write("were truly alone. The brand—whether literal or figurative—marks you still, a constant");
                messageHandler.Write("reminder of what you’ve lost.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("You’ve spent years surviving on the fringes of society, shunned by most, tolerated by few.");
                messageHandler.Write("Every town, every camp, every settlement has felt temporary, a place to hide rather than");
                messageHandler.Write("to live. Yet, somewhere deep within you, a flicker of hope remains. Perhaps, one day,");
                messageHandler.Write("you’ll find a place where you’re more than just an outcast.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("The forest seems to stretch endlessly around you, the evening shadows deepening. Your");
                messageHandler.Write("meager rations are running low, and your strength is waning. Then, through the gloom,");
                messageHandler.Write("you spot it: a village, its lights like beacons in the darkness. You hesitate for a moment,");
                messageHandler.Write("wary of the reception you might receive, but your need for shelter outweighs your fear.");
                messageHandler.Read();
                messageHandler.Clear();
            }
            else if (character.Background == CharacterBackground.Drifter)
            {
                messageHandler.Write("The road is your constant companion, your only home. You’ve spent years walking its");
                messageHandler.Write("winding paths, taking what odd jobs you could, never staying in one place long enough");
                messageHandler.Write("to be remembered. Faces blur together in your mind, towns and cities fading into distant");
                messageHandler.Write("memories. You don’t know where you belong, and a part of you wonders if you’re");
                messageHandler.Write("destined to wander forever.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("On this particular evening, the forest feels heavier than usual. The trees loom like dark ");
                messageHandler.Write("sentinels, and the air carries a faint chill. You tighten your grip on your pack, its weight a");
                messageHandler.Write("small comfort against the creeping unease. Just as you consider stopping to rest, the");
                messageHandler.Write("faint glow of lanterns cuts through the darkness. A village.");
                messageHandler.Read();
                messageHandler.Clear();
                messageHandler.Write("You sigh with relief as you make your way toward the light. The promise of a warm fire");
                messageHandler.Write("and a hot meal drives you forward. You’ve seen enough villages to know this one might ");
                messageHandler.Write("hold work for someone like you—a drifter with no ties, no questions asked.");
                messageHandler.Read();
            }
            messageHandler.Write("\t[Press ENTER to enter the local tavern]");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("The creak of the Dancing Willow Tavern’s wooden door is a welcome sound as you step");
            messageHandler.Write("inside. The warmth of the fire hits you like a wave, chasing away the evening chill. The");
            messageHandler.Write("scent of roasting meat and spiced ale fills the air, and the hum of conversation wraps");
            messageHandler.Write("around you like a familiar tune.");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("The villagers glance your way as you enter, their eyes flicking over your mud-splattered");
            messageHandler.Write("boots, your worn cloak, and the weariness etched into your features. They don’t speak,");
            messageHandler.Write("but their expressions say enough: another traveler, another story.");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("The barkeep, a stout man with a gray beard and a towel slung over his shoulder, beckons");
            messageHandler.Write("you to the bar.");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("B: Come in, stranger,");
            messageHandler.Write("(his voice gruff but welcoming)", false);
            messageHandler.Read();
            messageHandler.Write("B: You look like you’ve had a rough journey.", false);
            messageHandler.Read();
            messageHandler.Write("B: Care for a drink?", false);
            messageHandler.Read();
            messageHandler.Write("B: Something warm to eat?");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("You nod and take a seat at the bar. As you sip the ale he sets before you, the voices");
            messageHandler.Write("around you begin to rise. Farmers whisper about strange markings in their fields. A");
            messageHandler.Write("merchant complains of goods stolen during the night. A hunter, seated near the fire,");
            messageHandler.Write("mutters about wolves howling closer than ever.");
            messageHandler.Write("The barkeep leans in, lowering his voice.");
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("B: This village’s been quiet for years, but lately....  strange things’ve been happening.", false);
            messageHandler.Read();
            messageHandler.Write("B: If you’re looking for work, there’s plenty of trouble to go around.", false);
            messageHandler.Read();
            messageHandler.Clear();
            messageHandler.Write("You glance around the room, taking in the faces of the villagers. Some are hopeful,");
            messageHandler.Write("others wary, but all are burdened by something they can’t fight alone. Whether by fate,");
            messageHandler.Write("duty, or sheer chance, you realize this small village is where your story begins.");
            messageHandler.Read();
            messageHandler.Clear();
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
                        messageHandler.Clear();
                        CheckForTrap(room);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "c":
                        messageHandler.Clear();
                        CheckForTrapInChest(room.Chest);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        messageHandler.Clear();
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                            WriteRoomOptions(room);
                            if (gameWon)
                            {
                                GameOver();
                            }
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
                            messageHandler.Write(item.Description);

                            if (item.ObjectiveNumber == gameAdventure.FinalObjective)
                            {
                                gameWon = true;
                                gameWinningDescription = item.Description;
                                character.Gold += gameAdventure.CompleteGoldReward;
                                character.XP += gameAdventure.CompleteXpReward;
                                character.AdventurePlayed.Add(gameAdventure.GUID);
                            }
                        }
                        messageHandler.Write("\n");

                        character.Inventory.AddRange(chest.Treasure);
                        chest.Treasure = new List<Item>();
                        if (gameWon)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.White;
                            messageHandler.Write("**************************************************");
                            messageHandler.Write("*         YOU FOUND THE FINAL OBJECTIVE!         *");
                            messageHandler.Write("**************************************************");

                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                            messageHandler.Write($"You found : {gameWinningDescription}");
                            messageHandler.Write("Rewards:");
                            messageHandler.Write($"{gameAdventure.CompleteXpReward} XP");
                            messageHandler.Write($"{gameAdventure.CompleteGoldReward} Gold");
                            messageHandler.Write($"{character.Name} now has {character.XP} XP and {character.Gold} Gold");
                            character.HasUsedSpell = false;

                        }
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
                    if (gameWon)
                    {
                        GameOver();
                    }
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
                character.CauseOfDeath =  $"You were killed by a {trap.TrapType} trap!";
                character.DiedInAdventure = gameAdventure.Title;
                character.IsAlive = false;
                Death();
                GameOver();
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

            if (room.Monsters != null)
            {
                messageHandler.Write("THERE IS A MONSTER HERE!!");
                combatService.RunCombat(ref character, room.Monsters);
                if (!character.IsAlive)
                {
                    character.DiedInAdventure = gameAdventure.Title;
                    Death();
                    GameOver();
                }
                else
                {
                    room.Monsters = null;
                }
            }
        }

        private void CreateDescription(Adventure adventure)
        {
            messageHandler.Write($"\n{adventure.Description}");
            messageHandler.Write($"\nLevels: {adventure.MinLevel} - {adventure.MaxLevel}");
            messageHandler.Write($"\nCompletion Rewards: {adventure.CompleteGoldReward} gold & {adventure.CompleteXpReward} XP");
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

        private void Death()
        {
            messageHandler.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            messageHandler.Write("\t __   __  _______  __   __    ______   ___   _______  ______  ");
            messageHandler.Write("\t|  | |  ||       ||  | |  |  |      | |   | |       ||      | ");
            messageHandler.Write("\t|  |_|  ||   _   ||  | |  |  |  _    ||   | |    ___||  _    |");
            messageHandler.Write("\t|       ||  | |  ||  |_|  |  | | |   ||   | |   |___ | | |   |");
            messageHandler.Write("\t|_     _||  |_|  ||       |  | |_|   ||   | |    ___|| |_|   |");
            messageHandler.Write("\t  |   |  |       ||       |  |       ||   | |   |___ |       |");
            messageHandler.Write("\t  |___|  |_______||_______|  |______| |___| |_______||______| ");
            messageHandler.Write("\t                                                              ");
            Console.ForegroundColor = ConsoleColor.White;
            messageHandler.Write($"\t{character.CauseOfDeath} in {character.DiedInAdventure}");
            Thread.Sleep(5000);
            messageHandler.Clear();

        }

        private void GameOver()
        {
            characterService.SaveCharacter(character);
            //character = new Character();
            messageHandler.WriteRead("GAME IS OVER PRES ENTER TO RETURN TO MAIN MENU");
            messageHandler.Clear();
            Program.MainMenu();
        }

        private void CheckForTrapInChest(Chest chest)
        {
            if (chest.Trap != null)
            {
                if (chest.Trap.TrippedOrDisarmed)
                {
                    messageHandler.Write("You had already found this trap or tripped it");
                    return;
                }
                if (chest.Trap.SearchedFor)
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
                    chest.Trap.SearchedFor = true;
                    return;
                }
                messageHandler.Write("You found the trap and are forced to disarm it");
                var disarmTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

                if (disarmTrapRoll < 11)
                {
                    ProcessTrapMessagesAndDamage(chest.Trap);
                }
                else
                {
                    messageHandler.Write("Trap disarmed");
                }
                chest.Trap.TrippedOrDisarmed = true;
                return;
            }

            messageHandler.Write("You find no traps");
            return;
        }

    }
}
