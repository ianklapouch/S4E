using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using TesteS4E.Models;
using TesteS4E.Services;
using TesteS4E.Utils;

namespace TesteS4E
{
    public partial class AssociadoPesquisa : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly AssociadoService _associadoService;
        private readonly EmpresaService _empresaService;
        private readonly AssociadoEmpresaService _associadoEmpresaService;
        public AssociadoPesquisa()
        {
            _associadoService = new AssociadoService(_context);
            _empresaService = new EmpresaService(_context);
            _associadoEmpresaService = new AssociadoEmpresaService(_context);
        }
        //Enums para evitar "magic numbers". referência: https://en.wikipedia.org/wiki/Magic_number_(programming)
        private enum ColunasGridAssociados
        {
            Id,
            Nome,
            Cpf,
            DataNascimento,
            CommandDetalhes,
            CommandEditar,
            CommandExcluir
        }
        private enum ColunasGridEmpresas
        {
            Checkbox,
            Id,
            Nome,
            Cnpj
        }
        //Variáveis de controle, criadas como static para persistir o valor entre as requisiçoes.
        private static bool _atualizandoAssociado = false;
        private static int _associadoIdSelecionado = 0;
        private static List<int> _idsEmpresasAssociadas = new List<int>();
        protected void Page_Load(object sender, EventArgs e) { }

