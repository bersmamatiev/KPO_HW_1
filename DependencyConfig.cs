using System.Globalization;
using VyshKat.Models;
using VyshKat.Services;

namespace VyshKat;

public class ConsoleApplication
{
    private readonly Fleet _fleet;
    private readonly TextReader _input;
    private readonly TextWriter _output;

    public ConsoleApplication(Fleet fleet, TextReader input, TextWriter output)
    {
        _fleet = fleet;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        while (true)
        {
            _output.WriteLine("\nВышКат");
            _output.WriteLine("1 - добавить транспорт");
            _output.WriteLine("2 - добавить вещь");
            _output.WriteLine("3 - отчёт по количеству и энергии");
            _output.WriteLine("4 - транспорт для новичков");
            _output.WriteLine("5 - весь инвентарь");
            _output.WriteLine("0 - выход");
            _output.Write("Выбор: ");
            string? choice = _input.ReadLine()?.Trim();
            if (choice is null || choice == "0")
            {
                return;
            }

            try
            {
                switch (choice)
                {
                    case "1":
                        AddTransport();
                        break;
                    case "2":
                        AddThing();
                        break;
                    case "3":
                        PrintReport();
                        break;
                    case "4":
                        PrintItems(_fleet.GetBeginnerTransports());
                        break;
                    case "5":
                        PrintItems(_fleet.GetInventory());
                        break;
                    default:
                        _output.WriteLine("Неизвестный пункт меню");
                        break;
                }
            }
            catch (Exception error) when (error is ArgumentException or FormatException or OverflowException)
            {
                _output.WriteLine($"Ошибка: {error.Message}");
            }
            catch (EndOfStreamException)
            {
                _output.WriteLine("Ввод завершён");
                return;
            }
        }
    }

    public void RunDemo()
    {
        _output.WriteLine("ВышКат: демонстрация");
        PrintAdmission(new Scooter("Самокат A", "T-001", 8, 2.5));
        PrintAdmission(new Bicycle("Велосипед B", "T-002", 5));
        PrintAdmission(new ElectricBicycle("Электровелосипед C", "T-003", 6, 1.5));
        PrintAdmission(new Scooter("Неисправный самокат", "T-004", 9, 3.0, false));
        _fleet.AddThing(new Helmet("Шлем", "V-001"));
        _fleet.AddThing(new ChargingStation("Станция", "V-002", 4.0));

        _output.WriteLine("\nОтчёт:");
        PrintReport();
        _output.WriteLine("\nДля новичков:");
        PrintItems(_fleet.GetBeginnerTransports());
        _output.WriteLine("\nВсе объекты:");
        PrintItems(_fleet.GetInventory());
        _output.WriteLine("\nПопытка повторить инвентарный номер:");
        try
        {
            _fleet.AddThing(new Helmet("Второй шлем", "T-001"));
        }
        catch (ArgumentException error)
        {
            _output.WriteLine(error.Message);
        }
    }

    private void AddTransport()
    {
        _output.WriteLine("1 - электросамокат, 2 - велосипед, 3 - электровелосипед");
        string kind = ReadText("Вид транспорта: ");
        if (kind != "1" && kind != "2" && kind != "3")
        {
            throw new ArgumentException("Неизвестный вид транспорта");
        }

        string name = ReadText("Название: ");
        string number = ReadText("Инвентарный номер: ");
        int rating = int.Parse(ReadText("Простота для новичка (1-10): "));
        bool isOperational = ReadText("Техника исправна? (1 - да, 0 - нет): ") switch
        {
            "1" => true,
            "0" => false,
            _ => throw new FormatException("Для исправности введите 1 или 0")
        };

        Transport transport;
        if (kind == "2")
        {
            transport = new Bicycle(name, number, rating, isOperational);
        }
        else
        {
            double energy = ReadEnergy();
            transport = kind == "1"
                ? new Scooter(name, number, rating, energy, isOperational)
                : new ElectricBicycle(name, number, rating, energy, isOperational);
        }

        PrintAdmission(transport);
    }

    private void AddThing()
    {
        _output.WriteLine("1 - шлем, 2 - зарядная станция");
        string kind = ReadText("Вид вещи: ");
        if (kind != "1" && kind != "2")
        {
            throw new ArgumentException("Неизвестный вид вещи");
        }

        string name = ReadText("Название: ");
        string number = ReadText("Инвентарный номер: ");
        Thing thing = kind == "1"
            ? new Helmet(name, number)
            : new ChargingStation(name, number, ReadEnergy());
        _fleet.AddThing(thing);
        _output.WriteLine("Вещь добавлена на баланс");
    }

    private string ReadText(string prompt)
    {
        _output.Write(prompt);
        return _input.ReadLine()?.Trim() ?? throw new EndOfStreamException();
    }

    private double ReadEnergy()
    {
        string value = ReadText("Суточная энергия (кВт·ч): ").Replace(',', '.');
        return double.Parse(value, CultureInfo.InvariantCulture);
    }

    private void PrintAdmission(Transport transport)
    {
        string result = _fleet.AddTransport(transport) ? "принят в парк" : "техосмотр отклонён";
        _output.WriteLine($"{transport.Name}: {result}");
    }

    private void PrintReport()
    {
        _output.WriteLine($"Транспорт: {_fleet.TransportCount}");
        _output.WriteLine($"Вещи: {_fleet.ThingCount}");
        _output.WriteLine($"Всего на балансе: {_fleet.GetInventory().Count}");
        string energy = _fleet.GetTotalDailyEnergy().ToString("G", CultureInfo.InvariantCulture);
        _output.WriteLine($"Суточная энергия: {energy} кВт·ч");
    }

    private void PrintItems(IEnumerable<IInventoryItem> items)
    {
        bool empty = true;
        foreach (IInventoryItem item in items)
        {
            empty = false;
            _output.WriteLine($"{item.Kind}: {item.Name} | инвентарный номер: {item.InventoryNumber}");
        }

        if (empty)
        {
            _output.WriteLine("Список пуст");
        }
    }
}
