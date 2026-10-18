using HSEkatkat.DI;
using HSEkatkat.Models;
using HSEkatkat.Services;
using Moq;

namespace HSEkatkat.Tests;

// Подмена техосмотра через DI-контейнер
public class DiSubstitutionTests
{
    public class AcceptAllServiceCenter : IServiceCenter
    {
        public bool CanAccept(IInventoryItem? item) => true;
    }

    public class RejectAllServiceCenter : IServiceCenter
    {
        public bool CanAccept(IInventoryItem? item) => false;
    }

    [Fact]
    public void Resolve_Manager_WithRejectingStub_RejectsEverything()
    {
        // Arrange
        var container = new DiContainer();
        container.Register<IServiceCenter, RejectAllServiceCenter>();
        container.Register<Manager, Manager>();
        var manager = container.Resolve<Manager>();
        var scooter = new Scooter("1", "Самокат", 1.5, 8);

        // Act
        var result = manager.AddItem(scooter);

        // Assert
        Assert.False(result);
        Assert.Empty(manager.GetAllItems());
    }

    [Fact]
    public void Resolve_Manager_WithAcceptingStub_AcceptsEvenEmptyNumber()
    {
        // Arrange
        var container = new DiContainer();
        container.Register<IServiceCenter, AcceptAllServiceCenter>();
        container.Register<Manager, Manager>();
        var manager = container.Resolve<Manager>();
        var helmetWithoutNumber = new Helmet("", "Шлем");

        // Act
        var result = manager.AddItem(helmetWithoutNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Resolve_Manager_WithRealServiceCenter_RejectsEmptyNumber()
    {
        // Arrange
        var container = new DiContainer();
        container.Register<IServiceCenter, ServiceCenter>();
        container.Register<Manager, Manager>();
        var manager = container.Resolve<Manager>();
        var helmetWithoutNumber = new Helmet("", "Шлем");

        // Act
        var result = manager.AddItem(helmetWithoutNumber);

        // Assert
        Assert.False(result);
    }
}

// DiContainer

public class DiContainerTests
{
    [Fact]
    public void Resolve_NotRegisteredType_Throws()
    {
        // Arrange
        var container = new DiContainer();

        // Act
        Action act = () => container.Resolve<IServiceCenter>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void Resolve_TwiceSameType_ReturnsSameInstance()
    {
        // Arrange
        var container = new DiContainer();
        container.Register<IServiceCenter, ServiceCenter>();

        // Act
        var first = container.Resolve<IServiceCenter>();
        var second = container.Resolve<IServiceCenter>();

        // Assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Resolve_MissingDependency_Throws()
    {
        // Arrange
        var container = new DiContainer();
        container.Register<Manager, Manager>();

        // Act
        Action act = () => container.Resolve<Manager>();

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}

// Manager: stub

public class ManagerTests_WithStub
{
    private class StubServiceCenter : IServiceCenter
    {
        public bool CanAccept(IInventoryItem? item) => true;
    }

    private class StubRejectServiceCenter : IServiceCenter
    {
        public bool CanAccept(IInventoryItem? item) => false;
    }

    [Fact]
    public void Constructor_NullServiceCenter_Throws()
    {
        // Arrange
        IServiceCenter? center = null;

        // Act
        Action act = () => new Manager(center!);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void AddItem_WhenServiceCenterAccepts_ItemIsStored()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        var scooter = new Scooter("1", "Sport A", 1.5, 8);

        // Act
        var result = manager.AddItem(scooter);

        // Assert
        Assert.True(result);
        Assert.Single(manager.GetAllItems());
    }

    [Fact]
    public void AddItem_WhenServiceCenterRejects_ItemIsNotStored()
    {
        // Arrange
        var manager = new Manager(new StubRejectServiceCenter());
        var scooter = new Scooter("1", "Sport A", 1.5, 8);

        // Act
        var result = manager.AddItem(scooter);

        // Assert
        Assert.False(result);
        Assert.Empty(manager.GetAllItems());
    }

    [Fact]
    public void GetTotalDailyEnergy_SumsOnlyEnergyConsumers()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        manager.AddItem(new Scooter("1", "Самокат", 1.5, 8));
        manager.AddItem(new Helmet("2", "Шлем"));
        manager.AddItem(new EBike("3", "Электровелосипед", 2.0, 9));
        manager.AddItem(new Flashlight("4", "Фонарик", 0.5));

        // Act
        var energy = manager.GetTotalDailyEnergy();

        // Assert
        Assert.Equal(4.0, energy, precision: 5);
    }

    [Fact]
    public void GetTotalDailyEnergy_EmptyFleet_ReturnsZero()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());

        // Act
        var energy = manager.GetTotalDailyEnergy();

        // Assert
        Assert.Equal(0, energy);
    }

    [Fact]
    public void GetBeginnerFriendlyItems_ReturnsOnlyItemsAboveThreshold()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        manager.AddItem(new Scooter("1", "Хороший", 1.5, 8));
        manager.AddItem(new Scooter("2", "Плохой", 1.5, 3));

        // Act
        var items = manager.GetBeginnerFriendlyItems();

        // Assert
        Assert.Single(items);
        Assert.Equal("Хороший", items[0].Name);
    }

    [Theory]
    [InlineData(Manager.BeginnerScoreThreshold - 1, false)]
    [InlineData(Manager.BeginnerScoreThreshold, true)]
    [InlineData(Manager.BeginnerScoreThreshold + 1, true)]
    public void GetBeginnerFriendlyItems_BoundaryAroundThreshold(int score, bool expected)
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        manager.AddItem(new Bike("1", "Велосипед", score));

        // Act
        var items = manager.GetBeginnerFriendlyItems();

        // Assert
        Assert.Equal(expected, items.Count == 1);
    }

