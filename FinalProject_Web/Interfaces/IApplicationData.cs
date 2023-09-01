using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Model;

namespace FinalProject_Web.Interfaces
{
    public interface IApplicationData
    {
        #region Applications
        IEnumerable<Application> GetApplications(HttpContext httpContext);
        void AddApplication(Application application, HttpContext httpContext);
        void DeleteApplication(int id, HttpContext httpContext);
        void ChangeApplicationStatus(string status, int id, string name, 
            string eMail, string message, DateTime date, HttpContext httpContext);
        #endregion
        #region Projects
        IEnumerable<ProjectModel> GetProjects();
        void AddProject(ProjectModel project, HttpContext httpContext);
        void ChangeProject(string name, string imageName, string description,
            int id, HttpContext httpContext);
        void DeleteProject(int id, HttpContext httpContext);
        ProjectModel FindProjectById(int id, HttpContext httpContext);
        ProjectModel FindProjectById(int id);
        #endregion
        #region Services
        IEnumerable<Service> GetServices();
        void AddService(Service service, HttpContext httpContext);
        void ChangeService(string name, string description,
            int id, HttpContext httpContext);
        void DeleteService(int id, HttpContext httpContext);
        Service FindServiceById(int id, HttpContext httpContext);
        #endregion
        #region Blog
        IEnumerable<BlogModel> GetBlog();
        void AddBlog(BlogModel blog, HttpContext httpContext);
        void ChangeBlog(string name, string imageName, string description,
            string blogPost, int id, HttpContext httpContext);
        void DeleteBlog(int id, HttpContext httpContext);
        BlogModel FindBlogById(int id, HttpContext httpContext);
        BlogModel FindBlogById(int id);
        #endregion
        #region Contacts
        Contacts GetContacts();
        void ChangeContacts(Contacts contacts, HttpContext httpContext);
        #endregion
        #region Links
        IEnumerable<LinkModel> GetLinks();
        void AddLink(LinkModel link, HttpContext httpContext);
        void ChangeLink(string imageName, string urlStr, int id, HttpContext httpContext);
        void DeleteLink(int id, HttpContext httpContext);
        LinkModel FindLinkById(int id, HttpContext httpContext);
        #endregion
        #region Tag
        void AddTagMethod(TagModel tag, HttpContext httpContext);
        IEnumerable<TagModel> GetTags();
        void DeleteTag(int id, HttpContext httpContext);
        #endregion
        #region Title
        TitleModel GetTitle();
        void ChangeTitle(TitleModel titleModel, HttpContext httpContext);
        #endregion
        #region Authentication
        string IsRegister(UserRegistration model);
        string IsLogin(UserLoginProp model);
        bool CheckToken(HttpContext httpContext);
        #endregion
    }
}
