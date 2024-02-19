using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace proiectasp.Models
{
    public class FollowList
    {
        [Key]
        public string UserX { get; set; }
        public string FollowsUserY { get; set; }

        public virtual ApplicationUser User { get; set; }
        public ICollection<FollowList> FollowLists { get; set; }
    }
}
