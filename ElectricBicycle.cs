namespace VyshKat.Models;

public class Bicycle : Transport
{
    public override string Kind => "Велосипед";

    public Bicycle(string name, string inventoryNumber, int beginnerRating, bool isOperational = true)
        : base(name, inventoryNumber, beginnerRating, isOperational)
    {
    }
}
