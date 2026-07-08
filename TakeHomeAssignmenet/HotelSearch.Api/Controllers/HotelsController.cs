// File: HotelSearch.Api/Controllers/HotelsController.cs
using HotelSearch.Application.DTOs;
using HotelSearch.Domain.Entities;
using HotelSearch.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using HotelSearch.Application.Utils;

namespace HotelSearch.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelRepository _repository;

    public HotelsController(IHotelRepository repository)
    {
        _repository = repository;
    }

    // READ ALL: GET /api/hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelResponseDto>>> GetAll()
    {
        var hotels = await _repository.GetAllAsync();
        var response = hotels.Select(h => new HotelResponseDto(h.Id, h.Name, h.Price, h.Latitude, h.Longitude));
        return Ok(response);
    }

    // READ ONE: GET /api/hotels/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelResponseDto>> GetById(Guid id)
    {
        var hotel = await _repository.GetByIdAsync(id);
        if (hotel == null) return NotFound("Hotel not found.");

        return Ok(new HotelResponseDto(hotel.Id, hotel.Name, hotel.Price, hotel.Latitude, hotel.Longitude));
    }

    // CREATE: POST /api/hotels
    [HttpPost]
    public async Task<ActionResult<HotelResponseDto>> Create([FromBody] CreateUpdateHotelDto request)
    {
        var newHotel = new Hotel(request.Name, request.Price, request.Latitude, request.Longitude);
        
        await _repository.AddAsync(newHotel);

        var response = new HotelResponseDto(newHotel.Id, newHotel.Name, newHotel.Price, newHotel.Latitude, newHotel.Longitude);
        
        // Returns a 201 Created status code, standard REST practice
        return CreatedAtAction(nameof(GetById), new { id = newHotel.Id }, response); 
    }

    // UPDATE: PUT /api/hotels/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateUpdateHotelDto request)
    {
        var existingHotel = await _repository.GetByIdAsync(id);
        if (existingHotel == null) return NotFound("Hotel not found.");

        existingHotel.Update(request.Name, request.Price, request.Latitude, request.Longitude);
        await _repository.UpdateAsync(existingHotel);

        return NoContent(); // 204 No Content is standard for successful updates
    }

    // DELETE: DELETE /api/hotels/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingHotel = await _repository.GetByIdAsync(id);
        if (existingHotel == null) return NotFound("Hotel not found.");

        await _repository.DeleteAsync(id);
        return NoContent();
    }

    // SEARCH: POST /api/hotels/search
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<SearchResultDto>>> Search([FromBody] SearchRequestDto request)
    {
        // 1. EXTRACT DATA FROM PROMPT
        var promptLower = request.Prompt.ToLower();
        
        // Default to center of Croatia if city not found
        double userLat = 45.1; 
        double userLon = 15.2; 
        
        if (promptLower.Contains("zagreb")) { userLat = 45.8153; userLon = 15.9665; }
        else if (promptLower.Contains("split")) { userLat = 43.5081; userLon = 16.4401; }
        else if (promptLower.Contains("dubrovnik")) { userLat = 42.6506; userLon = 18.0944; }
        else if (promptLower.Contains("zadar")) { userLat = 44.1193; userLon = 15.2313; }

        // Extract budget (Smarter NLP Mock)
        decimal budget = 9999m; // Default high budget

        // Rule 1: Look for a number near currency words (e.g., "200 euro", "150eur")
        var currencyMatch = System.Text.RegularExpressions.Regex.Match(
            request.Prompt, 
            @"(\d+)\s*(euro|eur|€|\$|hrk)", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        if (currencyMatch.Success)
        {
            budget = decimal.Parse(currencyMatch.Groups[1].Value);
        }
        else
        {
            // Rule 2 (Fallback): Find all numbers in the text and assume the largest one is the budget 
            // (e.g., "2 adults 5 stars 300 budget" -> picks 300)
            var allNumbers = System.Text.RegularExpressions.Regex.Matches(request.Prompt, @"\d+");
            if (allNumbers.Count > 0)
            {
                budget = allNumbers.Max(m => decimal.Parse(m.Value));
            }
        }

        // 2. FETCH ALL HOTELS
        var allHotels = await _repository.GetAllAsync();

        // 3. FILTER & CALCULATE DISTANCE
        var validHotels = allHotels
            .Where(h => h.Price <= budget) // Filter by budget
            .Select(h => new
            {
                Hotel = h,
                Distance = GeoCalculator.GetDistanceInKm(userLat, userLon, h.Latitude, h.Longitude)
            });

        // 4. SORT (THIS IS WHERE THE ORDER BY IS!)
        // Cheaper and closer to the top. We normalize distance and price to a "Score".
        var sortedHotels = validHotels
            .OrderBy(x => x.Distance + ((double)x.Hotel.Price / 10.0)) 
            .ToList();

        // 5. PAGING (BONUS POINTS)
        var pagedItems = sortedHotels
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SearchResultDto(x.Hotel.Name, x.Hotel.Price, Math.Round(x.Distance, 2)))
            .ToList();

        // RETURN JUST THE LIST DIRECTLY!
        return Ok(pagedItems);
    }
}