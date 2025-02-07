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
    public class ArmorService : IArmorService
    {
        public List<Armor> GetArmor()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}Items";
            List<Armor> armorList = new List<Armor>();

            if (File.Exists($"{basePath}\\Armors.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var JsonFile = directory.GetFiles($"Armors.json");

                using (StreamReader fi = File.OpenText(JsonFile[0].FullName))
                {
                    armorList = JsonConvert.DeserializeObject<List<Armor>>(fi.ReadToEnd());
                }
            }
            return armorList;
        }
    }
}
