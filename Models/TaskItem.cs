using System.ComponentModel.DataAnnotations;

namespace ToDoMVC.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(100, ErrorMessage = "Task name cannot be longer than 100 characters.")]
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }
}
