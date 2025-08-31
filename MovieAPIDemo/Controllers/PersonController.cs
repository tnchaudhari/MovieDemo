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
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly IMapper _mapper;

        public PersonController(MovieDbContext context, IMapper mapper)
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
                var actorCount = _context.Person.Count();
                var actorListViewModel = _mapper.Map<List<ActorViewModel>>(_context.Person.Skip(pageIndex * pageSize).Take(pageSize).ToList());

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
                var actorData = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (actorData == null)
                {
                    response.Status = false;
                    response.Message = "Record Does Not Exist.";
                    return BadRequest(response);
                }

                var actorViewModel = new ActorDetailsViewModel
                {
                    Id = actorData.Id,
                    Name = actorData.Name,
                    DateOfBirth = actorData.DateOfBirth,
                    Movies = _context.Movie.Where(x => x.Actors.Contains(actorData)).Select(y => y.Title).ToArray(),
                };

                response.Status = true;
                response.Message = "Success";
                response.Data = actorViewModel;

                return Ok(response);
            }
            catch (Exception)
            {
                response.Status = false;
                response.Message = "Something went wrong";

                return BadRequest(response);
            }
        }


        [HttpGet("Search/{searchText}")]
        public IActionResult Get(string searchText)
        {
            BaseResponseModel response = new();
            try
            {
                var searchedPerson = _context.Person.Where(x => x.Name.Contains(searchText)).Select(y => new
                {
                    y.Id,
                    y.Name,
                }).ToList();


                response.Status = true;
                response.Message = "Success";
                response.Data = searchedPerson;

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

                    var actorDetails = _context.Person.Where(y => y.Id == model.Id).AsNoTracking().FirstOrDefault();

                    if (actorDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid actor record.";
                        return BadRequest(response);
                    }

                    var actorData = _mapper.Map<Person>(model);
                    actorData.ModifiedDate = DateTime.UtcNow;

                    _context.Person.Update(actorData);
                    _context.SaveChanges();

                    response.Status = true;
                    response.Message = "Updated Successfully.";
                    response.Data = actorData;

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
                var person = _context.Person.Where(x => x.Id == id).FirstOrDefault();

                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Invalid Person Record.";
                    return BadRequest(response);
                }

                _context.Person.Remove(person);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Deleted Successfully.";
                response.Data = person;

                return Ok(response);
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
