using Domain;
using DTOs;
namespace Services.Mappers;

public static class EntityStadiumMapper
{
    public static Stadium ToEntity(this StadiumRequest request) => new Stadium(request.stadiumName, request.stadiumCity, request.stadiumDescription ?? string.Empty, request.locativeCapacity);
}