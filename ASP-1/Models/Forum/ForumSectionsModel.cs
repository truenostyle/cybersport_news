namespace ASP_1.Models.Forum
{
    public class ForumSectionsModel
    {
        public Boolean UserCanCreate { get; set; }
        public String SectionId { get; set; } = null!;
        public List<ForumThemeViewModel> Themes { get; set; } = null!;

        public String? CreateMessage { get; set; }
        public bool? isMessagePositive { get; set; }
        public ForumThemeFormModel formModel { get; set; } = null!;
    }
}
