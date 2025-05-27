namespace FileFlowApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();
        public int UserId { get; set; }  // קשר למשתמש
        public User User { get; set; }
    }
}
