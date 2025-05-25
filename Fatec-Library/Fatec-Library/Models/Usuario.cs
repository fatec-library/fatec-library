using Microsoft.Build.Framework;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace Fatec_Library.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Nome é Um Campo Obrigatorio")]
        [BsonElement("nome")]
        public string? Nome { get; set; }

        [BsonElement("ra")]
        [Display(Name = "RA do Aluno")]
        public string? Ra { get; set; }


        [Required(ErrorMessage = "E-mail é Um Campo Obrigatorio")]
        [BsonElement("email")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Senha é Um Campo Obrigatorio")]
        [BsonElement("senha_hash")]
        public string? Senha { get; set; }

        [Required]
        [BsonElement("tipo")]
        public string? Tipo { get; set; }

    }
}
