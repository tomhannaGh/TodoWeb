
namespace WebAppMVC.Models
{
    public class TodoItemList
    {
        public required IEnumerable<ItemModel> Items { get; init; }
    }
}
