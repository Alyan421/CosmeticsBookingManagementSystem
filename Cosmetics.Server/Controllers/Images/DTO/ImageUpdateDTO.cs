using Microsoft.AspNetCore.Mvc;

public class ImageUpdateDTO
{
    [FromForm(Name = "id")]
    public int Id { get; set; }

    [FromForm(Name = "brandId")]
    public int BrandId { get; set; }

    [FromForm(Name = "categoryId")]
    public int CategoryId { get; set; }

    // URL should be set by the server, not submitted from the client
    public string URL { get; set; }
}