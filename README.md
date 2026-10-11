using Microsoft.Extensions.DependencyInjection;
using VyshKat.Models;
using VyshKat.Services;
using Xunit;

namespace VyshKat.Tests;

public class FleetTests
{
    [Fact]
    public void AcceptsTransportWhenFakeApproves()
    {
        var inspection = new FakeInspectionService(true);
        using var provider = DependencyConfig.Build(inspection);
        var fleet = provider.GetRequiredService<Fleet>();
        var scooter = new Scooter("Самокат", "T-1", 8, 2.0, false);

        Assert.True(fleet.AddTransport(scooter));
        Assert.Equal(1, fleet.TransportCount);
        Assert.Same(scooter, Assert.Single(fleet.GetInventory()));
        Assert.Same(scooter, Assert.Single(inspection.Checked));
    }

    [Fact]
    public void RejectsTransportWhenFakeRefuses()
    {
        var inspection = new FakeInspectionService(false);
        using var provider = DependencyConfig.Build(inspection);
        var fleet = provider.GetRequiredService<Fleet>();
        var scooter = new Scooter("Самокат", "T-1", 8, 2.0);

        Assert.False(fleet.AddTransport(scooter));
        Assert.Equal(0, fleet.TransportCount);
        Assert.Empty(fleet.GetInventory());
        Assert.Empty(fleet.GetBeginnerTransports());
        Assert.Equal(0.0, fleet.GetTotalDailyEnergy());
        Assert.Same(scooter, Assert.Single(inspection.Checked));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ServiceCenterChecksCondition(bool isOperational)
    {
        var transport = new Bicycle("Велосипед", "T-1", 7, isOperational);
        Assert.Equal(isOperational, new ServiceCenter().Inspect(transport));
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(10, true)]
    public void BeginnerRatingBoundary(int rating, bool suitable)
    {
        using var provider = DependencyConfig.Build();
        var fleet = provider.GetRequiredService<Fleet>();
        var bicycle = new Bicycle("Велосипед", "T-1", rating);
        fleet.AddTransport(bicycle);

        Assert.Equal(suitable, fleet.GetBeginnerTransports().Contains(bicycle));
    }

    [Fact]
    public void EnergyIncludesTransportAndChargingStation()
    {
        using var provider = DependencyConfig.Build();
        var fleet = provider.GetRequiredService<Fleet>();
        fleet.AddTransport(new Scooter("Самокат", "T-1", 8, 2.5));
        fleet.AddTransport(new ElectricBicycle("Электровелосипед", "T-2", 6, 1.5));
        fleet.AddTransport(new Bicycle("Велосипед", "T-3", 7));
        fleet.AddThing(new ChargingStation("Станция", "V-1", 4.0));
        fleet.AddThing(new Helmet("Шлем", "V-2"));

        Assert.Equal(8.0, fleet.GetTotalDailyEnergy(), 6);
        Assert.Equal(3, fleet.TransportCount);
        Assert.Equal(2, fleet.ThingCount);
    }

    [Fact]
    public void InventoryContainsNamesAndNumbers()
    {
        var inspection = new FakeInspectionService(true);
        var fleet = new Fleet(inspection);
        fleet.AddTransport(new Bicycle("Велосипед", "T-1", 7));
        fleet.AddThing(new Helmet("Шлем", "V-1"));

        var inventory = fleet.GetInventory();
        Assert.Equal(new[] { "Велосипед", "Шлем" }, inventory.Select(item => item.Name));
        Assert.Equal(new[] { "T-1", "V-1" }, inventory.Select(item => item.InventoryNumber));
        Assert.Single(inspection.Checked);
    }

    [Fact]
    public void DuplicateTransportDoesNotChangeFleet()
    {
        var inspection = new FakeInspectionService(true);
        var fleet = new Fleet(inspection);
        var bicycle = new Bicycle("Первый", "T-1", 7);
        fleet.AddTransport(bicycle);

        Assert.Throws<ArgumentException>(() => fleet.AddTransport(new Scooter("Второй", "T-1", 8, 2.0)));
        Assert.Same(bicycle, Assert.Single(fleet.GetInventory()));
        Assert.Single(inspection.Checked);
    }

    [Fact]
    public void InventoryNumbersAreUniqueAcrossAllObjects()
    {
        var fleet = new Fleet(new ServiceCenter());
        fleet.AddTransport(new Bicycle("Велосипед", "N-1", 7));
        Assert.Throws<ArgumentException>(() => fleet.AddThing(new Helmet("Шлем", "N-1")));

        fleet.AddThing(new Helmet("Шлем", "N-2"));
        Assert.Throws<ArgumentException>(() => fleet.AddTransport(new Bicycle("Второй", "N-2", 6)));
        Assert.Throws<ArgumentException>(() => fleet.AddThing(new Helmet("Другой шлем", "N-2")));
        Assert.Equal(2, fleet.GetInventory().Count);
    }

    [Fact]
    public void RejectedNumberCanBeUsedAgain()
    {
        var fleet = new Fleet(new ServiceCenter());
        Assert.False(fleet.AddTransport(new Bicycle("Сломанный", "T-1", 7, false)));
        Assert.True(fleet.AddTransport(new Bicycle("Исправный", "T-1", 7)));
    }

    [Fact]
    public void EmptyFleetHasEmptyReports()
    {
        var fleet = new Fleet(new ServiceCenter());
        Assert.Equal(0, fleet.TransportCount);
        Assert.Equal(0, fleet.ThingCount);
        Assert.Equal(0.0, fleet.GetTotalDailyEnergy());
        Assert.Empty(fleet.GetInventory());
        Assert.Empty(fleet.GetBeginnerTransports());
    }

    [Fact]
    public void ContainerReusesFleetWithinProvider()
    {
        using var provider = DependencyConfig.Build();
        using var otherProvider = DependencyConfig.Build();
        Assert.Same(provider.GetRequiredService<Fleet>(), provider.GetRequiredService<Fleet>());
        Assert.NotSame(provider.GetRequiredService<Fleet>(), otherProvider.GetRequiredService<Fleet>());
    }

    [Fact]
    public void NewTransportNeedsNoFleetChanges()
    {
        var fleet = new Fleet(new ServiceCenter());
        var cargo = new CargoBicycle("Грузовой", "T-1", 6);
        Assert.True(fleet.AddTransport(cargo));
        Assert.Same(cargo, Assert.Single(fleet.GetBeginnerTransports()));
        Assert.Equal(0.0, fleet.GetTotalDailyEnergy());
    }

    private class CargoBicycle : Bicycle
    {
        public CargoBicycle(string name, string number, int rating) : base(name, number, rating)
        {
        }
    }
}
