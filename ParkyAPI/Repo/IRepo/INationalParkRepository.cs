using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkyAPI.Models;

namespace ParkyAPI.Repo.IRepo
{
    public interface INationalParkRepository
    {
        ICollection<NationalPark> GetNationalParks();

        NationalPark GetNationalPark(int id);
        bool NationalParkExists(string name);
        bool NationalParkExists(int id);
        bool CreateNationalPark(NationalPark nationalPark);
        bool UpdateNationalPark(NationalPark nationalPark);
        bool DeleteNationalPark(NationalPark nationalPark);

        bool Save();

    }
}
