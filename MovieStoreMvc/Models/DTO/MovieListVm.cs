using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace MovieStoreMvc.Models.DTO
{
    public class MovieListVm
    {
        [Display(Name = "Genres")]
        public List<int> SelectedGenres { get; set; }
        public List<SelectListItem> GenreList { get; set; }
        public IQueryable<Movie> MovieList { get; set; }
        public int PageSize {  get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages {  get; set; }
        public string? Term { get; set; }
    
    }
}
