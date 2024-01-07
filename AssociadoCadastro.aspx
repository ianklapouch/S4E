<%@ Page Title="Cadastro de Associados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AssociadoCadastro.aspx.cs" Inherits="TesteS4E.AssociadoCadastro" %>

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

            <%-- Campos de cadastro --%>
            <div class="row mt-4">
                <div class="col">
                    <label for="txtNome" class="form-label">Nome</label>
                    <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome"
                        ErrorMessage="O campo Nome é obrigatório." Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />
                </div>
                <div class="col">
                    <label for="txtCpf" class="form-label">CPF</label>
                    <asp:TextBox ID="txtCpf" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvCpf" runat="server" ControlToValidate="txtCpf"
                        ErrorMessage="O campo CPF é obrigatório." Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />

                    <asp:RegularExpressionValidator ID="revCpf" runat="server" ControlToValidate="txtCpf"
                        ValidationExpression="^\d{3}\.\d{3}\.\d{3}-\d{2}$" ErrorMessage="O CPF deve ter o formato correto."
                        Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />
                </div>
                <div class="col">
                    <label for="txtDataNascimento" class="form-label">Data de Nascimento</label>
                    <asp:TextBox ID="txtDataNascimento" runat="server" CssClass="form-control" type="date" />
                    <asp:RequiredFieldValidator ID="rfvDataNascimento" runat="server" ControlToValidate="txtDataNascimento"
                        ErrorMessage="O campo Data de Nascimento é obrigatório." Display="Dynamic" CssClass="text-danger" ValidationGroup="CadastroValidationGroup" />

                </div>
            </div>

            <%-- Grid Empresas --%>
            <div class="row mt-4">
                <h2>Empresas</h2>
            </div>
            
            <div class="row mt-4">
                <asp:GridView ID="gridEmpresas" runat="server" AutoGenerateColumns="False" CssClass="table table-striped"
                    DataKeyNames="Id" HeaderStyle-HorizontalAlign="Center" AllowPaging="True" PageSize="10"
                    OnRowDataBound="GridEmpresas_RowDataBound"
                    >
                    <Columns>
                        <asp:TemplateField  ItemStyle-HorizontalAlign="Center" >
                            <HeaderTemplate>
                                <asp:CheckBox ID="chkSelectAll" runat="server" onclick="SelectAllCheckboxes(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" style="text-align: center;" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" Visible="false" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" SortExpression="Nome" ItemStyle-HorizontalAlign="Center" />
                        <asp:BoundField DataField="Cnpj" HeaderText="CNPJ" SortExpression="Cnpj" ItemStyle-HorizontalAlign="Center" />
                    </Columns>
                </asp:GridView>
            </div>

            <%-- Botões --%>
            <div class="row">
                <div class="col-2">
                    <asp:Button ID="btnLimpar" runat="server" Text="Limpar" OnClick="BtnLimpar_Click"
                        CssClass="btn btn-secondary w-100" CausesValidation="false" />
                </div>
                <div class="col-2">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" OnClick="BtnSalvar_Click"
                        CssClass="btn btn-primary w-100" ValidationGroup="CadastroValidationGroup" />
                </div>
            </div>
        </div>

    </section>
</asp:Content>
