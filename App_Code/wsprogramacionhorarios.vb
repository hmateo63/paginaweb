Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
Imports System.Data
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsprogramacionhorarios
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function asignarTarjeta(ByVal usuario As String, ByVal fecha As String, ByVal idhorario As Integer) As String

        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()

                    comando = New MySqlCommand("select idtarjeta,random from Boletos.tarjeta where id_detaHorario=" & idhorario & "", connection)
                    Dim LectorTarjeta As MySqlDataReader = comando.ExecuteReader

                    Dim Correlativo As Integer

                    LectorTarjeta.Read()
                    Dim Random As Integer = 0
                    If LectorTarjeta.HasRows Then

                        Correlativo = LectorTarjeta("idtarjeta")
                        Random = LectorTarjeta("random")
                        LectorTarjeta.Close()
                        Return crearManifiesto(Correlativo, idhorario, Random)

                    Else
                        Random = NumeroAleatorioPdf()
                        LectorTarjeta.Close()
                        Dim StrInserta As String = "insert into Boletos.tarjeta (fechacreacion,id_detaHorario,id_usr,random)" +
                        "values (curdate()," & idhorario & ",'" & usuario & "'," & Random & ");"

                        comando.CommandText = StrInserta
                        comando.ExecuteNonQuery()

                        comando.CommandText = "SELECT @@IDENTITY"
                        Correlativo = comando.ExecuteScalar
                        transaccion.Commit()

                        Return crearManifiesto(Correlativo, idhorario, Random)

                    End If
                Else
                    LectorUsuario.Close()
                    Return "ERROR: No se establecieron los datos del usuario"

                End If
            Catch ex As Exception
                transaccion.Rollback()
                Return "ERROR: " + ex.Message

            End Try
        End Using


    End Function

    <WebMethod()> _
    Public Function crearManifiesto(ByVal idtarjeta As Integer, ByVal idhorario As Integer, ByVal random As Integer) As String

        Dim Bus As String = ""
        Dim Hora As String = ""
        Dim Fecha As String = ""
        Dim TotalBoletos As String = 0
        Dim CantidadBoletos As Integer = 0

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim command As New MySqlCommand
            Try
                Dim SQLConsulta As String = "select monto_boletos,cantidad_boletos,hora,DATE_FORMAT(fecha,'%d/%m/%Y') fecha," +
                    "(select id_veh from horarios where id_detaHorario=" & idhorario & ") idvehiculo " +
                    "from vw_monto where " +
                    "id_horario = (select id_horario from horarios where id_detaHorario=" & idhorario & ") and " +
                    "fecha=(select fecha_detaHorario from horarios where id_detaHorario=" & idhorario & ") and " +
                    "id_ruta=(select id_ruta from horarios where id_detaHorario=" & idhorario & ");"

                command = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = command.ExecuteReader()

                reader.Read()
                If reader.HasRows Then
                    Bus = reader("idvehiculo")
                    Hora = reader("hora").ToString
                    Fecha = reader("fecha").ToString
                    TotalBoletos = reader("monto_boletos")
                    CantidadBoletos = reader("cantidad_boletos")
                    reader.Close()
                Else
                    reader.Close()
                    Bus = "N/A"
                    Hora = "N/A"
                    Fecha = "N/A"
                    TotalBoletos = 0
                    CantidadBoletos = 0
                End If

            Catch ex As Exception
                Return "ERROR: " + ex.Message
            End Try
        End Using

        Try
            Dim doc As Document = New iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER, 10, 10, 10, 10)
            doc.SetPageSize(PageSize.A4.Rotate())
            Dim Correlativo As Integer = NumeroAleatorio()
            Dim pd As PdfWriter = PdfWriter.GetInstance(doc, New FileStream(Server.MapPath("~\privado\pdf\manifiesto" + Correlativo.ToString + ".pdf"), FileMode.Create))
            doc.AddTitle("Manifiesto")
            doc.AddAuthor("RUTAS ORIENTALES")
            doc.AddCreationDate()

            doc.Open()

            Dim Tabla As PdfPTable = New PdfPTable(8)

            Tabla.TotalWidth = 750
            Tabla.LockedWidth = True
            Tabla.SetWidths({5, 5, 15, 15, 15, 15, 15, 15})


            'DECLARACION DE LA CELDA A UTILIZAR EN LA TABLAPDF
            Dim Celda As PdfPCell = New PdfPCell

            '=================CERO FILA======================'
            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Padding = 10
            Celda.Colspan = 6

            Dim pathImagen As String = Server.MapPath("~\images\rutas.jpg")
            Dim jpg As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(pathImagen)
            jpg.Alignment = iTextSharp.text.Image.LEFT_ALIGN
            jpg.WidthPercentage = 15
            Celda.AddElement(jpg)
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("No. " + PadL(random.ToString + idtarjeta.ToString, 12, "0"), FontFactory.GetFont("Arial", 15, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.RED)))
            Celda.BorderWidth = 0
            Celda.Padding = 10
            Celda.VerticalAlignment = Element.ALIGN_BOTTOM
            Celda.HorizontalAlignment = Element.ALIGN_RIGHT
            Celda.Colspan = 2
            Tabla.AddCell(Celda)

            '=================FIN CERO FILA=================="

            '=================PRIMERA FILA======================'
            Celda = New PdfPCell(New Paragraph(Fecha.ToString, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Celda.Rotation = 90
            Celda.Rowspan = 5
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 5
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("TALONARIOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("USADOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("DEVUELTOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("FALTANTES", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Colspan = 2
            Celda.Rowspan = 8


            '=====================TABLA INDEPENDIENTE==================='
            Dim TablaI1 As PdfPTable = New PdfPTable(2)
            Dim CeldaI1 = New PdfPCell

            CeldaI1 = New PdfPCell(New Paragraph("GASTOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0
            CeldaI1.HorizontalAlignment = Element.ALIGN_CENTER
            CeldaI1.Colspan = 2
            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("TRIPULACION", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0
            CeldaI1.Padding = 1
            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("LUBRICANTES", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("PEAJES", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("ATIEMPADA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("OFICINA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("*OTROS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("DIESEL", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("TOTAL", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            CeldaI1 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI1.BorderWidth = 0

            TablaI1.AddCell(CeldaI1)

            Celda.AddElement(TablaI1)
            '==================FIN DE LA TABLA INDEPENDIENTE======================='

            Tabla.AddCell(Celda)

            '=================FIN PRIMERA FILA=================='

            '=================SEGUNDA FILA======================'

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 4
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            '=================FIN SEGUNDA FILA=================='

            '=================TERCERA FILA'====================='

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            '=================FIN TERCERA FILA=================='

            '=================CUARTA FILA======================='

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            '=================FIN CUARTA FILA==================='

            '=================QUINTA FILA======================='

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            '=================FIN QUINTA FILA==================='

            '=================SEXTA FILA========================'

            Celda = New PdfPCell(New Paragraph("FECHA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Rotation = 90
            Celda.Padding = 10

            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("FECHA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Rotation = 90
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("ANOMALIAS:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Colspan = 4
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            '=================FIN SEXTA FILA===================='

            '=================SEPTIMA FILA========================'

            Celda = New PdfPCell(New Paragraph("HORA: " + Hora.ToString, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Rotation = 90
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("HORA:", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Rotation = 90
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 10
            Celda.Colspan = 3
            Celda.Padding = 0

            '=====================TABLA INDEPENDIENTE==================='
            Dim TablaI2 As PdfPTable = New PdfPTable(2)
            Dim CeldaI2 = New PdfPCell

            CeldaI2 = New PdfPCell(New Paragraph("CALCULO DE ENTREGA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            CeldaI2.Colspan = 3
            CeldaI2.HorizontalAlignment = Element.ALIGN_CENTER
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("CAMINO DE IDA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("CAMINO DE REGRESO", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("SUB-TOTAL", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            CeldaI2.Padding = 2
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("GASTOS (-)", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            CeldaI2.Padding = 2
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("ENTREGA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            CeldaI2 = New PdfPCell(New Paragraph("______________________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI2.BorderWidth = 0
            TablaI2.AddCell(CeldaI2)

            Celda.AddElement(TablaI2)

            '========================FIN DE LA FILA======================'

            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 10
            Celda.Colspan = 3
            Celda.Padding = 10

            '=====================TABLA INDEPENDIENTE==================='
            Dim TablaI3 As PdfPTable = New PdfPTable(3)
            TablaI3.SetWidths({50, 20, 30})
            Dim CeldaI3 = New PdfPCell

            CeldaI3 = New PdfPCell(New Paragraph("OFICINA GUATEMALA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.HorizontalAlignment = Element.ALIGN_CENTER
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph(CantidadBoletos.ToString, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.HorizontalAlignment = Element.ALIGN_RIGHT
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("Q. " + TotalBoletos.ToString, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.HorizontalAlignment = Element.ALIGN_RIGHT
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("CAMINO IDA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("OFICINA ESQUIPULAS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)
            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("CAMINO REGRESO", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("REGRESOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("CHIQUIMULA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("ZACAPA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("QUEZALTEPEQUE", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("OTROS INGRESOS", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("_______", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.Padding = 1
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("          SUB-TOTAL", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.Colspan = 2
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("          GASTOS (-)", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.Colspan = 2
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.Padding = 1
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("          INGRESOS DEL VIAJE", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            CeldaI3.Colspan = 2
            TablaI3.AddCell(CeldaI3)

            CeldaI3 = New PdfPCell(New Paragraph("___________", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            CeldaI3.BorderWidth = 0
            TablaI3.AddCell(CeldaI3)

            Celda.AddElement(TablaI3)

            '==================FIN DE LA TABLA INDEPENDIENTE======================='

            Tabla.AddCell(Celda)

            '=================FIN SEPTIMA FILA===================='

            '====================OCTAVA FILA===================='

            Celda = New PdfPCell(New Paragraph("BUS: " + Bus.ToString, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 4
            Celda.Rotation = 90
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("A", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.VerticalAlignment = Element.ALIGN_CENTER
            Celda.Padding = 10
            Celda.Rotation = 90
            Celda.Rowspan = 2
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("P", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.VerticalAlignment = Element.ALIGN_CENTER
            Celda.Padding = 10
            Celda.Rotation = 90
            Celda.Rowspan = 2
            Tabla.AddCell(Celda)

            '=================FIN OCTAVA FILA===================='

            '==================NOVENA FILA'======================'

            Celda = New PdfPCell(New Paragraph("IDA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Rotation = 90
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph(" ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.Rowspan = 3
            Celda.Padding = 10
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("ESTA CUENTA DEBE HACERSE SIN BORRONES, NI TACHADURAS. CASO CONTRARIO DEBE NOTIFICARSE AL ENCARGADO ", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 0
            Celda.Rowspan = 2
            Celda.Padding = 10
            Celda.Colspan = 6
            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("CALCULO", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.Rowspan = 2
            Celda.Padding = 10

            Tabla.AddCell(Celda)

            Celda = New PdfPCell(New Paragraph("FECHA", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
            Celda.BorderWidth = 1
            Celda.HorizontalAlignment = Element.ALIGN_CENTER
            Celda.Rowspan = 2
            Celda.Padding = 10

            Tabla.AddCell(Celda)

            '==================FIN NOVENA FILA==================='
            doc.Add(Tabla)

            doc.Close()
            Return "pdf/manifiesto" + Correlativo.ToString + ".pdf"
        Catch ex As Exception
            Return "ERROR: " + ex.Message
        End Try
    End Function

    Public Function PadL(ByVal cVar As String, ByVal nLen As Integer, ByVal cCar As String) As String
        Dim n As Integer
        cVar = Trim(cVar)
        nLen = nLen - Len(cVar)
        For n = 1 To nLen
            cVar = cCar & cVar
        Next
        PadL = cVar
    End Function

    <WebMethod()> _
    Public Function muestraHorarios(ByVal fecha As String, ByVal usuario As String) As List(Of HorariosSugeridos)
        'Dim Consulta As String = "call sp_retorna_horarios('" & fecha & "');"
        Dim Consulta As String = "select h.id_detaHorario idhorario,h.fecha_detaHorario fecha,h.id_veh idvehiculo,h.id_emp idpiloto," +
        "h.id_asistente idasistente,h.id_ruta idruta,r.nom_ruta ruta,h.idservicio,CAST(hora.hora as CHAR(10)) hora " +
        "from horarios h " +
        "INNER JOIN rutas r ON h.id_ruta=r.id_ruta " +
        "INNER JOIN hora ON h.id_horario=hora.id_horario " +
        "where h.fecha_detaHorario like '" & fecha & "' order by hora.hora;"

        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    comando = New MySqlCommand(Consulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New HorariosSugeridos
                        Elemento.idhorario = reader("idhorario")
                        Elemento.hora = reader("hora")
                        Elemento.ruta = reader("ruta")
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New HorariosSugeridos
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result

    End Function

    <WebMethod()> _
    Public Function guardarHorario(ByVal fecha As String, ByVal usuario As String, ByVal valor As Integer, ByVal horario As List(Of HorariosSugeridos)) As List(Of HorariosSugeridos)
        Dim Consulta As String = ""

        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc idagencia,id_empsa idempresa from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                Dim elemento As New HorariosSugeridos
                If LectorUsuario.HasRows Then
                    Dim idagencia As Integer = LectorUsuario("idagencia")
                    Dim idempresa As Integer = LectorUsuario("idempresa")

                    LectorUsuario.Close()


                    If valor = 1 Then
                        Consulta = "insert into horarios " +
                        "(id_salida,id_destino,id_horario,id_empsa,fecha_detaHorario," +
                        "id_veh,id_emp,id_asistente,id_usr,id_ruta,idservicio) " +
                        "values " +
                        "(0,0," & horario(0).idhora & "," & idempresa & ",'" & fecha & "'," & horario(0).idvehiculo & "," +
                        "" & horario(0).idpiloto & "," & horario(0).idasistente & ",'" & usuario & "'," & horario(0).idruta & "," & horario(0).idservicio & ")"

                        elemento.accion = "GUARDAR"

                        comando.CommandText = Consulta
                        comando.ExecuteNonQuery()

                        comando.CommandText = "SELECT @@IDENTITY"
                        elemento.idhorario = comando.ExecuteScalar
                        'elemento.idhorario = Val(lemento.idhorario)
                        elemento.mensaje = "Datos guardados correctamente"
                    Else
                        Consulta = "update horarios set id_horario=" & horario(0).idhora & ",id_veh=" & horario(0).idvehiculo & "," +
                            "id_emp=" & horario(0).idpiloto & ",id_asistente=" & horario(0).idasistente & "," +
                            "id_usr='" & usuario & "',idservicio=" & horario(0).idservicio & ",id_ruta=" & horario(0).idruta & " " +
                            "where id_detaHorario=" & horario(0).idhorario & ""

                        elemento.accion = "ACTUALIZAR"

                        comando.CommandText = Consulta
                        comando.ExecuteNonQuery()
                        elemento.mensaje = "Datos actualizados correctamente"
                    End If


                    elemento.correlativo = horario(0).correlativo
                    transaccion.Commit()

                    result.Add(elemento)

                Else
                    LectorUsuario.Close()
                    elemento.mensaje = "ERROR: No se establecieron los datos del usuario"
                    elemento.correlativo = horario(0).correlativo
                End If
            Catch ex As Exception

                Dim Elemento As New HorariosSugeridos
                Elemento.mensaje = "ERROR: " + ex.Message
                Elemento.correlativo = horario(0).correlativo

                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result

    End Function

    <WebMethod()> _
    Public Function sugierePilotoAsistente(ByVal idvehiculo As Integer, ByVal correlativo As Integer) As List(Of HorariosSugeridos)
        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand

            Try
                Dim Consulta As String = "select npiloto idpiloto,id_asistente idasistente from vehiculo where id=" & idvehiculo & ";"

                comando = New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = comando.ExecuteReader()

                While reader.Read()
                    Dim Elemento As New HorariosSugeridos

                    Elemento.idpiloto = IIf(reader("idpiloto") Is DBNull.Value, 0, reader("idpiloto"))
                    Elemento.idasistente = IIf(reader("idasistente") Is DBNull.Value, 0, reader("idasistente"))
                    Elemento.correlativo = correlativo
                    result.Add(Elemento)
                End While
                reader.Close()

            Catch ex As Exception

                Dim Elemento As New HorariosSugeridos
                result.Add(Elemento)
            End Try
        End Using
        Return result


    End Function

    <WebMethod()> _
    Public Function muestraServicio(ByVal usuario As String) As List(Of Servicio)

        Dim result As List(Of Servicio) = New List(Of Servicio)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim SQLConsulta As String
                    SQLConsulta = "select idservicio idservicio,descripcion servicio from Boletos.servicio order by descripcion"

                    comando = New MySqlCommand(SQLConsulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New Servicio
                        Elemento.idservicio = reader("idservicio")
                        Elemento.servicio = reader("servicio").ToString
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Servicio
                Elemento.idservicio = -1
                Elemento.servicio = ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function generaHorariosAnterior(ByVal fecha As String, ByVal usuario As String) As List(Of HorariosSugeridos)
        'Dim Consulta As String = "call sp_retorna_horarios('" & fecha & "');"
        Dim Consulta As String = "select tmp.codigohorario idhorario,hs.id_hora idhora,hs.id_ruta idruta,hs.idservicio idservicio," +
                "tmp.id_veh idvehiculo,tmp.id_emp idpiloto,tmp.id_asistente idasistente ,if(rod.id_origen in (1),'1','2') tipohorario " +
                "from hora_rutasugerida hs " +
                "LEFT JOIN " +
                "(SELECT h.id_detaHorario codigohorario,h.id_horario,h.id_ruta,h.idservicio,h.id_veh,h.id_emp,h.id_asistente " +
                "FROM horarios h " +
                "inner join rutas r " +
                "on h.id_ruta=r.id_ruta " +
                "where h.fecha_detaHorario='" & fecha & "' " +
                "and r.tipo_ruta=1) " +
                "tmp ON hs.id_hora=tmp.id_horario AND hs.id_ruta=tmp.id_ruta " +
                "AND hs.idservicio=tmp.idservicio " +
                "left join " +
                "Encomiendas.vw_rutas_origen_destino rod " +
                "on hs.id_ruta = rod.id_ruta " +
                "inner join hora on hs.id_hora=hora.id_horario " +
                "order by tipohorario,hora.hora;"

        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    comando = New MySqlCommand(Consulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New HorariosSugeridos
                        Elemento.idruta = reader("idruta")
                        Elemento.idhora = reader("idhora")
                        Elemento.idpiloto = IIf(reader("idpiloto") Is DBNull.Value, 0, reader("idpiloto"))
                        Elemento.idasistente = IIf(reader("idasistente") Is DBNull.Value, 0, reader("idasistente"))
                        Elemento.idvehiculo = IIf(reader("idvehiculo") Is DBNull.Value, 0, reader("idvehiculo"))
                        Elemento.clase = IIf(reader("idhorario") Is DBNull.Value, "Pendiente", "Guardado")
                        Elemento.idhorario = IIf(reader("idhorario") Is DBNull.Value, 0, reader("idhorario"))
                        Elemento.idservicio = reader("idservicio")
                        Elemento.tipohorario = reader("tipohorario")
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New HorariosSugeridos
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result

    End Function

    <WebMethod()> _
    Public Function generaHorarios(ByVal fecha As String, ByVal usuario As String) As List(Of HorariosSugeridos)
        'Dim Consulta As String = "call sp_retorna_horarios('" & fecha & "');"
        Dim Consulta As String = "call Boletos.sp_retorna_horarios_boletos('" & fecha & "');"

        Dim result As List(Of HorariosSugeridos) = New List(Of HorariosSugeridos)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    comando = New MySqlCommand(Consulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New HorariosSugeridos
                        Elemento.idruta = reader("idruta")
                        Elemento.idhora = reader("idhora")
                        Elemento.idpiloto = IIf(reader("idpiloto") Is DBNull.Value, 0, reader("idpiloto"))
                        Elemento.idasistente = IIf(reader("idasistente") Is DBNull.Value, 0, reader("idasistente"))
                        Elemento.idvehiculo = IIf(reader("idvehiculo") Is DBNull.Value, 0, reader("idvehiculo"))
                        Elemento.clase = IIf(reader("idhorario") Is DBNull.Value, "Pendiente", "Guardado")
                        Elemento.idhorario = IIf(reader("idhorario") Is DBNull.Value, 0, reader("idhorario"))
                        Elemento.idservicio = reader("idservicio")
                        Elemento.tipohorario = reader("tipohorario")
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New HorariosSugeridos
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result

    End Function

    <WebMethod()> _
    Public Function muestraAsistentes(ByVal usuario As String) As List(Of Asistente)

        Dim result As List(Of Asistente) = New List(Of Asistente)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim SQLConsulta As String
                    SQLConsulta = "select id_emp idpiloto,nom_emp piloto from empleado where id_pso=2 and estatus=1 order by nom_emp;"

                    comando = New MySqlCommand(SQLConsulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New Asistente
                        Elemento.idasistente = reader("idpiloto")
                        Elemento.asistente = reader("piloto").ToString
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Asistente
                Elemento.idasistente = -1
                Elemento.asistente = ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function muestraPilotos(ByVal usuario As String) As List(Of Piloto)

        Dim result As List(Of Piloto) = New List(Of Piloto)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim SQLConsulta As String
                    SQLConsulta = "select id_emp idpiloto,nom_emp piloto from empleado where id_pso=1 and estatus=1 order by nom_emp;"

                    comando = New MySqlCommand(SQLConsulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New Piloto
                        Elemento.idpiloto = reader("idpiloto")
                        Elemento.piloto = reader("piloto").ToString
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Piloto
                Elemento.idpiloto = -1
                Elemento.piloto = ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function muestraVehiculos(ByVal usuario As String) As List(Of Vehiculo)

        Dim result As List(Of Vehiculo) = New List(Of Vehiculo)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim SQLConsulta As String
                    SQLConsulta = "select id idvehiculo, id_veh vehiculo from vehiculo order by id_veh+0"

                    comando = New MySqlCommand(SQLConsulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New Vehiculo
                        Elemento.idvehiculo = reader("idvehiculo")
                        Elemento.vehiculo = reader("vehiculo").ToString
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Vehiculo
                Elemento.idvehiculo = -1
                Elemento.vehiculo = ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function muestraRutas(ByVal usuario As String) As List(Of Rutas)

        Dim result As List(Of Rutas) = New List(Of Rutas)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim codigoagencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim SQLConsulta As String
                    SQLConsulta = "select id_ruta idruta,nom_ruta nombre from rutas where ruta_pasajeros=1 order by nom_ruta"

                    comando = New MySqlCommand(SQLConsulta, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While reader.Read()
                        Dim Elemento As New Rutas
                        Elemento.idruta = reader("idruta")
                        Elemento.nombre = reader("nombre").ToString
                        result.Add(Elemento)
                    End While
                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Rutas
                Elemento.idruta = -1
                Elemento.nombre = ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function muestraHoras(ByVal usuario As String) As List(Of Horas)
        Dim result As List(Of [Horas]) = New List(Of [Horas])
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Try

                Dim SQLConsulta As String

                SQLConsulta = "select id_horario idhora,hora hora from hora order by hora;"

                comando = New MySqlCommand(SQLConsulta, connection)
                Dim reader As MySqlDataReader = comando.ExecuteReader()


                While reader.Read()
                    Dim Elemento As New Horas
                    Elemento.idhora = reader("idhora")
                    Elemento.hora = reader("hora").ToString
                    result.Add(Elemento)
                End While
                reader.Close()

            Catch ex As Exception
                Dim Elemento As New Horas
                Elemento.idhora = -1
                Elemento.hora = ex.Message
                result.Add(Elemento)
            End Try

        End Using
        Return result
    End Function

    Public Function NumeroAleatorioPdf() As Integer
        Dim LimiteMinimo As Integer = 1000
        Dim LimiteMaximo As Integer = 9999
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(1000, 9999)
        Return Aleatorio
    End Function

    Public Class Asistente
        Public idasistente As Integer
        Public asistente As String
    End Class

    Public Class Piloto
        Public idpiloto As Integer
        Public piloto As String
    End Class

    Public Class Horas
        Public idhora As Integer
        Public hora As String
    End Class

    Public Class Rutas
        Public idruta As Integer
        Public nombre As String
    End Class

    Public Class Vehiculo
        Public idvehiculo As Integer
        Public vehiculo As String
    End Class

    Public Class Servicio
        Public idservicio As Integer
        Public servicio As String
    End Class

    Public Function NumeroAleatorio() As Integer
        Dim LimiteMinimo As Integer = 0
        Dim LimiteMaximo As Integer = 20
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return Aleatorio
    End Function

    Public Class HorariosSugeridos
        Public idasistente As Integer
        Public idpiloto As Integer
        Public idhora As Integer
        Public idruta As Integer
        Public idvehiculo As Integer
        Public clase As String
        Public idservicio As Integer
        Public idhorario As Integer
        Public correlativo As Integer
        Public mensaje As String
        Public accion As String
        Public tipohorario As Integer
        Public hora As String
        Public ruta As String
    End Class
End Class