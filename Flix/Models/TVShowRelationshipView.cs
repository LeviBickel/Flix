using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flix.Models
{
    public class TVShowRelationshipView
    {
        public IEnumerable<Movies> Episodes { get; set; }
        public IEnumerable<TVShowRelationship> shows { get; set; }
        public Movies Movies { get; set; }
    }
}
