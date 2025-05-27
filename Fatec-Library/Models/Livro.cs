using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Fatec_Library.Models
{
    public class Livro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("titulo")]
        public string? Titulo { get; set; }

        [BsonElement("autor")]
        public List<string> Autores { get; set; } = new List<string> { };

        [BsonElement("editora")]
        public string? Editora { get; set; }

        [BsonElement("descricao")]
        public string? Descricao { get; set; }

        [BsonElement("ano_publicacao")]
        public int Ano_Publicacao { get; set; }

        [BsonElement("isbn")]
        public string? Isbn { get; set; }

        [BsonElement("idioma")]
        public string? Idioma { get; set; }

        [BsonElement("num_pagina")]
        public int Num_Paginas { get; set; }

        [BsonElement("capa_livro")]
        public string? Capa_Livro { get; set; }

        [BsonElement("area")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Area { get; set; }

        [BsonElement("codigo_exemplar")]
        public List<int> Codigo_Exemplar { get; set; } = new List<int> { };

    }
}
