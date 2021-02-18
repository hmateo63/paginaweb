<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DetalleViaticos.aspx.vb"
    Inherits="privado_DetalleViaticos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    <title>LITEGUA</title>
    <link href="../css/styles.css" rel="stylesheet" type="text/css" />
    <link href="source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="css/notifications.css" rel="stylesheet" type="text/css" />
    <script src="js/jquery1.8.3.js" type="text/javascript"></script>
    <script src="js/jquery1.92.2.js" type="text/javascript"></script>
    <script src="source/jquery.fancybox.pack.js" type="text/javascript"></script>
    <script src="js/jquery.notifications.js" type="text/javascript"></script>
    <script type="text/javascript" src="../js/plugins/charts/excanvas.min.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.orderBars.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.pie.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.flot.resize.js"></script>
    <script type="text/javascript" src="../js/plugins/charts/jquery.sparkline.min.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.dataTables.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.sortable.js"></script>
    <script type="text/javascript" src="../js/plugins/tables/jquery.resizable.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.autosize.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.uniform.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.inputlimiter.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.tagsinput.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.maskedinput.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.autotab.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.select2.min.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.dualListBox.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.cleditor.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.ibutton.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.validationEngine-en.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.validationEngine.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.html4.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/plupload.html5.js"></script>
    <script type="text/javascript" src="../js/plugins/uploader/jquery.plupload.queue.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.form.wizard.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.validate.js"></script>
    <script type="text/javascript" src="../js/plugins/wizards/jquery.form.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.collapsible.min.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.breadcrumbs.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.tipsy.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.progress.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.timeentry.min.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.colorpicker.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.jgrowl.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.fileTree.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.sourcerer.js"></script>
    <script type="text/javascript" src="../js/plugins/others/jquery.fullcalendar.js"></script>
    <script type="text/javascript" src="../js/plugins/others/jquery.elfinder.js"></script>
    <script type="text/javascript" src="../js/plugins/forms/jquery.mousewheel.js"></script>
    <script type="text/javascript" src="../js/plugins/ui/jquery.easytabs.min.js"></script>
    <script type="text/javascript" src="../js/files/bootstrap.js"></script>
    <script type="text/javascript" src="../js/files/functions.js"></script>
    <style type="text/css">
        .mostrar
        {
            position: relative;
            float: left;
            width: 350px;
        }
        .ocultar
        {
            position: relative;
            float: left;
            width: 350px;
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        var usuario = "";
        var numerodetalles = 0;
        var origenes = "";

        $(function () {
            $("#txtFechaInicial").datepicker({
                numberOfMonths: 1,
                dateFormat: 'yy/mm/dd',
                firstDay: 0
            });
        });

        $(function () {
            $("#txtFechaFinal").datepicker({
                numberOfMonths: 1,
                dateFormat: 'yy/mm/dd',
                firstDay: 0
            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <a href="DashBoard.aspx" class="btn btn-danger"><i class="icon-home"></i></a>
    <input type="hidden" runat="server" id="husuario" />
    <input type="hidden" runat="server" id="hagencia" />
    <!--DIV FLUID-->
    <div class="fluid">
        <!--DIV BUSQUEDA-->
        <div id="divbusqueda" class="widget grid12">
            <div class="whead">
                <h2 id="titulo">
                    Detalle de Viaticos</h2>
            </div>
            &nbsp;&nbsp;&nbsp;
            <div class='formrow'>
            </div>
            <asp:Label ID="Label1" runat="server" Text="Fecha Inicial"></asp:Label>
            <asp:TextBox ID="txtFechaInicial" runat="server" MaxLength="20" Style="text-align: right;
                height: 27px; border: solid 1px #BEBEBE;" name="regular"></asp:TextBox>
            <%--        <input id="txtfechainicialreporte" class="textboxshadow hasDatepicker" style="text-align: right;height: 27px;border: solid 1px #BEBEBE;" type="text" name="regular">
            --%>
            <asp:Label ID="Label2" runat="server" Text="Fecha Final"></asp:Label>
            <asp:TextBox ID="txtFechaFinal" runat="server" MaxLength="20" Style="text-align: right;
                height: 27px; border: solid 1px #BEBEBE;" name="regular"></asp:TextBox>
            <%--        <asp:TextBox ID="txtFechaFinal" runat="server" MaxLength="20" size="10" class="note" ></asp:TextBox>
            --%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnExport" runat="server" Text="Reporte" OnClick="MostrarGrid" class="buttonS bLightBlue"
                ToolTip="Genera la consulta, luego puede generar archivo en excel " />
            <asp:Button ID="Button1" runat="server" Text="Exportar a Excel" OnClick="ExportToExcel"
                class="buttonS bLightBlue" Visible="False" />
            <asp:Button ID="btnToPdf" runat="server" Text="Exportar a Pdf" OnClick="ExportToPdf"
                class="buttonS bLightBlue" Visible="False" />
            <asp:Button ID="btnToWord" runat="server" Text="Exportar a Word" OnClick="ExportToWord"
                class="buttonS bLightBlue" Visible="False" />
            <div>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:GridView ID="GridView1" HeaderStyle-BackColor="#059719" HeaderStyle-ForeColor="White"
                    RowStyle-BackColor="#469550" AlternatingRowStyle-BackColor="#ADCCB1" AlternatingRowStyle-ForeColor="#000"
                    runat="server" AutoGenerateColumns="false" AllowPaging="false" OnPageIndexChanging="OnPageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="fecha_horario" HeaderText="Fecha" ItemStyle-Width="200px" />
                        <asp:BoundField DataField="CodigoEmpleado" HeaderText="Id Empleado" ItemStyle-Width="150px" />
                        <asp:BoundField DataField="Empleado" HeaderText="Nombre de Empleado" ItemStyle-Width="250px" />
                        <asp:BoundField DataField="Puesto" HeaderText="Puesto" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Viaticos" HeaderText="Viaticos" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="numerotarjeta" HeaderText="Numero Tarjeta" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="hora" HeaderText="Hora" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Agencia" HeaderText="Agencia" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Usuario" HeaderText="Usuario" ItemStyle-Width="150px" />
                        <asp:BoundField DataField="Bus" HeaderText="Bus" ItemStyle-Width="100px" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" ItemStyle-Width="100px" />
                    </Columns>
                </asp:GridView>
                <br />
            </div>
        </div>
    </div>
    </form>
</body>
</html>
