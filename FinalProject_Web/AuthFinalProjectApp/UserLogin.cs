using System.ComponentModel.DataAnnotations;

namespace FinalProject_Web.AuthFinalProjectApp
{
    public class UserLogin
    {
        [Required, MaxLength(20)]
        public string? LoginProp { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
