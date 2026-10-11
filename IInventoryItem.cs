namespace VyshKat.Models;

public class Helmet : Thing
{
    public override string Kind => "Шлем";

    public Helmet(string name, string inventoryNumber) : base(name, inventoryNumber)
    {
    }
}
