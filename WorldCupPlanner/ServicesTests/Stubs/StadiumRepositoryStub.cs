using Domain;
using DomainTests.Stubs;
using IRepository;

namespace ServicesTests.Stubs;

public class StadiumRepositoryStub : IStadiumRepository
{
    public IStadium GetByName(string name)
    {
        return new StadiumStub(name);
    }
}