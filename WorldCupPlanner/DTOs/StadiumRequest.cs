using Domain;

namespace DTOs;

public class StadiumRequest
{
    public string StadiumName { get; set; }
    public string CityName { get; set; }
    public string StadiumDescription { get; set; }
    public int LocativeCapacity{get; set;}

    public static Stadium ToEntity(StadiumRequest dto)
    {
        return new Stadium(dto.StadiumName, dto.CityName, dto.StadiumDescription, dto.LocativeCapacity);
    }
}