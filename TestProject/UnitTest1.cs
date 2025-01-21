using Infrastracture.Repository.InMemoryRepository;
using Entities;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public void InMemory()
        {
            var mem = new InMemoryTodoItemListRepository();
            mem.Add(new TodoItem()
            {
                Id = 1,
                Title = "Test",
                Description = "Test",
                Priority = Priority.Hight,
            });
            var item2 = mem.FindById(1);
            //mem.Remove(item2!);

            var item = mem.FindById(1);

            Assert.NotNull(item);
            Assert.Equal("Test", item.Title);
            Assert.Equal("Test", item.Description);
            Assert.True(item.Priority == Priority.Hight);
            Assert.True(item.Id == 1);
        }
    }
}