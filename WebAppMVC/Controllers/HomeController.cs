using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly TodoItemManager todoItemManager;

		public HomeController(ILogger<HomeController> logger, TodoItemManager todoItemManager)
    {
        _logger = logger;
        this.todoItemManager = todoItemManager ?? throw new ArgumentNullException(nameof(todoItemManager));
    }
		public IActionResult Index()
    {
        var items = todoItemManager.GetAllTodoItem();
        var list = new TodoItemList()
        {
            Items = items.Select(i => new ItemModel()
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Priority = i.Priority.ToString(),
				IsComplete = i.IsComplete
            })
        };
        return View(list);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View("FormAdd");
    }

    [HttpPost]
    public IActionResult Create(ItemModel item)
    {
        todoItemManager.AddTodoItem(new TodoItem
        {
            Title = item.Title,
            Description = item.Description,
            Priority = item.Priority switch
            {
					"1" => Priority.Hight,
					"2" => Priority.Medium,
					"3" => Priority.Low,
					_ => Priority.Medium
				},
            IsComplete = item.IsComplete
        });
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(Dictionary<int,bool> checkboxs)
    {
        foreach (var checkbox in checkboxs)
        {
            todoItemManager.MarkComplete(checkbox.Key,checkbox.Value);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove( int id)
    {
        if (id < 0)
				return BadRequest("Invalid data.");
        else
        {
            todoItemManager.RemoveTodoItem(id);
            return RedirectToAction("Index");
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
