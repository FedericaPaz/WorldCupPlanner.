using System.Data;
using Domain;

namespace DTOs;

public class StadiumDTOs
{
    public class CreateStadiumDTO
    {
        public string stadiumName { get; set; }
        public string stadiumCity { get; set; }
        public string? stadiumDescription { get; set; }
        public int locativeCapacity { get; set; }
        
        public Stadium ToEntity()
        {
            return new Stadium(stadiumName, stadiumCity, stadiumDescription, locativeCapacity);
        }
    }

    public class StadiumResponseDTO
    {
        public string stadiumName { get; set; }
        public string stadiumCity { get; set; }
        public string? stadiumDescription { get; set; }
        public int locativeCapacity { get; set; }

        public static StadiumResponseDTO FromEntity(Stadium stadium)
        {
            return new StadiumResponseDTO
            {
                stadiumName = stadium.StadiumName,
                stadiumCity = stadium.CityName,
                stadiumDescription = stadium.StadiumDescription,
                locativeCapacity = stadium.LocativeCapacity
            };
        }
    }
}