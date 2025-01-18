using SoC.Adventures.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures
{
    public class Adventure
    {
        public string GUID;
        public string Title;
        public string Description;
        public int CompleteXpReward;
        public int CompleteEddiesReward;
        public int MaxLevel;
        public int MinLevel;
        public List<Room> Rooms;

        public Adventure() 
        {


        }

    }
}
