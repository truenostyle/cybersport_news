namespace ASP_1.Data.Entity
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public Guid? ReplyId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedDt { get; set; }
        public DateTime? DeleteDt { get; set; }

        public User Author { get; set; } = null!;
        public Post? Reply { get; set; }
    }
}
