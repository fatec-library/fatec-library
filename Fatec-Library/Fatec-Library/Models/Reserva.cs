using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Reserva
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("data_reserva")]
        public DateTime Data_Reserva { get; set; }


        [BsonElement("livro_id")]
        public string? Livro_Id { get; set; }

        [BsonElement("usuario_id")]
        public string? Usuario_Id { get; set; }

        public Usuario? Usuario { get; set; }

        public Livro? Livro { get; set; }
    }
}
