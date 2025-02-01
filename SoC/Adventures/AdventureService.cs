﻿using Newtonsoft.Json;
using SoC.Adventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Adventures
{
    public class AdventureService : IAdventureService
    {

        public Adventure GetAdventure(string name)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}adventures";
            var Adventure = new Adventure();

            if (File.Exists($"{basePath}\\{name}.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var JsonFile = directory.GetFiles($"{name}.json");

                using (StreamReader fi = File.OpenText(JsonFile[0].FullName))
                {
                    Adventure = JsonConvert.DeserializeObject<Adventure>(fi.ReadToEnd());
                }
            }
            return Adventure;
        }
    }
}
