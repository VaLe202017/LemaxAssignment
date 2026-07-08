// File: HotelSearch.Application/Utils/GeoCalculator.cs
namespace HotelSearch.Application.Utils;

public static class GeoCalculator
{
    // Uses the Haversine formula to calculate the exact distance between two coordinates on Earth
    public static double GetDistanceInKm(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = 
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * 
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2); 
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
        return R * c;
    }

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;
}