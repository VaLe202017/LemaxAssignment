// File: HotelSearch.Domain/Entities/Hotel.cs
namespace HotelSearch.Domain.Entities;

public class Hotel
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    // Constructor ensures a hotel cannot be created without required data
    public Hotel(string name, decimal price, double latitude, double longitude)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        Latitude = latitude;
        Longitude = longitude;
    }

    // Method to allow updates later for our CRUD interface
    public void Update(string name, decimal price, double latitude, double longitude)
    {
        Name = name;
        Price = price;
        Latitude = latitude;
        Longitude = longitude;
    }
}