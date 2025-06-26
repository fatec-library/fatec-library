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

        [Display(Name = "RA do Aluno")]
        [BsonElement("ra")]
        public string? Ra { get; set; }

        [Display(Name = "CPF")]
        [BsonElement("cpf")]
        public string? Cpf { get; set; }

        [Display(Name = "RG")]
        [BsonElement("rg")]
        public string? Rg { get; set; }

        [Display(Name = "Data de Nascimento")]
        [BsonElement("data_nascimento")]
        public DateTime DataNascimento { get; set; }

        [Display(Name = "Telefone")]
        [BsonElement("telefone")]
        public List<string> Telefones { get; set; } = new List<string>();

        [Display(Name = "Endereço")]
        [BsonElement("endereco")]
        public Endereco? Endereco { get; set; }


        [Required(ErrorMessage = "E-mail é Um Campo Obrigatorio")]
        [Display(Name = "E-mail")]
        [BsonElement("email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Senha é Um Campo Obrigatorio")]
        [BsonElement("senha")]
        public string? Senha { get; set; }

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("tipo_id")]
        public string? TipoId { get; set; }

        [BsonIgnore]
        public TipoUsuario? Tipo { get; set; }

    }
    public class Endereco
    {
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Cep { get; set; }

    }

}
