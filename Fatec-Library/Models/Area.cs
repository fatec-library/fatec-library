using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Area
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("descritivo")]
        public string? Descritivo { get; set; }
        
    }
}
