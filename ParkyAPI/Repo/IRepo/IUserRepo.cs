using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ParkyAPI.Models;

namespace ParkyAPI.Repo.IRepo
{
    public interface IUserRepo
    {
        bool IsUniqueUser(string username);

        User Authenticate(string username, string password);

        User Register(string username, string password);

    }
}
