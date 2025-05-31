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
        public DateTime Data_Retirada { get; set; }

        [BsonElement("data_devolucao")]
        public DateTime Data_Devolucao { get; set; }

        [BsonElement("status_emprestimo")]
        public string? Status_Emprestimo { get; set; }

        [BsonElement("codigo_exemplar")]
        public int Codigo_Exemplar { get; set; }

        [BsonElement("usuario_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Usuario_Id { get; set; }

        [BsonElement("livro_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Livro_Id { get; set; }

        public Usuario? Usuario { get; set; }

        public Livro? Livro { get; set; }


        // Método para realizar o empréstimo
        public void FazerEmprestimo(Livro livro)
        {
            if (livro.Status == "Disponível")
            {
                Data_Retirada = DateTime.Now;
                Data_Devolucao = Data_Retirada.AddDays(7); // Ex: 7 dias de prazo
                Status_Emprestimo = "Ativo";
                livro.MudarStatus(); // muda status para "Indisponível"
            }
            else
            {
                throw new InvalidOperationException("Livro indisponível para empréstimo.");
            }
        }

        // Método para devolução
        public void DevolverLivro(Livro livro)
        {
            if (Status_Emprestimo == "Ativo")
            {
                Status_Emprestimo = "Finalizado";
                livro.MudarStatus(); // muda status para "Disponível"
                Data_Devolucao = DateTime.Now;
            }
            else
            {
                throw new InvalidOperationException("Este empréstimo já foi finalizado.");
            }
        }
    }
}
