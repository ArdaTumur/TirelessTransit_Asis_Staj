using System.ComponentModel.DataAnnotations;

namespace MyAPI.DTOs;

public class UpdateBusLineDto
{
    [Required]
    [RegularExpression(@"^[A-Za-zşçğüıöÖÇŞİĞÜ]+(?:[-'\s][A-Za-zşçğüıöÖÇŞİĞÜ]+)*$",
        ErrorMessage = "Invalid driver name format.")]
    public string Driver { get; set; } = "";

    [Required]
    [RegularExpression(@"^(0[1-9]|[1-7][0-9]|8[01])\s*(([A-Z])\s*(\d{4,5})|([A-Z]{2})\s*(\d{3,4})|([A-Z]{3})\s*(\d{2,3}))$",
        ErrorMessage = "Invalid plate format. Example: '34 AB 1234' or '34 ABC 123'.")]
    public string Plate { get; set; } = "";

    [Required]
    [RegularExpression(@"^([01]\d|2[0-3]):([0-5]\d)\s*-\s*([01]\d|2[0-3]):([0-5]\d)$",
        ErrorMessage = "Invalid hours format. Use HH:MM - HH:MM.")]
    public string Hours { get; set; } = "";

    [Required]
    [RegularExpression(@"^(Scheduled|En Route|Delayed|Cancelled)$",
        ErrorMessage = "Invalid route status. Use Scheduled, En Route, Delayed, or Cancelled.")]
    public string RouteStatus { get; set; } = "Scheduled";

    [Required]
    [StringLength(100)]
    public string OriginCity { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string DestinationCity { get; set; } = "";
}
