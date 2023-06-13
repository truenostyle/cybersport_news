using Microsoft.AspNetCore.Mvc;

namespace ASP_1.Models.Forum
{
    public class ForumSectionForumModel
    {
        [FromForm(Name = "section-title")]
        public string Title { get; set; } = null!;


        [FromForm(Name = "section-description")]
        public string Description { get; set; } = null!;
    }
}
