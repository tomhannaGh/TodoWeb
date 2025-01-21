using Entities;

namespace UseCase.Repository
{
    public class TodoItemCreterias
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public Priority Priority { get; set; } = Priority.Low;
        public bool IsComplete { get; set; } = false;
        public static TodoItemCreterias Empty => new();
    }
}