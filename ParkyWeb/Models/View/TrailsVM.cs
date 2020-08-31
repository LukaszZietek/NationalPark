using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ParkyWeb.Models.View
{
    public class TrailsVM
    {
        public IEnumerable<SelectListItem> NationalParkList { get; set; }
        public Trail Trail { get; set; }
    }
}
