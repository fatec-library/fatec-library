using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Exemplare
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("codigo_exemplar")]
        public int Codigo_Exemplar { get; set; }

        [BsonElement("stataus_exemplar")]
        public string? Status_Exemplar { get; set; }

        public Livro? Livro { get; set; }

    }
}
