using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieDbContext _context;

        public MovieController(MovieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new();
            try
            {
                var movieCount = _context.Movie.Count();
                var movieList = _context.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize).ToList();

                response.Status = true;
                response.Message = "Success";
                response.Data = new
                {
                    Movies = movieList,
                    Count = movieCount
                };

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            BaseResponseModel response = new();

            try
            {
                var movie = _context.Movie.Include(x => x.Actors).Where(x => x.Id == id).FirstOrDefault();

                if (movie == null)
                {
                    response.Status = false;
                    response.Message = "Record Not Exist.";
                    return BadRequest(response);
                }

                response.Status = true;
                response.Message = "Success";
                response.Data = movie;

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }

        [HttpPost]
        public IActionResult Post(CreateMovieViewModel model)
        {
            BaseResponseModel response = new();

            try
            {
                if (ModelState.IsValid)
                {
                    var actors = _context.Person.Where(x => model.Actors!.Contains(x.Id)).ToList();

                    if (actors.Count != model.Actors!.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned.";

                        return BadRequest(response);
                    }

                    var newMovie = new Movie()
                    {
                        Title = model.Title,
                        CoverImage = model.CoverImage ?? "",
                        Language = model.Language,
                        Description = model.Description ?? "",
                        ReleaseDate = model.ReleaseDate,
                        Actors = actors,
                    };

                    _context.Movie.Add(newMovie);
                    _context.SaveChanges();
                    response.Status = true;
                    response.Message = "Created Successfully.";
                    response.Data = newMovie;

                    return Ok(response);
                }
                response.Status = false;
                response.Message = "Validation Failed.";
                response.Data = ModelState;

                return BadRequest(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong.";
                return BadRequest(response);
            }
        }

        [HttpPut]
        public IActionResult Put(CreateMovieViewModel model)
        {
            BaseResponseModel response = new();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid movie record.";
                        return BadRequest(response);
                    }

                    var actors = _context.Person.Where(x => model.Actors!.Contains(x.Id)).ToList();

                    if (actors.Count != model.Actors!.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned.";

                        return BadRequest(response);
                    }

                    var movie = _context.Movie.Include(x => x.Actors).Where(y => y.Id == model.Id).FirstOrDefault();

                    if (movie == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid movie record.";
                        return BadRequest(response);
                    }

                    movie.Title = model.Title;
                    movie.CoverImage = model.CoverImage ?? "";
                    movie.Language = model.Language;
                    movie.Description = model.Description ?? "";
                    movie.ReleaseDate = model.ReleaseDate;
                    movie.Actors = actors;

                    _context.Movie.Update(movie);
                    _context.SaveChanges();
                    response.Status = true;
                    response.Message = "Updated Successfully.";
                    response.Data = movie;

                    return Ok(response);
                }
                response.Status = false;
                response.Message = "Validation Failed.";
                response.Data = ModelState;

                return BadRequest(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong.";
                return BadRequest(response);
            }
        }
    }
}
