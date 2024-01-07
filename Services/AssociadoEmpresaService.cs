using System.Linq;
using TesteS4E.Models;

namespace TesteS4E.Services
{
    public class AssociadoEmpresaService : IDbService
    {
        private readonly AppDbContext _context;
        public AssociadoEmpresaService(AppDbContext context)
        {
            _context = context;
        }

        public AssociadoEmpresa GetAssociadoEmpresa(int associadoId, int empresaId)
        {
            return _context.AssociadoEmpresa.FirstOrDefault(x => x.AssociadoId == associadoId && x.EmpresaId == empresaId);
        }

        public IQueryable<AssociadoEmpresa> GetAssociadoEmpresasPorAssociadoId(int associadoId)
        {
            IQueryable<AssociadoEmpresa> associadoEmpresas = _context.AssociadoEmpresa;
            return associadoEmpresas.Where(a => a.AssociadoId == associadoId);
        }

        public IQueryable<AssociadoEmpresa> GetAssociadoEmpresasPorEmpresaId(int empresaId)
        {
            IQueryable<AssociadoEmpresa> associadoEmpresas = _context.AssociadoEmpresa;
            return associadoEmpresas.Where(a => a.EmpresaId == empresaId);
        }
        public void AdicionarAssociadoEmpresa(AssociadoEmpresa associadoEmpresa)
        {
            _context.AssociadoEmpresa.Add(associadoEmpresa);
        }

        public void ExcluirAssociadoEmpresa(AssociadoEmpresa associadoEmpresa)
        {
            _context.AssociadoEmpresa.Remove(associadoEmpresa);
        }

        public void ExcluirRelacionamentoPorAssociadoId(int associadoId)
        {
            var relacionamentos = _context.AssociadoEmpresa.Where(ae => ae.AssociadoId == associadoId).ToList();
            _context.AssociadoEmpresa.RemoveRange(relacionamentos);
        }

        public void ExcluirRelacionamentoPorEmpresaId(int empresaId)
        {
            var relacionamentos = _context.AssociadoEmpresa.Where(ae => ae.EmpresaId == empresaId).ToList();
            _context.AssociadoEmpresa.RemoveRange(relacionamentos);
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