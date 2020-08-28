using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TodoAPI.Entities
{
    public class TodoItem : ITimestamped
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public int TodoListId { get; set; }
        
        [JsonIgnore]
        public TodoList TodoList { get; set; }
        public List<TodoItemLabel> TodoItemLabels { get; set; } = new List<TodoItemLabel>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}