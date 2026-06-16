using DTOs.Cruces;

namespace IServices.PageFacades;

public interface ICrucesPageFacade
{
    bool HasFixture();
    List<CruceResponse> GetAllCruces();
    int GetPendingMatchCount();
    bool AllGroupMatchesPlayed();
    void GenerateRoundOf32(string userEmail);
    void GenerateNextRound(string userEmail);
}
