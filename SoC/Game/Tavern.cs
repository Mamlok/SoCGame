using SoC.Game.Interfaces;
using SoC.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class Tavern : ITavern
    {
        private readonly IMessageHandler messageHandler;

        public Tavern(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }


        public void NPCInteraction()
        {
            throw new NotImplementedException();
        }

        public void TavernMenu()
        {
            messageHandler.Clear();
            TavernMenuOptions();
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
                        TavernShop();
                        break;
                    case "v":
                        menuOptions = false;
                        NPCInteraction();
                        break;
                    default:
                        messageHandler.Write("Invalid option, please try again.");
                        menuOptions = true;
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                }
            }
        }

        public void TavernShop()
        {
            throw new NotImplementedException();
        }

        private void TavernMenuOptions()
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
            messageHandler.Write("Visit the barkeeps (S)hop");
            messageHandler.Write("Talk to the (V)illiagers");
        }
    }
}
