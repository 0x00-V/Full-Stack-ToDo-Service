using System.ComponentModel.DataAnnotations;

namespace todolist.Models
{
    public class ToDoTask
    {
        [Required]
        public string Title {get; set;} = default!;
        [Required]
        public string Description {get; set;} = default!;
    }
}
