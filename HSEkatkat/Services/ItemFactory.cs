namespace HSEkatkat.Services
{
    using System;
    using System.Linq;
    using HSEkatkat.Models;

    public class ItemFactory : IItemFactory
    {
        private static readonly (int Id, string Name, bool HasEnergy, bool HasBeginnerScore)[] ItemMetadata = 
        {
            (1, "Самокат", true, true),
            (2, "Велосипед", false, true),
            (3, "Электровелосипед", true, true),
            (4, "Шлем", false, false),
            (5, "Наколенники", false, false),
            (6, "Фонарик", true, false)
        };

        public string[] GetAvailableTypes()
        {
            return ItemMetadata.Select(m => m.Name).ToArray();
        }

        public bool HasEnergy(int typeChoice)
        {
            return ItemMetadata.FirstOrDefault(m => m.Id == typeChoice).HasEnergy;
        }

        public bool HasBeginnerScore(int typeChoice)
        {
            return ItemMetadata.FirstOrDefault(m => m.Id == typeChoice).HasBeginnerScore;
        }

        public IInventoryItem CreateItem(int typeChoice, string inventoryNumber, string name, double energy = 0, int beginnerScore = 0)
        {
            return typeChoice switch
            {
                1 => new Scooter(inventoryNumber, name, energy, beginnerScore),
                2 => new Bike(inventoryNumber, name, beginnerScore),
                3 => new EBike(inventoryNumber, name, energy, beginnerScore),
                4 => new Helmet(inventoryNumber, name),
                5 => new KneePads(inventoryNumber, name),
                6 => new Flashlight(inventoryNumber, name, energy),
                _ => throw new ArgumentException("Неизвестный тип техники")
            };
        }
    }
}