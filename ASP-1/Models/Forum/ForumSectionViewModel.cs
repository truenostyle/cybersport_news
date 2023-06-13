namespace ASP_1.Models.Forum
{
    public class ForumSectionViewModel
    {
        public String Title { get; set; } = null!;
        public String Description { get; set; } = null!;
        public String LogoUrl { get; set; } = null!;
        public String CreatedDtString { get; set; } = null!;
        public String UrlIdString { get; set; } = null!;


        public String AuthorName { get; set; } = null!;
        public String AuthorAvatarUrl { get; set; } = null!;
        public int LikesCount { get; set; }
        public int DislikeCount { get; set; }
        public int? GivenRating { get; set; }
    }
}
