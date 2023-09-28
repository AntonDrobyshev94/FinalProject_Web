using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Interfaces;
using FinalProject_Web.Vars;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace FinalProject_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IApplicationData _contactData;

        public AccountController(IApplicationData contactData)
        {
            _contactData = contactData;
        }

        /// <summary>
        /// Get запрос, в результате которого происходит переход
        /// на страницу Login (входа в аккаунт). В результате
        /// запроса происходит запоминание адреса страницы 
        /// при помощи ключевого слова return для дальнейшего
        /// возврата на эту страницу.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            SetVariables();
            return View();
        }

        /// <summary>
        /// Асинхронный метод, принимающий модель UserLogin,
        /// реализованную отдельным классом и возвращающий результат
        /// выполнения данной модели. В методе происходит проверка
        /// принимаемой модели на валидность и если модель валидна,
        /// то создается с помощью метода IsLogin создается запрос
        /// в API для проверки модели и получения токена в случае
        /// успешной авторизации. Если полученный токен равен
        /// пустой строке, то произойдет ошибка авторизации. Если
        /// не равен, то происходит проверка содержимого токена 
        /// на наличие Claims Администратора и имени пользователя,
        /// которые записываются в файлы Куки для дальнейшего
        /// использования.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult Login(UserLogin model)
        {
            SetVariables();
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null && model.Password != null)
                {
                    string loginProp = model.LoginProp;
                    string password = model.Password;
                    UserLoginProp userLogin = new UserLoginProp()
                    {
                        UserName = loginProp,
                        Password = password
                    };
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true
                    };
                    string token = string.Empty;
                    token = _contactData.IsLogin(userLogin);
                    if (token != string.Empty)
                    {
                        Response.Cookies.Append("AuthToken", token, cookieOptions);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);
                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                            {
                                if (item.Value == "Admin")
                                {
                                    Response.Cookies.Append("RoleCookie", item.Value, cookieOptions);
                                    break;
                                }
                                else
                                {
                                    Response.Cookies.Append("RoleCookie", item.Value, cookieOptions);
                                }
                            }
                        }
                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                            {
                                Response.Cookies.Append("UserNameCookie", item.Value, cookieOptions);
                                break;
                            }
                        }
                        return RedirectToAction("Index", "Web");
                    }
                    else
                    {
                        Response.Cookies.Append("AuthToken", "", cookieOptions);
                        TempData["AlertLogin"] = "Не верный логин или пароль";
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
            ModelState.AddModelError("", "Пользователь не найден");
            return View(model);
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации, который
        /// отправляет новый экземпляр UserRegistration в
        /// представление Register в качестве модели.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            SetVariables();
            return View(new UserRegistration());
        }

        /// <summary>
        /// Асинхронный Post запрос, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе
        /// с помощью метода IsRegiseter происходит запрос в API
        /// на регистрацию нового пользователя. Если полученный 
        /// токен равен пустой строке, то произойдет ошибка 
        /// авторизации. Если не равен, то происходит запись токена,
        /// имени пользователя и роли User  в куки для дальнейшего
        /// использования. В конце происходит редирект на главную
        /// страницу.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult Register(UserRegistration model)
        {
            SetVariables();
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    string token = string.Empty;
                    token = _contactData.IsRegister(model);

                    if (token != string.Empty)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtToken = tokenHandler.ReadJwtToken(token);

                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true
                        };

                        Response.Cookies.Append("AuthToken", token, cookieOptions);
                        Response.Cookies.Append("RoleCookie", "User", cookieOptions);

                        foreach (var item in jwtToken.Claims)
                        {
                            if (item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                            {
                                Response.Cookies.Append("UserNameCookie", item.Value, cookieOptions);
                            }
                        }
                        return RedirectToAction("Index", "Web");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ошибка регистрации");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Пароли не совпадают или ошибка формата");
            }
            return View(model);
        }

        /// <summary>
        /// Метод, осуществляющий переход на страницу входа Account
        /// Login.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            LogoutMethod();

            return RedirectToAction("Login", "Account");
        }

        /// Метод выхода из учетной записи. В данном
        /// методе происходит перезапись текущих куки на новые
        /// значения, которые равны пустым строкам (т.е. обнуление
        /// аутентификационных данных пользователя, хранящихся в куки).
        public void LogoutMethod()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };

            Response.Cookies.Append("AuthToken", string.Empty, cookieOptions);
            Response.Cookies.Append("RoleCookie", string.Empty, cookieOptions);
            Response.Cookies.Append("UserNameCookie", string.Empty, cookieOptions);
            Response.Cookies.Append("IsEditMode", string.Empty, cookieOptions);
        }

        public void SetVariables()
        {
            ViewBag.TitleModel = Variables.titleModelVars;
            if (!string.IsNullOrEmpty(Request.Cookies["IsEditMode"]))
            {
                if (Request.Cookies["IsEditMode"] == "true")
                {
                    ViewBag.IsEditMode = true;
                }
                else
                {
                    ViewBag.IsEditMode = false;
                }
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true
                };
                Response.Cookies.Append("IsEditMode", "false", cookieOptions);
                ViewBag.IsEditMode = false;
            }
        }
    }
}