    [Fact]
    public void GetBeginnerFriendlyItems_EquipmentIsNeverIncluded()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        manager.AddItem(new Helmet("1", "Шлем"));
        manager.AddItem(new Flashlight("2", "Фонарик", 0.1));

        // Act
        var items = manager.GetBeginnerFriendlyItems();

        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public void GetAllItems_ContainsBothVehiclesAndEquipment()
    {
        // Arrange
        var manager = new Manager(new StubServiceCenter());
        manager.AddItem(new Bike("1", "Велосипед", 7));
        manager.AddItem(new Helmet("2", "Шлем"));

        // Act
        var items = manager.GetAllItems();

        // Assert
        Assert.Equal(2, items.Count);
    }
}

// Manager: fake

public class ManagerTests_WithFake
{
    private class FakeServiceCenter : IServiceCenter
    {
        public int CallCount { get; private set; }
        public IInventoryItem? LastCheckedItem { get; private set; }

        public bool CanAccept(IInventoryItem? item)
        {
            CallCount++;
            LastCheckedItem = item;
            return item != null && !string.IsNullOrWhiteSpace(item.InventoryNumber);
        }
    }

    [Fact]
    public void AddItem_AsksServiceCenterOnce_AboutTheSameItem()
    {
        // Arrange
        var center = new FakeServiceCenter();
        var manager = new Manager(center);
        var scooter = new Scooter("1", "Самокат1", 1.5, 8);

        // Act
        var result = manager.AddItem(scooter);

        // Assert
        Assert.True(result);
        Assert.Equal(1, center.CallCount);
        Assert.Same(scooter, center.LastCheckedItem);
    }

    [Fact]
    public void AddItem_EmptyNumber_RejectedByFakeLogic()
    {
        // Arrange
        var center = new FakeServiceCenter();
        var manager = new Manager(center);
        var scooterWithoutNumber = new Scooter("", "Без номера", 1.5, 8);

        // Act
        var result = manager.AddItem(scooterWithoutNumber);

        // Assert
        Assert.False(result);
        Assert.Equal(1, center.CallCount);
        Assert.Empty(manager.GetAllItems());
    }
}

// Manager: mock

public class ManagerTests_WithMock
{
    [Fact]
    public void AddItem_VerifyServiceCenterCalledOnce()
    {
        // Arrange
        var mockCenter = new Mock<IServiceCenter>();
        mockCenter.Setup(x => x.CanAccept(It.IsAny<IInventoryItem?>())).Returns(true);
        var manager = new Manager(mockCenter.Object);
        var scooter = new Scooter("1", "Sport A", 1.5, 8);

        // Act
        manager.AddItem(scooter);

        // Assert
        mockCenter.Verify(x => x.CanAccept(scooter), Times.Once());
    }

