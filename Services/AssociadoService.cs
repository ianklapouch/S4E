using System;
using System.Collections.Generic;
using System.Linq;
using TesteS4E.Models;

namespace TesteS4E.Services
{
    public class AssociadoService : IDbService
    {
        private readonly AppDbContext _context;
        public AssociadoService(AppDbContext context)
        {
            _context = context;
        }

        public List<Associado> GetAssociados()
        {
            return _context.Associados.ToList();
        }
        public List<Associado> GetAssociados(string id, string nome, string cpf, string dataNascimento)
        {
            IQueryable<Associado> query = _context.Associados;

            if (!string.IsNullOrEmpty(id))
            {
                int intId = Convert.ToInt32(id);
                query = query.Where(a => a.Id == intId);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(a => a.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(cpf))
            {
                query = query.Where(a => a.Cpf.Contains(cpf));
            }

            if (!string.IsNullOrEmpty(dataNascimento))
            {
                DateTime dateTimeDataNascimento = DateTime.Parse(dataNascimento);
                query = query.Where(a => a.DataNascimento == dateTimeDataNascimento);
            }

            return query.ToList();
        }

        public Associado GetAssociado(int id)
        {
            var associado = _context.Associados.Find(id);
            return associado;
        }
        public void AdicionarAssociado(Associado associado)
        {
            _context.Associados.Add(associado);
        }
        public void ExcluirAssociado(Associado associado)
        {
            _context.Associados.Remove(associado);

        }
        public bool CpfExiste(string cpf, int associadoId = 0)
        {
            if (associadoId == 0)
            {
                return _context.Associados.Any(a => a.Cpf == cpf);
            }
            else
            {
                return _context.Associados.Any(a => a.Cpf == cpf && a.Id != associadoId);
            }
        }
        public void Commit()
        {
            _context.SaveChanges();
        }
        public void Rollback()
        {
            //Sem chamar SaveChanges() as mudanças não persistem automaticamente
        }
    }
}