using Domain.Entities;
using DTOs.Fixture;

namespace Services.Mappers;

public static class FixtureMapper
{
    public static FixtureResponse ToEntity(this Fixture fixture)
    {
        if (fixture == null)
            return new FixtureResponse();
        
        return new FixtureResponse
        {
            Groups = fixture.Groups.Select(g => g.ToEntity()).ToList(),
            CurrentPhase = fixture.CurrentPhase.ToString()
        };
    }
    
    public static GroupResponse ToEntity(this Group group)
    {
        return new GroupResponse
        {
            GroupLetter = group.GroupLetter,
            Teams = group.GetTeamsInGroup().Select(t => t.FromEntity()).ToList(),
            Matches = group.MatchesInGroup.Select(m => m.ToResponse()).ToList()
        };
    }
}