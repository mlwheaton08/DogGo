using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IWalksRepository
    {
        List<Walks> GetWalksByWalkerId(int walkerId);
    }
}