﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class Dice
    {
        public int RollDice(List<Die> DiceToRoll)
        {
            var randomRoller = new Random();
            var total = 0;
            foreach (var die in DiceToRoll)
            {
                total += randomRoller.Next(1, (int)die+1);
            }
            return total;
        }


    }

    public enum Die
    {
        D2 = 2,
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D60 = 60,
        D100 = 100
    }
}
