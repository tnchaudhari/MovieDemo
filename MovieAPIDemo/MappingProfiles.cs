using AutoMapper;
using MovieAPIDemo.Entities;
using MovieAPIDemo.Models;

namespace MovieAPIDemo
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            //Movie
            CreateMap<Movie, MovieListViewModel>();
            CreateMap<Movie, MovieDetailsViewModel>();
            CreateMap<MovieListViewModel, Movie>();
            CreateMap<CreateMovieViewModel, Movie>().ForMember(x => x.Actors, y => y.Ignore());


            //Person
            CreateMap<Person, ActorViewModel>();
            CreateMap<Person, ActorDetailsViewModel>();
            CreateMap<ActorViewModel, Person>();
        }
    }
}
