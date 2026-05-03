using Domain;

namespace IRepository;

public interface IStadiumRepository
{
    IStadium GetByName(string name);
}