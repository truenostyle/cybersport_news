namespace ASP_1.Data.Entity
{
    public class Section
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public DateTime CreatedDt { get; set; }
        public DateTime? DeleteDt { get; set; }
        public String? UrlId { get; set; } = null!;

        public User Author { get; set; } = null!;
        public List<Rate> RateList { get; set; } = null!;
    }
}
