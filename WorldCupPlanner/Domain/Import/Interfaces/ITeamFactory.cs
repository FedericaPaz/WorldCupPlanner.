using Domain.Entities;

namespace Domain.Import.Interfaces;

public interface ITeamFactory
{
    public Team CreateTeam(string name, string confederationName, string rankingValue, string flagBase64 = "");
}