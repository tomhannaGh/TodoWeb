using Entities;
using UseCase.Repository;

namespace UseCase
{
    public class TodoItemManager
    {
        private readonly ITodoItemRepository repository;
        public TodoItemManager(ITodoItemRepository repository)
        {
            this.repository = repository;
        }
        public IEnumerable<TodoItem> GetAllTodoItem()
        {
            return repository.GetAllTodoItem();
        }
        public void AddTodoItem(TodoItem item)
        {
            repository.Add(item);
        }
        public void RemoveTodoItem(int id)
        {
            repository.Remove(id);
        }

        public void MarkComplete(int id, bool isComplete)
        {
            var item = repository.FindById(id);
            if (item != null)
            {
                if (item.IsComplete != isComplete)
                {
                    item.IsComplete = isComplete;
                    repository.Update(item);
                }

            }
        }
    }
}
