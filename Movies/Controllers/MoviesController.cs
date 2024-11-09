using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Dtos;
using Movies.Models;
using Movies.Services;

namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        
        private readonly IMovieService _movieservice;
        private readonly IGenresService _genreservice;
        private readonly IMapper _mapper;


        private new List<string> _allowedExtension = new() { ".jpg", ".png" };

        private long _maxAllowedPosterSize = 1048576; //1024 * 1024

        public MoviesController(IMovieService movieservice, IGenresService genreservice, IMapper mapper)
        {

            _movieservice = movieservice;
            _genreservice = genreservice;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _movieservice.GetAll();
            //TO-DO: Map movies to DTO
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);


            return Ok(data);
        }


        [HttpGet("GenreId")]
        public async Task<IActionResult> GetMovieByGenreIdAsync(byte Gid)
        {
            var movies = await _movieservice.GetAll(Gid);
            var data = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(data);
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _movieservice.GetById(id);




            if (movie == null)
                return NotFound();

            var dto = _mapper.Map<MovieDetailsDto>(movie);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {

            if (!_allowedExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Incorrect file extension");

            if(dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max file is 1MB");

            var isValidGenre = await _genreservice.isValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");

            using var datastream = new MemoryStream();
            await dto.Poster.CopyToAsync(datastream);
            Movie movie = _mapper.Map<Movie>(dto);
            movie.Poster = datastream.ToArray();

            _movieservice.Add(movie);

            return Ok(movie);

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _movieservice.GetById(id);

            if (movie is null)
                return NotFound($"No movie was found with id: {id}");

            var isValidGenre = await _genreservice.isValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");

            if(dto.Poster != null)
            {
                if (!_allowedExtension.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Incorrect file extension");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max file is 1MB");

                using var datastream = new MemoryStream();
                await dto.Poster.CopyToAsync(datastream);
                movie.Poster = datastream.ToArray();
            }

            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoryLine = dto.StoryLine;
            movie.Rate = dto.Rate;

            _movieservice.Update(movie);
            return Ok(movie);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var movie = await _movieservice.GetById(id);

            if (movie is null)
                return NotFound($"No movie was found with id: {id}");

            _movieservice.Delete(movie);

            return Ok();


        }
    }
}
