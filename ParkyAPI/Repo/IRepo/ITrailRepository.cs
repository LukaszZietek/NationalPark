using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkyAPI.Models;

namespace ParkyAPI.Repo.IRepo
{
    public interface ITrailRepository
    {

        ICollection<Trail> GetTrails();

        ICollection<Trail> GetTrailsInNationalPark(int nationalParkId);

        Trail GetTrail(int id);
        bool TrailExists(string name);
        bool TrailExists(int id);
        bool CreateTrail(Trail trail);
        bool UpdateTrail(Trail trail);
        bool DeleteTrail(Trail trail);

        bool Save();
    }
}
