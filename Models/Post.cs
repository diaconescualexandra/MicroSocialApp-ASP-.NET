using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace proiectasp.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Captionul postarii este obligatoriu")]
        [MaxLength(500)]
        public string PostCaption { get; set; }

        [Required(ErrorMessage = "Continutul postarii este obligatoriu")]
        public string PostContent { get; set; }

        public string? UserId { get; set; }

        public int? GroupId { get; set; }

        //[Required]
        public DateTime PostCreationTime { get; set; }

        public string? Image{ get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Group? Group { get; set; }
        public virtual ICollection<GroupPost>? GroupPosts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Grp { get; set; }


    }
}
