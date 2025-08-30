using System.ComponentModel.DataAnnotations;

namespace MovieAPIDemo.Models
{
    public class CreateMovieViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name of the Movie is Required.")]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public List<int>? Actors { get; set; }
        public required string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
    }
}
