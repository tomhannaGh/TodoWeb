namespace WebAppMVC.Models
{
    public class Item
    {

        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsComplete { get; set; } = false;
        public  required Priority Priority { get; set; } = Priority.Medium;
    }
}
