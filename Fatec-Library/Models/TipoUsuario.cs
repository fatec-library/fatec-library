using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fatec_Library.Models
{
    public class TipoUsuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Nome")]
        public string? Nome { get; set; }

    }
}
