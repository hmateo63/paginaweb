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

Partial Class privado_ResumenDeTarjetas
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

            'cargar_Combo(DropDestino, "select 0 id_agencia,  'TODAS' nombre_agencia    union select id_agc as id_agencia,nom_agc as nombre_agencia from Facturacion.agencia where id_empsa=" & Session("id_empsa") & " order by id_agencia;")
            cargar_Combo(DropDestino, "select 0 id_destino,  'TODOS' nombre_agencia    union select id_destino,nom_destino as nombre_agencia from Encomiendas.destino order by id_destino;")
            'SELECT id_destino, nom_destino, id_mpioD, nemonico, destino_buses, destino_express, destino_agencia, recargo_especial, recargo_plus, recargo_clase, id_agencia FROM Encomiendas.destino;
            cargar_Combo(DropVehiculos, "select 0 id_vehiculo, 'TODOS' nombre_vehiculo union select id_veh as id_vehiculo,id_veh as nombre_vehiculo from Encomiendas.vehiculo where id_veh REGEXP '^[0-9]*[.]?[0-9]+$' and id_empsa=" & Session("id_empsa") & "   order by id_vehiculo;")
            cargar_Combo(DropPilotos, "select 0 as id_piloto, 'TODOS' nombre_piloto    union select id_emp as id_piloto, nom_emp as nombre_piloto from Encomiendas.empleado where id_pso=1  and id_empsa=" & Session("id_empsa") & " order by id_piloto;")
            cargar_Combo(DropServicios, "SELECT 0 as id_servicio ,'TODOS' nombre_servicio      union select idservicio as id_servicio,descripcion as nombre_servicio from Boletos.servicio where  id_empsa=" & Session("id_empsa") & " order by id_servicio;")

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
        Response.AddHeader("Content-Disposition", "attachment;filename=ResumenDeTarjetas.xls")
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

        Dim Destino As String = If(DropDestino.SelectedValue = 0, "%", DropDestino.SelectedValue)
        Dim Servicio As String = If(DropServicios.SelectedValue = 0, "%", DropServicios.SelectedValue)
        Dim Vehiculo As String = If(DropVehiculos.SelectedValue = 0, "%", DropVehiculos.SelectedValue)
        Dim Piloto As String = If(DropPilotos.SelectedValue = 0, "%", DropPilotos.SelectedValue)

        Dim strConnString As String = ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
        Using con As New MySqlConnection(strConnString)
            Using cmd As New MySqlCommand("call Boletos.sp_resumen_tarjetas_ruta_x_empresa('" & FechaInicial & "','" & FechaFinal & "','" & Destino & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "'," & Session("id_empsa") & ");")

                Dim pare As String = ""



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

    Public Function ResumenTarjetaPdf2(ByVal numerotarjeta As Integer, ByVal usuario As String) As String
        Dim strRutaArchivoReservacion As String = Server.MapPath("~/privado/pdf/")
        Dim strNombreArchivoReservacionPdf As String = "TARJETA" & numerotarjeta & ".PDF"
        Dim iTieneVales As Integer = 0
        Dim ArregloCarreras As List(Of Carrera) = New List(Of Carrera)
        Dim IdDetaHorario As Integer = 0, tipoboleto As Integer = 0
        Dim BoletosRuta As Integer = 0
        Dim BoletosAgencia As Integer = 0
        Dim TotalHorarioRuta As Integer = 0
        Dim TotalHorarioAgencia As Integer = 0

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle, TablaVales As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 12, 10, 12, 7)
                doc.SetPageSize(PageSize.A4)

                Dim Correlativo As Integer = NumeroAleatorioPdf()

                Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(strRutaArchivoReservacion & "\" & strNombreArchivoReservacionPdf, FileMode.Create))

                doc.AddTitle("TARJETA DE RUTA " + Correlativo.ToString)
                doc.AddAuthor("LINEAS TERRESTRES GUATEMALTECAS S.A")
                doc.AddCreationDate()

                doc.Open()

                Dim Tabla As PdfPTable = New PdfPTable(7)
                Tabla.TotalWidth = 550
                Tabla.LockedWidth = True
                Tabla.SetWidths({13, 13, 13, 13, 13, 13, 13})
                Dim Celda As PdfPCell = New PdfPCell
                Dim informacion As String = ""

                Dim contador As Integer = 0

                For contador = 0 To 1

                    doc.NewPage()

                    If contador = 0 Then


                        Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,ucase(a.nom_agc) agencia " +
                                                                "   from Facturacion.usuario u " +
                                                                "       INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                                "       INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa     " +
                                                                "           where u.id_usr='" & usuario & "'; "

                        Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                        Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                        TablaDatosEmpresa.Load(readerUsuario)


                        Dim SQLConsulta As String = "call Boletos.sp_tarjeta_ruta_detalle(" & numerotarjeta & ");"
                        command = New MySqlCommand(SQLConsulta, connection)
                        Dim reader As MySqlDataReader = command.ExecuteReader()
                        TablaDetalle.Load(reader)
                        reader.Close()


                        SQLConsulta = "call Boletos.sp_resumen_vales(" & numerotarjeta & ");"
                        command = New MySqlCommand(SQLConsulta, connection)
                        reader = command.ExecuteReader()
                        TablaVales.Load(reader)

                        ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''


                        Celda = New PdfPCell(New Paragraph("EMPRESA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("empresa").ToString), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.Colspan = 4
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("TARJETA #.:", FontFactory.GetFont("Palatino Linotype", 15, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("AGENCIA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(TablaDatosEmpresa.Rows(0).Item("agencia").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)


                        Celda = New PdfPCell(New Paragraph("USUARIO:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(UCase(usuario), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(numerotarjeta.ToString, FontFactory.GetFont("Palatino Linotype", 20, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                        Celda.Colspan = 2
                        Celda.Rowspan = 2
                        Tabla.AddCell(Celda)


                        Celda = New PdfPCell(New Paragraph("BUS #  :", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(TablaDetalle.Rows(0).Item("Vehiculo").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("ESTADO #  ", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(TablaDetalle.Rows(0).Item("idtarjetaestado").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)


                        Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.Colspan = 7
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)


                        Dim Control = 0
                        Dim cantidad_boletos As Integer = 0
                        Dim cantidad_boletos_ruta As Integer = 0
                        Dim monto_boletos As String = 0
                        Dim monto_boletos_ruta As String = 0
                        Dim total_cantidad_boletos As Integer = 0
                        Dim monto_total As String = 0

                        Dim elemento As New Carrera
                        Dim NumeroFilasRuta As Integer = 0
                        If TablaDetalle.Rows.Count > 0 Then

                            For fila = 0 To TablaDetalle.Rows.Count - 1


                                If IdDetaHorario <> TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString Then


                                    If fila > 1 Then
                                        Celda = New PdfPCell(New Paragraph(" SUBTOTAL " & TablaDetalle.Rows(fila - 1).Item("TotalHorarioTipoBoleto"), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                        Celda.BorderWidth = 1
                                        Celda.Colspan = 7
                                        Tabla.AddCell(Celda)
                                        BoletosAgencia = 1


                                        Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   |   " & " TOTAL CARGA " & TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                        '   Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))

                                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(192, 192, 192)
                                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                        Celda.BorderWidth = 0
                                        Celda.BorderWidthBottom = 1
                                        Celda.Colspan = 7
                                        Tabla.AddCell(Celda)
                                        BoletosAgencia = 1

                                    End If

                                    BoletosAgencia = 0
                                    BoletosRuta = 0

                                    TotalHorarioAgencia = 0
                                    TotalHorarioRuta = 0

                                    Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.Colspan = 7
                                    Celda.BorderWidth = 0
                                    Tabla.AddCell(Celda)

                                    IdDetaHorario = TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString

                                    informacion = "FECHA VIAJE:"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1

                                    Tabla.AddCell(Celda)

                                    informacion = "HORA VIAJE:"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1

                                    Tabla.AddCell(Celda)

                                    informacion = "RUTA"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Celda.Colspan = 2
                                    Tabla.AddCell(Celda)

                                    informacion = "SERVICIO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "PILOTO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "ASISTENTE"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    ''==============SEGUNDA FILA====================''

                                    informacion = TablaDetalle.Rows(fila).Item("FechaViaje")
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Hora").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1

                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Ruta").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Celda.Colspan = 2
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Servicio").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Piloto").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Asistente").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)


                                    ''=============TERCERA FILA===============''
                                    informacion = "SERIE"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "FACTURA"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "BOLETO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "TOTAL"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "CREACION"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "AGENCIA"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "USUARIO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)


                                    For columna = 0 To TablaDetalle.Columns.Count - 1

                                        If columna > 7 And columna < 15 Then

                                            If TablaDetalle.Rows(fila).Item("item") = 2 Then

                                                If BoletosAgencia = 0 Then
                                                    Celda = New PdfPCell(New Paragraph(" BOLETOS VENDIDOS EN AGENCIA ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(211, 211, 211)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 0
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosAgencia = 1
                                                End If

                                                informacion = TablaDetalle.Rows(fila).Item(columna).ToString
                                                Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                                                Celda.BorderWidth = 1
                                                Tabla.AddCell(Celda)

                                            Else

                                                If BoletosRuta = 0 Then

                                                    Celda = New PdfPCell(New Paragraph(" BOLETOS VENDIDOS EN RUTA *** ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(211, 211, 211)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 0
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosRuta = 1
                                                    TotalHorarioAgencia = 0

                                                End If

                                                informacion = TablaDetalle.Rows(fila).Item(columna).ToString
                                                Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                                                Celda.BorderWidth = 1
                                                Tabla.AddCell(Celda)

                                            End If

                                        End If

                                    Next

                                Else
                                    IdDetaHorario = TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString
                                    For columna = 0 To TablaDetalle.Columns.Count - 1

                                        If columna > 7 And columna < 15 Then


                                            If TablaDetalle.Rows(fila).Item("item").ToString = 2 Then

                                                If BoletosAgencia = 0 Then


                                                    Celda = New PdfPCell(New Paragraph(" BOLETOS VENDIDOS EN AGENCIA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(211, 211, 211)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 0
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosAgencia = 1
                                                End If

                                                informacion = TablaDetalle.Rows(fila).Item(columna)
                                                Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                                                Celda.BorderWidth = 1
                                                Tabla.AddCell(Celda)

                                            ElseIf TablaDetalle.Rows(fila).Item("item").ToString = 3 Then

                                                If BoletosRuta = 0 Then

                                                    Celda = New PdfPCell(New Paragraph(" SUBTOTAL " & TablaDetalle.Rows(fila - 1).Item("TotalHorarioTipoBoleto"), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 1
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosAgencia = 1


                                                    Celda = New PdfPCell(New Paragraph(" BOLETOS VENDIDOS EN RUTA **** ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(211, 211, 211)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 0
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosRuta = 1
                                                    TotalHorarioAgencia = 0


                                                End If

                                                informacion = TablaDetalle.Rows(fila).Item(columna)
                                                Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                                                Celda.BorderWidth = 1
                                                Tabla.AddCell(Celda)

                                                If fila = TablaDetalle.Rows.Count - 1 And columna = 14 Then

                                                    Celda = New PdfPCell(New Paragraph(" SUBTOTAL " & TablaDetalle.Rows(fila - 1).Item("TotalHorarioTipoBoleto"), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 1
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)


                                                    Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   |   " & " TOTAL CARGA " & TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                                    'Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))


                                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(192, 192, 192)
                                                    Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                                    Celda.BorderWidth = 0
                                                    Celda.BorderWidthBottom = 1
                                                    Celda.Colspan = 7
                                                    Tabla.AddCell(Celda)
                                                    BoletosAgencia = 1

                                                End If

                                            End If

                                        End If

                                    Next

                                End If

                            Next

                            Celda = New PdfPCell(New Paragraph("   TOTAL BOLETOS AGENCIA 1 :  " & TablaDetalle.Rows(0).Item("TotalBoletosAgencia") & "   |   TOTAL BOLETOS EN RUTA : " & TablaDetalle.Rows(0).Item("TotalBoletosRuta") & "   |   TOTAL CARGA : " & TablaDetalle.Rows(0).Item("TotalCarga") & "   ||    TOTAL TARJETA DE RUTA : " & TablaDetalle.Rows(0).Item("TotalTarjeta"), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(176, 176, 176)
                            Celda.HorizontalAlignment = Element.ALIGN_CENTER
                            Celda.BorderWidth = 1
                            Celda.Colspan = 7
                            Tabla.AddCell(Celda)
                            BoletosAgencia = 1


                            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                            Celda.Colspan = 7
                            Celda.BorderWidth = 0
                            Tabla.AddCell(Celda)

                        End If
                        doc.Add(Tabla)
                        reader.Close()
                    End If




                    If contador = 1 Then

                        Tabla.DeleteBodyRows()

                        Celda = New PdfPCell(New Paragraph("EMPRESA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("empresa").ToString), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.Colspan = 4
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("TARJETA #.:", FontFactory.GetFont("Palatino Linotype", 15, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("BUS #  :", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(TablaDetalle.Rows(0).Item("Vehiculo").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph("ESTADO #  ", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(TablaDetalle.Rows(0).Item("idtarjetaestado").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(numerotarjeta.ToString, FontFactory.GetFont("Palatino Linotype", 18, iTextSharp.text.Font.BOLD)))
                        Celda.BorderWidth = 0
                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                        Celda.Colspan = 2
                        Tabla.AddCell(Celda)

                        Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 2, iTextSharp.text.Font.BOLD)))
                        Celda.Colspan = 7
                        Celda.BorderWidth = 0
                        Tabla.AddCell(Celda)

                        Dim Control = 0
                        Dim cantidad_boletos As Integer = 0
                        Dim cantidad_boletos_ruta As Integer = 0
                        Dim monto_boletos As String = 0
                        Dim monto_boletos_ruta As String = 0
                        Dim total_cantidad_boletos As Integer = 0
                        Dim monto_total As String = 0

                        Dim elemento As New Carrera
                        Dim NumeroFilasRuta As Integer = 0
                        If TablaDetalle.Rows.Count > 0 Then

                            For fila = 0 To TablaDetalle.Rows.Count - 1

                                If IdDetaHorario <> TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString Then

                                    If fila > 1 Then

                                        Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   |   " & " TOTAL CARGA " & TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                        '   Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))

                                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                        Celda.BorderWidth = 0
                                        Celda.BorderWidthBottom = 1
                                        Celda.Colspan = 7
                                        Tabla.AddCell(Celda)
                                        BoletosAgencia = 1

                                    End If


                                    BoletosAgencia = 0
                                    BoletosRuta = 0

                                    TotalHorarioAgencia = 0
                                    TotalHorarioRuta = 0


                                    IdDetaHorario = TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString

                                    iTieneVales = 1


                                    informacion = "FECHA VIAJE :"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1

                                    Tabla.AddCell(Celda)

                                    informacion = "HORA VIAJE:"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1

                                    Tabla.AddCell(Celda)

                                    informacion = "RUTA"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Celda.Colspan = 2
                                    Tabla.AddCell(Celda)

                                    informacion = "SERVICIO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "PILOTO"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = "ASISTENTE"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    ''==============SEGUNDA FILA====================''


                                    informacion = TablaDetalle.Rows(fila).Item("FechaViaje")
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Hora").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Ruta").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Celda.Colspan = 2
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Servicio").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Piloto").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("Asistente").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)


                                    '==============================Tercera fila==========================='
                                    informacion = "VIATICOS"
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                                    Celda.BorderWidth = 1
                                    Celda.Colspan = 5
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("ViaticosPiloto").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                    informacion = TablaDetalle.Rows(fila).Item("ViaticosAsistente").ToString
                                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                                    Celda.BorderWidth = 1
                                    Tabla.AddCell(Celda)

                                Else
                                    IdDetaHorario = TablaDetalle.Rows(fila).Item("Id_detaHorarios").ToString
                                    For columna = 0 To TablaDetalle.Columns.Count - 1



                                        If fila = TablaDetalle.Rows.Count - 1 And columna = 14 Then

                                            'If BoletosAgencia = 0 Then

                                            Celda = New PdfPCell(New Paragraph(" TOTAL BOLETOS " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") & "   |   " & " TOTAL CARGA " & TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") & "   ||    TOTAL TURNO " & TablaDetalle.Rows(fila - 1).Item("TotalHorario") * 1 + TablaDetalle.Rows(fila - 1).Item("TotalCargaHorario") * 1, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)))
                                            'End If

                                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                                            Celda.HorizontalAlignment = Element.ALIGN_CENTER
                                            Celda.BorderWidth = 0
                                            Celda.BorderWidthBottom = 1
                                            Celda.Colspan = 7
                                            Tabla.AddCell(Celda)
                                            BoletosAgencia = 1


                                        End If

                                    Next

                                End If

                            Next


                            Celda = New PdfPCell(New Paragraph("   TOTAL BOLETOS AGENCIA 2:  " & TablaDetalle.Rows(0).Item("TotalBoletosAgencia") & "   |   TOTAL BOLETOS EN RUTA : " & TablaDetalle.Rows(0).Item("TotalBoletosRuta") & "   |   TOTAL CARGA : " & TablaDetalle.Rows(0).Item("TotalCarga") & "   ||    TOTAL TARJETA DE RUTA : " & TablaDetalle.Rows(0).Item("TotalTarjeta"), FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                            Celda.HorizontalAlignment = Element.ALIGN_CENTER
                            Celda.BorderWidth = 1
                            Celda.Colspan = 7
                            Tabla.AddCell(Celda)

                            BoletosAgencia = 1


                            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 3, iTextSharp.text.Font.BOLD)))
                            Celda.Colspan = 7
                            Celda.BorderWidth = 0
                            Tabla.AddCell(Celda)

                        End If

                        Celda = New PdfPCell(New Paragraph("   VALES ASOCIADOS A LA TARJETA:  ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(176, 176, 176)
                        Celda.HorizontalAlignment = Element.ALIGN_CENTER
                        Celda.BorderWidth = 1
                        Celda.Colspan = 7
                        Tabla.AddCell(Celda)
                        BoletosAgencia = 1

                        ''=============TERCERA FILA===============''
                        informacion = "ID VALE"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = "AGENCIA"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = "USUARIO"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = "TOTAL B. RUTA"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = " TOTAL CARGA "
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = "TOTAL EFECTIVO"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)

                        informacion = "TOTAL VALE"
                        Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                        Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                        Celda.BorderWidth = 1
                        Tabla.AddCell(Celda)


                        ' aqui inserto la pagina que onda
                        If iTieneVales = 0 Then


                            informacion = "FECHA VIAJE :"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1

                            Tabla.AddCell(Celda)

                            informacion = "HORA VIAJE:"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1

                            Tabla.AddCell(Celda)

                            informacion = "RUTA"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1
                            Celda.Colspan = 2
                            Tabla.AddCell(Celda)

                            informacion = "SERVICIO"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = "PILOTO"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = "ASISTENTE"
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BackgroundColor = New iTextSharp.text.BaseColor(195, 199, 200)
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            ''==============SEGUNDA FILA====================''



                            informacion = TablaDetalle.Rows(0).Item("FechaViaje")
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = TablaDetalle.Rows(0).Item("Hora").ToString
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = TablaDetalle.Rows(0).Item("Ruta").ToString
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Celda.Colspan = 2
                            Tabla.AddCell(Celda)

                            informacion = TablaDetalle.Rows(0).Item("Servicio").ToString
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = TablaDetalle.Rows(0).Item("Piloto").ToString
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)

                            informacion = TablaDetalle.Rows(0).Item("Asistente").ToString
                            Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL)))
                            Celda.BorderWidth = 1
                            Tabla.AddCell(Celda)



                            ' fin que onda
                        End If


                        doc.Add(Tabla)


                        ''''''''''''AGREGAMOS NUEVA TABLA PARA LAS FIRMAS DE LOS ENCARGADOS'''''''''''
                        Dim TablaFirmas As PdfPTable = New PdfPTable(2)

                        TablaFirmas.TotalWidth = 550
                        TablaFirmas.LockedWidth = True
                        TablaFirmas.SetWidths({30, 30})

                        Dim CeldaFirmas As PdfPCell = New PdfPCell

                        CeldaFirmas = New PdfPCell(New Paragraph(""))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)
                        CeldaFirmas = New PdfPCell(New Paragraph("", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)
                        CeldaFirmas = New PdfPCell(New Paragraph("", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)
                        CeldaFirmas = New PdfPCell(New Paragraph(""))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)
                        CeldaFirmas = New PdfPCell(New Paragraph("", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)
                        CeldaFirmas = New PdfPCell(New Paragraph("", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        CeldaFirmas.Colspan = 2
                        TablaFirmas.AddCell(CeldaFirmas)


                        CeldaFirmas = New PdfPCell(New Paragraph("F:_______________________", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        TablaFirmas.AddCell(CeldaFirmas)

                        CeldaFirmas = New PdfPCell(New Paragraph("F:_______________________", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        TablaFirmas.AddCell(CeldaFirmas)

                        CeldaFirmas = New PdfPCell(New Paragraph("ENTREGA", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        TablaFirmas.AddCell(CeldaFirmas)

                        CeldaFirmas = New PdfPCell(New Paragraph("RECIBE", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                        CeldaFirmas.BorderWidth = 0
                        TablaFirmas.AddCell(CeldaFirmas)

                        doc.Add(TablaFirmas)
                    End If

                Next

                doc.Close()

                Response.Redirect("~/privado/pdf/" + strNombreArchivoReservacionPdf)


            Catch ex As Exception
                Return "ERROR: " + ex.Message

            End Try

        End Using

    End Function



    Protected Sub Button3_Click(sender As Object, e As System.EventArgs) Handles Button3.Click
        DivGridView1.Visible = False
        DivArea1.Visible = True



    End Sub


    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
        filas = GridView1.SelectedRow
        Dim numerotarjeta As Integer = filas.Cells(2).Text
        Dim usuario As String = Session("id_usr")

        ResumenTarjetaPdf2(numerotarjeta, usuario)

    End Sub
End Class
