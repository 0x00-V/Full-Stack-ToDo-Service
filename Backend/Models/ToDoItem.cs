namespace api.Models
{
    public class ToDoItem
    {
        public int ItemId {get;set;}
        public required int UserId {get;set;}
        public required string Title {get;set;}
        public string Description {get;set;} = default!;
        public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
        //public bool Completed {get;set;} = false;

    }

    public class CreateToDoItem
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
}