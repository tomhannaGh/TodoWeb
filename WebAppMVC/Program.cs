using Infrastracture.Repository.InMemoryRepository;
using Infrastracture.Repository.SqlServer;
using Microsoft.Data.SqlClient;
using UseCase.Repository;

namespace WebAppMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<ITodoItemRepository>(
                server =>
                {
                    var conn = builder.Configuration.GetConnectionString("SqlServerConnection")?? string.Empty;
                    if (!string.IsNullOrEmpty(conn))
                    {
                        var sqlConnection = new SqlConnection(conn);
						sqlConnection.Open();
						return new SqlServerTodoItemListRepository(sqlConnection);
					}
                    else
                        return new InMemoryTodoItemListRepository();
                });
            builder.Services.AddTransient<TodoItemManager>();
            //builder.WebHost.ConfigureKestrel(option => option.ListenAnyIP(5255));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
