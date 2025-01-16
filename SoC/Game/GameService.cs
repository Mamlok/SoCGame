using Newtonsoft.Json;
using SoC.Adventures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoC.Game
{
    public class GameService
    {
        public void StartGame() 
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}adventures";
            var initialAdventure = new adventure();

            if (File.Exists($"{basePath}\\initial.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var InitialJsonFile = directory.GetFiles("initial.json");

                using (StreamReader fi = File.OpenText(InitialJsonFile[0].FullName))
                {
                    initialAdventure = JsonConvert.DeserializeObject<adventure>(fi.ReadToEnd());
                }

                Console.WriteLine($"Adventure : {initialAdventure.Title}");
                Console.WriteLine($"Adventure : {initialAdventure.Description}");

            }

        }
    }
}
