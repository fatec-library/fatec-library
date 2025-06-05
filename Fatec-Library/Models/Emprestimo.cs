using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Fatec_Library.Models
{
    public class Emprestimo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Display(Name = "RA do Aluno")]
        [BsonElement("ra_aluno")]
        public string? Ra_Aluno { get; set; }

        [Display(Name = "Nome do Aluno")]
        [BsonElement("nome_aluno")]
        public string? Nome_Aluno { get; set; }

        [Display(Name = "Data de Emprestimo")]
        [BsonElement("data_retirada")]
        public DateTime Data_Retirada { get; set; } = DateTime.Now;

        [Display(Name = "Data para Devolução")]
        [BsonElement("data_devolucao")]
        public DateTime Data_Devolucao { get; set; } = DateTime.Now.AddDays(7);

        [Display(Name = "Status")]
        [BsonElement("status_emprestimo")]
        public string? Status_Emprestimo { get; set; } = "Emprestado";

        [Display(Name = "Número Exemplar")]
        [BsonElement("codigo_exemplar")]
        public int Codigo_Exemplar { get; set; }

        [BsonElement("usuario_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Usuario_Id { get; set; }

        [BsonElement("livro_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Livro_Id { get; set; }

        [BsonIgnore]
        public Usuario? Usuario { get; set; }

        [BsonIgnore]
        public Livro? Livro { get; set; }

    }
}
