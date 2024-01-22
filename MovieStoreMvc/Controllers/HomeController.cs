using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;
using MovieStoreMvc.Repositories.Implementation;

namespace MovieStoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IMovieService movieService, IGenreService genreService, UserManager<ApplicationUser> userManager)
        {   
            _genreService = genreService;
            _movieService = movieService;
            this._userManager= userManager;
        }

        public IActionResult Index(string term = "", int currentPage = 1, List<int> selectedGenres = null)
        {
            var movies = _movieService.List(term, true, currentPage, selectedGenres);
            movies.GenreList = _genreService.List().Select(a => new SelectListItem { Text = a.GenreName, Value = a.Id.ToString() }).ToList();
            movies.SelectedGenres = selectedGenres;
            return View(movies);
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult MovieDetail(int movieId)
        {
            var movie = _movieService.GetById(movieId);

            return View(movie);
        }

        public async Task<IActionResult> WatchedMovies()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var userId = user.Id.ToString();
                
                var watchedMovies = _movieService.GetWatchedMovies(userId);

                var watchedMoviesViewModel = new WatchedMoviesViewModel
                {
                    WatchedMovies = watchedMovies
                };

                return View(watchedMoviesViewModel);
            }

            return RedirectToAction("Index");
        }
    }
}
