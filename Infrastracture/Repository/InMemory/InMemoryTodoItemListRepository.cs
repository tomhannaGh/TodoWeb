using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using UseCase;
using UseCase.Repository;

namespace Infrastracture.Repository.InMemoryRepository
{
    public class InMemoryTodoItemListRepository : ITodoItemRepository
    {
        private List<TodoItem> Items { get; set; } = [];
        private int nextId = 0;

		public void Add(TodoItem item)
        {
            item.Id = nextId++;
			Items.Add(item);
        }

        public void DeleteAll()
        {
            Items.Clear();
        }

        public IEnumerable<TodoItem>? Find(TodoItemCreterias item)
        {
            var list = from o in Items select o;
            if (item.Priority.Equals(Priority.Medium))
                list = list.Where(x => x.Priority != Priority.Low);
            else if(item.Priority.Equals(Priority.Hight))
                list = list.Where(x => x.Priority != Priority.Medium && x.Priority != Priority.Low);
            if (list.Any())
                list = list.Where(x => x.Description.Contains(item.Description,StringComparison.OrdinalIgnoreCase));
            if(list.Any())
                list = list.Where(x=>x.Title.Contains(item.Title,StringComparison.OrdinalIgnoreCase));
            if (list.Any())
                list = list.Where(x => x.IsComplete.Equals(item.IsComplete));
            return list;
        }
        public TodoItem? FindById(int id)
        {
            return Items.Where(x=>x.Id.Equals(id)).First();
        }

        public IEnumerable<TodoItem> GetAllTodoItem()
        {
            return Items;
        }

        public void Remove(int id)
        {
            Items.Remove(FindById(id)!);
        }

        public void Update(TodoItem item)
        {
            var index = Items.FindIndex(x => x.Id.Equals(item.Id));
            Items[index] = item;
		}
    }
}
