using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TesteS4E.Models
{
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            GetAssociados().ForEach(c => context.Associados.Add(c));
            GetEmpresas().ForEach(p => context.Empresas.Add(p));
            GetAssociadosEmpresas().ForEach(p => context.AssociadoEmpresa.Add(p));
        }

        private static List<Associado> GetAssociados()
        {
            var associados = new List<Associado>
            {
                new Associado
                {
                    Id = 1,
                    Nome = "Ian Gabriel Klapouch",
                    Cpf = "09756997923",
                    DataNascimento = new DateTime(1999, 5, 20)
                }
            };

            return associados;
        }

        private static List<Empresa> GetEmpresas()
        {
            var empresas = new List<Empresa>
            {
                new Empresa
                {
                    Id = 1,
                    Nome = "Klapouch Informatica",
                    Cnpj = "01234567891011"
                }
            };

            return empresas;
        }

        private static List<AssociadoEmpresa> GetAssociadosEmpresas()
        {
            var associadosEmpresas = new List<AssociadoEmpresa>
            {
                new AssociadoEmpresa
                {
                    AssociadoId = 1,
                    EmpresaId = 1
                }
            };

            return associadosEmpresas;
        }
    }

}