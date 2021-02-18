<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResumenLiquidacionFacturas.aspx.vb"
    Inherits="privado_ResumenLiquidacionFacturas" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
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

            $("#txtFechaInicial").datepicker().datepicker("setDate", new Date());  // por default la fecah del dia

        });


        //        $(function () {
        //            $('#orderDate').datepicker({
        //                dateFormat: 'dd-mm-yy'
        //            });
        //            $("#orderDate").datepicker().datepicker("setDate", new Date());
        //        });




        $(function () {
            $("#txtFechaFinal").datepicker({
                numberOfMonths: 1,
                dateFormat: 'yy/mm/dd',
                firstDay: 0
            });

            $("#txtFechaFinal").datepicker().datepicker("setDate", new Date());
        });



    </script>
</head>
<body>
    <form id="form1" runat="server">

    <a href="DashBoard.aspx" class="btn btn-info btn-lg"><i class="icon-home"></i></a>





    


    <%--        <ul class="nav navbar-nav navbar-right">
        <li><a href="DashBoard.aspx">  <img src="<% =Session("UrlImagen")%>" style="width: 90px" alt="Regresar al Menu"
                data-toggle="tooltip" title="Regresar al Menu" /></li>
    </ul>
    --%>
    <input type="hidden" runat="server" id="husuario" />
    <input type="hidden" runat="server" id="hagencia" />
    <!--DIV FLUID-->
    <div class="fluid">
        <!--DIV BUSQUEDA-->
        <div id="divbusqueda" class="widget grid12">
            <div class="whead">
                <h2 id="titulo">
                    Resumen de Liquidación de Facturas</h2>
            </div>
            &nbsp;&nbsp;&nbsp;
            <div id="DivArea1" runat="server">
                <div class='formrow'>
                    <asp:Label ID="Label1" runat="server" Text="Fecha Inicial"></asp:Label>
                    <asp:TextBox ID="txtFechaInicial" runat="server" MaxLength="20" Style="text-align: right;
                        height: 27px; border: solid 1px #BEBEBE;" name="regular"></asp:TextBox>
                </div>
                <div class='formrow'>
                    <asp:Label ID="Label2" runat="server" Text="Fecha Final"></asp:Label>
                    <asp:TextBox ID="txtFechaFinal" runat="server" MaxLength="20" Style="text-align: right;
                        height: 27px; border: solid 1px #BEBEBE;" name="regular"></asp:TextBox>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </div>
                <%--            ***************************--%>


<%--                <div class='formrow'>
                    <label for="tel2" class="col-md-1 control-label">
                        Agencia</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropAgencia" runat="server" class="form-control input-sm">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class='formrow'>
                    <label for="tel2" class="col-md-1 control-label">
                        Vehiculos</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropVehiculos" runat="server" class="form-control input-sm">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class='formrow'>
                    <label for="tel2" class="col-md-1 control-label">
                        Pilotos</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropPilotos" runat="server" class="form-control input-sm">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class='formrow'>
                    <label for="tel2" class="col-md-1 control-label">
                        Sevicios</label>
                    <div class="col-md-3">
                        <asp:DropDownList ID="DropServicios" runat="server" class="form-control input-sm">
                        </asp:DropDownList>
                    </div>
                </div>
--%>

                <div class="formrow">
                    <asp:Button ID="Button2" runat="server" Text="Reporte" class="buttonS bLightBlue"
                        ToolTip="Genera la consulta, luego puede generar archivo en excel " />
                </div>
            </div>
            <div runat="server" id="DivGridView1">
                <%--
                    <ul class="nav navbar-nav navbar-right">
        <li><a href="DashBoard.aspx">
            <img src="<% =Session("UrlImagen")%>" style="width: 90px" alt="Regresar al Menu"
                data-toggle="tooltip" title="Regresar al Menu" /></li>
    </ul>


                --%>
                <asp:Button ID="Button1" runat="server" Text="Exportar a Excel" OnClick="ExportToExcel"
                    class="buttonS bLightBlue" Visible="False" />
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="TRUE" AutoGenerateSelectButton="True"
                    CssClass="table table-bordered dataTable dataTable-scroll-x">
                </asp:GridView>
            </div>
        </div>
    </div>
    <div runat="server" id="DivGridView2">
        <div class="whead">
            <h6 id="H1">
                Vales De Ruta Electronico</h6>
            <asp:Button ID="Button3" runat="server" Text="Regresar a Tarjetas" class="buttonS bLightBlue" />
        </div>
        <!--CLIENTE-->
    </div>
    </form>
</body>
</html>
