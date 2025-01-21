using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace UseCase.Repository
{
    public interface ITodoItemRepository
    {
        void Add(TodoItem item);
        void Remove(int id);
        void Update(TodoItem item);
        TodoItem? FindById(int id);
        IEnumerable<TodoItem>? Find(TodoItemCreterias item);
        IEnumerable<TodoItem> GetAllTodoItem();
        void DeleteAll();
    }
}
