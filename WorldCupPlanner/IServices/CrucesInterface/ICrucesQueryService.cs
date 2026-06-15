using DTOs.Cruces;

namespace IServices.CrucesInterface;

public interface ICruceQueryService
{
    List<CruceResponse> GetAllCruces();
}