namespace HSEkatkat.Services
{
    using HSEkatkat.Models;

    public class ServiceCenter : IServiceCenter
    {
        public bool CanAccept(IInventoryItem? item)
        {
            if (item == null)
                return false;
            
            return !(string.IsNullOrWhiteSpace(item.InventoryNumber));
        }
    }
}