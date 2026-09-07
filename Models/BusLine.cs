namespace MyAPI.Models;

public class BusLine
{
    public int Id { get; set; }

    public bool Status { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int? CreateUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? UpdateUserId { get; set; }

    public User? UpdatedByUser { get; set; }

    public string Driver { get; set; } = "";

    public string Plate { get; set; } = "";

    public string Hours { get; set; } = "";

    public string RouteStatus { get; set; } = "Scheduled";

    public string OriginCity { get; set; } = "";

    public string DestinationCity { get; set; } = "";
}
