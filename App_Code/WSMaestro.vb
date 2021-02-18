Imports System.Web.Script.Services
Imports AjaxControlToolkit
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Collections.Generic
Imports System.Collections.Specialized
'Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.IO
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WSMaestro
    Inherits System.Web.Services.WebService
    <WebMethod()> _
    Public Function GuardarEmpresa(ByVal nombre As String, ByVal telefono As String, ByVal direccion As String, ByVal primerContacto As String, ByVal segundoContacto As String, ByVal primerCorreo As String, ByVal segundoCorreo As String, ByVal fax As String) As String
        Dim Sqlinserta As String = "insert into empresa (nom_empsa, tel_empsa, dire_empsa, contact1_empsa, contact2_empsa, correo1_empsa, correo2_empsa, fax_empsa, estado) values ('" + nombre + "','" + telefono + "', '" + direccion + "', '" + primerContacto + "', '" + segundoContacto + "', '" + primerCorreo + "', '" + segundoCorreo + "', '" + fax + "', true)"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function ModificarEmpresa(ByVal nombre As String, ByVal telefono As String, ByVal direccion As String, ByVal primerContacto As String, ByVal segundoContacto As String, ByVal primerCorreo As String, ByVal segundoCorreo As String, ByVal fax As String, ByVal valor As String) As String
        Dim SqlModifica As String = "update empresa set nom_empsa='" + nombre + "', tel_empsa='" + telefono + "', dire_empsa='" + direccion + "', contact1_empsa='" + primerContacto + "', contact2_empsa='" + segundoContacto + "', correo1_empsa='" + primerCorreo + "', correo2_empsa='" + segundoCorreo + "', fax_empsa='" + fax + "' where id_empsa='" & valor & "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlModifica, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlModifica
            Catch ex As MySqlException
                Return SqlModifica
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarEmpresa(ByVal empresa As String) As List(Of [ClaseEmpresa])
        Dim consulta As String = "select nom_empsa, tel_empsa, dire_empsa, contact1_empsa, contact2_empsa, correo1_empsa, correo2_empsa, fax_empsa from empresa where id_empsa='" + empresa + "'"
        Dim result As List(Of [ClaseEmpresa]) = New List(Of ClaseEmpresa)()
        Dim Elemento As New ClaseEmpresa
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                reader.Read()
                If reader.HasRows Then
                    Elemento.nombre = reader("nom_empsa")
                    Elemento.telefono = reader("tel_empsa")
                    Elemento.direccion = reader("dire_empsa")
                    Elemento.primerContacto = reader("contact1_empsa")
                    Elemento.segundoContacto = reader("contact2_empsa")
                    Elemento.primerCorreo = reader("correo1_empsa")
                    Elemento.segundoCorreo = reader("correo2_empsa")
                    Elemento.fax = reader("fax_empsa")
                    result.Add(Elemento)
                End If
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseEmpresa
        Public nombre As String
        Public telefono As String
        Public direccion As String
        Public primerContacto As String
        Public segundoContacto As String
        Public primerCorreo As String
        Public segundoCorreo As String
        Public fax As String
    End Class


    <WebMethod()> _
    Public Function BajaEmpresa(ByVal valor As String) As String
        Dim SqlBaja As String = "update empresa set estado='0' where id_empsa='" & valor & "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlBaja, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlBaja
            Catch ex As MySqlException
                Return SqlBaja
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod(True)> _
    Public Function EmpresaEstado(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim SqlEstado As String = "SELECT id_empsa, nom_empsa FROM empresa where estado=1 order by id_empsa"
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand(SqlEstado, conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("id_empsa").ToString() + ". " + dr("nom_empsa").ToString(), dr("id_empsa").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarAgencia(ByVal agencia As String) As List(Of [ClaseAgencia])
        Dim consulta As String = "SELECT fac_agc.nom_agc, fac_agc.dire_agc, fac_agc.tel_agc, fac_agc.contact1_agc, fac_agc.contact2_agc, fac_agc.nemonico, fac_agc.id_destino, fac_agc.id_empsa FROM facturacion.agencia fac_agc, encomiendas.destino en_des where en_des.id_destino='" + agencia + "' and fac_agc.id_agc=en_des.id_destino"

        Dim result As List(Of [ClaseAgencia]) = New List(Of ClaseAgencia)()
        Dim Elemento As New ClaseAgencia
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                reader.Read()

                If reader.HasRows Then
                    Elemento.nombre = reader("nom_agc")
                    Elemento.direccion = reader("dire_agc")
                    Elemento.telefono = reader("tel_agc")
                    Elemento.primerContacto = reader("contact1_agc")
                    Elemento.segundoContacto = reader("contact2_agc")
                    Elemento.nemonico = reader("nemonico")
                    Elemento.destino = reader("id_destino")
                    Elemento.empresa = reader("id_empsa")
                    result.Add(Elemento)
                End If
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseAgencia
        Public nombre As String
        Public direccion As String
        Public telefono As String
        Public primerContacto As String
        Public segundoContacto As String
        Public destino As String
        Public empresa As String
        Public nemonico As String
    End Class
    <WebMethod()> _
    Public Function GuardarAgencias(ByVal nombre As String, ByVal direccion As String, ByVal telefono As String, ByVal primerContacto As String, ByVal segundoContacto As String, ByVal nemonico As String, ByVal ubicacion As String, ByVal empresa As String) As String
        MsgBox(nombre + direccion + telefono + primerContacto + segundoContacto + nemonico + empresa + ubicacion)
        Dim Sqlinserta As String = "insert into facturacion.agencia (nom_agc, dire_agc, tel_agc, contact1_agc, contact2_agc, id_destino, id_empsa, nemonico) values ('" + nombre + "','" + telefono + "', '" + direccion + "', '" + primerContacto + "', '" + segundoContacto + "','" + ubicacion + "', '" + empresa + "','" + nemonico + "')"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod(True)> _
    Public Function Agencia(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_agc,nom_agc from Facturacion.agencia where estado=1 order by nom_agc", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_agc").ToString(), dr("id_agc").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod()> _
    Public Function BajaAgencia(ByVal valor As String) As String
        Dim SqlBaja As String = "update Facturacion.agencia set estado='0' where id_empsa='" & valor & "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlBaja, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlBaja
            Catch ex As MySqlException
                Return SqlBaja
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function ModificarAgencia(ByVal nombre As String, ByVal direccion As String, ByVal telefono As String, ByVal primerContacto As String, ByVal segundoContacto As String, ByVal nemonico As String, ByVal ubicacion As String, ByVal empresa As String, ByVal valor As String) As String
        Dim SqlModifica As String = "update facturacion.agencia set nom_agc='" + nombre + "', dire_agc='" + direccion + "', tel_agc='" + telefono + "', contact1_agc='" + primerContacto + "', contact2_agc='" + segundoContacto + "', nemonico='" + nemonico + "', id_destino='" + ubicacion + "', id_empsa='" + empresa + "' where id_agc='" & valor & "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlModifica, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlModifica
            Catch ex As MySqlException
                Return SqlModifica
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod(True)> _
    Public Function Rutas(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_ruta, nom_ruta from encomiendas.ruta where estado=1 order by id_ruta", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_ruta").ToString(), dr("id_ruta").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarRuta(ByVal ruta As String) As List(Of [ClaseRuta])
        Dim consulta As String = "select nom_ruta, id_salida, id_destino, id_empsa, tipo_ruta, id_agc, prioridad  from encomiendas.ruta where id_ruta='" + ruta + "'"
        Dim result As List(Of [ClaseRuta]) = New List(Of ClaseRuta)()
        Dim Elemento As New ClaseRuta
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                reader.Read()
                If reader.HasRows Then
                    Elemento.nombre = reader("nom_ruta")
                    Elemento.origen = reader("id_salida")
                    Elemento.destino = reader("id_destino")
                    Elemento.empresa = reader("id_empsa")
                    Elemento.tipoRuta = reader("tipo_ruta")
                    Elemento.agencia = reader("id_agc")
                    Elemento.prioridad = reader("prioridad")
                    result.Add(Elemento)
                End If
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseRuta
        Public nombre As String
        Public origen As String
        Public destino As String
        Public empresa As String
        Public tipoRuta As Integer
        Public agencia As String
        Public prioridad As String
    End Class
    <WebMethod()> _
    Public Function GuardarRuta(ByVal nombre As String, ByVal origen As String, ByVal destino As String, ByVal empresa As String, ByVal tipoRuta As String, ByVal agencia As String, ByVal prioridad As String) As String
        Dim Sqlinserta As String = "INSERT INTO encomiendas.ruta (nom_ruta, id_salida, id_destino, id_empsa, tipo_ruta, id_agc, prioridad, estado) VALUES ('" + nombre + "','" + origen + "', '" + destino + "', '" + empresa + "', '" + tipoRuta + "', '" + agencia + "', '" + prioridad + "', '1')"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function ModificarRuta(ByVal nombre As String, ByVal origen As String, ByVal destino As String, ByVal empresa As String, ByVal tipoRuta As String, ByVal agencia As String, ByVal prioridad As String, ByVal valor As String) As String
        Dim SqlModifica As String = "UPDATE ruta SET nom_ruta='" + nombre + "', id_salida='" + origen + "', id_destino='" + destino + "', id_empsa='" + empresa + "', tipo_ruta='" + tipoRuta + "', id_agc='" + agencia + "', prioridad='" + prioridad + "' where id_ruta='" & valor & "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlModifica, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlModifica
            Catch ex As MySqlException
                Return SqlModifica
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BajaRuta(ByVal valor As String) As String
        Try
            Dim SqlBaja As String = "DELETE FROM ruta where id_ruta='" & valor & "'"
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                Dim comando As MySqlCommand
                Try
                    connection.Open()
                    comando = New MySqlCommand(SqlBaja, connection)
                    comando.ExecuteNonQuery()
                    comando.Dispose()
                    Dim ResetAutoIncrement As String = "alter table encomiendas.ruta auto_increment=1"
                    Using connection1 As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                        connection1.Open()
                        comando = New MySqlCommand(ResetAutoIncrement, connection1)
                        comando.ExecuteNonQuery()
                        comando.Dispose()
                    End Using
                    
                    Return SqlBaja
                Catch ex1 As Exception
                    Return SqlBaja
                    MsgBox(ex1.Message)
                Finally
                    connection.Close()
                End Try
            End Using
        Catch ex As MySqlException
            Dim SqlBaja As String = "UPDATE ruta SET estado=0 where id_ruta='" & valor & "'"
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                Dim comando As MySqlCommand
                Try
                    connection.Open()
                    comando = New MySqlCommand(SqlBaja, connection)
                    comando.ExecuteNonQuery()
                    comando.Dispose()
                    Return SqlBaja
                Catch ex2 As Exception
                    Return SqlBaja
                    MsgBox(ex2.Message)
                Finally
                    connection.Close()
                End Try
            End Using
        End Try
    End Function
    <WebMethod()> _
    Public Function BuscarTR(ByVal buscarOrigen As String, ByVal buscarDestino As String) As List(Of [ClaseTR])
        Dim consulta As String = "select id_tramosunion, id_sale, id_llega, kms, tiempo from encomiendas.ruta_tramosunion where id_sale=" + buscarOrigen + " and id_llega=" + buscarDestino + ""
        Dim result As List(Of [ClaseTR]) = New List(Of ClaseTR)()
        Dim Elemento As New ClaseTR
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                If reader.HasRows Then
                    Elemento.id = reader("id_tramosunion")
                    Elemento.origen = reader("id_sale")
                    Elemento.destino = reader("id_llega")
                    Elemento.kms = reader("kms")
                    Elemento.tiempo = reader("tiempo")
                    result.Add(Elemento)
                End If

            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseTR
        Public id As String
        Public origen As String
        Public destino As String
        Public kms As String
        Public tiempo As String
    End Class
    <WebMethod()> _
    Public Function GuardarTR(ByVal origen As String, ByVal destino As String, ByVal kms As String, ByVal tiempo As String) As String
        Dim Sqlinserta As String = "INSERT INTO encomiendas.ruta_tramosunion (id_sale, id_llega, kms, tiempo) VALUES ('" + origen + "','" + destino + "', '" + kms + "', '" + tiempo + "');"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function ModificarTR(ByVal id As String, ByVal origen As String, ByVal destino As String, ByVal kms As String, ByVal tiempo As String) As String
        Dim Sqlmodifica As String = "UPDATE encomiendas.ruta_tramosunion SET id_sale='" + origen + "', id_llega='" + destino + "', kms='" + kms + "', tiempo='" + tiempo + "' WHERE id_tramosunion='" + id + "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlmodifica, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlmodifica
            Catch ex As MySqlException
                Return Sqlmodifica
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BajaTR(ByVal valor As String) As String
        Dim SqlBaja As String = "delete from encomiendas.ruta_tramosunion where id_tramosunion='" + valor + "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlBaja, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Dim alter As String = "alter table encomiendas.ruta_tramosunion auto_increment=1"
                Using connection1 As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                    connection1.Open()
                    comando = New MySqlCommand(alter, connection1)
                    comando.ExecuteNonQuery()
                    comando.Dispose()
                End Using
                Return SqlBaja
            Catch ex As MySqlException
                Return SqlBaja
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarDR(ByVal ruta As String) As List(Of [ClaseDR])
        Dim consulta As String = "select rt.id_ruta, rt.cod_tramo, rt.correlativo,rtu.id_sale,rtu.id_llega,s.nom_destino origen,d.nom_destino destino" +
                                    " from encomiendas.ruta_tramo rt" +
                                    " INNER JOIN encomiendas.ruta_tramosunion rtu ON rt.cod_tramo=rtu.id_tramosunion" +
                                    " INNER JOIN encomiendas.destino s ON rtu.id_sale=s.id_destino" +
                                    " INNER JOIN encomiendas.destino d ON rtu.id_llega=d.id_destino" +
                                    " where id_ruta='" + ruta + "' order by correlativo asc"
        Dim result As List(Of [ClaseDR]) = New List(Of ClaseDR)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read
                    Dim Elemento As New ClaseDR
                    Elemento.ruta = reader("id_ruta")
                    Elemento.tramo = reader("cod_tramo")
                    Elemento.origen = reader("origen")
                    Elemento.destino = reader("destino")
                    Elemento.correlativo = reader("correlativo")
                    result.Add(Elemento)
                    End While
            Catch ex As Exception
                End Try
            End Using
        Return result
    End Function
    Public Class ClaseDR
        Public ruta As String
        Public tramo As String
        Public origen As String
        Public destino As String
        Public correlativo As String
    End Class
    <WebMethod()> _
    Public Function GuardarDR(ByVal ruta As String, ByVal tramo As String, ByVal secuencia As String) As String
        Dim Sqlinserta As String = "insert into encomiendas.ruta_tramo (id_ruta, cod_tramo, correlativo ) Values ('" + ruta + "','" + tramo + "','" + secuencia + "')"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarHorarios() As List(Of [ClaseHorarios])
        Dim consulta As String = "SELECT hor.id_horario, hor.id_ruta, cast((hor.hora) as char(10)) as hora, rut.nom_ruta FROM hora AS hor INNER JOIN ruta AS rut on hor.id_ruta=rut.id_ruta and hor.estado=1 ORDER BY hor.hora ASC"
        Dim result As List(Of [ClaseHorarios]) = New List(Of ClaseHorarios)()

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read
                    Dim Elemento As New ClaseHorarios
                    Elemento.id_horario = reader("id_horario")
                    Elemento.ruta = reader("id_ruta")
                    Elemento.nombre = reader("nom_ruta")
                    Elemento.hora = reader("hora")
                    result.Add(Elemento)
                End While
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseHorarios
        Public id_horario As String
        Public ruta As String
        Public hora As String
        Public nombre As String
    End Class
    <WebMethod()> _
    Public Function GuardarHorarios(ByVal rutas As String, ByVal horas As String) As String
        Dim Sqlinserta As String = "INSERT INTO hora (hora, id_ruta, estado) VALUES ('" + horas + "','" + rutas + "', '1');"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()

                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BajaHorarios(ByVal valor As String) As String
        Dim SqlBaja As String = "update hora set estado=0 where id_horario='" + valor + "'"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlBaja, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Dim alter As String = "alter table hora auto_increment=1"
                Using connection1 As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                    connection1.Open()
                    comando = New MySqlCommand(alter, connection1)
                    comando.ExecuteNonQuery()
                    comando.Dispose()
                End Using
                Return SqlBaja
            Catch ex As MySqlException
                Return SqlBaja
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod(True)> _
    Public Function Departamentos(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT * FROM departamentod order by id_deptoD", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_deptoD").ToString(), dr("id_deptoD").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod()> _
    Public Function GuardarDepartamentos(ByVal departamento As String) As String
        Dim Sqlinserta As String = "insert into departamentod (nom_deptoD) values ('" + departamento + "');"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()

                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarDpto(ByVal depto As String) As List(Of [ClaseDptos])
        Dim consulta As String = "select nom_deptoD from departamentod where id_deptoD=" + depto + ";"
        Dim result As List(Of [ClaseDptos]) = New List(Of ClaseDptos)()
        Dim Elemento As New ClaseDptos
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read
                    Elemento.dpto = reader("nom_deptoD")
                    result.Add(Elemento)
                End While
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseDptos
        Public dpto As String
    End Class
    <WebMethod()> _
    Public Function BajaDpto(ByVal valor As String) As String
        Dim SqlBaja As String = "delete from departamentod where id_deptoD=" + valor + ";"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()
                comando = New MySqlCommand(SqlBaja, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return SqlBaja
            Catch ex As MySqlException
                Return SqlBaja
                MsgBox(ex.Message)

            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod(True)> _
    Public Function Destinos(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT * FROM destino order by id_destino", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_destino").ToString(), dr("id_destino").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod()> _
    Public Function GuardarDestino(ByVal nombre As String, ByVal departamento As String, ByVal municipio As String, ByVal nemonico As String) As String
        Dim Sqlinserta As String = "insert into destino (nom_destino, id_municipio, nemonico, id_dpto) values ('" + nombre + "','" + municipio + "','" + nemonico + "','" + departamento + "');"
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                connection.Open()

                comando = New MySqlCommand(Sqlinserta, connection)
                comando.ExecuteNonQuery()
                comando.Dispose()
                Return Sqlinserta
            Catch ex As MySqlException
                Return Sqlinserta
                MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
    End Function
    <WebMethod()> _
    Public Function BuscarDestino(ByVal valor As String) As List(Of [ClaseDestino])
        Dim consulta As String = "select nom_destino, id_municipio, nemonico, id_dpto from destino where id_destino=" + valor + ""
        Dim result As List(Of [ClaseDestino]) = New List(Of ClaseDestino)()
        Dim Elemento As New ClaseDestino
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read
                    Elemento.nombre = reader("nom_destino")
                    Elemento.municipio = reader("id_municipio")
                    Elemento.nemonico = reader("nemonico")
                    Elemento.dpto = reader("id_dpto")
                    result.Add(Elemento)
                End While
            Catch ex As Exception
            End Try
        End Using
        Return result
    End Function
    Public Class ClaseDestino
        Public nombre As String
        Public municipio As String
        Public nemonico As String
        Public dpto As String
    End Class
End Class