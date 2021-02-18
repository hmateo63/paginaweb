Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wscuadre_caja
    Inherits System.Web.Services.WebService

    <WebMethod(True)> _
    Public Function cargaUsuarios(ByVal usuario As String) As List(Of [Usuario])
        Dim result As List(Of [Usuario]) = New List(Of Usuario)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            connection.Open()
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim Agencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim Busqueda As String = "select u.id_usr idusuario from Facturacion.agencia_asig_caja agsc " +
                    "INNER JOIN Facturacion.usuario u ON agsc.id_usr=u.id_usr " +
                    "where u.id_agc=" & Agencia & " order by u.nom_usr"
                    comando = New MySqlCommand(Busqueda, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()
                    While (reader.Read())
                        Dim Elemento As New Usuario

                        Elemento.idusuario = reader("idusuario")

                        result.Add(Elemento)
                    End While

                    reader.Close()
                Else
                    LectorUsuario.Close()
                End If
                comando.Dispose()

            Catch ex As MySqlException

            Finally
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaCajas(ByVal usuario As String) As List(Of [Caja])
        Dim result As List(Of [Caja]) = New List(Of Caja)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            connection.Open()
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                comando = New MySqlCommand("select id_agc codigoagencia from Facturacion.usuario where id_usr='" & usuario & "'", connection)
                Dim LectorUsuario As MySqlDataReader = comando.ExecuteReader()

                LectorUsuario.Read()
                If LectorUsuario.HasRows Then
                    Dim Agencia As Integer = LectorUsuario("codigoagencia")
                    LectorUsuario.Close()

                    Dim Busqueda As String = "select id_agenciacaja idcaja,desc_agenciacaja nombrecaja from Facturacion.agenciacaja where id_agc=" & Agencia & " order by nombrecaja;"
                    comando = New MySqlCommand(Busqueda, connection)
                    Dim reader As MySqlDataReader = comando.ExecuteReader()

                    While (reader.Read())
                        Dim Elemento As New Caja

                        Elemento.idcaja = reader("idcaja")
                        Elemento.nombrecaja = reader("nombrecaja")
                        result.Add(Elemento)
                    End While

                    reader.Close()

                Else
                    LectorUsuario.Close()
                End If
                comando.Dispose()

            Catch ex As MySqlException

            Finally
                transaccion.Dispose()
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function fechaActualguiones() As String
        Dim fecha As String = ""
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim ComandoHorario As New MySqlCommand(String.Format("select DATE_FORMAT(curdate(),'%Y-%m-%d') as fecha;"), connection)
                Dim Lector As MySqlDataReader = ComandoHorario.ExecuteReader()
                Lector.Read()

                If Lector.HasRows Then
                    Return Lector("fecha")

                Else
                    Return "2013-00-00"

                End If

            Catch ex As Exception
                Return "no funciona " + ex.Message
            End Try
        End Using

    End Function

    Public Class Usuario
        Public idusuario As String
    End Class
    Public Class Caja
        Public idcaja As String
        Public nombrecaja As String
    End Class
End Class