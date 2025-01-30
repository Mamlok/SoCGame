using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Items.Models;
using SoC.Utilities;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoC.Entities
{
    public class CharacterService : ICharakterService
    {
        private readonly IMessageHandler messageHandler;
        
        public CharacterService(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }


        public Character LoadCharacter(string name)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var character = new Character();

            if (File.Exists($"{basePath}\\{name}.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var CharacterJsonFile = directory.GetFiles($"{name}.json");

                using (StreamReader fi = File.OpenText(CharacterJsonFile[0].FullName))
                {
                    character = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                }
            }
            else
            {
                throw new Exception("Character not found");
            }
            return character;
        }

        public List<Character> GetCharactersInRange(Guid adventureGUID, int minLevel = 0, int maxLevel = 20)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var charactersInRange = new List<Character>();

            try
            {
                var directory = new DirectoryInfo(basePath);
                foreach (var file in directory.GetFiles($"*.json"))
                {
                    using (StreamReader fi = File.OpenText(file.FullName))
                    {
                        var potentialCharacterInRange = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                        if (potentialCharacterInRange.IsAlive && (potentialCharacterInRange.Level >= minLevel && potentialCharacterInRange.Level <= maxLevel) && potentialCharacterInRange.AdventurePlayed != null && !potentialCharacterInRange.AdventurePlayed.Contains(adventureGUID))
                        {
                            charactersInRange.Add(potentialCharacterInRange);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OH NOZE! Goblins!!! {ex.Message}");
            }

            return charactersInRange;
        }

        public bool SaveCharacter(Character character)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            File.WriteAllText($"{basePath}\\{character.Name}.json", JsonConvert.SerializeObject(character));
            return true;
        }

        public void CreateCharacter()
        {
            messageHandler.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            messageHandler.Write("**********************************************************");
            Console.ForegroundColor = ConsoleColor.Yellow;
            messageHandler.Write("*                   CHARACTER CREATION                   *");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            messageHandler.Write("**********************************************************");
            Console.ForegroundColor = ConsoleColor.White;
            messageHandler.Write("Welcome to the character creation process.");
            messageHandler.Write("Please answer the following questions to create your character.");
            messageHandler.Write("**********************************************************");
            messageHandler.Read();
            messageHandler.Clear();

            var newCharacter = new Character();
            newCharacter.Inventory = new List<Item>();

            messageHandler.Write("What is your character's name?");
            newCharacter.Name = messageHandler.Read();
            messageHandler.Clear();

            messageHandler.Write("Class? (F)ighter, (T)hief, (M)agicUser, (H)ealer, (I)nfo"); //vysvětlit
            bool classChosen = false;
            while (!classChosen)
            {
                switch (messageHandler.Read().ToLower())
                {
                    case "f":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Fighter;
                        newCharacter.HitPoints = 8;
                        newCharacter.Abilities.Strength = 1;
                        newCharacter.ArmorClass = 12;
                        newCharacter.Attack = new Attack { BaseDie = 8, BonusDamage = 0 };
                        break;
                    case "t":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Thief;
                        newCharacter.HitPoints = 5;
                        newCharacter.Abilities.Dexterity = 1;
                        newCharacter.ArmorClass = 8;
                        newCharacter.Attack = new Attack { BaseDie = 4, BonusDamage = 0 };
                        break;
                    case "m":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.MagicUser;
                        newCharacter.HitPoints = 4;
                        newCharacter.Abilities.Intelligence = 1;
                        newCharacter.ArmorClass = 8;
                        newCharacter.Attack = new Attack { BaseDie = 4, BonusDamage = 0 };
                        break;
                    case "h":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Healer;
                        newCharacter.HitPoints = 6;
                        newCharacter.Abilities.Wisdom = 1;
                        newCharacter.ArmorClass = 10;
                        newCharacter.Attack = new Attack { BaseDie = 6, BonusDamage = 0 };
                        break;
                    case "i":
                        WriteInfo();
                        classChosen = false;
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Write("Please enter a valid option.");
                        classChosen = false;
                        break;
                    default:
                        messageHandler.Write("Please enter a valid option.");
                        classChosen = false;
                        break;
                }
            }
            messageHandler.Clear();

            messageHandler.Write("Background? (D)rifter, (N)oble, (O)utcast");
            bool backgroundChosen = false;
            while (!backgroundChosen)
            {
                switch (messageHandler.Read().ToLower())
                {
                    case "d":
                        backgroundChosen = true;
                        newCharacter.Background = CharacterBackground.Drifter;
                        break;
                    case "n":
                        backgroundChosen = true;
                        newCharacter.Background = CharacterBackground.Noble;
                        break;
                    case "o":
                        backgroundChosen = true;
                        newCharacter.Background = CharacterBackground.Outcast;
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Write("Please enter a valid option.");
                        backgroundChosen = false;
                        break;
                    default:
                        messageHandler.Write("Please enter a valid option.");
                        backgroundChosen = false;
                        break;
                }
            }

            newCharacter.Abilities = SetAbilities(newCharacter);
            messageHandler.Clear();
            DisplayCharacter(newCharacter);
            messageHandler.Write("(S)ave or (R)edo");
            bool saveCharacterRunning = true;
            while (saveCharacterRunning)
            {
                switch (messageHandler.Read().ToLower())
                {
                    case "s":
                        messageHandler.Clear();
                        saveCharacterRunning = false;
                        if (SaveCharacter(newCharacter))
                        {
                            Program.MainMenu();
                        }
                        break;
                    case "r":
                        messageHandler.Clear();
                        saveCharacterRunning = false;
                        CreateCharacter();
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        saveCharacterRunning = true;
                        break;
                    default:
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        saveCharacterRunning = true;
                        break;
                }
            }
            
        }

        private void DisplayCharacter(Character character)
        {
            messageHandler.Write("************************************************************");
            messageHandler.Write("                   HERE IS YOUR CHARACTER                   ");
            messageHandler.Write($"Name: {character.Name}");
            messageHandler.Write($"Class: {character.Class}");
            messageHandler.Write($"Background: {character.Background.ToString()}");
            messageHandler.Write($"Level: {character.Level}");
            WriteAbilities(character.Abilities);
            messageHandler.Write("************************************************************");
        }

        private Abilities SetAbilities(Character character)
        {
            var abilityPoints = 3;
            WriteAbilities(character.Abilities);

            while (abilityPoints > 0)
            {
                messageHandler.Write($"You have {abilityPoints} points to distribute among your abilities.");
                messageHandler.Write("Which ability would you like to increase? (S)trength, (D)exterity, (I)ntelligence, (W)isdom");
                switch (messageHandler.Read().ToLower())
                {
                    case "s":
                        messageHandler.Clear();
                        character.Abilities.Strength++;
                        abilityPoints--;
                        WriteAbilities(character.Abilities);
                        break;
                    case "d":
                        messageHandler.Clear();
                        character.Abilities.Dexterity++;
                        abilityPoints--;
                        WriteAbilities(character.Abilities);
                        break;
                    case "i":
                        messageHandler.Clear();
                        character.Abilities.Intelligence++;
                        abilityPoints--;
                        WriteAbilities(character.Abilities);
                        break;
                    case "w":
                        messageHandler.Clear();
                        character.Abilities.Wisdom++;
                        abilityPoints--;
                        WriteAbilities(character.Abilities);
                        break;
                    case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        break;
                    default:
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        break;
                }
            }
            return character.Abilities;
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

        private void WriteInfo()
        {
            var infoRunning = true;
            while (infoRunning)
            {
                messageHandler.Clear();
                messageHandler.Write("Chose class on which you want info: ");
                messageHandler.Write("(F)ighter, (T)hief, (M)agicUser, (H)ealer, (B)ack");
                switch (messageHandler.Read().ToLower())
                {
                    case "f":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        messageHandler.Write("Fighter: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("With 8 hit points, they stand tall and strong,");
                        messageHandler.Write("A shield of armor class 12, where foes belong.");
                        messageHandler.Write("Strength is their virtue, their might unparalleled,");
                        messageHandler.Write("A force of nature, with power unexcelled.");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                   case "t":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        messageHandler.Write("Thief: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("Fleet of foot with 5 hit points slight,");
                        messageHandler.Write("An armor class of 8, to dance through the fight.");
                        messageHandler.Write("Dexterity sharpens their wit and skill,");
                        messageHandler.Write("A shadow unseen, bending foes to their will.");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                   case "m":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        messageHandler.Write("MagicUser: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("With 4 hit points, they are frail but wise,");
                        messageHandler.Write("An armor class of 8, under watchful skies.");
                        messageHandler.Write("Intelligence fuels their arcane spark,");
                        messageHandler.Write("A conjurer of power, bending light and dark.");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                   case "h":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        messageHandler.Write("Healer: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("With 6 hit points, they carry the light,");
                        messageHandler.Write("An armor class of 10, defending what's right.");
                        messageHandler.Write("Wisdom flows through their gentle hands,");
                        messageHandler.Write("A balm for wounds across all lands.");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                   case "b":
                        messageHandler.Clear();
                        messageHandler.Write("Class? (F)ighter, (T)hief, (M)agicUser, (H)ealer, (I)nfo");
                        infoRunning = false;
                        break;
                   case string input when string.IsNullOrWhiteSpace(input):
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        Thread.Sleep(1000);
                        infoRunning = true;
                        break;
                    default:
                        messageHandler.Clear();
                        messageHandler.Write("Please enter a valid option.");
                        Thread.Sleep(1000);
                        infoRunning = true;
                        break;
                }
            }
            return;
        }
    }
}
