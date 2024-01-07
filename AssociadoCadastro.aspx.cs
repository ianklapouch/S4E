using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using TesteS4E.Models;
using TesteS4E.Services;
using TesteS4E.Utils;

namespace TesteS4E
{
    public partial class AssociadoCadastro : Page
    {

        private readonly AppDbContext _context = new AppDbContext();
        private readonly AssociadoService _associadoService;
        private readonly EmpresaService _empresaService;
        private readonly AssociadoEmpresaService _associadoEmpresaService;
        public AssociadoCadastro()
        {
            _associadoService = new AssociadoService(_context);
            _empresaService = new EmpresaService(_context);
            _associadoEmpresaService = new AssociadoEmpresaService(_context);
        }

        //Enums para evitar "magic numbers". referência: https://en.wikipedia.org/wiki/Magic_number_(programming)
        private enum ColunasGridEmpresas
        {
            Checkbox,
            Id,
            Nome,
            Cnpj
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Carrega as empresas apenas se a página está sendo carregada pela primeira vez e não em postBack's
            if (!IsPostBack)
            {
                CarregarEmpresas();
            }
        }

        #region Ações botões
        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimpaCampos();
        }
        protected void BtnSalvar_Click(object sender, EventArgs e)
        {

            if (Page.IsValid)
            {


                string nome = txtNome.Text;
                string cpf = CpfUtils.RemoveMascara(txtCpf.Text);
                DateTime dataNascimento = DateTime.Parse(txtDataNascimento.Text);

                Associado associado = new Associado
                {
                    Nome = nome,
                    Cpf = cpf,
                    DataNascimento = dataNascimento
                };

                if (_associadoService.CpfExiste(cpf))
                {
                    CriaMensagemErro("CPF já cadastrado. Por favor, insira um CPF único.");
                    return;
                }

                _associadoService.AdicionarAssociado(associado);
                _associadoService.Commit();

                int associadoId = associado.Id;

                foreach (GridViewRow row in gridEmpresas.Rows)
                {
                    if (row.FindControl("chkSelect") is CheckBox chkSelect && chkSelect.Checked)
                    {
                        int rowIndex = row.RowIndex;

                        if (gridEmpresas.DataKeys != null && rowIndex < gridEmpresas.DataKeys.Count)
                        {
                            int empresaId = Convert.ToInt32(gridEmpresas.DataKeys[rowIndex].Value);

                            var associadoEmpresa = new AssociadoEmpresa
                            {
                                AssociadoId = associadoId,
                                EmpresaId = empresaId
                            };

                            _associadoEmpresaService.AdicionarAssociadoEmpresa(associadoEmpresa);
                        }

                    }
                }
                _associadoEmpresaService.Commit();
                LimpaCampos();
            }
        }
        #endregion

        #region Ações Grid
        protected void GridEmpresas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cnpj = e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text;
                string cnpjFormatado = CnpjUtils.AdicionaMascara(cnpj);
                e.Row.Cells[(int)ColunasGridEmpresas.Cnpj].Text = cnpjFormatado;
            }
        }
        protected void CarregarEmpresas()
        {
            gridEmpresas.DataSource = _empresaService.GetEmpresas();
            gridEmpresas.DataBind();
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

        private void LimpaCampos()
        {
            RemoveMensagemErro();

            txtNome.Text = string.Empty;
            txtCpf.Text = string.Empty;
            txtDataNascimento.Text = string.Empty;

            gridEmpresas.DataSource = _empresaService.GetEmpresas();
            gridEmpresas.DataBind();
        }
    }
}