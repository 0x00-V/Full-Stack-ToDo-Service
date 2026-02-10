using System.ComponentModel.DataAnnotations;

namespace todolist.Models
{
    public class Credential
    {
        [Required]
        public string Username {get; set;} = default!;
        [Required]
        [DataType(DataType.Password)]
        public string Password {get; set;} = default!;
    }
}
