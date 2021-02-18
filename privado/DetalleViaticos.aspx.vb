Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient

Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.html
Imports iTextSharp.text.html.simpleparser

Imports System.IO
Imports System.Data
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

Imports System.Drawing
Partial Class privado_DetalleViaticos
    Inherits System.Web.UI.Page

    Dim Accion As Integer = 0
    Dim codigopagina As Integer = 3



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Accion = 1

        Dim strsql As String = ""
        Dim comando As MySqlCommand
        Dim DetalleViaticos As Integer = 0

        Using conexion As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            strsql = "select DetalleViaticos  FROM Facturacion.usuario where id_usr='" & Session("id_usr") & "';"
            comando = New MySqlCommand(strsql, conexion)
            conexion.Open()
            DetalleViaticos = comando.ExecuteScalar
            conexion.Close()
        End Using

        'If Session("id_usr").Equals("admin") OrElse Session("id_usr").Equals("lrivera") OrElse Session("id_usr").Equals("balvarado") OrElse Session("id_usr").Equals("llopez") OrElse Session("id_usr").Equals("rmartinez") OrElse Session("id_usr").Equals("kcruz") OrElse Session("id_usr").Equals("kgomezboletos") OrElse Session("id_usr").Equals("ehernandez") OrElse Session("id_usr").Equals("surizar") Then

        If DetalleViaticos = 1 Then
            If Session("id_usr") = "" Then
                FormsAuthentication.RedirectToLoginPage()
            Else
                husuario.Value = Session("id_usr")
                hagencia.Value = Session("id_agc")
                If DatosSql.permiso(husuario.Value, Accion, codigopagina) = 0 Then
                    Response.Redirect("error.html")
                End If
            End If
        Else
            Response.Redirect("error.html")
        End If

    End Sub
    Protected Sub MostrarGrid(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExport.Click
        Dim varFechaInicial As String = txtFechaInicial.Text
        Dim varFechaFinal As String = txtFechaFinal.Text
        If varFechaFinal.ToString >= varFechaInicial.ToString Then
            Me.Button1.Visible = True
            'Me.btnToPdf.Visible = True
            'Me.btnToWord.Visible = True
            Me.BindGrid()
        End If
    End Sub
    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.SelectedIndexChanged
    End Sub
    Protected Sub OnPageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        GridView1.PageIndex = e.NewPageIndex
        Me.BindGrid()
    End Sub
    Protected Sub ExportToExcel(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        Me.GenerarExcel()
    End Sub

    Protected Sub ExportToWord(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnToWord.Click
        Me.GenerarWord()
    End Sub
    Private Sub BindGrid()
        Dim varFechaInicial As String = txtFechaInicial.Text
        Dim varFechaFinal As String = txtFechaFinal.Text
        Dim strConnString As String = ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
        Using con As New MySqlConnection(strConnString)
            Using cmd As New MySqlCommand("call Planillas.sp_ViaticosTarjetasDetalle('" & varFechaInicial & "','" & varFechaFinal & "',1);")

                Using sda As New MySqlDataAdapter()
                    cmd.Connection = con
                    sda.SelectCommand = cmd
                    Using dt As New DataTable()
                        sda.Fill(dt)
                        GridView1.DataSource = dt
                        GridView1.DataBind()
                    End Using
                End Using
            End Using
        End Using
    End Sub
    Public Function GridViewToHtml(ByVal grid As GridView) As String
        Dim sTringBuilder As New StringBuilder()
        Dim sTringWriter As New StringWriter(sTringBuilder)
        Dim hTmlTextWriter As New HtmlTextWriter(sTringWriter)
        grid.RenderControl(hTmlTextWriter)
        Return sTringBuilder.ToString()
    End Function
    Private Sub GenerarPdf()
        'Dim columnsCount As Integer = GridView1.HeaderRow.Cells.Count
        'Dim pdfTable As New PdfPTable(columnsCount)
        'For Each gridViewHeaderCell As TableCell In GridView1.HeaderRow.Cells
        '    Dim font As New iTextSharp.text.Font()
        '    iTextSharp.text.Font.Color = New BaseColor(GridView1.HeaderStyle.ForeColor)
        '    Dim pdfCell As New PdfPCell(New Phrase(gridViewHeaderCell.Text, font))
        '    pdfCell.BackgroundColor = New BaseColor(GridView1.HeaderStyle.BackColor)
        '    pdfTable.AddCell(pdfCell)
        'Next
        'For Each gridViewRow As GridViewRow In GridView1.Rows
        '    If gridViewRow.RowType = DataControlRowType.DataRow Then
        '        For Each gridViewCell As TableCell In gridViewRow.Cells
        '            Dim font As New iTextSharp.text.Font()
        '            iTextSharp.text.Font.Color = New BaseColor(GridView1.RowStyle.ForeColor)
        '            Dim pdfCell As New PdfPCell(New Phrase(gridViewCell.Text, font))
        '            pdfCell.BackgroundColor = New BaseColor(GridView1.RowStyle.BackColor)
        '            pdfTable.AddCell(pdfCell)
        '        Next
        '    End If
        'Next
        ''Dim pdfDocument As New Document(PageSize.LETTER, 5.0F, 10.0F, 15.0F, 12.0F)
        'Dim pdfDocument As New Document(PageSize.LETTER, 5.0F, 5.0F, 5.0F, 5.0F)
        'PdfWriter.GetInstance(pdfDocument, Response.OutputStream)
        'pdfDocument.Open()
        'pdfDocument.Add(pdfTable)
        'pdfDocument.Close()
        'Response.ContentType = "application/pdf"
        'Response.AppendHeader("content-disposition", "attachment;filename=Viaticos.pdf")
        'Response.Write(pdfDocument)
        'Response.Flush()
        'Response.[End]()
    End Sub
    Private Sub EjemploGenerarPDF()
        Dim oDoc As New iTextSharp.text.Document(PageSize.A4, 0, 0, 0, 0)
        Dim pdfw As iTextSharp.text.pdf.PdfWriter
        Dim cb As PdfContentByte
        Dim fuente As iTextSharp.text.pdf.BaseFont
        Dim NombreArchivo As String = "d:\ejemplo.pdf"
        Try
            pdfw = PdfWriter.GetInstance(oDoc, New FileStream(NombreArchivo, _
            FileMode.Create, FileAccess.Write, FileShare.None))
            'Apertura del documento.
            oDoc.Open()
            cb = pdfw.DirectContent
            'Agregamos una pagina.
            oDoc.NewPage()
            'Iniciamos el flujo de bytes.
            cb.BeginText()
            'Instanciamos el objeto para la tipo de letra.
            fuente = FontFactory.GetFont(FontFactory.HELVETICA, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL).BaseFont
            'Seteamos el tipo de letra y el tamaño.
            cb.SetFontAndSize(fuente, 12)
            'Seteamos el color del texto a escribir.

            'cb.SetColorFill(iTextSharp.text.Color.BLACK)

            'Aqui es donde se escribe el texto.
            'Aclaracion: Por alguna razon la coordenada vertical siempre es tomada desde el borde inferior (de ahi que se calcule como "PageSize.A4.Height - 50")
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Ejemplo basico con iTextSharp", 200, PageSize.A4.Height - 50, 0)
            'Fin del flujo de bytes.
            cb.EndText()
            'Forzamos vaciamiento del buffer.
            pdfw.Flush()
            'Cerramos el documento.
            oDoc.Close()
        Catch ex As Exception
            'Si hubo una excepcion y el archivo existe ...
            If File.Exists(NombreArchivo) Then
                'Cerramos el documento si esta abierto.
                'Y asi desbloqueamos el archivo para su eliminacion.
                If oDoc.IsOpen Then oDoc.Close()
                '... lo eliminamos de disco.
                File.Delete(NombreArchivo)
            End If
            Throw New Exception("Error al generar archivo PDF ")
        Finally
            cb = Nothing
            pdfw = Nothing
            oDoc = Nothing
        End Try
    End Sub

    Private Sub GenerarWord()
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
        Response.AddHeader("content-disposition", "attachment;filename=Viaticos.doc")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-word "
        Response.Charset = "UTF-8"
        Response.ContentEncoding = Encoding.Default
        Response.Write(sb.ToString())
        Response.End()

    End Sub

    Protected Sub ExportToPdf(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnToPdf.Click
        Me.GenerarPdf()

        'InsertarTabla(cb, dt, New Single() {1.0F, 2.0F}, True)

    End Sub
    Private Sub InsertarTabla(ByRef pCb As pdf.PdfContentByte, ByRef pTabla As DataTable, ByRef pDimensionColumnas() As Single, ByVal pIncluirEncabezado As Boolean)

        Dim table As New iTextSharp.text.pdf.PdfPTable(pTabla.Columns.Count)
        table.TotalWidth = pCb.PdfDocument.PageSize.Width - 100

        table.SetWidths(pDimensionColumnas)

        table.SpacingBefore = 1.0F
        table.SpacingAfter = 1.0F

        If pIncluirEncabezado Then
            Dim fuenteEncabezado As iTextSharp.text.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, iTextSharp.text.Font.NORMAL)

            For Each oColumna As Data.DataColumn In pTabla.Columns
                Dim celda As New pdf.PdfPCell(New Phrase(oColumna.Caption, fuenteEncabezado))
                celda.Colspan = 1
                celda.Padding = 5
                celda.BackgroundColor = iTextSharp.text.pdf.ExtendedColor.CYAN
                celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER
                celda.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP
                table.AddCell(celda)
            Next oColumna
        End If

        Dim fuenteDatos As iTextSharp.text.Font = FontFactory.GetFont(FontFactory.COURIER, 8, iTextSharp.text.Font.NORMAL)

        For Each oFila As Data.DataRow In pTabla.Rows
            For Each oColumna As Data.DataColumn In pTabla.Columns
                Dim celda As New pdf.PdfPCell(New Phrase(oFila(oColumna), fuenteDatos))
                celda.Colspan = 1
                celda.Padding = 3
                celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER
                celda.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP
                table.AddCell(celda)
            Next oColumna
        Next oFila
        table.WriteSelectedRows(0, -1, 50, 800, pCb)
        pCb.PdfWriter.Flush()
    End Sub
    Private Sub s_GenerarExcel()

        Response.Clear()
        Response.Buffer = True

        Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)

        GridView1.AllowPaging = False
        GridView1.DataBind()

        GridView1.HeaderRow.Style.Add("background-color", "#FFFFFF")
        GridView1.HeaderRow.Cells(0).Style.Add("background-color", "green")
        GridView1.HeaderRow.Cells(1).Style.Add("background-color", "green")
        GridView1.HeaderRow.Cells(2).Style.Add("background-color", "green")

        Dim arr As ArrayList = DirectCast(ViewState("States"), ArrayList)
        GridView1.HeaderRow.Cells(0).Visible = Convert.ToBoolean(arr(0))
        GridView1.HeaderRow.Cells(1).Visible = Convert.ToBoolean(arr(1))
        GridView1.HeaderRow.Cells(2).Visible = Convert.ToBoolean(arr(2))

        GridView1.HeaderRow.Cells(0).FindControl("chkCol0").Visible = False
        GridView1.HeaderRow.Cells(1).FindControl("chkCol1").Visible = False
        GridView1.HeaderRow.Cells(2).FindControl("chkCol2").Visible = False

        For i As Integer = 0 To GridView1.Rows.Count - 1
            Dim row As GridViewRow = GridView1.Rows(i)
            row.Cells(0).Visible = Convert.ToBoolean(arr(0))
            row.Cells(1).Visible = Convert.ToBoolean(arr(1))
            row.Cells(2).Visible = Convert.ToBoolean(arr(2))
            row.BackColor = System.Drawing.Color.White
            row.Attributes.Add("class", "textmode")
            If i Mod 2 <> 0 Then
                row.Cells(0).Style.Add("background-color", "#C2D69B")
                row.Cells(1).Style.Add("background-color", "#C2D69B")
                row.Cells(2).Style.Add("background-color", "#C2D69B")
            End If
        Next
        GridView1.RenderControl(hw)
        Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
        Response.Write(style)
        Response.Output.Write(sw.ToString())
        Response.[End]()

    End Sub

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
        Response.AddHeader("Content-Disposition", "attachment;filename=Viaticos.xls")
        Response.Charset = "UTF-8"
        Response.ContentEncoding = Encoding.Default
        Response.Write(sb.ToString())
        Response.End()
    End Sub
    Private Sub s3GenerarExcel()

        Response.Clear()
        Response.Buffer = True
        Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls")
        Response.Charset = ""
        Response.ContentType = "application/vnd.ms-excel"

        'Dim sb As StringBuilder = New StringBuilder()
        Using sw As StringWriter = New StringWriter()

            Dim hw As HtmlTextWriter = New HtmlTextWriter(sw)

            'Using sw As New StringWriter()

            'To Export all pages
            'GridView1.AllowPaging = False
            'Me.BindGrid()

            GridView1.HeaderRow.BackColor = Color.White
            For Each cell As TableCell In GridView1.HeaderRow.Cells
                cell.BackColor = GridView1.HeaderStyle.BackColor
            Next
            For Each row As GridViewRow In GridView1.Rows
                row.BackColor = Color.White
                For Each cell As TableCell In row.Cells
                    If row.RowIndex Mod 2 = 0 Then
                        cell.BackColor = GridView1.AlternatingRowStyle.BackColor
                    Else
                        cell.BackColor = GridView1.RowStyle.BackColor
                    End If
                    cell.CssClass = "textmode"
                Next
            Next

            GridView1.RenderControl(hw)
            'style to format numbers to string
            Dim style As String = "<style> .textmode { mso-number-format:\@; } </style>"
            Response.Write(style)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.[End]()
        End Using

    End Sub

    Private Sub s4GenerarExcel()
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
        Response.AddHeader("Content-Disposition", "attachment;filename=Viaticos.xls")
        Response.Charset = "UTF-8"
        Response.ContentEncoding = Encoding.Default

        For Each row As GridViewRow In GridView1.Rows
            row.BackColor = Color.AliceBlue
            For Each cell As TableCell In row.Cells
                If row.RowIndex Mod 2 = 0 Then
                    cell.BackColor = GridView1.AlternatingRowStyle.BackColor
                Else
                    cell.BackColor = GridView1.RowStyle.BackColor
                End If
                cell.CssClass = "textmode"
            Next
        Next

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

    Private Sub s5GenerarExcel()
        Response.ClearContent()
        Response.AppendHeader("content-disposition", "attachment; filename=studentDetails.xls")
        Response.ContentType = "application/excel"
        Response.Write(Formatted_GridViewToHtml(GridView1))
        Response.[End]()

    End Sub


    Public Overrides Sub VerifyRenderingInServerForm(ByVal control As Control)
        'This method will Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
    End Sub

End Class
