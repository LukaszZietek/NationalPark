using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repo.IRepo;

namespace ParkyAPI.Repo
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public ICollection<Trail> GetTrails()
        {
            var value = _db.Trails.Include(x => x.NationalPark).OrderBy(x=> x.Name).ToList();
            return value;
        }

        public ICollection<Trail> GetTrailsInNationalPark(int nationalParkId)
        {
            var value = _db.Trails.Include(x => x.NationalParkId).Where(c => c.NationalParkId == nationalParkId)
                .ToList();
            return value;
        }

        public Trail GetTrail(int id)
        {
            var value = _db.Trails.Include(x => x.NationalPark).FirstOrDefault(x => x.Id == id);
            return value;
        }

        public bool TrailExists(string name)
        {
            var trail = _db.Trails.Any(x => x.Name.ToLower().Trim() == name.ToLower().Trim());
            return trail;
        }

        public bool TrailExists(int id)
        {
            var trail = _db.Trails.Any(x => x.Id == id);
            return trail;
        }

        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }
    }
}
