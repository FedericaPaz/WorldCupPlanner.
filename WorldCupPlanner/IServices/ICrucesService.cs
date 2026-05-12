using DTOs;

namespace IServices;

public interface ICruceService
{
    void GenerateRoundOf32();
    void GenerateNextRound();
    List<CruceResponse> GetAllCruces();
}