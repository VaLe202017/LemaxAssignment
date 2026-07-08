// File: HotelSearch.Infrastructure/Repositories/InMemoryHotelRepository.cs
using System.Collections.Concurrent;
using HotelSearch.Domain.Entities;
using HotelSearch.Domain.Interfaces;

namespace HotelSearch.Infrastructure.Repositories;

public class InMemoryHotelRepository : IHotelRepository
{
    // This dictionary IS our database. 
    private readonly ConcurrentDictionary<Guid, Hotel> _hotels = new();

    public InMemoryHotelRepository()
    {
        // SEED DATA: Pre-filling the "database" with Croatian hotels so we don't have to manually create them every time.
        var seedHotels = new List<Hotel>
        {
            new Hotel("Esplanade Zagreb", 150.00m, 45.8058, 15.9754),      // Zagreb
            new Hotel("Hotel Dubrovnik", 120.00m, 45.8130, 15.9775),        // Zagreb (center)
            new Hotel("Radisson Blu Split", 200.00m, 43.5048, 16.4673),     // Split
            new Hotel("Hotel Excelsior", 350.00m, 42.6410, 18.1152),        // Dubrovnik
            new Hotel("Cheap Hostel Zadar", 45.00m, 44.1194, 15.2314)       // Zadar
        };

        foreach (var hotel in seedHotels)
        {
            _hotels.TryAdd(hotel.Id, hotel);
        }
    }

    public Task<IEnumerable<Hotel>> GetAllAsync()
    {
        return Task.FromResult((IEnumerable<Hotel>)_hotels.Values.ToList());
    }

    public Task<Hotel?> GetByIdAsync(Guid id)
    {
        _hotels.TryGetValue(id, out var hotel);
        return Task.FromResult(hotel);
    }

    public Task AddAsync(Hotel hotel)
    {
        _hotels.TryAdd(hotel.Id, hotel);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Hotel hotel)
    {
        _hotels[hotel.Id] = hotel;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _hotels.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}