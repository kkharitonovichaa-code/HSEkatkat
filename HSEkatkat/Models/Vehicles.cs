namespace HSEkatkat.Models
{
    // Самокат
    public class Scooter : IInventoryItem, IEnergyConsumer, IBeginnerFriendly
    {
        public string InventoryNumber { get; }
        public string Name { get; }
        public double DailyEnergyConsumption { get; }
        public int BeginnerScore { get; }

        public Scooter(string inventoryNumber, string name, double dailyEnergy, int beginnerScore)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
            DailyEnergyConsumption = dailyEnergy;
            BeginnerScore = beginnerScore;
        }
    }

    // Велосипед
    public class Bike : IInventoryItem, IBeginnerFriendly
    {
        public string InventoryNumber { get; }
        public string Name { get; }
        public int BeginnerScore { get; }

        public Bike(string inventoryNumber, string name, int beginnerScore)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
            BeginnerScore = beginnerScore;
        }
    }

    // Электровелосипед
    public class EBike : IInventoryItem, IEnergyConsumer, IBeginnerFriendly
    {
        public string InventoryNumber { get; }
        public string Name { get; }
        public double DailyEnergyConsumption { get; }
        public int BeginnerScore { get; }

        public EBike(string inventoryNumber, string name, double dailyEnergy, int beginnerScore)
        {
            InventoryNumber = inventoryNumber;
            Name = name;
            DailyEnergyConsumption = dailyEnergy;
            BeginnerScore = beginnerScore;
        }
    }
}