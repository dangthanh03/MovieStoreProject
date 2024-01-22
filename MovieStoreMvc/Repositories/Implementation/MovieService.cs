using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Repositories.Implementation
{
    public class MovieService : IMovieService
    {
        private readonly DatabaseContext ctx;
      
        public MovieService(DatabaseContext ctx)
        {

            this.ctx = ctx;
        }
        public bool Add(Movie model)
        {
            try
            {
                ctx.Movie.Add(model);
                ctx.SaveChanges();
                foreach (int genreId in model.Genres)
                {
                    var movieGenre = new MovieGenre
                    {
                        MovieId = model.Id,
                        GenreId = genreId
                    };

                    ctx.MovieGenre.Add(movieGenre);
                }
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var data = this.GetById(id);
                if (data == null)
                {
                    return false;
                }

                var movieGenres = ctx.MovieGenre.Where(a => a.MovieId == data.Id);
                foreach (var movieGenre in movieGenres)
                {
                    ctx.MovieGenre.Remove(movieGenre);
                }
                ctx.Movie.Remove(data);
                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Movie GetById(int id)
        {
            var movie = ctx.Movie.Find(id);

            var genres = (from genre in ctx.Genre
                          join mg in ctx.MovieGenre on genre.Id equals mg.GenreId
                          where mg.MovieId == movie.Id
                          select ((string)genre.GenreName).ToString()  // Explicit cast
             ).ToList();

            var genreNames = string.Join(',', genres);
            movie.GenreNames = genreNames;

            return movie;
        }

        public MovieListVm List(string term = "", bool paging = false, int currentPage = 0, List<int> selectedGenres = null)
        {
            var data = new MovieListVm();
            var list = ctx.Movie.ToList();

            if (!string.IsNullOrEmpty(term))
            {
                term = term.ToLower();
                list = list.Where(a => a.Title.ToLower().StartsWith(term)).ToList();
            }

            // Lọc theo thể loại
            // Ví dụ với bảng MovieGenre có cấu trúc (MovieId, GenreId)
            if (selectedGenres != null && selectedGenres.Any())
            {
                list = (from movie in list
                        join movieGenre in ctx.MovieGenre on movie.Id equals movieGenre.MovieId
                        where selectedGenres.Contains(movieGenre.GenreId)
                        select movie).ToList();
            }

            if (paging)
            {
                int pageSize = 5;
                int count = list.Count;
                int TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                list = list.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                data.PageSize = pageSize;
                data.CurrentPage = currentPage;
                data.TotalPages = TotalPages;
            }

            foreach (var movie in list)
            {
                var genres = (from genre in ctx.Genre
                              join mg in ctx.MovieGenre on genre.Id equals mg.GenreId
                              where mg.MovieId == movie.Id
                              select genre.GenreName
                ).ToList();

                var genreNames = string.Join(',', genres);
                movie.GenreNames = genreNames;
            }

            data.MovieList = list.AsQueryable();
            return data;
        }

        public bool Update(Movie model)
        {
            try
            {
                var genresToDelete = ctx.MovieGenre.Where(a => a.MovieId == model.Id && !model.Genres.Contains(a.GenreId)).ToList();
                foreach (var mGenre in genresToDelete)
                {

                    ctx.MovieGenre.Remove(mGenre);
                }
                foreach (int genId in model.Genres)
                {
                    var movieGenre = ctx.MovieGenre.FirstOrDefault(a => a.MovieId == model.Id && a.GenreId == genId);
                    if (movieGenre == null)
                    {
                        movieGenre = new MovieGenre { GenreId = genId, MovieId = model.Id };
                        ctx.MovieGenre.Add(movieGenre);
                    }
                }
                ctx.Movie.Update(model);

                ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<int> GetGenreByMovieId(int movieID)
        {
            var genreIds = ctx.MovieGenre.Where(a => a.MovieId == movieID).Select(a => a.GenreId).ToList();
            return genreIds;
        }

        public async Task AddWatchHistory(string userId, int movieId)
        {
         

          
                var watchHistory = ctx.WatchHistories
                    .SingleOrDefault(w => w.UserId == userId && w.MovieId == movieId);

                if (watchHistory == null)
                {
                    ctx.WatchHistories.Add(new WatchHistory
                    {
                        UserId = userId,
                        MovieId = movieId
                    });

                    await ctx.SaveChangesAsync();
                }
            
        }

        public List<Movie> GetWatchedMovies(string userId)
        {
            var watchedMovies = (from wh in ctx.WatchHistories
                                 where wh.UserId == userId
                                 join movie in ctx.Movie on wh.MovieId equals movie.Id
                                 select movie).ToList();

            return watchedMovies;
        }



    }
}
