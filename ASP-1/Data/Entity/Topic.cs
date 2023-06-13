namespace ASP_1.Data.Entity
{
    public class Topic
    {
        public Guid Id { get; set; }
        public Guid ThemeId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public DateTime CreatedDt { get; set; }
        public DateTime? DeleteDt { get; set; }
    }
}
