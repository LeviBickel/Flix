using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Flix.Models
{
    public class MoviesView
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public IFormFile Video { get; set; }
        public IFormFile VideoCover { get; set; }
        public string Description { get; set; }
        [DisplayName("Category")]
        public string videoCategory { get; set; }
    }
}
