namespace VyshKat.Models;

public abstract class Thing : InventoryObject
{
    protected Thing(string name, string inventoryNumber) : base(name, inventoryNumber)
    {
    }
}
