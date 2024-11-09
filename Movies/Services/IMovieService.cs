using Movies.Models;

namespace Movies.Services
{
    public interface IMovieService
    {
        Task<IEnumerable<Movie>> GetAll(byte id = 0);

        Task<Movie> GetById(int id);
        Task<Movie> Add(Movie movie);
        Movie Update(Movie movie);
        Movie Delete(Movie movie);
    }
}
