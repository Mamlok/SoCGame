﻿using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items.Interfaces
{
    public interface IWeaponService
    {
        List<Weapon> GetWeapons();
    }
}
