Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
Imports System.Data
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsreportes
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function generarReporte(ByVal usuario As String, ByVal tiporeporte As Integer, ByVal fechainicio As String, ByVal fechafinal As String, ByVal origen As String, ByVal destino As String, ByVal ruta As String, ByVal hora As String, ByVal servicio As String, ByVal vehiculo As String, ByVal piloto As String, ByVal asistente As String, ByVal inicio As Integer, ByVal numerofilas As Integer) As List(Of Reporte)
        Dim result As List(Of Reporte) = New List(Of Reporte)
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
                    SQLConsulta = "call Boletos.sp_reporte(" & tiporeporte & ",'" & fechainicio & "','" & fechafinal & "','" & origen & "','" & destino & "','" & ruta & "','" & hora & "','" & servicio & "','" & vehiculo & "','" & piloto & "','" & asistente & "'," & inicio & "," & numerofilas & ");"

                    Dim TablaDatos As DataTable = DatosSql.cargar_datatable(SQLConsulta)
                    Dim Elemento As Reporte
                    For fila = 0 To TablaDatos.Rows.Count - 1
                        Elemento = New Reporte
                        For columna = 0 To TablaDatos.Columns.Count - 1
                            Select Case columna
                                Case 0 : Elemento.Fecha = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 1 : Elemento.Hora = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 2 : Elemento.Ruta = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 3 : Elemento.Servicio = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 4 : Elemento.Boletos = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 5 : Elemento.Total_Boletos = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 6 : Elemento.Boletos_Ruta = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 7 : Elemento.Total_Boletos_Ruta = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 8 : Elemento.Piloto = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 9 : Elemento.Asistente = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 10 : Elemento.Vehiculo = TablaDatos.Rows(fila).Item(columna).ToString
                                Case 11 : Elemento.totalregistros = TablaDatos.Rows(fila).Item(columna).ToString
                            End Select

                        Next
                        result.Add(Elemento)
                    Next
                Else
                    LectorUsuario.Close()
                End If
            Catch ex As Exception

                Dim Elemento As New Reporte
                Elemento.Fecha = "ERROR: " + ex.Message
                result.Add(Elemento)
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    Public Class Reporte

        Public Fecha As String
        Public Hora As String
        Public Ruta As String
        Public Servicio As String
        Public Boletos As Integer
        Public Total_Boletos As Double
        Public Boletos_Ruta As Integer
        Public Total_Boletos_Ruta As Double
        Public Piloto As String
        Public Asistente As String
        Public Vehiculo As String
        Public totalregistros As Integer
    End Class

End Class