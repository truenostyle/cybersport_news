namespace ASP_1.Data.Entity
{
    public class Theme
    {
        public Guid Id { get; set; }
        public Guid SectionId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public DateTime CreatedDt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public User Author { get; set; } = null!;
    }
}
