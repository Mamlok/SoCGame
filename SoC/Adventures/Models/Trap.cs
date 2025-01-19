using SoC.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures.Models
{
    public class Trap
    {
        public TrapType TrapType;
        public Die DamageDie = Die.D4;
        public bool SearchedFor = false;
        public bool TrippedOrDisarmed = false;

    }

    public enum TrapType
    {
        Pit,
        Poison,
        Spike
    }
}
