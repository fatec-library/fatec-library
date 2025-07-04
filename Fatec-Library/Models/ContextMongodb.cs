﻿using MongoDB.Driver;
using System.Drawing;

namespace Fatec_Library.Models
{
    
    public class ContextMongodb
    {
        public static string? ConnectionString { get; set; }
        public static string? Database { get; set; }
        public static bool IsSSL { get; set; }
        private IMongoDatabase _database { get; }


        public ContextMongodb()
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                }
                var mongoCliente = new MongoClient(settings);
                _database = mongoCliente.GetDatabase(Database);

            }
            catch (Exception)
            {
                throw new Exception("Não foi possível conectar Mongodb");
            }
        }

        public IMongoCollection<Livro> Livros
        {
            get
            {
                return _database.GetCollection<Livro>("Livros");
            }
        }

        public IMongoCollection<Emprestimo> Emprestimos
        {
            get
            {
                return _database.GetCollection<Emprestimo>("Emprestimos");
            }
        }

        public IMongoCollection<Exemplar> Exemplares
        {
            get
            {
                return _database.GetCollection<Exemplar>("Exemplares");
            }
        }

        public IMongoCollection<Reserva> Reservas
        {
            get
            {
                return _database.GetCollection<Reserva>("Reservas");
            }
        }

        public IMongoCollection<Usuario> Usuarios
        {
            get
            {
                return _database.GetCollection<Usuario>("Usuarios");
            }
        }

        public IMongoCollection<Area> Areas
        {
            get
            {
                return _database.GetCollection<Area>("Areas");
            }
        }

        public IMongoCollection<TipoUsuario> TiposUsuarios
        {
            get
            {
                return _database.GetCollection<TipoUsuario>("TiposUsuarios");
            }
        }


    }
}
