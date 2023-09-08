using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Interfaces;
using FinalProject_Web.Model;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FinalProject_Web.Data
{
    public class ApplicationDataApi: IApplicationData
    {
        private HttpClient httpClient { get; set; }

        public ApplicationDataApi()
        {
            httpClient = new HttpClient();
        }

        #region Application
        /// <summary>
        /// Запрос на получение всех заявок, передающийся на API 
        /// сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится 
        /// токен. Запрос возвращает результат в виде коллекции
        /// объектов типа Application.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IEnumerable<Application> GetApplications(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            AddTokenHeaderMethod(httpContext);
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<Application>>(json);
        }

        /// <summary>
        /// Запрос на создание новой заявки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="httpContext"></param>
        public void AddApplication(Application application, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(application), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление заявки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteApplication(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на изменение статуса заявки, передающийся 
        /// на API сервер. Данный запрос принимает строковую переменную,
        /// которая используется для создания нового экземпляра Application
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Application. Данный метод является невозвратным.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="eMail"></param>
        /// <param name="message"></param>
        /// <param name="date"></param>
        /// <param name="httpContext"></param>
        public void ChangeApplicationStatus(string status, int id, string name,
            string eMail, string message, DateTime date, HttpContext httpContext)
        {
            Application application = new Application()
            {
                ID = id,
                Name = name,
                EMail = eMail,
                Message = message,
                Date = date,
                Status = status
            };

            string url = $"https://localhost:7037/api/values/ChangeStatus";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(application), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }
        #endregion
        #region Projects
        /// <summary>
        /// Запрос на получение всех проектов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа ProjectModel.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProjectModel> GetProjects()
        {
            string url = @"https://localhost:7037/api/values/GetProjects";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<ProjectModel>>(json);
        }

        /// <summary>
        /// Запрос на создание новго проекта, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="httpContext"></param>
        public void AddProject(ProjectModel project, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/AddProject";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(project), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на изменение проекта, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра ProjectModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра ProjectModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageName"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void ChangeProject(string name, string imageName, string description, 
            int id, HttpContext httpContext)
        {
            ProjectModel project = new ProjectModel()
            {
                Id = id,
                Description = description,
                Name = name,
                ImageName = imageName,
            };

            string url = $"https://localhost:7037/api/values/ChangeProject";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(project), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteProject(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/DeleteProject/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на поиск проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра ProjectModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public ProjectModel FindProjectById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/ProjectDetails/{id}";
            AddTokenHeaderMethod(httpContext);
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<ProjectModel>(json);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Запрос на поиск проекта по указанному id, передающийся 
        /// на API сервер. Является второй перегрузкой метода, не принимающей
        /// httpcontext. Запрос возвращает результат в 
        /// виде экземпляра ProjectModel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProjectModel FindProjectById(int id)
        {
            string url = $"https://localhost:7037/api/values/ProjectDetails/{id}";
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<ProjectModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Services
        /// <summary>
        /// Запрос на получение всех услуг, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа Service.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Service> GetServices()
        {
            string url = @"https://localhost:7037/api/values/GetServices";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<Service>>(json);
        }

        /// <summary>
        /// Запрос на создание новой услуги, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContext"></param>
        public void AddService(Service service, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/AddService";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на изменение услуги, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра Service
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Service. Данный метод является невозвратным.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void ChangeService(string name, string description,
            int id, HttpContext httpContext)
        {
            Service service = new Service()
            {
                Id = id,
                Description = description,
                Name = name
            };

            string url = $"https://localhost:7037/api/values/ChangeService";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteService(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/DeleteService/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на поиск услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра Service.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Service FindServiceById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/FindService/{id}";
            AddTokenHeaderMethod(httpContext);
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<Service>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Blog
        /// <summary>
        /// Запрос на получение записей в блоге, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа BlogModel.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BlogModel> GetBlog()
        {
            string url = @"https://localhost:7037/api/values/GetBlog";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject <IEnumerable<BlogModel>>(json);
        }

        /// <summary>
        /// Запрос на создание записи в блоге, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр записи блога и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="httpContext"></param>
        public void AddBlog(BlogModel blog, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/AddBlog";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на изменение записи блога, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра BlogModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра BlogModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="imageName"></param>
        /// <param name="description"></param>
        /// <param name="blogPost"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void ChangeBlog(string name, string imageName, string description,
            string blogPost, int id, HttpContext httpContext)
        {
            BlogModel blog = new BlogModel()
            {
                Id = id,
                Description = description,
                Name = name,
                ImageName = imageName,
                BlogPost = blogPost
            };

            string url = $"https://localhost:7037/api/values/ChangeBlog";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(blog), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление записи в блоге по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteBlog(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/DeleteBlog/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на поиск записи блога по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра BlogModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public BlogModel FindBlogById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/BlogDetails/{id}";
            AddTokenHeaderMethod(httpContext);
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<BlogModel>(json);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Запрос на поиск поста в блоге по указанному id, передающийся 
        /// на API сервер. Является второй перегрузкой метода, не принимающей
        /// httpcontext. Запрос возвращает результат в виде экземпляра BlogModel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BlogModel FindBlogById(int id)
        {
            string url = $"https://localhost:7037/api/values/BlogDetails/{id}";
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<BlogModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Contacts
        /// <summary>
        /// Запрос на получение контакта, передающийся на API 
        /// сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа Contacts.
        /// </summary>
        /// <returns></returns>
        public Contacts GetContacts()
        {
            string url = @"https://localhost:7037/api/values/GetContacts";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<Contacts>(json);
        }

        /// <summary>
        /// Запрос на изменение контактов, передающийся 
        /// на API сервер. Данный запрос принимает модель Contacts,
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Contacts. Данный метод является невозвратным.
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="httpContext"></param>
        public void ChangeContacts(Contacts contacts, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/ChangeContact";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(contacts), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }
        #endregion
        #region Links
        /// <summary>
        /// Запрос на получение всех ссылок, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа LinkModel.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LinkModel> GetLinks()
        {
            string url = @"https://localhost:7037/api/values/GetLinks";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<IEnumerable<LinkModel>>(json);
        }

        /// <summary>
        /// Запрос на создание новой ссылки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="httpContext"></param>
        public void AddLink(LinkModel link, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/AddLink";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(link), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на изменение ссылки, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра LinkModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра LinkModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="urlStr"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void ChangeLink(string imageName, string urlStr,
            int id, HttpContext httpContext)
        {
            LinkModel link = new LinkModel()
            {
                Id = id,
                ImageName = imageName,
                Url = urlStr
            };

            string url = $"https://localhost:7037/api/values/ChangeLink";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(link), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteLink(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/DeleteLink/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }

        /// <summary>
        /// Запрос на поиск ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра LinkModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public LinkModel FindLinkById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/LinkDetails/{id}";
            AddTokenHeaderMethod(httpContext);
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<LinkModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Title
        /// <summary>
        /// Запрос на получение экземпляра титульного листа, передающийся 
        /// на API сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа TitleModel.
        /// </summary>
        /// <returns></returns>
        public TitleModel GetTitle()
        {
            string url = @"https://localhost:7037/api/values/GetTitle";
            string json = httpClient.GetStringAsync(url).Result;
            return JsonConvert.DeserializeObject<TitleModel>(json);
        }

        /// <summary>
        /// Запрос на изменение титульного листа, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра TitleModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра TitleModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="editModel"></param>
        /// <param name="httpContext"></param>
        public void ChangeTitle(TitleModel editModel, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/ChangeTitle";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(editModel), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }
        #endregion
        #region Tag
        /// <summary>
        /// Запрос на получение тэгов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа TagModel.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public IEnumerable<TagModel> GetTags()
        {
            string url = @"https://localhost:7037/api/values/GetTags";
            try
            {
                string json = httpClient.GetStringAsync(url).Result;
                return JsonConvert.DeserializeObject<IEnumerable<TagModel>>(json);
            }
            catch
            {
                return new List<TagModel>();
            }
        }

        /// <summary>
        /// Запрос на создание нового тэга, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="httpContext"></param>
        public void AddTagMethod(TagModel tag, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values/AddTag";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(tag), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
        }

        /// <summary>
        /// Запрос на удаление тэга по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public void DeleteTag(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/DeleteTag/{id}";
            AddTokenHeaderMethod(httpContext);
            var r = httpClient.DeleteAsync(
                requestUri: url);
        }
        #endregion
        #region Authentication
        /// <summary>
        /// Запрос на регистрацию, передающийся в API сервер.
        /// Данный запрос принимает модель регистрации и возвращает
        /// результат запроса в виде строки, содержащей токен.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string IsRegister(UserRegistration model)
        {
            string url = $"https://localhost:7037/api/values/Registration/";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;
            string token = "";
            if (r.IsSuccessStatusCode)
            {
                string json = r.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(json);
                string tokenAuth = tokenResponse.access_token;
                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
        }

        /// <summary>
        /// Запрос на вход пользователя, передающийся в API
        /// Данный запрос принимает модель UserLoginProp
        /// и возвращает строковую переменную с ответом,
        /// который будет включать в себя токен при удачном
        /// входе или пустую строку при неудачном входе.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string IsLogin(UserLoginProp model)
        {
            string url = $"https://localhost:7037/api/values/Authenticate/";

            var r = httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                mediaType: "application/json")
                ).Result;

            string token = "";
            if (r.IsSuccessStatusCode)
            {
                string json = r.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(json);
                string tokenAuth = tokenResponse.access_token;
                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
        }

        /// <summary>
        /// Запрос для проверки валидности токена. Запрос пытается
        /// обратиться к API и если нет ошибки в блоке try (запрос 
        /// удался), то токен валидный. Если возникает ошибка, то 
        /// происходит переход в catch (токен не валидный).
        /// </summary>
        /// <returns></returns>
        public bool CheckToken(HttpContext httpContext)
        {
            string response = string.Empty;
            string url = @"https://localhost:7037/api/values/CheckToken";
            try
            {
                AddTokenHeaderMethod(httpContext);
                response = httpClient.GetStringAsync(url).Result;
                if (response == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Метод добавления токена в заголовок запроса. Данный метод
        /// принимает в себя текущий HTTP-контекст (то есть текущий
        /// запрос), что в свою очередь позволяет обратиться к методу
        /// Request для вызова токена, сохраненного в куки.
        /// В методе происходит запись токена из куки и дальнейшее
        /// добавление токена в заголовок запроса с помощью 
        /// создания нового экземпляра заголовка аутентификации
        /// AuthenticationHeaderValue.
        /// </summary>
        /// <param name="httpContext"></param>
        private void AddTokenHeaderMethod(HttpContext httpContext)
        {
            string tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                Console.WriteLine($"{tokenValue}");
            }
        }
        #endregion
    }
}
