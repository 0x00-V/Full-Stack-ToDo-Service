
namespace todolist.Models
{
  public class LoginResponse
{
    public string AccountName { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string TokenType { get; set; } = "";
    public int ExpiresIn { get; set; }
}  
}
