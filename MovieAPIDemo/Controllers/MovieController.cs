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
                var movieListViewModel = _context.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize).
                    Select(x => new MovieListViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Language = x.Language,
                        CoverImage = x.CoverImage,
                        ReleaseDate = x.ReleaseDate,
                        Actors = x.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                    }).ToList();

                response.Status = true;
                response.Message = "Success";
                response.Data = new
                {
                    Movies = movieListViewModel,
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
                var movieDetailsViewModel = _context.Movie.Include(x => x.Actors).Where(x => x.Id == id).
                    Select(x => new MovieDetailsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Language = x.Language,
                        CoverImage = x.CoverImage,
                        ReleaseDate = x.ReleaseDate,
                        Description = x.Description,
                        Actors = x.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                    }).FirstOrDefault();

                if (movieDetailsViewModel == null)
                {
                    response.Status = false;
                    response.Message = "Record Does Not Exist.";
                    return BadRequest(response);
                }

                response.Status = true;
                response.Message = "Success";
                response.Data = movieDetailsViewModel;

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

                    var responseData = new MovieDetailsViewModel
                    {
                        Id = newMovie.Id,
                        Title = newMovie.Title,
                        Language = newMovie.Language,
                        CoverImage = newMovie.CoverImage,
                        ReleaseDate = newMovie.ReleaseDate,
                        Description = newMovie.Description,
                        Actors = newMovie.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                    };

                    response.Status = true;
                    response.Message = "Created Successfully.";
                    response.Data = responseData;

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

                    var movieDetails = _context.Movie.Include(x => x.Actors).Where(y => y.Id == model.Id).FirstOrDefault();

                    if (movieDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid movie record.";
                        return BadRequest(response);
                    }

                    movieDetails.Title = model.Title;
                    movieDetails.CoverImage = model.CoverImage ?? "";
                    movieDetails.Language = model.Language;
                    movieDetails.Description = model.Description ?? "";
                    movieDetails.ReleaseDate = model.ReleaseDate;
                    movieDetails.Actors = actors;

                    _context.Movie.Update(movieDetails);
                    _context.SaveChanges();

                    var responseData = new MovieDetailsViewModel
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Language = movieDetails.Language,
                        CoverImage = movieDetails.CoverImage,
                        ReleaseDate = movieDetails.ReleaseDate,
                        Description = movieDetails.Description,
                        Actors = movieDetails.Actors.Select(y => new ActorViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DateOfBirth = y.DateOfBirth,
                        }).ToList(),
                    };

                    response.Status = true;
                    response.Message = "Updated Successfully.";
                    response.Data = responseData;

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
