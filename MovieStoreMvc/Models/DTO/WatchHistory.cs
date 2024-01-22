using System.ComponentModel.DataAnnotations;

namespace MovieStoreMvc.Models.DTO
{
    public class WatchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int MovieId { get; set; }
    }
}
