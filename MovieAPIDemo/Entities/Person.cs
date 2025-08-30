namespace MovieAPIDemo.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
    }
}
