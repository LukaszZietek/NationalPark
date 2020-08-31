using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Models.Dtos
{
    public class TrailCreateDto
    {
        

        [Required]
        public string Name { get; set; }
        [Required]
        public double Distance { get; set; }

        [Range(0,3)]
        public Trail.DifficultyType Difficulty { get; set; }

        [Required]
        public int NationalParkId { get; set; }

        [Required]
        public double Elevation { get; set; }
    }
}
