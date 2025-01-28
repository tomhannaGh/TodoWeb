using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Entities;
using UseCase.Repository;
using System.Transactions;

namespace Infrastracture.Repository.SqlServer
{
    public class SqlServerTodoItemListRepository: ITodoItemRepository
    {
        private readonly string AddQurery = "INSERT INTO TODOITEM ( title, description, isComplete, priority) VALUES ( @title, @description, @isComplete, @priority)";
        private readonly string ClearQuery = "DELETE from TODOITEM";
        private readonly string FindByIdQuery = "SELECT * FROM TODOITEM WHERE ID = @ID";
        private readonly string DeleteQuery = "Delete from todoitem where id = @id";
        private readonly string UpdateQuery = "update todoitem set title = @title, description = @des, isComplete = @isComplete, priority = @priority where id = @id";
        private readonly string SELECT = "select ";
        private readonly string FindAll = "id, title, description, isComplete, priority where (1=1)";
        private readonly string RemoveQuery = "DElete from TODOITEM where id=@id";
        private readonly string LoadingAllTodo = "Select * from TODOITEM";
        private readonly SqlConnection conn;

		public SqlServerTodoItemListRepository(SqlConnection conn)
        {
			this.conn = conn ?? throw new ArgumentNullException(nameof(conn));
        }

        public void Add(TodoItem item)
        {
			var cmd = conn.CreateCommand();
			cmd.CommandText = AddQurery;
			//cmd.Parameters.Add("@id",SqlDbType.Int).Value = item.Id;
			cmd.Parameters.Add("@title", SqlDbType.NVarChar, 50).Value = item.Title;
			cmd.Parameters.Add("@description", SqlDbType.NVarChar, 250).Value = item.Description ?? string.Empty;
			cmd.Parameters.Add("@isComplete", SqlDbType.Bit).Value = item.IsComplete;
			cmd.Parameters.Add("@priority", SqlDbType.Int).Value = (int)item.Priority;
			cmd.ExecuteNonQuery();
		}

        public void DeleteAll()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = ClearQuery;
            cmd.ExecuteNonQuery();
        }
        public void Remove(int id)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = RemoveQuery;
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;
            cmd.ExecuteNonQuery();
        }
        public IEnumerable<TodoItem>? Find(TodoItemCreterias item)
        {
            var cmd = conn.CreateCommand();
            var sql = new StringBuilder(SELECT);
            sql.Append(FindAll);
            if (item.Priority == Priority.Hight)
                sql.Append(" AND (priority = Hight)");
            else if (item.Priority == Priority.Medium)
                sql.Append(" AND (priority != Low)");

            if (item.IsComplete == false)
                sql.Append(" AND (isComplete = 0)");
            else
                sql.Append(" AND (isComplete = 1)");

            if (item.Title != string.Empty)
            {
                sql.Append(" AND (title Like @title)");
                cmd.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar)).Value = item.Title;
            }

            if (item.Description != string.Empty)
            {
                sql.Append(" AND (description Like @description)");
                cmd.Parameters.Add(new SqlParameter("@description", SqlDbType.NVarChar).Value = item.Description);
            }
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                var list = new List<TodoItem>();
                while (reader.Read())
                {
                    list.Add(new TodoItem()
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        IsComplete = reader.GetBoolean(3),
                        Priority = (reader.GetString(4).Equals("Low") ? Priority.Low
                                                                    : reader.GetBoolean(4).Equals("Hight") ?
                                                                    Priority.Hight : Priority.Medium)
                    });
                }
                return list;
            }
            return null;
        }

        public TodoItem? FindById(int id)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = FindByIdQuery;
            cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int)).Value = id;
            using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                if (reader.Read())
                {
                    return new TodoItem()
                    {
                        Id = id,
                        Title= reader.GetString(1),
                        Description = reader.GetString(2),
                        IsComplete = reader.GetBoolean(3),
                        Priority = reader.GetInt32(4) switch
                        {
                            1 => Priority.Hight,
                            2 => Priority.Medium,
                            3 => Priority.Low,
                            _ => Priority.Medium
                        }
					};
                }
            }
            return null;
        }

        public IEnumerable<TodoItem> GetAllTodoItem()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = LoadingAllTodo;
            List<TodoItem> list = [];
			using var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read() && reader!=null)
                {
                    list. Add(new TodoItem()
					{
						Id = reader.GetInt32(0),
						Title = reader.GetString(1),
						Description = reader.GetString(2),
                        IsComplete = reader.GetBoolean(3),
						Priority = reader.GetInt32(4) switch
						{
							1 => Priority.Hight,
							2 => Priority.Medium,
							3 => Priority.Low,
							_ => Priority.Medium
						}
					});
				}
            }
            //cmd.Transaction.Commit();
			return list;
		}

        public void Remove(TodoItem item)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = DeleteQuery;
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value= item.Id;
            cmd.ExecuteNonQuery();
        }

        public void Update(TodoItem item)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = UpdateQuery;
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = item.Id;
			cmd.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar, 50)).Value= item.Title;
            cmd.Parameters.Add(new SqlParameter("@des", SqlDbType.NVarChar, 250)).Value = item.Description;
            cmd.Parameters.Add(new SqlParameter("@isComplete", SqlDbType.Bit)).Value= item.IsComplete;
            cmd.Parameters.Add(new SqlParameter("@priority", SqlDbType.Int)).Value= (int)item.Priority;
            cmd.ExecuteNonQuery();
        }
    }
}
