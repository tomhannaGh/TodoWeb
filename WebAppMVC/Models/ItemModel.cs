namespace WebAppMVC.Models
{
    public class ItemModel
    {

        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; } = string.Empty;
		public bool IsComplete { get; set; } = false;
        public  string Priority { get; set; } = "2";
    }
}
