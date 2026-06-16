using Domain.Interfaces;
using Services;
using ServicesTests.Stubs;

namespace ServicesTests;

[TestClass]
public class SimulationEngineFactoryTests
{
    [TestMethod]
    public void Constructor_WithNullEngines_ShouldThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new SimulationEngineFactory(null!));
    }

    [TestMethod]
    public void Create_WithRegisteredName_ShouldReturnCorrectEngine()
    {
        var engine = new TestSimulationEngine("Motor1");
        var factory = new SimulationEngineFactory(new[] { engine });

        var result = factory.Create("Motor1");

        Assert.AreSame(engine, result);
    }

    [TestMethod]
    public void Create_WithCaseInsensitiveName_ShouldReturnCorrectEngine()
    {
        var engine = new TestSimulationEngine("Motor1");
        var factory = new SimulationEngineFactory(new[] { engine });

        var result = factory.Create("motor1");

        Assert.AreSame(engine, result);
    }

    [TestMethod]
    public void Create_WithUnregisteredName_ShouldThrowInvalidOperationException()
    {
        var factory = new SimulationEngineFactory(new[] { new TestSimulationEngine("Motor1") });

        Assert.ThrowsException<InvalidOperationException>(() => factory.Create("MotorInexistente"));
    }

    [TestMethod]
    public void ListEngines_ShouldReturnAllRegisteredNames()
    {
        var engines = new ISimulationEngine[]
        {
            new TestSimulationEngine("Motor1"),
            new TestSimulationEngine("Motor2")
        };
        var factory = new SimulationEngineFactory(engines);

        var names = factory.ListEngines().ToList();

        Assert.AreEqual(2, names.Count);
        CollectionAssert.Contains(names, "Motor1");
        CollectionAssert.Contains(names, "Motor2");
    }

    [TestMethod]
    public void Constructor_WithDuplicateEngineNames_ShouldThrowException()
    {
        var engines = new ISimulationEngine[]
        {
            new TestSimulationEngine("Motor1"),
            new TestSimulationEngine("Motor1")
        };

        Assert.ThrowsException<ArgumentException>(() => new SimulationEngineFactory(engines));
    }

    [TestMethod]
    public void Create_WithEmptyEngineList_ShouldThrowForAnyName()
    {
        var factory = new SimulationEngineFactory(Array.Empty<ISimulationEngine>());

        Assert.ThrowsException<InvalidOperationException>(() => factory.Create("Motor1"));
    }

    [TestMethod]
    public void ListEngines_WithEmptyList_ShouldReturnEmpty()
    {
        var factory = new SimulationEngineFactory(Array.Empty<ISimulationEngine>());

        Assert.AreEqual(0, factory.ListEngines().Count());
    }
}
