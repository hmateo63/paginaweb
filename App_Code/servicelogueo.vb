Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Web.Services
Imports MySql.Data.MySqlClient
Imports AjaxControlToolkit
Imports System.Web.Script.Services
Imports System.Web
Imports System.Web.Services.Protocols
Imports System.Collections.Specialized
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
<WebService([Namespace]:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<System.ComponentModel.ToolboxItem(False)> _
<System.Web.Script.Services.ScriptService()> _
Public Class servicelogueo
    Inherits System.Web.Services.WebService


    <WebMethod()> _
    Public Function VerificaExistenciaUsuario(ByVal usuario As String) As List(Of [Claselogin])
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)

            Dim consulta As String = "SELECT id_usr FROM Facturacion.usuario " +
           "where id_usr='" + usuario + "';"
            Dim TablaUsuario As DataTable = DatosSql.cargar_datatable(consulta)
            Dim result As List(Of [Claselogin]) = New List(Of Claselogin)()
            Try
                Dim Elemento As New Claselogin

                If TablaUsuario.Rows.Count > 0 Then

                    connection.Open()

                    Dim consultaloguin As String = "SELECT id_usr,id_agc,cast(fecha as char(10))as fechalogueo,hora FROM Encomiendas.log_registrologueos " +
                    "where id_usr='" + usuario + "' and fecha=curdate();"

                    Dim cmd2 As New MySqlCommand(consultaloguin, connection)
                    Dim reader2 As MySqlDataReader = cmd2.ExecuteReader()
                    reader2.Read()

                    If reader2.HasRows Then
                        Elemento.usuario = TablaUsuario.Rows(0).Item("id_usr").ToString
                        result.Add(Elemento)
                    Else
                        Elemento.usuario = "false"
                        result.Add(Elemento)
                    End If
                    Return result
                Else
                    Elemento.usuario = "falseusuario"
                    result.Add(Elemento)
                    Return result
                End If


            Catch ex As Exception
                connection.Close()
                Dim Elemento As New Claselogin
                Elemento.usuario = "false"
                Elemento.fechalogueo = "null"
                Return result
            End Try
        End Using

    End Function

    Public Class Claselogin
        Public usuario As String
        Public fechalogueo As String
    End Class

End Class
