using Newtonsoft.Json;
using SoC.Items.Interfaces;
using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items
{
    public class WeaponService : IWeaponService
    {
        public List<Weapon> GetWeapons()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}Items";
            List<Weapon> weaponList = new List<Weapon>();

            if (File.Exists($"{basePath}\\Weapons.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var JsonFile = directory.GetFiles($"Weapons.json");

                using (StreamReader fi = File.OpenText(JsonFile[0].FullName))
                {
                    weaponList = JsonConvert.DeserializeObject<List<Weapon>>(fi.ReadToEnd());
                }
            }
            return weaponList;
        }
    }
}
