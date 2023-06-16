using Microsoft.AspNetCore.Mvc;
using ASP_1.Models.User;
using System.Text.RegularExpressions;
using ASP_1.Services.Hash;
using System.IO;
using ASP_1.Data;
using ASP_1.Services.Random;
using ASP_1.Data.Entity;
using ASP_1.Services.Kdf;
using Microsoft.Extensions.Primitives;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Claims;
using ASP_1.Services.Validation;
using ASP_1.Services.Email;
using ASP_1.Models;

namespace ASP_1.Controllers
{
    public class UserController : Controller
    {
        public readonly IHashService _hashService;
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dataContext;
        private readonly IRandomService _randomService;
        private readonly IKdfService _kdfService;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;

        public UserController(IHashService hashService, ILogger<UserController> logger, DataContext dataContext,
            IRandomService randomService, IKdfService kdfService, IValidationService validationService, IEmailService emailService)
        {
            _hashService = hashService;
            _logger = logger;
            _dataContext = dataContext;
            _randomService = randomService;
            _kdfService = kdfService;
            _validationService = validationService;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult Register(RegistrationModel registrationModel)
        {
            bool isModelValid = true;
            byte minPasswordLength = 3;
            RegisterValidationModel registerValidation = new();

            #region Login Validation
            if (String.IsNullOrEmpty(registrationModel.Login))
            {
                registerValidation.LoginResult = "Логин не может быть пустым";
                isModelValid = false;
            }
            if (_dataContext.Users.Any(u => u.Login == registrationModel.Login))
            {
                registerValidation.LoginResult = "Логин занят";
                isModelValid = false;
            }
            #endregion
            #region Password Validation / Repeat Password Validation
            if (String.IsNullOrEmpty(registrationModel.Password))
            {
                registerValidation.PasswordResult = "Пароль не может быть пустым";
                isModelValid = false;
            }
            else if (registrationModel.Password.Length < minPasswordLength)
            {
                registerValidation.PasswordResult = $"Пароль должен иметь больше {minPasswordLength} символов";
                registerValidation.RepeatPasswordResult = "";
                isModelValid = false;
            }
            else if (!registrationModel.Password.Equals(registrationModel.RepeatPassword))
            {
                registerValidation.PasswordResult =
                     registerValidation.RepeatPasswordResult = "Пароли не совпадают";
                isModelValid = false;
            }
            #endregion
            #region Email Validation
            if (!_validationService.Validate(registrationModel.Email, ValidationTerms.NotEmpty))
            {
                registerValidation.EmailResult = "Email не может быть пустым";
                isModelValid = false;
            }
            else if (!_validationService.Validate(registrationModel.Email, ValidationTerms.Email))
            {
                registerValidation.EmailResult = "Email не отвечает требованием";
                isModelValid = false;
            }
            #endregion
            #region Name Validation
            if (String.IsNullOrEmpty(registrationModel.RealName))
            {
                registerValidation.RealNameResult = "Имя не может быть пустым";
                isModelValid = false;
            }
            else
            {
                String nameRegex = @"^.+$";
                if (!Regex.IsMatch(registrationModel.RealName, nameRegex))
                {
                    registerValidation.RealNameResult = "Имя не отвечает требованием";
                    isModelValid = false;
                }
            }
            #endregion
            #region IsAgree Validation
            if (registrationModel.IsAgree == false)
            {
                registerValidation.IsAgreeResult = "Нужно принять все условия";
                isModelValid = false;
            }
            #endregion
            #region Avatar Load
            String savedName = null!;
            if (registrationModel.Avatar is not null)
            {
                if (registrationModel.Avatar.Length > 1024)
                {

                    String ext = Path.GetExtension(registrationModel.Avatar.FileName);
                    savedName = _randomService.RandomAvatar(registrationModel, ext);

                    String path = "wwwroot/avatars/" + savedName;
                    int i = 1;
                    while (System.IO.File.Exists(path))
                    {
                        savedName = _hashService.Hash(registrationModel.Avatar.FileName + DateTime.Now + i++)[..16] + ext;
                        path = "wwwroot/avatars/" + savedName;
                    }
                    using FileStream fs = new(path, FileMode.Create);
                    registrationModel.Avatar.CopyTo(fs);
                    ViewData["savedName"] = savedName;
                }
                else
                {
                    registerValidation.AvatarResult = "Размер файла должен быть боль 1Кб";
                }
            }

            #endregion


            if (isModelValid)
            {
                String salt = _randomService.RandomString(16);
                String confirmEmailCode = _randomService.ConfirmCode(6);

                User user = new()
                {
                    Id = Guid.NewGuid(),
                    Login = registrationModel.Login,
                    RealName = registrationModel.RealName,
                    Email = registrationModel.Email,
                    EmailCode = confirmEmailCode,
                    PasswordSalt = salt,
                    PasswordHash = _kdfService.GetDerivedKey(registrationModel.Password, salt),
                    Avatar = savedName,
                    RegisterDt = DateTime.Now,
                    LastEnterDt = null
                };
                _dataContext.Users.Add(user);


                var emailConfirmToken = _GenerateEmailConfirmToken(user);
                
                _SendConfirmEmail(user, emailConfirmToken);

                _dataContext.SaveChangesAsync();


                return View(registrationModel);
            }
            else
            {
                ViewData["registerValidationModel"] = registerValidation;
                return View("Registration");
            }

        }
        [HttpPost]
        public String AuthUser()
        {
            StringValues loginValues = Request.Form["user-login"];
            if (loginValues.Count == 0)
            {
                return "Missed required parametr: user-login";
            }
            String login = loginValues[0] ?? "";

            StringValues passwordValues = Request.Form["user-password"];
            if (passwordValues.Count == 0)
            {
                return "Missed required parametr: user-password";
            }
            String password = passwordValues[0] ?? "";

            User? user = _dataContext.Users.Where(u => u.Login == login).FirstOrDefault();
            if (user is not null)
            {
                if (user.PasswordHash ==
                _kdfService.GetDerivedKey(password, user.PasswordSalt))
                {
                    HttpContext.Session.SetString("authUserId", user.Id.ToString());
                    return "OK";
                }
            }

            return $"Авторизация отклонена";
        }
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Remove("authUserId");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Profile([FromRoute] String id)
        {
            User? user = _dataContext.Users.FirstOrDefault(u => u.Login == id);

            if (user is not null)
            {
                Models.User.ProfileModel model = new(user);
                if (HttpContext.User.Identity is not null
                    && HttpContext.User.Identity.IsAuthenticated)
                {
                    String userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)
                        .Value;
                    if(userLogin == user.Login)
                    {
                        model.IsPersonal = true;
                        model.IsModerator = user.IsModerator;
                        model.Description = user.Description;
                    }
                }
                return View(model);
            }
            else
            {
                return NotFound();
            }
           
        }
        [HttpPut]
        public IActionResult Update([FromBody] UpdateRequestModel model)
        {
            UpdateResponseModel responseModel = new();
            try
            {
                if (model is null) throw new Exception("No or empty data");
                if (HttpContext.User.Identity?.IsAuthenticated == false)
                {
                    throw new Exception("UnAuthenticated");
                }

                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid).Value
                        ));
                if (user is null) throw new Exception("UnAuthorized");

