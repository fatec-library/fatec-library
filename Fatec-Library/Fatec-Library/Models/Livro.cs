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
        public List<Autor> Autores { get; set; } = new List<Autor> { };

        [BsonElement("editora")]
        public string? Editora { get; set; }

        [BsonElement("descricao")]
        public string? Descricao { get; set; }

        [BsonElement("ano_pulicacao")]
        public int Ano_Publicacao { get; set; }

        [BsonElement("isbn")]
        public string? Isbn { get; set; }

        [BsonElement("Idioma")]
        public string? Idioma { get; set; }

        [BsonElement("num_paginas")]
        public int Num_Paginas { get; set; }

        [BsonElement("capa_livro")]
        public string? Capa_Livro { get; set; }

        [BsonElement("codigo_exemplar")]
        public List<int> Codigo_Exemplar { get; set; } = new List<int> { };

        public List<Exemplare> Exemplares { get; set; } = new List<Exemplare> { };

        public class Autor
        {
            public Autor(string nome_Autor)
            {
                Nome_Autor = nome_Autor;
            }

            public string Nome_Autor { get; set; }

        }
    }
}
