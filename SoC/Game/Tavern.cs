using SoC.Entities.Interfaces;
using SoC.Entities.Model;
using SoC.Game.Interfaces;
using SoC.Items;
using SoC.Items.Interfaces;
using SoC.Items.Models;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoC.Game
{
    public class Tavern : ITavern
    {
        
        private readonly IMessageHandler messageHandler;
        private readonly ICharakterService charakterService;
        private readonly ICharakterInfo charakterInfo;
        private readonly IItemService itemService;
        private readonly IWeaponService weaponService;
        private readonly IArmorService armorService;

        public List<Item> Items = new List<Item>();
        public List<Weapon> Weapons = new List<Weapon>();
        public List<Armor> Armors = new List<Armor>();

        public Tavern(IMessageHandler MessageHandler, ICharakterService CharakterService, ICharakterInfo CharakterInfo, IWeaponService WeaponService, IArmorService ArmorService, IItemService ItemService)
        {
            messageHandler = MessageHandler;
            charakterService = CharakterService;
            charakterInfo = CharakterInfo;
            weaponService = WeaponService;
            armorService = ArmorService;
            itemService = ItemService;

            Items = itemService.GetItems().ToList();
            Weapons = weaponService.GetWeapons().ToList();
            Armors = armorService.GetArmor().ToList();
        }
        



        public void NPCInteraction(Character character,int adventureNumber)
        {
            bool isRunning = true;
            while (isRunning)
            {
                NPCMenu();
                var playerDecision = messageHandler.Read().ToLower();
                switch (playerDecision)
                {
                    case "t":
                        NPCInteractionText(1, character, adventureNumber);
                        isRunning = true;
                        break;
                    case "g":
                        NPCInteractionText(2, character, adventureNumber);
                        isRunning = true;
                        break;
                    case "e":
                        NPCInteractionText(3, character, adventureNumber);
                        isRunning = true;
                        break;
                    case "r":
                        NPCInteractionText(4, character, adventureNumber);
                        isRunning = true;
                        break;
                    case "d":
                        NPCInteractionText(5, character, adventureNumber);
                        isRunning = true;
                        break;
                    case "b":
                        isRunning = false;
                        break;
                    case "i":
                        isRunning = true;
                        charakterInfo.ShowCharakterInfo(character);
                        break;
                    default:
                        messageHandler.Write("Invalid option, please try again.");
                        isRunning = true;
                        break;
                }
            }
            TavernMenu(character, adventureNumber);
        }

        public void TavernMenu(Character character, int adventureNumber)
        {
            messageHandler.Clear();
            TavernMenuOptions(character);
            bool menuOptions = true;
            var playerDecision = messageHandler.Read().ToLower();
            while (menuOptions)
            {
                switch (playerDecision)
                {
                    case "q":
                        menuOptions = false;
                        break;
                    case "s":
                        menuOptions = false;
                        TavernShop(character, adventureNumber, Items, Weapons, Armors);
                        break;
                    case "v":
                        menuOptions = false;
                        NPCInteraction(character, adventureNumber);
                        break;
                    case "i":
                        menuOptions = false;
                        charakterInfo.ShowCharakterInfo(character);
                        TavernMenu(character, adventureNumber);
                        break;
                    default:
                        messageHandler.Write("Invalid option, please try again.");
                        menuOptions = true;
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                }
            }
        }

        public void TavernShop(Character character, int adventureNumber, List<Item> Items, List<Weapon> Weapons, List<Armor> Armors)
        {
            List<Item> itemList = new List<Item>();
            List<Weapon> weaponList = new List<Weapon>();
            List<Armor> armorList = new List<Armor>();
            if (adventureNumber == 1)
            {
                itemList.Add(Items[0]);
                itemList.Add(Items[1]);
                weaponList.Add(Weapons[4]);
                armorList.Add(Armors[4]);
            }

            bool isRunning = true;
            while (isRunning)
            {
                ShopText(character, adventureNumber);
                var playerChoice = messageHandler.Read().ToLower();
                switch (playerChoice)
                {
                    case "i":
                        ShopItemText(itemList);
                        ShopItemChoice(itemList, character);
                        isRunning = true;
                        break;
                    case "a":
                        ShopArmorText(armorList);
                        ShopArmorChoice(armorList, character);
                        isRunning = true;
                        break;
                    case "w":
                        ShopWeaponText(weaponList);
                        ShopWeaponChoice(weaponList, character);
                        isRunning = true;
                        break;
                    case "b":
                        isRunning = false;
                        TavernMenu(character, adventureNumber);
                        break;
                    default:
                        messageHandler.Write("Invalid choice");
                        isRunning = true;
                        break;

                }
            }


        }

        private void TavernMenuOptions(Character character)
        {
            messageHandler.Write(@" ______   ____  __ __    ___  ____    ____  ");
            messageHandler.Write(@"|      | /    ||  |  |  /  _]|    \  |    \ ");
            messageHandler.Write(@"|      ||  o  ||  |  | /  [_ |  D  ) |  _  |");
            messageHandler.Write(@"|_|  |_||     ||  |  ||    _]|    /  |  |  |");
            messageHandler.Write(@"  |  |  |  _  ||  :  ||   [_ |    \  |  |  |");
            messageHandler.Write(@"  |  |  |  |  | \   / |     ||  .  \ |  |  |");
            messageHandler.Write(@"  |__|  |__|__|  \_/  |_____||__|\\_||__|__|");
            messageHandler.Write("----------------------------------------------");
            messageHandler.Write("Go on your (Q)uest");
            if (character.shopOpen)
            {
                messageHandler.Write("Visit the Gideon´s (S)hop");
            }
            messageHandler.Write("Talk to the (V)illiagers");
            messageHandler.Write("Character (I)nfo");
        }

        private void NPCMenu()
        {
            messageHandler.Clear();
            messageHandler.Write("Talk to worried farmer - (T)homas");
            messageHandler.Write("Talk to suspicious merchant - (G)ideon");
            messageHandler.Write("Talk to nervous child - (E)liza");
            messageHandler.Write("Talk to village hunter - (R)owan");
            messageHandler.Write("Talk to barkeep - (D)uncan");
            messageHandler.Write("Character (I)nfo");
            messageHandler.Write("(B)ack");
        }

        private void NPCInteractionText(int npc, Character character, int adventureNumber)
        {
            if (adventureNumber == 1)
            {

                if (npc == 1)
                {
                    messageHandler.Clear();
                    messageHandler.Write("[Worried farmer - Thomas]", false);
                    messageHandler.Read();
                    messageHandler.Write("T: You new around here?", false);
                    messageHandler.Read();
                    messageHandler.Write("T: Name’s Thomas.", false);
                    messageHandler.Read();
                    messageHandler.Write("T: I run a small farm out past the village", false);
                    messageHandler.Read();
                    messageHandler.Write("T: If you’re heading toward Harlan’s place, best watch yourself.", false);
                    messageHandler.Read();
                    messageHandler.Write("T: Strange howls been coming from the woods lately...", false);
                    messageHandler.Read();
                    messageHandler.Write("T: and my livestock’s been uneasy.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: What’s causing it?", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("T: Ain’t normal wolves, I tell ya.", false);
                    messageHandler.Read();
                    messageHandler.Write("T: They move different, like they got a purpose.", false);
                    messageHandler.Read();
                    messageHandler.Write("T: Something’s off.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: I’ll look into it.", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("T: Harlan’s already looking into it.", false);
                    messageHandler.Read();
                    messageHandler.Write("T: Maybe you can lend a hand?");
                    messageHandler.Write("[ENTER to END]", false);
                    messageHandler.Read();
                }
                else if (npc == 2)
                {
                    messageHandler.Clear();
                    messageHandler.Write("[Suspicious merchant - Gideon]", false);
                    messageHandler.Read();
                    messageHandler.Write("G: Hah, another traveler thinking they’ll make a name for themselves?", false);
                    messageHandler.Read();
                    messageHandler.Write("G: Name’s Gideon.", false);
                    messageHandler.Read();
                    messageHandler.Write("G: Been in this village long enough to know when trouble’s brewing.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: What kind of trouble?", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("G: That forest.", false);
                    messageHandler.Read();
                    messageHandler.Write("G: Best stay clear.", false);
                    messageHandler.Read();
                    messageHandler.Write("G: I’ve seen too many fools walk in and never come back.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: I might need supplies.", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("G: If you got coin, I might have something useful.", false);
                    messageHandler.Read();
                    messageHandler.Write("G: But don’t expect charity.");
                    character.shopOpen = true;
                    messageHandler.Write("[ENTER to END]", false);
                    messageHandler.Read();

                }
                else if (npc == 3)
                {
                    messageHandler.Clear();
                    messageHandler.Write("[Nervous child - Eliza]", false);
                    messageHandler.Read();
                    messageHandler.Write("E: Have you seen the shadows move at night?", false);
                    messageHandler.Read();
                    messageHandler.Write("E: My pa says it’s just my imagination,", false);
                    messageHandler.Read();
                    messageHandler.Write("E: but I know what I saw!", false);
                    messageHandler.Read();
                    messageHandler.Write("E: I’m Eliza, by the way.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: What did you see?", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("E: A big shadow, taller than a man!", false);
                    messageHandler.Read();
                    messageHandler.Write("E: And glowing eyes, peering from the trees...", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: That sounds scary. Stay inside at night.", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("E: You think so?", false);
                    messageHandler.Read();
                    messageHandler.Write("E: Maybe you can make the shadows go away.");
                    messageHandler.Write("[ENTER to END]", false);
                    messageHandler.Read();
                }
                else if (npc == 4)
                {
                    messageHandler.Clear();
                    messageHandler.Write("[Village hunter - Rowan]", false);
                    messageHandler.Read();
                    messageHandler.Write("R: You planning to go into those woods?", false);
                    messageHandler.Read();
                    messageHandler.Write("R: Name’s Rowan.", false);
                    messageHandler.Read();
                    messageHandler.Write("R: I’ve been hunting here since I could hold a bow.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: What do you know about the wolves?", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("R: They ain't normal.", false);
                    messageHandler.Read();
                    messageHandler.Write("R: They hunt together, too well-organized. I’ve seen packs,", false);
                    messageHandler.Read();
                    messageHandler.Write("R: but never like this. Something's driving them.", false);
                    messageHandler.Read();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    messageHandler.Write("Y: Have you fought them?", false);
                    Console.ForegroundColor = ConsoleColor.White;
                    messageHandler.Read();
                    messageHandler.Write("R: Took a shot at one last night.", false);
                    messageHandler.Read();
                    messageHandler.Write("R: Arrow hit, but the damn thing barely flinched.", false);
                    messageHandler.Read();
                    messageHandler.Write("R: If you're going after them, take something stronger than a bow.");
                    messageHandler.Write("[ENTER to END]", false);
                    messageHandler.Read();

                }
                else if (npc == 5)
                {
                    messageHandler.Clear();
                    messageHandler.Write("[Barkeep - Duncan]", false);
                    messageHandler.Read();
                    messageHandler.Write("D: Name’s Duncan.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: I run this place.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: I’ve seen travelers come and go, but lately, something feels off.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: People talk of strange figures near the woods,", false);
                    messageHandler.Read();
                    messageHandler.Write("D: and at night, the howling sounds unnatural.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: I don’t know what’s lurking out there,", false);
                    messageHandler.Read();
                    messageHandler.Write("D: but it’s got the village on edge.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: If you plan on sticking around,", false);
                    messageHandler.Read();
                    messageHandler.Write("D: keep your wits about you.", false);
                    messageHandler.Read();
                    messageHandler.Write("D: Trouble has a way of finding the unprepared.");
                    messageHandler.Write("[ENTER to END]", false);
                    messageHandler.Read();
                }
            }
            charakterService.SaveCharacter(character);


        }

        private void ShopText(Character character,int adventureNumber)
        {
            messageHandler.Clear();
            messageHandler.Write("====================================");
            messageHandler.Write("       Welcome to Gideon's Shop     ");
            messageHandler.Write("====================================");
            messageHandler.Write("What would you like to buy?\n");
            messageHandler.Write("(I)tems, (A)rmor, (W)eapons");
            messageHandler.Write("(B)ack");

        }

        private void ShopItemText(List<Item> itemList)
        {
            var counter = 0;
            foreach (var item in itemList)
            {
                messageHandler.Write("*********************************");
                messageHandler.Write($"Name: ({counter}). {item.Description}");
                messageHandler.Write($"Value: {item.GoldValue} Gold");
                if (item.HealthValue > 0)
                {
                    messageHandler.Write($"Health Value: {item.HealthValue} HP");
                }
                counter++;
            }
            messageHandler.Write("*********************************");
        }
        private void ShopArmorText(List<Armor> armorList)
        {
            var counter = 0;
            foreach (var item in armorList)
            {
                messageHandler.Write("*********************************");
                messageHandler.Write($"Name: ({counter}). {item.Description}");
                messageHandler.Write($"Value: {item.GoldValue} Gold");
                messageHandler.Write($"ArmorClass: {item.ArmorValue}");
                counter++;
            }
            messageHandler.Write("*********************************");
        }
        private void ShopWeaponText(List<Weapon> weaponList)
        {
            var counter = 0;
            foreach (var item in weaponList)
            {
                messageHandler.Write("*********************************");
                messageHandler.Write($"Name: ({counter}). {item.Description}");
                messageHandler.Write($"Value: {item.GoldValue} Gold");
                messageHandler.Write($"Damage: {item.DamageValue} HP");
                counter++;
            }
            messageHandler.Write("*********************************");
        }
        private void ShopItemChoice(List<Item> itemList, Character character)
        {
            bool isRunning = true;
            while (isRunning)
            {
                var playerChoice = Convert.ToInt32(messageHandler.Read());
                if (character.Gold < itemList[playerChoice].GoldValue)
                {
                    messageHandler.Write("You dont have enough gold");
                    isRunning = true;
                }
                else
                {
                    character.Inventory.Add(itemList[playerChoice]);
                    messageHandler.Write("Your item was added succesfully");
                    character.Gold -= itemList[playerChoice].GoldValue;
                    messageHandler.Read();
                    isRunning = false;

                }
            }
        }
        private void ShopWeaponChoice(List<Weapon> weaponList, Character character)
        {
            bool isRunning = true;
            while (isRunning)
            {
                var playerChoice = Convert.ToInt32(messageHandler.Read());
                if (character.Gold < weaponList[playerChoice].GoldValue)
                {
                    messageHandler.Write("You dont have enough gold");
                    isRunning = true;
                }
                else
                {
                    character.Weapons.Add(weaponList[playerChoice]);
                    messageHandler.Write("Your item was added succesfully");
                    character.Gold -= weaponList[playerChoice].GoldValue;
                    messageHandler.Read();
                    isRunning = false;

                }
            }
        }
        private void ShopArmorChoice(List<Armor> armorList, Character character)
        {
            bool isRunning = true;
            while (isRunning)
            {
                var playerChoice = Convert.ToInt32(messageHandler.Read());
                if (character.Gold < armorList[playerChoice].GoldValue)
                {
                    messageHandler.Write("You dont have enough gold");
                    isRunning = true;
                }
                else
                {
                    character.Armors.Add(armorList[playerChoice]);
                    messageHandler.Write("Your item was added succesfully");
                    character.Gold -= armorList[playerChoice].GoldValue;
                    messageHandler.Read();
                    isRunning = false;

                }
            }
        }
    }
}
