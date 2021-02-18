
'Partial Class privado_ResumenLiquidacionFacturas
'    Inherits System.Web.UI.Page

'End Class

Imports System.Data
Imports iTextSharp.text
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Web.Services
Imports iTextSharp.text.pdf
Imports System.Configuration
Imports iTextSharp.text.html.simpleparser

Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System
Imports System.Web.Security
Imports System.Web.UI.WebControls.WebParts

Partial Class privado_ResumenLiquidacionFacturas
    Inherits System.Web.UI.Page

    Dim Accion As Integer = 0
    Dim codigopagina As Integer = 3

    Dim filas As GridViewRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Accion = 1
        Dim strsql As String = ""
        If Session("id_usr") = "" Then
            FormsAuthentication.RedirectToLoginPage()
        Else
            husuario.Value = Session("id_usr")
            hagencia.Value = Session("id_agc")
            If DatosSql.permiso(husuario.Value, Accion, codigopagina) = 0 Then
                Response.Redirect("error.html")
            End If
        End If

        If (Not IsPostBack()) Then

            'cargar_Combo(DropAgencia, "select 0 id_agencia,  'TODAS' nombre_agencia    union select id_agc as id_agencia,nom_agc as nombre_agencia from Facturacion.agencia where id_empsa=" & Session("id_empsa") & " order by id_agencia;")
            'cargar_Combo(DropVehiculos, "select 0 id_vehiculo, 'TODOS' nombre_vehiculo union select id_veh as id_vehiculo,id_veh as nombre_vehiculo from Encomiendas.vehiculo where id_veh REGEXP '^[0-9]*[.]?[0-9]+$' and id_empsa=" & Session("id_empsa") & "   order by id_vehiculo;")
            'cargar_Combo(DropPilotos, "select 0 as id_piloto, 'TODOS' nombre_piloto    union select id_emp as id_piloto, nom_emp as nombre_piloto from Encomiendas.empleado where id_pso=1  and id_empsa=" & Session("id_empsa") & " order by id_piloto;")
            'cargar_Combo(DropServicios, "SELECT 0 as id_servicio ,'TODOS' nombre_servicio      union select idservicio as id_servicio,descripcion as nombre_servicio from Boletos.servicio where  id_empsa=" & Session("id_empsa") & " order by id_servicio;")

        End If

        DivGridView1.Visible = False
        DivGridView2.Visible = False

    End Sub
    Protected Sub ExportToExcel(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.GenerarExcel()
    End Sub
    Public Function GridViewToHtml(ByVal grid As GridView) As String
        Dim sTringBuilder As New StringBuilder()
        Dim sTringWriter As New StringWriter(sTringBuilder)
        Dim hTmlTextWriter As New HtmlTextWriter(sTringWriter)
        grid.RenderControl(hTmlTextWriter)
        Return sTringBuilder.ToString()
    End Function

    Private Sub GenerarExcel()
        Dim sb As StringBuilder = New StringBuilder()
        Dim sw As StringWriter = New StringWriter(sb)
        Dim htw As HtmlTextWriter = New HtmlTextWriter(sw)
        Dim pagina As Page = New Page
        Dim form = New HtmlForm
        GridView1.EnableViewState = False
        pagina.EnableEventValidation = False
        pagina.DesignerInitialize()
        pagina.Controls.Add(form)
        form.Controls.Add(GridView1)
        pagina.RenderControl(htw)
        Response.Clear()
        Response.Buffer = True
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "attachment;filename=ResumenLiquidacionFacturas.xls")
        Response.Charset = "UTF-8"
        Response.ContentEncoding = Encoding.Default
        Response.Write(sb.ToString())
        Response.End()
    End Sub
    Public Function Formatted_GridViewToHtml(ByVal grid As GridView) As String
        Dim sTringBuilder As New StringBuilder()
        Dim sTringWriter As New StringWriter(sTringBuilder)
        Dim hTmlTextWriter As New HtmlTextWriter(sTringWriter)

        grid.HeaderRow.Style.Add("background-color", "#FFFFFF")

        For Each tableCell As TableCell In grid.HeaderRow.Cells
            tableCell.Style("background-color") = "#A55129"
        Next

        For Each gridViewRow As GridViewRow In grid.Rows
            gridViewRow.BackColor = System.Drawing.Color.White
            For Each gridViewRowTableCell As TableCell In gridViewRow.Cells
                gridViewRowTableCell.Style("background-color") = "#FFF7E7"
            Next

        Next
        grid.RenderControl(hTmlTextWriter)
        Return sTringBuilder.ToString()
    End Function
    Public Shared Sub cargar_Combo(ByVal ComboBox As DropDownList, ByVal strsql As String)
        Using conexion As New MySqlConnection
            'conexion = New MySqlConnection()
            conexion.ConnectionString = cadenaCon()
            Try
                conexion.Open()
                Dim comando As New MySqlCommand(strsql, conexion)
                Dim da As New MySqlDataAdapter(comando)
                Dim ds As New System.Data.DataSet
                da.Fill(ds)
                ComboBox.DataSource = ds.Tables(0)
                ComboBox.DataValueField = ds.Tables(0).Columns(0).Caption.ToString
                ComboBox.DataTextField = ds.Tables(0).Columns(1).Caption.ToString
                ComboBox.DataBind()
                ComboBox.SelectedIndex = -1
                comando.Dispose()
                da.Dispose()
            Catch ex As MySqlException
            Finally
                conexion.Close()
            End Try
        End Using
    End Sub

    Shared Function cadenaCon() As String
        Return System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
    End Function

    Protected Sub Button2_Click(sender As Object, e As System.EventArgs) Handles Button2.Click
        DivGridView1.Visible = True
        DivArea1.Visible = False
        Dim FechaInicial As String = txtFechaInicial.Text
        Dim FechaFinal As String = txtFechaFinal.Text

        'Dim Agencia As String = If(DropAgencia.SelectedValue = 0, "%", DropAgencia.SelectedValue)
        'Dim Servicio As String = If(DropServicios.SelectedValue = 0, "%", DropServicios.SelectedValue)
        'Dim Vehiculo As String = If(DropVehiculos.SelectedValue = 0, "%", DropVehiculos.SelectedValue)
        'Dim Piloto As String = If(DropPilotos.SelectedValue = 0, "%", DropPilotos.SelectedValue)

        Dim Agencia As String = "%"
        Dim Servicio As String = "%"
        Dim Vehiculo As String = "%"
        Dim Piloto As String = "%"







        Dim strConnString As String = ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
        Using con As New MySqlConnection(strConnString)
            Using cmd As New MySqlCommand("call Boletos.sp_ResumenLiquidacionFacturas('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "'," & Session("id_empsa") & ");")

                'Using cmd As New MySqlCommand("call Boletos.sp_ResumenLiquidacionFacturas2('" & FechaInicial & "','" & FechaFinal & "'," & Session("id_empsa") & ");")

                Dim altos As String = "alto"
                Using sda As New MySqlDataAdapter()
                    cmd.Connection = con
                    sda.SelectCommand = cmd
                    Using dt As New DataTable()
                        sda.Fill(dt)
                        GridView1.DataSource = dt
                        GridView1.DataBind()

                        Me.Button1.Visible = True

                    End Using
                End Using
            End Using
        End Using

    End Sub

    Public Function NumeroAleatorioPdf() As Integer
        Dim LimiteMinimo As Integer = 1
        Dim LimiteMaximo As Integer = 20
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return Aleatorio
    End Function

    Public Class Carrera
        Public cantidad_boletos As Integer
        Public cantidad_boletos_ruta As Integer
        Public total_cantidad_boletos As Integer
        Public monto_boletos As Double
        Public monto_boletos_ruta As Double
        Public monto_total As Double

    End Class
    Protected Sub Button3_Click(sender As Object, e As System.EventArgs) Handles Button3.Click
        DivGridView1.Visible = False
        DivArea1.Visible = True
    End Sub

    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
        filas = GridView1.SelectedRow
        Dim numerotarjeta As Integer = filas.Cells(2).Text
        Dim usuario As String = Session("id_usr")
        ResumenPdf(numerotarjeta, usuario, filas.Cells(8).Text)
    End Sub

    Public Function ResumenPdf(ByVal numerotarjeta As Integer, ByVal usuario As String, ByVal Asistente As String) As String
        Dim strRutaArchivoReservacion As String = Server.MapPath("~/privado/pdf/")
        Dim strNombreArchivoReservacionPdf As String = "LIQUIDACION" & numerotarjeta & ".PDF"

        Dim dtEncabezado As DataTable
        Dim Encabezado As String = "call Boletos.sp_tarjetatalonario_entregafacturas('Encabezado'," & numerotarjeta & ");"
        dtEncabezado = DatosSql.cargar_cuadrecaja(Encabezado)

        Dim dtDetalle As DataTable
        Dim Detalle As String = "call Boletos.sp_tarjetatalonario_entregafacturas('Detalle'," & numerotarjeta & ");"
        dtDetalle = DatosSql.cargar_cuadrecaja(Detalle)

        '1- Creo document y writer (con writer escribo el archivo en disco duro): 

        Dim doc As Document = New Document
        Dim writer As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(strRutaArchivoReservacion & "\" & strNombreArchivoReservacionPdf, FileMode.Create))

        'Dummy data for Invoice (Bill).
        Dim companyName As String = ""
        Dim orderNo As Integer = 2303

        Using sw As New StringWriter()
            Using hw As New HtmlTextWriter(sw)
                Dim sb As New StringBuilder()

                'Generate Invoice (Bill) Header.

                sb.Append("<table widtEncabezadoh='100%' cellspacing='5' cellpadding='2' border= '1' >   ")

                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>" & dtEncabezado.Rows(0).Item("nom_empsa").ToString & "</b></td></tr>")
                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>" & dtEncabezado.Rows(0).Item("nom_agc").ToString & "</b></td></tr>")
                sb.Append("<tr><td colspan = '2'></td></tr>")
                sb.Append("<tr><td><b>Tarjeta No: </b>" & dtEncabezado.Rows(0).Item("numerotarjeta").ToString & "</td><td align = 'right'><b>Fecha y Hora de Impresión: </b> </td></tr>  ")
                sb.Append("<tr><td colspan = '2'></td></tr>")

                sb.Append("<tr><td><b>Piloto: </b>")
                sb.Append(dtEncabezado.Rows(0).Item("nombrepiloto").ToString)
                sb.Append("</td><td align = 'right'><b></b>")
                sb.Append(Date.Now)
                sb.Append("</td></tr>")


                sb.Append("<tr><td><b>Usuario:</b>")
                sb.Append(dtEncabezado.Rows(0).Item("id_usr").ToString)
                sb.Append("</td><td align = 'right'><b>Asistente:</b>")
                sb.Append(Asistente)
                sb.Append("</td></tr>")

                sb.Append("</table>")
                sb.Append("<br />")

                sb.Append("<table widtEncabezadoh='100%' cellspacing='5' cellpadding='2' border= '1' >   ")
                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Asignacion de Facturas</b></td></tr>")
                sb.Append("</table>")

                'Generate Invoice (Bill) Items Grid.
                sb.Append("<table border = '1'>")
                sb.Append("<tr>")

                For Each column As DataColumn In dtDetalle.Columns
                    'sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>")
                    sb.Append("<th>")

                    sb.Append(column.ColumnName)
                    sb.Append("</th>")
                Next
                sb.Append("</tr>")
                For Each row As DataRow In dtDetalle.Rows
                    sb.Append("<tr>")
                    For Each column As DataColumn In dtDetalle.Columns
                        sb.Append("<td>")
                        sb.Append(row(column))
                        sb.Append("</td>")
                    Next
                    sb.Append("</tr>")
                Next

                sb.Append("<tr><td align = 'right' colspan = '")
                sb.Append(dtDetalle.Columns.Count - 1)
                sb.Append("'>Facturas</td>")
                sb.Append("<td>")
                sb.Append(dtDetalle.Compute("sum(facturas)", ""))
                sb.Append("</td>")
                sb.Append("</tr></table>")

                ' aqui inicia la liquidacion

                Encabezado = "call Boletos.sp_tarjetatalonario_liquidarfacturas('Encabezado'," & numerotarjeta & ");"

                dtEncabezado = DatosSql.cargar_cuadrecaja(Encabezado)


                Detalle = "call Boletos.sp_tarjetatalonario_liquidarfacturas('Detalle'," & numerotarjeta & ");"

                dtDetalle = DatosSql.cargar_cuadrecaja(Detalle)

                '1- Creo document y writer (con writer escribo el archivo en disco duro): 

                sb.Append("<table widtEncabezadoh='100%' cellspacing='5' cellpadding='2' border= '1' >   ")
                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Liquidación Facturas </b></td></tr>")
                sb.Append("</tr></table>")


                'Generate Invoice (Bill) Items Grid.
                sb.Append("<table border = '1'>")
                sb.Append("<tr>")

                For Each column As DataColumn In dtDetalle.Columns
                    'sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>")
                    sb.Append("<th>")

                    sb.Append(column.ColumnName)
                    sb.Append("</th>")
                Next
                sb.Append("</tr>")
                For Each row As DataRow In dtDetalle.Rows
                    sb.Append("<tr>")
                    For Each column As DataColumn In dtDetalle.Columns
                        sb.Append("<td>")
                        sb.Append(row(column))
                        sb.Append("</td>")
                    Next
                    sb.Append("</tr>")
                Next

                sb.Append("<tr><td align = 'right' colspan = '")
                sb.Append(dtDetalle.Columns.Count - 1)
                sb.Append("'>Total</td>")
                sb.Append("<td>")
                sb.Append(dtDetalle.Compute("sum(valor)", ""))
                sb.Append("</td>")
                sb.Append("</tr></table>")


                ' aqui finaliza la liquidacion

                'Export HTML String as PDF.
                Dim sr As New StringReader(sb.ToString())
                Dim htmlparser As New HTMLWorker(doc)

                doc.Open()
                htmlparser.Parse(sr)
                doc.Close()

                Response.Redirect("~/privado/pdf/" + strNombreArchivoReservacionPdf)

            End Using
        End Using

    End Function


End Class
