Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WSboletoenruta
    Inherits System.Web.Services.WebService

    <WebMethod(True)> _
    Public Function seleccionarCaja(ByVal idvehiculo As String) As List(Of [Caja])
        Dim Busqueda As String = "select s.id_agenciacaja idcaja,s.correlativoactual numeroactual,a.serie serie " +
        "from Boletos.series s " +
        "INNER JOIN Facturacion.agenciacaja a ON s.id_agenciacaja=a.id_agenciacaja " +
        "where s.id_veh='" & idvehiculo & "';"

        Dim result As List(Of [Caja]) = New List(Of Caja)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                Dim Elemento As New Caja

                If reader.HasRows Then

                    Elemento.idcaja = reader("idcaja")
                    Elemento.serie = reader("serie")
                    Elemento.numeroactual = reader("numeroactual")
                    result.Add(Elemento)
                Else

                    Elemento.idcaja = 0
                    Elemento.serie = "sin serie"
                    Elemento.numeroactual = 0
                    result.Add(Elemento)

                End If

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Caja
                Elemento.idcaja = 0
                Elemento.serie = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function seleccionarDatos(ByVal idhorario As Integer) As List(Of [HorarioEspecifico])
        Dim Busqueda As String = "select h.id_horario idhora,h.id_veh idvehiculo,h.id_emp idpiloto," +
        "p.nom_emp piloto,h.id_asistente idasistente,a.nom_emp asistente,h.idservicio idservicio," +
        "h.id_ruta idruta " +
        "from horarios h " +
        "INNER JOIN empleado p ON h.id_emp=p.id_emp " +
        "INNER JOIN empleado a ON h.id_asistente=a.id_emp " +
        "where h.id_detaHorario = " & idhorario & ""
        Dim result As List(Of [HorarioEspecifico]) = New List(Of HorarioEspecifico)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New HorarioEspecifico

                    Elemento.piloto = reader("piloto")
                    Elemento.idpiloto = reader("idpiloto")
                    Elemento.asistente = reader("asistente")
                    Elemento.idasistente = reader("idasistente")
                    Elemento.idvehiculo = reader("idvehiculo")
                    Elemento.idruta = reader("idruta")
                    Elemento.idservicio = IIf(reader("idservicio") IsNot DBNull.Value, reader("idservicio"), 1)
                    Elemento.idhora = reader("idhora")
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New HorarioEspecifico
                Elemento.idpiloto = 0
                Elemento.piloto = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaHoras(ByVal tipo As Integer, ByVal fecha As String) As List(Of [Horario])
        Dim Busqueda As String = "call Encomiendas.sp_retornahorarios_boletosenruta(" & tipo & ",'" & fecha & "')"
        Dim result As List(Of [Horario]) = New List(Of Horario)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Horario

                    Elemento.idhorario = reader("idhorario")
                    Elemento.hora = reader("hora").ToString
                    Elemento.ruta = reader("ruta").ToString
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Horario
                Elemento.idhorario = 0
                Elemento.hora = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function guardaBoletoenruta(ByVal Tarjeta As List(Of Tarjeta), ByVal TarjetaDetalle As List(Of TarjetaDetalle)) As String

        Dim SQLInserta, SQLInsertadetalle As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            Try
                '----------------------INSERT TARJETA----------------------'
                SQLInserta = "insert into Boletos.tarjeta (fechacreacion,fechaviaje,id_detaHorario,id_veh,idpiloto,idasistente,boleta," +
                "valorentregado,valorcarga,valorexcursion,valorviaticos,montoboletos," +
                "total,id_usr,estatus) values " +
                "(now(),'" & Tarjeta(0).fechaviaje & "'," & Tarjeta(0).id_detaHorario & ",'" & Tarjeta(0).id_veh & "'," & Tarjeta(0).idpiloto & "," +
                "" & Tarjeta(0).idasistente & "," & Tarjeta(0).boleta & "," & Tarjeta(0).valorentregado & "," +
                "" & Tarjeta(0).valorcarga & "," & Tarjeta(0).valorexcursion & "," & Tarjeta(0).valorviaticos & "," +
                "" & Tarjeta(0).montoboletos & "," & Tarjeta(0).total & ",'" & Tarjeta(0).id_usr & "',1 );"

                comando.CommandText = SQLInserta
                comando.ExecuteNonQuery()

                '-------------------RETORNO DEL AUTONUMERICO---------------------'
                comando.CommandText = "SELECT @@IDENTITY"
                Dim Correlativo As Integer = comando.ExecuteScalar

                Dim Contador As Integer = TarjetaDetalle.Count
                For i = 0 To Contador - 1
                    '---------------------------INSERT TARJETA DETALLE----------------------'
                    SQLInsertadetalle = "insert into Boletos.tarjetadetalle " +
                            "(idtarjeta,serie,idboleto,valor) " +
                            "values (" & Correlativo & "," +
                            "'" & TarjetaDetalle(i).serie & "'," +
                            "" & TarjetaDetalle(i).idboleto & "," +
                            "" & TarjetaDetalle(i).valor & ")"

                    comando.CommandText = SQLInsertadetalle
                    comando.ExecuteNonQuery()
                    '--------------------INSERT BOLETOS------------------------'
                    SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                            "destino,nitcliente,seguro," +
                            "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                            "agencia,usuario,estadofacturacion," +
                            "estadoboleto,saldo,total,horario,ruta," +
                            "secuenciaorigen,secuenciadestino,idservicio,idtipoboleto) " +
                            "values " +
                            "(now(),'" & Tarjeta(0).fechaviaje & "',0," +
                            "0,'C/F'," +
                            "3,2," +
                            "0,'ocupado'," & Tarjeta(0).idempresa & "," +
                            "" & Tarjeta(0).idagencia & ",'" & Tarjeta(0).id_usr & "','FACTURADO'," +
                            "'VALIDO',0," & TarjetaDetalle(i).valor & "," +
                            "" & Tarjeta(0).idhora & "," & Tarjeta(0).idruta & "," +
                            "0,0," & Tarjeta(0).idservicio & ",2)"

                    comando.CommandText = SQLInserta
                    comando.ExecuteNonQuery()

                    '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------
                    SQLInserta = "insert into Facturacion.factura " +
                    "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                    "nit_clt,nom_clt,id_agc," +
                    "id_usr,id_agenciacaja,kae,face," +
                    "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                    "values " +
                    "(" & TarjetaDetalle(i).idboleto & ",'" & TarjetaDetalle(i).serie & "',curdate(),curtime()," +
                    "" & Val(TarjetaDetalle(i).valor) & "," +
                    "'C/F','Consumidor Final'," & Tarjeta(0).idagencia & "," +
                    "'" & Tarjeta(0).id_usr & "'," & Tarjeta(0).idcaja & ",'N/A','N/A'," +
                    "0," & Val(TarjetaDetalle(i).valor) & "," +
                    "0,0,0)"

                    comando.CommandText = SQLInserta
                    comando.ExecuteNonQuery()

                    SQLInserta = "insert into Facturacion.factura_detalle " +
                    "(num_fac,serie_fac,item,id_doc,valor) " +
                    "values (" & TarjetaDetalle(i).idboleto & ",'" & TarjetaDetalle(i).serie & "',3," +
                    "" & TarjetaDetalle(i).idboleto & "," & Val(TarjetaDetalle(i).valor) & ")"

                    comando.CommandText = SQLInserta
                    comando.ExecuteNonQuery()

                Next
                '-------ACTUALIZACION DE LAS SERIES DE LOS BOLETOS, PARA QUE INCREMENTE EL AUTONUMERICO--------'
                SQLInserta = "update Boletos.series set correlativoactual=" & TarjetaDetalle(Contador - 1).idboleto & " " +
                "where id_agenciacaja='" & Tarjeta(0).idcaja & "'"

                comando.CommandText = SQLInserta
                comando.ExecuteNonQuery()

                transaccion.Commit()
                SQLInserta = "Tarjeta creada correctamente #: " + Correlativo.ToString

                Return SQLInserta

            Catch ex As MySqlException

                SQLInserta = "ERROR: " + ex.Message
                Return SQLInserta
            Finally
                connection.Close()
                comando.Dispose()
                transaccion.Dispose()
            End Try
        End Using
    End Function

    <WebMethod(True)> _
    Public Function cargaVehiculos() As List(Of [Vehiculo])
        Dim Busqueda As String = "select id_veh from Encomiendas.vehiculo order by id_veh;"
        Dim result As List(Of [Vehiculo]) = New List(Of Vehiculo)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Vehiculo

                    Elemento.idvehiculo = reader("id_veh")
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Vehiculo

                Elemento.idvehiculo = ex.Message

                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaHoras1() As List(Of [Hora])
        Dim Busqueda As String = "select id_horario,hora from Encomiendas.hora order by hora;"
        Dim result As List(Of [Hora]) = New List(Of Hora)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Hora

                    Elemento.idhora = reader("id_horario")
                    Elemento.hora = reader("hora").ToString
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Hora

                Elemento.idhora = 0
                Elemento.hora = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaAgencias(ByVal usuario As String) As List(Of [Agencia])
        Dim Busqueda As String = "select id_agc,nom_agc from Facturacion.agencia order by nom_agc"
        Dim result As List(Of [Agencia]) = New List(Of Agencia)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Agencia

                    Elemento.idagencia = reader("id_agc")
                    Elemento.nombre = reader("nom_agc")
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Agencia

                Elemento.idagencia = 0
                Elemento.nombre = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaPilotos() As List(Of [Empleado])
        Dim Busqueda As String = "select id_emp,nom_emp from Encomiendas.empleado where id_pso=1 order by nom_emp"
        Dim result As List(Of [Empleado]) = New List(Of Empleado)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Empleado

                    Elemento.idempleado = reader("id_emp")
                    Elemento.nombre = reader("nom_emp")
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Empleado

                Elemento.idempleado = 0
                Elemento.nombre = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod(True)> _
    Public Function cargaAsistentes() As List(Of [Empleado])
        Dim Busqueda As String = "select id_emp,nom_emp from Encomiendas.empleado where id_pso=2 order by nom_emp"
        Dim result As List(Of [Empleado]) = New List(Of Empleado)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New Empleado

                    Elemento.idempleado = reader("id_emp")
                    Elemento.nombre = reader("nom_emp")
                    result.Add(Elemento)
                End While

                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New Empleado

                Elemento.idempleado = 0
                Elemento.nombre = ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    Public Class Empleado
        Public idempleado As Integer
        Public nombre As String
    End Class
    Public Class Agencia
        Public idagencia As Integer
        Public nombre As String
    End Class
    Public Class Hora
        Public idhora As Integer
        Public hora As String
    End Class
    Public Class Vehiculo
        Public idvehiculo As String
    End Class
    Public Class TarjetaDetalle
        Public idtarjetadetalle As Integer
        Public serie As String
        Public idboleto As Integer
        Public valor As Double
    End Class
    Public Class HorarioEspecifico
        Public idpiloto As Integer
        Public piloto As String
        Public idasistente As Integer
        Public asistente As String
        Public idvehiculo As String
        Public idhora As Integer
        Public idruta As Integer
        Public idservicio As Integer
    End Class
    Public Class Caja
        Public idcaja As Integer
        Public serie As String
        Public numeroactual As Integer
    End Class
    Public Class Tarjeta
        Public idtarjeta As Integer
        Public fechacreacion As String
        Public fechaviaje As String
        Public id_detaHorario As Integer
        Public id_veh As String
        Public idpiloto As Integer
        Public idasistente As Integer
        Public boleta As Double
        Public valorentregado As Double
        Public valorcarga As Double
        Public valorexcursion As Double
        Public valorviaticos As Double
        Public montoboletos As Double
        Public total As Double
        Public id_usr As String
        Public estatus As Integer
        Public idcaja As Integer
        Public idagencia As Integer
        Public idempresa As Integer
        Public idhora As Integer
        Public idruta As Integer
        Public idservicio As Integer
    End Class
    Public Class Factura
        Public correlativo As Integer
        Public num_fac As Integer
        Public serie_fac As String
        Public fecha_fac As String
        Public hora_fac As String
        Public valor_fac As Double
        Public nit_clt As String
        Public nom_clt As String
        Public id_agc As Integer
        Public id_usr As String
        Public id_agenciacaja As Integer
        Public kae As String
        Public face As String
        Public recargo As Double
        Public efectivo As Double
        Public tarjetacredito As Double
        Public cheque As Double
        Public referencia As String
        Public nimpresion As Integer
    End Class
    Public Class FacturaDetalle
        Public correlativo As Integer
        Public num_fac As Integer
        Public serie_fac As String
        Public id_doc As Integer
        Public valor As Double
    End Class
    Public Class Series
        Public correlativo As Integer
        Public serie As String
        Public inicio As Integer
        Public final As Integer
        Public id_veh As String
        Public correlativoactual As Integer
        Public estatus As Integer
        Public idempresa As Integer
    End Class
    Public Class Horario
        Public idhorario As Integer
        Public hora As String
        Public ruta As String
    End Class
End Class