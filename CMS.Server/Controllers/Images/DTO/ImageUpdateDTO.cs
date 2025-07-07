using Microsoft.AspNetCore.Mvc;

public class ImageUpdateDTO
{
    [FromForm(Name = "id")]
    public int Id { get; set; }

    [FromForm(Name = "clothId")]
    public int ClothId { get; set; }

    [FromForm(Name = "colorId")]
    public int ColorId { get; set; }

    // URL should be set by the server, not submitted from the client
    public string URL { get; set; }
}