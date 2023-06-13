using ASP_1.Data;
using ASP_1.Models;
using ASP_1.Services;
using ASP_1.Services.Hash;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ASP_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DateService _dateService;
        private readonly TimeService _timeService;
        private readonly StampService _stampService;
        private readonly IHashService _hashService;
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,
                                      DateService dateService,
                                      TimeService timeService,
                                      StampService stampService,
                                      IHashService hashService,
                                      DataContext dataContext,
                                      IConfiguration configuration)
        {
            _logger = logger;
            _dateService = dateService;
            _timeService = timeService;
            _stampService = stampService;
            _hashService = hashService;
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["authUser"] = HttpContext.Session.GetString("authUserId");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Intro()
        {
            

            return View();
        }

        public IActionResult Scheme()
        {
            ViewBag.data = "Data is ViewBag";
            ViewData["data"] = "Data in ViewData";
            return View();
        }

        public IActionResult Url()
        {
            return View();
        }
        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult PassData()
        {
            Models.Home.PassDataModel model = new()
            {
                Header = "Модели",
                Title = "Модели передачи данных",
                Products = new()
                {
                    new() { Name = "Зарядный Кабель", Price = 210},
                    new() { Name = "Клавиатура", Price = 50.40},
                    new() { Name = "Мышь", Price = 60.50},
                    new() { Name = "Монитор", Price = 150},
                    new() { Name = "Блок питания", Price = 350}
                }
            };
            return View(model);
        }

        public IActionResult DisplayTemplates ()
        {
            Models.Home.PassDataModel model = new()
            {
                Header = "Шаблоны",
                Title = "Шаблоны отображения данных",
                Products = new()
                {
                    new() { Name = "Зарядный Кабель", Price = 210,   Image = "product1.png"},
                    new() { Name = "Клавиатура",      Price = 50.40, Image = "product2.png"},
                    new() { Name = "Мышь",            Price = 60.50, Image = "product3.jpg"},
                    new() { Name = "Монитор",         Price = 150},
                    new() { Name = "Блок питания",    Price = 350}
                }
            };
            return View(model);
        }

        public IActionResult TagHelpers()
        {
            return View();  
        }

        public ViewResult Services()
        {
            ViewData["date_service"] = _dateService.GetMoment();
            ViewData["date_hashcode"] = _dateService.GetHashCode();

            ViewData["time_service"] = _timeService.GetMoment();    
            ViewData["time_hashcode"] = _timeService.GetHashCode();

            ViewData["stamp_service"] = _stampService.GetMoment();
            ViewData["stamp_hashcode"] = _stampService.GetHashCode();

            ViewData["hash_service"] = _hashService.Hash("123");
            return View();
        }

        public ViewResult Sessions([FromQuery(Name = "session-attr")]String? sessionAttr)
        {
            if(sessionAttr is not null)
            {
                HttpContext.Session.SetString("session-attribute", sessionAttr);
            }
            return View();
        }

        public ViewResult Middleware()
        {
            
            return View();
        }

        public ViewResult EmailConfirmation()
        {
            ViewData["configHost"] = _configuration["Smtp:Gmail:Host"];
            ViewData["configPort"] = _configuration["Smtp:Gmail:Port"];
            ViewData["configEmail"] = _configuration["Smtp:Gmail:Email"];
            ViewData["configSsl"] = _configuration["Smtp:Gmail:Ssl"];
            return View();
        }

        public ViewResult Context()
        {
            ViewData["UsersCount"] = _dataContext.Users.Count();

            
            //const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            //var random = new Random();
            //var code = new string(Enumerable.Repeat(chars, 6)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}