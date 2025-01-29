using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
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

            messageHandler.Write("What is your character's name?");
            newCharacter.Name = messageHandler.Read();
            messageHandler.Clear();

            messageHandler.Write("Class? (F)ighter, (T)hief, (M)agicUser, (H)ealer"); //vysvětlit
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
                        break;
                    case "t":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Thief;
                        newCharacter.HitPoints = 5;
                        newCharacter.Abilities.Dexterity = 1;
                        break;
                    case "m":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.MagicUser;
                        newCharacter.HitPoints = 4;
                        newCharacter.Abilities.Intelligence = 1;
                        break;
                    case "h":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Healer;
                        newCharacter.HitPoints = 6;
                        newCharacter.Abilities.Wisdom = 1;
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

            messageHandler.Write("Background : ");
            newCharacter.Background = messageHandler.Read();
            messageHandler.Clear();

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
            messageHandler.Write($"Background: {character.Background}");
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
    }
}
