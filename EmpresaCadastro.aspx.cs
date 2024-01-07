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
    public partial class EmpresaCadastro : Page
    {
        private readonly AppDbContext _context = new AppDbContext();
        private readonly AssociadoService _associadoService;
        private readonly EmpresaService _empresaService;
        private readonly AssociadoEmpresaService _associadoEmpresaService;
        public EmpresaCadastro()
        {
            _associadoService = new AssociadoService(_context);
            _empresaService = new EmpresaService(_context);
            _associadoEmpresaService = new AssociadoEmpresaService(_context);
        }

        //Enums para evitar "magic numbers". referência: https://en.wikipedia.org/wiki/Magic_number_(programming)
        private enum ColunasGridAssociados
        {
            Checkbox,
            Id,
            Nome,
            Cpf,
            DataNascimento
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Carrega as empresas apenas se a página está sendo carregada pela primeira vez e não em postBack's
            if (!IsPostBack)
            {
                CarregaAssociados();
            }
        }

        #region Ações botões
        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimpaCampos();
        }
        protected void BtnSalvar_Click(object sender, EventArgs e)
        {
            string nome = txtNome.Text;
            string cnpj = CnpjUtils.RemoveMascara(txtCnpj.Text);

            Empresa empresa = new Empresa
            {
                Nome = nome,
                Cnpj = cnpj
            };

            if (_empresaService.CnpjExiste(cnpj))
            {
                CriaMensagemErro("CNPJ já cadastrado. Por favor, insira um CNPJ único.");
                return;
            }

            _empresaService.AdicionarEmpresa(empresa);
            _empresaService.Commit();

            int empresaId = empresa.Id;

            foreach (GridViewRow row in gridAssociados.Rows)
            {
                if (row.FindControl("chkSelect") is CheckBox chkSelect && chkSelect.Checked)
                {
                    int rowIndex = row.RowIndex;

                    if (gridAssociados.DataKeys != null && rowIndex < gridAssociados.DataKeys.Count)
                    {
                        int associadoId = Convert.ToInt32(gridAssociados.DataKeys[rowIndex].Value);

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
        #endregion

        #region Ações Grid
        protected void GridAssociados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string cpf = e.Row.Cells[(int)ColunasGridAssociados.Cpf].Text;
                string cpfFormatado = CnpjUtils.AdicionaMascara(cpf);
                e.Row.Cells[(int)ColunasGridAssociados.Cpf].Text = cpfFormatado;
            }
        }
        protected void CarregaAssociados()
        {
            gridAssociados.DataSource = _associadoService.GetAssociados();
            gridAssociados.DataBind();
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
            txtCnpj.Text = string.Empty;

            gridAssociados.DataSource = _associadoService.GetAssociados();
            gridAssociados.DataBind();
        }
    }
}