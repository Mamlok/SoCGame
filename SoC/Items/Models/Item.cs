﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items.Models
{
    public class Item
    {
        public ItemType Name;
        public string Description;
        public int ObjectiveNumber;
        public int GoldValue;
        public int HealthValue;
    }

    public enum ItemType
    {
        Torch,
        Food,
        Rope,
        HolySymbol,
        Water,
        TinderBox,
        Key,
        Lockpicks
        

    }


}
