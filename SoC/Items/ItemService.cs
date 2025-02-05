using Newtonsoft.Json;
using SoC.Adventures;
using SoC.Items.Interfaces;
using SoC.Items.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Items
{
    public class ItemService : IItemService
    {
        public List<Item> GetItems()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}Items";
            List<Item> itemList = new List<Item>();

            if (File.Exists($"{basePath}\\Items.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var JsonFile = directory.GetFiles($"Items.json");

                using (StreamReader fi = File.OpenText(JsonFile[0].FullName))
                {
                    itemList = JsonConvert.DeserializeObject<List<Item>>(fi.ReadToEnd());
                }
            }
            return itemList;
        }
    }
}
