using System.Net.Http.Headers;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public MovieController(MovieDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new();
            try
            {
                var movieCount = _context.Movie.Count();
                var movieListViewModel = _mapper.Map<List<MovieListViewModel>>(_context.Movie.Include(x => x.Actors).Skip(pageIndex * pageSize).Take(pageSize).ToList());

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
                var movie = _context.Movie.Include(x => x.Actors).Where(x => x.Id == id).FirstOrDefault();

                if (movie == null)
                {
                    response.Status = false;
                    response.Message = "Record Does Not Exist.";
                    return BadRequest(response);
                }

                var movieData = _mapper.Map<MovieDetailsViewModel>(movie);

                response.Status = true;
                response.Message = "Success";
                response.Data = movieData;

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

                    var newMovie = _mapper.Map<Movie>(model);

                    newMovie.Actors = actors;

                    _context.Movie.Add(newMovie);
                    _context.SaveChanges();

                    var responseData = _mapper.Map<MovieDetailsViewModel>(newMovie);

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
                        response.Message = "Invalid Movie Record.";
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

                    var responseData = _mapper.Map<MovieDetailsViewModel>(movieDetails);

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


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var movie = _context.Movie.Where(x => x.Id == id).FirstOrDefault();

                if(movie == null)
                {
                    response.Status = false;
                    response.Message = "Invalid Movie Record.";
                    return BadRequest(response);
                }

                _context.Movie.Remove(movie);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Deleted Successfully.";
                response.Data = movie;

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong.";
                return BadRequest(response);
            }
        }

        [HttpPost("upload-movie-poster")]
        public async Task<IActionResult> UploadMoviePoster(IFormFile image)
        {
            try
            {
                var fileName = ContentDispositionHeaderValue.Parse(image.ContentDisposition).FileName.TrimStart('\"').TrimEnd('\"');
                string newPath = @"N:\to-delete";

                if (!Directory.Exists(newPath))
                {
                    Directory.CreateDirectory(newPath);
                }

                string[] allowedImageExtensions = [".jpg", ".jpeg", ".png"];

                if (!allowedImageExtensions.Contains(Path.GetExtension(fileName)))
                {
                    return BadRequest(new BaseResponseModel
                    {
                        Status = false,
                        Message = "Only .jpg, .jpeg, .png files are only allowed"
                    });
                }

                string newFileName = Guid.NewGuid() + Path.GetExtension(fileName);
                string fullFilePath = Path.Combine(newPath, newFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                return Ok(new { ProfileImage = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/StaticFiles/{newFileName}" });
            }
            catch (Exception)
            {
                return BadRequest(new BaseResponseModel
                {
                    Status = false,
                    Message = "Error Occurred."
                });
            }
        }
    }
}
