using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flix.Models
{
    public class Movies
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string VideoPath { get; set; }
        [Required]
        public string CoverPath { get; set; }
        public string UniqueName { get; set; }
        public string Description { get; set; }
        [DisplayName("Category")]
        public string videoCategory { get; set; }
        public bool watched { get; set; }
    }
}
