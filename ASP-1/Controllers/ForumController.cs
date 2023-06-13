using ASP_1.Data;
using ASP_1.Models.Forum;
using ASP_1.Services.Transliterate;
using ASP_1.Services.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_1.Controllers
{
    public class ForumController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ForumController> _logger;
        private readonly IValidationService _validationService;
        private readonly ITransliterationService _transliterationService;
        public ForumController(DataContext dataContext, ILogger<ForumController> logger, IValidationService validationService, ITransliterationService transliterationService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _validationService = validationService;
            _transliterationService = transliterationService;
        }

        private int _counter = 0;

        private int Counter { get => _counter++; set => _counter = value; }
        public IActionResult Index()
        {
            Counter = 0;
            String? userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value;
            ForumIndexModel model = new()
            {

                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Sections = _dataContext
                .Sections
                .Include(s => s.Author)
                .Include(s => s.RateList)
                .Where(s => s.DeleteDt == null)
                .OrderBy(s => s.CreatedDt)
                .AsEnumerable()
                .Select(s => new ForumSectionViewModel()
                {
                    Title = s.Title,
                    Description = s.Description,
                    LogoUrl = $"/img/logos/section{Counter}.png",
                    UrlIdString = s.UrlId ?? s.Id.ToString(),
                    AuthorName = s.Author.IsRealNamePublic ? s.Author.RealName : s.Author.Login,
                    AuthorAvatarUrl = s.Author.Avatar == null ? "/avatars/noAvatar.png" : $"/avatars/{s.Author.Avatar}",
                    CreatedDtString = DateTime.Today == s.CreatedDt.Date ? "Сегодня" : s.CreatedDt.ToString("dd.MM.yyyy HH:mm"),
                    LikesCount = s.RateList.Count(r => r.Rating > 0),
                    DislikeCount = s.RateList.Count(r => r.Rating < 0),
                    GivenRating = userId == null ? null : s.RateList.FirstOrDefault(r => r.UserId == Guid.Parse(userId))?.Rating
                })
                .ToList()

            };
            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                HttpContext.Session.Remove("CreateSectionMessage");
                model.CreateMessage = message;
                model.isMessagePositive = HttpContext.Session.GetInt32("isMessagePositive") == 1;
                if(model.isMessagePositive == false)
                {
                    model.formModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                        
                    };
                }
                
                HttpContext.Session.Remove("SavedDescription");
                HttpContext.Session.Remove("SavedTitle");
            }

            return View(model);
        }
        public IActionResult Themes([FromRoute] String id)
        {
            Guid themeId;
            try
            {
                themeId = Guid.Parse(id);
            }
            catch
            {
                themeId = Guid.Empty;//_dataContext.Themes.First(s => s.UrlId == id).Id;
            }
            var theme = _dataContext.Themes.Find(themeId);
            if (theme == null)
            {
                return NotFound();
            }

            ForumThemesModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = theme.Title,
                ThemeId = id,
                Topics = _dataContext
                    .Topics
                    .Where(t => t.DeleteDt == null && t.ThemeId == themeId)
                    .Select(t => new ForumTopicViewModel()
                    {
                        Title = t.Title,
                        Description = t.Description,
                        UrlIdString = t.Id.ToString(),
                        CreatedDtString = DateTime.Today == t.CreatedDt.Date ? "Сегодня" : t.CreatedDt.ToString("dd.MM.yyyy HH:mm")
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                HttpContext.Session.Remove("CreateSectionMessage");
                model.CreateMessage = message;
                model.isMessagePositive = HttpContext.Session.GetInt32("isMessagePositive") == 1;
                if (model.isMessagePositive == false)
                {
                    model.formModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                }

                HttpContext.Session.Remove("SavedDescription");
                HttpContext.Session.Remove("SavedTitle");
            }

            return View(model);
        }
        public IActionResult Topics([FromRoute] String id)
        {
            Guid topicId;
            try
            {
                topicId = Guid.Parse(id);
            }
            catch
            {
                topicId = Guid.Empty;
            }
            var topic = _dataContext.Topics.Find(topicId);
            if(topic == null)
            {
                return NotFound();
            }
            ForumTopicsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                Title = topic.Title,
                Description = topic.Description,
                TopicId = id,
                Posts = _dataContext
                    .Posts
                    .Include(p => p.Author)
                    .Include(p => p.Reply)
                    .Where(p => p.DeleteDt == null && p.TopicId == topicId)
                    .Select(p => new ForumPostViewModel
                    {
                        Content = p.Content,
                        AuthorName = p.Author.IsRealNamePublic ? p.Author.RealName : p.Author.Login,
                        AuthorAvatarUrl = $"/avatars/{p.Author.Avatar ?? "noAvatar.png"}"
                    })
                    .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {

                model.CreateMessage = message;
                model.isMessagePositive = HttpContext.Session.GetInt32("isMessagePositive") == 1;
                if (model.isMessagePositive == false)
                {
                    model.formModel = new()
                    {
                        Content = HttpContext.Session.GetString("SavedContent")!,
                        ReplyId = HttpContext.Session.GetString("SavedReplyId")!
                    };
                }
                HttpContext.Session.Remove("CreateSectionMessage");
                HttpContext.Session.Remove("IsMessagePositive");
            }
                return View(model);
        }
        public ViewResult Sections([FromRoute] String id)
        {
            Guid sectionId;
            try
            {
                sectionId = Guid.Parse(id);
            }
            catch
            {
                sectionId = _dataContext.Sections.First(s => s.UrlId == id).Id;
            }
            ForumSectionsModel model = new()
            {
                UserCanCreate = HttpContext.User.Identity?.IsAuthenticated == true,
                SectionId = sectionId.ToString(),
                Themes = _dataContext
                .Themes
                .Include(t => t.Author)
                .Where(t => t.DeleteDt == null && t.SectionId == sectionId)
                .Select(t => new ForumThemeViewModel()
                {
                    Title = t.Title,
                    Description = t.Description,
                    CreatedDtString = DateTime.Today == t.CreatedDt.Date ? "Сегодня" : t.CreatedDt.ToString("dd.MM.yyyy HH:mm"),
                    UrlIdString = t.Id.ToString(),
                    SectionId = t.SectionId.ToString(),
                    AuthorName = t.Author.IsRealNamePublic ? t.Author.RealName : t.Author.Login,
                    AuthorAvatarUrl = $"/avatars/{t.Author.Avatar ?? "noAvatar.png"}"
                })
                .ToList()
            };

            if (HttpContext.Session.GetString("CreateSectionMessage") is String message)
            {
                HttpContext.Session.Remove("CreateSectionMessage");
                model.CreateMessage = message;
                model.isMessagePositive = HttpContext.Session.GetInt32("isMessagePositive") == 1;
                if (model.isMessagePositive == false)
                {
                    model.formModel = new()
                    {
                        Title = HttpContext.Session.GetString("SavedTitle")!,
                        Description = HttpContext.Session.GetString("SavedDescription")!
                    };
                }

                HttpContext.Session.Remove("SavedDescription");
                HttpContext.Session.Remove("SavedTitle");
            }


            return View(model);
        }

        [HttpPost]
        public RedirectToActionResult CreateSection(ForumSectionForumModel formModel)
        {
            _logger.LogInformation("Title: {t}, Description: {d}", formModel.Title, formModel.Description);

            if( ! _validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Название не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else  if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Описание не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);
                    string trans = _transliterationService.Transliterate(formModel.Title);
                    string UrlId = trans;
                    int n = 2;
                    while (_dataContext.Sections.Any(s => s.UrlId == UrlId))
                    {
                        UrlId = $"{trans}{n++}";
                    }

                    _dataContext.Sections.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        UrlId = UrlId
                    });
                    _dataContext.SaveChanges();
                    HttpContext.Session.SetString("CreateSectionMessage", "Успешно добавлено");
                    HttpContext.Session.SetInt32("isMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage", "Вам отказано в авторизации");
                    HttpContext.Session.SetInt32("isMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }
                
                
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public RedirectToActionResult CreateTheme(ForumThemeFormModel formModel)
        {
            if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Название не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Описание не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);
                    _dataContext.Themes.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        SectionId = Guid.Parse(formModel.SectionId)
                    });
                    _dataContext.SaveChanges();
                    
                    HttpContext.Session.SetString("CreateSectionMessage", "Успешно добавлено");
                    HttpContext.Session.SetInt32("isMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage", "Вам отказано в авторизации");
                    HttpContext.Session.SetInt32("isMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }


            }
            return RedirectToAction(nameof(Sections), new {id = formModel.SectionId});
        }

        [HttpPost]
        public RedirectToActionResult CreateTopic(ForumTopicFormModel formModel)
        {
            if (!_validationService.Validate(formModel.Title, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Название не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else if (!_validationService.Validate(formModel.Description, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Описание не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);
                    _dataContext.Topics.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Title = formModel.Title,
                        Description = formModel.Description,
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        ThemeId = Guid.Parse(formModel.ThemeId)
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateSectionMessage", "Успешно добавлено");
                    HttpContext.Session.SetInt32("isMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage", "Вам отказано в авторизации");
                    HttpContext.Session.SetInt32("isMessagePositive", 0);
                    HttpContext.Session.SetString("SavedTitle", formModel.Title ?? String.Empty);
                    HttpContext.Session.SetString("SavedDescription", formModel.Description ?? String.Empty);
                }


            }
            return RedirectToAction(nameof(Themes), new {id= formModel.ThemeId});
        }

        [HttpPost]
        public RedirectToActionResult CreatePost(ForumPostFormModel formModel)
        {
            if (!_validationService.Validate(formModel.Content, ValidationTerms.NotEmpty))
            {
                HttpContext.Session.SetString("CreateSectionMessage", "Ответ не может быть пустым");
                HttpContext.Session.SetInt32("isMessagePositive", 0);
                HttpContext.Session.SetString("SavedContent", formModel.Content ?? String.Empty);
                HttpContext.Session.SetString("SavedReplyId", formModel.ReplyId ?? String.Empty);
            }
            else
            {
                Guid userId;
                try
                {
                    userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value);
                    _dataContext.Posts.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        Content = formModel.Content,
                        ReplyId = String.IsNullOrEmpty(formModel.ReplyId) ? null : Guid.Parse(formModel.ReplyId),
                        CreatedDt = DateTime.Now,
                        AuthorId = userId,
                        TopicId = Guid.Parse(formModel.TopicId)
                    });
                    _dataContext.SaveChanges();

                    HttpContext.Session.SetString("CreateSectionMessage", "Успешно добавлено");
                    HttpContext.Session.SetInt32("isMessagePositive", 1);
                }
                catch
                {
                    HttpContext.Session.SetString("CreateSectionMessage", "Вам отказано в авторизации");
                    HttpContext.Session.SetInt32("isMessagePositive", 0);
                    HttpContext.Session.SetString("SavedContent", formModel.Content ?? String.Empty);
                    HttpContext.Session.SetString("SavedReplyId", formModel.ReplyId ?? String.Empty);
                }


            }
            return RedirectToAction(nameof(Topics), new { id = formModel.TopicId });
        }
    }
}
