namespace HSEkatkat.Models
{
    // 1. Всё, что имеет инвентарный номер
    public interface IInventoryItem
    {
        string InventoryNumber { get; }
        string Name { get; }
    }

    // 2. Всё, что потребляет энергию
    public interface IEnergyConsumer
    {
        double DailyEnergyConsumption { get; } // кВт·ч в сутки
    }

    // 3. Всё, что можно выдать новичку
    public interface IBeginnerFriendly
    {
        int BeginnerScore { get; }
    }

    // Техосмотр
    public interface IServiceCenter
    {
        bool CanAccept(IInventoryItem? item);
    }

    // Фабрика создания предметов
    public interface IItemFactory
    {
        IInventoryItem CreateItem(int typeChoice, string inventoryNumber, string name, double energy = 0,
            int beginnerScore = 0);

        string[] GetAvailableTypes();
        bool HasEnergy(int typeChoice);
        bool HasBeginnerScore(int typeChoice);
    }
}