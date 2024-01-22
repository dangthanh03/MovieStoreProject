using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieStoreMvc.Models.Domain
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? ReleaseYear { get; set; }

        public string? MovieImage { get; set; }

        [Required]
        public string? Cast { get; set; }

        [Required]
        public string? Director { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        [Required]
        public List<int>? Genres { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? GenreList { get; set; }

        [NotMapped]
        public string? GenreNames { get; set; }

        [NotMapped]
        public MultiSelectList? MultiGenreList { get; set; }

        // Thêm thuộc tính để lưu đường dẫn của video
        
        public string? VideoPath { get; set; }

        [Display(Name = "Video file")]
        [DataType(DataType.Upload)]
        [NotMapped]
        public IFormFile VideoFile { get; set; }

    }
}
