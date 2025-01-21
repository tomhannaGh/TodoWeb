namespace Entities
{
    public class TodoItem
    {
        public int Id { get; set; } = default;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty ;
        public bool IsComplete { get; set; } = false ;
        public Priority Priority { get; set; } = Priority.Medium;
    }
}
