// File: HotelSearch.Domain/Interfaces/IHotelRepository.cs
using HotelSearch.Domain.Entities;

namespace HotelSearch.Domain.Interfaces;

public interface IHotelRepository
{
    Task<IEnumerable<Hotel>> GetAllAsync();
    Task<Hotel?> GetByIdAsync(Guid id);
    Task AddAsync(Hotel hotel);
    Task UpdateAsync(Hotel hotel);
    Task DeleteAsync(Guid id);
}