using Microsoft.AspNetCore.Mvc;
using FinalProject_Web.Model;
using FinalProject_Web.Vars;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Controllers
{
    public class WebController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IApplicationData _applicationData;
        
        public WebController(IWebHostEnvironment hostEnvironment, 
            IApplicationData applicationData)
        {
            this._hostEnvironment = hostEnvironment;
            this._applicationData = applicationData;
        }

        /// <summary>
        /// Метод перехода во View главной страницы.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            CheckCookieMethod();
            SetVariables();
            IsRedact(true);
            GetTagViewMethod();
            return View();
        }
        #region Application

        /// <summary>
        /// Метод добавления заявки, принимающей строковые переменные имени, 
        /// почты и сообщения. В методе используется блок try catch,
        /// в котором происходит запуск метода обращения к API для добавления
        /// заявки AddAplication. При успешном запросе происходит передача
        /// новой заявки в API. В TempData сохраняется результат успешной
        /// отправки, который считывается во View и отображается для 
        /// пользователя в виде сообщения об успешной отправке. В противном
        /// случае в TempData сохраняется результат ошибки и считывается
        /// во View.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eMail"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddRequest(string name, string eMail, string message)
        {
            bool isCorrect = false;
            string requestMessage = string.Empty;
            try
            {
                var application = new Application()
                {
                    Name = name,
                    EMail = eMail,
                    Message = message,
                    Date = DateTime.Now,
                    Status = "Получена"
                };
                _applicationData.AddApplication(application, HttpContext);
                requestMessage = "Заявка успешно отправлена";
                isCorrect = true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                
                requestMessage = "Произошла ошибка при отправке заявки";
                isCorrect = false;
            }
            TempData["RequestMessage"] = requestMessage;
            TempData["IsCorrectRequest"] = isCorrect;
            return Redirect("~/");
        }

        /// <summary>
        /// Метод изменения статуса заявки, принимающий строковую переменную
        /// заявки, значение id заявки и остальные переменные (их значения
        /// не учитываются). Изменение статуса происходит с помощью метода
        /// взаимодействия с API ChangeApplicationStatus
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="eMail"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeRequestStatus(string status, int id, string name, string eMail, string message, DateTime date)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            _applicationData.ChangeApplicationStatus(status, id, name, eMail,message,date, HttpContext);
            return Redirect("~/Web/DekstopWindow");
        }

        /// <summary>
        /// Метод перехода во View рабочего стола, в котором происходит
        /// проверка наличия куки диапазона дат. Если куки не установлены, 
        /// то проверяются куки рассматриваемого периода, в зависимости
        /// от которого устанавливается диапазон дат. Если куки установлены,
        /// то период устаналивается из принимаемых значений диапазона.
        /// Далее, происходит перебор коллекции заявок в цикле и запись
        /// заявок, совпадающих по условию диапазона дат, в новую коллекцию
        /// заявок с дальнейшей передачей коллекции во ViewBag в качестве
        /// модели.
        /// </summary>
        /// <returns></returns>
        public IActionResult DekstopWindow()
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            string period = string.Empty;
            int allRequestCounter = 0;
            int currentRequestCounter = 0;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            CheckCookieMethod();
            SetVariables();
            IsRedact(false);
            if (!string.IsNullOrEmpty(Request.Cookies["StartDate"]) &&
                !string.IsNullOrEmpty(Request.Cookies["EndDate"]))
            {
                DateTime.TryParse(Request.Cookies["StartDate"], out DateTime startingDate);
                DateTime.TryParse(Request.Cookies["EndDate"], out DateTime endingDate);
                startDate = startingDate;
                endDate = endingDate;
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true
                };
                if (!string.IsNullOrEmpty(Request.Cookies["Period"]))
                {
                    period = Request.Cookies["Period"];
                    if (period == string.Empty)
                    {
                        startDate = DateTime.Now.AddDays(-1000);
                        endDate = DateTime.Now.AddDays(1000);
                    }
                    else if (period == "today")
                    {
                        startDate = DateTime.Now.Date;
                        endDate = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
                    }
                    else if (period == "yesterday")
                    {
                        DateTime yesterday = DateTime.Now.AddDays(-1).Date;
                        startDate = yesterday;
                        DateTime endOfYesterday = yesterday.AddDays(1).AddSeconds(-1);
                        endDate = endOfYesterday;
                    }
                    else if (period == "week")
                    {
                        DateTime today = DateTime.Now.Date;
                        int daysToSubtract = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
                        DateTime startOfWeek = today.AddDays(-daysToSubtract);

                        startDate = startOfWeek;

                        int daysUntilEndOfWeek = (int)DayOfWeek.Saturday - (int)today.DayOfWeek;
                        DateTime endOfWeek = today.AddDays(daysUntilEndOfWeek).AddDays(1).AddSeconds(-1);

                        endDate = endOfWeek;
                    }
                    else if (period == "month")
                    {
                        DateTime today = DateTime.Now.Date;
                        DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);

                        startDate = startOfMonth;

                        DateTime startOfNextMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1);
                        DateTime endOfMonth = startOfNextMonth.AddSeconds(-1);
                        endDate = endOfMonth;
                    }
                    
                    Response.Cookies.Append("StartDate", startDate.ToString(), cookieOptions);
                    Response.Cookies.Append("EndDate", endDate.ToString(), cookieOptions);
                }
                else
                {
                    startDate = DateTime.Now.AddDays(-10000);
                    endDate = DateTime.Now.AddDays(10000);
                    Response.Cookies.Append("StartDate", startDate.ToString(), cookieOptions);
                    Response.Cookies.Append("EndDate", endDate.ToString(), cookieOptions);
                }
            }
            
            List<Application> applicationsModel = new List<Application>();
            foreach (var application in _applicationData.GetApplications(HttpContext))
            {
                allRequestCounter++;
                if (application.Date > startDate && application.Date < endDate)
                {
                    currentRequestCounter++;
                    applicationsModel.Add(application);
                }
            }
            ViewBag.ButtonChangeColor = period;
            ViewBag.AllRequestCounter = allRequestCounter;
            ViewBag.CurrentRequestCounter = currentRequestCounter;
            ViewBag.ApplicationList = applicationsModel;
            ViewBag.StartDate = startDate.ToShortDateString();
            ViewBag.EndDate = endDate.ToShortDateString();
            return View();
        }

        /// <summary>
        /// Метод записи периода дат, принимающий строковое значение
        /// периода. В методе происходит запись значения в куки
        /// для дальнейшего использования.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public IActionResult DateMethod(string period)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };
            if (period == null)
            {
                period = string.Empty;
                Response.Cookies.Append("Period", period, cookieOptions);
            }
            else
            {
                Response.Cookies.Append("Period", period, cookieOptions);
                Response.Cookies.Append("StartDate", string.Empty, cookieOptions);
                Response.Cookies.Append("EndDate", string.Empty, cookieOptions);
            }
            
            return RedirectToAction("DekstopWindow", "Web");
        }

        /// <summary>
        /// Метод определения диапазона дат, принимающий в себя начальную дату
        /// и конечную дату. Полученные даты записываются в куки.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DateSelect(DateTime startDate, DateTime endDate)
        {
            DateTime theEndDate = endDate.AddDays(1).AddSeconds(-1);
            string startDateStr = startDate.ToString();
            string endDateStr = theEndDate.ToString();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };
            Response.Cookies.Append("StartDate", startDateStr, cookieOptions);
            Response.Cookies.Append("EndDate", endDateStr, cookieOptions);
            return RedirectToAction("DekstopWindow", "Web");
        }
        #endregion
        #region Services
        /// <summary>
        /// Метод перехода во View меню Услуги.
        /// В качестве модели во View передаётся коллекция
        /// услуг List<Service>, получаемая по запросу в API
        /// с помощью метода _applicationData.GetServices.
        /// </summary>
        /// <returns></returns>
        public IActionResult Services()
        {
            CheckCookieMethod();
            SetVariables();
            IsRedact(true);
            GetTagViewMethod();
            return View(_applicationData.GetServices());
        }

        /// <summary>
        /// Метод перехода во View добавления услуги
        /// </summary>
        /// <returns></returns>
        public IActionResult AddServiceWindow()
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            SetVariables();
            IsRedact(false);
            GetTagViewMethod();
            return View();
        }

        /// <summary>
        /// Метод добавления услуги, в котором с помощью метода
        /// _applicationData.AddService происходит создание
        /// нового проекта в блоге с параметрами, переданными 
        /// в экземпляре Service.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IActionResult AddService(string name, string description)
        {
            CheckCookieMethod();
            var model = new Service
            {
                Name = name,
                Description = description
            };
            if (ModelState.IsValid)
            {
                _applicationData.AddService(model, HttpContext);
            }
            return RedirectToAction("Services");
        }

        /// <summary>
        /// Метод удаления услуги. Метод принимает int значение id 
        /// услуги и с помощью метода _applicationData.DeleteService
        /// происходит запрос на удаление услуги в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteService(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();

            _applicationData.DeleteService(id, HttpContext);
            return RedirectToAction("Services");
        }

        /// <summary>
        /// Метод перехода во View изменения услуги, в
        /// котором происходит поиск конкретной услуги с помощью
        /// метода FindServiceById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeServiceWindow(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            Service concreteService = _applicationData.FindServiceById(id, HttpContext);
            SetVariables();
            IsRedact(false);
            return View(concreteService);
        }

        /// <summary>
        /// Метод изменения услуги, в котором происходит изменение услуги
        /// методом _applicationData.ChangeService
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeService(string name, string description, int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            _applicationData.ChangeService(name, description, id, HttpContext);
            return RedirectToAction("Services");
        }
        #endregion
        #region Projects

        /// <summary>
        /// Метод перехода во View меню Проектов.
        /// В качестве модели во View передаётся коллекция
        /// проектов List<ProjectModel>, получаемая из API
        /// методом _applicationData.GetProjects
        /// </summary>
        /// <returns></returns>
        public IActionResult Projects()
        {
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(true);
            return View(_applicationData.GetProjects());
        }

        /// <summary>
        /// Метод перехода во View добавления проекта
        /// </summary>
        /// <returns></returns>
        public IActionResult AddProjectWindow()
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(false);
            return View();
        }

        /// <summary>
        /// Метод добавления проекта, в котором создаётся
        /// уникальное имя для файла картинки проекта и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _applicationData.AddProject происходит создание
        /// нового проекта в блоге с параметрами, переданными 
        /// в экземпляре ProjectModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddProject(IFormFile imageFile, string name, string description)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            try
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                UploadImageMethod(imageFile, uniqueFileName);
                
                ProjectModel newProject = new ProjectModel
                {
                    Name = name,
                    Description = description,
                    ImageName = uniqueFileName
                };
                _applicationData.AddProject(newProject, HttpContext);
                return RedirectToAction("Projects");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Projects");
            }
            
        }

        /// <summary>
        /// Метод перехода во View изменения проекта, в
        /// котором происходит поиск конкретного проекта с помощью
        /// метода FindProjectById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeProjectWindow(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            ProjectModel concreteProject = _applicationData.FindProjectById(id, HttpContext);
            SetVariables();
            IsRedact(false);
            return View(concreteProject);
        }

        /// <summary>
        /// Метод изменения проекта, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение проекта
        /// методом _applicationData.ChangeProject
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="imageName"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeProject(IFormFile imageFile, string imageName, string name, string description, int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
            UploadImageMethod(imageFile, Path.Combine(uploadsFolder, imageName));

            _applicationData.ChangeProject(name, imageName, description, id, HttpContext);
            SetVariables();
            return RedirectToAction("Projects");
        }

        /// <summary>
        /// Метод перехода во View "детали проекта" 
        /// (открывает конкретный проект). В методе
        /// происходит поиск конкретного проекта с помощью
        /// метода FindProjectById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ProjectDetails(int id)
        {
            CheckCookieMethod();
            ProjectModel concreteProject = _applicationData.FindProjectById(id);
            SetVariables();
            IsRedact(false);
            return View(concreteProject);
        }

        /// <summary>
        /// Метод удаления проекта. Метод принимает int значение id 
        /// проекта. В методе происходит сохранение в экземпляр ProjectModel 
        /// результата запроса FindProjectById по указанному Id проекта.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _applicationData.DeleteProject
        /// происходит запрос на удаление проекта в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteProject(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            ProjectModel concreteProject = new ProjectModel();
            try
            {
                concreteProject = _applicationData.FindProjectById(id, HttpContext);
                _applicationData.DeleteProject(id ,HttpContext);
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
                var uniqueFileName = concreteProject.ImageName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return RedirectToAction("Projects");
            }
            catch
            {
                return RedirectToAction("Projects");
            }
        }
        #endregion
        #region Blog
        /// <summary>
        /// Метод перехода во View блога.
        /// В качестве модели во View передаётся
        /// коллекция List<BlogModel> (запрос в API
        /// с помощью метода _applicationData.GetBlog)
        /// </summary>
        /// <returns></returns>
        public IActionResult Blog()
        {
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(true);
            return View(_applicationData.GetBlog());
        }

        /// <summary>
        /// Метод перехода во View добавления поста в блог
        /// </summary>
        /// <returns></returns>
        public IActionResult AddBlogWindow()
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(false);
            return View();
        }

        /// <summary>
        /// Метод добавления поста, в котором создаётся
        /// уникальное имя для файла картинки поста и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _applicationData.AddBlog происходит создание
        /// нового поста в блоге с параметрами, переданными 
        /// в экземпляре BlogModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="blogPost"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddBlog(IFormFile imageFile, string name, string description, string blogPost)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            try
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                UploadImageMethod(imageFile, uniqueFileName);

                BlogModel newBlog = new BlogModel
                {
                    Name = name,
                    Description = description,
                    BlogPost = blogPost,
                    ImageName = uniqueFileName,
                    DateTimePublication = DateTime.Now
                };
                _applicationData.AddBlog(newBlog, HttpContext);
                return RedirectToAction("Blog");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Blog");
            }
        }

        /// <summary>
        /// Метод перехода во View изменения поста в блоге, в
        /// котором происходит поиск конкретного поста с помощью
        /// метода FindBlogById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeBlogPostWindow(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            if (id != null)
            {
                GetTagViewMethod();
                BlogModel concretePost = _applicationData.FindBlogById(id);
                SetVariables();
                IsRedact(false);
                return View(concretePost);
            }
            else
            {
                SetVariables();
                return RedirectToAction("Blog");
            }
        }

        /// <summary>
        /// Метод изменения поста, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение поста
        /// методом _applicationData.ChangeBlog
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="imageName"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="blogPost"></param>
        /// <returns></returns>
        public IActionResult ChangeBlogPost(IFormFile imageFile, string imageName, string name, string description, int id, string blogPost)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
            UploadImageMethod(imageFile, Path.Combine(uploadsFolder, imageName));

            _applicationData.ChangeBlog(name, imageName, description, blogPost, id, HttpContext);
            SetVariables();
            return RedirectToAction("Blog");
        }

        /// <summary>
        /// Метод перехода во View "детали поста в блоге" 
        /// (открывает конкретный пост в блоге). В методе
        /// происходит поиск конкретного поста с помощью
        /// метода FindBlogById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult BlogPostDetails(int id)
        {
            CheckCookieMethod();
            if (id != null)
            {
                ViewBag.TitleModel = false;
                GetTagViewMethod();
                BlogModel concretePost = _applicationData.FindBlogById(id);
                SetVariables();
                IsRedact(false);
                return View(concretePost);
            }
            else
            {
                SetVariables();
                return RedirectToAction("Blog");
            }
        }

        /// <summary>
        /// Метод удаления блога. Метод принимает int значение id 
        /// блога. В методе происходит сохранение в экземпляр BlogModel 
        /// результата запроса FindLinkById по указанному Id блога.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _applicationData.DeleteBlog
        /// происходит запрос на удаление блога в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteBlogPost(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            BlogModel concretePost = new BlogModel();
            try
            {
                concretePost = _applicationData.FindBlogById(id, HttpContext);
                _applicationData.DeleteBlog(id, HttpContext);
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
                var uniqueFileName = concretePost.ImageName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return RedirectToAction("Blog");
            }
            catch
            {
                return RedirectToAction("Blog");
            }
        }
        #endregion
        #region Contacts
        /// <summary>
        /// Метод для перехода во View меню Контакты.
        /// В качестве модели во View передаётся коллекция объектов
        /// List<LinkModel>
        /// </summary>
        /// <returns></returns>
        public IActionResult Contacts()
        {
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(true);
            ViewBag.Contacts = Variables.contacts;
            return View(_applicationData.GetLinks());
        }

        /// <summary>
        /// Метод для перехода во View изменения контакта. Во
        /// View метода в качестве модели передаётся экземпляр
        /// Contacts.
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangeContactWindow()
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            SetVariables();
            IsRedact(false);
            return PartialView(Variables.contacts);
        }

        /// <summary>
        /// Метод изменения контакта, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение проекта
        /// методом _applicationData.ChangeContacts
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="fileName"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="fax"></param>
        /// <param name="telephone"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeContact(IFormFile imageFile, string fileName, string address,
            string email, string fax, string telephone)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            UploadImageMethod(imageFile, fileName);
            Variables.contacts = new Contacts
            {
                Address = address,
                Email = email,
                Id = 0,
                Fax = fax,
                Telephone = telephone
            };

            _applicationData.ChangeContacts(Variables.contacts, HttpContext);
            SetVariables();
            return RedirectToAction("Contacts");
        }
        #endregion
        #region ContactLinks

        /// <summary>
        /// Метод перехода во View добавления ссылки
        /// </summary>
        /// <returns></returns>
        public IActionResult AddLinkWindow()
        {
            GetTagViewMethod();
            SetVariables();
            IsRedact(false);
            return PartialView();
        }

        /// <summary>
        /// Метод добавления ссылки, в котором создаётся
        /// уникальное имя для файла картинки ссылки и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _applicationData.AddLink происходит создание
        /// новой ссылки с параметрами, переданными в экземпляре
        /// LinkModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddLink(IFormFile imageFile, string url)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            try
            {
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                UploadImageMethod(imageFile, uniqueFileName);

                LinkModel newLink = new LinkModel
                {
                    ImageName = uniqueFileName,
                    Url = url
                };
                _applicationData.AddLink(newLink, HttpContext);
                return RedirectToAction("Contacts");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Contacts");
            }
        }

        /// <summary>
        /// Метод перехода на View изменения ссылки, в котором
        /// происходит поиск и кеширование модели ссылки с
        /// помощью метода FindLinkById по id, который принимается
        /// методом. По окончанию экземпляр ссылки передаётся
        /// во View в качестве модели.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeLinkWindow(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            GetTagViewMethod();
            LinkModel concreteLink = _applicationData.FindLinkById(id, HttpContext);
            SetVariables();
            IsRedact(false);
            return View(concreteLink);
        }

        /// <summary>
        /// Метод изменения ссылки, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение ссылки
        /// методом _applicationData.ChangeLink
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="url"></param>
        /// <param name="imageName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ChangeLink(IFormFile imageFile, string url, string imageName, int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
            UploadImageMethod(imageFile, Path.Combine(uploadsFolder, imageName));
            _applicationData.ChangeLink(imageName, url, id, HttpContext);
            SetVariables();
            return RedirectToAction("Contacts");
        }
        /// <summary>
        /// Метод удаления ссылки. Метод принимает int значение id 
        /// ссылки. В методе происходит сохранение в экземпляр LinkModel 
        /// результата запроса FindLinkById по указанному Id ссылки.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _applicationData.DeleteLink
        /// происходит запрос на удаление ссылки в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteLink(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            LinkModel concreteLink = new LinkModel();
            try
            {
                concreteLink = _applicationData.FindLinkById(id, HttpContext);
                _applicationData.DeleteLink(id, HttpContext);
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
                var uniqueFileName = concreteLink.ImageName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return RedirectToAction("Contacts");
            }
            catch
            {
                return RedirectToAction("Contacts");
            }
        }
        #endregion
        #region Tag
        /// <summary>
        /// Метод добавления тэга
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns></returns>
        public IActionResult AddTag(string newTag)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            var tag = new TagModel
            {
                Tag = newTag
            };
            _applicationData.AddTagMethod(tag, HttpContext);
            return Redirect("~/");
        }

        /// <summary>
        /// Метод удаления тэга
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteTag(int id)
        {
            if (!_applicationData.CheckToken(HttpContext))
            {
                LogoutMethod();
                return RedirectToAction("Login", "Account");
            }
            CheckCookieMethod();
            _applicationData.DeleteTag(id, HttpContext);
            return Redirect("~/");
        }
        /// <summary>
        /// Метод генерации случайного значения тэга, 
        /// который возвращает результат генерации случайного 
        /// значения типа int с помощью ключевого слова return
        /// Диапазон случайных значений определяется с помощью
        /// перебора в цикле foreach коллекции TagModel,
        /// принимаемой с помощью метода GetTags. В результате
        /// каждой итерации происходит увеличение счётчика
        /// counter на значение +1.
        /// </summary>
        /// <returns></returns>
        private int RandomNumberTagMethod()
        {
            List<TagModel> tagModelsList = new List<TagModel>();
            tagModelsList = _applicationData.GetTags().ToList();
            int counter = 1;
            foreach (var tag in tagModelsList)
            {
                counter++;
            }
            Random random = new Random();
            int randomNumber = random.Next(1, counter);
            return randomNumber;
        }

        /// <summary>
        /// Метод для передачи случайного id тэга и коллекции тэгов
        /// во View невозвращаемого типа
        /// </summary>
        private void GetTagViewMethod()
        {
            ViewBag.RandomTag = RandomNumberTagMethod();
            ViewBag.TagModelList = _applicationData.GetTags();
        }
        #endregion
        #region Other
        /// <summary>
        /// Метод загрузки изображения, принимающий в себя Http запрос IFormFile
        /// изображения и имени файла, в котором происходит сохранение изображения
        /// с указаным названием в папку wwwroot/img 
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="fileName"></param>
        public void UploadImageMethod(IFormFile imageFile, string fileName)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img");
                var uniqueFileName = fileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }
            }
        }

        /// <summary>
        /// Метод установки значений переменных невозвращаемого типа,
        /// в котором происходит запись значений во ViewBag для
        /// модели TitleModel, а также в зависимости от значения куки
        /// с ключем IsEditMode устанавливается режим Viewbag.IsEditMode.
        /// </summary>
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

        /// <summary>
        /// Метод редактирования невозвращаемого типа, который служит 
        /// для определения, редактируется ли View, в которой он применяется.
        /// Метод принимает в себя bool переменную. Далее происходит
        /// проверка переменной. При условии true во ViewBag.IsRedactWindow
        /// передаётся значение true. В обратном случае передаётся false,
        /// а также передаётся значение false для ViewBag.IsEditMode
        /// (чтобы выключить режим редактирования).
        /// </summary>
        /// <param name="isTrue"></param>
        public void IsRedact(bool isTrue)
        {
            if (isTrue)
            {
                ViewBag.IsRedactWindow = true;
            }
            else
            {
                ViewBag.IsRedactWindow = false;
                ViewBag.IsEditMode = false;
            }
        }

        /// <summary>
        /// Метод проверки наличия куки с именем пользователя, ролью и
        /// токеном. Если куки существуют и не равны нулю или пустой 
        /// строке, то происходит их запись в строковые переменные.
        /// Если строка токена не равна нулю или пустой строке, то
        /// происходит запись значение IsAuth во ViewBag равное true.
        /// Значение имени и роли также записываются во ViewBag.
        /// </summary>
        private void CheckCookieMethod()
        {
            if (!string.IsNullOrEmpty(Request.Cookies["UserNameCookie"]) &&
                !string.IsNullOrEmpty(Request.Cookies["RoleCookie"]) &&
                !string.IsNullOrEmpty(Request.Cookies["AuthToken"]))
            {
                string nameValue = Request.Cookies["UserNameCookie"];
                string roleValue = Request.Cookies["RoleCookie"];
                string tokenValue = Request.Cookies["AuthToken"];
                if (!string.IsNullOrEmpty(tokenValue))
                {
                    ViewBag.IsAuth = true;
                }
                else
                {
                    ViewBag.IsAuth = false;
                }
                ViewBag.UserName = nameValue;
                ViewBag.RoleName = roleValue;
            }
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
        }

        public IActionResult EditModeMethod(string viewName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true
            };
            if (!string.IsNullOrEmpty(Request.Cookies["IsEditMode"]))
            {
                string mode = Request.Cookies["IsEditMode"];
                if (mode == "true")
                {
                    mode = "false";
                    Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
                else
                {
                    mode = "true";
                    Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
            }
            else
            {
                Response.Cookies.Append("IsEditMode", "false", cookieOptions);
            }

            return RedirectToAction(viewName);
        }

        public IActionResult EditTitleMethod(string newTitle, string mainTitle,
            string servicesTitle, string projectsTitle,
            string blogTitle, string contactsTitle)
        {
            if (!string.IsNullOrEmpty(newTitle))
            {
                Variables.titleModelVars.Title = newTitle;
            }
            if (!string.IsNullOrEmpty(mainTitle))
            {
                Variables.titleModelVars.MainTitle = mainTitle;
            }
            if (!string.IsNullOrEmpty(servicesTitle))
            {
                Variables.titleModelVars.ServicesTitle = servicesTitle;
            }
            if (!string.IsNullOrEmpty(projectsTitle))
            {
                Variables.titleModelVars.ProjectsTitle = projectsTitle;
            }
            if (!string.IsNullOrEmpty(blogTitle))
            {
                Variables.titleModelVars.BlogTitle = blogTitle;
            }
            if (!string.IsNullOrEmpty(contactsTitle))
            {
                Variables.titleModelVars.ContactsTitle = contactsTitle;
            }
            try
            {
                _applicationData.ChangeTitle(Variables.titleModelVars, HttpContext);

                ViewBag.TitleModel = Variables.titleModelVars;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile imageFile, string fileName)
        {
            UploadImageMethod(imageFile, fileName);
            return RedirectToAction("Index");
        }
        #endregion
    }
}