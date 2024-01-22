using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IFileService _fileService;
        private readonly IGenreService _genreService;
        private readonly UserManager<ApplicationUser> _userManager;
        public MovieController(IMovieService MovieService, IFileService fileService, IGenreService genreService, UserManager<ApplicationUser> userManager)
        {
             this._userManager= userManager;
            _movieService = MovieService;
            _fileService = fileService;
            _genreService= genreService;
        }
        public IActionResult Add()
        {
            var model = new Movie();
            model.GenreList = _genreService.List().Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });


            return View(model);
        }

        [HttpPost]
        public IActionResult Add(Movie model)
        {
            model.GenreList = _genreService.List().Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });


            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ImageFile != null)
            {
                var fileResult = this._fileService.UploadImage(model.ImageFile);
                model.MovieImage = fileResult;
            }
            if (model.VideoFile != null)
            {
                var fileResult = this._fileService.UploadVideo(model.VideoFile);
                model.VideoPath = fileResult;
            }
            var result = _movieService.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successully";
                return RedirectToAction(nameof(Add));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }

        }

        public IActionResult Edit(int id)
        {
            var model = _movieService.GetById(id);
            //   model.GenreList = _genreService.List().Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            var selectedGenres = _movieService.GetGenreByMovieId(model.Id);
            MultiSelectList multiGenreList = new MultiSelectList(_genreService.List(),"Id","GenreName",selectedGenres);
            model.MultiGenreList = multiGenreList;
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Movie model)
        {
            //  model.GenreList = _genreService.List().Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = a.GenreName, Value = a.Id.ToString() });
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.ImageFile != null)
            {
                var fileResult = this._fileService.UploadImage(model.ImageFile);
                if (string.IsNullOrEmpty(fileResult))
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                model.MovieImage = fileResult;
            }
            if (model.VideoFile != null)
            {
                var fileResult = this._fileService.UploadVideo(model.VideoFile);
                if (string.IsNullOrEmpty(fileResult))
                {
                    TempData["msg"] = "File could not saved";
                    return View(model);
                }
                model.VideoPath = fileResult;
            }
            var result = _movieService.Update(model);
            if (result)
            {
                TempData["msg"] = "Added Successully";
                return RedirectToAction(nameof(MovieList));
            }
            else
            {
                TempData["msg"] = "Error on server side";
                return View(model);
            }

        }


        public IActionResult MovieList()
        {
            var data = this._movieService.List();
            return View(data);
        }

        
        public IActionResult Delete(int id)
        {
            var result = _movieService.Delete(id);
            return RedirectToAction(nameof(MovieList));

        }

        [HttpPost]
        public async Task<IActionResult> WatchMovie(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            string userId = user.Id.ToString();
            await _movieService.AddWatchHistory(userId, id);
            var movie = _movieService.GetById(id);

            
            return View(movie);
        }



    }
}
