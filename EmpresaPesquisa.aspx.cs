using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TesteS4E.Models;
using TesteS4E.Services;
using TesteS4E.Utils;

namespace TesteS4E
{
    public partial class EmpresaPesquisa : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly AssociadoService _associadoService;
        private readonly EmpresaService _empresaService;
        private readonly AssociadoEmpresaService _associadoEmpresaService;
        public EmpresaPesquisa()
        {
            _associadoService = new AssociadoService(_context);
            _empresaService = new EmpresaService(_context);
            _associadoEmpresaService = new AssociadoEmpresaService(_context);
        }
        //Enums para evitar "magic numbers". referência: https://en.wikipedia.org/wiki/Magic_number_(programming)
        private enum ColunasGridEmpresas
        {
            Id,
            Nome,
            Cnpj,
            CommandDetalhes,
            CommandEditar,
            CommandExcluir
        }
        private enum ColunasGridAssociados
        {
            Checkbox,
            Id,
            Nome,
            Cpf,
            DataNascimento
        }
        //Variáveis de controle, criadas como static para persistir o valor entre as requisiçoes.
        private static bool _atualizandoEmpresa = false;
        private static int _idEmpresaSelecionada = 0;
        private static List<int> _idAssociados = new List<int>();
        protected void Page_Load(object sender, EventArgs e) { }

