// File: HotelSearch.Application/DTOs/HotelDto.cs
namespace HotelSearch.Application.DTOs;

// What the user sends us when creating/updating a hotel
public record CreateUpdateHotelDto(
    string Name, 
    decimal Price, 
    double Latitude, 
    double Longitude
);

// What we send back to the user
public record HotelResponseDto(
    Guid Id, 
    string Name, 
    decimal Price, 
    double Latitude, 
    double Longitude
);

// What the user types into the search
public record SearchRequestDto(string Prompt, int Page = 1, int PageSize = 5);

// The exact output required by the PDF
public record SearchResultDto(string Name, decimal Price, double DistanceInKm);

// Wrapper for the bonus points "Paging"
public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int Page, int PageSize);