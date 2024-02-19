using System.ComponentModel.DataAnnotations.Schema;

namespace proiectasp.Models
{
    public class GroupPost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? PostId { get; set; }

        public int? GroupId { get; set; }

        public virtual Group? Group { get; set; }

        public virtual Post? Post { get; set; }

        public DateTime PostDate { get; set; }

    }
}
