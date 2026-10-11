namespace VyshKat.Models;

public abstract class ElectricTransport : Transport, IEnergyConsumer
{
    public double DailyEnergy { get; }

    protected ElectricTransport(string name, string inventoryNumber, int beginnerRating,
        double dailyEnergy, bool isOperational)
        : base(name, inventoryNumber, beginnerRating, isOperational)
    {
        if (!double.IsFinite(dailyEnergy) || dailyEnergy < 0)
        {
            throw new ArgumentException("Расход энергии должен быть конечным и неотрицательным");
        }

        DailyEnergy = dailyEnergy;
    }
}
