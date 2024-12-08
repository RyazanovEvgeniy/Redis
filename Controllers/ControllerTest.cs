using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace API.Controllers;

[Route("api/test")]
[ApiController]
public class ControllerTest : ControllerBase
{
    private readonly ILogger<ControllerTest> _logger;
    private int _count = 1;

    public ControllerTest(ILogger<ControllerTest> logger) : base()
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> Get()
    {   
        var data = await GetBooksFromSession();

        var book = new Book(_count, "dsa", 20.0 + _count);

        if (book is not null)
        {
            data.Add(book);

            var json = JsonSerializer.Serialize(data);

            //Local in-memory session
            HttpContext.Session.SetString("cart", json);

            // TempData["Success"] = "The book is added successfully";
        }

        return data;
    }

    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }

    private async Task<List<Book>> GetBooksFromSession()
    {
        await HttpContext.Session.LoadAsync();

        var sessionString = HttpContext.Session.GetString("cart");

        if (sessionString is not null)
        {
            return JsonSerializer.Deserialize<List<Book>>(sessionString);
        }

        return (Enumerable.Empty<Book>()).ToList();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }

        public Book(int id, string title, double price)
        {
            Id = id;
            Title = title;
            Price = price;
        }
    }
}