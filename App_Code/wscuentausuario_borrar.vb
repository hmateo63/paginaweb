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
Public Class wscuentausuario
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function CambioPassword(ByVal idusuario As String, ByVal passwordactual As String, ByVal passwordnueva As String) As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)

            Dim consulta As String = "SELECT id_usr FROM Facturacion.usuario " +
           "where id_usr='" + idusuario + "' and pass_usr='" & passwordactual & "';"
            Dim TablaUsuario As DataTable = DatosSql.cargar_datatable(consulta)
            Try

                If TablaUsuario.Rows.Count > 0 Then
                    connection.Open()
                    Dim actualizacion As String = "update Facturacion.usuario set pass_usr='" & passwordnueva & "' where id_usr='" & idusuario & "';"

                    Dim cmd2 As New MySqlCommand(actualizacion, connection)
                    cmd2.ExecuteNonQuery()

                    Return "Datos guardados correctamente"
                Else
                    Return "ERROR: Datos incorrectos"
                End If


            Catch ex As Exception
                connection.Close()
                Return "ERROR: " + ex.Message
            End Try
        End Using
    End Function

End Class