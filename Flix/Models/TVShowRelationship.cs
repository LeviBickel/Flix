using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Flix.Models
{
    public class TVShowRelationship
    {
        [Key]
        public int ID { get; set; }
        public int VideoID { get; set; }
        public int ShowID { get; set; }

    }
}
