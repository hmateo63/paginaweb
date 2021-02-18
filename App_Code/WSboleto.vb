Imports System.Web.Script.Services
Imports AjaxControlToolkit
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Diagnostics
Imports System.Data

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WSboleto

    Inherits System.Web.Services.WebService
    Dim CodigoPagina As Integer = 1

    <WebMethod(True)> _
    Public Function cambiarHorarioBoleto(ByVal idboleto As Integer,
                                         ByVal fecha As String, ByVal idorigen As Integer, ByVal iddestino As Integer,
                                         ByVal idhora As Integer, ByVal idruta As Integer, ByVal idempresa As Integer,
                                         ByVal idservicio As Integer, ByVal numerobutaca As Integer, ByVal observaciones As String) As String

        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            '---------EJECUTAMOS PROCEDIMIENTO ALMACENADO QUE RETORNA DATOS NECESARIOS PARA INSERTS DE LA TRANSACCION
            'parametro _usuario y retorna caja, resolucion, serie, fecharesolucion, codigoagencia, Empresa, dispositivo nombreagencia, direccionagencia
            'Dim usuario As String = 
            Dim ConsultaDatos As String = "call Facturacion.sp_retorna_datos_usuario('" & Session("id_usr") & "')"
            Dim Cmd As New MySqlCommand(ConsultaDatos, connection)
            Dim Lector As MySqlDataReader = Cmd.ExecuteReader()



            Lector.Read()
            Dim Serie As String = ""
            Try
                Lector.Read()
                If Lector.HasRows Then
                    Dim Resolucion As String = Lector("resolucion")
                    Dim FechaResolucion As String = Lector("fecharesolucion")
                    'Dim Serie As String = Lector("serie")
                    Dim CodigoAgencia As String = Lector("codigoagencia")
                    Dim Agencia As String = Lector("agencia")
                    Dim DireccionAgencia As String = Lector("direccionagencia")
                    Dim Empresa As String = Lector("empresa")
                    Dim Caja As String = Lector("caja")
                    Dim Dispositivo As String = Lector("dispositivo")
                    Lector.Close()
                    'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
                    '/*_usuario,_idboleto,_fecha varchar(12),
                    '_idorigen,_iddestino,_idhora,_idruta,
                    '_idempresa,_idservicio,_numerobutaca,_idagencia*/
                    Dim ConsultaSeries As String = "select boletos.fn_cambiar_horario_boleto " +
                    "('" & Session("id_usr") & "'," & idboleto & ",'" & fecha & "'," +
                    "" & idorigen & "," & iddestino & "," & idhora & "," +
                    "" & idruta & "," & idempresa & "," & idservicio & "," +
                    "" & numerobutaca & "," & CodigoAgencia & ",'" & observaciones & "') as resultado"

                    Dim CmdSeries As New MySqlCommand(ConsultaSeries, connection)
                    Dim LectorSeries As MySqlDataReader = CmdSeries.ExecuteReader()

                    LectorSeries.Read()
                    Dim Resultado As String
                    If LectorSeries.HasRows Then
                        Resultado = LectorSeries("resultado")

                        LectorSeries.Close()
                        transaccion.Commit()
                        '******************************CREACION DEL ARCHIVO POS***********************************'
                        '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
                        Return Resultado

                    Else
                        LectorSeries.Close()
                        Return "Boleto invalido"
                    End If
                Else
                    Lector.Close()
                    Return "ERROR. No posee caja de cobro asignada"
                End If
            Catch ex As MySqlException

                transaccion.Rollback()
                Return "ERROR." + ex.Message

            Finally
                connection.Close()
                comando.Dispose()
                transaccion.Dispose()
            End Try
        End Using
    End Function

    <WebMethod()> _
    Public Function estadoboleto(ByVal idboleto As Integer, ByVal usuario As String) As List(Of ClaseEstadoBoleto)
        Dim consulta As String = "select idestadoboleto from Boletos.boleto where id_boleto=" & idboleto & ";"
        Dim result As List(Of [ClaseEstadoBoleto]) = New List(Of ClaseEstadoBoleto)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                Dim Elemento As New ClaseEstadoBoleto

                If reader.HasRows Then
                    Elemento.idestatus = reader("idestadoboleto")
                    reader.Close()
                    If Elemento.idestatus = 1 Then
                        Elemento.idestatus = 2
                        Elemento.estatus = "Pasajero aborda bus"
                    ElseIf Elemento.idestatus = 2 Then
                        Elemento.idestatus = 3
                        Elemento.estatus = "Pasajero abandona bus"
                    ElseIf Elemento.idestatus = 3 Then
                        Elemento.idestatus = 2
                        Elemento.estatus = "Pasajero aborda bus"

                    End If
                    Dim SQLinserta As String = "update Boletos.boleto set idestadoboleto =" & Elemento.idestatus & ""

                    Dim newcmd As New MySqlCommand(SQLinserta, connection)
                    newcmd.ExecuteNonQuery()

                    Elemento.mensaje = "Correcto"
                    cmd.Dispose()
                    newcmd.Dispose()
                Else
                    Elemento.mensaje = "ERROR: No se ha establecido el estatus"
                    reader.Close()
                End If
                result.Add(Elemento)
            Catch ex As MySqlException
                Dim Elemento As New ClaseEstadoBoleto
                Elemento.mensaje = "ERROR: " + ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    Public Class ClaseEstadoBoleto
        Public idboleto As Integer
        Public mensaje As String
        Public estatus As String
        Public idestatus As Integer
    End Class

    <WebMethod()> _
    Public Function permisoboletos(ByVal usuario As String, ByVal accion As Integer) As Integer
        Return DatosSql.permiso(usuario, accion, CodigoPagina)
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                                   PROCEDIMIENTO QUE CONVIERTE UNA RESERVACION EN BOLETO                                     *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    Public Class DetalleOrden
        Public fecha As String
        Public hora As String
        Public cliente As String
        Public origen As String
        Public destino As String
        Public asientos As String
    End Class

    <WebMethod()> _
    Public Function GuardaBoletoReservado(ByVal Asientos As String, ByVal Datos As String) As String
        Return Nothing
    End Function

    <WebMethod()> _
    Public Function DeterminaAsientosNuevos(ByVal Fecha As String, _
                                            ByVal Hora As String, _
                                            ByVal Ruta As String, _
                                            ByVal Empresa As String, _
                                            ByVal Origen As String, _
                                            ByVal Destino As String, _
                                            ByVal Asientos As String, ByVal Servicio As String) As List(Of [ClaseVehiculo])
        Dim Consulta As String = "call sp_consulta_butacas(" & Origen & "," & Destino & "," & Ruta & "," & Hora & ",'" & Fecha & "'," & Empresa & "," & Servicio & ")"
        'MsgBox("DeterminaAsientosNuevos")
        Dim result As List(Of [ClaseVehiculo]) = New List(Of ClaseVehiculo)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseVehiculo
                    Elemento.asientonumero = reader("numerobutaca")
                    Elemento.asientoestado = reader("estadobutaca")
                    Elemento.cliente = reader("nom_clt")
                    Elemento.viaje = reader("viaje")
                    Elemento.total = reader("total")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                                   PROCEDIMIENTO QUE ALMACENA UNA RESERVACION DE BOLETOS                                     *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function ReservaBoleto(ByVal Asientos As String, ByVal Datos As String) As String
        Dim Arreglo() As String = Datos.Split("|")
        Dim ArregloButaca() As String = Asientos.Split("|")
        Arreglo(5) = Session("id_usr")
        Dim Origen As String = Arreglo(2).ToString
        Dim Destino As String = Arreglo(3).ToString
        Dim SQLInserta, SQLInsertadetalle As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            comando.CommandText = "select fn_retornar_secuencia(" & Origen & "," & Arreglo(8).ToString & ",1)"
            Dim StrSecuenciaOrigen As Integer = comando.ExecuteScalar
            comando.CommandText = "select fn_retornar_secuencia(" & Destino & "," & Arreglo(8).ToString & ",2)"
            Dim StrSecuenciaDestino As Integer = comando.ExecuteScalar
            Try
                SQLInserta = "insert into Boletos.reservacion (fechacreacion," +
                "fechaviaje,id_hora,origen,destino," +
                "anombre,usuario,empresa,agencia,estadoreservacion," +
                "ruta,secuenciaorigen,secuenciadestino,idservicio) " +
                "values (now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                "" & Origen & "," & Destino & "," +
                "'" & Arreglo(4).ToString & "'," +
                "'" & Arreglo(5).ToString & "'," & Arreglo(6).ToString & "," +
                "" & Session("id_agc") & ",'VALIDA'," & Arreglo(8).ToString & "," +
                "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(10).ToString & ")"
                comando.CommandText = SQLInserta
                comando.ExecuteNonQuery()
                comando.CommandText = "SELECT @@IDENTITY"
                Dim Correlativo As Integer = comando.ExecuteScalar
                For i = 0 To ArregloButaca.Length - 1
                    SQLInsertadetalle = "insert into Boletos.reservaciondetalle " +
                            "(id_reservacion,numerobutaca,estadobutaca,total) " +
                            "values (" & Correlativo & "," +
                            "" & ArregloButaca(i) & ",'reservado'," & Arreglo(9).ToString & ")"
                    comando.CommandText = SQLInsertadetalle
                    comando.ExecuteNonQuery()
                Next
                transaccion.Commit()
                SQLInserta = "Reservacion creada correctamente #: " + Correlativo.ToString
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
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*              PROCEDIMIENTO QUE RETORNA LAS BUTACAS OCUPADAS Y RESERVADAS, PROXIMAMANTE APARTADAS EN TIEMPO REAL             *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function DeterminaCantidadAsientos(ByVal Fecha As String, ByVal Hora As String, ByVal Ruta As String, ByVal Empresa As String, ByVal Origen As String, ByVal Destino As String) As List(Of [ClassActualizar])
        Dim Consulta As String = "select fn_consulta_butacas_cantidad(" & Origen & "," & Destino & "," & Ruta & "," & Hora & ",'" & Fecha & "'," & Empresa & ") cantidad"
        Dim result As List(Of [ClassActualizar]) = New List(Of ClassActualizar)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                If reader.Read() Then
                    Dim Elemento As New ClassActualizar
                    Elemento.cantidad = reader("cantidad")
                    result.Add(Elemento)
                Else
                    Dim Elemento As New ClassActualizar
                    Elemento.cantidad = -1
                    result.Add(Elemento)
                End If
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                Dim Elemento As New ClassActualizar
                Elemento.cantidad = -1
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function DeterminaAsientos(ByVal Fecha As String, ByVal Hora As String, ByVal Ruta As String, ByVal Empresa As String, ByVal Origen As String, ByVal Destino As String, ByVal Servicio As String) As List(Of [ClaseVehiculo])
        'MsgBox("DeterminaAsientos")
        Dim Consulta As String = "call sp_consulta_butacas(" & Origen & "," & Destino & "," & Ruta & "," & Hora & ",'" & Fecha & "'," & Empresa & "," & Servicio & ")"
        Dim result As List(Of [ClaseVehiculo]) = New List(Of ClaseVehiculo)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    If Not IsDBNull(reader("numerobutaca")) Then
                        Dim Elemento As New ClaseVehiculo
                        Elemento.asientonumero = IIf(reader("numerobutaca") IsNot DBNull.Value, reader("numerobutaca"), 0)
                        Elemento.asientoestado = reader("estadobutaca")
                        Elemento.cliente = IIf(reader("nom_clt") IsNot DBNull.Value, reader("nom_clt"), "")
                        Elemento.viaje = reader("viaje")
                        Elemento.total = reader("total")
                        result.Add(Elemento)
                    End If
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    'FACTURACION TEMPORAL
    Public Function FacturaElectronicaTemporal() As String
        Dim FacturaAleatoria As String = "FACE63FA110" + PadL(NumeroAleatorioFactura().ToString, 9, "0")
        Dim Cae As String = "7e1c800bb9bb1145a4c5ebc1ea147139bb7e14ae0f899c24384b41d9ef6d29eb"

        Return "CORRECTO:|" + FacturaAleatoria.ToString + "|" + Cae

    End Function
    'OTRA FACTURACION TEMPORAL
    Public Function FacturaElectronicaTemporalLocal(ByVal Serie As String) As String
        Try
            Dim TablaFactura As DataTable = DatosSql.cargar_datatable("select Facturacion.fn_nuevo_correlativo('" & Serie & "');")

            Dim NumeroFactura As Integer = Val(IIf(TablaFactura.Rows(0).Item(0) IsNot DBNull.Value, TablaFactura.Rows(0).Item(0), 0)) + 1

            Dim FacturaAleatoria As String = "FACE63TE12" + PadL(NumeroFactura.ToString, 10, "0")
            Dim Cae As String = "PENDIENTE"

            Return "CORRECTO:|" + FacturaAleatoria.ToString + "|" + Cae
        Catch ex As Exception
            Return "ERROR:|" + ex.Message
        End Try

    End Function

    Public Function NumeroAleatorioFactura() As Integer
        Dim LimiteMinimo As Integer = 1
        Dim LimiteMaximo As Integer = 9999999
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return Aleatorio
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                            PROCEDIMIENTO QUE GUARDA LOS DATOS DE UNO O VARIOS BOLETOS SOLO DE DIA                           *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function GuardaBoleto(ByVal Asientos As String, ByVal Datos As String, ByVal DatosFactura As String, ByVal Recargos As String) As String
        Dim CorrelativoFactura As Long = 0
        Dim Arreglo() As String = Datos.Split("|")
        Dim ArregloButaca() As String = Asientos.Split("|")
        Dim descuento As Double
        descuento = Arreglo(21) / ArregloButaca.Length
        Arreglo(18) = Arreglo(18) - descuento
        Arreglo(8) = Session("id_usr")
        Dim ArregloFactura() As String = DatosFactura.Split("|")

        'Dim ArregloFacturaElectronica() As String
        Dim StrFechaViaje As String = ""
        Dim Origen As String = Arreglo(12).ToString
        Dim Destino As String = Arreglo(13).ToString
        Dim OrigenNemonico As String = ""
        Dim DestinoNemonico As String = ""
        Dim StrFecha As String = ""
        Dim NBoleto As Long
        Dim SQLInserta As String
        Dim ValorBoleto As Double = 0
        Dim Seguro As Double = 0
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            '---------EJECUTAMOS PROCEDIMIENTO ALMACENADO QUE RETORNA DATOS NECESARIOS PARA INSERTS DE LA TRANSACCION
            'parametro _usuario y retorna caja, resolucion, serie, fecharesolucion, codigoagencia, Empresa, dispositivo nombreagencia, direccionagencia
            Dim ConsultaDatos As String = "call Facturacion.sp_retorna_datos_usuario('" & Session("id_usr") & "')"
            Dim Cmd As New MySqlCommand(ConsultaDatos, connection)
            Dim Lector As MySqlDataReader = Cmd.ExecuteReader()



            Lector.Read()
            Dim Serie As String = ""
            Try
                Lector.Read()
                If Lector.HasRows Then
                    Dim Resolucion As String = Lector("resolucion")
                    Dim FechaResolucion As String = Lector("fecharesolucion")
                    'Dim Serie As String = Lector("serie")
                    Dim CodigoAgencia As String = Lector("codigoagencia")
                    Dim Agencia As String = Lector("agencia")
                    Dim DireccionAgencia As String = Lector("direccionagencia")
                    Dim Empresa As String = Lector("empresa")
                    Dim Caja As String = Lector("caja")
                    Dim Dispositivo As String = Lector("dispositivo")
                    Lector.Close()
                    'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
                    Dim ConsultaSeries As String = "select serie " +
                    "from Facturacion.agenciaseries " +
                    "where idagencia=" & CodigoAgencia & " and idorigen=" & Arreglo(1) & " and iddestino=" & Arreglo(2) & " and estado=1;"

                    Dim CmdSeries As New MySqlCommand(ConsultaSeries, connection)
                    Dim LectorSeries As MySqlDataReader = CmdSeries.ExecuteReader()

                    LectorSeries.Read()
                    If LectorSeries.HasRows Then

                        Serie = LectorSeries("serie")
                        'CorrelativoFactura = LectorSeries("correlativo")
                        LectorSeries.Close()
                        '---------------EJECUTAMOS LA FUNCION QUE RETORNA ALGUNOS DATOS NECESARIOS PARA LA IMPRESION---------
                        'enviar origen,fecha,destino,ruta retorna nemonicoorigen,nemonico destino,fecha_actual con hora, fecha de viaje en formato personalizado,secuencia_origen,secuencia_destino,fecha formato factura electronica
                        Dim StrEjecuta As String = "select fn_retorna_datos_impresion " +
                        "(" & Arreglo(1).ToString & ",'" & Arreglo(0).ToString & "'," +
                        "" & Arreglo(2).ToString & "," & Arreglo(17).ToString & ")"
                        comando.CommandText = StrEjecuta

                        Dim ConsultaDatosEn As String = comando.ExecuteScalar.ToString

                        Dim ArregloDatosEn() As String = ConsultaDatosEn.Split("|")

                        Seguro = ArregloDatosEn(7)
                        Dim NemonicoOrigen As String = ArregloDatosEn(0)
                        Dim NemonicoDestino As String = ArregloDatosEn(1)
                        StrFecha = ArregloDatosEn(2)

                        Dim mifecha As Array
                        mifecha = Split(StrFecha, "-")
                        Dim mes As String = ""
                        Select Case mifecha(1)
                            Case mifecha(1) = 1 : mes = "ENE"
                            Case mifecha(1) = 2 : mes = "FEB"
                            Case mifecha(1) = 3 : mes = "MAR"
                            Case mifecha(1) = 4 : mes = "ABR"
                            Case mifecha(1) = 5 : mes = "MAY"
                            Case mifecha(1) = 6 : mes = "JUN"
                            Case mifecha(1) = 7 : mes = "JUL"
                            Case mifecha(1) = 8 : mes = "AGO"
                            Case mifecha(1) = 9 : mes = "SEP"
                            Case mifecha(1) = 10 : mes = "OCT"
                            Case mifecha(1) = 11 : mes = "NOV"
                            Case mifecha(1) = 12 : mes = "DIC"
                        End Select
                        StrFecha = mifecha(0) + "/" + mes + "/" + mifecha(2)
                        mifecha = Split(StrFecha, " ")
                        StrFecha = mifecha(0)
                        StrFechaViaje = ArregloDatosEn(3)
                        Dim StrSecuenciaOrigen As String = ArregloDatosEn(4)
                        Dim StrSecuenciaDestino As String = ArregloDatosEn(5)
                        Dim StrFechaFactura As String = ArregloDatosEn(6)
                        'Dim NumeroFacTemporal As Integer = NumeroAleatorio()
                        Dim NitCliente As String = ""
                        Dim NombreCliente As String = ""
                        Dim DireccionCliente As String = ""
                        Dim NitClienteImpresion As String = ""
                        Dim NombreClienteImpresion As String = ""
                        Dim DireccionClienteImpresion As String = ""
                        If Trim(Arreglo(3).ToString) = "N/A" Then
                            NitCliente = "C/F"
                            NombreCliente = "CONSUMIDOR FINAL"
                            DireccionCliente = "N/A"
                            NitClienteImpresion = "___________________________"
                            NombreClienteImpresion = "___________________________"
                            DireccionClienteImpresion = "___________________________"
                        ElseIf Trim(Arreglo(3).ToString) = "C/F" Then
                            NitCliente = "C/F"
                            NombreCliente = "CONSUMIDOR FINAL"
                            DireccionCliente = "N/A"
                            NitClienteImpresion = "___________________________"
                            NombreClienteImpresion = "___________________________"
                            DireccionClienteImpresion = "___________________________"
                        Else
                            NitCliente = Arreglo(3).ToString
                            NombreCliente = Arreglo(15).ToString
                            DireccionCliente = Arreglo(20).ToString
                            NitClienteImpresion = Arreglo(3).ToString
                            NombreClienteImpresion = Arreglo(15).ToString
                            DireccionClienteImpresion = Arreglo(20).ToString
                        End If
                        '-----------------INSERT PARA LOS CLIENTES, POR SI NO EXISTE--------------------'
                        If NitCliente = "C/F" Or NitCliente = "N/A" Then
                        Else
                            Dim SQLInsertacliente As String = "call Facturacion.sp_guarda_cliente" +
                            "('" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "'," & CodigoAgencia & "," & Empresa & ")"
                            comando.CommandText = SQLInsertacliente
                            comando.ExecuteNonQuery()
                        End If

                        'MsgBox(CorrelativoFactura)

                        '-----------------------CONSULTAMOS LA DESCRIPCION DEL TIPO DE SERVICIO SELECCIONADO-------------------
                        Dim ConsultaServicio As String = "select descripcion from Boletos.servicio where idservicio=" & Arreglo(19) & " and id_empsa=" & Empresa & ""
                        comando.CommandText = ConsultaServicio
                        Dim Servicio As String = comando.ExecuteScalar

                        '------------------------CONSULTAMOS EL BUS ASIGNADO AL HORARIO---------------------------------'
                        'Dim ConsultaBus As String = "select if(curdate()='" & Arreglo(0).ToString & "'," +
                        '"(select id_veh from Encomiendas.horarios where " +
                        '"fecha_detaHorario=curdate() and id_empsa=" & Empresa & " and id_ruta=" & Arreglo(17) & " limit 1),'---')"
                        Dim ConsultaBus As String = "select encomiendas.fn_determina_vehiculo_turno('" & Arreglo(0).ToString & "'," & Arreglo(11) & "," & Arreglo(17) & "," & Empresa & ")"
                        '                                                                              fecha,                  horario,    ruta,           empresa
                        comando.CommandText = ConsultaBus
                        'Elemento.descripcion = IIf(reader("descripcion") IsNot DBNull.Value, reader("descripcion"), "No se pudo cargar")

                        Dim Bus As String = IIf(comando.ExecuteScalar IsNot DBNull.Value, comando.ExecuteScalar, "")
                        '---------------------LECTURA DEL ARREGLO QUE CONTIENE LAS BUTACAS QUE SE ESTAN CREANDO--------------'
                        Dim CadenaBoletos As String = ""
                        Dim CadenaFacturas As String = ""

                        Dim StrNumeroCorrelativo As String = "select facturacion.fn_nuevo_correlativo " +
                        "(" & CodigoAgencia & "," & Arreglo(1) & "," & Arreglo(2) & ");"
                        Dim StrActualizaCorrelativo As String = ""

                        Dim NumeroFactura As Long = 0

                        


                        For i = 0 To ArregloButaca.Length - 1
                            '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

                            StrActualizaCorrelativo = "select idagenciaseries " +
                            "from facturacion.agenciaseries " +
                            "where idorigen=" & Arreglo(1) & " and iddestino=" & Arreglo(2) & " And " +
                            "idagencia=" & CodigoAgencia & " and estado=1 order by prioridad LIMIT 1"

                            comando.CommandText = StrActualizaCorrelativo
                            Dim correlativoserie As Integer = comando.ExecuteScalar

                            comando.CommandText = StrNumeroCorrelativo
                            NumeroFactura = comando.ExecuteScalar

                            If NumeroFactura = -1 Then
                                transaccion.Rollback()

                                Return "ERROR: El correlativo de facturas para esa serie no es suficiente"
                            Else
                                NumeroFactura = NumeroFactura + 1

                                '--------------------INSERCION A LA TABLA BOLETO---------------------------------'

                                If Recargos <> "0" Then
                                    Dim recargo As Array
                                    recargo = Split(Recargos, ",")
                                    Dim credito As Double
                                    credito = Val(Arreglo(18)) - Val(recargo(0))
                                    Dim recargoBoleto As Double = 0
                                    If recargo(1) > recargo(0) Then
                                        recargoBoleto = Val(recargo(1)) - Val(recargo(0))
                                    End If

                                    '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------
                                    Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                                    "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                                    "nit_clt,nom_clt,id_agc," +
                                    "id_usr,id_agenciacaja,kae,face," +
                                    "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                                    "values " +
                                    "(" & NumeroFactura & ",'" & Serie & "',curdate(),curtime()," & Val(recargo(1)) & "," +
                                    "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                                    "'" & Session("id_usr") & "'," & Caja & ",'pendiente','pendiente'," +
                                    "" & Val(ArregloFactura(1)) & "," & Val(recargo(1)) & "," +
                                    "" & Val(ArregloFactura(3)) & "," & Val(ArregloFactura(4)) & ",1)" 'FALTA INSERTAR DATO DE EMPRESA EN ESTA TABLA
                                    comando.CommandText = StrInsertaFacEnca
                                    comando.ExecuteNonQuery()

                                    '" & Arreglo(4).ToString & " SEGURO
                                    SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                                   "destino,nitcliente,seguro," +
                                   "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                                   "agencia,usuario,estadofacturacion," +
                                   "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
                                   "secuenciaorigen,secuenciadestino,idservicio,idtipoboleto,recargo,credito,boletocredito) " +
                                   "values " +
                                   "(now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                                   "" & Arreglo(2).ToString & ",'" & NitCliente & "'," +
                                   "" & Seguro & ",2," +
                                   "" & ArregloButaca(i) & ",'ocupado'," & Arreglo(6).ToString & "," +
                                   "" & CodigoAgencia & ",'" & Arreglo(8).ToString & "','FACTURADO'," +
                                   "'VALIDO',0," & Val(recargo(1)) & "," +
                                   "" & descuento & ",'" & Arreglo(22) & "'," & Arreglo(11) & "," & Arreglo(17) & "," +
                                   "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(19) & ",1," & recargoBoleto & "," & credito & "," & recargo(2) & ")"
                                    Dim SqlUpdateBoleto As String
                                    SqlUpdateBoleto = "update Boletos.boleto set numerobutaca=null, estadoboleto='INVALIDO' where id_boleto=" & Arreglo(23) & ""
                                    comando.CommandText = SqlUpdateBoleto
                                    comando.ExecuteNonQuery()
                                    ValorBoleto = Val(recargo(1))

                                Else
                                    '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------
                                    Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                                    "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                                    "nit_clt,nom_clt,id_agc," +
                                    "id_usr,id_agenciacaja,kae,face," +
                                    "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                                    "values " +
                                    "(" & NumeroFactura & ",'" & Serie & "',curdate(),curtime()," & Val(Arreglo(18)) & "," +
                                    "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                                    "'" & Session("id_usr") & "'," & Caja & ",'pendiente','pendiente'," +
                                    "" & Val(ArregloFactura(1)) & "," & Val(Arreglo(18)) & "," +
                                    "" & Val(ArregloFactura(3)) & "," & Val(ArregloFactura(4)) & ",1)" 'FALTA INSERTAR DATO DE EMPRESA EN ESTA TABLA
                                    comando.CommandText = StrInsertaFacEnca
                                    comando.ExecuteNonQuery()

                                    SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                                   "destino,nitcliente,seguro," +
                                   "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                                   "agencia,usuario,estadofacturacion," +
                                   "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
                                   "secuenciaorigen,secuenciadestino,idservicio,idtipoboleto) " +
                                   "values " +
                                   "(now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                                   "" & Arreglo(2).ToString & ",'" & NitCliente & "'," +
                                   "" & Seguro & ",2," +
                                   "" & ArregloButaca(i) & ",'ocupado'," & Arreglo(6).ToString & "," +
                                   "" & CodigoAgencia & ",'" & Session("id_usr") & "','FACTURADO'," +
                                   "'VALIDO',0," & Val(Arreglo(18)) & "," +
                                   "" & descuento & ",'" & Arreglo(22) & "'," & Arreglo(11) & "," & Arreglo(17) & "," +
                                   "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(19) & ",1)"
                                    ValorBoleto = Val(Arreglo(18)) '- Val(ArregloDatosEn(7))

                                End If

                                comando.CommandText = SQLInserta
                                comando.ExecuteNonQuery()
                                '------------------------------EXTRAEMOS EL NUMERO DE BOLETO CREADO-----------------------------
                                comando.CommandText = "SELECT @@IDENTITY"
                                NBoleto = comando.ExecuteScalar
                                If i = 0 Then
                                    CadenaBoletos = PadL(NBoleto.ToString, 9, "0")
                                    CadenaFacturas = NumeroFactura.ToString

                                Else
                                    CadenaBoletos = CadenaBoletos + "|" + PadL(NBoleto.ToString, 9, "0")
                                    CadenaFacturas = CadenaFacturas + "|" + NumeroFactura.ToString
                                End If
                                ''-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'
                                Dim StrInsertaFacDeta As String = "insert into Facturacion.factura_detalle " +
                                "(num_fac,serie_fac,item,id_doc,valor) " +
                                "values (" & NumeroFactura & ",'" & Serie & "',2," & NBoleto & "," & Val(Arreglo(18)) & ")"

                                comando.CommandText = StrInsertaFacDeta
                                comando.ExecuteNonQuery()

                                StrActualizaCorrelativo = "update Facturacion.agenciaseries set correlativoactual=correlativoactual+1 " +
                                "where idagenciaseries=" & correlativoserie & ""

                                comando.CommandText = StrActualizaCorrelativo
                                comando.ExecuteNonQuery()

                                StrActualizaCorrelativo = "update facturacion.agenciaseries set estado=0 " +
                                "where idagenciaseries=" & correlativoserie & " and correlativoactual>=final;"

                                comando.CommandText = StrActualizaCorrelativo
                                comando.ExecuteNonQuery()
                                'CmdActualiza.ExecuteNonQuery()

                            End If

                        Next
                        Dim ArregloBoletos() As String = CadenaBoletos.Split("|")
                        Dim ArregloFacturas() As String = CadenaFacturas.Split("|")
                        ''*******************************************Factura Electronica *********************************
                        Dim esp As String = " "
                        Dim outStr As String = ""
                        Dim NitFactura As String = NitCliente.Replace("-", "")

                        Dim Kae = "PENDIENTE"
                        Dim Face As String = "FACE63TE"
                        Dim Cae1 As String
                        Dim Cae2 As String
                        Dim LargoKae As Integer = Len(Kae)
                        If LargoKae Mod 2 = 0 Then
                            Cae1 = Left(Kae, LargoKae / 2)
                            Cae2 = Right(Kae, LargoKae / 2)
                        Else
                            Cae1 = Left(Kae, Int((LargoKae / 2) + 1))
                            Cae2 = Right(Kae, LargoKae / 2)
                        End If

                        Dim DTE As String = Left(Face, 8)

                        transaccion.Commit()

                        '******************************CREACION DEL ARCHIVO POS***********************************'
                        Dim ArchivoPos As String = "boleto" + NumeroAleatorio.ToString + "-" + Serie.ToString + ".pos"
                        Dim ruta As String = Server.MapPath("~/privado/boletopos/" + ArchivoPos)
                        Dim TextFile As New FileInfo(ruta)
                        Dim Fichero As StreamWriter = TextFile.CreateText

                        Fichero.WriteLine(" ")
                        Fichero.WriteLine("#Guia#No")
                        Fichero.WriteLine("#Factura#No")
                        Fichero.WriteLine("#Boleto#Si")
                        Fichero.WriteLine("#FechaEmision#" + StrFecha)
                        Fichero.WriteLine("#PaseDeAbordaje#Si")
                        Fichero.WriteLine("#BARCODE#" + NBoleto.ToString)
                        Fichero.WriteLine("#NombreEmpresa#Rutas Orientales.")
                        Fichero.WriteLine("#NitEmpresa#1234567")
                        Fichero.WriteLine("#NombreComercial#Rutas Orientales")
                        Fichero.WriteLine("#DireccionAgencia#" + DireccionAgencia)
                        Fichero.WriteLine("#SerieFactura#")
                        Fichero.WriteLine("#NumeroFactura#")
                        Fichero.WriteLine("#SujetoAPagosTrimestrales#SEGUN ART. 72 LEY ISR, NO REALIZAR RETENCION")
                        Fichero.WriteLine("#NumeroResolucion#" + Resolucion)
                        Fichero.WriteLine("#FechaResolucion#" + FechaResolucion)
                        Fichero.WriteLine("#Face#" + Face)
                        Fichero.WriteLine("#Cae1#" + Cae1)
                        Fichero.WriteLine("#Cae2#" + Cae2)
                        Fichero.WriteLine("#NitCliente#" + NitClienteImpresion)
                        Fichero.WriteLine("#NombreCliente#" + NombreClienteImpresion)
                        Fichero.WriteLine("#DireccionCliente#" + DireccionClienteImpresion)
                        Fichero.WriteLine("#Cantidad1#" + (ArregloButaca.Length).ToString)
                        Fichero.WriteLine("#Descripcion1#Boletos")
                        Fichero.WriteLine("#Valor1#" + ArregloFactura(0).ToString)
                        Fichero.WriteLine("#Total#" + ArregloFactura(0).ToString)
                        Fichero.WriteLine("#NumeroAsiento#0")
                        Fichero.WriteLine("#Servicio#" + Servicio.ToString)
                        Fichero.WriteLine("#Reimpresion#-")
                        Fichero.WriteLine("#Correlativo#" + NBoleto.ToString)
                        Fichero.WriteLine("#DTE#" + Face)
                        Fichero.WriteLine("#AtendidoPor#" + Session("id_usr"))
                        Fichero.WriteLine("#BoletosCantidad#" + (ArregloButaca.Length).ToString)
                        ''---------DETALLE DE LOS BOLETOS QUE SE DESEAN IMPRIMIR---------------''
                        For i = 0 To ArregloButaca.Length - 1
                            Fichero.WriteLine("#NumeroBoleto#" + ArregloBoletos(i).ToString)
                            Fichero.WriteLine("#Origen#" + NemonicoOrigen)
                            Fichero.WriteLine("#FechaViaje#" + StrFechaViaje)
                            Fichero.WriteLine("#HoraViaje#" + Arreglo(14).ToString)
                            Fichero.WriteLine("#Bus#" + Bus)
                            Fichero.WriteLine("#Pasajero#-")
                            Fichero.WriteLine("#Destino#" + NemonicoDestino)
                            Fichero.WriteLine("#NombreDestino#" + Arreglo(13).ToString)
                            Fichero.WriteLine("#FechaEmision#" + StrFecha)
                            Fichero.WriteLine("#Butaca#" + ArregloButaca(i).ToString)
                            Fichero.WriteLine("#ValorSinSeguro#" + (Val(ValorBoleto) - Val(Seguro)).ToString)
                            Fichero.WriteLine("#BoletoSerie#" + Serie)
                            Fichero.WriteLine("#BoletoFactura#" + ArregloFacturas(i).ToString)
                            Fichero.WriteLine("#FinBoleto#SI")
                        Next
                        ''---------FIN DETALLE DE LOS BOLETOS QUE SE DESEAN IMPRIMIR---------------''
                        Fichero.WriteLine(" ")
                        Fichero.Close()
                        '******************************FIN CREACIggON DEL ARCHIVO POS*******************************'
                        Return "downloadfiles.aspx?archivo=" & ArchivoPos & ""
                    Else
                        LectorSeries.Close()
                        Return "ERROR. No se establecieron los datos de la serie de Factura para ese Origen,Destino o Agencia"
                    End If
                Else
                    Lector.Close()
                    Return "ERROR. No posee caja de cobro asignada"
                End If
            Catch ex As MySqlException

                transaccion.Rollback()
                SQLInserta = "ERROR." + ex.Message
                Return SQLInserta
            Finally
                connection.Close()
                comando.Dispose()
                transaccion.Dispose()
            End Try
        End Using
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                       PROCEDIMIENTO QUE GUARDA LOS DATOS DE UNO O VARIOS BOLETOS DE DIA Y DE VUELTA                         *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function GuardaBoletoIdaVuelta(ByVal AsientosIda As String, ByVal AsientosVuelta As String, ByVal DatosIda As String, ByVal DatosVuelta As String, ByVal DatosFactura As String, ByVal Recargos As String) As String
        Dim CorrelativoFactura As Long = 0
        Dim Arreglo() As String = DatosIda.Split("|")
        Dim ArregloButaca() As String = AsientosIda.Split("|")
        Dim ArregloVuelta() As String = DatosVuelta.Split("|")
        Dim ArregloButacaVuelta() As String = AsientosVuelta.Split("|")
        Dim descuento As Double
        descuento = Arreglo(21) / (ArregloButaca.Length + ArregloButacaVuelta.Length)
        Arreglo(18) = Arreglo(18) - descuento
        ArregloVuelta(18) = ArregloVuelta(18) - descuento
        Dim ArregloFactura() As String = DatosFactura.Split("|")

        Arreglo(8) = Session("id_usr")
        Dim Seguro As Double = 0
        Dim SeguroVuelta As Double = 0
        Dim Origen As String = Arreglo(12).ToString
        Dim Destino As String = Arreglo(13).ToString
        Dim OrigenVuelta As String = ArregloVuelta(12).ToString
        Dim DestinoVuelta As String = ArregloVuelta(13).ToString
        Dim OrigenNemonico As String = ""
        Dim DestinoNemonico As String = ""
        Dim StrFecha As String = ""
        Dim StrFechaViaje As String = ""
        Dim StrFechaViajeVuelta As String = ""
        Dim NBoleto As Long
        Dim SQLInserta As String
        Dim ValorBoleto As Double = 0
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion
            'parametro _usuario y retorna caja, resolucion, serie, fecharesolucion, codigoagencia, Empresa, dispositivo nombreagencia, direccionagencia

            Dim Serie As String = ""
            Dim SerieVuelta As String = ""
            Dim ConsultaDatos As String = "call Facturacion.sp_retorna_datos_usuario('" & Session("id_usr") & "')"
            Dim Cmd As New MySqlCommand(ConsultaDatos, connection)
            Dim Lector As MySqlDataReader = Cmd.ExecuteReader()
            Lector.Read()
            Try
                If Lector.HasRows Then
                    Dim Resolucion As String = Lector("resolucion")
                    Dim FechaResolucion As String = Lector("fecharesolucion")
                    'Dim Serie As String = Lector("serie")
                    Dim CodigoAgencia As String = Lector("codigoagencia")
                    Dim Agencia As String = Lector("agencia")
                    Dim DireccionAgencia As String = Lector("direccionagencia")
                    Dim Empresa As String = Lector("empresa")
                    Dim Caja As String = Lector("caja")
                    Dim Dispositivo As String = Lector("dispositivo")
                    Lector.Close()

                    'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
                    Dim ConsultaSeries As String = "select serie " +
                    "from Facturacion.agenciaseries " +
                    "where idagencia=" & CodigoAgencia & " and idorigen=" & Arreglo(1) & " and iddestino=" & Arreglo(2) & ";"

                    'ESTO ES POR LA NUEVA IMPLEMENTACION DE GENERAR FACTURA PREIMPRESA POR AGENCIA,DESTINO Y ORIGEN
                    Dim ConsultaSeriesVuelta As String = "select serie " +
                    "from Facturacion.agenciaseries " +
                    "where idagencia=" & CodigoAgencia & " and idorigen=" & ArregloVuelta(1) & " and iddestino=" & ArregloVuelta(2) & ";"

                    Dim CmdSeries As New MySqlCommand(ConsultaSeries, connection)
                    Dim LectorSeries As MySqlDataReader = CmdSeries.ExecuteReader()

                    LectorSeries.Read()
                    If LectorSeries.HasRows Then

                        Serie = LectorSeries("serie")
                        LectorSeries.Close()

                        Dim CmdSeriesVuelta As New MySqlCommand(ConsultaSeriesVuelta, connection)
                        Dim LectorSeriesVuelta As MySqlDataReader = CmdSeriesVuelta.ExecuteReader()
                        LectorSeriesVuelta.Read()

                        If LectorSeriesVuelta.HasRows Then

                            SerieVuelta = LectorSeriesVuelta("serie")
                            LectorSeriesVuelta.Close()

                            '-------------------------------------------BOLETO DE IDA--------------------------------------------'
                            'enviar origen,fecha,destino,ruta retorna nemonicoorigen,nemonico destino,fecha_actual con hora, fecha de viaje en formato personalizado,secuencia_origen,secuencia_destino
                            Dim StrEjecuta As String = "select fn_retorna_datos_impresion " +
                            "(" & Arreglo(1).ToString & ",'" & Arreglo(0).ToString & "'," +
                            "" & Arreglo(2).ToString & "," & Arreglo(17).ToString & ")"
                            comando.CommandText = StrEjecuta
                            Dim ConsultaDatosEn As String = comando.ExecuteScalar.ToString
                            'ALMACENAMOS EL ARREGLO DE DATOS COMO RESULTADO DE LA FUNCION FN_RETORNADATOS
                            Dim ArregloDatosEn() As String = ConsultaDatosEn.Split("|")
                            Seguro = ArregloDatosEn(7)

                            Dim NemonicoOrigen As String = ArregloDatosEn(0)
                            Dim NemonicoDestino As String = ArregloDatosEn(1)
                            StrFecha = ArregloDatosEn(2)
                            Dim mifecha As Array
                            mifecha = Split(StrFecha, "-")
                            Dim mes As String = ""
                            Select Case mifecha(1)
                                Case mifecha(1) = 1 : mes = "ENE"
                                Case mifecha(1) = 2 : mes = "FEB"
                                Case mifecha(1) = 3 : mes = "MAR"
                                Case mifecha(1) = 4 : mes = "ABR"
                                Case mifecha(1) = 5 : mes = "MAY"
                                Case mifecha(1) = 6 : mes = "JUN"
                                Case mifecha(1) = 7 : mes = "JUL"
                                Case mifecha(1) = 8 : mes = "AGO"
                                Case mifecha(1) = 9 : mes = "SEP"
                                Case mifecha(1) = 10 : mes = "OCT"
                                Case mifecha(1) = 11 : mes = "NOV"
                                Case mifecha(1) = 12 : mes = "DIC"
                            End Select
                            StrFecha = mifecha(0) + "/" + mes + "/" + mifecha(2)
                            mifecha = Split(StrFecha, " ")
                            StrFecha = mifecha(0)

                            StrFechaViaje = ArregloDatosEn(3)
                            Dim StrSecuenciaOrigen As String = ArregloDatosEn(4)
                            Dim StrSecuenciaDestino As String = ArregloDatosEn(5)
                            Dim StrFechaFactura As String = ArregloDatosEn(6)
                            '---------------------------------------------------------------BOLETOS DE VUELTA------------------------------------------'
                            'enviar origen,fecha,destino,ruta retorna nemonicoorigen,nemonico destino,fecha_actual con hora, fecha de viaje en formato personalizado,secuencia_origen,secuencia_destino
                            Dim StrEjecutaVuelta As String = "select fn_retorna_datos_impresion " +
                            "(" & ArregloVuelta(1).ToString & ",'" & ArregloVuelta(0).ToString & "'," +
                            "" & ArregloVuelta(2).ToString & "," & ArregloVuelta(17).ToString & ")"
                            comando.CommandText = StrEjecutaVuelta
                            Dim ConsultaDatosEnVuelta As String = comando.ExecuteScalar.ToString
                            'ALMACENAMOS EL ARREGLO DE DATOS COMO RESULTADO DE LA FUNCION FN_RETORNADATOS
                            Dim ArregloDatosEnVuelta() As String = ConsultaDatosEnVuelta.Split("|")

                            SeguroVuelta = ArregloDatosEnVuelta(7)

                            Dim NemonicoOrigenVuelta As String = ArregloDatosEnVuelta(0)
                            Dim NemonicoDestinoVuelta As String = ArregloDatosEnVuelta(1)
                            StrFechaViajeVuelta = ArregloDatosEnVuelta(3)
                            Dim StrSecuenciaOrigenVuelta As String = ArregloDatosEnVuelta(4)
                            Dim StrSecuenciaDestinoVuelta As String = ArregloDatosEnVuelta(5)
                            'Dim NumeroFacTemporal As Integer = NumeroAleatorio()
                            Dim NitCliente As String = ""
                            Dim NombreCliente As String = ""
                            Dim DireccionCliente As String = ""
                            Dim NitClienteImpresion As String = ""
                            Dim NombreClienteImpresion As String = ""
                            Dim DireccionClienteImpresion As String = ""
                            If Trim(Arreglo(3).ToString) = "N/A" Then
                                NitCliente = "C/F"
                                NombreCliente = "CONSUMIDOR FINAL"
                                DireccionCliente = "N/A"
                                NitClienteImpresion = "___________________________"
                                NombreClienteImpresion = "___________________________"
                                DireccionClienteImpresion = "___________________________"
                            ElseIf Trim(Arreglo(3).ToString) = "C/F" Then
                                NitCliente = "C/F"
                                NombreCliente = "CONSUMIDOR FINAL"
                                DireccionCliente = "N/A"
                                NitClienteImpresion = "___________________________"
                                NombreClienteImpresion = "___________________________"
                                DireccionClienteImpresion = "___________________________"
                            Else
                                NitCliente = Arreglo(3).ToString
                                NombreCliente = Arreglo(15).ToString
                                DireccionCliente = Arreglo(20).ToString
                                NitClienteImpresion = Arreglo(3).ToString
                                NombreClienteImpresion = Arreglo(15).ToString
                                DireccionClienteImpresion = Arreglo(20).ToString
                            End If
                            '-----------------INSERT PARA LOS CLIENTES, POR SI NO EXISTE--------------------'
                            If NitCliente = "C/F" Or NitCliente = "N/A" Then
                            Else
                                Dim SQLInsertacliente As String = "call Facturacion.sp_guarda_cliente" +
                                "('" & NitCliente & "','" & NombreCliente & "','" & DireccionCliente & "'," & CodigoAgencia & "," & Empresa & ")"
                                comando.CommandText = SQLInsertacliente
                                comando.ExecuteNonQuery()
                            End If
                            '----------------------EXTRAEMOS EL CORRELATIVO DEL NUMERO DE FACTURA 
                            
                            Dim ConsultaServicio As String = "select descripcion from Boletos.servicio where idservicio=" & Arreglo(19) & " and id_empsa=" & Empresa & ""
                            comando.CommandText = ConsultaServicio
                            Dim Servicio As String = comando.ExecuteScalar
                            Dim ConsultaServicioVuelta As String = "select descripcion from Boletos.servicio where idservicio=" & ArregloVuelta(19) & " and id_empsa=" & Empresa & ""
                            comando.CommandText = ConsultaServicioVuelta
                            Dim ServicioVuelta As String = comando.ExecuteScalar
                            Dim ConsultaBus As String = "select if(curdate()='" & Arreglo(0).ToString & "'," +
                            "(select id_veh from Encomiendas.horarios where " +
                            "fecha_detaHorario=curdate() and id_empsa=" & Empresa & " and id_ruta=" & Arreglo(17) & " limit 1),'---')"
                            comando.CommandText = ConsultaBus
                            Dim Bus As String = IIf(comando.ExecuteScalar IsNot DBNull.Value, comando.ExecuteScalar, "---")

                            If Bus = "SIN ASIGNACION" Then
                                Bus = "S/A"
                            End If
                            Dim ConsultaBusVuelta As String = "select if(curdate()='" & ArregloVuelta(0).ToString & "'," +
                            "(select id_veh from Encomiendas.horarios where " +
                            "fecha_detaHorario=curdate() and id_empsa=" & Empresa & " and id_ruta=" & ArregloVuelta(17) & " limit 1),'---')"
                            comando.CommandText = ConsultaBusVuelta
                            Dim BusVuelta As String = comando.ExecuteScalar
                            Dim CadenaBoletos As String = ""
                            Dim CadenaFacturas As String = ""
                            If BusVuelta = "SIN ASIGNACION" Then
                                BusVuelta = "S/A"
                            End If

                            Dim StrNumeroCorrelativo As String = "select facturacion.fn_nuevo_correlativo " +
                            "(" & CodigoAgencia & "," & Arreglo(1) & "," & Arreglo(2) & ");"
                            Dim StrActualizaCorrelativo As String = ""

                            Dim NumeroFactura As Long = 0
                            '--------------INSERCION DE LOS DATOS DE FACTURACION-------------'

                            Dim correlativoserie As Integer = 0
                            'BOLETOS DE IDA=============================================
                            For i = 0 To ArregloButaca.Length - 1

                                StrActualizaCorrelativo = "select idagenciaseries " +
                                "from facturacion.agenciaseries " +
                                "where idorigen=" & Arreglo(1) & " and iddestino=" & Arreglo(2) & " And " +
                                "idagencia=" & CodigoAgencia & " and estado=1 order by prioridad LIMIT 1"

                                comando.CommandText = StrActualizaCorrelativo
                                correlativoserie = comando.ExecuteScalar

                                comando.CommandText = StrNumeroCorrelativo
                                NumeroFactura = comando.ExecuteScalar

                                If NumeroFactura = -1 Then
                                    transaccion.Rollback()

                                    Return "ERROR: El correlativo de facturas para esa serie no es suficiente"
                                Else
                                    NumeroFactura = NumeroFactura + 1


                                    If Recargos <> "0" Then
                                        Dim recargo As Array
                                        recargo = Split(Recargos, ",")
                                        Dim credito As Double
                                        credito = Val(Arreglo(18)) - Val(recargo(0))
                                        Dim recargoBoleto As Double = 0
                                        If recargo(1) > recargo(0) Then
                                            recargoBoleto = Val(recargo(1)) - Val(recargo(0))
                                        End If

                                        Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                                        "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                                        "nit_clt,nom_clt,id_agc," +
                                        "id_usr,id_agenciacaja,kae,face," +
                                        "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                                        "values " +
                                        "(" & NumeroFactura & ",'" & Serie & "',curdate(),curtime()," & Val(recargo(1)) & "," +
                                        "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                                        "'" & Arreglo(8).ToString & "'," & Caja & ",'pendiente','pendiente'," +
                                        "" & Val(ArregloFactura(1)) & "," & Val(ArregloFactura(2)) & "," +
                                        "" & Val(ArregloFactura(3)) & "," & Val(ArregloFactura(4)) & ",1)"

                                        SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                                       "destino,nitcliente,seguro," +
                                       "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                                       "agencia,usuario,estadofacturacion," +
                                       "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
                                       "secuenciaorigen,secuenciadestino,idservicio,idtipoboleto,recargo,credito,boletocredito) " +
                                       "values " +
                                       "(now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                                       "" & Arreglo(2).ToString & ",'" & NitCliente & "'," +
                                       "" & Seguro & ",2," +
                                       "" & ArregloButaca(i) & ",'ocupado'," & Arreglo(6).ToString & "," +
                                       "" & CodigoAgencia & ",'" & Arreglo(8).ToString & "','FACTURADO'," +
                                       "'VALIDO',0," & Val(recargo(1)) & "," +
                                       "" & descuento & ",'" & Arreglo(22) & "'," & Arreglo(11) & "," & Arreglo(17) & "," +
                                       "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(19) & ",1," & recargoBoleto & "," & credito & "," & recargo(2) & ")"
                                        Dim SqlUpdateBoleto As String
                                        SqlUpdateBoleto = "update Boletos.boleto set numerobutaca=null, estadoboleto='INVALIDO' where id_boleto=" & Arreglo(23) & ""
                                        comando.CommandText = SqlUpdateBoleto
                                        comando.ExecuteNonQuery()
                                        ValorBoleto = Val(recargo(1))
                                    Else

                                        Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                                        "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                                        "nit_clt,nom_clt,id_agc," +
                                        "id_usr,id_agenciacaja,kae,face," +
                                        "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                                        "values " +
                                        "(" & NumeroFactura & ",'" & Serie & "',curdate(),curtime()," & Val(Arreglo(18)) & "," +
                                        "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                                        "'" & Arreglo(8).ToString & "'," & Caja & ",'pendiente','pendiente'," +
                                        "" & Val(ArregloFactura(1)) & "," & Val(ArregloFactura(2)) & "," +
                                        "" & Val(ArregloFactura(3)) & "," & Val(ArregloFactura(4)) & ",1)"
                                        'FALTA INSERTAR DATO DE EMPRESA EN ESTA TABLA
                                        comando.CommandText = StrInsertaFacEnca
                                        comando.ExecuteNonQuery()

                                        SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                                            "destino,nitcliente,seguro," +
                                            "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                                            "agencia,usuario,estadofacturacion," +
                                            "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta,secuenciaorigen,secuenciadestino,idservicio) " +
                                            "values (now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                                            "" & Arreglo(2).ToString & ",'" & NitCliente & "'," & Seguro & "," +
                                            "2," & ArregloButaca(i) & ",'ocupado'," & Arreglo(6).ToString & "," +
                                            "" & CodigoAgencia & ",'" & Arreglo(8).ToString & "','FACTURADO'," +
                                            "'VALIDO',0," & Val(Arreglo(18)) & "," +
                                            "" & descuento & ",'" & Arreglo(22) & "'," & Arreglo(11) & "," & Arreglo(17) & "," +
                                            "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(19) & ")"
                                    End If
                                    comando.CommandText = SQLInserta
                                    comando.ExecuteNonQuery()
                                    comando.CommandText = "SELECT @@IDENTITY"
                                    NBoleto = comando.ExecuteScalar
                                    If i = 0 Then
                                        CadenaBoletos = PadL(NBoleto.ToString, 9, "0")
                                        CadenaFacturas = NumeroFactura.ToString
                                    Else
                                        CadenaBoletos = CadenaBoletos + "|" + PadL(NBoleto.ToString, 9, "0")
                                        CadenaFacturas = CadenaFacturas + "|" + NumeroFactura.ToString
                                    End If

                                    Dim StrInsertaFacDeta As String = "insert into Facturacion.factura_detalle " +
                                    "(num_fac,serie_fac,item,id_doc,valor) " +
                                    "values (" & NumeroFactura & ",'" & Serie & "',2," & NBoleto & "," & Val(Arreglo(18)) & ")"
                                    comando.CommandText = StrInsertaFacDeta
                                    comando.ExecuteNonQuery()

                                    StrActualizaCorrelativo = "update Facturacion.agenciaseries set correlativoactual=correlativoactual+1 " +
                                    "where idagenciaseries=" & correlativoserie & ""

                                    comando.CommandText = StrActualizaCorrelativo
                                    comando.ExecuteNonQuery()

                                    StrActualizaCorrelativo = "update facturacion.agenciaseries set estado=0 " +
                                    "where idagenciaseries=" & correlativoserie & " and correlativoactual>=final;"

                                    comando.CommandText = StrActualizaCorrelativo
                                    comando.ExecuteNonQuery()

                                End If

                            Next
                            Dim ArregloBoletos() As String = CadenaBoletos.Split("|")
                            Dim ArregloFacturas() As String = CadenaFacturas.Split("|")
                            '---------------------------PARA LOS ASIENTOS DE VUELTA------------------------------------
                            Dim CadenaBoletosVuelta As String = ""
                            Dim CadenaFacturasVuelta As String = ""

                            StrNumeroCorrelativo = "select facturacion.fn_nuevo_correlativo " +
                            "(" & CodigoAgencia & "," & ArregloVuelta(1) & "," & ArregloVuelta(2) & ");"

                            
                            'BOLETOS DE VUELTA====================================
                            For i = 0 To ArregloButacaVuelta.Length - 1

                                StrActualizaCorrelativo = "select idagenciaseries " +
                                "from facturacion.agenciaseries " +
                                "where idorigen=" & ArregloVuelta(1) & " and iddestino=" & ArregloVuelta(2) & " And " +
                                "idagencia=" & CodigoAgencia & " and estado=1 order by prioridad LIMIT 1"

                                comando.CommandText = StrActualizaCorrelativo
                                correlativoserie = comando.ExecuteScalar

                                comando.CommandText = StrNumeroCorrelativo
                                NumeroFactura = comando.ExecuteScalar

                                If NumeroFactura = -1 Then
                                    transaccion.Rollback()

                                    Return "ERROR: El correlativo de facturas para esa serie no es suficiente"
                                Else
                                    NumeroFactura = NumeroFactura + 1

                                    Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                                    "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                                    "nit_clt,nom_clt,id_agc," +
                                    "id_usr,id_agenciacaja,kae,face," +
                                    "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                                    "values " +
                                    "(" & NumeroFactura & ",'" & SerieVuelta & "',curdate(),curtime()," & Val(ArregloVuelta(18)) & "," +
                                    "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                                    "'" & Arreglo(8) & "'," & Caja & ",'pendiente','pendiente'," +
                                    "0," & Val(ArregloVuelta(18)) & "," +
                                    "0,0,1)"
                                    'FALTA INSERTAR DATO DE EMPRESA EN ESTA TABLA
                                    comando.CommandText = StrInsertaFacEnca
                                    comando.ExecuteNonQuery()


                                    SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                                        "destino,nitcliente,seguro," +
                                        "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                                        "agencia,usuario,estadofacturacion," +
                                        "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta,secuenciaorigen,secuenciadestino,idservicio) " +
                                        "values (now(),'" & ArregloVuelta(0).ToString & "'," & ArregloVuelta(1).ToString & "," +
                                        "" & ArregloVuelta(2).ToString & ",'" & NitCliente & "'," & SeguroVuelta & "," +
                                        "2," & ArregloButacaVuelta(i) & ",'ocupado'," & ArregloVuelta(6).ToString & "," +
                                        "" & ArregloVuelta(7).ToString & ",'" & Arreglo(8).ToString & "','FACTURADO'," +
                                        "'VALIDO',0," & Val(ArregloVuelta(18)) & "," +
                                        "" & descuento & ",'" & Arreglo(22) & "'," & ArregloVuelta(11) & "," & ArregloVuelta(17) & "," +
                                        "" & StrSecuenciaOrigenVuelta & "," & StrSecuenciaDestinoVuelta & "," & ArregloVuelta(19) & ")"
                                    comando.CommandText = SQLInserta
                                    comando.ExecuteNonQuery()
                                    comando.CommandText = "SELECT @@IDENTITY"
                                    NBoleto = comando.ExecuteScalar
                                    If i = 0 Then
                                        CadenaBoletosVuelta = PadL(NBoleto.ToString, 9, "0")
                                        CadenaFacturasVuelta = NumeroFactura.ToString
                                    Else
                                        CadenaBoletosVuelta = CadenaBoletosVuelta + "|" + PadL(NBoleto.ToString, 9, "0")
                                        CadenaFacturasVuelta = CadenaFacturasVuelta + "|" + NumeroFactura.ToString

                                    End If
                                    Dim StrInsertaFacDeta As String = "insert into Facturacion.factura_detalle " +
                                    "(num_fac,serie_fac,item,id_doc,valor) " +
                                    "values (" & NumeroFactura & ",'" & SerieVuelta & "',2," & NBoleto & "," & Val(ArregloVuelta(18)) & ")"
                                    comando.CommandText = StrInsertaFacDeta
                                    comando.ExecuteNonQuery()

                                    StrActualizaCorrelativo = "select idagenciaseries " +
                                    "from Facturacion.agenciaseries " +
                                    "where idorigen=" & ArregloVuelta(1) & " and iddestino=" & ArregloVuelta(2) & " And " +
                                    "idagencia=" & CodigoAgencia & " and estado=1 order by prioridad LIMIT 1"

                                    StrActualizaCorrelativo = "update Facturacion.agenciaseries set correlativoactual=correlativoactual+1 " +
                                    "where idagenciaseries=" & correlativoserie & ""

                                    comando.CommandText = StrActualizaCorrelativo
                                    comando.ExecuteNonQuery()

                                    StrActualizaCorrelativo = "update facturacion.agenciaseries set estado=0 " +
                                    "where idagenciaseries=" & correlativoserie & " and correlativoactual>=final;"

                                    comando.CommandText = StrActualizaCorrelativo
                                    comando.ExecuteNonQuery()


                                End If
                            Next
                            Dim ArregloBoletosVuelta() As String = CadenaBoletosVuelta.Split("|")
                            Dim ArregloFacturasVuelta() As String = CadenaFacturasVuelta.Split("|")
                            '*******************************************Factura Electronica *********************************
                            Dim esp As String = " "
                            Dim outStr As String = ""
                            Dim NitFactura As String = NitCliente.Replace("-", "")

                            'ArregloFacturaElectronica = FacturaElectronicaTemporalLocal(Serie).Split("|")
                            Dim Kae = "PENDIENTE"
                            Dim Face As String = "PENDIENTE"
                            Dim Cae1 As String
                            Dim Cae2 As String
                            Dim LargoKae As Integer = Len(Kae)
                            If LargoKae Mod 2 = 0 Then
                                Cae1 = Left(Kae, LargoKae / 2)
                                Cae2 = Right(Kae, LargoKae / 2)
                            Else
                                Cae1 = Left(Kae, Int((LargoKae / 2) + 1))
                                Cae2 = Right(Kae, LargoKae / 2)
                            End If
                            'Face = ArregloFacturaElectronica(1)
                            If Len(Face) > 1 Then
                                Face = Right(Face, Len(Face))
                            End If
                            'Dim CorrelativoReal = Right(Face, 9)
                            Dim DTE As String = Left(Face, 8)

                            transaccion.Commit()

                            Dim ArchivoPos As String = "boleto" + NumeroAleatorio.ToString + ".pos"
                            Dim ruta As String = Server.MapPath("~/privado/boletopos/" + ArchivoPos)
                            Dim TextFile As New FileInfo(ruta)
                            Dim Fichero As StreamWriter = TextFile.CreateText
                            '********************************************DATOS PARA EL ARCHIVO POS SOLO IDA********************************************
                            Fichero.WriteLine(" ")
                            Fichero.WriteLine("#Guia#No")
                            Fichero.WriteLine("#Factura#No")
                            Fichero.WriteLine("#Boleto#Si")
                            Fichero.WriteLine("#PaseDeAbordaje#Si")
                            Fichero.WriteLine("#BARCODE#" + NBoleto.ToString)
                            Fichero.WriteLine("#NombreEmpresa#Rutas Orientales")
                            Fichero.WriteLine("#NitEmpresa#1234567")
                            Fichero.WriteLine("#NombreComercial#Rutas Orientales")
                            Fichero.WriteLine("#DireccionAgencia#" + DireccionAgencia)
                            Fichero.WriteLine("#FechaEmision#" + StrFecha)
                            Fichero.WriteLine("#SerieFactura#" + Serie)
                            Fichero.WriteLine("#NumeroFactura#")
                            Fichero.WriteLine("#SujetoAPagosTrimestrales#SEGUN ART. 72 LEY ISR, NO REALIZAR RETENCION")
                            Fichero.WriteLine("#NumeroResolucion#" + Resolucion)
                            Fichero.WriteLine("#FechaResolucion#" + FechaResolucion)
                            Fichero.WriteLine("#Face#" + Face)
                            Fichero.WriteLine("#Cae1#" + Cae1)
                            Fichero.WriteLine("#Cae2#" + Cae2)
                            Fichero.WriteLine("#NitCliente#" + NitClienteImpresion)
                            Fichero.WriteLine("#NombreCliente#" + NombreClienteImpresion)
                            Fichero.WriteLine("#DireccionCliente#" + NombreClienteImpresion)
                            Fichero.WriteLine("#Cantidad1#" + (Val(ArregloButaca.Length) + Val(ArregloButacaVuelta.Length)).ToString)
                            Fichero.WriteLine("#Descripcion1#Boletos")
                            Fichero.WriteLine("#Valor1#" + ArregloFactura(0).ToString)
                            Fichero.WriteLine("#Total#" + ArregloFactura(0).ToString)
                            Fichero.WriteLine("#NumeroAsiento#0")
                            Fichero.WriteLine("#Servicio#" + Servicio.ToString)
                            Fichero.WriteLine("#Reimpresion#-")
                            Fichero.WriteLine("#Correlativo#" + NBoleto.ToString)
                            Fichero.WriteLine("#DTE#" + Face)
                            Fichero.WriteLine("#AtendidoPor#" + Arreglo(8).ToString)
                            Fichero.WriteLine("#BoletosCantidad#" + (Val(ArregloButaca.Length) + Val(ArregloButacaVuelta.Length)).ToString)

                            For i = 0 To ArregloButaca.Length - 1
                                Fichero.WriteLine("#NumeroBoleto#" + ArregloBoletos(i).ToString)
                                Fichero.WriteLine("#Origen#" + NemonicoOrigen)
                                Fichero.WriteLine("#FechaViaje#" + StrFechaViaje)
                                Fichero.WriteLine("#HoraViaje#" + Arreglo(14).ToString)
                                Fichero.WriteLine("#Bus#" + Bus)
                                Fichero.WriteLine("#Pasajero#-")
                                Fichero.WriteLine("#Destino#" + NemonicoDestino)
                                Fichero.WriteLine("#NombreDestino#" + Arreglo(13).ToString)
                                Fichero.WriteLine("#FechaEmision#" + StrFecha)
                                Fichero.WriteLine("#Butaca#" + ArregloButaca(i).ToString)
                                Fichero.WriteLine("#ValorSinSeguro#" + (Val(Arreglo(18)) - Val(Seguro)).ToString)
                                Fichero.WriteLine("#BoletoSerie#" + Serie)
                                Fichero.WriteLine("#BoletoFactura#" + ArregloFacturas(i).ToString)
                                Fichero.WriteLine("#FinBoleto#SI")
                            Next

                            '***************************************** DATOS PARA EL ARCHIVO POS IDA Y VUELTA**********************
                            For i = 0 To ArregloButacaVuelta.Length - 1
                                '----------------------SEGUIMOS AGREGANDO LINEAS AL ARCHIVO A POS A DESCARGAR, BOLETOS DE VUELTA----------------
                                Fichero.WriteLine("#NumeroBoleto#" + ArregloBoletosVuelta(i).ToString)
                                Fichero.WriteLine("#Origen#" + NemonicoOrigenVuelta)
                                Fichero.WriteLine("#FechaViaje#" + StrFechaViajeVuelta)
                                Fichero.WriteLine("#HoraViaje#" + ArregloVuelta(14).ToString)
                                Fichero.WriteLine("#Bus#" + BusVuelta)
                                Fichero.WriteLine("#Pasajero#-")
                                Fichero.WriteLine("#Destino#" + NemonicoDestinoVuelta)
                                Fichero.WriteLine("#NombreDestino#" + ArregloVuelta(13).ToString)
                                Fichero.WriteLine("#FechaEmision#" + StrFecha)
                                Fichero.WriteLine("#Butaca#" + ArregloButacaVuelta(i).ToString)
                                Fichero.WriteLine("#ValorSinSeguro#" + (Val(ArregloVuelta(18)) - Val(Seguro)).ToString)
                                Fichero.WriteLine("#BoletoSerie#" + SerieVuelta)
                                Fichero.WriteLine("#BoletoFactura#" + ArregloFacturasVuelta(i).ToString)
                                Fichero.WriteLine("#FinBoleto#SI")
                            Next
                            'CERRAMOS EL FICHERO------------------------------------------------------!
                            Fichero.Close()
                            '******************************FIN CREACION DEL ARCHIVO POS*******************************'
                            Return "downloadfiles.aspx?archivo=" & ArchivoPos & ""
                        Else
                            Return "ERROR. No se han establecido los datos de Serie por origen,destino o agencia (VUELTA)"
                            LectorSeriesVuelta.Close()
                        End If
                    Else
                        Return "ERROR. No se han establecido los datos de Serie por origen,destino o agencia (IDA)"
                        LectorSeries.Close()
                    End If
                Else

                    Return "ERROR. No posee caja de cobro asignada"
                End If
            Catch ex As MySqlException
                transaccion.Rollback()
                SQLInserta = "ERROR." + ex.Message
                Return SQLInserta
            Finally
                connection.Close()
                comando.Dispose()
                transaccion.Dispose()
            End Try
        End Using
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                            PROCEDIMIENTO QUE GUARDA LOS DATOS DE UNO O VARIOS BOLETOS SOLO DE DIA                           *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function PrecioBoleto(ByVal origen As String, ByVal destino As String, ByVal servicio As String, ByVal empresa As String) As String
        Dim Precio As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim ConsultaViajeroF As String = "select fn_calcula_precio_boleto" +
                    "(" & origen & "," & destino & "," & servicio & "," & empresa & ") as precio"
                Dim cmd As New MySqlCommand(String.Format(ConsultaViajeroF), connection)

                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                If reader.HasRows Then
                    Precio = reader("precio")
                Else
                    Precio = "100.00"
                End If

            Catch ex As Exception
                Precio = "100.00"
            End Try
        End Using
        Return Precio
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*FUNCION QUE GENERA UN NUMERO ALEATORIO ENTRE 1 Y 1000, UTILIZADO PARA  NUMERO DE FACTURA TEMPORAL, ANTES DE EXTRAER DATOS    *'
    '*                                                            DEL FACE                                                         *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************
    Public Function NumeroAleatorio() As Integer
        Dim LimiteMinimo As Integer = 1
        Dim LimiteMaximo As Integer = 9999
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return 999990000 + Aleatorio
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*      PROCEDIMIENTO QUE RETORNA EL CODIGO HTML. DEL BUS SELECCIONADO O EL ESTANDARD DE ACUERDO AL TIPO DE SERVICIO           *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function ExtraeHtml(ByVal Horario As String, ByVal Ruta As String, ByVal Empresa As String, ByVal Campo As String, ByVal Fecha As String, ByVal Servicio As String) As String
        Dim CampoHtml As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim ComandoHorario As New MySqlCommand(String.Format("select fn_determina_bus(" & Horario & "," & Ruta & "," & Empresa & "," & Campo & ",'" & Fecha & "', '" & Servicio & "') as codigohtml"), connection)
                Dim Lector As MySqlDataReader = ComandoHorario.ExecuteReader()
                Lector.Read()

                If Lector.HasRows Then
                    CampoHtml = Lector("codigohtml")
                    Return CampoHtml
                Else
                    CampoHtml = "sin resultado"
                    Return CampoHtml
                End If
                Return CampoHtml
            Catch ex As Exception
                Return "no funciona " + ex.Message
            End Try
        End Using

    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*            PROCEDIMIENTO QUE RETORNA EL PORCENTAJE EXTRA QUE SE COBRA AL UTILIZAR UN SERVICIO DE PAGO                       *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function CalculaRecargo(ByVal tarjeta As String) As String
        Dim porcentaje, recargo As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim ComandoHorario As New MySqlCommand(String.Format("select porcentaje from Facturacion.forma_pago where id_forma_pago=2"), connection)
                Dim Lector As MySqlDataReader = ComandoHorario.ExecuteReader()
                Lector.Read()
                If Lector.HasRows Then
                    porcentaje = Lector("porcentaje")

                Else
                    porcentaje = "sin resultado"
                End If
                recargo = Val(tarjeta) * Val(porcentaje) / 100
                Return recargo
            Catch ex As Exception
                Return "no funciona " + ex.Message
            End Try
        End Using

    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*      PROCEDIMIENTO QUE SIRVE PARA RELLENAR EL NUMERO DE BOLETO CON "0" O DEPENDIENDO DEL CARACTER QUE SE QUIERA UTILIZAR    *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    Public Function PadL(ByVal cVar As String, ByVal nLen As Integer, ByVal cCar As String) As String
        Dim n As Integer
        cVar = Trim(cVar)
        nLen = nLen - Len(cVar)
        For n = 1 To nLen
            cVar = cCar & cVar
        Next
        PadL = cVar
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                PROCEDIMIENTO QUE DETERMINA LOS HORARIOS VALIDOS SEGUN ORIGEN, DESTINO Y FECHA INGRESADOS                    *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function HoraSugeridaAjax(ByVal Origen As String, ByVal Destino As String, ByVal Fecha As String) As List(Of [ClaseHora])
        Dim Busqueda As String = "call sp_retorna_turnos_validos(" & Origen & ",'" & Fecha & "')"
        Dim result As List(Of [ClaseHora]) = New List(Of ClaseHora)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseHora
                    If Not IsDBNull(reader("hora")) Then
                        Elemento.datos = reader("datos")
                        Elemento.hora = reader("hora")
                        Elemento.estado = reader("estado")
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
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                PROCEDIMIENTO QUE DETERMINA LOS TURNOS VALIDOS SEGUN EL HORIGEN Y LA FECHA INGRESADA                    *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function TurnoSugerido(ByVal Origen As String, ByVal Fecha As String) As List(Of [ClaseHora])
        ' MsgBox("** TurnoSugerido" & Origen & " - " & Fecha)
        Dim fechaStr As Array
        fechaStr = Split(Fecha, "/")
        Dim fechaFinal As String
        fechaFinal = fechaStr(2) & "-" & fechaStr(1) & "-" & fechaStr(0)
        Dim Busqueda As String = "call sp_retorna_turnos_validos(" & Origen & ",'" & fechaFinal.ToString & "')"
        Dim result As List(Of [ClaseHora]) = New List(Of ClaseHora)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseHora
                    If Not IsDBNull(reader("hora")) Then
                        Elemento.datos = reader("datos")
                        Elemento.hora = reader("hora")
                        Elemento.estado = reader("estado")
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
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*PROCEDIMIENTO QUE DETERMINA EL LUGAR DE ORIGEN AL QUE PERTENECE EL USUARIO CONECTADO, MOSTRANDO TODOS LOS ORIGENES EN LA BD  *'
    '*                              PERO MOSTRANDO COMO "SELECT" EL OPTION DEL ORIGEN DEL USUARIO                                  *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function OrigenAjax() As List(Of [ClaseOrigen])
        Dim Busqueda As String = "call sp_retorna_origen_usuario('" & "admin" & "')"
        Dim result As List(Of [ClaseOrigen]) = New List(Of ClaseOrigen)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseOrigen

                    Elemento.iddestino = reader("codigodestino")
                    Elemento.nombredestino = reader("destino")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                               PROCEDIMIENTO QUE RETORNA LOS DESTINOS REGISTRADOS EN LA BD                                   *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod(True)> _
    Public Function DestinoAjax() As List(Of [ClaseOrigen])
        ' MsgBox("** DestinoAjax")
        Dim Busqueda As String = "call sp_retorna_destinos()"
        Dim result As List(Of [ClaseOrigen]) = New List(Of ClaseOrigen)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Busqueda, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New ClaseOrigen
                    Elemento.iddestino = reader("codigodestino")
                    Elemento.nombredestino = reader("destino")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                                  PROCEDIMIENTO QUE RETORNA LA FECHA EN FORMATO dd/mm/yyyy                                   *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function FechaHora() As List(Of [ClaseFechaHora])
        Dim result As List(Of [ClaseFechaHora]) = New List(Of ClaseFechaHora)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                'Dim StrConsulta As String = "select DATE_FORMAT(curdate(),'%d/%m/%Y') as fecha,CAST(curtime() as char(8)) as hora"
                Dim StrConsulta As String = "select DATE_FORMAT(curdate(),'%d/%m/%Y') as fecha"
                Dim ComandoHorario As New MySqlCommand(String.Format(StrConsulta), connection)
                Dim Lector As MySqlDataReader = ComandoHorario.ExecuteReader()
                Lector.Read()
                Dim Elemento As New ClaseFechaHora
                If Lector.HasRows Then
                    Elemento.fecha = Lector("fecha")
                    'Elemento.hora = Lector("hora")
                Else
                    Elemento.fecha = 1
                    'Elemento.hora = "INGRESE FECHA"
                End If
                result.Add(Elemento)
            Catch ex As Exception
                Dim Elemento As New ClaseFechaHora
                Elemento.fecha = "00/00/0000"
                Elemento.hora = "00:00:00"
                result.Add(Elemento)
            End Try
            Return result
        End Using

    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*                                   PROCEDIMIENTO QUE DETERMINA SI UN CLIENTE EXISTE O NO                                     *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function BuscaNit(ByVal Nit As String) As List(Of [ClaseCliente])
        Dim result As List(Of [ClaseCliente]) = New List(Of [ClaseCliente])
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(String.Format("select nom_clt,dire_clt from Facturacion.cliente where nit_clt='" & Nit & "'"), connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                Dim Elemento As New ClaseCliente
                If reader.HasRows Then
                    Elemento.nitcliente = Nit
                    Elemento.nombrecliente = reader("nom_clt")
                    Elemento.direccioncliente = IIf(reader("dire_clt") IsNot DBNull.Value, reader("dire_clt"), "__________________")
                    result.Add(Elemento)
                Else
                    Elemento.nitcliente = Nit
                    Elemento.nombrecliente = "INGRESE NOMBRE"
                    Elemento.direccioncliente = "INGRESE DIRECCION"
                    result.Add(Elemento)
                End If
            Catch ex As Exception
                Dim Elemento As New ClaseCliente
                Elemento.nitcliente = "C/F"
                Elemento.nombrecliente = ex.Message
                Elemento.direccioncliente = "ERROR"
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function
    '*******************************************************************************************************************************'
    '*                                                                                                                             *'
    '*          PROCEDIMIENTO QUE BUSCA EN LA BD POR EL CODIGO DEL "ViajeroFrecuente", retornando DESCUENTO                        *'
    '*                                                                                                                             *'
    '*******************************************************************************************************************************'
    <WebMethod()> _
    Public Function Buscaviajerofrecuente(ByVal codigo As String) As List(Of [ClaseViajerofrecuente])
        Dim result As List(Of [ClaseViajerofrecuente]) = New List(Of [ClaseViajerofrecuente])
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim ConsultaViajeroF As String = "select " +
                "CONCAT(PrimerNombre,' ',SegundoNombre,' ',PrimerApellido,' ',SegundoApellido) as nombrecompleto," +
                "Nit as nit," +
                "Direccion as direccion," +
                "descuento from ViajeroFrecuente.cliente where IdCliente=" & codigo & ""
                Dim cmd As New MySqlCommand(String.Format(ConsultaViajeroF), connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                Dim Elemento As New ClaseViajerofrecuente
                If reader.HasRows Then
                    Elemento.nitviajero = reader("nit")
                    Elemento.nombreviajero = reader("nombrecompleto")
                    Elemento.direccionviajero = reader("direccion")
                    Elemento.descuentoviajero = reader("descuento")
                    result.Add(Elemento)
                Else
                    Elemento.descuentoviajero = "0"
                    result.Add(Elemento)
                End If
            Catch ex As Exception
                Dim Elemento As New ClaseViajerofrecuente
                Elemento.descuentoviajero = "ERROR " + ex.Message
                result.Add(Elemento)
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function verificarReservacion(ByVal Asientos As String, ByVal valores As String, ByVal fecha As String, ByVal butacas As String) As String
        Dim parametros As Array
        parametros = Split(valores, "|")
        Dim asiento As Array
        asiento = Split(Asientos, ",")
        Dim arrayfecha As Array
        arrayfecha = Split(fecha, "/")
        Dim formatFecha As String = arrayfecha(2) + arrayfecha(1) + arrayfecha(0)
        Dim cantidad As Integer = Val(asiento.Length)
        Dim i As Integer = 0
        While i <= cantidad - 1
            Dim consulta As String = "select resdeta.* from Boletos.reservaciondetalle resdeta inner join Boletos.reservacion res" +
                                     " where resdeta.id_reservacion = res.id_reservacion" +
                                     " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                     " and resdeta.estadobutaca='reservado'" +
                                     " and resdeta.numerobutaca=" + asiento(i) + ""
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                Try
                    connection.Open()
                    Dim cmd As New MySqlCommand(consulta, connection)
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        Dim SqlUpdate As String = "update Boletos.reservaciondetalle resdeta" +
                                 " inner join Boletos.reservacion res" +
                                 " set estadobutaca='ocupado'" +
                                 " where(resdeta.id_reservacion = res.id_reservacion)" +
                                 " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                 " and resdeta.estadobutaca='reservado'" +
                                 " and resdeta.numerobutaca=" + asiento(i) + ""
                        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                            Dim comando As MySqlCommand
                            Try
                                conn.Open()
                                comando = New MySqlCommand(SqlUpdate, conn)
                                comando.ExecuteNonQuery()
                                comando.Dispose()
                            Catch ex As MySqlException
                            End Try
                        End Using
                    End If
                Catch ex As Exception
                    'MsgBox(ex.Message)
                End Try
            End Using
            i = i + 1
        End While
        Dim arrayButacas As Array
        arrayButacas = Split(butacas, ",")
        cantidad = Val(arrayButacas.Length)
        Dim j As Integer = 0
        While j <= cantidad - 1
            Dim consulta As String = "select resdeta.* from Boletos.reservaciondetalle resdeta inner join Boletos.reservacion res" +
                                     " where resdeta.id_reservacion = res.id_reservacion" +
                                     " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                     " and resdeta.estadobutaca='reservado'" +
                                     " and resdeta.numerobutaca=" + arrayButacas(j) + ""
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                Try
                    connection.Open()
                    Dim cmd As New MySqlCommand(consulta, connection)
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        Dim SqlUpdate As String = "update Boletos.reservaciondetalle resdeta" +
                                                 " inner join Boletos.reservacion res" +
                                                 " set estadobutaca='libre'" +
                                                 " where(resdeta.id_reservacion = res.id_reservacion)" +
                                                 " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                                 " and resdeta.estadobutaca='reservado'" +
                                                 " and resdeta.numerobutaca=" + arrayButacas(j) + ""
                        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                            Dim comando As MySqlCommand
                            Try
                                conn.Open()
                                comando = New MySqlCommand(SqlUpdate, conn)
                                comando.ExecuteNonQuery()
                                comando.Dispose()
                            Catch ex As MySqlException
                                'MsgBox(ex.Message)
                            End Try
                        End Using
                    End If
                Catch ex As Exception
                End Try
            End Using
            j = j + 1
        End While
        Return "Edición Correcta"
    End Function

    <WebMethod()> _
    Public Function anularReservacion(ByVal Asientos As String, ByVal valores As String, ByVal fecha As String) As String
        ' MsgBox("** anularReservacion")
        Dim parametros As Array
        parametros = Split(valores, "|")
        Dim asiento As Array
        asiento = Split(Asientos, ",")
        Dim arrayfecha As Array
        arrayfecha = Split(fecha, "/")
        Dim formatFecha As String = arrayfecha(2) + arrayfecha(1) + arrayfecha(0)
        Dim cantidad As Integer = Val(asiento.Length)
        Dim i As Integer = 0
        While i <= cantidad - 1
            Dim consulta As String = "select resdeta.* from Boletos.reservaciondetalle resdeta inner join Boletos.reservacion res" +
                                     " where resdeta.id_reservacion = res.id_reservacion" +
                                     " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                     " and resdeta.estadobutaca='reservado'" +
                                     " and resdeta.numerobutaca=" + asiento(i) + ""
            Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                Try
                    connection.Open()
                    Dim cmd As New MySqlCommand(consulta, connection)
                    Dim reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.HasRows Then
                        Dim SqlUpdate As String = "update Boletos.reservaciondetalle resdeta" +
                                 " inner join Boletos.reservacion res" +
                                 " set estadobutaca='libre'" +
                                 " where(resdeta.id_reservacion = res.id_reservacion)" +
                                 " and res.id_hora=" + parametros(0) + " and res.ruta=" + parametros(1) + " and res.empresa=" + parametros(2) + " and res.fechaviaje='" + formatFecha + "'" +
                                 " and resdeta.estadobutaca='reservado'" +
                                 " and resdeta.numerobutaca=" + asiento(i) + ""
                        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
                            Dim comando As MySqlCommand
                            Try
                                conn.Open()
                                comando = New MySqlCommand(SqlUpdate, conn)
                                comando.ExecuteNonQuery()
                                comando.Dispose()
                            Catch ex As MySqlException
                            End Try
                        End Using
                    End If
                Catch ex As Exception
                    'MsgBox(ex.Message)
                End Try
            End Using
            i = i + 1
        End While
        Return "Edición Correcta"
    End Function

    <WebMethod()> _
    Public Function liberarButaca(ByVal butaca As String, ByVal fecha As String, ByVal origen As String, ByVal destino As String, ByVal hora As String) As String
        Dim fechaformat As Array = Split(fecha, "/")
        fecha = fechaformat(2) + fechaformat(1) + fechaformat(0)
        Dim idhora As Array = Split(hora, "|")
        Dim SqlUpdate As String = "update Boletos.boleto set numerobutaca=null where fechaviaje='" + fecha + "' and origen='" + origen + "' and destino='" + destino + "' and numerobutaca='" + butaca + "'"
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Dim comando As MySqlCommand
            Try
                conn.Open()
                comando = New MySqlCommand(SqlUpdate, conn)
                comando.ExecuteNonQuery()
                comando.Dispose()
            Catch ex As MySqlException
                'MsgBox(ex.Message)
            End Try
        End Using
        Return "Edición Correcta"
    End Function

    <WebMethod()> _
    Public Function verificarBoleto(ByVal boleto As String) As List(Of [claseBoleto])
        Dim Consulta As String = "SELECT numerobutaca,estadoboleto,total,credito FROM Boletos.boleto where id_boleto=" & boleto & "  order by id_boleto desc limit 1;"
        Dim result As List(Of [claseBoleto]) = New List(Of claseBoleto)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New claseBoleto
                    If Not IsDBNull(reader("numerobutaca")) Or reader("estadoboleto") = "VALIDO" Then
                        Elemento.estado = "sin asignar butaca"
                        If Not IsDBNull(reader("credito")) Then
                            Elemento.precio = reader("credito")
                        Else
                            Elemento.precio = reader("total")
                        End If
                    Else
                        Elemento.estado = "ERROR"
                        Elemento.precio = reader("total")
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

    <WebMethod()> _
    Public Function DeterminaUsuario(ByVal usuario As String, ByVal pass As String) As String
        '  MsgBox("** DeterminaUsuario")
        Dim Consulta As String = "select id_tipoU from Facturacion.usuario where id_usr='" + usuario + "' and pass_usr='" + pass + "'"
        Dim result As String = ""
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While reader.Read()
                    result = reader("id_tipoU")
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
            End Try
        End Using
        Return result
    End Function

    <WebMethod()> _
    Public Function MostrarDestinosRuta(ByVal Ruta As String, ByVal Origen As String) As List(Of [DestinosRuta])
        'MsgBox("** MostrarDestinosRuta")
        Dim Consulta As String = "call sp_mostrar_destinos_ruta(" & Ruta & "," & Origen & ")"
        Dim result As List(Of [DestinosRuta]) = New List(Of DestinosRuta)
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(Consulta, connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                While (reader.Read())
                    Dim Elemento As New [DestinosRuta]
                    Elemento.id = reader("id_destino")
                    Elemento.nombre = reader("nom_destino")
                    Elemento.sugerencia = reader("sugerencia")
                    result.Add(Elemento)
                End While
                reader.Close()
                cmd.Dispose()
            Catch ex As MySqlException
                'MsgBox(ex.Message)
            Finally
                connection.Close()
            End Try
        End Using
        Return result
    End Function

    Public Class ClaseVehiculo
        Public asientonumero As String
        Public asientoestado As String
        Public cliente As String
        Public viaje As String
        Public total As Double
        Public cantidadasientos As Integer
    End Class

    Public Class ClaseHora
        Public datos As String
        Public hora As String
        Public estado As String
        Public empresa As String
    End Class

    Public Class ClaseOrigen
        Public iddestino As Integer
        Public nombredestino As String
    End Class

    Public Class ClaseFechaHora
        Public fecha As String
        Public hora As String
    End Class

    Public Class DatosFecha
        Public fecha As String
        Public hora As String
    End Class

    Public Class ClaseCliente
        Public nitcliente As String
        Public nombrecliente As String
        Public direccioncliente As String
    End Class

    Public Class ClaseViajerofrecuente
        Public nombreviajero As String
        Public direccionviajero As String
        Public descuentoviajero As String
        Public nitviajero As String
    End Class

    Public Class ClassActualizar
        Public cantidad As String
    End Class

    Public Class claseBoleto
        Public precio As Double
        Public estado As String
    End Class

    Public Class DestinosRuta
        Public id As String
        Public nombre As String
        Public sugerencia As String
    End Class
End Class