        #region Ações Botões
        //Botões Pesquisa
        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimpaControles();
        }
        protected void BtnPesquisar_Click(object sender, EventArgs e)
        {
            var associados = _associadoService.GetAssociados(txtId.Text, txtNome.Text, txtCpf.Text, txtDataNascimento.Text);
            gridAssociados.DataSource = associados;
            gridAssociados.DataBind();
        }
        //Botões Edição
        protected void BtnVoltar_Click(object sender, EventArgs e)
        {
            panelControles.Visible = true;

            panelBotoesPesquisa.Visible = true;
            panelBotoesDetalhes.Visible = false;

            gridAssociados.DataSource = _associadoService.GetAssociados();
            gridAssociados.DataBind();
            MostraComandosGridAssociados();

            panelGridEmpresas.Visible = false;
            gridEmpresas.DataSource = null;
            gridEmpresas.DataBind();

            EscondeCheckBoxGridEmpresas();
        }
        protected void BtnSalvar_Click(object sender, EventArgs e)
        {
         
                var associado = _associadoService.GetAssociado(_associadoIdSelecionado);

            if (associado != null)
            {

                string novoCpf = CpfUtils.RemoveMascara(txtCpf.Text);

                //Verifica se o cpf já está ligado com outro associado que não o selecionado para atualização
                if (_associadoService.CpfExiste(novoCpf, associado.Id))
                {
                    CriaMensagemErro("CPF já cadastrado. Por favor, insira um CPF único.");
                    return;
                }

                //Atualiza associado
                associado.Nome = txtNome.Text;
                associado.Cpf = novoCpf;
                associado.DataNascimento = DateTime.Parse(txtDataNascimento.Text);

                _associadoService.Commit();

                // Pega as empresas selecionadas na Grid
                var empresasSelecionadas = new List<int>();
                foreach (GridViewRow row in gridEmpresas.Rows)
                {
                    if (row.FindControl("chkSelect") is CheckBox chkSelect && chkSelect.Checked)
                    {
                        // Obter o ID (Chave Primária) da linha
                        int rowIndex = row.RowIndex;

                        if (gridEmpresas.DataKeys != null && rowIndex < gridEmpresas.DataKeys.Count)
                        {
                            int empresaId = Convert.ToInt32(gridEmpresas.DataKeys[rowIndex].Value);
                            empresasSelecionadas.Add(empresaId);
                        }
                    }
                }

                //Verifica se tem alguma empresa que estava associada e agora foi removida
                var idsEmpresasRemovidas = new List<int>();
                foreach (int empresaId in _idsEmpresasAssociadas)
                {
                    if (!empresasSelecionadas.Contains(empresaId))
                    {
                        idsEmpresasRemovidas.Add(empresaId);
                    }
                }

                //Se teve empresas removidas, remove a relação com as empresas removidas
                if (idsEmpresasRemovidas.Count > 0)
                {
                    foreach (int empresaRemovidaId in idsEmpresasRemovidas)
                    {
                        var empresaRemovida = _associadoEmpresaService.GetAssociadoEmpresa(associado.Id, empresaRemovidaId);

                        if (empresaRemovida != null)
                        {
                            _associadoEmpresaService.ExcluirAssociadoEmpresa(empresaRemovida);
                        }
                    }
                    _associadoEmpresaService.Commit();
                }

                //Verifica se alguma empresa foi adicionada
                var idsEmpresasAdicionadas = new List<int>();
                foreach (int empresaSelecionadaId in empresasSelecionadas)
                {
                    if (!_idsEmpresasAssociadas.Contains(empresaSelecionadaId))
                    {
                        idsEmpresasAdicionadas.Add(empresaSelecionadaId);
                    }
                }

                //Se teve empresas adiciondas, cria a relação com as empresas adicionadas
                if (idsEmpresasAdicionadas.Count > 0)
                {
                    foreach (int empresaAdicionadaId in idsEmpresasAdicionadas)
                    {
                        var associadoEmpresa = new AssociadoEmpresa
                        {
                            AssociadoId = associado.Id,
                            EmpresaId = empresaAdicionadaId
                        };

                        _associadoEmpresaService.AdicionarAssociadoEmpresa(associadoEmpresa);
                    }
                    _associadoEmpresaService.Commit();
                }

                //Limpando campos
                txtId.Enabled = true;

                panelBotoesPesquisa.Visible = true;
                panelBotoesEditar.Visible = false;

                MostraComandosGridAssociados();

                gridAssociados.DataSource = null;
                gridAssociados.DataBind();

                panelGridEmpresas.Visible = false;
                EscondeCheckBoxGridEmpresas();
                gridEmpresas.DataSource = null;
                gridEmpresas.DataBind();

                LimpaControles();

                LimpaVariaveisDeControle();
            }
          
        }
        //Botões Detalhes
        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            txtId.Enabled = true;

            panelBotoesPesquisa.Visible = true;
            panelBotoesEditar.Visible = false;

            MostraComandosGridAssociados();

            gridAssociados.DataSource = _associadoService.GetAssociados();
            gridAssociados.DataBind();

    

            panelGridEmpresas.Visible = false;
            gridEmpresas.DataSource = null;
            gridEmpresas.DataBind();

            LimpaVariaveisDeControle();
            LimpaControles();
        }
        #endregion

        #region Ações Grid
        //Grid Associados
        protected void GridAssociados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells.Count >= 4)
                {
                    string cpf = e.Row.Cells[2].Text;
                    string cpfFormatado = CpfUtils.AdicionaMascara(cpf);
                    e.Row.Cells[2].Text = cpfFormatado;
                }
            }
        }
        protected void GridAssociados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gridAssociados.DataKeys[rowIndex]["Id"]);

            switch (e.CommandName)
            {
                case "Detalhes":
                    Detalhes(id);
                    break;
                case "Editar":
                    Editar(id);
                    break;
                case "Excluir":
                    Excluir(id);
                    break;
            }
        }
        private void MostraComandosGridAssociados()
        {
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandDetalhes].Visible = true;
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandEditar].Visible = true;
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandExcluir].Visible = true;
        }
        private void EscondeComandosGridAssociados()
        {
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandDetalhes].Visible = false;
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandEditar].Visible = false;
            gridAssociados.Columns[(int)ColunasGridAssociados.CommandExcluir].Visible = false;
        }
        //Grid Empresas
        protected void GridEmpresas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cnpj = e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text;
                string cnpjFormatado = CnpjUtils.AdicionaMascara(cnpj);
                e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text = cnpjFormatado;

                if (_atualizandoAssociado && _idsEmpresasAssociadas.Count > 0)
                {
                    int empresaId = Convert.ToInt32(gridEmpresas.DataKeys[e.Row.RowIndex].Value);

                    if (_idsEmpresasAssociadas.Contains(empresaId))
                    {
                        CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                        if (chkSelect != null)
                        {
                            chkSelect.Checked = true;
                        }
                    }
                }
            }
        }
        private void MostraCheckboxGridEmpresas()
        {
            gridEmpresas.Columns[(int)ColunasGridEmpresas.Checkbox].Visible = true;
        }
        private void EscondeCheckBoxGridEmpresas()
        {
            gridEmpresas.Columns[(int)ColunasGridEmpresas.Checkbox].Visible = false;
        }
        #endregion
     
        #region Comandos Grid Associados
        protected void Detalhes(int id)
        {
            var context = new AppDbContext();
            var associado = context.Associados.Find(id);

            if (associado != null)
            {
                panelControles.Visible = false;

                panelBotoesPesquisa.Visible = false;
                panelBotoesDetalhes.Visible = true;

                EscondeComandosGridAssociados();
                EscondeCheckBoxGridEmpresas();

                gridAssociados.DataSource = new List<Associado> { associado };
                gridAssociados.DataBind();

                panelGridEmpresas.Visible = true;

                var associadoEmpresas = _associadoEmpresaService.GetAssociadoEmpresasPorAssociadoId(associado.Id);
                List<int> empresaIds = associadoEmpresas.Select(a => a.EmpresaId).ToList();

                List<Empresa> empresasAssociadas = context.Empresas
                        .Where(e => empresaIds.Contains(e.Id))
                        .ToList();

                gridEmpresas.DataSource = empresasAssociadas;
                gridEmpresas.DataBind();
            }
        }
        protected void Editar(int id)
        {
            var associado = _associadoService.GetAssociado(id);

            if (associado != null)
            {

                txtId.Enabled = false;

                txtId.Text = associado.Id.ToString();
                txtNome.Text = associado.Nome;
                txtCpf.Text = CpfUtils.AdicionaMascara(associado.Cpf);
                txtDataNascimento.Text = associado.DataNascimento.ToString("yyyy-MM-dd");

                panelBotoesPesquisa.Visible = false;

                panelBotoesEditar.Visible = true;

                EscondeComandosGridAssociados();

                gridAssociados.DataSource = new List<Associado> { associado };
                gridAssociados.DataBind();

                panelGridEmpresas.Visible = true;
                MostraCheckboxGridEmpresas();

                var associadoEmpresas = _associadoEmpresaService.GetAssociadoEmpresasPorAssociadoId(associado.Id);

                List<int> empresaIds = associadoEmpresas.Select(a => a.EmpresaId).ToList();

                var empresas = _empresaService.GetEmpresas();

                List<int> idsEmpresasAssociadas = empresas
                        .Where(e => empresaIds.Contains(e.Id))
                        .Select(e => e.Id)
                        .ToList();

                _atualizandoAssociado = true;
                _idsEmpresasAssociadas = idsEmpresasAssociadas;
                _associadoIdSelecionado = id;

                gridEmpresas.DataSource = empresas.ToList();
                gridEmpresas.DataBind();
            }
        }
        protected void Excluir(int id)
        {
            var associado = _associadoService.GetAssociado(id);

            if (associado != null)
            {
                // Remover relacionamentos na tabela AssociadoEmpresa
                _associadoEmpresaService.ExcluirRelacionamentoPorAssociadoId(id);
                _associadoEmpresaService.Commit();

                _associadoService.ExcluirAssociado(associado);
                _associadoService.Commit();

                // Recarregar a grid
                gridAssociados.DataSource = _associadoService.GetAssociados();
                gridAssociados.DataBind();
            }
        }
        #endregion

        #region Limpar Campos
        private void LimpaControles()
        {
            RemoveMensagemErro();
            txtId.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtCpf.Text = string.Empty;
            txtDataNascimento.Text = string.Empty;
        }
        private void LimpaVariaveisDeControle()
        {
            _atualizandoAssociado = false;
            _idsEmpresasAssociadas.Clear();
            _associadoIdSelecionado = 0;
        }
        #endregion

        #region Mensagem de Erro
        private void CriaMensagemErro(string mensagemDeError)
        {
            panelMsgErro.Visible = true;
            lblError.Text = mensagemDeError;
        }

        private void RemoveMensagemErro()
        {
            panelMsgErro.Visible = false;
            lblError.Text = string.Empty;
        }
        #endregion
    }
}