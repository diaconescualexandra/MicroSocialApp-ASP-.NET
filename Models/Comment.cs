using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace proiectasp.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string CommentContent { get; set; }
        public DateTime CommentCreationTime { get; set; }

        public int? PostId { get; set; }
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }
        public virtual Post? Post { get; set; }
    }
}
