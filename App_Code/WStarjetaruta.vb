Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Data
Imports System.Drawing


'Imports iTextSharp.text.pdf.Barcode
'Imports iTextSharp.text.pdf.PdfWriter
'Imports System.Configuration
Imports iTextSharp.text.html.simpleparser



Imports iTextSharp.text.html

Imports System.Configuration
Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System
Imports System.Web.Security
Imports System.Web.UI.WebControls.WebParts


' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WStarjetaruta
    Inherits System.Web.Services.WebService


    Public Shared Function ActualizaTransaccionGface(ByVal Serie As String, ByVal Correlativo As String, ByVal Cae As String, ByVal Face As String, ByVal IdTransaccion As String) As String
        Dim PosAlmacenado As String = "", PosActualizado As String = ""
        Dim strsql As String = ""
        Dim comando As MySqlCommand
        Dim estado As String

        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)

            strsql = "update Facturacion.transaccionesgface set num_fac=" & Correlativo & ",cae='" & Cae & "',face='" & Face & "', factura_detalle=replace(factura_detalle,'new.num_fac','" & Correlativo & "'),estatus_factura=true where idtransaccion='" & IdTransaccion & "';"

            comando = New MySqlCommand(strsql, conexion)


            Try
                conexion.Open()

                estado = comando.ExecuteNonQuery

                If estado >= 1 Then
                    Return "Actualizada"
                Else
                    Return "Error"
                End If

            Catch ex As MySqlException

                strsql = "update Facturacion.transaccionesgface set error='" & Replace(ex.Message.ToString, "'", "") & "' where idtransaccion='" & IdTransaccion & "';"
                comando = New MySqlCommand(strsql, conexion)
                estado = comando.ExecuteNonQuery

                Return ex.Message.ToString

            Finally
                conexion.Close()
            End Try
        End Using
    End Function

    Public Function ConvierteFecha(ByVal Entrada As String) As Date


        Dim fecha As Date
        Dim dia, mes, año As String
        año = (Left(Entrada, 4))
        mes = Right(Left(Entrada, 6), 2)
        dia = (Right(Entrada, 2))
        fecha = año + "-" + mes + "-" + dia
        Return fecha

    End Function

    Public Class ClaseTabla
        Public fila As String
    End Class

    Public Class ClaseResumenTarjetas
        Public fechaviaje As String
        Public numerotarjeta As String
        Public vehiculo As String
        Public horarios As String
        Public servicio As String
        Public ruta As String
        Public piloto As String
        Public asistente As String
        Public agencia As String
        Public usuario As String
        Public totalruta As String
        Public totalagencia As String
        Public totalcarga As String
        Public totaltarjeta As String
    End Class



    <WebMethod()> _
    Public Function ImprimeResumen(ByVal FechaInicial As String, ByVal FechaFinal As String, ByVal Agencia As String, ByVal Servicio As String, ByVal Vehiculo As String, ByVal Piloto As String, ByVal Usuario As String) As String
        FechaFinal = FechaFinal.ToString
        FechaInicial = FechaInicial.ToString
        Agencia = Agencia.ToString
        Servicio = Servicio.ToString
        Vehiculo = Vehiculo.ToString
        Piloto = Piloto.ToString

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Try
                Dim TablaDetalle As New DataTable
                Dim SQLConsulta As String = "call Boletos.sp_resumen_tarjetas_ruta('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "');"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)

                'Dim informacion As String = ""
                Dim GridView1 As New GridView
                GridView1.AllowPaging = False
                GridView1.DataSource = TablaDetalle
                GridView1.DataBind()





                Context.Response.Clear()
                Context.Response.Buffer = True
                Context.Response.AddHeader("content-disposition", "attachment;filename=Reporte.csv")
                Context.Response.Charset = ""
                Context.Response.ContentType = "application/vnd.ms-excel"
                Using sw As New StringWriter()
                    Dim hw As New HtmlTextWriter(sw)

                    GridView1.HeaderRow.BackColor = Color.White

                    For Each row As GridViewRow In GridView1.Rows
                        row.BackColor = Color.White
                        For Each cell As TableCell In row.Cells
                            cell.CssClass = "textmode"
                        Next
                    Next

                    GridView1.RenderControl(hw)
                    'style to format numbers to string
                    Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
                    Context.Response.Write(style)
                    Context.Response.Output.Write(sw.ToString())
                    Context.Response.Flush()
                    Context.Response.End()
                End Using

                Return "Reporte.csv"

            Catch ex As Exception
                Return "Error" + ex.ToString
            End Try

        End Using

    End Function




    <WebMethod()> _
    Public Function ResumenTarjetaPdf(ByVal numerotarjeta As Integer, ByVal usuario As String) As String

        Dim iTieneVales As Integer = 0


        numerotarjeta = numerotarjeta.ToString

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
                Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\privado\pdf\Tarjeta de Ruta -" + Correlativo.ToString + ".pdf"), FileMode.Create))
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
                                                                "from Facturacion.usuario u " +
                                                                "INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                                "INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc " +
                                                                "where u.id_usr='" & usuario & "'; "

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

                                    'Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 11, iTextSharp.text.Font.BOLD)))
                                    'Celda.Colspan = 7
                                    'Celda.BorderWidth = 0
                                    'Tabla.AddCell(Celda)

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















                        'For fila = 0 To TablaVales.Rows.Count - 1
                        '    Dim color As iTextSharp.text.BaseColor = New iTextSharp.text.BaseColor(Drawing.Color.White)
                        '    Dim tipoletra As iTextSharp.text.Font = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)

                        '    If Left(TablaVales.Rows(fila).Item("id_vale"), 5) = "TOTAL" Then
                        '        TablaVales.Rows(fila).Item("agencia") = ""
                        '        TablaVales.Rows(fila).Item("usuario") = ""
                        '        color = New iTextSharp.text.BaseColor(240, 240, 240)
                        '        tipoletra = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)
                        '    Else
                        '        color = New iTextSharp.text.BaseColor(Drawing.Color.White)
                        '        tipoletra = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL)
                        '    End If


                        '    informacion = TablaVales.Rows(fila).Item("id_vale")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("agencia")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("usuario")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("total_boletos_ruta")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("total_carga")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("total_efectivo")
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        '    informacion = TablaVales.Rows(fila).Item("total_vale").ToString
                        '    Celda = New PdfPCell(New Paragraph(informacion, tipoletra))
                        '    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                        '    Celda.BackgroundColor = color
                        '    Celda.BorderWidth = 1
                        '    Tabla.AddCell(Celda)

                        'Next

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

                Return "pdf/Tarjeta de Ruta -" + Correlativo.ToString + ".pdf"

            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    <WebMethod()> _
    Public Function ValePdf(ByVal numerotarjeta As Integer, ByVal numerovale As Integer, ByVal TotalBoletosRuta As Decimal, ByVal TotalEfectivo As Decimal, ByVal TotalCarga As Decimal, ByVal TotalVale As Decimal, ByVal usuario As String) As String

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

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 10, 10, 20, 10)
                doc.SetPageSize(PageSize.LETTER)

                Dim Correlativo As Integer = NumeroAleatorioPdf()
                Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\privado\pdf\VALE-" + Correlativo.ToString + ".pdf"), FileMode.Create))
                doc.AddTitle("VALE " + Correlativo.ToString)
                doc.AddAuthor("LINEAS TERRESTRES GUATEMALTECAS S.A")
                doc.AddCreationDate()

                doc.Open()

                Dim Tabla As PdfPTable = New PdfPTable(7)

                Tabla.TotalWidth = 550
                Tabla.LockedWidth = True
                Tabla.SetWidths({18, 13, 13, 13, 13, 13, 13})


                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia " +
                                                        "from Facturacion.usuario u " +
                                                        "INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc " +
                                                        "where u.id_usr='" & usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta(" & numerotarjeta & ");"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)

                'DECLARACION DE LA CELDA A UTILIZAR EN LA TABLAPDF
                Dim Celda As PdfPCell = New PdfPCell

                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Celda = New PdfPCell(New Paragraph("EMPRESA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("empresa").ToString), FontFactory.GetFont("Palatino Linotype", 13, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Celda.Colspan = 4
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph("VALE RUTA #", FontFactory.GetFont("Palatino Linotype", 15, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Celda.HorizontalAlignment = Element.ALIGN_CENTER
                Celda.Colspan = 2
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph("AGENCIA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("agencia").ToString), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Celda.Colspan = 2
                Tabla.AddCell(Celda)


                Celda = New PdfPCell(New Paragraph("USUARIO:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(usuario), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(numerovale.ToString, FontFactory.GetFont("Palatino Linotype", 20, iTextSharp.text.Font.BOLD)))
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
                Celda.Colspan = 4
                Tabla.AddCell(Celda)


                Celda = New PdfPCell(New Paragraph("   ", FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                Celda.Colspan = 7
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Dim informacion As String = ""
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



                    BoletosAgencia = 0
                    BoletosRuta = 0

                    TotalHorarioAgencia = 0
                    TotalHorarioRuta = 0

                    informacion = "FECHA VIAJE:"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)

                    informacion = "HORA VIAJE:"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)

                    informacion = "PILOTO"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = "ASISTENTE"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    ''==============SEGUNDA FILA====================''


                    informacion = TablaDetalle.Rows(0).Item("FechaViaje")
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = TablaDetalle.Rows(0).Item("Hora").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)


                    informacion = TablaDetalle.Rows(0).Item("Piloto").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.Colspan = 2
                    Tabla.AddCell(Celda)

                    informacion = TablaDetalle.Rows(0).Item("Asistente").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    ''=============TERCERA FILA===============''
                    informacion = "TOTAL B. RUTA"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    informacion = "TOTAL CARGA"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = "TOTAL EFECTIVO"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    informacion = "TOTAL VALE."
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)



                    informacion = TablaDetalle.Rows(0).Item("TotalBoletosRuta").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    informacion = Decimal.Round(TotalCarga, 2)
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = Decimal.Round(TotalEfectivo, 2)
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = TotalVale
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    '===================FIRMAS=================


                    informacion = " "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 7
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = " "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 7
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = "  ______________________________"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 3
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)

                    informacion = "   _____________________________"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 4
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = "            NOMBRE Y FIRMA PILOTO    "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 3
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)

                    informacion = "      NOMBRE Y FIRMA CAJERO  "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 4
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)







                End If
                doc.Add(Tabla)



                doc.Close()


                reader.Close()

                Return "pdf/VALE-" + Correlativo.ToString + ".pdf"


            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    <WebMethod()> _
    Public Function mostrarHorarios(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of HorarioResumen)


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As HorarioResumen
        Dim ArregloSerie As List(Of HorarioResumen) = New List(Of HorarioResumen)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    Consulta = "select tdh.idtarjetadetallehorarios," +
                                    "horarios.id_detaHorario datos," +
                                    "cast(concat(replace(hora.hora,':','.'), ' | ' , fecha_detaHorario , ' | ' ," +
                                    "bs.descripcion, ' | ' ," +
                                    "rod.nom_ruta, ' | ' ," +
                                    "'BUS-',v.id_veh, ' | ' ," +
                                    "'PIL. ' , piloto.nom_emp , ' | ' ," +
                                    "'ASIS. ' , asistente.nom_emp) as char) hora " +
                                    "from horarios " +
                                    "inner join vw_rutas_origen_destino rod " +
                                        "on horarios.id_ruta=rod.id_ruta " +
                                    "inner join Boletos.tarjetadetallehorarios tdh " +
                                        "on horarios.id_detaHorario=tdh.id_detaHorario " +
                                    "inner join hora " +
                                        "on horarios.id_horario=hora.id_horario " +
                                    "inner join Boletos.servicio bs " +
                                        "on bs.idservicio=horarios.idservicio " +
                                    "inner join vehiculo v " +
                                        "on v.id_veh=horarios.id_veh " +
                                   "inner join empleado piloto " +
                                        "on piloto.id_emp=horarios.id_emp " +
                                   "inner join empleado asistente " +
                                        "on asistente.id_emp=horarios.id_asistente " +
                                    "where horarios.id_detaHorario in(select id_detaHorario from Boletos.tarjetadetallehorarios where numerotarjeta=" & numerotarjeta & ") " +
                                    "order by hora.hora;"

                    comando.CommandText = Consulta
                    Dim LectorHorarios As MySqlDataReader = comando.ExecuteReader()

                    While LectorHorarios.Read()
                        Elemento = New HorarioResumen
                        Elemento.datos = LectorHorarios("datos")
                        Elemento.hora = LectorHorarios("hora")
                        ArregloSerie.Add(Elemento)
                    End While
                    LectorHorarios.Close()
                Else
                    LectorUsuario.Close()
                    Elemento = New HorarioResumen

                    ArregloSerie.Add(Elemento)
                End If
                transaccion.Commit()

            Catch ex As Exception
                Elemento = New HorarioResumen

                ArregloSerie.Add(Elemento)
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    Public Class ClaseDestinos
        Public iddestino As Integer
        Public nombredestino As String
    End Class

    <WebMethod(True)> _
    Public Function DestinosBoletos(ByVal usuario As String) As List(Of [ClaseDestinos])
        Dim Busqueda As String = "(select a.id_destino as codigodestino ,d.nom_destino as destino from Facturacion.agencia a INNER JOIN destino d ON a.id_destino=d.id_destino where a.id_agc in (select id_agc from Facturacion.usuario where id_usr='" & usuario & "') order by d.nom_destino limit 1)    " +
               " UNION ALL  (select id_destino as codigodestino,nom_destino as destino from destino where id_destino!=(select a.id_destino from Facturacion.agencia a    " +
               " INNER JOIN destino d ON a.id_destino=d.id_destino where a.id_agc in (select id_agc from Facturacion.usuario where id_usr='" & usuario & "' and id_empsa))  " +
               " and destino.destino_buses=1 and destino.destino_buses=1 order by nom_destino asc limit 200);"
        Dim result As List(Of [ClaseDestinos]) = New List(Of ClaseDestinos)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseDestinos

                    Elemento.iddestino = reader("codigodestino")
                    Elemento.nombredestino = reader("destino")
                    If Elemento.nombredestino = "Ciudad Capital" Or Elemento.nombredestino = "Centra Norte" Then
                        Elemento.nombredestino = "Guatemala"
                    End If


                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException

            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function TipoCobroRuta(ByVal usuario As String) As List(Of [ClaseDestinos])
        Dim Busqueda As String = "SELECT idtipocobroruta, descripcion FROM Boletos.tipocobroruta;"
        Dim result As List(Of [ClaseDestinos]) = New List(Of ClaseDestinos)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseDestinos

                    Elemento.iddestino = reader("idtipocobroruta")
                    Elemento.nombredestino = reader("descripcion")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException

            End Try
        End Using
        Return result
    End Function




    <WebMethod(True)> _
    Public Function CargarAgencias(ByVal usuario As String) As List(Of [ClaseAgencias])
        Dim fid_empsa = idEmpresa(usuario)
        'Dim Busqueda As String = "select id_agc as id_agencia,nom_agc as nombre_agencia from Facturacion.agencia where id_empsa=4 order by nom_agc;"
        Dim Busqueda As String = "select id_agc as id_agencia,nom_agc as nombre_agencia from Facturacion.agencia where id_empsa=" & fid_empsa & " order by id_agc;"
        Dim result As List(Of [ClaseAgencias]) = New List(Of ClaseAgencias)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseAgencias

                    Elemento.idagencia = reader("id_agencia")
                    Elemento.nombreagencia = reader("nombre_agencia")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    <WebMethod(True)> _
    Public Function CargarVehiculo(ByVal usuario As String) As List(Of [ClaseVehiculos])
        Dim fid_empsa = idEmpresa(usuario)
        ' sqlString = "SELECT id_veh  id_veh, id_veh descripcion FROM vehiculo WHERE id_veh REGEXP '^[0-9]*[.]?[0-9]+$' and id_empsa=" & Session("id_empsa") & ";"
        Dim Busqueda As String = "select id_veh as id_vehiculo,id_veh as nombre_vehiculo from Encomiendas.vehiculo where id_veh REGEXP '^[0-9]*[.]?[0-9]+$' and id_empsa=" & fid_empsa & "   order by id_veh;"
        Dim result As List(Of [ClaseVehiculos]) = New List(Of ClaseVehiculos)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseVehiculos

                    Elemento.idvehiculo = reader("id_vehiculo")
                    Elemento.nombrevehiculo = reader("nombre_vehiculo")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    <WebMethod(True)> _
    Public Function CargarPilotos(ByVal usuario As String) As List(Of [ClasePilotos])
        Dim fid_empsa = idEmpresa(usuario)

        Dim Busqueda As String = "select nom_emp as id_piloto, nom_emp as nombre_piloto from Encomiendas.empleado where id_pso=1  and id_empsa=" & fid_empsa & "   group by id_piloto order by nombre_piloto;"
        Dim result As List(Of [ClasePilotos]) = New List(Of ClasePilotos)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClasePilotos

                    Elemento.idpiloto = reader("id_piloto")
                    Elemento.nombrepiloto = reader("nombre_piloto")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function


    <WebMethod(True)> _
    Public Function CargarServicios(ByVal usuario As String) As List(Of [ClaseServicios])
        Dim fid_empsa = idEmpresa(usuario)
        Dim Busqueda As String = "select idservicio as id_servicio,descripcion as nombre_servicio from Boletos.servicio where  id_empsa=" & fid_empsa & " order by nombre_servicio;"
        Dim result As List(Of [ClaseServicios]) = New List(Of ClaseServicios)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseServicios

                    Elemento.idservicio = reader("id_servicio")
                    Elemento.nombreservicio = reader("nombre_servicio")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function


    <WebMethod()> _
    Public Function asignarHorario(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idhorario As Integer, ByVal vehiculo As String, ByVal fechaviaje As String) As List(Of HorarioResumen)

        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As HorarioResumen
        Dim ArregloSerie As List(Of HorarioResumen) = New List(Of HorarioResumen)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    Consulta = "SELECT if(count(id_detaHorario)>0,'SI','NO') registro  FROM Boletos.tarjetadetallehorarios where id_detaHorario = '" & idhorario & "';"
                    comando.CommandText = Consulta
                    Consulta = comando.ExecuteScalar

                    If Consulta = "NO" Then
                        Consulta = "insert into Boletos.tarjetadetallehorarios (numerotarjeta,id_detaHorario,fechacreacion,id_usr,id_agc,id_empsa,fecha_horario,id_horario,id_ruta,id_piloto,id_asistente) " +
                                    " select '" & numerotarjeta & "','" & idhorario & "',now(),'" & usuario & "','" & idagencia & "','" & idempresa & "',fecha_detaHorario,id_horario,id_ruta,id_emp,id_asistente " +
                                    " from Encomiendas.horarios where id_detaHorario =" & idhorario & ";"
                        comando.CommandText = Consulta
                        comando.ExecuteNonQuery()
                    End If


                    Consulta = "update Boletos.tarjeta set id_veh=" & vehiculo & " ,fechaviaje=" & "concat(Facturacion.fn_split('" & fechaviaje & "','/',3),'-',Facturacion.fn_split('" & fechaviaje & "','/',2),'-',Facturacion.fn_split('" & fechaviaje & "','/',1)) " +
                    " where numerotarjeta=" & numerotarjeta & " and isnull(fechaviaje);"
                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()





                    Consulta = "select tdh.idtarjetadetallehorarios," +
                                    "cast(concat(horarios.id_detaHorario,'|',horarios.id_horario,'|',rod.id_ruta,'|',horarios.id_empsa, '|' , horarios.idservicio) as char) datos," +
                                    "cast(concat(replace(hora.hora,':','.'), ' | ' ," +
                                    "bs.descripcion, ' | ' ," +
                                    "rod.nom_ruta, ' | ' ," +
                                    "'BUS-',v.id_veh, ' | ' ," +
                                    "'PIL. ' , piloto.nom_emp , ' | ' ," +
                                    "'ASIS. ' , asistente.nom_emp) as char) hora " +
                                    "from horarios " +
                                    "inner join vw_rutas_origen_destino rod " +
                                        "on horarios.id_ruta=rod.id_ruta " +
                                    "inner join Boletos.tarjetadetallehorarios tdh " +
                                        "on horarios.id_detaHorario=tdh.id_detaHorario " +
                                    "inner join hora " +
                                        "on horarios.id_horario=hora.id_horario " +
                                    "inner join Boletos.servicio bs " +
                                        "on bs.idservicio=horarios.idservicio " +
                                    "inner join vehiculo v " +
                                        "on v.id_veh=horarios.id_veh " +
                                   "inner join empleado piloto " +
                                        "on piloto.id_emp=horarios.id_emp " +
                                   "inner join empleado asistente " +
                                        "on asistente.id_emp=horarios.id_asistente " +
                                    "where horarios.id_detaHorario not in (select id_detaHorario from Boletos.tarjetadetallehorarios where numerotarjeta=" & numerotarjeta & ") " +
                                    " and tdh.numerotarjeta ='" & numerotarjeta & "' order by hora.hora;"

                    comando.CommandText = Consulta
                    Dim LectorHorarios As MySqlDataReader = comando.ExecuteReader()

                    While LectorHorarios.Read()
                        Elemento = New HorarioResumen
                        Elemento.correcto = True
                        Elemento.datos = LectorHorarios("idtarjetadetallehorarios")
                        Elemento.hora = LectorHorarios("hora")
                        ArregloSerie.Add(Elemento)
                    End While
                    LectorHorarios.Close()
                Else
                    LectorUsuario.Close()
                    Elemento = New HorarioResumen
                    Elemento.correcto = False

                    ArregloSerie.Add(Elemento)
                End If
                transaccion.Commit()

            Catch ex As Exception
                Elemento = New HorarioResumen
                Elemento.correcto = False
                ArregloSerie.Add(Elemento)
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    <WebMethod()> _
    Public Function muestraResumen(ByVal usuario As String, ByVal idhorario As Integer, ByVal fecha As String, ByVal idhora As Integer, ByVal idruta As Integer, ByVal idservicio As Integer) As List(Of Horario)
        Dim Elemento As Horario
        Dim ArregloSerie As List(Of Horario) = New List(Of Horario)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select " +
                "DATE_FORMAT(h.fecha_detaHorario,'%d/%m/%Y') fecha," +
                "h.id_horario," +
                "hora.hora," +
                "h.id_ruta," +
                "bs.idservicio," +
                "bs.descripcion," +
                "r.nom_ruta," +
                "r.secuencia," +
                "r.id_agencia," +
                "r.agencia," +
                "ifnull(b.cantidad_boletos, 0) cantidad_boletos," +
                "ifnull(b.monto_boletos, 0) monto_boletos " +
                "from " +
                "Encomiendas.horarios h " +
                "Left Join " +
                "Encomiendas.hora hora ON h.id_horario = hora.id_horario " +
                "Left Join " +
                "Boletos.vw_agencias_rutas r ON h.id_empsa = r.id_empresa and h.id_ruta = r.id_ruta " +
                "Left Join " +
                "(select * " +
                "from " +
                "Boletos.vw_agencias_boletos_ruta sb " +
                "where " +
                "(sb.fecha = '" & fecha & "' or isnull(sb.fecha))) b ON h.fecha_detaHorario = b.fecha and " +
                "h.id_empsa = b.id_empresa And r.id_agencia = b.origen And h.id_horario = b.id_horario And r.id_ruta = b.id_ruta " +
                "Left Join " +
                "Boletos.servicio bs ON h.idservicio = bs.idservicio " +
                "where " +
                "h.fecha_detaHorario = " & fecha & " And (h.id_horario = " & idhora & ") And (r.id_ruta = " & idruta & ") And (bs.idservicio = " & idservicio & ") " +
                "order by fecha , hora , id_horario , id_ruta , secuencia;"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    '
                    Elemento = New Horario
                    Elemento.origen = LectorTarjeta("agencia")
                    Elemento.fecha = LectorTarjeta("fecha").ToString
                    Elemento.cantidadboletos = LectorTarjeta("cantidad_boletos")
                    Elemento.montoboletos = LectorTarjeta("monto_boletos")
                    Elemento.ruta = LectorTarjeta("nom_ruta")
                    Elemento.servicio = LectorTarjeta("descripcion")


                    ArregloSerie.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New Horario
                'Elemento.correcto = False
                'Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    <WebMethod()> _
    Public Function muestraHorarios(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of Horario)
        Dim Elemento As Horario
        Dim ArregloSerie As List(Of Horario) = New List(Of Horario)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select DISTINCT td.id_detaHorario idhorario,DATE_FORMAT(h.fecha_detaHorario,'%d/%m/%Y') fecha," +
                "h.id_horario idhora,hora.hora hora,h.id_ruta idruta,r.nom_ruta ruta,h.idservicio idservicio," +
                "s.descripcion servicio,p.nom_emp piloto,a.nom_emp asistente " +
                "from Boletos.tarjetadetalle td " +
                "INNER JOIN Encomiendas.horarios h ON td.id_detaHorario=h.id_detaHorario " +
                "INNER JOIN Encomiendas.hora hora ON h.id_horario=hora.id_horario " +
                "INNER JOIN Encomiendas.rutas r ON h.id_ruta=r.id_ruta " +
                "INNER JOIN Boletos.servicio s ON h.idservicio=s.idservicio " +
                "INNER JOIN Encomiendas.empleado p ON h.id_emp=p.id_emp " +
                "INNER JOIN Encomiendas.empleado a ON h.id_asistente=a.id_emp " +
                "WHERE td.numerotarjeta=" & numerotarjeta & " order by td.idtarjetadetalle asc;"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    'Dim serie As String = 
                    'Dim idusuario As String = 
                    'Dim fechacreacion As String =
                    '
                    Elemento = New Horario
                    Elemento.idhorario = LectorTarjeta("idhorario")
                    Elemento.fecha = LectorTarjeta("fecha").ToString
                    Elemento.hora = LectorTarjeta("hora").ToString
                    Elemento.idhora = LectorTarjeta("idhora").ToString
                    Elemento.idruta = LectorTarjeta("idruta").ToString
                    Elemento.ruta = LectorTarjeta("ruta")
                    Elemento.idservicio = LectorTarjeta("idservicio").ToString
                    Elemento.servicio = LectorTarjeta("servicio")
                    Elemento.piloto = LectorTarjeta("piloto")
                    Elemento.asistente = LectorTarjeta("asistente")


                    ArregloSerie.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New Horario
                'Elemento.correcto = False
                'Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function


    <WebMethod()> _
    Public Function muestraUltimoboleto(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As List(Of BoletoResumen)
        Dim Elemento As BoletoResumen
        Dim ArregloBoletos As List(Of BoletoResumen) = New List(Of BoletoResumen)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select b.id_boleto idboleto,b.total,b.estadoboleto,b.fechacreacion,b.usuario " +
                "from Boletos.boleto b " +
                "INNER JOIN Boletos.tarjetadetalledocumentos tdb ON b.id_boleto=tdb.id_boleto " +
                "where tdb.idtarjetadetalle=" & idtarjetadetalle & " and tdb.numerotarjeta=" & numerotarjeta & " order by tdb.idtarjetadetalle desc limit 1;"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    Elemento = New BoletoResumen
                    Elemento.idboleto = LectorTarjeta("idboleto")
                    Elemento.total = LectorTarjeta("total")
                    Elemento.estadoboleto = LectorTarjeta("estadoboleto")
                    Elemento.fechacreacion = LectorTarjeta("fechacreacion")
                    Elemento.idusuario = LectorTarjeta("usuario")
                    ArregloBoletos.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New BoletoResumen

                ArregloBoletos.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloBoletos
    End Function

    <WebMethod()> _
    Public Function muestraBoletosvendidos(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As List(Of BoletoResumen)


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As BoletoResumen
        Dim ArregloBoletos As List(Of BoletoResumen) = New List(Of BoletoResumen)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            comando.Connection = connection


            Try
                Dim Consulta As String = ""

                Consulta = "select b.id_boleto idboleto,b.total,b.estadoboleto," +
                "b.fechacreacion,b.usuario,fd.num_fac numerofactura,fd.serie_fac serie,fd.item " +
                "from Boletos.boleto b " +
                "INNER JOIN Boletos.tarjetadetalledocumentos tdb ON b.id_boleto=tdb.id_boleto " +
                "INNER JOIN Facturacion.factura_detalle fd ON b.id_boleto=fd.id_doc  and fd.item in (2,3) " +
                "where tdb.idtarjetadetalle=" & idtarjetadetalle & " and tdb.numerotarjeta=" & numerotarjeta & " "

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    Elemento = New BoletoResumen
                    Elemento.idboleto = LectorTarjeta("idboleto")
                    Elemento.total = LectorTarjeta("total")
                    Elemento.estadoboleto = LectorTarjeta("estadoboleto")
                    Elemento.fechacreacion = LectorTarjeta("fechacreacion")
                    Elemento.idusuario = LectorTarjeta("usuario")
                    Elemento.serie = LectorTarjeta("serie")
                    Elemento.numerofactura = LectorTarjeta("numerofactura")
                    Elemento.item = LectorTarjeta("item")
                    ArregloBoletos.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New BoletoResumen

                ArregloBoletos.Add(Elemento)

            End Try
        End Using
        Return ArregloBoletos
    End Function

    <WebMethod()> _
    Public Function muestraCargavendida(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As List(Of CargaResumen)


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As CargaResumen
        Dim ArregloCarga As List(Of CargaResumen) = New List(Of CargaResumen)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            comando.Connection = connection


            Try
                Dim Consulta As String = ""

                Consulta = "select c.id_carga idcarga,c.total,c.estadocarga,c.fechacreacion,c.usuario,fd.num_fac numerofactura,fd.serie_fac serie,fd.item " +
                    "from Boletos.carga c INNER JOIN Facturacion.factura_detalle fd ON c.id_carga=fd.id_doc  and fd.item in (5) " +
                    "where c.idtarjeta=" & numerotarjeta & ";"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    Elemento = New CargaResumen
                    Elemento.idcarga = LectorTarjeta("idcarga")
                    Elemento.total = LectorTarjeta("total")
                    Elemento.estadocarga = LectorTarjeta("estadocarga")
                    Elemento.fechacreacion = LectorTarjeta("fechacreacion")
                    Elemento.idusuario = LectorTarjeta("usuario")
                    Elemento.serie = LectorTarjeta("serie")
                    Elemento.numerofactura = LectorTarjeta("numerofactura")
                    Elemento.item = LectorTarjeta("item")
                    ArregloCarga.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New CargaResumen

                ArregloCarga.Add(Elemento)

            End Try
        End Using
        Return ArregloCarga
    End Function

    <WebMethod()> _
    Public Function muestraCargaAgenciaVendida(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As Decimal
        Dim carga As Decimal


        numerotarjeta = numerotarjeta.ToString
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand

            comando.Connection = connection


            Try
                Dim Consulta As String = ""

                Consulta = "select carga from Boletos.tarjetadetalle where numerotarjeta=" & numerotarjeta & " and idtarjetadetalle=" & idtarjetadetalle & ";"



                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()

                    carga = LectorTarjeta("carga")
                    carga = Decimal.Round(carga, 2)


                End While
                LectorTarjeta.Close()


            Catch ex As Exception


            End Try
        End Using
        Return carga
    End Function

    <WebMethod()> _
    Public Function GuardaCargaAgencia(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer, ByVal totalcarga As Decimal) As String
        Dim carga As String


        numerotarjeta = numerotarjeta.ToString
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand

            comando.Connection = connection


            Try
                Dim Consulta As String = ""

                Consulta = "update Boletos.tarjetadetalle set carga=" & totalcarga & " where numerotarjeta=" & numerotarjeta & " and idtarjetadetalle=" & idtarjetadetalle & ";"
                comando.CommandText = Consulta
                comando.ExecuteScalar()

                carga = "Carga Guardada Satisfactoriamente"

            Catch ex As Exception

                carga = "ERROR: " & ex.Message.ToString

            End Try
        End Using
        Return carga
    End Function

    <WebMethod()> _
    Public Function registraBoletosAgencia(ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer, ByVal idboleto As Integer) As String

        Dim Resultado As String = ""
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            comando.Connection = connection

            Try
                Dim Consulta As String = ""

                Consulta = "select if(count(id_boleto)>0,'SI','NO') registrar from Boletos.boleto where " +
                           " id_boleto = " & idboleto & " and idtipoboleto=1 and  isnull(idtarjeta) "


                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                LectorTarjeta.Read()

                If LectorTarjeta.HasRows Then

                    If LectorTarjeta("registrar") = "SI" Then

                        LectorTarjeta.Close()
                        Consulta = "insert into Boletos.tarjetadetalledocumentos (idtarjetadetalle,numerotarjeta,id_boleto,item,valor) values(" & idtarjetadetalle & "," & numerotarjeta & "," & idboleto & ",2,(select ifnull(valor,0.00) valor from Facturacion.factura_detalle where item=2 and id_doc=" & idboleto & "))"
                        comando.CommandText = Consulta
                        comando.ExecuteScalar()


                        Consulta = "update Boletos.boleto set idtarjeta=" & numerotarjeta & " where id_boleto=" & idboleto & ";"
                        comando.CommandText = Consulta
                        comando.ExecuteScalar()

                        Resultado = "Boleto Guardado Correctamente"
                    Else
                        Resultado = "ERROR: No Se Puede Registrar, Boleto # " & idboleto & " , Ya Esta Registrado"
                        LectorTarjeta.Close()
                    End If

                End If




            Catch ex As Exception

                Resultado = ex.Message.ToString
                If Left(Resultado, 9) = "Duplicate" Then

                    Resultado = "ERROR: ESTE BOLETO YA ESTA REGISTRADO EN UNA DE LAS TARJETAS"

                Else

                    Resultado = "ERROR" & ex.Message.ToString

                End If


            End Try
        End Using
        Return Resultado
    End Function

    <WebMethod()> _
    Public Function LiberarHorarioBoletosAgencia(ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer, ByVal usuario As String) As String

        Dim Resultado As String = ""
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            comando.Connection = connection

            Try
                Dim Consulta As String = ""


                Consulta = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                Dim idagencia As Integer
                Dim idempresa As Integer

                If LectorUsuario.HasRows Then
                    idagencia = LectorUsuario("idagencia")
                    idempresa = LectorUsuario("idempresa")
                End If

                LectorUsuario.Close()

                Consulta = "select id_detaHorario from Boletos.tarjetadetallehorarios tdh left join Boletos.tarjetadetalle td on tdh.idtarjetadetallehorarios=td.idtarjetadetallehorarios where td.idtarjetadetalle=" & idtarjetadetalle & ";"
                comando.CommandText = Consulta

                Dim idhorario As String


                idhorario = comando.ExecuteScalar


                Consulta = " call Boletos.sp_desasignacion_pases_abordaje('" & numerotarjeta & "','" & idhorario & "','" & usuario & "','" & idagencia & "','" & idempresa & "'); "
                comando.CommandText = Consulta
                comando.ExecuteScalar()


                Resultado = "Horario Liberado Correctamente, Buscar Tarjeta De Nuevo"

            Catch ex As Exception

                Resultado = ex.Message.ToString
                Resultado = "ERROR" & ex.Message.ToString


            End Try
        End Using
        Return Resultado
    End Function


    '<WebMethod(True)> _
    'Public Function GuardaCarga(ByVal Factura As List(Of Factura), ByVal Facturadetalle As List(Of FacturaDetalle), _
    '                             ByVal Carga As List(Of Boleto), ByVal idagenciacaja As Integer, _
    '                             ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As String

    '    numerotarjeta = numerotarjeta.ToString


    '    Dim DatosFacturaElectronica As String = "", DatosPos As String = "", IdRemitente As String = "", IdDestinatario As String = "", EstatusGface As String = "", VarNomFac = " ", VarNitFac As String = " ", VarDireFac As String = " ", VerificaButacaVendida As String = "", CodigoAgencia As String = "", CodigoAgenciaUsuario As String = ""
    '    Dim Agencia As String = "", DireccionAgencia As String = "", Empresa As String = "", Caja As String = "", Dispositivo As String = "", NumeroFactura As Long = 0
    '    Dim QueryCarga As String = "", QueryCargaDetalle As String = "", QueryFacturaDetalle As String = "", StrInsertaFacEnca As String = "", strActualizarCorrelativo = "", NemonicoAgenciaDestino As String = "", NemonicoAgenciaSalida As String = "", NemonicoDestino As String = "", NemonicoSalida As String = "", Correlativo, Cae1, Cae2, Cae, Face, Resultado As String
    '    Dim ArrayFacturaelectronica As Array
    '    Dim Resolucion As String = "", FechaResolucion As String = ""

    '    Dim NitCliente As String = "C/F"
    '    Dim NombreCliente As String = "Consumidor Final"
    '    Dim DireccionCliente As String = "__________________"
    '    Dim Recargo As Double
    '    Dim Tarjeta As Double
    '    Dim Cheque As Double

    '    Dim SQLInserta As String
    '    Dim ValorBoleto As Double = 0
    '    Dim Seguro As Double = 0
    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        connection.Open()
    '        Dim comando As New MySqlCommand
    '        comando.Connection = connection

    '        Try

    '            If Session("id_usr") = "" Then

    '                Return "ERROR. SU SESION EXPIRO!"

    '            Else
    '                Dim ConsultaDatos As String = "call Boletos.sp_retorna_series_ruta(" & idagenciacaja & ",'" & Session("id_usr") & "');"

    '                NumeroFactura = 0

    '                comando.CommandText = ConsultaDatos
    '                Dim Lector As MySqlDataReader = comando.ExecuteReader()

    '                While Lector.Read()

    '                    Caja = Lector("caja")
    '                    Resolucion = Lector("resolucion")
    '                    FechaResolucion = Lector("fecha_resolucion")
    '                    CodigoAgencia = Lector("codigoagencia")
    '                    CodigoAgenciaUsuario = Lector("codigoagenciausuario")
    '                    Empresa = Lector("empresa")
    '                    Dispositivo = Lector("dispositivo")
    '                    Agencia = Lector("agencia")
    '                    DireccionAgencia = Lector("direccionagencia")
    '                End While
    '                Lector.Close()



    '                If Len(Resolucion) > 0 Then


    '                    For i = 0 To Factura.Count - 1
    '                        '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

    '                        '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------


    '                        QueryCarga = " insert into Boletos.carga(id_carga,fechacreacion,nitcliente,estadofacturacion,estadocarga,saldo,total,empresa,agencia,usuario,idtarjeta) " +
    '                                     " values(0,now(),\'" & NitCliente & "\',\'FACTURADO\',\'VALIDO\',0.00," & Carga(i).total & "," & Empresa & "," & CodigoAgenciaUsuario & ",\'" & Session("id_usr") & "\'," & numerotarjeta & ");"


    '                        '********************************** Generacion de Gface Electronica*************************************

    '                        ''-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'
    '                        QueryFacturaDetalle = "insert into Facturacion.factura_detalle " +
    '                        "(num_fac,serie_fac,item,descripcion,id_doc,valor) " +
    '                        "values (new.num_fac,\'" & Trim(Facturadetalle(i).seriefactura) & "\',5,\' Tarjeta# " & numerotarjeta & "\',@documento," & Facturadetalle(i).valor & ")"

    '                        QueryCargaDetalle = "insert into Boletos.tarjetadetalledocumentos " +
    '                       "(idtarjetadetalle,numerotarjeta,id_boleto,item,valor) " +
    '                       "values(" & idtarjetadetalle & "," & numerotarjeta & ",@documento,5," & Facturadetalle(i).valor & ")"



    '                        If Session("idtransaccion") = 0 Then

    '                            StrInsertaFacEnca = "insert into Facturacion.transaccionesgface " +
    '                            "(num_fac,serie_fac,fecha_transaccion,hora_transaccion,valor_fac,nit_clt,nom_clt," +
    '                            "id_empresa,id_agc,id_usr,id_agenciacaja,cae,face,recargo,efectivo,tarjetacredito,cheque,nimpresion,documento,documento_detalle,documento_historial,factura_detalle,item,archivoimpresion) " +
    '                            "values (null,'" & Trim(Factura(i).seriefactura) & "',curdate(),curtime()," & Factura(i).valorfactura & "," +
    '                            "'" & NitCliente & "','" & NombreCliente & "'," +
    '                            "1," & CodigoAgenciaUsuario & ",'" & Session("id_usr") & "'," & idagenciacaja & "," +
    '                            "'" & "N/A" & "','" & "N/A" & "'," & Recargo & ", " & Factura(i).efectivo & "," +
    '                            "" & Tarjeta & "," & Cheque & ",1,'" & QueryCarga & "','','" & QueryCargaDetalle & "','" & QueryFacturaDetalle & "',5,' TARJETA # " & numerotarjeta & "')"


    '                            comando.CommandText = StrInsertaFacEnca
    '                            comando.ExecuteScalar()

    '                            comando.CommandText = "SELECT @@IDENTITY"
    '                            Session("idtransaccion") = comando.ExecuteScalar


    '                        End If

    '                        DatosFacturaElectronica = GenerarFacturaElectronica(Dispositivo, NitCliente, Trim(Factura(i).seriefactura), Factura(i).valorfactura, Factura(i).numerofactura, Resolucion, NombreCliente, DireccionCliente, DireccionAgencia, FechaResolucion, CodigoAgencia)
    '                        ArrayFacturaelectronica = Split(DatosFacturaElectronica, "|")

    '                        Face = ArrayFacturaelectronica(0)
    '                        Cae = ArrayFacturaelectronica(1)
    '                        Cae1 = ArrayFacturaelectronica(3)
    '                        Cae2 = ArrayFacturaelectronica(4)
    '                        Correlativo = ArrayFacturaelectronica(2)
    '                        Session("correlativofactura") = Correlativo
    '                        EstatusGface = ArrayFacturaelectronica(5)
    '                        Resultado = ArrayFacturaelectronica(6)

    '                        If Resultado = "valido" Then



    '                            Dim ActualizaTransaccion As String = ""
    '                            ActualizaTransaccion = ActualizaTransaccionGface(Session("serie").ToString, Correlativo, Cae, Face, Session("idtransaccion"))


    '                            If ActualizaTransaccion = "Actualizada" Then

    '                                ActualizaTransaccion = "CALL Facturacion.sp_guarda_documentos('" & Session("idtransaccion") & "')"
    '                                comando.CommandText = ActualizaTransaccion
    '                                Resultado = comando.ExecuteScalar

    '                                If Resultado = "IMPRIMIR" Then

    '                                    Session("idtransaccion") = 0

    '                                    strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Correlativo & " " +
    '                                                               " where id_agenciacaja=" & idagenciacaja & ""
    '                                    comando.CommandText = strActualizarCorrelativo
    '                                    comando.ExecuteNonQuery()


    '                                Else
    '                                    Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " Presione de nuevo Facturar"
    '                                End If
    '                            Else

    '                                Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " " & ActualizaTransaccion & " Presione de nuevo Facturar"

    '                            End If

    '                        Else

    '                            comando.CommandText = "update Facturacion.transaccionesgface set error='" & Replace(EstatusGface, "'", "") & "' where idtransaccion= " & Session("idtransaccion")
    '                            comando.ExecuteScalar()

    '                            Resultado = "ERROR:EN LA TRANSACCION " & Session("idtransaccion") & " " & EstatusGface & " Presione de nuevo Facturar"
    '                            Session("idtransaccion") = 0

    '                            Return Resultado




    '                        End If



    '                    Next

    '                    'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
    '                    Lector.Close()


    '                    strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Factura(Factura.Count - 1).numerofactura & " " +
    '                        " where id_agenciacaja=" & idagenciacaja & ""
    '                    comando.CommandText = strActualizarCorrelativo
    '                    comando.ExecuteNonQuery()






    '                    '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
    '                    Return "Datos guardados correctamente"
    '                Else

    '                    Return "ERROR. No se establecieron los datos del usuario"
    '                End If

    '            End If
    '        Catch ex As MySqlException


    '            SQLInserta = "ERROR." + ex.Message
    '            Return SQLInserta
    '        Catch ex As Exception

    '            SQLInserta = "ERROR." + ex.Message
    '            Return SQLInserta
    '        Finally
    '            connection.Close()
    '            comando.Dispose()

    '        End Try
    '    End Using
    'End Function

    <WebMethod()> _
    Public Function muestraSeriesporTarjeta(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of Serie)
        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select t.numerotarjeta numerotarjeta,ags.serie,tt.id_usr idusuario,tt.fechacreacion " +
                " from Boletos.tarjetatalonario tt " +
                " INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
                " INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
                " WHERE t.numerotarjeta=" & numerotarjeta & ";"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    Dim serie As String = LectorTarjeta("serie")
                    Dim idusuario As String = LectorTarjeta("idusuario")
                    Dim fechacreacion As String = LectorTarjeta("fechacreacion")
                    Elemento = New Serie
                    Elemento.correlativo = 0
                    Elemento.serie = serie
                    Elemento.usuario = idusuario
                    Elemento.fechacreacion = fechacreacion
                    ArregloSerie.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    '<WebMethod()> _
    'Public Function muestraSeries(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of Serie)


    '    numerotarjeta = numerotarjeta.ToString

    '    Dim Elemento As Serie
    '    Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        connection.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = connection.BeginTransaction
    '        comando.Connection = connection
    '        comando.Transaction = transaccion

    '        Try
    '            Dim Consulta As String = ""

    '            Consulta = "select ags.id_agenciacaja correlativo,t.numerotarjeta numerotarjeta,tt.id_usr idusuario,tt.fechacreacion," +
    '            "CONCAT(ags.serie,' | ',CAST(ags.inicio as char(10)),' - ',CAST(ags.final as char(10))) as serie " +
    '            "from Boletos.tarjetatalonario tt " +
    '            "INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
    '            "INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
    '            "WHERE t.numerotarjeta=" & numerotarjeta & ";"

    '            comando.CommandText = Consulta
    '            Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

    '            While LectorTarjeta.Read()
    '                'Dim serie As String = 
    '                'Dim idusuario As String = 
    '                'Dim fechacreacion As String =
    '                '
    '                Elemento = New Serie
    '                Elemento.correlativo = LectorTarjeta("correlativo")
    '                Elemento.serie = LectorTarjeta("serie")
    '                Elemento.usuario = LectorTarjeta("idusuario")
    '                Elemento.fechacreacion = LectorTarjeta("fechacreacion")
    '                ArregloSerie.Add(Elemento)
    '            End While
    '            LectorTarjeta.Close()


    '        Catch ex As Exception
    '            Elemento = New Serie
    '            Elemento.correcto = False
    '            Elemento.mensaje = "ERROR: " + ex.Message
    '            ArregloSerie.Add(Elemento)
    '            'transaccion.Rollback()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    '    Return ArregloSerie
    'End Function



    <WebMethod()> _
    Public Function cargarSeries(ByVal usuario As String) As List(Of Serie)
        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    'Consulta = "call Boletos.sp_series_boletos_rutas();"
                    Consulta = "call Boletos.sp_series_boletos_rutas2(" & idempresa & ");"

                    comando.CommandText = Consulta
                    Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                    While LectorTarjeta.Read()
                        Dim correlativo As Integer = LectorTarjeta("correlativo")
                        Dim serie As String = LectorTarjeta("serie")

                        Elemento = New Serie
                        Elemento.correlativo = correlativo
                        Elemento.serie = serie
                        ArregloSerie.Add(Elemento)
                    End While
                    LectorTarjeta.Close()
                Else
                    Elemento = New Serie
                    Elemento.correcto = False
                    Elemento.mensaje = "No se ha encontrado la serie"
                    ArregloSerie.Add(Elemento)
                End If

            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    '<WebMethod()> _
    'Public Function buscarSerie(ByVal serie As String, ByVal ultimocorrelativo As Integer, ByVal usuario As String) As Serie
    '    Dim Elemento As Serie

    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        connection.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = connection.BeginTransaction
    '        comando.Connection = connection
    '        comando.Transaction = transaccion

    '        Try
    '            Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

    '            comando.CommandText = Consulta
    '            Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

    '            LectorUsuario.Read()

    '            If LectorUsuario.HasRows Then
    '                Dim idagencia As Integer = LectorUsuario("idagencia")
    '                Dim idempresa As Integer = LectorUsuario("idempresa")

    '                LectorUsuario.Close()

    '                Consulta = "select correlativo,final from Facturacion.agenciacaja where id_agenciacaja=" & serie & ";"

    '                comando.CommandText = Consulta
    '                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

    '                LectorTarjeta.Read()
    '                If LectorTarjeta.HasRows Then
    '                    Dim correlativoactual As Integer = LectorTarjeta("correlativo")
    '                    Dim final As Integer = LectorTarjeta("final")

    '                    LectorTarjeta.Close()
    '                    Elemento = New Serie
    '                    Elemento.mensaje = "Dato encontrado"

    '                    If correlativoactual + 1 > ultimocorrelativo Then
    '                        Elemento.correcto = False
    '                        Elemento.mensaje = "El ultimo correlativo ingresado es mayor que el correlativo actual"
    '                    ElseIf ultimocorrelativo > final Then
    '                        Elemento.correcto = False
    '                        Elemento.mensaje = "El rango vendido es invalido"

    '                    Else
    '                        Elemento.correcto = True
    '                        Elemento.correlativo = correlativoactual + 1
    '                        Elemento.mensaje = "Rango valido"
    '                    End If

    '                Else
    '                    Elemento = New Serie
    '                    Elemento.correcto = False
    '                    Elemento.mensaje = "No se ha encontrado la serie"
    '                End If

    '                'transaccion.Commit()
    '            Else
    '                LectorUsuario.Close()
    '                Elemento = New Serie
    '                Elemento.correcto = False
    '                Elemento.mensaje = "No se establecieron los datos del usuario"
    '            End If
    '        Catch ex As Exception
    '            Elemento = New Serie
    '            Elemento.correcto = False
    '            Elemento.mensaje = "ERROR: " + ex.Message
    '            'transaccion.Rollback()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    '    Return Elemento
    'End Function

    Public Class Serie
        Public correlativo As Integer
        Public serie As String
        Public fechacreacion As String
        Public usuario As String
        Public numerotarjeta As Integer
        Public correcto As Boolean
        Public mensaje As String


        Public correlativoinicial As Integer
        Public correlativofinal As Integer





    End Class

    <WebMethod()> _
    Public Function crearTarjetadetalle(ByVal numerotarjetap As Integer, ByVal usuario As String, ByVal idhorariop As Integer, ByVal cargap As Double, ByVal excursionp As Double, ByVal viaticospilotop As Double, ByVal viaticosasistentep As Double, ByVal numerovalep As Integer) As TarjetaDetalle

        numerotarjetap = numerotarjetap.ToString

        Dim ElementoDetalle As TarjetaDetalle
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            Dim iddetallehorario As String = ""

            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"
                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()


                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    Dim espacio As String = "''"

                    Dim extra As String = "null"

                    iddetallehorario = "Select idtarjetadetallehorarios from Boletos.tarjetadetallehorarios where id_detaHorario='" & idhorariop & "' and numerotarjeta=" & numerotarjetap & ";"
                    comando.CommandText = iddetallehorario
                    iddetallehorario = comando.ExecuteScalar

                    Consulta = "insert into Boletos.tarjetadetalle (numerotarjeta,fechacreacion,idtarjetadetallehorarios," +
                    "id_usr,id_agc,id_empsa,carga,excursion,viaticospiloto,viaticosasistente,numerovale) " +
                    "values " +
                    "(" & numerotarjetap & ",curdate(),'" & iddetallehorario & "','" & usuario & "'," & idagencia & "," +
                    "" & idempresa & "," & cargap & "," & excursionp & "," & viaticospilotop & "," +
                    "" & viaticosasistentep & "," & IIf(numerovalep = 0, extra, numerovalep) & " )"

                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()

                    Consulta = "SELECT @@IDENTITY"

                    comando.CommandText = Consulta
                    Dim correlativoactual As Integer = comando.ExecuteScalar

                    Consulta = "select td.idtarjetadetalle idtarjetadetalle," +
                        "DATE_FORMAT(td.fechacreacion,'%d/%m/%Y') fechacreacion,DATE_FORMAT(h.fecha_detaHorario,'%Y-%m-%d') fechahorario," +
                        "h.id_detahorario idhorario,td.id_usr idusuario,td.carga,td.excursion," +
                        "td.viaticospiloto,td.viaticosasistente,td.numerovale," +
                        "e.id_empsa idempresa,e.nom_empsa empresa,a.id_agc idagencia," +
                        "a.nom_agc agencia,piloto.nom_emp piloto,asistente.nom_emp asistente," +
                        "r.nom_ruta ruta,s.descripcion servicio,hora.hora," +
                        "h.id_horario idhora,h.id_ruta idruta,h.idservicio idservicio " +
                        "from Boletos.tarjetadetalle td " +
                        "INNER JOIN Facturacion.empresa e ON td.id_empsa=e.id_empsa " +
                        "INNER JOIN Facturacion.agencia a ON td.id_agc=a.id_agc " +
                        "INNER JOIN Boletos.tarjetadetallehorarios tdh ON td.idtarjetadetallehorarios=tdh.idtarjetadetallehorarios " +
                        "INNER JOIN Encomiendas.horarios h ON tdh.id_detaHorario=h.id_detaHorario " +
                        "INNER JOIN Encomiendas.empleado piloto ON h.id_emp=piloto.id_emp " +
                        "INNER JOIN Encomiendas.empleado asistente ON h.id_asistente=asistente.id_emp " +
                        "INNER JOIN Encomiendas.rutas r ON h.id_ruta=r.id_ruta " +
                        "INNER JOIN Boletos.servicio s ON h.idservicio=s.idservicio " +
                        "LEFT JOIN Encomiendas.hora hora ON h.id_horario=hora.id_horario " +
                        "where td.idtarjetadetalle=" & correlativoactual & " order by td.idtarjetadetalle desc limit 1"

                    comando.CommandText = Consulta
                    Dim LectorTarjetaDetalle As MySqlDataReader = comando.ExecuteReader()


                    Dim idtarjetadetalle As Integer = 0
                    Dim fechacreaciond As String = ""
                    Dim idhorario As Integer = 0
                    Dim idusuariod As String = ""
                    Dim idagenciad As Integer = 0
                    Dim agenciad As String = ""
                    Dim idempresad As Integer = 0
                    Dim empresad As String = ""
                    Dim carga As Double = 0
                    Dim excursion As Double = 0
                    Dim viaticospiloto As Double = 0
                    Dim viaticosasistente As Double = 0
                    Dim numerovale As String = 0
                    Dim piloto As String = ""
                    Dim asistente As String = ""
                    Dim idruta As Integer = 0
                    Dim ruta As String = ""
                    Dim idservicio As Integer = 0
                    Dim servicio As String = ""
                    Dim idhora As Integer = 0
                    Dim hora As String = ""
                    Dim fechahorario As String = ""

                    Dim contador As Integer = 0
                    While (LectorTarjetaDetalle.Read())
                        idtarjetadetalle = LectorTarjetaDetalle("idtarjetadetalle")
                        fechacreaciond = LectorTarjetaDetalle("fechacreacion").ToString
                        idhorario = LectorTarjetaDetalle("idhorario")
                        idusuariod = LectorTarjetaDetalle("idusuario")
                        idagenciad = 0
                        agenciad = LectorTarjetaDetalle("agencia")
                        idempresad = 0
                        empresad = LectorTarjetaDetalle("empresa")
                        carga = LectorTarjetaDetalle("carga")
                        excursion = LectorTarjetaDetalle("excursion")
                        viaticospiloto = LectorTarjetaDetalle("viaticospiloto")
                        viaticosasistente = LectorTarjetaDetalle("viaticosasistente")
                        numerovale = IIf(LectorTarjetaDetalle("numerovale") IsNot DBNull.Value, LectorTarjetaDetalle("numerovale"), "N/A")
                        piloto = LectorTarjetaDetalle("piloto")
                        asistente = LectorTarjetaDetalle("asistente")
                        idruta = LectorTarjetaDetalle("idruta")
                        ruta = LectorTarjetaDetalle("ruta")
                        idservicio = LectorTarjetaDetalle("idservicio")
                        servicio = LectorTarjetaDetalle("servicio")
                        idhora = LectorTarjetaDetalle("idhora")
                        hora = LectorTarjetaDetalle("hora").ToString
                        fechahorario = LectorTarjetaDetalle("fechahorario")

                        ElementoDetalle = New TarjetaDetalle(idtarjetadetalle, numerotarjetap, fechacreaciond, idhorario, idusuariod, _
                                                             idempresad, empresad, idagenciad, agenciad, carga, excursion, _
                                                             viaticospiloto, viaticosasistente, numerovale, piloto, _
                                                             asistente, idruta, ruta, idservicio, servicio, idhora, hora, fechahorario)
                        contador = contador + 1
                        'ArregloDetalle.Add(ElementoDetalle)
                    End While
                    LectorTarjetaDetalle.Close()
                    transaccion.Commit()

                    If contador <= 0 Then
                        ElementoDetalle = New TarjetaDetalle(0, 0, "ERROR: No se logro determinar el ultimo detalle agregado", 0, "", _
                                                 0, "", 0, "", 0, 0, _
                                                 0, 0, 0, "", _
                                                "", 0, "", 0, "", 0, "", "")
                    End If

                Else
                    ElementoDetalle = New TarjetaDetalle(0, 0, "ERROR: No se determinaron datos del usuario", 0, "", _
                                             0, "", 0, "", 0, 0, _
                                             0, 0, 0, "", _
                                             "", 0, "", 0, "", 0, "", "")
                End If
            Catch ex As Exception

                Dim arrayerror As Array

                arrayerror = (ex.Message).Split(" ")

                If arrayerror.Length = 6 And arrayerror(5).ToString = "'idtarjetadetalle_UNIQUE'" Then

                    ElementoDetalle = New TarjetaDetalle(0, 0, "ERROR: ESTE HORARIO YA ESTA REGISTRADO EN ESTA TARJETA", 0, "", _
                                                            0, "", 0, "", 0, 0, _
                                                            0, 0, 0, "", _
                                                            "", 0, "", 0, "", 0, "", "")


                Else

                    ElementoDetalle = New TarjetaDetalle(0, 0, "ERROR: " + ex.Message, 0, "", _
                                                           0, "", 0, "", 0, 0, _
                                                           0, 0, 0, "", _
                                                           "", 0, "", 0, "", 0, "", "")



                End If


                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ElementoDetalle

    End Function

    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                PROCEDIMIENTO QUE DETERMINA LOS TURNOS VALIDOS SEGUN EL HORIGEN Y LA FECHA INGRESADA                    *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'

    <WebMethod(True)> _
    Public Function TurnoSugerido(ByVal Vehiculo As String, ByVal Fecha As String) As List(Of WSboleto.ClaseHora)
        ' MsgBox("** TurnoSugerido" & Origen & " - " & Fecha)
        Dim fechaStr As Array
        fechaStr = Split(Fecha, "/")
        Dim fechaFinal As String
        fechaFinal = fechaStr(2) & "-" & fechaStr(1) & "-" & fechaStr(0)
        Dim Busqueda As String = "call sp_retorna_turnos_vehiculo('" & fechaFinal.ToString & "','" & Vehiculo & "')"
        Dim result As List(Of WSboleto.ClaseHora) = New List(Of WSboleto.ClaseHora)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New WSboleto.ClaseHora
                    If Not IsDBNull(reader("hora")) Then
                        Elemento.datos = reader("datos")
                        Elemento.hora = reader("hora")
                        'Elemento.estado = reader("estado")
                        result.Add(Elemento)
                    End If
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                'MsgBox(ex.Message)
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function buscarTarjeta(ByVal numerotarjeta As Integer, ByVal usuario As String) As Tarjeta


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As Tarjeta
        Dim ElementoDetalle As TarjetaDetalle
        Dim ArregloDetalle As List(Of TarjetaDetalle) = New List(Of TarjetaDetalle)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            comando.Connection = connection

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()



                    Consulta = "select t.idtarjeta idtarjeta" +
                    ",DATE_FORMAT(t.fechacreacion,'%d/%m/%Y') fechacreacion" +
                    ",ifnull(t.id_veh,'S/A') vehiculo" +
                    ",t.id_usr usuario," +
                    "empresa.id_empsa idempresa" +
                    ",empresa.nom_empsa empresa" +
                    ",a.id_agc idagencia," +
                    "a.nom_agc agencia" +
                    ",e.idtarjetaestado" +
                    ",e.descripcion tarjetaestado" +
                    ",t.random " +
                    "from Boletos.tarjeta t " +
                    "INNER JOIN Boletos.tarjetaestado e ON t.idtarjetaestado=e.idtarjetaestado " +
                    "INNER JOIN Facturacion.empresa empresa ON t.id_empsa=empresa.id_empsa " +
                    "INNER JOIN Facturacion.agencia a ON t.id_empsa=a.id_empsa and t.id_agc=a.id_agc  " +
                    "where t.numerotarjeta=" & numerotarjeta & " and t.id_empsa=" & idempresa & ";"

                    comando.CommandText = Consulta
                    Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                    LectorTarjeta.Read()
                    If LectorTarjeta.HasRows Then
                        Dim idtarjeta As Integer = LectorTarjeta("idtarjeta")
                        Dim vehiculo As String = LectorTarjeta("vehiculo")
                        Dim fechacreacion As String = LectorTarjeta("fechacreacion").ToString
                        Dim user As String = LectorTarjeta("usuario")
                        Dim agencia As String = LectorTarjeta("agencia")
                        Dim empresa As String = LectorTarjeta("empresa")
                        Dim codigoempresa As Integer = LectorTarjeta("idempresa")
                        Dim codigoagencia As Integer = LectorTarjeta("idagencia")
                        Dim idtarjetaestado As Integer = LectorTarjeta("idtarjetaestado")
                        Dim tarjetaestado As String = LectorTarjeta("tarjetaestado")
                        LectorTarjeta.Close()

                        Consulta = "select td.idtarjetadetalle idtarjetadetalle,DATE_FORMAT(td.fechacreacion,'%d/%m/%Y') fechacreacion," +
                        "DATE_FORMAT(h.fecha_detaHorario,'%d/%m/%Y') fechahorario," +
                        "h.id_detaHorario idhorario,td.id_usr idusuario,td.carga,td.excursion,td.viaticospiloto," +
                        "td.viaticosasistente,td.numerovale,e.id_empsa idempresa,e.nom_empsa empresa," +
                        "a.id_agc idagencia,a.nom_agc agencia,piloto.nom_emp piloto,asistente.nom_emp asistente," +
                        "r.nom_ruta ruta,s.descripcion servicio,hora.hora,h.id_horario idhora,h.id_ruta idruta,h.idservicio idservicio " +
                        "from Boletos.tarjetadetalle td " +
                        "LEFT JOIN Facturacion.empresa e ON td.id_empsa=e.id_empsa " +
                        "LEFT JOIN Facturacion.agencia a ON td.id_agc=a.id_agc and e.id_empsa = a.id_empsa " +
                        "LEFT JOIN Boletos.tarjetadetallehorarios tdh ON td.numerotarjeta = tdh.numerotarjeta and td.idtarjetadetallehorarios = tdh.idtarjetadetallehorarios " +
                        "INNER JOIN Encomiendas.horarios h ON tdh.id_detaHorario=h.id_detaHorario " +
                        "LEFT JOIN Encomiendas.empleado piloto ON h.id_emp=piloto.id_emp " +
                        "LEFT JOIN Encomiendas.empleado asistente ON h.id_asistente=asistente.id_emp " +
                        "LEFT JOIN Encomiendas.rutas r ON h.id_ruta=r.id_ruta " +
                        "LEFT JOIN Boletos.servicio s ON h.idservicio=s.idservicio " +
                        "LEFT JOIN Encomiendas.hora hora ON h.id_horario=hora.id_horario " +
                        "where td.numerotarjeta=" & numerotarjeta & ""

                        comando.CommandText = Consulta
                        Dim LectorTarjetaDetalle As MySqlDataReader = comando.ExecuteReader()

                        Dim idtarjetadetalle As Integer = 0
                        Dim fechacreaciond As String = ""
                        Dim idhorario As Integer = 0
                        Dim idusuariod As String = ""
                        Dim idagenciad As Integer = 0
                        Dim agenciad As String = ""
                        Dim idempresad As Integer = 0
                        Dim empresad As String = ""
                        Dim carga As Double = 0
                        Dim excursion As Double = 0
                        Dim viaticospiloto As Double = 0
                        Dim viaticosasistente As Double = 0
                        Dim numerovale As String = 0
                        Dim piloto As String = ""
                        Dim asistente As String = ""
                        Dim idruta As Integer = 0
                        Dim ruta As String = ""
                        Dim idservicio As Integer = 0
                        Dim servicio As String = ""
                        Dim idhora As Integer = 0
                        Dim hora As String = ""
                        Dim fechahorario As String = ""

                        While (LectorTarjetaDetalle.Read())
                            idtarjetadetalle = LectorTarjetaDetalle("idtarjetadetalle")
                            fechacreaciond = LectorTarjetaDetalle("fechahorario").ToString
                            idhorario = LectorTarjetaDetalle("idhorario")
                            idusuariod = LectorTarjetaDetalle("idusuario")
                            idagenciad = 0
                            agenciad = LectorTarjetaDetalle("agencia")
                            idempresad = 0
                            empresad = LectorTarjetaDetalle("empresa")
                            carga = LectorTarjetaDetalle("carga")
                            excursion = LectorTarjetaDetalle("excursion")
                            viaticospiloto = LectorTarjetaDetalle("viaticospiloto")
                            viaticosasistente = LectorTarjetaDetalle("viaticosasistente")
                            numerovale = IIf(LectorTarjetaDetalle("numerovale") IsNot DBNull.Value, LectorTarjetaDetalle("numerovale"), "N/A")
                            piloto = LectorTarjetaDetalle("piloto")
                            asistente = LectorTarjetaDetalle("asistente")
                            idruta = LectorTarjetaDetalle("idruta")
                            ruta = LectorTarjetaDetalle("ruta")
                            idservicio = LectorTarjetaDetalle("idservicio")
                            servicio = LectorTarjetaDetalle("servicio")

                            'Session("Especial") = LectorTarjetaDetalle("servicio")

                            idhora = LectorTarjetaDetalle("idhora")
                            hora = LectorTarjetaDetalle("hora").ToString
                            fechahorario = LectorTarjetaDetalle("fechahorario")

                            ElementoDetalle = New TarjetaDetalle(idtarjetadetalle, numerotarjeta, fechacreaciond, idhorario, idusuariod, _
                                                                 idempresad, empresad, idagenciad, agenciad, carga, excursion, _
                                                                 viaticospiloto, viaticosasistente, numerovale, piloto, _
                                                                 asistente, idruta, ruta, idservicio, servicio, idhora, hora, fechahorario)
                            ArregloDetalle.Add(ElementoDetalle)
                        End While

                        LectorTarjetaDetalle.Close()
                        Elemento = New Tarjeta(idtarjeta, vehiculo, numerotarjeta, fechacreacion, user, _
                                               codigoagencia, agencia, codigoempresa, empresa, _
                                               idtarjetaestado, tarjetaestado, True, _
                                               "Datos encontrados", 0, ArregloDetalle)


                    Else
                        LectorTarjeta.Close()
                        Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "ESTA TARJETA NO ESTA REGISTRADA", 0, New List(Of TarjetaDetalle))

                    End If

                Else
                    LectorUsuario.Close()
                    Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "No se establecieron los datos de agencia y empresa", 0, New List(Of TarjetaDetalle))

                End If
            Catch ex As Exception
                Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "ERROR: " + ex.Message, 0, New List(Of TarjetaDetalle))
            End Try
        End Using
        Return Elemento
    End Function

    <WebMethod()> _
    Public Function crearTarjeta(ByVal usuario As String) As Tarjeta
        Dim Elemento As Tarjeta
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()


                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    'Consulta = "insert into Boletos.tarjeta (numerotarjeta,fechacreacion,id_usr,id_agc,id_empsa,idtarjetaestado) " +
                    '"values " +
                    '"(" & numerotarjeta & ",curdate(),'" & usuario & "'," & idagencia & "," & idempresa & ",1)"

                    Dim Aleatorio As Integer = NumeroAleatorio()

                    Consulta = "insert into Boletos.tarjeta (fechacreacion,id_usr,id_agc,id_empsa,idtarjetaestado,random) " +
                    "values " +
                    "(curdate(),'" & usuario & "'," & idagencia & "," & idempresa & ",1," & Aleatorio & ")"

                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()

                    comando.CommandText = "SELECT @@IDENTITY"
                    Dim idtarjeta As Integer = comando.ExecuteScalar

                    Consulta = "update Boletos.tarjeta set idtarjeta=" & idtarjeta & " where numerotarjeta=" & idtarjeta & ""
                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()

                    Consulta = "select DATE_FORMAT(t.fechacreacion,'%d/%m/%Y') fechacreacion,t.id_usr usuario," +
                    "empresa.id_empsa idempresa,empresa.nom_empsa empresa,a.id_agc idagencia," +
                    "a.nom_agc agencia,e.idtarjetaestado,e.descripcion tarjetaestado,t.random " +
                    "from Boletos.tarjeta t " +
                    "INNER JOIN Boletos.tarjetaestado e ON t.idtarjetaestado=e.idtarjetaestado " +
                    "INNER JOIN Facturacion.empresa empresa ON t.id_empsa=empresa.id_empsa " +
                    "INNER JOIN Facturacion.agencia a ON t.id_agc=a.id_agc " +
                    "where idtarjeta=" & idtarjeta & ";"

                    comando.CommandText = Consulta
                    Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                    LectorTarjeta.Read()
                    If LectorTarjeta.HasRows Then
                        Dim fechacreacion As String = LectorTarjeta("fechacreacion").ToString
                        Dim user As String = LectorTarjeta("usuario")
                        Dim agencia As String = LectorTarjeta("agencia")
                        Dim empresa As String = LectorTarjeta("empresa")
                        Dim codigoempresa As Integer = LectorTarjeta("idempresa")
                        Dim codigoagencia As Integer = LectorTarjeta("idagencia")
                        Dim idtarjetaestado As Integer = LectorTarjeta("idtarjetaestado")
                        Dim tarjetaestado As String = LectorTarjeta("tarjetaestado")
                        Dim random As Integer = LectorTarjeta("random")
                        LectorTarjeta.Close()
                        Elemento = New Tarjeta(idtarjeta, 0, idtarjeta, fechacreacion, user, _
                                               codigoagencia, agencia, codigoempresa, empresa, _
                                               idtarjetaestado, tarjetaestado, True, _
                                               "Datos creados correctamente", random, New List(Of TarjetaDetalle))
                    Else
                        LectorTarjeta.Close()
                        Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "No se encontraron los datos despues de guardar", 0, New List(Of TarjetaDetalle))

                    End If

                    transaccion.Commit()


                Else
                    LectorUsuario.Close()
                    Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "No se establecieron los datos de agencia y empresa", 0, New List(Of TarjetaDetalle))

                End If
            Catch ex As Exception
                Elemento = New Tarjeta(0, 0, 0, "", "", 0, "", 0, "", 0, "", False, "ERROR: " + ex.Message, 0, New List(Of TarjetaDetalle))
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return Elemento

    End Function

    Public Class Factura
        Public correlativo As Integer
        Public numerofactura As Integer
        Public seriefactura As String
        Public fechafactura As String
        Public horafactura As String
        Public valorfactura As Double
        Public nitcliente As String
        Public nombrecliente As String
        Public idagencia As Integer
        Public idusuario As String
        Public idcaja As Integer
        Public cae As String
        Public face As String
        Public recargo As Double
        Public efectivo As Double
        Public tarjetacredito As Double
        Public cheque As Double
        Public referencia As String
        Public numeroimpresion As Integer
        Public estadocobro As String
        Public idempresa As Integer
        Public acreditado As String
    End Class

    Public Class FacturaDetalle
        Public correlativo As Integer
        Public numerofactura As Integer
        Public seriefactura As String
        Public item As Integer
        Public boleto As Integer 'id_doc
        Public valor As Double
        Public descripcion As String
    End Class

    Public Class Boleto
        Public idboleto As Integer
        Public fechacreacion As String
        Public fechaviaje As String
        Public origen As Integer
        Public destino As Integer
        Public nitcliente As String
        Public seguro As Double
        Public tipo_cobro As Integer
        Public numerobutaca As Integer
        Public estadobutaca As String
        Public idempresa As Integer
        Public idagencia As Integer
        Public idusuario As String
        Public estadofacturacion As String
        Public estadoboleto As String
        Public saldo As Double
        Public total As Double
        Public Descuento As Double
        Public AutorizaDescuento As String
        Public recargo As Double
        Public credito As String
        Public boletocredito As String
        Public idhora As Integer
        Public idruta As Integer
        Public secuenciaorigen As Integer
        Public secuenciadestino As Integer
        Public idservicio As Integer
        Public IdCliente As Integer
        Public idtipoboleto As Integer
        Public idestadoboleto As Integer

    End Class

    Public Class BoletoResumen
        Public idboleto As Integer
        Public serie As String
        Public numerofactura As String
        Public fechacreacion As String
        Public idusuario As String
        Public estadofacturacion As String
        Public estadoboleto As String
        Public item As Integer
        Public total As Double

    End Class

    Public Class ResumenTarjeta
        Public idboleto As Integer
        Public serie As String
        Public numerofactura As String
        Public fechacreacion As String
        Public idusuario As String
        Public estadofacturacion As String
        Public estadoboleto As String
        Public item As Integer
        Public total As Double

    End Class

    Public Class CargaResumen
        Public idcarga As Integer
        Public serie As String
        Public numerofactura As String
        Public fechacreacion As String
        Public idusuario As String
        Public estadofacturacion As String
        Public estadocarga As String
        Public item As Integer
        Public total As Double

    End Class

    Public Class Tarjeta
        Public idtarjeta As Integer
        Public vehiculo As String
        Public numerotarjeta As Integer
        Public fechacreacion As String
        Public idusuario As String
        Public idagencia As Integer
        Public agencia As String
        Public idempresa As Integer
        Public empresa As String
        Public idtarjetaestado As Integer
        Public tarjetaestado As String
        Public encontro As Boolean
        Public mensaje As String
        Public random As Integer
        Public Detalle As List(Of TarjetaDetalle)

        Public Sub New(ByVal _idtarjeta As Integer, ByVal _vehiculo As String, ByVal _numerotarjeta As Integer, ByVal _fechacreacion As String,
                       ByVal _idusuario As String, ByVal _idagencia As Integer, ByVal _agencia As String, ByVal _idempresa As Integer,
                       ByVal _empresa As String, ByVal _idtarjetaestado As Integer, ByVal _tarjetaestado As String,
                       ByVal _encontro As Boolean, ByVal _mensaje As String, ByVal _random As Integer, ByVal _Detalle As List(Of TarjetaDetalle))
            idtarjeta = _idtarjeta
            vehiculo = _vehiculo
            numerotarjeta = _numerotarjeta
            fechacreacion = _fechacreacion
            idusuario = _idusuario
            idagencia = _idagencia
            agencia = _agencia
            idempresa = _idempresa
            empresa = _empresa
            idtarjetaestado = _idtarjetaestado
            tarjetaestado = _tarjetaestado
            encontro = _encontro
            mensaje = _mensaje
            random = _random
            Detalle = _Detalle
        End Sub

    End Class

    Public Class TarjetaDetalle
        Public idtarjetadetalle As Integer
        Public numerotarjeta As Integer
        Public fechacreacion As String
        Public idhorario As Integer
        Public idusuario As String
        Public idagencia As Integer
        Public agencia As String
        Public idempresa As Integer
        Public empresa As String
        Public piloto As String
        Public asistente As String
        Public idruta As Integer
        Public ruta As String
        Public idservicio As Integer
        Public servicio As String
        Public idhora As Integer
        Public hora As String
        Public carga As Double
        Public excursion As Double
        Public viaticospiloto As Double
        Public viaticosasistente As Double
        Public numerovale As String
        Public fechahorario As String




        Public Sub New(ByVal _idtarjetadetalle As Integer, ByVal _numerotarjeta As Integer, ByVal _fechacreacion As String,
                      ByVal _idhorario As Integer, ByVal _idusuario As String, ByVal _idempresa As Integer,
                      ByVal _empresa As String, ByVal _idagencia As Integer, ByVal _agencia As String,
                      ByVal _carga As Double, ByVal _excursion As Double, ByVal _viaticospiloto As Double,
                      ByVal _viaticosasistente As Double, ByVal _numerovale As String, ByVal _piloto As String,
                      ByVal _asistente As String, ByVal _idruta As Integer, ByVal _ruta As String,
                      ByVal _idservicio As Integer, ByVal _servicio As String, ByVal _idhora As Integer,
                      ByVal _hora As String, ByVal _fechahorario As String)

            idtarjetadetalle = _idtarjetadetalle
            numerotarjeta = _numerotarjeta
            fechacreacion = _fechacreacion
            idhorario = _idhorario
            idusuario = _idusuario
            idagencia = _idagencia
            agencia = _agencia
            idempresa = _idempresa
            empresa = _empresa
            carga = _carga
            excursion = _excursion
            viaticospiloto = _viaticospiloto
            viaticosasistente = _viaticosasistente
            numerovale = _numerovale
            piloto = _piloto
            asistente = _asistente
            idruta = _idruta
            ruta = _ruta
            idservicio = _idservicio
            servicio = _servicio
            idhora = _idhora
            hora = _hora
            fechahorario = _fechahorario
        End Sub

    End Class

    Public Class Horario
        Public idhorario As Integer
        Public fecha As String
        Public idhora As Integer
        Public hora As String
        Public idservicio As Integer
        Public servicio As String
        Public idruta As Integer
        Public ruta As String
        Public idpiloto As Integer
        Public piloto As String
        Public idasistente As Integer
        Public asistente As String
        Public cantidadboletos As Integer
        Public montoboletos As Double
        Public idorigen As Integer
        Public origen As String

    End Class

    Public Class HorarioResumen
        Public datos As String
        Public hora As String
        Public correcto As Boolean
    End Class

    Public Function NumeroAleatorioPdf() As Integer
        Dim LimiteMinimo As Integer = 1
        Dim LimiteMaximo As Integer = 20
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return Aleatorio
    End Function

    Public Function NumeroAleatorio() As Integer
        Dim LimiteMinimo As Integer = 1000
        Dim LimiteMaximo As Integer = 9999
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

    Public Class ClaseAgencias
        Public idagencia As String
        Public nombreagencia As String
    End Class

    Public Class ClaseServicios
        Public idservicio As String
        Public nombreservicio As String
    End Class

    Public Class ClaseVehiculos
        Public idvehiculo As String
        Public nombrevehiculo As String
    End Class

    Public Class ClasePilotos
        Public idpiloto As String
        Public nombrepiloto As String
    End Class









    '<WebMethod()> _
    'Public Function asignarSerieTarjeta(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idagenciacaja As Integer) As List(Of Serie)


    '    numerotarjeta = numerotarjeta.ToString

    '    Dim Elemento As Serie
    '    Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        connection.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = connection.BeginTransaction
    '        comando.Connection = connection
    '        comando.Transaction = transaccion

    '        Try
    '            Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

    '            comando.CommandText = Consulta
    '            Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

    '            LectorUsuario.Read()

    '            If LectorUsuario.HasRows Then
    '                Dim idagencia As Integer = LectorUsuario("idagencia")
    '                Dim idempresa As Integer = LectorUsuario("idempresa")

    '                LectorUsuario.Close()

    '                Consulta = "insert into Boletos.tarjetatalonario " +
    '                "(numerotarjeta,id_agenciacaja,id_usr,id_agc,id_empsa,fechacreacion) " +
    '                "values(" & numerotarjeta & "," & idagenciacaja & ",'" & usuario & "'," & idagencia & "," & idempresa & ",now())"
    '                comando.CommandText = Consulta
    '                comando.ExecuteNonQuery()

    '                transaccion.Commit()

    '                Elemento = New Serie
    '                Elemento.correcto = True
    '                Elemento.mensaje = "Datos creados correctamente"
    '                ArregloSerie.Add(Elemento)
    '            Else
    '                LectorUsuario.Close()
    '                Elemento = New Serie
    '                Elemento.correcto = False
    '                Elemento.mensaje = "ERROR: No se establecieron los datos "
    '                ArregloSerie.Add(Elemento)
    '            End If

    '        Catch ex As Exception
    '            Elemento = New Serie
    '            Elemento.correcto = False
    '            Elemento.mensaje = "ERROR: " + ex.Message
    '            ArregloSerie.Add(Elemento)
    '            transaccion.Rollback()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    '    Return ArregloSerie
    'End Function





    <WebMethod()> _
    Public Function muestraSeries(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of Serie)


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select ags.id_agenciacaja correlativo " +
                  ",t.numerotarjeta numerotarjeta " +
                  ",tt.id_usr idusuario " +
                  ",tt.fechacreacion " +
                  ",CONCAT(ags.serie,' | ',CAST((ags.Correlativo+1) as char(10)),' - ',CAST(tt.correlativofinal as char(10))) as serie" +
                  ",ifnull((ags.Correlativo+1),0) correlativoinicial " +
                  ",ifnull(tt.correlativofinal,0)   correlativofinal " +
                 " from Boletos.tarjetatalonario tt " +
                " INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
                " INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
                " WHERE t.numerotarjeta=" & numerotarjeta & ";"


                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()

                    Elemento = New Serie
                    Elemento.correlativo = LectorTarjeta("correlativo")
                    Elemento.serie = LectorTarjeta("serie")
                    Elemento.usuario = LectorTarjeta("idusuario")
                    Elemento.fechacreacion = LectorTarjeta("fechacreacion")

                    Elemento.correlativoinicial = LectorTarjeta("correlativoinicial")
                    Elemento.correlativofinal = LectorTarjeta("correlativofinal")


                    ArregloSerie.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    '<WebMethod()> _
    'Public Function buscarSerie(ByVal serie As String, ByVal ultimocorrelativo As Integer, ByVal usuario As String, ByVal tarjetanumero As Integer) As Serie
    '    Dim Elemento As Serie
    '    tarjetanumero = tarjetanumero.ToString
    '    Dim correlativoaoperar As Integer = ultimocorrelativo
    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        connection.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = connection.BeginTransaction
    '        comando.Connection = connection
    '        comando.Transaction = transaccion

    '        Try
    '            Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

    '            comando.CommandText = Consulta
    '            Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

    '            LectorUsuario.Read()

    '            If LectorUsuario.HasRows Then
    '                Dim idagencia As Integer = LectorUsuario("idagencia")
    '                Dim idempresa As Integer = LectorUsuario("idempresa")

    '                LectorUsuario.Close()

    '                'Consulta = "select correlativo,final from Facturacion.agenciacaja where id_agenciacaja=" & serie & ";"
    '                'Consulta = "select idtarjetatalonario, numerotarjeta, id_agenciacaja, id_usr, id_agc, id_empsa, fechacreacion, estado, correlativoinicial,   correlativoinicial correlativo    , correlativofinal, correlativofinal final     from Boletos.tarjetatalonario  WHERE numerotarjeta= " & tarjetanumero & "  and " & ultimocorrelativo & " between correlativoinicial and correlativofinal   ;"

    '                Consulta = "select  tt.correlativoinicial  correlativo,  tt.correlativofinal final " +
    '                " ,ags.id_agenciacaja " +
    '                " ,t.numerotarjeta numerotarjeta " +
    '                " ,tt.idtarjetatalonario, tt.numerotarjeta, tt.id_agenciacaja, tt.id_usr, tt.id_agc, tt.id_empsa, tt.fechacreacion, tt.estado, tt.correlativoinicial, tt.correlativofinal " +
    '                " from Boletos.tarjetatalonario tt " +
    '                " INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
    '                " INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
    '                " WHERE tt.numerotarjeta= " & tarjetanumero & "  and " & correlativoaoperar & " between tt.correlativoinicial and tt.correlativofinal " +
    '                " AND ags.id_agenciacaja=" & serie & ";"

    '                ' MsgBox(Consulta)


    '                comando.CommandText = Consulta
    '                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

    '                LectorTarjeta.Read()
    '                If LectorTarjeta.HasRows Then
    '                    Dim inicial As Integer = LectorTarjeta("correlativoinicial")
    '                    Dim final As Integer = LectorTarjeta("correlativofinal")

    '                    LectorTarjeta.Close()
    '                    Elemento = New Serie
    '                    Elemento.mensaje = "Dato encontrado"


    '                    '                        If correlativoactual + 1 > ultimocorrelativo Then

    '                    If correlativoaoperar < inicial Then  ' es menor que el rango inicial 

    '                        ' MsgBox(correlativoaoperar)
    '                        ' MsgBox(inicial)


    '                        Elemento.correcto = False
    '                        Elemento.mensaje = "El ultimo correlativo ingresado es menor que el rango inicial "



    '                    ElseIf correlativoaoperar > final Then
    '                        Elemento.correcto = False
    '                        Elemento.mensaje = "El rango vendido es invalido"

    '                    Else
    '                        Elemento.correcto = True
    '                        Elemento.correlativo = correlativoaoperar
    '                        Elemento.mensaje = "Rango valido"
    '                    End If

    '                Else
    '                    Elemento = New Serie
    '                    Elemento.correcto = False
    '                    Elemento.mensaje = "No se ha encontrado la serie o el correlativo no esta en el rango"
    '                End If

    '                'transaccion.Commit()
    '            Else
    '                LectorUsuario.Close()
    '                Elemento = New Serie
    '                Elemento.correcto = False
    '                Elemento.mensaje = "No se establecieron los datos del usuario"
    '            End If
    '        Catch ex As Exception
    '            Elemento = New Serie
    '            Elemento.correcto = False
    '            Elemento.mensaje = "ERROR: " + ex.Message
    '            'transaccion.Rollback()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    '    Return Elemento
    'End Function

    <WebMethod()> _
    Public Function buscarSerie(ByVal serie As String, ByVal ultimocorrelativo As Integer, ByVal usuario As String, ByVal tarjetanumero As Integer) As Serie
        Dim Elemento As Serie

        tarjetanumero = tarjetanumero.ToString


        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    'Consulta = "select correlativo,final from Facturacion.agenciacaja where id_agenciacaja=" & serie & ";"



                    Consulta = "select  (ags.Correlativo+1) correlativo, tt.correlativofinal final " +
                    " from Boletos.tarjetatalonario tt " +
                    " INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
                    " INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
                    " WHERE tt.numerotarjeta= " & tarjetanumero & " " +
                    " AND ags.id_agenciacaja=" & serie & ";"

                    '" WHERE tt.numerotarjeta= " & tarjetanumero & "  and " & correlativoaoperar & " between tt.correlativoinicial and tt.correlativofinal " +






                    comando.CommandText = Consulta
                    Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                    LectorTarjeta.Read()
                    If LectorTarjeta.HasRows Then
                        Dim correlativoactual As Integer = LectorTarjeta("correlativo")
                        Dim final As Integer = LectorTarjeta("final")

                        LectorTarjeta.Close()
                        Elemento = New Serie
                        Elemento.mensaje = "Dato encontrado"

                        'If correlativoactual + 1 > ultimocorrelativo Then
                        If correlativoactual > ultimocorrelativo Then

                            Elemento.correcto = False
                            Elemento.mensaje = "El ultimo correlativo ingresado es mayor que el correlativo actual"
                        ElseIf ultimocorrelativo > final Then
                            Elemento.correcto = False
                            Elemento.mensaje = "El rango vendido es invalido"

                        Else
                            Elemento.correcto = True
                            'Elemento.correlativo = correlativoactual + 1
                            Elemento.correlativo = correlativoactual

                            Elemento.mensaje = "Rango valido"
                        End If

                    Else
                        Elemento = New Serie
                        Elemento.correcto = False
                        Elemento.mensaje = "No se ha encontrado la serie"
                    End If

                    'transaccion.Commit()
                Else
                    LectorUsuario.Close()
                    Elemento = New Serie
                    Elemento.correcto = False
                    Elemento.mensaje = "No se establecieron los datos del usuario"
                End If
            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return Elemento
    End Function

    <WebMethod()> _
    Public Function talonarioUtilizadosEntregar(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idagenciacaja As Integer, ByVal correlativoinicial As Integer, ByVal correlativofinal As Integer, ByVal correlativoutilizado As Integer, ByVal valor As Decimal, ByVal nombrepiloto As String) As List(Of Serie)
        numerotarjeta = numerotarjeta.ToString
        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    '           call Boletos.sp_tarjetatalonario(@xOpcion, @xnumerotarjeta, @xid_agenciacaja, @xid_usr, @xid_agc, @xid_empsa, @xcorrelativoinicial, @xcorrelativofinal, @xcorrelativoutilizado, @xvalor, @xnombrepiloto);
                    Consulta = "call Boletos.sp_tarjetatalonario(2," & numerotarjeta & "," & idagenciacaja & ",'" & usuario & "'," & idagencia & "," & idempresa & "," & correlativoinicial & "," & correlativofinal & "," & correlativoutilizado & "," & valor & ",'" & nombrepiloto & "')"



                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()

                    transaccion.Commit()

                    Elemento = New Serie
                    Elemento.correcto = True
                    Elemento.mensaje = "Datos creados correctamente"
                    ArregloSerie.Add(Elemento)
                Else
                    LectorUsuario.Close()
                    Elemento = New Serie
                    Elemento.correcto = False
                    Elemento.mensaje = "ERROR: No se establecieron los datos "
                    ArregloSerie.Add(Elemento)
                End If

            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function

    <WebMethod()> _
    Public Function muestraSeries2(ByVal usuario As String, ByVal numerotarjeta As Integer) As List(Of Serie)


        numerotarjeta = numerotarjeta.ToString

        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = ""

                Consulta = "select ags.id_agenciacaja correlativo " +
                  ",t.numerotarjeta numerotarjeta " +
                  ",tt.id_usr idusuario " +
                  ",tt.fechacreacion " +
                  ",CONCAT(ags.serie,' | ',CAST(tt.correlativoinicial as char(10)),' - ',CAST(tt.correlativofinal as char(10))) as serie" +
                  ",ifnull(tt.correlativoinicial,0) correlativoinicial " +
                  ",ifnull(tt.correlativofinal,0)   correlativofinal " +
                  ",tt.idtarjetatalonario,  tt.id_agenciacaja, tt.id_agc, tt.id_empsa, tt.fechacreacion, tt.estado, tt.fecharecepcion, tt.facturasentregadas, tt.facturasutilizadas, tt.facturasdevueltas, tt.nombrepiloto, tt.correlativoinicialutilizado, tt.correlativofinalutilizado, tt.valor" +
                 " from Boletos.tarjetatalonarioliquidacion tt " +
                " INNER JOIN Boletos.tarjeta t ON tt.numerotarjeta=t.numerotarjeta " +
                " INNER JOIN Facturacion.agenciacaja ags ON tt.id_agenciacaja=ags.id_agenciacaja " +
                " WHERE t.numerotarjeta=" & numerotarjeta & ";"

                comando.CommandText = Consulta
                Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader()

                While LectorTarjeta.Read()
                    'Dim serie As String = 
                    'Dim idusuario As String = 
                    'Dim fechacreacion As String =
                    '
                    Elemento = New Serie
                    Elemento.correlativo = LectorTarjeta("correlativo")
                    Elemento.serie = LectorTarjeta("serie")
                    Elemento.usuario = LectorTarjeta("idusuario")
                    Elemento.fechacreacion = LectorTarjeta("fechacreacion")

                    Elemento.correlativoinicial = LectorTarjeta("correlativoinicial")
                    Elemento.correlativofinal = LectorTarjeta("correlativofinal")


                    ArregloSerie.Add(Elemento)
                End While
                LectorTarjeta.Close()


            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                'transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function


    <WebMethod(True)> _
    Public Function TarjetaTalonario_LiquidarFacturasPdf(ByVal numerotarjeta As Integer, ByVal usuario As String) As String
        Dim strRutaArchivoReservacion As String = Server.MapPath("~/privado/boletopos/")
        Dim strNombreArchivoReservacionPdf As String = "LIQUIDACION" & numerotarjeta & ".PDF"

        Dim dtEncabezado As DataTable
        Dim Encabezado As String = "call Boletos.sp_tarjetatalonario_liquidarfacturas('Encabezado'," & numerotarjeta & ");"
        dtEncabezado = DatosSql.cargar_cuadrecaja(Encabezado)

        Dim dtDetalle As DataTable
        Dim Detalle As String = "call Boletos.sp_tarjetatalonario_liquidarfacturas('Detalle'," & numerotarjeta & ");"
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

                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Liquidacion de Facturas</b></td></tr>")





                sb.Append("<tr><td colspan = '2'></td></tr>")
                sb.Append("<tr><td><b>Tarjeta No: </b>" & dtEncabezado.Rows(0).Item("numerotarjeta").ToString & "</td><td align = 'right'><b>Fecha y Hora de Impresion: </b> </td></tr>  ")
                sb.Append("<tr><td colspan = '2'></td></tr>")
                sb.Append("<tr><td><b>Piloto: </b>")
                sb.Append(dtEncabezado.Rows(0).Item("nombrepiloto").ToString)
                sb.Append("</td><td align = 'right'><b></b>")
                sb.Append(Date.Now)
                sb.Append(" </td></tr>")




                sb.Append(" </td></tr>")
                sb.Append("<tr><td colspan = '2'><b>Usuario: </b>")
                sb.Append(dtEncabezado.Rows(0).Item("id_usr").ToString)
                sb.Append("</td></tr>")
                sb.Append("</table>")
                sb.Append("<br />")



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

                sb.Append("<table widtEncabezadoh='100%' cellspacing='5' cellpadding='2' border= '1' >   ")
                sb.Append("<tr><td align='left' style='background-color: #18B5F0' colspan = '2'><b>Firma Piloto:</b></td></tr>")
                sb.Append("<tr><td align='left' style='background-color: #18B5F0' colspan = '2'><b>Firma  Recibido:</b></td></tr>")
                sb.Append("</tr></table>")


                'Export HTML String as PDF.
                Dim sr As New StringReader(sb.ToString())
                'Dim pdfDoc As New Document(PageSize.LETTER, 10.0F, 10.0F, 10.0F, 0.0F)
                Dim htmlparser As New HTMLWorker(doc)

                doc.Open()
                htmlparser.Parse(sr)
                doc.Close()


                Return "downloadfiles.aspx?ruta=" & strRutaArchivoReservacion & "&archivo=" & strNombreArchivoReservacionPdf & ""




            End Using
        End Using

    End Function

    <WebMethod(True)> _
    Public Function TarjetaTalonario_EntregaFacturasPdf(ByVal numerotarjeta As Integer, ByVal usuario As String) As String
        Dim strRutaArchivoReservacion As String = Server.MapPath("~/privado/boletopos/")
        Dim strNombreArchivoReservacionPdf As String = "ASIGNACION" & numerotarjeta & ".PDF"

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

                sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Asignacion de Facturas</b></td></tr>")


                sb.Append("<tr><td colspan = '2'></td></tr>")
                sb.Append("<tr><td><b>Tarjeta No: </b>" & dtEncabezado.Rows(0).Item("numerotarjeta").ToString & "</td><td align = 'right'><b>Fecha y Hora de Impresión: </b> </td></tr>  ")
                sb.Append("<tr><td colspan = '2'></td></tr>")
                sb.Append("<tr><td><b>Piloto: </b>")
                sb.Append(dtEncabezado.Rows(0).Item("nombrepiloto").ToString)
                sb.Append("</td><td align = 'right'><b></b>")
                sb.Append(Date.Now)
                sb.Append(" </td></tr>")




                sb.Append(" </td></tr>")
                sb.Append("<tr><td colspan = '2'><b>Usuario: </b>")
                sb.Append(dtEncabezado.Rows(0).Item("id_usr").ToString)
                sb.Append("</td></tr>")
                sb.Append("</table>")
                sb.Append("<br />")



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

                sb.Append("<table widtEncabezadoh='100%' cellspacing='5' cellpadding='2' border= '1' >   ")
                sb.Append("<tr><td align='left' style='background-color: #18B5F0' colspan = '2'><b>Firma  Entregado:</b></td></tr>")
                sb.Append("<tr><td align='left' style='background-color: #18B5F0' colspan = '2'><b>Firma Piloto:</b></td></tr>")

                sb.Append("</tr></table>")

                'Export HTML String as PDF.
                Dim sr As New StringReader(sb.ToString())
                'Dim pdfDoc As New Document(PageSize.LETTER, 10.0F, 10.0F, 10.0F, 0.0F)
                Dim htmlparser As New HTMLWorker(doc)

                doc.Open()
                htmlparser.Parse(sr)
                doc.Close()

                Return "downloadfiles.aspx?ruta=" & strRutaArchivoReservacion & "&archivo=" & strNombreArchivoReservacionPdf & ""

            End Using
        End Using

    End Function

    <WebMethod()> _
    Public Function asignarSerieTarjeta(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idagenciacaja As Integer, ByVal correlativoinicial As Integer, ByVal correlativofinal As Integer, ByVal nombrepiloto As String) As List(Of Serie)
        numerotarjeta = numerotarjeta.ToString
        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    Consulta = "call Boletos.sp_tarjetatalonario(1," & numerotarjeta & "," & idagenciacaja & ",'" & usuario & "'," & idagencia & "," & idempresa & "," & correlativoinicial & "," & correlativofinal & ",0,0,'" & nombrepiloto & "')"

                    comando.CommandText = Consulta
                    comando.ExecuteNonQuery()

                    transaccion.Commit()

                    Elemento = New Serie
                    Elemento.correcto = True
                    Elemento.mensaje = "Datos creados correctamente"
                    ArregloSerie.Add(Elemento)
                Else
                    LectorUsuario.Close()
                    Elemento = New Serie
                    Elemento.correcto = False
                    Elemento.mensaje = "ERROR: No se establecieron los datos "
                    ArregloSerie.Add(Elemento)
                End If

            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function


    <WebMethod()> _
    Public Function ExcelImprimeResumen(ByVal FechaInicial As String, ByVal FechaFinal As String, ByVal Agencia As String, ByVal Servicio As String, ByVal Vehiculo As String, ByVal Piloto As String, ByVal Usuario As String) As String
        FechaFinal = FechaFinal.ToString
        FechaInicial = FechaInicial.ToString
        Agencia = Agencia.ToString
        Servicio = Servicio.ToString
        Vehiculo = Vehiculo.ToString
        Piloto = Piloto.ToString

        Dim strRutaArchivoReservacion As String = Server.MapPath("~/privado/boletopos/")
        Dim strNombreArchivoReservacionPdf As String = "Reporte.csv"







        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Try
                Dim TablaDetalle As New DataTable
                Dim SQLConsulta As String = "call Boletos.sp_resumen_tarjetas_ruta('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "');"





                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)

                'Dim informacion As String = ""
                Dim GridView1 As New GridView
                GridView1.AllowPaging = False
                GridView1.DataSource = TablaDetalle
                GridView1.DataBind()





                Context.Response.Clear()
                Context.Response.Buffer = True
                Context.Response.AddHeader("content-disposition", "attachment;filename=" & strRutaArchivoReservacion & strNombreArchivoReservacionPdf)

                Context.Response.Charset = ""
                Context.Response.ContentType = "application/vnd.ms-excel"
                Using sw As New StringWriter()
                    Dim hw As New HtmlTextWriter(sw)

                    GridView1.HeaderRow.BackColor = Color.White

                    For Each row As GridViewRow In GridView1.Rows
                        row.BackColor = Color.White
                        For Each cell As TableCell In row.Cells
                            cell.CssClass = "textmode"
                        Next
                    Next

                    GridView1.RenderControl(hw)
                    'style to format numbers to string
                    Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
                    Context.Response.Write(style)
                    Context.Response.Output.Write(sw.ToString())
                    Context.Response.Flush()
                    Context.Response.End()
                End Using

                'Return "Reporte.csv"
                Dim hola As String = "hola"

                Return "downloadfiles.aspx?ruta=" & strRutaArchivoReservacion & "&archivo=" & strNombreArchivoReservacionPdf & ""

            Catch ex As Exception
                Return "Error" + ex.ToString
            End Try

        End Using

    End Function




    Public Function idEmpresa(ByVal usuario As String) As Integer

        Dim ridEmpresa As Integer = 0


        Dim Busqueda As String = " select e.nom_empsa empresa,a.nom_agc agencia , u.id_empsa " +
                                                " from Facturacion.usuario u " +
                                                " INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                " INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa  " +
                                                " where u.id_usr='" & usuario & "';"






        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    ridEmpresa = reader("id_empsa")
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return ridEmpresa
    End Function

    Public Shared Function SinFaceActualizaTransaccionGface(ByVal Serie As String, ByVal Correlativo As String, ByVal Cae As String, ByVal Face As String, ByVal IdTransaccion As String) As String
        Dim PosAlmacenado As String = "", PosActualizado As String = ""
        Dim strsql As String = ""
        Dim comando As MySqlCommand
        Dim estado As String

        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


            'MsgBox("aqui voy a")
            strsql = "update Facturacion.transaccionesgface set num_fac=" & Correlativo & ",cae='" & Cae & "',face='" & Face & "', factura_detalle=replace(factura_detalle,'new.num_fac','" & Correlativo & "'),estatus_factura=true where idtransaccion='" & IdTransaccion & "';"

            'MsgBox("aqui voy b")



            comando = New MySqlCommand(strsql, conexion)


            Try
                conexion.Open()

                estado = comando.ExecuteNonQuery

                If estado >= 1 Then
                    Return "Actualizada"
                Else
                    Return "Error"
                End If

            Catch ex As MySqlException

                strsql = "update Facturacion.transaccionesgface set error='" & Replace(ex.Message.ToString, "'", "") & "' where idtransaccion='" & IdTransaccion & "';"
                comando = New MySqlCommand(strsql, conexion)
                estado = comando.ExecuteNonQuery

                Return ex.Message.ToString

            Finally
                conexion.Close()
            End Try
        End Using
    End Function

    Public Shared Function SinFaceGenerarFacturaElectronica(ByVal dispositivo As String, ByVal nitcliente As String, ByVal serie As String, ByVal totalfactura As String, ByVal idtransaccion As String, ByVal numeroresolucion As String, ByVal nombrecliente As String, ByVal direccioncliente As String, ByVal direccionagencia As String, ByVal fecharesolucion As String, ByVal idagencia As String, ByVal idagenciacaja As Integer) As String
        Dim face As String = "", numerofactura As String = "", cae As String = "", cae1 As String = "", cae2 As String = "", detalles As String = ""


        'MsgBox("aqui voy 5")



        Dim strsql As String = ""
        Dim comando As MySqlCommand
        'Dim fechaactual As Date
        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


            'MsgBox("aqui voy 6")



            strsql = "Select cast( md5( concat( curdate(),curtime() )  ) as char(132) );"
            comando = New MySqlCommand(strsql, conexion)
            conexion.Open()

            'MsgBox("aqui voy 7")


            cae = comando.ExecuteScalar


            cae1 = cae




        End Using



        'Dim fechaactual As Date
        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)

            'strsql = "SELECT ifnull(max(num_fac),0) +1 UltimoCorrelativo  FROM Facturacion.factura where serie_fac='" & serie & "';"
            strsql = "SELECT (Correlativo+1) UltimoCorrelativo  FROM Facturacion.agenciacaja where id_agenciacaja=" & idagenciacaja & ";"





            comando = New MySqlCommand(strsql, conexion)
            conexion.Open()

            numerofactura = comando.ExecuteScalar


        End Using

        face = "CFACE1" & serie & numerofactura



        'CFACE1Z60000057735

        '    'Val dispositivo As String, ByVal nitcliente As String, ByVal serie As String, ByVal totalfactura As String, ByVal idtransaccion As String, ByVal numeroresolucion As String, ByVal nombrecliente As String, ByVal direccioncliente As String, ByVal direccionagencia As String, ByVal fecharesolucion As String, ByVal idagencia As String) As String






        Return face & "|" & cae & "|" & numerofactura & "|" & cae1 & "|" & cae2 & "|" & detalles & "|" & "valido"

    End Function

    <WebMethod()> _
    Public Function TotalValeElectronico(ByVal NumeroTarjeta As Integer, ByVal Usuario As String) As String

        Dim Resultado As String = ""
        Dim Estilo As String = ""
        Dim color As Boolean = True

        Dim TotalTarjeta As Decimal = 0.0, TotalCarga As Decimal = 0.0, TotalBoletosAgencia As Decimal = 0.0, TotalBoletosRuta As Decimal = 0.0
        Dim TotalBoletosRutaElectronico As Decimal = 0.0




        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia " +
                                                        "from Facturacion.usuario u " +
                                                        "INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc " +
                                                        "where u.id_usr='" & Usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta_electronico('" & NumeroTarjeta & "');"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)


                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Resultado = "<div class='formRow'>"


                For fila = 0 To TablaDetalle.Rows.Count - 1

                    Resultado += "<div class='gridvale' style=' margin-left: 13.5px; '> <h6>Fecha Viaje</h6> " & TablaDetalle.Rows(fila).Item("FechaViaje") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Tarjeta #</h6> " & TablaDetalle.Rows(fila).Item("Tarjeta") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Vehiculo</h6> " & TablaDetalle.Rows(fila).Item("Vehiculo") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Horarios</h6> " & TablaDetalle.Rows(fila).Item("Hora") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Pilotos</h6> " & TablaDetalle.Rows(fila).Item("Piloto") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Asistente</h6> " & TablaDetalle.Rows(fila).Item("Asistente") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total B. Ruta</h6> " & TablaDetalle.Rows(fila).Item("TotalBoletosRuta") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total B. Ruta Electronico</h6> " & TablaDetalle.Rows(fila).Item("TotalBoletosRutaElectronico") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total Carga</h6> " & TablaDetalle.Rows(fila).Item("TotalCarga") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total Efectivo</h6> " & TablaDetalle.Rows(fila).Item("TotalEfectivo") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total Total Vale</h6> " & TablaDetalle.Rows(fila).Item("TotalTarjeta") & "</div>"



                    '                     Resultado += "<div class='gridvale' style=' text-align: center;font-size: 1.6em; width: 95.5%; background-color: rgb(202, 202, 202); '> <h6>Total Vale</h6> " & TablaDetalle.Rows(fila).Item("TotalTarjeta") & "</div>"

                Next



                Resultado += "</div>"




                reader.Close()
                Return Resultado

            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    <WebMethod()> _
    Public Function GuardarValeElectronico(ByVal NumeroTarjeta As Integer, ByVal Usuario As String) As String

        Dim Resultado As String = ""
        Dim Estilo As String = ""
        Dim color As Boolean = True

        Dim TotalTarjeta As Decimal = 0.0, TotalCarga As Decimal = 0.0, TotalEfectivo As Decimal = 0.0, TotalBoletosRuta As Decimal = 0.0
        Dim Agencia As String = "", Empresa As String = ""
        Dim Idvale As Integer
        Dim TotalBoletosRutaElectronico As Decimal = 0.0






        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia, u.id_agc id_agencia,u.id_empsa id_empresa " +
                                                        "   from Facturacion.usuario u " +
                                                        "      INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "      INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa   " +
                                                        "         where u.id_usr='" & Usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta_electronico('" & NumeroTarjeta & "');"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)


                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Agencia = TablaDatosEmpresa.Rows(0).Item("id_agencia")
                Empresa = TablaDatosEmpresa.Rows(0).Item("id_empresa")
                TotalBoletosRuta = TablaDetalle.Rows(0).Item("TotalBoletosRuta")
                TotalBoletosRutaElectronico = TablaDetalle.Rows(0).Item("TotalBoletosRutaElectronico")
                TotalEfectivo = TablaDetalle.Rows(0).Item("TotalEfectivo")
                TotalCarga = TablaDetalle.Rows(0).Item("TotalCarga")
                TotalTarjeta = TablaDetalle.Rows(0).Item("TotalTarjeta")



                reader.Close()


                If TotalTarjeta > 0 Then


                    SQLConsulta = " insert into Boletos.valesruta(id_tarjeta,total_excursion,total_efectivo,total_boletos_ruta,total_carga,total_vale,agencia,usuario,empresa,fecha,hora)" +
                              " values(" & NumeroTarjeta & ",0," & TotalEfectivo & "," & (TotalBoletosRuta + TotalBoletosRutaElectronico) & ", " & TotalCarga & "," & TotalTarjeta & "," & Agencia & ",'" & Usuario & "','" & Empresa & "',curdate(),curtime());"

                    Comando.CommandText = SQLConsulta
                    Comando.ExecuteScalar()


                    SQLConsulta = "select @@identity;"
                    Comando.CommandText = SQLConsulta
                    Idvale = Comando.ExecuteScalar

                    Resultado = ValePdfElectronico(NumeroTarjeta, Idvale, TotalBoletosRuta, TotalEfectivo, TotalCarga, TotalTarjeta, Usuario)


                    SQLConsulta = "update Boletos.tarjetadetalledocumentos set vale_numero=" & Idvale & " where numerotarjeta=" & NumeroTarjeta & ";"
                    Comando.CommandText = SQLConsulta
                    Comando.ExecuteScalar()

                    Return Resultado

                Else


                    Return "ERROR:EL MONTO DEL VALE ES INVALIDO"

                End If



            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    <WebMethod()> _
    Public Function TotalVale(ByVal NumeroTarjeta As Integer, ByVal Usuario As String) As String

        Dim Resultado As String = ""
        Dim Estilo As String = ""
        Dim color As Boolean = True

        Dim TotalTarjeta As Decimal = 0.0, TotalCarga As Decimal = 0.0, TotalBoletosAgencia As Decimal = 0.0, TotalBoletosRuta As Decimal = 0.0




        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia " +
                                                        "    from Facturacion.usuario u " +
                                                        "        INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "        INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa " +
                                                        "            where u.id_usr='" & Usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta('" & NumeroTarjeta & "');"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)


                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Resultado = "<div class='formRow'>"


                For fila = 0 To TablaDetalle.Rows.Count - 1

                    Resultado += "<div class='gridvale' style=' margin-left: 13.5px; '> <h6>Fecha Viaje</h6> " & TablaDetalle.Rows(fila).Item("FechaViaje") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Tarjeta #</h6> " & TablaDetalle.Rows(fila).Item("Tarjeta") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Vehiculo</h6> " & TablaDetalle.Rows(fila).Item("Vehiculo") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Horarios</h6> " & TablaDetalle.Rows(fila).Item("Hora") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Pilotos</h6> " & TablaDetalle.Rows(fila).Item("Piloto") & "</div>"
                    Resultado += "<div class='gridvale'> <h6>Asistente</h6> " & TablaDetalle.Rows(fila).Item("Asistente") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total B. Ruta</h6> " & TablaDetalle.Rows(fila).Item("TotalBoletosRuta") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total Carga</h6> " & TablaDetalle.Rows(fila).Item("TotalCarga") & "</div>"
                    Resultado += "<div class='gridvale' style=' font-size: 1.6em; '> <h6>Total Efectivo</h6> " & TablaDetalle.Rows(fila).Item("TotalEfectivo") & "</div>"
                    Resultado += "<div class='gridvale' style=' text-align: center;font-size: 1.6em; width: 95.5%; background-color: rgb(202, 202, 202); '> <h6>Total Vale</h6> " & TablaDetalle.Rows(fila).Item("TotalTarjeta") & "</div>"

                Next



                Resultado += "</div>"




                reader.Close()
                Return Resultado

            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function


    <WebMethod()> _
    Public Function GuardarVale(ByVal NumeroTarjeta As Integer, ByVal Usuario As String) As String

        Dim Resultado As String = ""
        Dim Estilo As String = ""
        Dim color As Boolean = True

        Dim TotalTarjeta As Decimal = 0.0, TotalCarga As Decimal = 0.0, TotalEfectivo As Decimal = 0.0, TotalBoletosRuta As Decimal = 0.0
        Dim Agencia As String = "", Empresa As String = ""
        Dim Idvale As Integer






        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia, u.id_agc id_agencia,u.id_empsa id_empresa " +
                                                        "   from Facturacion.usuario u " +
                                                        "       INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "       INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa  " +
                                                        "           where u.id_usr='" & Usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta('" & NumeroTarjeta & "');"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)


                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Agencia = TablaDatosEmpresa.Rows(0).Item("id_agencia")
                Empresa = TablaDatosEmpresa.Rows(0).Item("id_empresa")
                TotalBoletosRuta = TablaDetalle.Rows(0).Item("TotalBoletosRuta")
                TotalEfectivo = TablaDetalle.Rows(0).Item("TotalEfectivo")
                TotalCarga = TablaDetalle.Rows(0).Item("TotalCarga")
                TotalTarjeta = TablaDetalle.Rows(0).Item("TotalTarjeta")



                reader.Close()


                If TotalTarjeta > 0 Then


                    SQLConsulta = " insert into Boletos.valesruta(id_tarjeta,total_excursion,total_efectivo,total_boletos_ruta,total_carga,total_vale,agencia,usuario,empresa,fecha,hora)" +
                              " values(" & NumeroTarjeta & ",0," & TotalEfectivo & "," & TotalBoletosRuta & ", " & TotalCarga & "," & TotalTarjeta & "," & Agencia & ",'" & Usuario & "','" & Empresa & "',curdate(),curtime());"

                    Comando.CommandText = SQLConsulta
                    Comando.ExecuteScalar()


                    SQLConsulta = "select @@identity;"
                    Comando.CommandText = SQLConsulta
                    Idvale = Comando.ExecuteScalar

                    Resultado = ValePdf(NumeroTarjeta, Idvale, TotalBoletosRuta, TotalEfectivo, TotalCarga, TotalTarjeta, Usuario)


                    SQLConsulta = "update Boletos.tarjetadetalledocumentos set vale_numero=" & Idvale & " where numerotarjeta=" & NumeroTarjeta & ";"
                    Comando.CommandText = SQLConsulta
                    Comando.ExecuteScalar()

                    Return Resultado

                Else


                    Return "ERROR:EL MONTO DEL VALE ES INVALIDO"

                End If



            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    <WebMethod()> _
    Public Function ValePdfElectronico(ByVal numerotarjeta As Integer, ByVal numerovale As Integer, ByVal TotalBoletosRuta As Decimal, ByVal TotalEfectivo As Decimal, ByVal TotalCarga As Decimal, ByVal TotalVale As Decimal, ByVal usuario As String) As String

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

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 10, 10, 20, 10)
                doc.SetPageSize(PageSize.LETTER)

                Dim Correlativo As Integer = NumeroAleatorioPdf()
                Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\privado\pdf\VALE-" + Correlativo.ToString + ".pdf"), FileMode.Create))
                doc.AddTitle("VALE " + Correlativo.ToString)
                doc.AddAuthor("LINEAS TERRESTRES GUATEMALTECAS S.A")
                doc.AddCreationDate()

                doc.Open()

                Dim Tabla As PdfPTable = New PdfPTable(7)

                Tabla.TotalWidth = 550
                Tabla.LockedWidth = True
                Tabla.SetWidths({18, 13, 13, 13, 13, 13, 13})


                Dim SqlConsultaDatosUsuario As String = "select e.nom_empsa empresa,a.nom_agc agencia " +
                                                        "   from Facturacion.usuario u " +
                                                        "       INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        "       INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa  " +
                                                        "           where u.id_usr='" & usuario & "'; "

                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)


                Dim SQLConsulta As String = "Call Boletos.sp_vales_ruta_electronico(" & numerotarjeta & ");"
                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()
                TablaDetalle.Load(reader)

                'DECLARACION DE LA CELDA A UTILIZAR EN LA TABLAPDF
                Dim Celda As PdfPCell = New PdfPCell

                ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                Celda = New PdfPCell(New Paragraph("EMPRESA:", FontFactory.GetFont("Palatino Linotype", 10, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("empresa").ToString), FontFactory.GetFont("Palatino Linotype", 11, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Celda.Colspan = 4
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph("VALE RUTA / ELECTRONICO#", FontFactory.GetFont("Palatino Linotype", 15, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Celda.HorizontalAlignment = Element.ALIGN_CENTER
                Celda.Colspan = 2
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph("AGENCIA:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(TablaDatosEmpresa.Rows(0).Item("agencia").ToString), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Celda.Colspan = 2
                Tabla.AddCell(Celda)


                Celda = New PdfPCell(New Paragraph("USUARIO:", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(UCase(usuario), FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(numerovale.ToString, FontFactory.GetFont("Palatino Linotype", 20, iTextSharp.text.Font.BOLD)))
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
                Celda.Colspan = 4
                Tabla.AddCell(Celda)


                Celda = New PdfPCell(New Paragraph("TARJETA #  :", FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.NORMAL)))
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Celda = New PdfPCell(New Paragraph(TablaDetalle.Rows(0).Item("Tarjeta").ToString, FontFactory.GetFont("Palatino Linotype", 14, iTextSharp.text.Font.BOLD)))
                Celda.BorderWidth = 0
                Celda.Colspan = 4
                Tabla.AddCell(Celda)






                Celda = New PdfPCell(New Paragraph("   ", FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                Celda.Colspan = 7
                Celda.BorderWidth = 0
                Tabla.AddCell(Celda)

                Dim informacion As String = ""
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



                    BoletosAgencia = 0
                    BoletosRuta = 0

                    TotalHorarioAgencia = 0
                    TotalHorarioRuta = 0

                    informacion = "FECHA VIAJE:"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)

                    informacion = "HORA VIAJE:"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)

                    informacion = "PILOTO"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = "ASISTENTE"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)




                    ''==============SEGUNDA FILA====================''


                    informacion = TablaDetalle.Rows(0).Item("FechaViaje")
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = TablaDetalle.Rows(0).Item("Hora").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1

                    Tabla.AddCell(Celda)


                    informacion = TablaDetalle.Rows(0).Item("Piloto").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.BorderWidth = 1
                    Celda.Colspan = 2
                    Tabla.AddCell(Celda)

                    informacion = TablaDetalle.Rows(0).Item("Asistente").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.NORMAL)))
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)




                    ''=============TERCERA FILA===============''
                    informacion = "TOTAL B. RUTA /ELECTRONICO"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    informacion = "TOTAL CARGA"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = "TOTAL EFECTIVO"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)


                    informacion = "TOTAL VALE."
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.BackgroundColor = New iTextSharp.text.BaseColor(240, 240, 240)
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)






                    informacion = TablaDetalle.Rows(0).Item("TotalBoletosRuta").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)





                    informacion = ""
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = ""
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = ""
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)












                    informacion = TablaDetalle.Rows(0).Item("TotalBoletosRutaElectronico").ToString
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)



                    informacion = Decimal.Round(TotalCarga, 2)
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = Decimal.Round(TotalEfectivo, 2)
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)

                    informacion = TotalVale
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 14, iTextSharp.text.Font.BOLD)))
                    Celda.HorizontalAlignment = Element.ALIGN_RIGHT
                    Celda.Colspan = 2
                    Celda.BorderWidth = 1
                    Tabla.AddCell(Celda)





                    '===================FIRMAS=================


                    informacion = " "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 7
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = " "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 7
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = "  ______________________________"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 3
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)

                    informacion = "   _____________________________"
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 4
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)


                    informacion = "            NOMBRE Y FIRMA PILOTO    "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 3
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)

                    informacion = "      NOMBRE Y FIRMA CAJERO  "
                    Celda = New PdfPCell(New Paragraph(informacion, FontFactory.GetFont("Arial", 13, iTextSharp.text.Font.BOLD)))
                    Celda.Colspan = 4
                    Celda.BorderWidth = 0
                    Tabla.AddCell(Celda)







                End If
                doc.Add(Tabla)



                doc.Close()


                reader.Close()

                Return "pdf/VALE-" + Correlativo.ToString + ".pdf"


            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try

        End Using

    End Function

    '<WebMethod()> _
    'Public Function ResumenTarjetas2(ByVal FechaInicial As String, ByVal FechaFinal As String, ByVal Agencia As String, ByVal Servicio As String, ByVal Vehiculo As String, ByVal Piloto As String, ByVal Usuario As String) As List(Of [ClaseServicios])


    '    'Dim fid_empsa = idEmpresa(Usuario)

    '    'Dim Busqueda As String = "select idservicio as id_servicio,descripcion as nombre_servicio from Boletos.servicio where  id_empsa=" & fid_empsa & " order by nombre_servicio;"

    '    Dim Busqueda As String = "call Boletos.sp_resumen_tarjetas_ruta('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "');"


    '    Dim result As List(Of [ClaseServicios]) = New List(Of ClaseServicios)()
    '    Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '        Try
    '            connection.Open()
    '            Dim cmd As New MySqlCommand(Busqueda, connection)
    '            Dim reader As MySqlDataReader = cmd.ExecuteReader()
    '            While (reader.Read())
    '                Dim Elemento As New ClaseServicios

    '                Dim nombre As String = reader("servicio")



    '                Elemento.idservicio = reader("tarjeta")
    '                Elemento.nombreservicio = nombre
    '                result.Add(Elemento)
    '            End While
    '            reader.Close()
    '            cmd.Dispose()
    '        Catch ex As MySqlException
    '        End Try
    '    End Using
    '    Return result

    'End Function

    <WebMethod()> _
    Public Function ResumenTarjetas(ByVal FechaInicial As String, ByVal FechaFinal As String, ByVal Agencia As String, ByVal Servicio As String, ByVal Vehiculo As String, ByVal Piloto As String, ByVal Usuario As String) As List(Of [ClaseTabla])


        'MsgBox("aqui voy 1")


        Dim ArrayTarjetas As Array
        Dim Resultado As String = ""
        Dim Estilo As String = ""
        Dim color As Boolean = True

        Dim result As List(Of [ClaseTabla]) = New List(Of ClaseTabla)()


        Dim TotalTarjeta As Decimal = 0.0, TotalCarga As Decimal = 0.0, TotalBoletosAgencia As Decimal = 0.0, TotalBoletosRuta As Decimal = 0.0




        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConStringReplica").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Dim Comando As New MySqlCommand
            Try

                Dim TablaDetalle As New DataTable
                Dim TablaDatosEmpresa As New DataTable

                Dim SqlConsultaDatosUsuario As String = " select e.nom_empsa empresa,a.nom_agc agencia , u.id_empsa " +
                                                        " from Facturacion.usuario u " +
                                                        " INNER JOIN Facturacion.empresa e ON u.id_empsa=e.id_empsa " +
                                                        " INNER JOIN Facturacion.agencia a ON u.id_agc=a.id_agc and u.id_empsa=a.id_empsa  " +
                                                        " where u.id_usr='" & Usuario & "';"






                Comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                Dim readerUsuario As MySqlDataReader = Comando.ExecuteReader()
                TablaDatosEmpresa.Load(readerUsuario)

                Dim iId_EmpSa As Integer = TablaDatosEmpresa.Rows(0).Item("id_empsa")



                'Boletos.sp_resumen_tarjetas_ruta_x_empresa



                Dim SQLConsulta As String = "call Boletos.sp_resumen_tarjetas_ruta_x_empresa('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "'," & iId_EmpSa & ");"

                'Dim SQLConsulta As String = "call Boletos.sp_resumen_tarjetas_ruta('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "');"

                'MsgBox(SQLConsulta)

                'MsgBox("aqui voy 1 " & SQLConsulta)




                'Dim SQLConsulta As String = "call Boletos.sp_resumen_tarjetas_ruta('" & FechaInicial & "','" & FechaFinal & "','" & Agencia & "','" & Servicio & "','" & Vehiculo & "','" & Piloto & "');"




                'MsgBox(SQLConsulta)


                command = New MySqlCommand(SQLConsulta, connection)
                command.CommandTimeout = 120


                'MySqlCommand cmd = new MySqlCommand();
                'cmd.CommandTimeout = 60;



                Dim reader As MySqlDataReader = command.ExecuteReader()
                reader.Read()


                If reader.HasRows Then

                    TablaDetalle.Load(reader)


                    ''=======================E N C A B E Z A D O   D E   P A G I N A=======================''

                    Resultado += "<table>"

                    'MsgBox("aqui voy 2 " & Resultado)



                    For fila = 0 To TablaDetalle.Rows.Count - 1


                        If (fila Mod 10) = 0 And fila > 1 Then
                            Resultado += "$$#tr"
                        Else
                            Resultado += "#tr"
                        End If

                        TotalTarjeta += TablaDetalle.Rows(fila).Item("TotalTarjeta")
                        TotalTarjeta = Decimal.Round(TotalTarjeta, 2)

                        TotalCarga += TablaDetalle.Rows(fila).Item("TotalCarga")
                        TotalCarga = Decimal.Round(TotalCarga, 2)

                        TotalBoletosAgencia += TablaDetalle.Rows(fila).Item("TotalBoletosAgencia")
                        TotalBoletosAgencia = Decimal.Round(TotalBoletosAgencia, 2)

                        TotalBoletosRuta += TablaDetalle.Rows(fila).Item("TotalBoletosRuta")
                        TotalBoletosRuta = Decimal.Round(TotalBoletosRuta, 2)


                        If color = False Then
                            Estilo = "#off"

                            color = True
                        Else
                            Estilo = "#on"
                            color = False
                        End If

                        For columna = 0 To TablaDetalle.Columns.Count - 1


                            If columna = 1 Then

                                Resultado += "#td" & Estilo & "#pdf" & TablaDetalle.Rows(fila).Item(columna) & "');> " & TablaDetalle.Rows(fila).Item(columna) & "</a></td>"
                            ElseIf columna >= 10 And columna <= 13 Then

                                Resultado += "#td" & Estilo & "#Q" & "'>Q. " & TablaDetalle.Rows(fila).Item(columna) & "</td>"
                            Else
                                Resultado += "#td" & Estilo & "'> " & TablaDetalle.Rows(fila).Item(columna) & "</td>"
                            End If





                        Next


                        'MsgBox("aqui voy 3 " & Resultado)


                        If fila = TablaDetalle.Rows.Count - 1 Then
                            Resultado += "</tr>#tr"



                            For columna = 0 To TablaDetalle.Columns.Count - 1

                                If columna >= 10 Then

                                    Select Case columna

                                        Case 10 : Resultado += "#td" & Estilo & "#Q" & "#dbl" & "'> <p>Q. " & TotalBoletosRuta & "</p></td>"
                                        Case 11 : Resultado += "#td" & Estilo & "#Q" & "#dbl" & "'> <p>Q. " & TotalBoletosAgencia & "</p></td>"
                                        Case 12 : Resultado += "#td" & Estilo & "#Q" & "#dbl" & "'> <p>Q. " & TotalCarga & "</p></td>"
                                        Case 13 : Resultado += "#td" & Estilo & "#Q" & "#dbl" & "'> <p>Q. " & TotalTarjeta & "</p></td>"

                                    End Select

                                ElseIf columna <= 2 Then

                                    If columna = 0 Then
                                        Resultado += "#totales"
                                    End If

                                Else

                                    Resultado += "#td" & Estilo & "#dbl" & "'> <p> </p></td>"

                                End If

                            Next



                        End If

                        Resultado += "</tr>"

                    Next


                    Resultado += "</table>"

                    Dim altos As String = "alto"


                    reader.Close()


                    Dim altoss As String = "alto"



                    ArrayTarjetas = Resultado.Split("$$")

                    Dim contador = 0

                    While contador <= ArrayTarjetas.Length - 1
                        Dim Elemento As New ClaseTabla

                        Elemento.fila = ArrayTarjetas(contador)
                        result.Add(Elemento)
                        contador += 1

                    End While









                End If

                Dim alto As String = "alto"


                Return result


            Catch ex As Exception

                Dim Elemento As New ClaseTabla
                Elemento.fila = "ERROR: " + ex.Message
                result.Add(Elemento)
                Return result

            End Try

        End Using

    End Function





    'Public Shared Function GenerarFacturaElectronica(ByVal dispositivo As String, ByVal nitcliente As String, ByVal serie As String, ByVal totalfactura As String, ByVal idtransaccion As String, ByVal numeroresolucion As String, ByVal nombrecliente As String, ByVal direccioncliente As String, ByVal direccionagencia As String, ByVal fecharesolucion As String, ByVal idagencia As String) As String

    '    Dim dte As New WebReference.dte
    '    Dim deta As New WebReference.detalleDte
    '    Dim registro As New WebReference.requestDte
    '    Dim resultado As New WebReference.responseDte
    '    Dim ws As New WebReference.ingfaceClient
    '    Dim face As String = "", numerofactura As String = "", cae As String = "", cae1 As String = "", cae2 As String = "", detalles As String = ""

    '    Dim strsql As String = ""
    '    Dim comando As MySqlCommand
    '    Dim fechaactual As Date


    '    Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


    '        strsql = "Select curdate();"
    '        comando = New MySqlCommand(strsql, conexion)
    '        conexion.Open()
    '        fechaactual = comando.ExecuteScalar


    '    End Using

    '    If nitcliente = "N/A" Then
    '        nombrecliente = "CONSUMIDOR FINAL"
    '        nitcliente = "C/F"
    '        direccioncliente = "_____________________________________"
    '    Else
    '        nitcliente = nitcliente.Replace("-", "")
    '    End If


    '    dte.idDispositivo = dispositivo
    '    dte.estadoDocumento = "Activo".ToString
    '    dte.codigoMoneda = "GTQ".ToString
    '    dte.tipoDocumento = "CFACE".ToString
    '    dte.nitComprador = UCase(nitcliente) 'Colocar aquí un nit valido o C/F
    '    dte.regimen2989 = "0"
    '    dte.nitVendedor = "599635K" 'Colocar aquí el nit del vendedor
    '    dte.serieAutorizada = serie
    '    dte.montoTotalOperacion = totalfactura
    '    dte.fechaDocumento = fechaactual
    '    dte.fechaAnulacionSpecified = False
    '    dte.fechaDocumentoSpecified = True
    '    dte.fechaResolucionSpecified = True
    '    dte.tipoCambioSpecified = True
    '    dte.detalleImpuestosIvaSpecified = True
    '    dte.importeOtrosImpuestosSpecified = True
    '    dte.numeroDocumento = idtransaccion
    '    dte.telefonoComprador = "n/a"
    '    dte.importeDescuento = Convert.ToDouble(0.0)
    '    dte.importeDescuentoSpecified = True
    '    dte.importeTotalExento = Convert.ToDouble(0.0)
    '    dte.importeTotalExentoSpecified = True
    '    dte.importeNetoGravado = Convert.ToDouble(totalfactura)
    '    dte.importeNetoGravadoSpecified = True
    '    dte.detalleImpuestosIva = FormatNumber(((totalfactura / 1.12) * 0.12), 2)
    '    dte.tipoCambio = Convert.ToDouble(0)
    '    dte.direccionComercialComprador = direccioncliente
    '    dte.serieDocumento = "1"
    '    dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    dte.numeroResolucion = numeroresolucion 'Colocar aquí el numero de la resolucion
    '    dte.municipioComprador = "N/A"
    '    dte.nombreComercialComprador = nombrecliente
    '    dte.departamentoComprador = "N/A"
    '    dte.nombreComercialRazonSocialVendedor = "Líneas Terrestres Guatemaltecas, S. A."
    '    dte.nombreCompletoVendedor = "Litegua"
    '    dte.municipioVendedor = "N/A"
    '    dte.departamentoVendedor = "N/A"
    '    dte.direccionComercialVendedor = direccionagencia
    '    dte.fechaResolucion = fecharesolucion 'Colocar aquí fecha resolucion"
    '    dte.regimenISR = "SUJETO A PAGOS TRIMESTRALES. ART. 72 LEY ISR"
    '    dte.observaciones = "N/A"
    '    dte.personalizado_01 = "N/A"
    '    dte.personalizado_02 = "N/A"
    '    dte.personalizado_03 = "N/A"
    '    dte.personalizado_04 = "N/A"
    '    dte.personalizado_05 = "N/A"
    '    dte.personalizado_06 = "N/A"
    '    dte.personalizado_07 = "N/A"
    '    dte.personalizado_08 = "N/A"
    '    dte.personalizado_09 = "N/A"
    '    dte.personalizado_10 = "N/A"
    '    dte.personalizado_11 = "N/A"
    '    dte.personalizado_12 = "N/A"
    '    dte.personalizado_13 = "N/A"
    '    dte.personalizado_14 = "N/A"
    '    dte.personalizado_15 = "N/A"
    '    dte.personalizado_16 = "N/A"
    '    dte.personalizado_17 = "N/A"
    '    dte.personalizado_18 = "N/A"
    '    dte.personalizado_19 = "N/A"
    '    dte.personalizado_20 = "N/A"
    '    dte.importeBruto = FormatNumber((totalfactura / 1.12), 2)
    '    dte.importeBrutoSpecified = True
    '    dte.nitGFACE = "12521337"
    '    dte.codigoEstablecimiento = idagencia 'Colocar aui el codigo del establecimiento 
    '    dte.correoComprador = "n/a"
    '    dte.descripcionOtroImpuesto = "n/a"

    '    deta.cantidadSpecified = True
    '    deta.cantidad = 1
    '    deta.codigoProducto = "Enc"
    '    deta.detalleImpuestosIvaSpecified = True
    '    deta.descripcionProducto = "Envio de Encomiendas"
    '    deta.montoBruto = FormatNumber((totalfactura / 1.12), 2)
    '    deta.precioUnitario = Convert.ToDouble(totalfactura)
    '    deta.precioUnitarioSpecified = True
    '    deta.importeExento = Convert.ToDouble(0.0)
    '    deta.importeExentoSpecified = True
    '    deta.importeNetoGravado = Convert.ToDouble(totalfactura)
    '    deta.importeNetoGravadoSpecified = True
    '    deta.importeTotalOperacion = Convert.ToDouble(totalfactura)
    '    deta.montoDescuento = Convert.ToDouble(0.0)
    '    deta.montoBrutoSpecified = True
    '    deta.montoDescuentoSpecified = True
    '    deta.importeTotalOperacionSpecified = True
    '    deta.importeOtrosImpuestosSpecified = True
    '    deta.unidadMedida = "LBS"
    '    deta.detalleImpuestosIva = FormatNumber(((totalfactura / 1.12) * 0.12), 2)
    '    deta.tipoProducto = "S"
    '    deta.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    deta.personalizado_01 = "N/A"
    '    deta.personalizado_02 = "N/A"
    '    deta.personalizado_03 = "N/A"
    '    deta.personalizado_04 = "N/A"
    '    deta.personalizado_05 = "N/A"
    '    deta.personalizado_06 = "N/A"



    '    ReDim dte.detalleDte(0)

    '    dte.detalleDte(0) = deta
    '    registro.dte = dte


    '    registro.usuario = "LITEGUA" 'Colocar aquí el usuario
    '    registro.clave = "ADA46643279EAF21F08B251526FAA8CDEF26D52F0989C8FFE9900A7647880799" 'Colocar aquí la clave 
    '    System.Net.ServicePointManager.Expect100Continue = False

    '    resultado = ws.registrarDte(registro)

    '    If (resultado.valido) Then
    '        face = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '        cae = resultado.cae      ' le envia el CAE al textbox2
    '        numerofactura = Right(resultado.numeroDte, 10)
    '    Else
    '        Dim arrayfacturaelectronica As Array
    '        resultado.descripcion = Replace(resultado.descripcion, "]", "[")
    '        arrayfacturaelectronica = Split(resultado.descripcion, "[")

    '        If arrayfacturaelectronica.Length >= 13 Then

    '            If Left(arrayfacturaelectronica(13), 5) = "CFACE" Then
    '                face = arrayfacturaelectronica(13)
    '                cae = arrayfacturaelectronica(11)
    '                numerofactura = Right(face, 10)
    '                resultado.valido = True
    '            Else
    '                face = ""
    '                cae = ""
    '                numerofactura = ""
    '                detalles = "ERROR:" & resultado.descripcion
    '            End If
    '        Else
    '            face = ""
    '            cae = ""
    '            numerofactura = ""
    '            detalles = "ERROR:" & resultado.descripcion
    '        End If
    '    End If


    '    '*********************************************************************************************


    '    Dim LargoKae As Integer = Len(cae)
    '    If LargoKae Mod 2 = 0 Then
    '        cae1 = Left(cae, LargoKae / 2)
    '        cae2 = Right(cae, LargoKae / 2)
    '    Else
    '        cae1 = Left(cae, Int((LargoKae / 2) + 1))
    '        cae2 = Right(cae, LargoKae / 2)
    '    End If


    '    Return face & "|" & cae & "|" & numerofactura & "|" & cae1 & "|" & cae2 & "|" & detalles & "|" & IIf(resultado.valido = True, "valido", "invalido")

    'End Function





    'Public Shared Function GenerarFacturaElectronica(ByVal dispositivo As String, ByVal nitcliente As String, ByVal serie As String, ByVal totalfactura As String, ByVal idtransaccion As String, ByVal numeroresolucion As String, ByVal nombrecliente As String, ByVal direccioncliente As String, ByVal direccionagencia As String, ByVal fecharesolucion As String, ByVal idagencia As String) As String



    '    Dim dte As New WebReference.dte
    '    Dim deta As New WebReference.detalleDte
    '    Dim registro As New WebReference.requestDte
    '    Dim resultado As New WebReference.responseDte
    '    Dim ws As New WebReference.ingfaceClient
    '    Dim face As String = "", numerofactura As String = "", cae As String = "", cae1 As String = "", cae2 As String = "", detalles As String = ""








    '    Dim strsql As String = ""
    '    Dim comando As MySqlCommand
    '    Dim fechaactual As Date


    '    Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


    '        strsql = "Select curdate();"
    '        comando = New MySqlCommand(strsql, conexion)
    '        conexion.Open()
    '        fechaactual = comando.ExecuteScalar


    '    End Using

    '    If nitcliente = "N/A" Then
    '        nombrecliente = "CONSUMIDOR FINAL"
    '        nitcliente = "C/F"
    '        direccioncliente = "_____________________________________"
    '    Else
    '        nitcliente = nitcliente.Replace("-", "")
    '    End If


    '    dte.idDispositivo = dispositivo
    '    dte.estadoDocumento = "Activo".ToString
    '    dte.codigoMoneda = "GTQ".ToString
    '    dte.tipoDocumento = "CFACE".ToString
    '    dte.nitComprador = UCase(nitcliente) 'Colocar aquí un nit valido o C/F
    '    dte.regimen2989 = "0"
    '    dte.nitVendedor = "599635K" 'Colocar aquí el nit del vendedor
    '    dte.serieAutorizada = serie
    '    dte.montoTotalOperacion = totalfactura
    '    dte.fechaDocumento = fechaactual
    '    dte.fechaAnulacionSpecified = False
    '    dte.fechaDocumentoSpecified = True
    '    dte.fechaResolucionSpecified = True
    '    dte.tipoCambioSpecified = True
    '    dte.detalleImpuestosIvaSpecified = True
    '    dte.importeOtrosImpuestosSpecified = True
    '    dte.numeroDocumento = idtransaccion
    '    dte.telefonoComprador = "n/a"
    '    dte.importeDescuento = Convert.ToDouble(0.0)
    '    dte.importeDescuentoSpecified = True
    '    dte.importeTotalExento = Convert.ToDouble(0.0)
    '    dte.importeTotalExentoSpecified = True
    '    dte.importeNetoGravado = Convert.ToDouble(totalfactura)
    '    dte.importeNetoGravadoSpecified = True
    '    dte.detalleImpuestosIva = FormatNumber(((totalfactura / 1.12) * 0.12), 2)
    '    dte.tipoCambio = Convert.ToDouble(0)
    '    dte.direccionComercialComprador = direccioncliente
    '    dte.serieDocumento = "1"
    '    dte.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    dte.numeroResolucion = numeroresolucion 'Colocar aquí el numero de la resolucion
    '    dte.municipioComprador = "N/A"
    '    dte.nombreComercialComprador = nombrecliente
    '    dte.departamentoComprador = "N/A"
    '    dte.nombreComercialRazonSocialVendedor = "Líneas Terrestres Guatemaltecas, S. A."
    '    dte.nombreCompletoVendedor = "Litegua"
    '    dte.municipioVendedor = "N/A"
    '    dte.departamentoVendedor = "N/A"
    '    dte.direccionComercialVendedor = direccionagencia
    '    dte.fechaResolucion = fecharesolucion 'Colocar aquí fecha resolucion"
    '    dte.regimenISR = "SUJETO A PAGOS TRIMESTRALES. ART. 72 LEY ISR"
    '    dte.observaciones = "N/A"
    '    dte.personalizado_01 = "N/A"
    '    dte.personalizado_02 = "N/A"
    '    dte.personalizado_03 = "N/A"
    '    dte.personalizado_04 = "N/A"
    '    dte.personalizado_05 = "N/A"
    '    dte.personalizado_06 = "N/A"
    '    dte.personalizado_07 = "N/A"
    '    dte.personalizado_08 = "N/A"
    '    dte.personalizado_09 = "N/A"
    '    dte.personalizado_10 = "N/A"
    '    dte.personalizado_11 = "N/A"
    '    dte.personalizado_12 = "N/A"
    '    dte.personalizado_13 = "N/A"
    '    dte.personalizado_14 = "N/A"
    '    dte.personalizado_15 = "N/A"
    '    dte.personalizado_16 = "N/A"
    '    dte.personalizado_17 = "N/A"
    '    dte.personalizado_18 = "N/A"
    '    dte.personalizado_19 = "N/A"
    '    dte.personalizado_20 = "N/A"
    '    dte.importeBruto = FormatNumber((totalfactura / 1.12), 2)
    '    dte.importeBrutoSpecified = True
    '    dte.nitGFACE = "12521337"
    '    dte.codigoEstablecimiento = idagencia 'Colocar aui el codigo del establecimiento 
    '    dte.correoComprador = "n/a"
    '    dte.descripcionOtroImpuesto = "n/a"

    '    deta.cantidadSpecified = True
    '    deta.cantidad = 1
    '    deta.codigoProducto = "Enc"
    '    deta.detalleImpuestosIvaSpecified = True
    '    deta.descripcionProducto = "Envio de Encomiendas"
    '    deta.montoBruto = FormatNumber((totalfactura / 1.12), 2)
    '    deta.precioUnitario = Convert.ToDouble(totalfactura)
    '    deta.precioUnitarioSpecified = True
    '    deta.importeExento = Convert.ToDouble(0.0)
    '    deta.importeExentoSpecified = True
    '    deta.importeNetoGravado = Convert.ToDouble(totalfactura)
    '    deta.importeNetoGravadoSpecified = True
    '    deta.importeTotalOperacion = Convert.ToDouble(totalfactura)
    '    deta.montoDescuento = Convert.ToDouble(0.0)
    '    deta.montoBrutoSpecified = True
    '    deta.montoDescuentoSpecified = True
    '    deta.importeTotalOperacionSpecified = True
    '    deta.importeOtrosImpuestosSpecified = True
    '    deta.unidadMedida = "LBS"
    '    deta.detalleImpuestosIva = FormatNumber(((totalfactura / 1.12) * 0.12), 2)
    '    deta.tipoProducto = "S"
    '    deta.importeOtrosImpuestos = Convert.ToDouble(0.0)
    '    deta.personalizado_01 = "N/A"
    '    deta.personalizado_02 = "N/A"
    '    deta.personalizado_03 = "N/A"
    '    deta.personalizado_04 = "N/A"
    '    deta.personalizado_05 = "N/A"
    '    deta.personalizado_06 = "N/A"



    '    ReDim dte.detalleDte(0)

    '    dte.detalleDte(0) = deta
    '    registro.dte = dte


    '    If serie = "ABCD" Then
    '        registro.usuario = "LPRUEBAS" 'Colocar aquí el usuario
    '        registro.clave = "7F1F2031620CAFBD5FFD7C44055E7A954B2F827E110B7531CFB6A9F80B699A68" 'Colocar aquí la clave 

    '    Else



    '        registro.usuario = "LITEGUA" 'Colocar aquí el usuario
    '        registro.clave = "ADA46643279EAF21F08B251526FAA8CDEF26D52F0989C8FFE9900A7647880799" 'Colocar aquí la clave 

    '    End If



    '    System.Net.ServicePointManager.Expect100Continue = False

    '    resultado = ws.registrarDte(registro)

    '    If (resultado.valido) Then
    '        face = resultado.numeroDte ' le envia el numero de documento electronico al textbox1
    '        cae = resultado.cae      ' le envia el CAE al textbox2
    '        numerofactura = Right(resultado.numeroDte, 10)
    '    Else
    '        Dim arrayfacturaelectronica As Array
    '        resultado.descripcion = Replace(resultado.descripcion, "]", "[")
    '        arrayfacturaelectronica = Split(resultado.descripcion, "[")

    '        If arrayfacturaelectronica.Length >= 13 Then

    '            If Left(arrayfacturaelectronica(13), 5) = "CFACE" Then
    '                face = arrayfacturaelectronica(13)
    '                cae = arrayfacturaelectronica(11)
    '                numerofactura = Right(face, 10)
    '                resultado.valido = True
    '            Else
    '                face = ""
    '                cae = ""
    '                numerofactura = ""
    '                detalles = "ERROR:" & resultado.descripcion
    '            End If
    '        Else
    '            face = ""
    '            cae = ""
    '            numerofactura = ""
    '            detalles = "ERROR:" & resultado.descripcion
    '        End If
    '    End If


    '    '*********************************************************************************************


    '    Dim LargoKae As Integer = Len(cae)
    '    If LargoKae Mod 2 = 0 Then
    '        cae1 = Left(cae, LargoKae / 2)
    '        cae2 = Right(cae, LargoKae / 2)
    '    Else
    '        cae1 = Left(cae, Int((LargoKae / 2) + 1))
    '        cae2 = Right(cae, LargoKae / 2)
    '    End If


    '    Return face & "|" & cae & "|" & numerofactura & "|" & cae1 & "|" & cae2 & "|" & detalles & "|" & IIf(resultado.valido = True, "valido", "invalido")

    'End Function








    '<WebMethod(True)> _
    'Public Function GuardaBoleto(ByVal Factura As List(Of Factura), _
    '                             ByVal Facturadetalle As List(Of FacturaDetalle), _
    '                             ByVal Boleto As List(Of Boleto), _
    '                             ByVal idagenciacaja As Integer, _
    '                             ByVal numerotarjeta As Integer, _
    '                             ByVal idtarjetadetalle As Integer) As String

    '    numerotarjeta = numerotarjeta.ToString

    '    Dim id_empresa As String = Session("id_empsa")


    '    If Session("FacturaElectronica") Then
    '        Dim DatosFacturaElectronica As String = "", DatosRemitente As String = "", DatosDestinatario As String = "", DatosPos As String = "", IdRemitente As String = "", IdDestinatario As String = "", EstatusGface As String = "", VarNomFac = " ", VarNitFac As String = " ", VarDireFac As String = " ", VerificaButacaVendida As String = "", CodigoAgencia As String = "", CodigoAgenciaUsuario As String = ""
    '        Dim Agencia As String = "", DireccionAgencia As String = "", Empresa As String = "", Caja As String = "", Dispositivo As String = "", NumeroFactura As Long = 0
    '        Dim QueryBoleto As String = "", QueryBoletoDetalle As String = "", QueryFacturaDetalle As String = "", QueryFacturaDetalleSap As String = "", StrInsertaFacEnca As String = "", strActualizarCorrelativo = "", NemonicoAgenciaDestino As String = "", NemonicoAgenciaSalida As String = "", NemonicoDestino As String = "", NemonicoSalida As String = "", Correlativo, Cae1, Cae2, Cae, Face, Resultado As String
    '        Dim ArrayFacturaelectronica As Array
    '        Dim Resolucion As String = "", FechaResolucion As String = ""

    '        Dim NitCliente As String = "C/F"
    '        Dim NombreCliente As String = "Consumidor Final"
    '        Dim DireccionCliente As String = "__________________"
    '        Dim CodSap As String = ""
    '        Dim NomSap As String = ""
    '        Dim Recargo As Double
    '        Dim Tarjeta As Double
    '        Dim Cheque As Double
    '        Dim LineaDetalleSap As Integer = 0
    '        Dim CodVehSap As String = ""
    '        Dim DescVehSap As String = ""

    '        Dim NBoleto As Long = 0
    '        Dim SQLInserta As String
    '        Dim ValorBoleto As Double = 0
    '        Dim Seguro As Double = 0
    '        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '            connection.Open()
    '            Dim comando As New MySqlCommand
    '            comando.Connection = connection

    '            Try

    '                If Session("id_usr") = "" Then

    '                    Return "ERROR. SU SESION EXPIRO!"

    '                Else
    '                    Dim ConsultaDatos As String = "call Boletos.sp_retorna_series_ruta(" & idagenciacaja & ",'" & Session("id_usr") & "');"

    '                    NumeroFactura = 0

    '                    comando.CommandText = ConsultaDatos
    '                    Dim Lector As MySqlDataReader = comando.ExecuteReader()

    '                    While Lector.Read()

    '                        Caja = Lector("caja")
    '                        Resolucion = Lector("resolucion")
    '                        FechaResolucion = Lector("fecha_resolucion").ToString
    '                        CodigoAgencia = Lector("codigoagencia")
    '                        CodigoAgenciaUsuario = Lector("codigoagenciausuario")
    '                        Empresa = Lector("empresa")
    '                        Dispositivo = Lector("dispositivo")
    '                        Agencia = Lector("agencia")
    '                        DireccionAgencia = Lector("direccionagencia")
    '                    End While
    '                    Lector.Close()



    '                    If Len(Resolucion) > 0 Then


    '                        For i = 0 To Factura.Count - 1
    '                            '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

    '                            '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------

    '                            Dim Ruta As String = Boleto(i).idruta
    '                            Dim Servicio As String = Boleto(i).idservicio
    '                            Dim Hora As String = Boleto(i).idhora

    '                            Dim queryselectSap As String = "select concat(s.id_sap,h.id_sap,r.id_sap) IdSap, cast(concat(h.hora,'-',s.descripcion,'-',r.nom_ruta)as char) NomSap from Encomiendas.rutas r inner join Boletos.servicio s on s.idservicio =" & Servicio & " inner join Encomiendas.hora h on h.id_horario = " & Hora & " where r.id_ruta = " & Ruta & " and s.idservicio = " & Servicio & " and h.id_horario = " & Hora & ";"

    '                            comando.CommandText = queryselectSap
    '                            Dim resultante As MySqlDataReader = comando.ExecuteReader()

    '                            While resultante.Read()
    '                                CodVehSap = resultante("IdSap").ToString
    '                                DescVehSap = resultante("NomSap").ToString
    '                            End While
    '                            resultante.Close()

    '                            QueryBoleto = " insert into Boletos.boleto (id_boleto,fechacreacion,fechaviaje,origen," +
    '                           " destino,nitcliente,seguro," +
    '                           " tipo_cobro,numerobutaca,estadobutaca,empresa," +
    '                           " agencia,usuario,estadofacturacion," +
    '                           " estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
    '                           " secuenciaorigen,secuenciadestino,idservicio,idtipoboleto,idtarjeta) " +
    '                           " values(0,now(),\'" & Boleto(i).fechaviaje & "\'," & Boleto(i).origen & "," +
    '                           "" & Boleto(i).destino & ",\'" & NitCliente & "\'," +
    '                           "" & Seguro & ",2," +
    '                           "null,\'ocupado\'," & Empresa & "," +
    '                           "" & CodigoAgenciaUsuario & ",\'" & Session("id_usr") & "\',\'FACTURADO\'," +
    '                           " \'VALIDO\',0," & Boleto(i).total & "," +
    '                           " 0,\'\'," & Boleto(i).idhora & "," & Boleto(i).idruta & "," +
    '                           " Encomiendas.fn_retornar_secuencia(" & Boleto(i).origen & "," & Boleto(i).idruta & ",1)," +
    '                           " Encomiendas.fn_retornar_secuencia(" & Boleto(i).destino & "," & Boleto(i).idruta & ",1)," +
    '                           "" & Boleto(i).idservicio & ",2," & numerotarjeta & ")"

    '                            MsgBox(QueryBoleto)


    '                            '********************************** Generacion de Gface Electronica*************************************

    '                            ''-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'

    '                            QueryFacturaDetalle = "insert into Facturacion.factura_detalle " +
    '                            "(num_fac,serie_fac,item,id_doc,descripcion,cantidad,valor,SubTotal,codigo_producto,id_unidad_medida,id_empresa) " +
    '                            "values (new.num_fac,\'" & Trim(Facturadetalle(i).seriefactura) & "\',3,@documento,\'" & Facturadetalle(i).descripcion & "\',1," & Facturadetalle(i).valor & "," & Facturadetalle(i).valor & ",\'" & CodVehSap & "\',\'UNI\','" & id_empresa & "')"

    '                            MsgBox(QueryFacturaDetalle)




    '                            QueryBoletoDetalle = "insert into Boletos.tarjetadetalledocumentos " +
    '                            "(idtarjetadetalle,numerotarjeta,id_boleto,item,valor) " +
    '                            "values(" & idtarjetadetalle & "," & numerotarjeta & ",@documento,3," & Facturadetalle(i).valor & ")"

    '                            MsgBox(QueryBoletoDetalle)


    '                            QueryFacturaDetalleSap = "insert into SapLitegua.INV1 (DocEntry,LineNum,ItemCode,Dscription,Text,Quantity,Price,WhsCode,TaxCode,Procesado,SAPaintermedia)" +
    '                                                   " values (@identitysap," & LineaDetalleSap & ",\'" & CodVehSap & "\',\'" & DescVehSap & "\',\'BOLETO RUTA\',1," & Facturadetalle(i).valor & ",\'Servicio\',\'IVA\',0,0);"


    '                            MsgBox(QueryFacturaDetalleSap)



    '                            If CodSap = "" Then
    '                                CodSap = "PRI00024"
    '                                NomSap = "CLIENTE MOSTRADOR"
    '                            End If


    '                            Dim detener As String = ""




    '                            If Session("idtransaccion") = 0 Then

    '                                StrInsertaFacEnca = "insert into Facturacion.transaccionesgface  (num_fac,serie_fac,fecha_transaccion,hora_transaccion,valor_fac,tipo_cambio,monto_total,nit_clt,nom_clt,dire_clt, id_empresa,id_agc,id_usr,id_agenciacaja,cae,face,recargo,efectivo,tarjetacredito,cheque,nimpresion,documento,documento_detalle,documento_historial,factura_detalle_sap,factura_detalle,item,archivoimpresion,codigo_cliente_sap,nombre_cliente_sap,tipo_cliente_sap) " +
    '                                                                                        "  values (null,'" & Trim(Factura(i).seriefactura) & "',curdate(),curtime()," & Factura(i).valorfactura & ",0.00," & Factura(i).valorfactura & ",'" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "','" & id_empresa & "'," & CodigoAgenciaUsuario & ",'" & Session("id_usr") & "'," & idagenciacaja & ",'" & "N/A" & "','" & "N/A" & "'," & Recargo & ", " & Factura(i).efectivo & "," & Tarjeta & "," & Cheque & ",1,'" & QueryBoleto & "','" & QueryBoletoDetalle & "','','" & QueryFacturaDetalleSap & "','" & QueryFacturaDetalle & "',3,' TARJETA # " & numerotarjeta & "', '" & CodSap & "','" & NomSap & "',1)"

    '                                MsgBox(StrInsertaFacEnca)


    '                                comando.CommandText = StrInsertaFacEnca
    '                                comando.ExecuteScalar()

    '                                comando.CommandText = "SELECT @@IDENTITY"
    '                                Session("idtransaccion") = comando.ExecuteScalar


    '                            End If

    '                            DatosFacturaElectronica = GenerarFacturaElectronica(Dispositivo, NitCliente, Trim(Factura(i).seriefactura), Factura(i).valorfactura, Factura(i).numerofactura, Resolucion, NombreCliente, DireccionCliente, DireccionAgencia, FechaResolucion, CodigoAgencia)
    '                            ArrayFacturaelectronica = Split(DatosFacturaElectronica, "|")

    '                            Face = ArrayFacturaelectronica(0)
    '                            Cae = ArrayFacturaelectronica(1)
    '                            Cae1 = ArrayFacturaelectronica(3)
    '                            Cae2 = ArrayFacturaelectronica(4)
    '                            Correlativo = ArrayFacturaelectronica(2)
    '                            Session("correlativofactura") = Correlativo
    '                            EstatusGface = ArrayFacturaelectronica(5)
    '                            Resultado = ArrayFacturaelectronica(6)

    '                            If Resultado = "valido" Then



    '                                Dim ActualizaTransaccion As String = ""


    '                                ActualizaTransaccion = ActualizaTransaccionGface(Trim(Factura(i).seriefactura), Correlativo, Cae, Face, Session("idtransaccion"))


    '                                If ActualizaTransaccion = "Actualizada" Then

    '                                    ActualizaTransaccion = "CALL Facturacion.sp_guarda_documentos('" & Session("idtransaccion") & "')"
    '                                    comando.CommandText = ActualizaTransaccion
    '                                    Resultado = comando.ExecuteScalar

    '                                    If Resultado = "IMPRIMIR" Then

    '                                        Session("idtransaccion") = 0

    '                                        strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Correlativo & " " +
    '                                                                   " where id_agenciacaja=" & idagenciacaja & ""
    '                                        comando.CommandText = strActualizarCorrelativo
    '                                        comando.ExecuteNonQuery()


    '                                    Else
    '                                        Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " Presione de nuevo Facturar"
    '                                    End If
    '                                Else

    '                                    Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " " & ActualizaTransaccion & " Presione de nuevo Facturar"

    '                                End If

    '                            Else

    '                                comando.CommandText = "update Facturacion.transaccionesgface set error='" & Replace(EstatusGface, "'", "") & "' where idtransaccion= " & Session("idtransaccion")
    '                                comando.ExecuteScalar()

    '                                Resultado = "ERROR:EN LA TRANSACCION " & Session("idtransaccion") & " " & EstatusGface & " Presione de nuevo Facturar"
    '                                Session("idtransaccion") = 0

    '                                Return Resultado

    '                            End If

    '                        Next
    '                        'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
    '                        Lector.Close()


    '                        strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Factura(Factura.Count - 1).numerofactura & " " +
    '                            " where id_agenciacaja=" & idagenciacaja & ""
    '                        comando.CommandText = strActualizarCorrelativo
    '                        comando.ExecuteNonQuery()

    '                        '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
    '                        Return "Datos guardados correctamente"
    '                    Else

    '                        Return "ERROR. No se establecieron los datos del usuario"
    '                    End If
    '                End If
    '            Catch ex As MySqlException


    '                SQLInserta = "ERROR." + ex.Message
    '                Return SQLInserta
    '            Catch ex As Exception

    '                SQLInserta = "ERROR." + ex.Message
    '                Return SQLInserta

    '            Finally
    '                connection.Close()
    '                comando.Dispose()

    '            End Try
    '        End Using

    '        'End If
    '    Else
    '        ' nuevo

    '        'If Session("FacturaElectronica") = 0 Then  ' NO USA FACE

    '        'MsgBox("aqui voy 0")




    '        Dim DatosFacturaElectronica As String = "", DatosRemitente As String = "", DatosDestinatario As String = "", DatosPos As String = "", IdRemitente As String = "", IdDestinatario As String = "", EstatusGface As String = "", VarNomFac = " ", VarNitFac As String = " ", VarDireFac As String = " ", VerificaButacaVendida As String = "", CodigoAgencia As String = "", CodigoAgenciaUsuario As String = ""
    '        Dim Agencia As String = "", DireccionAgencia As String = "", Empresa As String = "", Caja As String = "", Dispositivo As String = "", NumeroFactura As Long = 0
    '        Dim QueryBoleto As String = "", QueryBoletoDetalle As String = "", QueryFacturaDetalle As String = "", QueryFacturaDetalleSap As String = "", StrInsertaFacEnca As String = "", strActualizarCorrelativo = "", NemonicoAgenciaDestino As String = "", NemonicoAgenciaSalida As String = "", NemonicoDestino As String = "", NemonicoSalida As String = "", Correlativo, Cae1, Cae2, Cae, Face, Resultado As String
    '        Dim ArrayFacturaelectronica As Array
    '        Dim Resolucion As String = "", FechaResolucion As String = ""

    '        Dim NitCliente As String = "C/F"
    '        Dim NombreCliente As String = "Consumidor Final"
    '        Dim DireccionCliente As String = "__________________"
    '        Dim CodSap As String = ""
    '        Dim NomSap As String = ""
    '        Dim Recargo As Double
    '        Dim Tarjeta As Double
    '        Dim Cheque As Double
    '        Dim LineaDetalleSap As Integer = 0
    '        Dim CodVehSap As String = ""
    '        Dim DescVehSap As String = ""

    '        Dim NBoleto As Long = 0
    '        Dim SQLInserta As String
    '        Dim ValorBoleto As Double = 0
    '        Dim Seguro As Double = 0
    '        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
    '            connection.Open()
    '            Dim comando As New MySqlCommand
    '            comando.Connection = connection

    '            Try

    '                If Session("id_usr") = "" Then

    '                    Return "ERROR. SU SESION EXPIRO!"

    '                Else

    '                    '     MsgBox(idagenciacaja)


    '                    Dim ConsultaDatos As String = "call Boletos.sp_ME_retorna_series_ruta(" & idagenciacaja & ",'" & Session("id_usr") & "');"

    '                    'MsgBox("aqui voy 1")


    '                    NumeroFactura = 0

    '                    comando.CommandText = ConsultaDatos
    '                    Dim Lector As MySqlDataReader = comando.ExecuteReader()

    '                    While Lector.Read()

    '                        Caja = Lector("caja")
    '                        Resolucion = Lector("resolucion")
    '                        FechaResolucion = Lector("fecha_resolucion").ToString
    '                        CodigoAgencia = Lector("codigoagencia")
    '                        CodigoAgenciaUsuario = Lector("codigoagenciausuario")
    '                        Empresa = Lector("empresa")
    '                        Dispositivo = Lector("dispositivo")
    '                        Agencia = Lector("agencia")
    '                        DireccionAgencia = Lector("direccionagencia")
    '                    End While
    '                    Lector.Close()



    '                    'MsgBox("aqui voy 2")


    '                    If Len(Resolucion) > 0 Then

    '                        'MsgBox(Resolucion)



    '                        For i = 0 To Factura.Count - 1
    '                            '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

    '                            '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------

    '                            Dim Ruta As String = Boleto(i).idruta
    '                            Dim Servicio As String = Boleto(i).idservicio
    '                            Dim Hora As String = Boleto(i).idhora

    '                            Dim queryselectSap As String = "select concat(s.id_sap,h.id_sap,r.id_sap) IdSap, cast(concat(h.hora,'-',s.descripcion,'-',r.nom_ruta)as char) NomSap from Encomiendas.rutas r inner join Boletos.servicio s on s.idservicio =" & Servicio & " inner join Encomiendas.hora h on h.id_horario = " & Hora & " where r.id_ruta = " & Ruta & " and s.idservicio = " & Servicio & " and h.id_horario = " & Hora & ";"

    '                            comando.CommandText = queryselectSap
    '                            Dim resultante As MySqlDataReader = comando.ExecuteReader()

    '                            While resultante.Read()
    '                                CodVehSap = resultante("IdSap").ToString
    '                                DescVehSap = resultante("NomSap").ToString
    '                            End While
    '                            resultante.Close()

    '                            'MsgBox("aqui voy 3")


    '                            QueryBoleto = " insert into Boletos.boleto (id_boleto,fechacreacion,fechaviaje,origen," +
    '                           " destino,nitcliente,seguro," +
    '                           " tipo_cobro,numerobutaca,estadobutaca,empresa," +
    '                           " agencia,usuario,estadofacturacion," +
    '                           " estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
    '                           " secuenciaorigen,secuenciadestino,idservicio,idtipoboleto,idtarjeta) " +
    '                           " values(0,now(),\'" & Boleto(i).fechaviaje & "\'," & Boleto(i).origen & "," +
    '                           "" & Boleto(i).destino & ",\'" & NitCliente & "\'," +
    '                           "" & Seguro & ",2," +
    '                           "null,\'ocupado\'," & Empresa & "," +
    '                           "" & CodigoAgenciaUsuario & ",\'" & Session("id_usr") & "\',\'FACTURADO\'," +
    '                           " \'VALIDO\',0," & Boleto(i).total & "," +
    '                           " 0,\'\'," & Boleto(i).idhora & "," & Boleto(i).idruta & "," +
    '                           " Encomiendas.fn_retornar_secuencia(" & Boleto(i).origen & "," & Boleto(i).idruta & ",1)," +
    '                           " Encomiendas.fn_retornar_secuencia(" & Boleto(i).destino & "," & Boleto(i).idruta & ",1)," +
    '                           "" & Boleto(i).idservicio & ",2," & numerotarjeta & ")"


    '                            '********************************** Generacion de Gface Electronica*************************************

    '                            ''-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'
    '                            QueryFacturaDetalle = "insert into Facturacion.factura_detalle " +
    '                            "(num_fac,serie_fac,item,id_doc,descripcion,cantidad,valor,SubTotal,codigo_producto,id_unidad_medida,id_empresa) " +
    '                             "values (new.num_fac,\'" & Trim(Facturadetalle(i).seriefactura) & "\',3,@documento,\'" & Facturadetalle(i).descripcion & "\',1," & Facturadetalle(i).valor & "," & Facturadetalle(i).valor & ",\'" & CodVehSap & "\',\'UNI\','" & id_empresa & "')"





    '                            QueryBoletoDetalle = "insert into Boletos.tarjetadetalledocumentos " +
    '                            "(idtarjetadetalle,numerotarjeta,id_boleto,item,valor) " +
    '                            "values(" & idtarjetadetalle & "," & numerotarjeta & ",@documento,3," & Facturadetalle(i).valor & ")"



    '                            QueryFacturaDetalleSap = "insert into SapLitegua.INV1 (DocEntry,LineNum,ItemCode,Dscription,Text,Quantity,Price,WhsCode,TaxCode,Procesado,SAPaintermedia)" +
    '                                                   " values (@identitysap," & LineaDetalleSap & ",\'" & CodVehSap & "\',\'" & DescVehSap & "\',\'BOLETO RUTA\',1," & Facturadetalle(i).valor & ",\'Servicio\',\'IVA\',0,0);"



    '                            If CodSap = "" Then
    '                                CodSap = "PRI00024"
    '                                NomSap = "CLIENTE MOSTRADOR"
    '                            End If



    '                            If Session("idtransaccion") = 0 Then

    '                                StrInsertaFacEnca = "insert into Facturacion.transaccionesgface  (num_fac,serie_fac,fecha_transaccion,hora_transaccion,valor_fac,tipo_cambio,monto_total,nit_clt,nom_clt,dire_clt, id_empresa,id_agc,id_usr,id_agenciacaja,cae,face,recargo,efectivo,tarjetacredito,cheque,nimpresion,documento,documento_detalle,documento_historial,factura_detalle_sap,factura_detalle,item,archivoimpresion,codigo_cliente_sap,nombre_cliente_sap,tipo_cliente_sap) " +
    '                                                                                         "values (null,'" & Trim(Factura(i).seriefactura) & "',curdate(),curtime()," & Factura(i).valorfactura & ",0.00," & Factura(i).valorfactura & ",'" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "','" & id_empresa & "'," & CodigoAgenciaUsuario & ",'" & Session("id_usr") & "'," & idagenciacaja & ",'" & "N/A" & "','" & "N/A" & "'," & Recargo & ", " & Factura(i).efectivo & "," & Tarjeta & "," & Cheque & ",1,'" & QueryBoleto & "','" & QueryBoletoDetalle & "','','" & QueryFacturaDetalleSap & "','" & QueryFacturaDetalle & "',3,' TARJETA # " & numerotarjeta & "', '" & CodSap & "','" & NomSap & "',1)"

    '                                comando.CommandText = StrInsertaFacEnca
    '                                comando.ExecuteScalar()

    '                                comando.CommandText = "SELECT @@IDENTITY"
    '                                Session("idtransaccion") = comando.ExecuteScalar


    '                            End If

    '                            DatosFacturaElectronica = SinFaceGenerarFacturaElectronica(Dispositivo, NitCliente, Trim(Factura(i).seriefactura), Factura(i).valorfactura, Factura(i).numerofactura, Resolucion, NombreCliente, DireccionCliente, DireccionAgencia, FechaResolucion, CodigoAgencia, idagenciacaja)

    '                            ArrayFacturaelectronica = Split(DatosFacturaElectronica, "|")

    '                            Dim alto As String = "alto"







    '                            Face = ArrayFacturaelectronica(0)
    '                            Cae = ArrayFacturaelectronica(1)
    '                            Cae1 = ArrayFacturaelectronica(3)
    '                            Cae2 = ArrayFacturaelectronica(4)
    '                            Correlativo = ArrayFacturaelectronica(2)
    '                            Session("correlativofactura") = Correlativo
    '                            EstatusGface = ArrayFacturaelectronica(5)
    '                            Resultado = ArrayFacturaelectronica(6)

    '                            'Dim _Resultado As String = "valido"


    '                            'MsgBox(Resultado)


    '                            If Resultado = "valido" Then



    '                                Dim ActualizaTransaccion As String = ""




    '                                ActualizaTransaccion = SinFaceActualizaTransaccionGface(Trim(Factura(i).seriefactura), Correlativo, Cae, Face, Session("idtransaccion"))
    '                                'MsgBox("aqui voy 6")



    '                                If ActualizaTransaccion = "Actualizada" Then

    '                                    ActualizaTransaccion = "CALL Facturacion.sp_guarda_documentos('" & Session("idtransaccion") & "')"
    '                                    comando.CommandText = ActualizaTransaccion
    '                                    Resultado = comando.ExecuteScalar

    '                                    If Resultado = "IMPRIMIR" Then

    '                                        Session("idtransaccion") = 0

    '                                        strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Correlativo & " " +
    '                                                                   " where id_agenciacaja=" & idagenciacaja & ""
    '                                        comando.CommandText = strActualizarCorrelativo
    '                                        comando.ExecuteNonQuery()



    '                                    Else
    '                                        Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " Presione de nuevo Facturar"
    '                                    End If
    '                                Else

    '                                    Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " " & ActualizaTransaccion & " Presione de nuevo Facturar"

    '                                End If

    '                            Else

    '                                comando.CommandText = "update Facturacion.transaccionesgface set error='" & Replace(EstatusGface, "'", "") & "' where idtransaccion= " & Session("idtransaccion")
    '                                comando.ExecuteScalar()

    '                                Resultado = "ERROR:EN LA TRANSACCION " & Session("idtransaccion") & " " & EstatusGface & " Presione de nuevo Facturar"
    '                                Session("idtransaccion") = 0

    '                                Return Resultado

    '                            End If

    '                        Next
    '                        'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
    '                        Lector.Close()


    '                        strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Factura(Factura.Count - 1).numerofactura & " " +
    '                            " where id_agenciacaja=" & idagenciacaja & ""
    '                        comando.CommandText = strActualizarCorrelativo
    '                        comando.ExecuteNonQuery()

    '                        '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
    '                        Return "Datos guardados correctamente"
    '                    Else

    '                        Return "ERROR. No se establecieron los datos del usuario"
    '                    End If
    '                End If
    '            Catch ex As MySqlException


    '                SQLInserta = "ERROR." + ex.Message
    '                Return SQLInserta
    '            Catch ex As Exception

    '                SQLInserta = "ERROR." + ex.Message
    '                Return SQLInserta

    '            Finally
    '                connection.Close()
    '                comando.Dispose()

    '            End Try
    '        End Using

    '    End If

    'End Function




    <WebMethod(True)> _
    Public Function GuardaBoleto(ByVal Factura As List(Of Factura), ByVal Facturadetalle As List(Of FacturaDetalle), _
                                 ByVal Boleto As List(Of Boleto), ByVal idagenciacaja As Integer, _
                                 ByVal numerotarjeta As Integer, ByVal idtarjetadetalle As Integer) As String

        numerotarjeta = numerotarjeta.ToString
        Dim id_empresa As String = Session("id_empsa")




        If Session("FacturaElectronica") Then
        Else
            ' nuevo
            Dim DatosFacturaElectronica As String = "", DatosRemitente As String = "", DatosDestinatario As String = "", DatosPos As String = "", IdRemitente As String = "", IdDestinatario As String = "", EstatusGface As String = "", VarNomFac = " ", VarNitFac As String = " ", VarDireFac As String = " ", VerificaButacaVendida As String = "", CodigoAgencia As String = "", CodigoAgenciaUsuario As String = ""
            Dim Agencia As String = "", DireccionAgencia As String = "", Empresa As String = "", Caja As String = "", Dispositivo As String = "", NumeroFactura As Long = 0
            Dim QueryBoleto As String = "", QueryBoletoDetalle As String = "", QueryFacturaDetalle As String = "", QueryFacturaDetalleSap As String = "", StrInsertaFacEnca As String = "", strActualizarCorrelativo = "", NemonicoAgenciaDestino As String = "", NemonicoAgenciaSalida As String = "", NemonicoDestino As String = "", NemonicoSalida As String = "", Correlativo, Cae1, Cae2, Cae, Face, Resultado As String
            Dim ArrayFacturaelectronica As Array
            Dim Resolucion As String = "", FechaResolucion As String = ""

            Dim NitCliente As String = "C/F"
            Dim NombreCliente As String = "Consumidor Final"
            Dim DireccionCliente As String = "__________________"
            Dim CodSap As String = ""
            Dim NomSap As String = ""
            Dim Recargo As Double
            Dim Tarjeta As Double
            Dim Cheque As Double
            Dim LineaDetalleSap As Integer = 0
            Dim CodVehSap As String = ""
            Dim DescVehSap As String = ""

            Dim NBoleto As Long = 0
            Dim SQLInserta As String
            Dim ValorBoleto As Double = 0
            Dim Seguro As Double = 0
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                connection.Open()
                Dim comando As New MySqlCommand
                comando.Connection = connection

                Try

                    If Session("id_usr") = "" Then

                        Return "ERROR. SU SESION EXPIRO!"

                    Else

                        '     MsgBox(idagenciacaja)


                        Dim ConsultaDatos As String = "call Boletos.sp_ME_retorna_series_ruta(" & idagenciacaja & ",'" & Session("id_usr") & "');"

                        'MsgBox("aqui voy 1")


                        NumeroFactura = 0

                        comando.CommandText = ConsultaDatos
                        Dim Lector As MySqlDataReader = comando.ExecuteReader()

                        While Lector.Read()

                            Caja = Lector("caja")
                            Resolucion = Lector("resolucion")
                            FechaResolucion = Lector("fecha_resolucion").ToString
                            CodigoAgencia = Lector("codigoagencia")
                            CodigoAgenciaUsuario = Lector("codigoagenciausuario")
                            Empresa = Lector("empresa")
                            Dispositivo = Lector("dispositivo")
                            Agencia = Lector("agencia")
                            DireccionAgencia = Lector("direccionagencia")
                        End While
                        Lector.Close()



                        'MsgBox("aqui voy 2")


                        If Len(Resolucion) > 0 Then

                            'MsgBox(Resolucion)



                            For i = 0 To Factura.Count - 1
                                '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

                                '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------

                                Dim Ruta As String = Boleto(i).idruta
                                Dim Servicio As String = Boleto(i).idservicio
                                Dim Hora As String = Boleto(i).idhora
                                Dim fechaviaje As String = ConvierteFechaToMysql(Boleto(i).fechaviaje)


                                Dim queryselectSap As String = "select concat(s.id_sap,h.id_sap,r.id_sap) IdSap, cast(concat(h.hora,'-',s.descripcion,'-',r.nom_ruta)as char) NomSap from Encomiendas.rutas r inner join Boletos.servicio s on s.idservicio =" & Servicio & " inner join Encomiendas.hora h on h.id_horario = " & Hora & " where r.id_ruta = " & Ruta & " and s.idservicio = " & Servicio & " and h.id_horario = " & Hora & ";"

                                comando.CommandText = queryselectSap
                                Dim resultante As MySqlDataReader = comando.ExecuteReader()

                                While resultante.Read()
                                    CodVehSap = resultante("IdSap").ToString
                                    DescVehSap = resultante("NomSap").ToString
                                End While
                                resultante.Close()

                                'MsgBox("aqui voy 3")


                                QueryBoleto = " insert into Boletos.boleto (id_boleto,fechacreacion,fechaviaje,origen," +
                               " destino,nitcliente,seguro," +
                               " tipo_cobro,numerobutaca,estadobutaca,empresa," +
                               " agencia,usuario,estadofacturacion," +
                               " estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
                               " secuenciaorigen,secuenciadestino,idservicio,idtipoboleto,idtarjeta) " +
                               " values(0,now(),\'" & fechaviaje & "\'," & Boleto(i).origen & "," +
                               "" & Boleto(i).destino & ",\'" & NitCliente & "\'," +
                               "" & Seguro & ",2," +
                               "null,\'ocupado\'," & Empresa & "," +
                               "" & CodigoAgenciaUsuario & ",\'" & Session("id_usr") & "\',\'FACTURADO\'," +
                               " \'VALIDO\',0," & Boleto(i).total & "," +
                               " 0,\'\'," & Boleto(i).idhora & "," & Boleto(i).idruta & "," +
                               " Encomiendas.fn_retornar_secuencia(1,1,1)," +
                               " Encomiendas.fn_retornar_secuencia(1,1,1)," +
                               "" & Boleto(i).idservicio & ",2," & numerotarjeta & ")"


                                '   " Encomiendas.fn_retornar_secuencia(" & Boleto(i).origen & "," & Boleto(i).idruta & ",1)," +
                                '" Encomiendas.fn_retornar_secuencia(" & Boleto(i).destino & "," & Boleto(i).idruta & ",1)," +
                                '"" & Boleto(i).idservicio & ",2," & numerotarjeta & ")"








                                '********************************** Generacion de Gface Electronica*************************************

                                ''-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'
                                'QueryFacturaDetalle = "insert into Facturacion.factura_detalle " +
                                '"(num_fac,serie_fac,item,id_doc,descripcion,cantidad,valor,SubTotal,codigo_producto,id_unidad_medida,id_empresa) " +
                                ' "values (new.num_fac,\'" & Trim(Facturadetalle(i).seriefactura) & "\',3,@documento,\'" & Facturadetalle(i).descripcion & "\',1," & Facturadetalle(i).valor & "," & Facturadetalle(i).valor & ",\'" & CodVehSap & "\',\'UNI\',1)"

                                QueryFacturaDetalle = "insert into Facturacion.factura_detalle " +
                             "(num_fac,serie_fac,item,id_doc,descripcion,cantidad,valor,SubTotal,codigo_producto,id_unidad_medida,id_empresa) " +
                             "values (new.num_fac,\'" & Trim(Facturadetalle(i).seriefactura) & "\',3,@documento,\'" & Facturadetalle(i).descripcion & "\',1," & Facturadetalle(i).valor & "," & Facturadetalle(i).valor & ",\'" & CodVehSap & "\',\'UNI\',\'" & id_empresa & "\')"




                                QueryBoletoDetalle = "insert into Boletos.tarjetadetalledocumentos " +
                                "(idtarjetadetalle,numerotarjeta,id_boleto,item,valor) " +
                                "values(" & idtarjetadetalle & "," & numerotarjeta & ",@documento,3," & Facturadetalle(i).valor & ")"



                                QueryFacturaDetalleSap = "insert into SapLitegua.INV1 (DocEntry,LineNum,ItemCode,Dscription,Text,Quantity,Price,WhsCode,TaxCode,Procesado,SAPaintermedia)" +
                                                       " values (@identitysap," & LineaDetalleSap & ",\'" & CodVehSap & "\',\'" & DescVehSap & "\',\'BOLETO RUTA\',1," & Facturadetalle(i).valor & ",\'Servicio\',\'IVA\',0,0);"



                                If CodSap = "" Then
                                    CodSap = "PRI00024"
                                    NomSap = "CLIENTE MOSTRADOR"
                                End If



                                If Session("idtransaccion") = 0 Then


                                    StrInsertaFacEnca = "insert into Facturacion.transaccionesgface  (num_fac,serie_fac,fecha_transaccion,hora_transaccion,valor_fac,tipo_cambio,monto_total,nit_clt,nom_clt,dire_clt, id_empresa,id_agc,id_usr,id_agenciacaja,cae,face,recargo,efectivo,tarjetacredito,cheque,nimpresion,documento,documento_detalle,documento_historial,factura_detalle_sap,factura_detalle,item,archivoimpresion,codigo_cliente_sap,nombre_cliente_sap,tipo_cliente_sap) " & _
                                                        "  values (null,'" & Trim(Factura(i).seriefactura) & "','" & fechaviaje & "',curtime()," & Factura(i).valorfactura & ",0.00," & Factura(i).valorfactura & ",'" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "','" & id_empresa & "'," & CodigoAgenciaUsuario & ",'" & Session("id_usr") & "'," & idagenciacaja & ",'" & "N/A" & "','" & "N/A" & "'," & Recargo & ", " & Factura(i).efectivo & "," & Tarjeta & "," & Cheque & ",1,'" & QueryBoleto & "','" & QueryBoletoDetalle & "','','" & QueryFacturaDetalleSap & "','" & QueryFacturaDetalle & "',3,' TARJETA # " & numerotarjeta & "', '" & CodSap & "','" & NomSap & "',1)"


                                    'StrInsertaFacEnca = "insert into Facturacion.transaccionesgface  (num_fac,serie_fac,fecha_transaccion,hora_transaccion,valor_fac,tipo_cambio,monto_total,nit_clt,nom_clt,dire_clt, id_empresa,id_agc,id_usr,id_agenciacaja,cae,face,recargo,efectivo,tarjetacredito,cheque,nimpresion,documento,documento_detalle,documento_historial,factura_detalle_sap,factura_detalle,item,archivoimpresion,codigo_cliente_sap,nombre_cliente_sap,tipo_cliente_sap) " & _
                                    '                    "  values (null,'" & Trim(Factura(i).seriefactura) & "','" & fechaviaje & "',curtime()," & Factura(i).valorfactura & ",0.00," & Factura(i).valorfactura & ",'" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "',1," & CodigoAgenciaUsuario & ",'" & Session("id_usr") & "'," & idagenciacaja & ",'" & "N/A" & "','" & "N/A" & "'," & Recargo & ", " & Factura(i).efectivo & "," & Tarjeta & "," & Cheque & ",1,'" & QueryBoleto & "','" & QueryBoletoDetalle & "','','" & QueryFacturaDetalleSap & "','" & QueryFacturaDetalle & "',3,' TARJETA # " & numerotarjeta & "', '" & CodSap & "','" & NomSap & "',1)"







                                    comando.CommandText = StrInsertaFacEnca
                                    comando.ExecuteScalar()

                                    comando.CommandText = "SELECT @@IDENTITY"
                                    Session("idtransaccion") = comando.ExecuteScalar


                                End If

                                DatosFacturaElectronica = SinFaceGenerarFacturaElectronica(Dispositivo, NitCliente, Trim(Factura(i).seriefactura), Factura(i).valorfactura, Factura(i).numerofactura, Resolucion, NombreCliente, DireccionCliente, DireccionAgencia, FechaResolucion, CodigoAgencia, idagenciacaja)

                                ArrayFacturaelectronica = Split(DatosFacturaElectronica, "|")

                                Dim alto As String = "alto"







                                Face = ArrayFacturaelectronica(0)
                                Cae = ArrayFacturaelectronica(1)
                                Cae1 = ArrayFacturaelectronica(3)
                                Cae2 = ArrayFacturaelectronica(4)
                                Correlativo = ArrayFacturaelectronica(2)
                                Session("correlativofactura") = Correlativo
                                EstatusGface = ArrayFacturaelectronica(5)
                                Resultado = ArrayFacturaelectronica(6)

                                'Dim _Resultado As String = "valido"


                                'MsgBox(Resultado)


                                If Resultado = "valido" Then



                                    Dim ActualizaTransaccion As String = ""




                                    ActualizaTransaccion = SinFaceActualizaTransaccionGface(Trim(Factura(i).seriefactura), Correlativo, Cae, Face, Session("idtransaccion"))
                                    'MsgBox("aqui voy 6")



                                    If ActualizaTransaccion = "Actualizada" Then

                                        ActualizaTransaccion = "CALL Facturacion.sp_guarda_documentos('" & Session("idtransaccion") & "')"
                                        comando.CommandText = ActualizaTransaccion
                                        Resultado = comando.ExecuteScalar

                                        If Resultado = "IMPRIMIR" Then

                                            Session("idtransaccion") = 0

                                            strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Correlativo & " " +
                                                                       " where id_agenciacaja=" & idagenciacaja & ""
                                            comando.CommandText = strActualizarCorrelativo
                                            comando.ExecuteNonQuery()

                                            UpdateFechCreacionEnFactura(Trim(Facturadetalle(i).seriefactura), Correlativo, idagenciacaja, id_empresa)

                                            tarjetadetalledocumentos_depurador(numerotarjeta)




                                        Else
                                            Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " Presione de nuevo Facturar"
                                        End If
                                    Else

                                        Return "ERROR: Sucedio un ERROR al procesar la transaccion " & Session("idtransaccion") & " " & ActualizaTransaccion & " Presione de nuevo Facturar"

                                    End If

                                Else

                                    comando.CommandText = "update Facturacion.transaccionesgface set error='" & Replace(EstatusGface, "'", "") & "' where idtransaccion= " & Session("idtransaccion")
                                    comando.ExecuteScalar()

                                    Resultado = "ERROR:EN LA TRANSACCION " & Session("idtransaccion") & " " & EstatusGface & " Presione de nuevo Facturar"
                                    Session("idtransaccion") = 0

                                    Return Resultado

                                End If

                            Next
                            'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
                            Lector.Close()


                            strActualizarCorrelativo = "update Facturacion.agenciacaja set correlativo=" & Factura(Factura.Count - 1).numerofactura & " " +
                                " where id_agenciacaja=" & idagenciacaja & ""
                            comando.CommandText = strActualizarCorrelativo
                            comando.ExecuteNonQuery()

                            '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
                            Return "Datos guardados correctamente"
                        Else

                            Return "ERROR. No se establecieron los datos del usuario"
                        End If
                    End If
                Catch ex As MySqlException


                    SQLInserta = "ERROR." + ex.Message
                    Return SQLInserta
                Catch ex As Exception

                    SQLInserta = "ERROR." + ex.Message
                    Return SQLInserta

                Finally
                    connection.Close()
                    comando.Dispose()

                End Try
            End Using

        End If

    End Function

    Public Function ConvierteFechaToMysql(ByVal Fecha As String) As String
        Dim fechaStr As Array
        fechaStr = Split(Fecha, "/")
        Dim fechaFinal As String
        fechaFinal = fechaStr(2) & "-" & fechaStr(1) & "-" & fechaStr(0)
        Return fechaFinal

    End Function



    Public Shared Function UpdateFechCreacionEnFactura(ByVal serie_fac As String, ByVal num_fac As String, ByVal id_agenciacaja As String, id_empresa As String) As String
        Dim PosAlmacenado As String = "", PosActualizado As String = ""
        Dim strsql As String = ""
        Dim comando As MySqlCommand
        Dim estado As String

        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


            strsql = "UPDATE Facturacion.factura " +
                " SET fecha_creacion = curdate(), " +
                " hora_creacion = curtime() " +
                " WHERE num_fac ='" & num_fac & "' AND serie_fac = '" & serie_fac & "' AND id_agenciacaja = '" & id_agenciacaja & "' AND id_empresa = '" & id_empresa & "'; "

            comando = New MySqlCommand(strsql, conexion)

            Try
                conexion.Open()

                estado = comando.ExecuteNonQuery

                If estado >= 1 Then
                    Return "Actualizada"
                Else
                    Return "Error"
                End If

                'Catch ex As MySqlException

                '    strsql = "update Facturacion.transaccionesgface set error='" & Replace(ex.Message.ToString, "'", "") & "' where idtransaccion='" & IdTransaccion & "';"
                '    comando = New MySqlCommand(strsql, conexion)
                '    estado = comando.ExecuteNonQuery

                '    Return ex.Message.ToString

            Finally
                conexion.Close()
            End Try
        End Using
    End Function

    Public Shared Function tarjetadetalledocumentos_depurador(ByVal numerotarjeta As String) As String
        Dim PosAlmacenado As String = "", PosActualizado As String = ""
        Dim strsql As String = ""
        Dim comando As MySqlCommand
        Dim estado As String

        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)


            strsql = "CALL Boletos.sp_tarjetadetalledocumentos_depurador('" & numerotarjeta & "'); "

            comando = New MySqlCommand(strsql, conexion)

            Try
                conexion.Open()

                estado = comando.ExecuteNonQuery

                If estado >= 1 Then
                    Return "Actualizada"
                Else
                    Return "Error"
                End If

                'Catch ex As MySqlException

                '    strsql = "update Facturacion.transaccionesgface set error='" & Replace(ex.Message.ToString, "'", "") & "' where idtransaccion='" & IdTransaccion & "';"
                '    comando = New MySqlCommand(strsql, conexion)
                '    estado = comando.ExecuteNonQuery

                '    Return ex.Message.ToString

            Finally
                conexion.Close()
            End Try
        End Using
    End Function

    <WebMethod()> _
    Public Function cerrarTarjeta(ByVal usuario As String, ByVal numerotarjeta As Integer, ByVal idtarjetaestado As Integer) As List(Of Serie)
        Dim Elemento As Serie
        Dim ArregloSerie As List(Of Serie) = New List(Of Serie)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                Dim Consulta As String = "select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'"

                comando.CommandText = Consulta
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()

                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    Dim TablaDetalle As New DataTable
                    Dim TablaDatosEmpresa As New DataTable

                    Dim SqlConsultaDatosUsuario As String = "call Boletos.sp_vales_ruta(" & numerotarjeta & ");"

                    comando = New MySqlCommand(SqlConsultaDatosUsuario, connection)
                    Dim readerUsuario As MySqlDataReader = comando.ExecuteReader()
                    TablaDatosEmpresa.Load(readerUsuario)


                    If (TablaDatosEmpresa.Rows.Count > 0) Then





                        If TablaDatosEmpresa.Rows(0).Item("TotalEfectivo") > 0 Then

                            Elemento = New Serie
                            Elemento.correcto = False
                            Elemento.mensaje = "ERROR: Esta Tarjeta Tiene Pendientes Vales Para Emitir, Generarlos para cerrar la tarjeta... "
                            ArregloSerie.Add(Elemento)

                        Else
                            Consulta = "update Boletos.tarjeta set idtarjetaestado=" & idtarjetaestado & " ,fecha_cierre=curdate(),hora_cierre=curtime(),usuario_cierre='" & usuario & "' where numerotarjeta=" & numerotarjeta & ""
                            comando.CommandText = Consulta
                            comando.ExecuteNonQuery()

                            Elemento = New Serie
                            Elemento.correcto = True
                            Elemento.mensaje = "Tarjeta cerrada correctamente"
                            ArregloSerie.Add(Elemento)

                        End If
                    Else
                        Consulta = "update Boletos.tarjeta set idtarjetaestado=" & idtarjetaestado & " ,fecha_cierre=curdate(),hora_cierre=curtime(),usuario_cierre='" & usuario & "' where numerotarjeta=" & numerotarjeta & ""
                        comando.CommandText = Consulta
                        comando.ExecuteNonQuery()

                        Elemento = New Serie
                        Elemento.correcto = True
                        Elemento.mensaje = "Tarjeta cerrada correctamente"
                        ArregloSerie.Add(Elemento)


                    End If

                    transaccion.Commit()


                Else
                    LectorUsuario.Close()
                    Elemento = New Serie
                    Elemento.correcto = False
                    Elemento.mensaje = "ERROR: No se establecieron los datos del usuario"
                    ArregloSerie.Add(Elemento)
                End If

            Catch ex As Exception
                Elemento = New Serie
                Elemento.correcto = False
                Elemento.mensaje = "ERROR: " + ex.Message
                ArregloSerie.Add(Elemento)
                transaccion.Rollback()
                transaccion.Dispose()
            End Try
        End Using
        Return ArregloSerie
    End Function


End Class



