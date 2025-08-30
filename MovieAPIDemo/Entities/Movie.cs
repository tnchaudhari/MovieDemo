namespace MovieAPIDemo.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }

        // List of Actors
        public ICollection<Person> Actors { get; set; } = new List<Person>();
        public required string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? CoverImage { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
    }
}
