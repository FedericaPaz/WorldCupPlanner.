using Domain.Entities;
using DTOs.Team;

namespace Services.Mappers;

 public static class TeamXMappers
 {
     public static Team ToEntity(this TeamDTO request)
     {
         var confederation = new Confederation(request.ConfederationName);
         return new Team(
             request.Name,
             confederation,
             request.CurrentRanking,
             request.FlagBase64 ?? ""
         );
     }
 
     public static TeamDTO FromEntity(this Team team)
     {
         return new TeamDTO
         {
             Name = team.Name,
             ConfederationName = team.Confederation.Name,
             FifaRanking = team.FifaRanking,
             FlagBase64 = team.FlagBase64,
             CurrentRanking = team.CurrentRanking
         };
     }
 }