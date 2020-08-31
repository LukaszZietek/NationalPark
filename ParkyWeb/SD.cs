using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb
{
    public static class SD
    {
        public static string ApiBaseUrl = "https://localhost:44352/";
        public static string NationalParkApiPath = ApiBaseUrl + "api/v1/nationalparks/";
        public static string TrailApiPath = ApiBaseUrl + "api/v1/trail/";
        public static string AccountApiPath = ApiBaseUrl + "api/v1/Users/";
    }
}
