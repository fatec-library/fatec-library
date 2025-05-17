using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Fatec_Library.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string? Id { get; set; }

        [BsonElement("nome")]

        public string? Nome { get; set; }

        [BsonElement("ra")]
        public string? Ra { get; set; }


        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("senha_hash")]
        public string? Senha { get; set; }

        [BsonElement("tipo")]
        public string? Tipo { get; set; }

    }
}
