using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Livro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? Titulo { get; set; }

        public List<string> Autores { get; set; } = new List<string> { };

        public string? Editora { get; set; }

        public string? Descricao { get; set; }

        public int Ano_Publicacao { get; set; }

        // identificadores e classficações
        public string? Isbn { get; set; }

        public string? Cdd { get; set; }

        public string? Tha { get; set; }

        // ----
        public string? Idioma { get; set; }

        public int Num_Paginas { get; set; }

        public string? Capa_Livro { get; set; }

        public Area? Area_Conhecimento { get; set; }

        public List<Exemplar> Codigo_Exemplar { get; set; } = new List<Exemplar> { };

        public List<Curso> Cursos { get; set; } = new List<Curso> { };

        [BsonElement("status")]
        public string Status { get; set; } = "Disponível"; // Padrão é "Disponível"

        public void MudarStatus()
        {
            Status = (Status == "Disponível") ? "Indisponível" : "Disponível";
        }

    }
}
