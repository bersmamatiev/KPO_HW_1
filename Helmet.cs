namespace VyshKat.Models;

public class ElectricBicycle : ElectricTransport
{
    public override string Kind => "Электровелосипед";

    public ElectricBicycle(string name, string inventoryNumber, int beginnerRating,
        double dailyEnergy, bool isOperational = true)
        : base(name, inventoryNumber, beginnerRating, dailyEnergy, isOperational)
    {
    }
}
