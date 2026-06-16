using DTOs.Fixture;

namespace IServices.PageFacades;

public interface IFixturePageFacade
{
    bool HasFixture();
    FixtureResponse GetFixture();
    int GetTeamCount();
    int GetStadiumCount();
    bool MatchesHaveBeenPlayed();
    void GenerateFixture(string engineName, string userEmail);
    void RegenerateFixture(string engineName, string userEmail);
    void DeleteFixture(string userEmail);
}
