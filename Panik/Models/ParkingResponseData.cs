using System.Text.Json.Serialization;

namespace Panik.Models
{
    public class ParkingResponseData
    {
        [JsonPropertyName("features")]
        public List<FeaturesData> Features { get; set; }
    }

    public class FeaturesData
    {
        [JsonPropertyName("properties")]
        public PropertiesData Properties { get; set; }
    }

    public class PropertiesData
    {
        [JsonPropertyName("PARKING_RATE")]
        public string ParkingRate { get; set; }
        [JsonPropertyName("OTHER_INFO")]
        public string OtherInfo { get; set; }
        [JsonPropertyName("ADDRESS")]
        public string Address { get; set; }
    }
}
