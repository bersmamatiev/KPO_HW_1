namespace VyshKat.Models;

public abstract class InventoryObject : IInventoryItem
{
    public string Name { get; }
    public string InventoryNumber { get; }
    public abstract string Kind { get; }

    protected InventoryObject(string name, string inventoryNumber)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(inventoryNumber))
        {
            throw new ArgumentException("Название и инвентарный номер не должны быть пустыми");
        }

        Name = name.Trim();
        InventoryNumber = inventoryNumber.Trim();
    }
}
