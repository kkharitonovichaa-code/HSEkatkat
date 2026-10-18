namespace HSEkatkat.Models
{
    // Шлем
    public class Helmet : IInventoryItem
    {
        public string InventoryNumber { get; } 
        public string Name { get; }

        public Helmet(string inventoryNumber, string name)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
        }
    }
    // Наколенники
    public class KneePads : IInventoryItem
    {
        public string InventoryNumber { get; }
        public string Name { get; }

        public KneePads(string inventoryNumber, string name)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
        }
    }
    // Фонарик
    public class Flashlight : IInventoryItem, IEnergyConsumer
    {
        public string InventoryNumber { get; }
        public string Name { get; }
        public double DailyEnergyConsumption { get; }

        public Flashlight(string inventoryNumber, string name, double dailyEnergy)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
            DailyEnergyConsumption = dailyEnergy;
        }
    }
}