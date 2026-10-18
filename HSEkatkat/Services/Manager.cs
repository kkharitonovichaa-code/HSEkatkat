namespace HSEkatkat.Services
{
    using System;
    using System.Collections.Generic;
    using HSEkatkat.Models;

    public class Manager
    {
        private readonly List<IInventoryItem> _inventory = new();
        private readonly IServiceCenter _serviceCenter;
        public const int BeginnerScoreThreshold = 6;
        
        // конструктор
        public Manager(IServiceCenter serviceCenter)
        {
            if (serviceCenter == null)
            {
                throw new ArgumentNullException(nameof(serviceCenter));
            }
            _serviceCenter = serviceCenter;
        }
        
        // добавляеем экземпляр
        public bool AddItem(IInventoryItem item)
        {
            if (!_serviceCenter.CanAccept(item))
            {
                return false;
            }
            foreach (var existItem in _inventory)
            {
                if (existItem.InventoryNumber == item.InventoryNumber)
                {
                    return false;
                }
            }
            _inventory.Add(item);
            return true;
        }
        
        // считаем сумарную дневную энергию
        public double GetTotalDailyEnergy()
        {
            double sum = 0;
            foreach (var item in _inventory)
            {
                if (item is IEnergyConsumer consumer)
                {
                    sum += consumer.DailyEnergyConsumption;
                }
            }
            return sum;
        }
        
        //список для новичков
        public IReadOnlyList<IInventoryItem> GetBeginnerFriendlyItems()
        {
            var result = new List<IInventoryItem>();
            foreach (var item in _inventory)
            {
                if (item is IBeginnerFriendly beginner && beginner.BeginnerScore >= BeginnerScoreThreshold)
                {
                    result.Add(item);
                }
            }
            return result;
        }
        
        // Получение списка инвентаря
        public IReadOnlyList<IInventoryItem> GetAllItems()
        {
            return _inventory.AsReadOnly();
        }
    }
}