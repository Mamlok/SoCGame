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
        public Guid GUID;
        public string Title;
        public string Description;
        public int CompleteXpReward;
        public int CompleteGoldReward;
        public int FinalObjective;
        public List<Room> Rooms;
        public List<Npc> Npcs;

        public Adventure() 
        {


        }

    }
}
