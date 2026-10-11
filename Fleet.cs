namespace VyshKat.Models;

public abstract class Transport : InventoryObject
{
    public int BeginnerRating { get; }
    public bool IsOperational { get; }

    protected Transport(string name, string inventoryNumber, int beginnerRating, bool isOperational)
        : base(name, inventoryNumber)
    {
        if (beginnerRating < 1 || beginnerRating > 10)
        {
            throw new ArgumentException("Простота для новичка должна быть от 1 до 10");
        }

        BeginnerRating = beginnerRating;
        IsOperational = isOperational;
    }
}
