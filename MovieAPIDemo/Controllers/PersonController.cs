using Microsoft.AspNetCore.Mvc;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;

        public PersonController(MovieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new();
            try
            {
                var actorCount = _context.Person.Count();
                var actorListViewModel = _context.Person.Skip(pageIndex * pageSize).Take(pageSize).
                    Select(x => new ActorViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        DateOfBirth = x.DateOfBirth,
                    }).ToList();

                response.Status = true;
                response.Message = "Success";
                response.Data = new
                {
                    Persons = actorListViewModel,
                    Count = actorCount
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
        public IActionResult GetPersonById(int id)
        {
            BaseResponseModel response = new();

            try
            {
                var actorViewModel = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (actorViewModel == null)
                {
                    response.Status = false;
                    response.Message = "Record Does Not Exist.";
                    return BadRequest(response);
                }

                var actorData = new ActorDetailsViewModel
                {
                    Id = actorViewModel.Id,
                    Name = actorViewModel.Name,
                    DateOfBirth = actorViewModel.DateOfBirth,
                    Movies = _context.Movie.Where(x => x.Actors.Contains(actorViewModel)).Select(y => y.Title).ToArray(),
                };

                response.Status = true;
                response.Message = "Success";
                response.Data = actorData;

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
        public IActionResult Post(ActorViewModel model)
        {
            BaseResponseModel response = new();

            try
            {
                if (ModelState.IsValid)
                {
                    var newActor = new Person()
                    {
                       Name = model.Name,
                       DateOfBirth = model.DateOfBirth,
                    };

                    _context.Person.Add(newActor);
                    _context.SaveChanges();

                    model.Id = newActor.Id;

                    response.Status = true;
                    response.Message = "Created Successfully.";
                    response.Data = model;

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
        public IActionResult Put(ActorViewModel model)
        {
            BaseResponseModel response = new();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid actor record.";
                        return BadRequest(response);
                    };

                    var actorDetails = _context.Person.Where(y => y.Id == model.Id).FirstOrDefault();

                    if (actorDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid actor record.";
                        return BadRequest(response);
                    }

                    actorDetails.Name = model.Name;
                    actorDetails.DateOfBirth = model.DateOfBirth;
                    actorDetails.ModifiedDate = DateTime.UtcNow;


                    _context.Person.Update(actorDetails);
                    _context.SaveChanges();

                    response.Status = true;
                    response.Message = "Updated Successfully.";
                    response.Data = actorDetails;

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
