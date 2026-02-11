namespace dto
{
    public class ToDoItem
    {
        public int ItemId {get; set;} = 0;
        public int UserId {get; set;} = 0;
        public string Title {get; set;} = "";
        public string Description {get; set;} = "";
        public DateTime CreatedAt {get; set;} = default!;

    }
}