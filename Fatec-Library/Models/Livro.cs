using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Fatec_Library.Models
{
    public class Livro
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Display(Name = "Titulo")]
        [BsonElement("titulo")]
        public string? Titulo { get; set; }

        [Display(Name = "Autores")]
        [BsonElement("autor")]
        public List<string> Autores { get; set; } = new List<string> { };

        [Display(Name = "Editora")]
        [BsonElement("editora")]
        public string? Editora { get; set; }

        [Display(Name = "Descrição")]
        [BsonElement("descricao")]
        public string? Descricao { get; set; }

        [Display(Name = "Ano de Publicação")]
        [BsonElement("ano_publicacao")]
        public int Ano_Publicacao { get; set; }

        // identificadores e classficações

        [Display(Name = "ISBN")]
        [BsonElement("isbn")]
        public string? Isbn { get; set; }

        [Display(Name = "CDD")]
        [BsonElement("cdd")] //codigo cdd
        public string? Cdd { get; set; }

        [Display(Name = "CDU")]
        [BsonElement("cdu")] //codigo de classificação cdu
        public int? Cdu { get; set; }

        [Display(Name = "Tabela PHA")]
        [BsonElement("pha")]
        public string? Pha { get; set; }

        // ----

        [Display(Name = "Idioma")]
        [BsonElement("idioma")]
        public string? Idioma { get; set; }

        [Display(Name = "Números de Páginas")]
        [BsonElement("num_pagina")]
        public int Num_Paginas { get; set; }

        [Display(Name = "Imagem de capa")]
        [BsonElement("capa_livro")]
        public string? Capa_Livro { get; set; }

        [Display(Name = "Área")]
        [BsonElement("area")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? AreaId { get; set; }

        [Display(Name = "Codigo Exemplar")]
        [BsonElement("codigo_exemplar")]
        public List<int> codigo_exemplar { get; set; } = new List<int> { };

        [Display(Name = "Cursos")]
        [BsonElement("Curso")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string?> Cursos { get; set; } = new List<string?> { };

        [BsonIgnore]
        public List<Exemplar> Codigo_Exemplar { get; set; } = new List<Exemplar> { };

        [BsonIgnore]
        public Area? Area { get; set; }

    }
}
