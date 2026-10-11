namespace VyshKat.Models;

public class Scooter : ElectricTransport
{
    public override string Kind => "Электросамокат";

    public Scooter(string name, string inventoryNumber, int beginnerRating,
        double dailyEnergy, bool isOperational = true)
        : base(name, inventoryNumber, beginnerRating, dailyEnergy, isOperational)
    {
    }
}