    [Fact]
    public void AddItem_Duplicate_ServiceCenterIsAskedForEachAttempt()
    {
        // Arrange
        var mockCenter = new Mock<IServiceCenter>();
        mockCenter.Setup(x => x.CanAccept(It.IsAny<IInventoryItem?>())).Returns(true);
        var manager = new Manager(mockCenter.Object);
        manager.AddItem(new Bike("1", "A", 7));
        var duplicate = new Bike("1", "B", 7);

        // Act
        var result = manager.AddItem(duplicate);

        // Assert
        Assert.False(result);
        mockCenter.Verify(x => x.CanAccept(It.IsAny<IInventoryItem?>()), Times.Exactly(2));
    }
}

// ServiceCenter

public class ServiceCenterTests
{
    private readonly ServiceCenter _center = new();

    [Fact]
    public void CanAccept_ValidItem_ReturnsTrue()
    {
        // Arrange
        var item = new Scooter("1", "Test", 1.0, 5);

        // Act
        var result = _center.CanAccept(item);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CanAccept_EmptyOrWhitespaceNumber_ReturnsFalse(string number)
    {
        // Arrange
        var item = new Scooter(number, "Test", 1.0, 5);

        // Act
        var result = _center.CanAccept(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CanAccept_NullItem_ReturnsFalse()
    {
        // Arrange
        IInventoryItem? item = null;

        // Act
        var result = _center.CanAccept(item);

        // Assert
        Assert.False(result);
    }
}

// ItemFactory

public class ItemFactoryContractTests
{
    private static readonly ItemFactory Factory = new();
    
    public static IEnumerable<object[]> AllTypeChoices()
    {
        for (int i = 1; i <= Factory.GetAvailableTypes().Length; i++)
        {
            yield return new object[] { i };
        }
    }

    [Fact]
    public void GetAvailableTypes_ReturnsNonEmptyUniqueNames()
    {
        // Arrange
        // (фабрика создана в поле класса)

        // Act
        var types = Factory.GetAvailableTypes();

        // Assert
        Assert.NotEmpty(types);
        Assert.All(types, name => Assert.False(string.IsNullOrWhiteSpace(name)));
        Assert.Equal(types.Length, types.Distinct().Count());
    }

    [Theory]
    [MemberData(nameof(AllTypeChoices))]
    public void CreateItem_ForEveryAvailableType_KeepsNumberAndName(int typeChoice)
    {
        // Arrange
        const string number = "INV-1";
        const string name = "Тестовое имя";

        // Act
        var item = Factory.CreateItem(typeChoice, number, name, 2.5, 7);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(number, item.InventoryNumber);
        Assert.Equal(name, item.Name);
    }

    [Theory]
    [MemberData(nameof(AllTypeChoices))]
    public void HasEnergy_AgreesWithCreatedItem(int typeChoice)
    {
        // Arrange
        const double energy = 2.5;

        // Act
        var hasEnergy = Factory.HasEnergy(typeChoice);
        var item = Factory.CreateItem(typeChoice, "1", "X", energy, 7);

        // Assert
        Assert.Equal(hasEnergy, item is IEnergyConsumer);
    }

    [Theory]
    [MemberData(nameof(AllTypeChoices))]
    public void HasBeginnerScore_AgreesWithCreatedItem(int typeChoice)
    {
        // Arrange
        const int score = 7;

        // Act
        var hasScore = Factory.HasBeginnerScore(typeChoice);
        var item = Factory.CreateItem(typeChoice, "1", "X", 2.5, score);

        // Assert
        Assert.Equal(hasScore, item is IBeginnerFriendly);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(99)]
    public void InvalidTypeChoice_CreateItemThrows_AndCapabilitiesAreFalse(int typeChoice)
    {
        // Arrange
        Action create = () => Factory.CreateItem(typeChoice, "1", "X");

        // Act
        var hasEnergy = Factory.HasEnergy(typeChoice);
        var hasScore = Factory.HasBeginnerScore(typeChoice);

        // Assert
        Assert.Throws<ArgumentException>(create);
        Assert.False(hasEnergy);
        Assert.False(hasScore);
    }

}

// Модели

public class ModelTests
{
    [Fact]
    public void Scooter_StoresAllValues()
    {
        // Arrange
        const string number = "S-1";
        const string name = "Самокат";

        // Act
        var scooter = new Scooter(number, name, 1.5, 8);

        // Assert
        Assert.Equal(number, scooter.InventoryNumber);
        Assert.Equal(name, scooter.Name);
        Assert.Equal(1.5, scooter.DailyEnergyConsumption);
        Assert.Equal(8, scooter.BeginnerScore);
    }

    [Fact]
    public void Bike_StoresValues_AndHasNoEnergy()
    {
        // Arrange
        const string number = "B-1";
        const string name = "Велосипед";

        // Act
        var bike = new Bike(number, name, 9);

        // Assert
        Assert.Equal(number, bike.InventoryNumber);
        Assert.Equal(name, bike.Name);
        Assert.Equal(9, bike.BeginnerScore);
        Assert.False(bike is IEnergyConsumer);
    }

    [Fact]
    public void Helmet_StoresValues_AndHasNoEnergyNorScore()
    {
        // Arrange
        const string number = "H-1";
        const string name = "Шлем";

        // Act
        var helmet = new Helmet(number, name);

        // Assert
        Assert.Equal(number, helmet.InventoryNumber);
        Assert.Equal(name, helmet.Name);
        Assert.False(helmet is IEnergyConsumer);
        Assert.False(helmet is IBeginnerFriendly);
    }

    [Fact]
    public void Flashlight_StoresEnergy_AndHasNoScore()
    {
        // Arrange
        const string number = "F-1";
        const string name = "Фонарик";

        // Act
        var flashlight = new Flashlight(number, name, 0.1);

        // Assert
        Assert.Equal(number, flashlight.InventoryNumber);
        Assert.Equal(name, flashlight.Name);
        Assert.Equal(0.1, flashlight.DailyEnergyConsumption);
        Assert.False(flashlight is IBeginnerFriendly);
    }
}


// Консоль
[Collection("Console")]
public class ProgramTests
{
    // Запускает Main с заготовленным вводом и возвращает всё, что программа напечатала
    private static string RunProgram(string input)
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        var output = new StringWriter();

        try
        {
            Console.SetIn(new StringReader(input));
            Console.SetOut(output);

            Program.Main(Array.Empty<string>());
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        return output.ToString();
    }

    [Fact]
    public void Main_ExitImmediately_SaysGoodbye()
    {
        // Arrange: сразу выбираем "0. Выйти"
        var input = "0\n";

        // Act
        var output = RunProgram(input);

        // Assert
        Assert.Contains("До свидания", output);
    }

    [Fact]
    public void Main_AddScooterThenOpenAllReports_ShowsExpectedData()
    {
        // Arrange: добавить самокат (тип 1, номер S-1, энергия 2, оценка 8),
        // затем отчёты 2, 3, 4, 5 и выход. Пустые строки это нажатия Enter после каждого действия.
        var input = string.Join("\n",
            "1", "1", "S-1", "Самокат", "2", "8", "",
            "2", "",
            "3", "",
            "4", "",
            "5", "",
            "0") + "\n";

        // Act
        var output = RunProgram(input);

        // Assert
        Assert.Contains("успешно добавлен", output);
        Assert.Contains("Всего единиц техники в парке: 1", output);
        Assert.Contains("id: S-1 - Самокат", output);
        Assert.Contains("Суммарное суточное потребление: 2 кВт·ч", output);
        Assert.Contains("Техника для новичков", output);
    }

    [Fact]
    public void Main_ReportsOnEmptyFleet_ShowEmptyMessages()
    {
        // Arrange: сразу отчёты по инвентарю и новичкам на пустом парке
        var input = string.Join("\n",
            "3", "",
            "5", "",
            "0") + "\n";

        // Act
        var output = RunProgram(input);

        // Assert
        Assert.Contains("Парк пуст.", output);
        Assert.Contains("Нет подходящей техники.", output);
    }
}