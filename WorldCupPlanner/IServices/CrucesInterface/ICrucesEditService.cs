using Domain.Interfaces;

namespace IServices.CrucesInterface;

public interface ICruceEditService
{
    void GenerateRoundOf32();
    void GenerateNextRound();
    void SimulateCruces();
}