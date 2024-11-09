using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.Dtos;
using Movies.Models;
using Movies.Services;

namespace Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenresController(IGenresService genresService)
        {
            
            _genresService = genresService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genres = await _genresService.GetAll();

            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            Genre genre = new() { Name = dto.Name };

            await _genresService.Add(genre);

            return Ok(genre);
        }

        [HttpPut("{id:int}")]  //api/genres/id
        public async Task<IActionResult> UpdateAsync(byte id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _genresService.GetById(id);

            if (genre is null)
                return NotFound($"No genre found with Id : {id}");

            genre.Name = dto.Name;

            _genresService.Update(genre);


            return Ok(genre);
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(byte id)
        {
            var genre = await _genresService.GetById(id);

            if (genre is null)
                return NotFound($"No genre found with Id : {id}");

            _genresService.Delete(genre);


            return Ok();
        }
        
    }
}
