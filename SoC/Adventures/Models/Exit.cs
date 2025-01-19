using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures.Models
{
    public class Exit
    {
        public bool Locked = false;
        public CompassDirection WallLocation;
        public int LeadsToRoomNumber;
        public Riddle Riddle;

    }

    public enum CompassDirection
    {
        North,
        East,
        South,
        West
    }
}
