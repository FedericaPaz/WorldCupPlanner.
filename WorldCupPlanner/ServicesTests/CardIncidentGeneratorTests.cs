using System;
using System.Linq;
using Domain.Entities;
using Domain.Enums;
using Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServicesTests
{
    [TestClass]
    public class CardIncidentGeneratorTests
    {
        [TestMethod]
        public void Generate_AddsOnlyCardIncidents_WithinExpectedRanges()
        {
            // Arrange
            var conf = new Confederation("UEFA");
            var home = new Team("Home", conf, 1000);
            var away = new Team("Away", conf, 1100);
            var stadium = new Stadium("S1", "City", "Desc", 20000);
            var clock = new Services.SystemDateTimeMatch();
            var match = new Match(1, DateTime.UtcNow.AddDays(1), stadium, home, away, 'A', clock);

            // Act
            CardIncidentGenerator.Generate(match, seed: 42);

            // Assert
            var incidents = match.Incidents;
            Assert.IsTrue(incidents.All(i => i.Type == IncidentType.YellowCard || i.Type == IncidentType.RedCard));
            Assert.IsTrue(incidents.Count <= 6); // max 4 yellow per team + 1 red per team
            foreach (var incident in incidents)
            {
                Assert.IsTrue(incident.Quantity >= 1);
                if (incident.Type == IncidentType.YellowCard)
                    Assert.IsTrue(incident.Quantity <= 4);
                if (incident.Type == IncidentType.RedCard)
                    Assert.IsTrue(incident.Quantity <= 1);
            }
        }
    }
}
