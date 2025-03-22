using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game;
using SoC.Items;
using SoC.Items.Interfaces;
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
        private readonly IWeaponService weaponService;
        private readonly IArmorService armorService;

        public CharacterService(IMessageHandler messageHandler,IWeaponService weaponService, IArmorService armorService)
        {
            this.messageHandler = messageHandler;
            this.weaponService = weaponService;
            this.armorService = armorService;
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

        public List<Character> GetCharactersInRange()
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
                        if (potentialCharacterInRange.IsAlive)
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
            messageHandler.Write("\t\t[ENTER to START]");
            messageHandler.Read();
            messageHandler.Clear();

            var newCharacter = new Character();
            newCharacter.Inventory = new List<Item>();
            newCharacter.Weapons = new List<Weapon>();
            newCharacter.WeaponEquipped = new List<Weapon>();
            newCharacter.ArmorEquipped = new List<Armor>();
            newCharacter.Armors = new List<Armor>();

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
                        newCharacter.Armors.Add(armorService.GetArmor().FirstOrDefault(a => a.Description == "Worn chainmail"));
                        newCharacter.ArmorEquipped.Add(newCharacter.Armors[0]);
                        newCharacter.ArmorClass = newCharacter.ArmorEquipped[0].ArmorValue;
                        newCharacter.Weapons.Add(weaponService.GetWeapons().FirstOrDefault(w => w.Description == "Old rusted sword"));
                        newCharacter.WeaponEquipped.Add(newCharacter.Weapons[0]);
                        newCharacter.Attack = new Attack { BaseDie = Die.D8, BonusDamage = newCharacter.WeaponEquipped[0].DamageValue };
                        break;
                    case "t":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Thief;
                        newCharacter.HitPoints = 5;
                        newCharacter.Abilities.Dexterity = 1;
                        newCharacter.Armors.Add(armorService.GetArmor().FirstOrDefault(a => a.Description == "Ragged leather vest"));
                        newCharacter.ArmorEquipped.Add(newCharacter.Armors[0]);
                        newCharacter.ArmorClass = newCharacter.ArmorEquipped[0].ArmorValue;
                        newCharacter.Weapons.Add(weaponService.GetWeapons().FirstOrDefault(w => w.Description == "Old rusted dagger"));
                        newCharacter.WeaponEquipped.Add(newCharacter.Weapons[0]);
                        newCharacter.Attack = new Attack { BaseDie = Die.D4, BonusDamage = newCharacter.WeaponEquipped[0].DamageValue };
                        break;
                    case "m":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.MagicUser;
                        newCharacter.HitPoints = 4;
                        newCharacter.Abilities.Intelligence = 1;
                        newCharacter.Armors.Add(armorService.GetArmor().FirstOrDefault(a => a.Description == "Apprentice’s robe"));
                        newCharacter.ArmorEquipped.Add(newCharacter.Armors[0]);
                        newCharacter.ArmorClass = newCharacter.ArmorEquipped[0].ArmorValue;
                        newCharacter.Weapons.Add(weaponService.GetWeapons().FirstOrDefault(w => w.Description == "Old basic magic staff"));
                        newCharacter.WeaponEquipped.Add(newCharacter.Weapons[0]);
                        newCharacter.Attack = new Attack { BaseDie = Die.D4, BonusDamage = newCharacter.WeaponEquipped[0].DamageValue };
                        break;
                    case "h":
                        classChosen = true;
                        newCharacter.Class = CharacterClass.Healer;
                        newCharacter.HitPoints = 6;
                        newCharacter.Abilities.Wisdom = 1;
                        newCharacter.Armors.Add(armorService.GetArmor().FirstOrDefault(a => a.Description == "Blessed chainmail"));
                        newCharacter.ArmorEquipped.Add(newCharacter.Armors[0]);
                        newCharacter.ArmorClass = newCharacter.ArmorEquipped[0].ArmorValue;
                        newCharacter.Weapons.Add(weaponService.GetWeapons().FirstOrDefault(w => w.Description == "Old rusted rapier"));
                        newCharacter.WeaponEquipped.Add(newCharacter.Weapons[0]);
                        newCharacter.Attack = new Attack { BaseDie = Die.D6, BonusDamage = newCharacter.WeaponEquipped[0].DamageValue };
                        break;
                    case "i":
                        WriteInfoClass();
                        classChosen = false;
                        break;
                    default:
                        messageHandler.Write("Please enter a valid option.");
                        classChosen = false;
                        break;
                }
            }
            messageHandler.Clear();

            messageHandler.Write("Background? (D)rifter, (N)oble, (O)utcast, (I)nfo");
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
                    case "i":
                        backgroundChosen = false;
                        WriteInfoBackground();
                        break;
                    default:
                        messageHandler.Write("Please enter a valid option.");
                        backgroundChosen = false;
                        break;
                }
            }
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

        private void WriteInfoClass()
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
                        messageHandler.Write("Their attack is Vril, pure and immense,");
                        messageHandler.Write("A strike that grows in strength, leaving foes in suspense.");
                        messageHandler.Write("Starting equipment:");
                        messageHandler.Write("Old rusted sword – A blade weathered by time, dull yet still capable of cutting through foes.");
                        messageHandler.Write("Worn chainmail – A dented but sturdy armor, bearing the marks of past battles.");
                        messageHandler.Write("[ENTER to END]");
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
                        messageHandler.Write("They steal the armor class of those they betray,");
                        messageHandler.Write("Leaving enemies exposed, an easy prey.");
                        messageHandler.Write("Starting equipment:");
                        messageHandler.Write("Old rusted dagger – A light, well-worn blade that still finds its mark in the right hands.");
                        messageHandler.Write("Ragged leather vest – Tattered and flexible, favoring speed over protection.");
                        messageHandler.Write("[ENTER to END]");
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
                        messageHandler.Write("They cast a powerful spell with dazzling might,");
                        messageHandler.Write("Shaping the battle in radiant light.");
                        messageHandler.Write("Starting equipment:");
                        messageHandler.Write("Old basic magic staff – A staff worn smooth with age, yet still pulsing faintly with arcane energy.");
                        messageHandler.Write("Apprentice’s robe – A simple cloth robe, more for tradition than protection.");
                        messageHandler.Write("[ENTER to END]");
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
                        messageHandler.Write("When in peril, they heal themselves anew,");
                        messageHandler.Write("Restoring their strength to see the battle through.");
                        messageHandler.Write("Starting equipment:");
                        messageHandler.Write("Old rusted rapier – A slender, slightly dulled blade, more precise than powerful, yet effective in practiced hands.");
                        messageHandler.Write("Blessed chainmail – A light armor infused with divine protection, offering resilience to corruption.");
                        messageHandler.Write("[ENTER to END]");
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

        private void WriteInfoBackground()
        {
            var infoRunning = true;
            while (infoRunning)
            {
                messageHandler.Clear();
                messageHandler.Write("Chose background on which you want info: ");
                messageHandler.Write("(N)oble, (D)rifter, (O)utcast, (B)ack");
                switch (messageHandler.Read().ToLower())
                {
                    case "n":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        messageHandler.Write("Noble: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("Born of lineage, with banners held high,");
                        messageHandler.Write("The weight of a name beneath the sky.");
                        messageHandler.Write("Elegance and duty, their burdens entwine,");
                        messageHandler.Write("A legacy of honor in every sign.");
                        messageHandler.Write("From gilded halls to the fray untold,");
                        messageHandler.Write("Their destiny is forged with a heart of gold.");
                        messageHandler.Write("[ENTER to END]");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                    case "d":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        messageHandler.Write("Drifter: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("A wanderer beneath the stars' embrace,");
                        messageHandler.Write("No home, no anchor, just endless space.");
                        messageHandler.Write("With tales untold and roads unpaved,");
                        messageHandler.Write("A life of freedom, the heart unshaved.");
                        messageHandler.Write("They tread where others dare not roam,");
                        messageHandler.Write("The world their path, the unknown their home.");
                        messageHandler.Write("[ENTER to END]");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                    case "o":
                        messageHandler.Clear();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        messageHandler.Write("Outcast: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        messageHandler.Write("Marked by whispers, their shadow looms,");
                        messageHandler.Write("A tale of exile, of silent dooms.");
                        messageHandler.Write("Shunned by the world, yet fiercely they rise,");
                        messageHandler.Write("Fire in their heart and steel in their eyes.");
                        messageHandler.Write("From ashes and ruin, they carve their way,");
                        messageHandler.Write("Defying the night to reclaim the day.");
                        messageHandler.Write("[ENTER to END]");
                        messageHandler.Read();
                        infoRunning = true;
                        break;
                    case "b":
                        messageHandler.Clear();
                        messageHandler.Write("Background? (D)rifter, (N)oble, (O)utcast, (I)nfo");
                        infoRunning = false;
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
