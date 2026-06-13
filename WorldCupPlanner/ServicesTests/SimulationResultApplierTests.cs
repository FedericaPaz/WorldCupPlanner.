using System;
using Domain.Entities;
using IRepository;
using IServices;
using Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServicesTests
{
    [TestClass]
    public class SimulationResultApplierTests
    {
        private class RankingRecorderStub : IRankingService
        {
            public bool Updated { get; private set; }
            public void UpdateRankingsAfterMatch(Match match)
            {
                Updated = true;
            }
        }

        [TestMethod]
        public void ApplyAndPersistResult_SetsMatchResultAndPersistsAndUpdatesRanking()
        {
            // Arrange
            var matchRepo = new ServicesTests.Stubs.MatchRepositoryStub();
            var rankingStub = new RankingRecorderStub();

            var conf = new Domain.Entities.Confederation("UEFA");
            var home = new Team("Home", conf, 1000);
            var away = new Team("Away", conf, 1100);
            var stadium = new Stadium("S1", "City", "Desc", 20000);
            var clock = new Services.SystemDateTimeMatch();
            var match = new Match(10, DateTime.UtcNow.AddDays(1), stadium, home, away, 'A', clock);

            matchRepo.AddMatch(match);

            var applier = new SimulationResultApplier(matchRepo, rankingStub);

            // Act
            applier.ApplyAndPersistResult(match, homeGoals: 2, awayGoals: 1);

            // Assert
            var persisted = matchRepo.GetMatchById(10);
            Assert.AreEqual(2, persisted.HomeGoals);
            Assert.AreEqual(1, persisted.AwayGoals);
            Assert.IsTrue(rankingStub.Updated);
        }
    }
}
