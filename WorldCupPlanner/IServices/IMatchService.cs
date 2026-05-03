using DTOs;

namespace IServices;

public interface IMatchService
{
    MatchResponse Create(MatchRequest request);
}