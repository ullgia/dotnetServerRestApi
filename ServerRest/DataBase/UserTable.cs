using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerRest.DataBase;

[Table("UserTable")]
public class UserTable
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; } = "password";
    public string Email { get; set; } = "email@email.com";
    public string Token { get; set; } = "";
    public string Role { get; set; } = "S";
}