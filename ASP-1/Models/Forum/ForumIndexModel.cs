namespace ASP_1.Models.Forum
{
    public class ForumIndexModel
    {
        public List<ForumSectionViewModel> Sections { get; set; } = null!;
        public Boolean UserCanCreate { get; set; }


        public String? CreateMessage { get; set; }
        public bool? isMessagePositive { get; set; }
        public ForumSectionForumModel? formModel { get; set; }
    }
}
