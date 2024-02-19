using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace proiectasp.Models
{
    public class Group
    {
        public Group() { GroupJoinMethod = false; }
        [Key]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Grupul trebuie sa aiba un nume")]
        public string GroupName { get; set; }

        public string? UserId { get; set; }

        public string? GroupDescription { get; set; }
        public bool? GroupJoinMethod { get; set; }

        public virtual ICollection<ApplicationUser>? User { get; set; }

        public virtual ICollection<GroupPost>? GroupPosts { get; set; }


        [NotMapped]
        public IEnumerable<SelectListItem>? JoinMet { get; set; }
    }
}
