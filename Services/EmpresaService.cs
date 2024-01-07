using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TesteS4E.Models;

namespace TesteS4E.Services
{
    public class EmpresaService : IDbService
    {
        private readonly AppDbContext _context;
        public EmpresaService(AppDbContext context)
        {
            _context = context;
        }
        public Empresa GetEmpresa(int id)
        {
            var empresa = _context.Empresas.Find(id);
            return empresa;
        }
        public List<Empresa> GetEmpresas()
        {
            var empresas = _context.Empresas.ToList();
            return empresas;
        }

        public List<Empresa> GetEmpresas(string id, string nome, string cnpj)
        {
            IQueryable<Empresa> query = _context.Empresas;

            if (!string.IsNullOrEmpty(id))
            {
                int intId = Convert.ToInt32(id);
                query = query.Where(a => a.Id == intId);
            }

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(a => a.Nome.Contains(nome));
            }

            if (!string.IsNullOrEmpty(cnpj))
            {
                query = query.Where(a => a.Cnpj.Contains(cnpj));
            }

            return query.ToList();
        }
        public void AdicionarEmpresa(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
        }

        public bool CnpjExiste(string cnpj, int empresaId = 0)
        {
            if (empresaId == 0)
            {
                return _context.Empresas.Any(a => a.Cnpj == cnpj);
            }
            else
            {
                return _context.Empresas.Any(a => a.Cnpj == cnpj && a.Id != empresaId);
            }
        }

        public void ExcluirEmpresa(Empresa empresa)
        {
            _context.Empresas.Remove(empresa);
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