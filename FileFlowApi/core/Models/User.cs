namespace FileFlowApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // שמירת הסיסמה בצורה מאובטחת

        public List<Category> Categories { get; set; } = new List<Category>(); // קשר לקטגוריות
        public string Role { get; set; } = "User";
    }
}
