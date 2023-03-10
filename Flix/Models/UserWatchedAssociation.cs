using System.ComponentModel.DataAnnotations;

namespace Flix.Models
{
    public class UserWatchedAssociation
    {
        public int ID { get; set; }
        public string User { get; set; }
        public int VideoID { get; set; }
    }
}
