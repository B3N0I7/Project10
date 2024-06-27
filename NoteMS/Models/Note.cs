using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace NoteMS.Models
{
    public class Note
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("Patient")]
        [JsonPropertyName("Patient")]
        // Add patId
        public string PatientName { get; set; }
        public string PatientNote { get; set; }
        public int? PatientId { get; set; }
    }
}