                switch(model.Field)
                {
                    case "realname":
                        if (_validationService.Validate(model.Value, ValidationTerms.RealName))
                        {
                            user.RealName = model.Value;
                            _dataContext.SaveChanges();
                        }
                        else throw new Exception(
                                $"Validation error: field '{model.Field}' with value '{model.Value}'");
                       
                        break;
                    case "description":
                        if (_validationService.Validate(model.Value, ValidationTerms.Description))
                        {
                            user.Description = model.Value;
                            _dataContext.SaveChanges();
                        }
                        else throw new Exception(
                                $"Validation error: field '{model.Field}' with value '{model.Value}'");

                        break;
                    default:
                        throw new Exception($"Invalid attribute '{model.Field}'") ;
                }

                responseModel.Status = "Ok";
                responseModel.Data = $"Field '{model.Field}' updated by value '{model.Value}'";
            }
            catch(Exception ex)
            {
                responseModel.Status = "Error";
                responseModel.Data = ex.Message;
            }
            return Json(responseModel);
        }
        [HttpPost]
        public JsonResult ConfirmEmail([FromBody] string emailCode)
        {
            StatusDataModel model = new();

            if(String.IsNullOrEmpty(emailCode))
            {
                model.Status = "406";
                model.Data = "Empty code not acceptable";
            }
            else if(HttpContext.User.Identity?.IsAuthenticated == false)
            {
                model.Status = "401";
                model.Data = "Unauthenticated";
            }
            else
            {
                User? user = _dataContext.Users.Find(
                   Guid.Parse(
                       HttpContext.User.Claims
                       .First(c => c.Type == ClaimTypes.Sid).Value
                       ));
                if (user is null)
                {
                    model.Status = "403";
                    model.Data = "Forbidden (UnAthorized)";
                }
                else if (user.EmailCode is null)
                {
                    model.Status = "208";
                    model.Data = "Already confirmed";
                }
                else if(user.EmailCode != emailCode)
                {
                    model.Status = "406";
                    model.Data = "Code not Accepted";
                }
                else
                {
                    user.EmailCode = null;
                    _dataContext.SaveChanges();
                    model.Status = "200";
                    model.Data = "Ok";
                }
            }
            return Json(model);
        }
        [HttpGet]
        public ViewResult ConfirmToken([FromQuery] String token)
        {
            try
            {
                var confirmToken = _dataContext.EmailConfirmTokens
                    .Find(Guid.Parse(token))
                    ?? throw new Exception();

                var user = _dataContext.Users.Find(
                    confirmToken.UserId)
                    ?? throw new Exception();

                if (user.Email != confirmToken.UserEmail)
                    throw new Exception();
                user.EmailCode = null;
                confirmToken.Used += 1;
                _dataContext.SaveChangesAsync();
                ViewData["result"] = "Почта успешно подтверждена!";
            }
            catch
            {
                ViewData["result"] = "Проверка не пройдена";
            }

            return View();
        }
        private bool _SendConfirmEmail(Data.Entity.User user, Data.Entity.EmailConfirmToken emailConfirmToken)
        {
            String confirmLink = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/User/ConfirmToken?token={emailConfirmToken.Id}";

           return _emailService.Send(
                "confirm_email", "congratulation_email",
                new Models.Email.ConfirmEmailModel
                {
                    Email = user.Email,
                    RealName = user.RealName,
                    EmailCode = user.EmailCode!,
                    ConfirmLink = confirmLink
                });
        }
        private EmailConfirmToken _GenerateEmailConfirmToken(Data.Entity.User user)
        {
            Data.Entity.EmailConfirmToken emailConfirmToken = new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UserEmail = user.Email,
                Moment = DateTime.Now,
                Used = 0
            };

            _dataContext.EmailConfirmTokens.Add(emailConfirmToken);
            return emailConfirmToken;
        }
        [HttpPatch]
        public String ResendConfirmEmail()
        {
            if(HttpContext.User.Identity?.IsAuthenticated == false)
            {
                return "Unauthenticated";
            }
            try
            {
                User? user = _dataContext.Users.Find(
                    Guid.Parse(
                        HttpContext.User.Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value
                        )) ?? throw new Exception();

                user.EmailCode = _randomService.ConfirmCode(6);
                var emailConfirmToken = _GenerateEmailConfirmToken(user);
                _dataContext.SaveChangesAsync();

                if (_SendConfirmEmail(user, emailConfirmToken))
                    return "OK";
                else
                    return "send err";
            }
            catch
            {
                return "Unauthorized";
            }
        }
    }


}
