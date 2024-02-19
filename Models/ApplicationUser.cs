using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using proiectasp.Models;

namespace proiectasp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            LockStatus = false;
        }

        public virtual ICollection<FollowList>? FollowLists { get; set; }

        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }

        public string? Signature { get; set; }

        public bool LockStatus { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? LockOpt { get; set; }
    }
}
