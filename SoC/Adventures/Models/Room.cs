using SoC.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures.Models
{
    public class Room
    {
        public int RoomNumber;
        public string Description;
        public string SubDescription;
        public Trap Trap;
        public List<Monster> Monsters;
        public Chest Chest;
        public List<Exit> Exits;
        public List<Event> Events;
        public bool Hidden;
        public int RoomVisited = 0;
    }
}
