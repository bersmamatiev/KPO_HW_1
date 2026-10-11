namespace VyshKat.Models;

public class ChargingStation : Thing, IEnergyConsumer
{
    public override string Kind => "Зарядная станция";
    public double DailyEnergy { get; }

    public ChargingStation(string name, string inventoryNumber, double dailyEnergy)
        : base(name, inventoryNumber)
    {
        if (!double.IsFinite(dailyEnergy) || dailyEnergy < 0)
        {
            throw new ArgumentException("Расход энергии должен быть конечным и неотрицательным");
        }

        DailyEnergy = dailyEnergy;
    }
}
