namespace MyAPI.Models;

public class User
{
    public int Id { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string Role { get; set; } = "User";

    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public ICollection<BusLine> CreatedBusLines { get; set; } = new List<BusLine>();

    public ICollection<BusLine> UpdatedBusLines { get; set; } = new List<BusLine>();

}
