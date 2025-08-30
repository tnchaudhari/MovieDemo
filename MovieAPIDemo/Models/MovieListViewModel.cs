using System.ComponentModel.DataAnnotations;

namespace MovieAPIDemo.Models
{
    public class MovieListViewModel
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public List<ActorViewModel>? Actors { get; set; }
        public required string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
    }
}
