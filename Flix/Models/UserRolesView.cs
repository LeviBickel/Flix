using System.ComponentModel.DataAnnotations;

namespace Flix.Models
{
    public class UserRolesView
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }

    }
}
