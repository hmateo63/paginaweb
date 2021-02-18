<%@ Page Title="" Language="VB" MasterPageFile="~/privado/MasterPage.master" AutoEventWireup="false"
    CodeFile="ResumenTarjetasDeRuta.aspx.vb" Inherits="privado_ResumenTarjetasDeRuta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <input type="hidden" runat="server" id="husuario" />
    <input type="hidden" runat="server" id="hagencia" />
    <div class="whead">
        <h2 id="titulo">
            Resumen de Liquidación de Facturas</h2>
    </div>
    <div class="form-group">
        <label for="inputdefault">
            Inicial</label>
        <input type="date" id="txtFechaInicial" name="txtFechaInicial" class="form-control">
    </div>
    <div class="form-group">
        <label for="inputdefault">
            Inicial:</label>
        <input type="date" id="txtFechaFinal" name="txtFechaFinal" class="form-control">
    </div>
    <div class='form-group'>
        <label for="sel1">
            Agencia</label>
        <asp:DropDownList ID="DropAgencia" runat="server" class="form-control">
        </asp:DropDownList>
    </div>
    <div class='form-group'>
        <label for="sel1">
            Vehiculos</label>
        <asp:DropDownList ID="DropVehiculos" runat="server" class="form-control">
        </asp:DropDownList>
    </div>
    <div class='form-group'>
        <label for="sel1">
            Pilotos</label>
        <asp:DropDownList ID="DropPilotos" runat="server" class="form-control">
        </asp:DropDownList>
    </div>
    <div class='form-group'>
        <label for="sel1">
            Sevicios</label>
        <asp:DropDownList ID="DropServicios" runat="server" class="form-control">
        </asp:DropDownList>
    </div>



                <div class="control-group">
                <label for="textfield" class="control-label">
                    1) Fecha</label>
                <asp:TextBox ID="TxtFecha" runat="server" class="input-medium datepick" data-date-format="yyyy/mm/dd"
                    placeholder="Fecha" AutoPostBack="True" CausesValidation="True"></asp:TextBox>
            </div>









    <div class="form-group">
        <asp:Button ID="Button2" runat="server" Text="Reporte" class="btn btn-primary btn-block"
            ToolTip="Genera la consulta, luego puede generar archivo en excel " />
    </div>
</asp:Content>
