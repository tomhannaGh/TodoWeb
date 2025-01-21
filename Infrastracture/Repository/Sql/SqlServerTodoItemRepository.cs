
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

namespace Infrastracture.Repository.SqlServer
{
    internal class SqlServerTodoItemRepository: ITodoItemRepository
    {
        private readonly string AddQurery = "INSERT INTO TODOITEM ( id, title, desciption, isComplete, priority) VALUES (@id, @title, @des, @complete)";
        private readonly string ClearQuery = "DELETE from TODOITEM";
        private readonly string FindByIdQuery = "SELECT * FROM TODOITEM WHERE ID = @ID";
        private readonly string DeleteQuery = "Delete rom todoitem where id = @id";
        private readonly string UpdateQuery = "update todoitem set title = @title, desciption = @des, isComplete = @isComplete, priority = @priority where id = @id";
        private readonly string SELECT = "select ";
        private readonly string FindAll = "id, title, description, isComplete, priority where (1=1)";
        private readonly string RemoveQuery = "DElete from TODOITEM where id=@id";
        private readonly SqlConnection conn;
        private readonly SqlTransaction? transaction;

        public SqlServerTodoItemRepository(SqlConnection conn, SqlTransaction transaction)
        {
            this.conn = conn ?? throw new ArgumentNullException(nameof(conn));
            this.transaction = transaction;
        }

        public void Add(TodoItem item)
        {
            if(transaction != null)
            {
                var cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = AddQurery;
                cmd.Parameters.Add("@id",SqlDbType.Int).Value = item.Id;
                cmd.Parameters.Add("@title",SqlDbType.NVarChar).Value = item.Title;
                cmd.Parameters.Add("@des",SqlDbType.NVarChar).Value = item.Description;
                cmd.Parameters.Add("@complete",SqlDbType.Bit).Value = item.IsComplete;
                cmd.Parameters.Add("@priority",SqlDbType.NVarChar,6).Value = item.Priority.ToString();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAll()
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = ClearQuery;
            if(transaction != null)
                cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();
        }
        public void Remove(int id)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = RemoveQuery;
            if (transaction != null)
                cmd.Transaction = transaction;
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value = id;
            cmd.ExecuteNonQuery();
        }
        public IEnumerable<TodoItem>? Find(TodoItemCreterias item)
        {
            var cmd = conn.CreateCommand();
            if (transaction != null)
                cmd.Transaction = transaction;
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
            if(transaction != null)
                cmd.Transaction= transaction;
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
                        Priority = (reader.GetString(4).Equals("Low") ? Priority.Low
                                                                    : reader.GetBoolean(4).Equals("Hight") ? 
                                                                    Priority.Hight : Priority.Medium)
                    };
                }
            }
            return null;
        }

        public IEnumerable<TodoItem> GetAllTodoItem()
        {
            throw new NotImplementedException();
        }

        public void Remove(TodoItem item)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = DeleteQuery;
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Value= item.Id;
            if (transaction != null) cmd.Transaction= transaction;
            cmd.ExecuteNonQuery();
        }

        public void Update(TodoItem item)
        {
            var cmd = conn.CreateCommand();
            if(transaction != null)
                cmd.Transaction= transaction;
            cmd.CommandText = UpdateQuery;
            cmd.Parameters.Add(new SqlParameter("@title", SqlDbType.NVarChar)).Value= item.Title;
            cmd.Parameters.Add(new SqlParameter("@des", SqlDbType.NVarChar)).Value = item.Description;
            cmd.Parameters.Add(new SqlParameter("@isComplete", SqlDbType.Bit)).Value= item.IsComplete;
            cmd.Parameters.Add(new SqlParameter("@priority", SqlDbType.NVarChar)).Value= item.Priority.ToString();
            cmd.ExecuteNonQuery();
        }
    }
}
