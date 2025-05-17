using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Emprestimo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("ra_aluno")]
        public string? Ra_Aluno { get; set; }

        [BsonElement("nome_aluno")]
        public string? Nome_Aluno { get; set; }

        [BsonElement("data_retirada")]
        public string? Data_Retirada { get; set; }

        [BsonElement("data_devolucao")]
        public string? Data_Devolucao { get; set; }

        [BsonElement("status_emprestimo")]
        public string? Status_Emprestimo { get; set; }

        [BsonElement("usuario_id")]
        public string? Usuario_Id { get; set; }

        [BsonElement("livro_id")]
        public string? Livro_Id { get; set; }
        public Usuario? Usuario { get; set; }

        public Livro? Livro { get; set; }
    }
}
