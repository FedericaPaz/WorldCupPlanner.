using Domain;
using DTOs;

namespace Services.Mappers;

 public static class TeamXMappers
 {
     public static Team ToEntity(this TeamDTO request)
     {
         var confederation = new Confederation(request.ConfederationName);
         return new Team(request.Name, confederation, request.FifaRanking);
     }
 
     public static TeamDTO FromEntity(this Team team)
     {
         return new TeamDTO
         {
             Name = team.Name,
             ConfederationName = team.Confederation.Name,
             FifaRanking = team.FifaRanking
         };
     }
 }