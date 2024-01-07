<%@ Page Title="Pesquisa de Associados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssociadoPesquisa.aspx.cs" Inherits="TesteS4E.AssociadoPesquisa
    " %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section>

        <script type="text/javascript">
            //Função para selecionar todas as empresas quando a checkbox do cabeçalho for selecionada
            function SelectAllCheckboxes(checkbox) {
                var checkboxes = document.querySelectorAll('[id*=chkSelect]');
                for (var i = 0; i < checkboxes.length; i++) {
                    checkboxes[i].checked = checkbox.checked;
                }
            }
            //Máscara para o campo CPF
            $(document).ready(function () {
                $('#<%=txtCpf.ClientID %>').mask('000.000.000-00', { reverse: true });
            });
        </script>


        <div class="container-fluid">

            <%-- Titulo da Página --%>
            <div class="row mt-4">
                <hgroup>
                    <h2><%: Page.Title %></h2>
                </hgroup>
            </div>

            <%-- Mensagem de Erro --%>
            <asp:Panel ID="panelMsgErro" runat="server" Visible="false" class="row mt-4">
                <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>
            </asp:Panel>

            <%-- Campos de pesquisa e edição de associado --%>
            <asp:Panel ID="panelControles" runat="server" class="row mt-4">
                <div class="col">
                    <asp:Label ID="lblId" for="txtId" runat="server" class="form-label">ID</asp:Label>
                    <asp:TextBox ID="txtId" runat="server" CssClass="form-control" />
                </div>

                <div class="col">
                    <asp:Label ID="lblNome" for="txtNome" runat="server" class="form-label">Nome</asp:Label>
                    <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome"
                        ErrorMessage="O campo Nome é obrigatório." Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />
                </div>

                <div class="col">
                    <asp:Label ID="lblCpf" for="txtCpf" runat="server" class="form-label">CPF</asp:Label>
                    <asp:TextBox ID="txtCpf" runat="server" CssClass="form-control" />

                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtCpf"
                        ErrorMessage="O campo CPF é obrigatório." Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup"/>

                    <asp:RegularExpressionValidator ID="revCpf" runat="server" ControlToValidate="txtCpf"
                        ValidationExpression="^\d{3}\.\d{3}\.\d{3}-\d{2}$" ErrorMessage="O CPF deve ter o formato correto."
                        Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />

                </div>

                <div class="col">
                    <asp:Label ID="lblDataNascimento" for="txtDataNascimento" runat="server" class="form-label">Data de Nascimento</asp:Label>
                    <asp:TextBox ID="txtDataNascimento" runat="server" CssClass="form-control" type="date" />
                    <asp:RequiredFieldValidator ID="rfvDataNascimento" runat="server" ControlToValidate="txtDataNascimento"
                        ErrorMessage="O campo Data de Nascimento é obrigatório." Display="Dynamic" CssClass="text-danger" />
                </div>
            </asp:Panel>

            <%-- Botões Pesquisa --%>
            <asp:Panel ID="panelBotoesPesquisa" runat="server" class="row mt-4">
                <div class="col-2">
                    <asp:Button ID="btnLimpar" runat="server" Text="Limpar" OnClick="BtnLimpar_Click" CssClass="btn btn-secondary w-100" CausesValidation="false" />
                </div>
                <div class="col-2">
                    <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" OnClick="BtnPesquisar_Click" CssClass="btn btn-primary w-100" CausesValidation="false" />
                </div>
            </asp:Panel>

            <%-- Botões Edição --%>
            <asp:Panel ID="panelBotoesEditar" runat="server" class="row mt-4" Visible="false">
                <div class="col-2">
                    <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" OnClick="BtnCancelar_Click" CssClass="btn btn-secondary w-100" CausesValidation="false" />
                </div>
                <div class="col-2">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="BtnSalvar_Click" CssClass="btn btn-primary w-100" ValidationGroup="CadastroValidationGroup" />
                </div>
            </asp:Panel>

            <%-- Botões Detalhes --%>
            <asp:Panel ID="panelBotoesDetalhes" runat="server" class="row mt-4" Visible="false">
                <div class="col-2">
                    <asp:Button ID="btnVoltar" runat="server" Text="Voltar" OnClick="BtnVoltar_Click" CssClass="btn btn-primary w-100" CausesValidation="false" />
                </div>
            </asp:Panel>

            <%-- Grid Associados --%>
            <div class="row mt-4">
                <asp:GridView ID="gridAssociados" runat="server" AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                    CssClass="table table-striped" DataKeyNames="Id" HeaderStyle-HorizontalAlign="Center" OnRowCommand="GridAssociados_RowCommand"
                    OnRowDataBound="GridAssociados_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" SortExpression="Id" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Cpf" HeaderText="CPF" SortExpression="Cpf" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="DataNascimento" HeaderText="Data de Nascimento" SortExpression="DataNascimento" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-HorizontalAlign="Center" />

                        <asp:ButtonField Text="Detalhes" CommandName="Detalhes" ItemStyle-HorizontalAlign="Center" />
                        <asp:ButtonField Text="Editar" CommandName="Editar" ItemStyle-HorizontalAlign="Center" />
                        <asp:ButtonField Text="Excluir" CommandName="Excluir" ItemStyle-HorizontalAlign="Center" />

                    </Columns>
                </asp:GridView>
            </div>

            <%-- Grid Empresas --%>
            <asp:Panel ID="panelGridEmpresas" runat="server" Visible="false">
                <div class="row mt-4">
                    <h2>Empresas Associadas</h2>
                </div>

                <div class="row mt-4">
                    <asp:GridView ID="gridEmpresas" runat="server" AutoGenerateColumns="False" CssClass="table table-striped"
                        DataKeyNames="Id" HeaderStyle-HorizontalAlign="Center" AllowPaging="True" PageSize="10"
                        OnRowDataBound="GridEmpresas_RowDataBound" EmptyDataText="Nenhuma empresa encontrada!">
                        <Columns>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="false">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkSelectAll" runat="server" onclick="SelectAllCheckboxes(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkSelect" runat="server" Style="text-align: center;" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="false" />
                            <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" ItemStyle-HorizontalAlign="Center" />
                            <asp:BoundField DataField="Cnpj" HeaderText="CNPJ" SortExpression="Cnpj" ItemStyle-HorizontalAlign="Center" />
                        </Columns>
                    </asp:GridView>
                </div>
            </asp:Panel>

        </div>
    </section>
</asp:Content>
