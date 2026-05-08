using Domain;
using DTOs;

namespace Services.Mappers;

public static class FixtureMapper
{
    public static FixtureResponse ToEntity(this Fixture fixture)
    {
        if (fixture == null)
            return new FixtureResponse();
        
        return new FixtureResponse
        {
            Groups = fixture.GetGroups.Select(g => g.ToEntity()).ToList(),
            CurrentPhase = fixture.CurrentPhase.ToString()
        };
    }
    
    public static GroupResponse ToEntity(this Group group)
    {
        return new GroupResponse
        {
            GroupLetter = group.GroupLetter,
            Teams = group.TeamsInGroup.Select(t => t.FromEntity()).ToList(),
            Matches = group.MatchesInGroup.Select(m => m.ToResponse()).ToList()
        };
    }
}