        #region Ações Botões
        //Botões Pesquisa
        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimpaControles();
        }
        protected void BtnPesquisar_Click(object sender, EventArgs e)
        {
            var empresas = _empresaService.GetEmpresas(txtId.Text, txtNome.Text, txtCnpj.Text);
            gridEmpresas.DataSource = empresas;
            gridEmpresas.DataBind();
        }
        //Botões Edição
        protected void BtnVoltar_Click(object sender, EventArgs e)
        {
            panelControles.Visible = true;

            panelBotoesPesquisa.Visible = true;
            panelBotoesDetalhes.Visible = false;

            gridEmpresas.DataSource = _empresaService.GetEmpresas();
            gridEmpresas.DataBind();
            MostraComandosGridEmpresas();

            panelGridAssociados.Visible = false;
            gridAssociados.DataSource = null;
            gridAssociados.DataBind();

            EscondeCheckBoxGridAssociados();
        }
        protected void BtnSalvar_Click(object sender, EventArgs e)
        {
            var empresa = _empresaService.GetEmpresa(_idEmpresaSelecionada);

            if (empresa != null)
            {

                string cnpj = CnpjUtils.RemoveMascara(txtCnpj.Text);

                //Verifica se o cpf já está ligado com outro associado que não o selecionado para atualização
                if (_empresaService.CnpjExiste(cnpj, empresa.Id))
                {
                    CriaMensagemErro("CNPJ já cadastrado. Por favor, insira um CNPJ único.");
                    return;
                }

                //Atualiza associado
                empresa.Nome = txtNome.Text;
                empresa.Cnpj = cnpj;

                _empresaService.Commit();

                // Pega as empresas selecionadas na Grid
                var associadosSelecionados = new List<int>();
                foreach (GridViewRow row in gridAssociados.Rows)
                {
                    if (row.FindControl("chkSelect") is CheckBox chkSelect && chkSelect.Checked)
                    {
                        // Obter o ID (Chave Primária) da linha
                        int rowIndex = row.RowIndex;

                        if (gridEmpresas.DataKeys != null && rowIndex < gridAssociados.DataKeys.Count)
                        {
                            int empresaId = Convert.ToInt32(gridAssociados.DataKeys[rowIndex].Value);
                            associadosSelecionados.Add(empresaId);
                        }
                    }
                }

                //Verifica se tem alguma empresa que estava associada e agora foi removida
                var idsAssociadosRemovidos = new List<int>();
                foreach (int associadoId in _idAssociados)
                {
                    if (!associadosSelecionados.Contains(associadoId))
                    {
                        idsAssociadosRemovidos.Add(associadoId);
                    }
                }

                //Se teve empresas removidas, remove a relação com as empresas removidas
                if (idsAssociadosRemovidos.Count > 0)
                {
                    foreach (int idAssociadoRemovido in idsAssociadosRemovidos)
                    {
                        var associadoRemovido = _associadoEmpresaService.GetAssociadoEmpresa(idAssociadoRemovido, empresa.Id);

                        if (associadoRemovido != null)
                        {
                            _associadoEmpresaService.ExcluirAssociadoEmpresa(associadoRemovido);
                        }
                    }
                    _associadoEmpresaService.Commit();
                }

                //Verifica se alguma empresa foi adicionada
                var idsAssociadosAdicionados = new List<int>();
                foreach (int associadoSelecionadoId in associadosSelecionados)
                {
                    if (!_idAssociados.Contains(associadoSelecionadoId))
                    {
                        idsAssociadosAdicionados.Add(associadoSelecionadoId);
                    }
                }

                //Se teve empresas adiciondas, cria a relação com as empresas adicionadas
                if (idsAssociadosAdicionados.Count > 0)
                {
                    foreach (int associoadoAdicionadoId in idsAssociadosAdicionados)
                    {
                        var associadoEmpresa = new AssociadoEmpresa
                        {
                            AssociadoId = associoadoAdicionadoId,
                            EmpresaId = empresa.Id
                        };

                        _associadoEmpresaService.AdicionarAssociadoEmpresa(associadoEmpresa);
                    }
                    _associadoEmpresaService.Commit();
                }

                //Limpando campos
                txtId.Enabled = true;

                panelBotoesPesquisa.Visible = true;
                panelBotoesEditar.Visible = false;

                MostraComandosGridEmpresas();

                gridEmpresas.DataSource = null;
                gridEmpresas.DataBind();

                panelGridAssociados.Visible = false;
                EscondeCheckBoxGridAssociados();
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

            gridEmpresas.DataSource = _empresaService.GetEmpresas();
            gridEmpresas.DataBind();
            MostraComandosGridEmpresas();

            panelGridAssociados.Visible = false;
            gridAssociados.DataSource = null;
            gridAssociados.DataBind();
            

            LimpaVariaveisDeControle();
            LimpaControles();
        }
        #endregion

        #region Ações Grid
        //Grid Empresas
        protected void GridEmpresas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cnpj = e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text;
                string cnpjFormatado = CnpjUtils.AdicionaMascara(cnpj);
                e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text = cnpjFormatado;
            }
        }
        protected void GridEmpresas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int id = Convert.ToInt32(gridEmpresas.DataKeys[rowIndex]["Id"]);

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
        private void MostraComandosGridEmpresas()
        {
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandDetalhes].Visible = true;
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandEditar].Visible = true;
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandExcluir].Visible = true;
        }
        private void EscondeComandosGridEmpresas()
        {
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandDetalhes].Visible = false;
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandEditar].Visible = false;
            gridEmpresas.Columns[(int)ColunasGridEmpresas.CommandExcluir].Visible = false;
        }
        //Grid Associados
        protected void GridAssociados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cpf = e.Row.Cells[(int)ColunasGridAssociados.Cpf].Text;
                string cpfFormatado = CpfUtils.AdicionaMascara(cpf);
                e.Row.Cells[(int)ColunasGridAssociados.Cpf].Text = cpfFormatado;

                if (_atualizandoEmpresa && _idAssociados.Count > 0)
                {
                    int associadoId = Convert.ToInt32(gridAssociados.DataKeys[e.Row.RowIndex].Value);

                    if (_idAssociados.Contains(associadoId))
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
        private void MostraCheckboxGridAssociados()
        {
            gridAssociados.Columns[(int)ColunasGridAssociados.Checkbox].Visible = true;
        }
        private void EscondeCheckBoxGridAssociados()
        {

            gridAssociados.Columns[(int)ColunasGridAssociados.Checkbox].Visible = false;
        }
        #endregion

        #region Comandos Grid Empresas
        protected void Detalhes(int id)
        {
            var empresa = _empresaService.GetEmpresa(id);

            if (empresa != null)
            {
                panelControles.Visible = false;

                panelBotoesPesquisa.Visible = false;
                panelBotoesDetalhes.Visible = true;

                EscondeComandosGridEmpresas();

                gridEmpresas.DataSource = new List<Empresa> { empresa };
                gridEmpresas.DataBind();

                panelGridAssociados.Visible = true;

                var relacionamentos = _associadoEmpresaService.GetAssociadoEmpresasPorEmpresaId(empresa.Id);
                List<int> associadoIds = relacionamentos.Select(a => a.AssociadoId).ToList();

                var associados = _associadoService.GetAssociados();

                List<Associado> associadosEmpresa = associados
                        .Where(e => associadoIds.Contains(e.Id))
                        .ToList();

                gridAssociados.DataSource = associadosEmpresa;
                gridAssociados.DataBind();
            }
        }
        protected void Editar(int id)
        {
            var empresa = _empresaService.GetEmpresa(id);

            if (empresa != null)
            {

                txtId.Enabled = false;

                txtId.Text = empresa.Id.ToString();
                txtNome.Text = empresa.Nome;
                txtCnpj.Text = CnpjUtils.AdicionaMascara(empresa.Cnpj);

                panelBotoesPesquisa.Visible = false;

                panelBotoesEditar.Visible = true;

                EscondeComandosGridEmpresas();

                gridEmpresas.DataSource = new List<Empresa> { empresa };
                gridEmpresas.DataBind();

                panelGridAssociados.Visible = true;
                MostraCheckboxGridAssociados();

                var associadoEmpresas = _associadoEmpresaService.GetAssociadoEmpresasPorAssociadoId(empresa.Id);

                List<int> associadosId = associadoEmpresas.Select(a => a.AssociadoId).ToList();

                var associados = _associadoService.GetAssociados();

                List<int> idsAssociados = associados
                        .Where(e => associadosId.Contains(e.Id))
                        .Select(e => e.Id)
                        .ToList();

                _atualizandoEmpresa = true;
                _idAssociados = idsAssociados;
                _idEmpresaSelecionada = id;

                gridAssociados.DataSource = associados;
                gridAssociados.DataBind();
            }
        }
        protected void Excluir(int id)
        {
            var empresa = _empresaService.GetEmpresa(id);

            if (empresa != null)
            {
                _empresaService.ExcluirEmpresa(empresa);
                _empresaService.Commit();

                _associadoEmpresaService.ExcluirRelacionamentoPorEmpresaId(id);
                _associadoEmpresaService.Commit();

                gridEmpresas.DataSource = _empresaService.GetEmpresas();
                gridEmpresas.DataBind();
            }
        }
        #endregion

        #region Limpar Campos
        private void LimpaControles()
        {
            RemoveMensagemErro();
            txtId.Text = string.Empty;
            txtNome.Text = string.Empty;
            txtCnpj.Text = string.Empty;
        }
        private void LimpaVariaveisDeControle()
        {
            _atualizandoEmpresa = false;
            _idAssociados.Clear();
            _idEmpresaSelecionada = 0;
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