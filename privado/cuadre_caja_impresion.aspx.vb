Imports System.IO
Imports MySql.Data.MySqlClient
Partial Class cierredecaja
    Inherits System.Web.UI.Page
    Dim fila As GridViewRow

    Dim entrada, año, mes, dia, fecha As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("id_usr") = "" Then
            FormsAuthentication.RedirectToLoginPage()
        Else
            CargaGrid()
        End If


    End Sub

    Sub CargaGrid()

        Dim TableGrid, TableGridCaja As System.Data.DataTable
        Dim agencia As String = ""
        Dim nombreagencia As String = ""
        Dim Reporte As String
        Dim FechaInicial, FechaFinal, caja, caja1, cajas, query2, usuario As String
        Dim fila As Integer = 0
        Dim columna As Integer = 0
        Dim resaltado As Boolean = False
        Dim nuevacaja As String = ""

        FechaInicial = Request.QueryString("fechaInicial")
        FechaFinal = Request.QueryString("fechaFinal")
        caja = Request.QueryString("caja")
        usuario = Request.QueryString("usuario")

        Dim Busqueda As String = "select u.id_agc idagencia,u.id_empsa idempresa,aca.id_agenciacaja idcaja " +
        "from Facturacion.usuario u LEFT JOIN Facturacion.agencia_asig_caja aca ON u.id_usr=aca.id_usr where u.id_usr='" & Session("id_usr") & "'"

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()

                reader.Read()
                If reader.HasRows Then
                    agencia = reader("idagencia")
                End If
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException

            End Try
        End Using


        FechaInicial = Request.QueryString("fechaInicial")
        FechaFinal = Request.QueryString("fechaFinal")
        caja = Request.QueryString("caja")
        usuario = Request.QueryString("usuario")
        query2 = "  "

        If Trim(caja) = "%" Then
            caja1 = "%"
            nombreagencia = "SELECT nom_agc FROM Facturacion.agencia where id_agc=" & agencia & ""
            TableGridCaja = DatosSql.cargar_datatable(nombreagencia)
            nombreagencia = TableGridCaja.Rows(0).Item(0).ToString
        Else
            cajas = "SELECT ac.id_agenciacaja as idcaja, desc_agenciacaja as caja,upper(a.nom_agc), id_usr as usuario, a.nom_agc" +
               " FROM Facturacion.agencia_asig_caja ac inner join Facturacion.agenciacaja c on ac.id_agenciacaja = c.id_agenciacaja" +
             " inner join Facturacion.agencia a on c.id_agc=a.id_agc" +
             " where(a.id_agc = '" & agencia & "') and ac.id_agenciacaja=" & caja & ""


            TableGridCaja = DatosSql.cargar_datatable(cajas)
            caja1 = Trim(caja)

        End If


        Reporte = "call Facturacion.CuadreCaja('" & Trim(FechaInicial) & "','" & Trim(FechaFinal) & "','" & agencia & "','" & usuario & "','" & caja1 & "')"
        TableGrid = DatosSql.cargar_cuadrecaja(Reporte)
        'For I = 1 To 3
        'MsgBox(TableGrid.Rows(I).Item(2).ToString)
        'Next

        '---------Se genera la tabla con la informacion del reporte-----------'

        Response.Write("<div><table class='bordered'> <tr style='background:white;'>" +
                "<td colspan=" & TableGrid.Columns.Count & ">" +
                "<a href='cuadre_caja.aspx'>" +
                    "<p style='font-size:x-large;text-align:center;'>REPORTE DE INGRESOS DIARIOS</p>" +
                "</a> </td></tr> ")


        '---------Se determina si el reporte es por caja o por Facturacion.usuario -------------

        If Trim(usuario) = "%" Then
            Response.Write("<tr style='background:white;'><td colspan=" & TableGrid.Columns.Count & ">" +
                           " <center> AGENCIA: " & UCase(nombreagencia) & " </center>" +
                               "<center> REPORTE DE LA CAJA: " & UCase(caja) & "</center>" +
                               "<center> DEL " & FechaInicial & " AL " & FechaFinal & " GENERADO POR " & UCase(Session("id_usr")) & "</center></td></tr>")

        ElseIf Trim(caja) = "%" Then
            Response.Write("<tr style='background:white;'><td colspan=" & TableGrid.Columns.Count & "><center> AGENCIA: " & UCase(nombreagencia) & " </center>" +
                "<center> REPORTE DEL USUARIO: " & UCase(usuario) & "</center>" +
                 "<center> DEL " & FechaInicial & " AL " & FechaFinal & " GENERADO POR " & UCase(Session("id_usr")) + "</center></td></tr>")
        Else
            Response.Write("<tr style='background:white;'><td colspan=" & TableGrid.Columns.Count & " class=td></td></tr>")
        End If



        '----------Encabezado de Reporte Cierre de Caja--------------

        Response.Write("<td class='encabezado'><center>" & "Tipo De Documento" & "</td>" +
                  "<td class='encabezado'><center>" & "Rango Del" & "</td>" +
                  "<td class='encabezado'><center>" & "Al" & "</td>" +
                  "<td class='encabezado'><center>" & "Subtotal" & "</td>" +
                  "<td class='encabezado'><center>" & "   Total    " & "</center></td>")


        '''<td class='encabezado'><center>" & "Venta Neta" & "</td>'''
        '''<td class='encabezado'><center>" & "Iva" & "</td>'''


        '----------
        For fila = 0 To TableGrid.Rows.Count - 1
            Response.Write("<tr>")

            columna = 0

            If TableGrid.Rows(fila).Item(0).ToString.Length > 4 Then

                If TableGrid.Rows(fila).Item(0).ToString.Substring(0, 5) = "Total" Then

                    resaltado = True

                Else
                    resaltado = False

                End If
            Else
                resaltado = False

            End If

            For columna = 0 To TableGrid.Columns.Count - 1



                Response.Write("<td class=" & IIf(columna > 2, "tdnumero ", "td ") & "style=background-color:" & IIf(resaltado, "lightgrey>", "white>") & TableGrid.Rows(fila).Item(columna) & "</td>")


            Next
            Response.Write("</tr>")
        Next


        Response.Write("</table>")


        Response.Write(" </div>")

       
        '--------FINALIZA TOTAL A DEPOSITAR-------


    End Sub
End Class

