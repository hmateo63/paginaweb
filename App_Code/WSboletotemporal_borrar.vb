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
' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WSboletotemporal
    Inherits System.Web.Services.WebService


    <WebMethod()> _
    Public Function GuardaBoletoyAnula(ByVal asientonuevo As String, ByVal boletoanular As String, ByVal Datos As String, ByVal DatosFactura As String) As String
        Dim CorrelativoFactura As Long = 0
        Dim Arreglo() As String = Datos.Split("|")
        Dim descuento As Double
        descuento = Val(Arreglo(21))

        Arreglo(18) = Arreglo(18) - descuento
        'Dim ArregloButaca() As String = Asientos.Split("|")
        Dim ArregloFactura() As String = DatosFactura.Split("|")
        Dim ArregloFacturaElectronica() As String
        Dim StrFechaViaje As String = ""
        Dim Origen As String = Arreglo(12).ToString
        Dim Destino As String = Arreglo(13).ToString
        Dim OrigenNemonico As String = ""
        Dim DestinoNemonico As String = ""
        Dim StrFecha As String = ""
        Dim NBoleto As Long
        Dim SQLInserta As String
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            connection.Open()
            Dim comando As New MySqlCommand
            Dim transaccion As MySqlTransaction
            transaccion = connection.BeginTransaction
            comando.Connection = connection
            comando.Transaction = transaccion

            '---------EJECUTAMOS PROCEDIMIENTO ALMACENADO QUE RETORNA DATOS NECESARIOS PARA INSERTS DE LA TRANSACCION
            'parametro _usuario y retorna caja, resolucion, serie, fecharesolucion, codigoagencia, Empresa, dispositivo nombreagencia, direccionagencia

            Dim ConsultaDatos As String = "call Facturacion.sp_retornadatos_usuario('" & Arreglo(8).ToString & "')"
            Dim Cmd As New MySqlCommand(ConsultaDatos, connection)
            Dim Lector As MySqlDataReader = Cmd.ExecuteReader()
            Lector.Read()
            Try
                If Lector.HasRows Then
                    Dim Resolucion As String = Lector("resolucion")
                    Dim FechaResolucion As String = Lector("fecharesolucion")
                    Dim Serie As String = Lector("serie")
                    Dim CodigoAgencia As String = Lector("codigoagencia")
                    Dim Agencia As String = Lector("agencia")
                    Dim DireccionAgencia As String = Lector("direccionagencia")
                    Dim Empresa As String = Lector("empresa")
                    Dim Caja As String = Lector("caja")
                    Dim Dispositivo As String = Lector("dispositivo")
                    Lector.Close()

                    '---------------EJECUTAMOS LA FUNCION QUE RETORNA ALGUNOS DATOS NECESARIOS PARA LA IMPRESION---------
                    'enviar origen,fecha,destino,ruta
                    'retorna nemonicoorigen,nemonico destino,fecha_actual con hora,
                    'fecha de viaje en formato personalizado,secuencia_origen,secuencia_destino,fecha formato factura electronica
                    Dim StrEjecuta As String = "select fn_retornadatos_impresion " +
                    "(" & Arreglo(1).ToString & ",'" & Arreglo(0).ToString & "'," +
                    "" & Arreglo(2).ToString & "," & Arreglo(17).ToString & ")"
                    comando.CommandText = StrEjecuta

                    Dim ConsultaDatosEn As String = comando.ExecuteScalar.ToString
                    Dim ArregloDatosEn() As String = ConsultaDatosEn.Split("|")
                    Dim NemonicoOrigen As String = ArregloDatosEn(0)
                    Dim NemonicoDestino As String = ArregloDatosEn(1)
                    StrFecha = ArregloDatosEn(2)
                    StrFechaViaje = ArregloDatosEn(3)
                    Dim StrSecuenciaOrigen As String = ArregloDatosEn(4)
                    Dim StrSecuenciaDestino As String = ArregloDatosEn(5)
                    Dim StrFechaFactura As String = ArregloDatosEn(6)

                    ''" & Arreglo(8).ToString & "' USUARIO
                    Dim NumeroFacTemporal As Integer = NumeroAleatorio()
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

                    '-----------------INSERT PARA LA TABLA FACTURA-----------------------------------
                    Dim StrInsertaFacEnca As String = "insert into Facturacion.factura " +
                    "(num_fac,serie_fac,fecha_fac,hora_fac,valor_fac," +
                    "nit_clt,nom_clt,id_agc," +
                    "id_usr,id_agenciacaja,kae,face," +
                    "recargo,efectivo,tarjetacredito,cheque,nimpresion) " +
                    "values " +
                    "(" & NumeroFacTemporal & ",'" & Serie & "',curdate(),curtime()," & Val(ArregloFactura(0)) & "," +
                    "'" & NitCliente & "','" & NombreCliente & "'," & CodigoAgencia & "," +
                    "'" & Arreglo(8).ToString & "'," & Caja & ",'pendiente','pendiente'," +
                    "" & Val(ArregloFactura(1)) & "," & Val(ArregloFactura(2)) & "," +
                    "" & Val(ArregloFactura(3)) & "," & Val(ArregloFactura(4)) & ",1)" 'FALTA INSERTAR DATO DE EMPRESA EN ESTA TABLA

                    comando.CommandText = StrInsertaFacEnca
                    comando.ExecuteNonQuery()

                    '----------------------EXTRAEMOS EL CORRELATIVO DEL NUMERO DE FACTURA 
                    comando.CommandText = "SELECT @@IDENTITY"
                    CorrelativoFactura = comando.ExecuteScalar

                    Dim SqlModificaBoleto As String = "update Boletos.boleto set estadobutaca='cambio' where id_boleto=" & boletoanular & ""
                    comando.CommandText = SqlModificaBoleto
                    comando.ExecuteNonQuery()

                    '-----------------------CONSULTAMOS LA DESCRIPCION DEL TIPO DE SERVICIO SELECCIONADO-------------------
                    Dim ConsultaServicio As String = "select descripcion from Boletos.servicio where idservicio=" & Arreglo(19) & " and id_empsa=" & Empresa & ""
                    comando.CommandText = ConsultaServicio
                    Dim Servicio As String = comando.ExecuteScalar

                    '------------------------CONSULTAMOS EL BUS ASIGNADO AL HORARIO---------------------------------'
                    Dim ConsultaBus As String = "select if(curdate()='" & Arreglo(0).ToString & "'," +
                    "(select id_veh from Encomiendas.horarios where " +
                    "fecha_detaHorario=curdate() and id_empsa=" & Empresa & " and id_ruta=" & Arreglo(17) & " limit 1),'---')"

                    comando.CommandText = ConsultaBus
                    'Elemento.descripcion = IIf(reader("descripcion") IsNot DBNull.Value, reader("descripcion"), "No se pudo cargar")

                    Dim Bus As String = IIf(comando.ExecuteScalar IsNot DBNull.Value, comando.ExecuteScalar, "")

                    '---------------------LECTURA DEL ARREGLO QUE CONTIENE LAS BUTACAS QUE SE ESTAN CREANDO--------------'
                    'For i = 0 To ArregloButaca.Length - 1
                    '--------------------INSERCION A LA TABLA BOLETO---------------------------------'
                    SQLInserta = "insert into Boletos.boleto (fechacreacion,fechaviaje,origen," +
                        "destino,nitcliente,seguro," +
                        "tipo_cobro,numerobutaca,estadobutaca,empresa," +
                        "agencia,usuario,estadofacturacion," +
                        "estadoboleto,saldo,total,Descuento,AutorizaDescuento,horario,ruta," +
                        "secuenciaorigen,secuenciadestino,idservicio) " +
                        "values " +
                        "(now(),'" & Arreglo(0).ToString & "'," & Arreglo(1).ToString & "," +
                        "" & Arreglo(2).ToString & ",'" & NitCliente & "'," +
                        "" & Arreglo(4).ToString & ",2," +
                        "" & asientonuevo & ",'ocupado'," & Arreglo(6).ToString & "," +
                        "" & Arreglo(7).ToString & ",'" & Arreglo(8).ToString & "','FACTURADO'," +
                        "'VALIDO',0," & Val(Arreglo(18)) & "," & descuento & ",'" & Arreglo(22) & "'," +
                        "" & Arreglo(11) & "," & Arreglo(17) & "," +
                        "" & StrSecuenciaOrigen & "," & StrSecuenciaDestino & "," & Arreglo(19) & ")"

                    comando.CommandText = SQLInserta
                    comando.ExecuteNonQuery()

                    '------------------------------EXTRAEMOS EL NUMERO DE BOLETO CREADO-----------------------------
                    comando.CommandText = "SELECT @@IDENTITY"
                    NBoleto = comando.ExecuteScalar

                    '-----------------------------INSERCION A LA TABLA DETALLE DE LA FACTURA------------------------'
                    Dim StrInsertaFacDeta As String = "insert into Facturacion.factura_detalle " +
                    "(num_fac,serie_fac,item,id_doc,valor) " +
                    "values (" & CorrelativoFactura & ",'" & Serie & "',2," & NBoleto & "," & Val(Arreglo(18)) & ")"
                    comando.CommandText = StrInsertaFacDeta
                    comando.ExecuteNonQuery()
                    'Next


                    '*******************************************Factura Electronica *********************************
                    Dim esp As String = " "
                    Dim outStr As String = ""
                    Dim NitFactura As String = NitCliente.Replace("-", "")
                    Dim parametros As String = Dispositivo + esp + NitFactura + esp + Serie + esp + _
                    ArregloFactura(0).ToString + esp + StrFechaFactura + esp + Resolucion + _
                    esp + Chr(34) + NombreCliente + Chr(34) + esp + FechaResolucion + esp + _
                    CodigoAgencia + esp + Chr(34) + DireccionCliente + Chr(34)

                    Dim ProcessProperties As New ProcessStartInfo
                    ProcessProperties.FileName = "C:\Program Files\Java\jre7\bin\java.exe "
                    ProcessProperties.Arguments = "-jar C:\temp\WSGFacE.jar " + Trim(parametros)
                    ProcessProperties.RedirectStandardOutput = True
                    ProcessProperties.UseShellExecute = False

                    Dim myProcess As Process = Process.Start(ProcessProperties)
                    myProcess.WaitForExit()
                    outStr = myProcess.StandardOutput.ReadToEnd()

                    Dim SaltoLinea As String = Chr(13)

                    ArregloFacturaElectronica = outStr.Split(SaltoLinea)

                    Dim Kae = ArregloFacturaElectronica(0)
                    Dim Face As String
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

                    Face = ArregloFacturaElectronica(1).Replace(SaltoLinea, "")

                    If Len(Face) > 1 Then
                        Face = Right(Face, Len(Face) - 1)
                    End If

                    Dim CorrelativoReal = Right(Face, 9)
                    Dim DTE As String = Left(Face, 5)


                    If Face.Length > 1 Then
                        '****************************************Si el wsgface retorna face hacemos un transaction.commit*******************
                        'If 1 = 1 Then
                        Dim StrActualizaFac As String = "update Facturacion.factura set num_fac=" & CorrelativoReal & ",kae='" & Kae & "',face='" & Face & "' " +
                        "where correlativo=" & CorrelativoFactura & ""    'serie_fac='" & Session("serie") & "' and 
                        comando.CommandText = StrActualizaFac
                        comando.ExecuteNonQuery()

                        Dim StrActualizaFacDeta As String = "update Facturacion.factura_detalle set num_fac=" & CorrelativoReal & " where num_fac=" & CorrelativoFactura & ""
                        comando.CommandText = StrActualizaFacDeta
                        comando.ExecuteNonQuery()

                        transaccion.Commit()

                        '******************************CREACION DEL ARCHIVO POS***********************************'

                        Dim ArchivoPos As String = "boleto" + CorrelativoReal.ToString + "-" + Serie.ToString + ".pos"

                        Dim ruta As String = Server.MapPath("~/privado/boletopos/" + ArchivoPos)
                        Dim TextFile As New FileInfo(ruta)
                        Dim Fichero As StreamWriter = TextFile.CreateText


                        Fichero.WriteLine(" ")
                        Fichero.WriteLine("#Guia#No")
                        Fichero.WriteLine("#Factura#Si")
                        Fichero.WriteLine("#Boleto#Si")
                        Fichero.WriteLine("#PaseDeAbordaje#Si")
                        Fichero.WriteLine("#BARCODE#" + NBoleto.ToString)
                        Fichero.WriteLine("#NombreEmpresa#Líneas Terrestres Guatemaltecas, S. A.")
                        Fichero.WriteLine("#NitEmpresa#599635-K")
                        Fichero.WriteLine("#NombreComercial#Litegua")
                        Fichero.WriteLine("#DireccionAgencia#" + DireccionAgencia)
                        Fichero.WriteLine("#SerieFactura#" + Serie)
                        Fichero.WriteLine("#NumeroFactura#" + CorrelativoReal)
                        Fichero.WriteLine("#SujetoAPagosTrimestrales#SEGUN ART. 72 LEY ISR, NO REALIZAR RETENCION")
                        Fichero.WriteLine("#NumeroResolucion#" + Resolucion)
                        Fichero.WriteLine("#FechaResolucion#" + FechaResolucion)
                        Fichero.WriteLine("#Face#" + Face)
                        Fichero.WriteLine("#Cae1#" + Cae1)
                        Fichero.WriteLine("#Cae2#" + Cae2)
                        Fichero.WriteLine("#NitCliente#" + NitClienteImpresion)
                        Fichero.WriteLine("#NombreCliente#" + NombreClienteImpresion)
                        Fichero.WriteLine("#DireccionCliente#" + DireccionClienteImpresion)
                        Fichero.WriteLine("#Cantidad1#1")
                        Fichero.WriteLine("#Descripcion1#Boletos")
                        Fichero.WriteLine("#Valor1#" + ArregloFactura(0).ToString)
                        Fichero.WriteLine("#Total#" + ArregloFactura(0).ToString)
                        Fichero.WriteLine("#NumeroAsiento#0")
                        Fichero.WriteLine("#Servicio#" + Servicio.ToString)
                        Fichero.WriteLine("#Reimpresion#-")
                        Fichero.WriteLine("#Correlativo#" + NBoleto.ToString)
                        Fichero.WriteLine("#DTE#" + DTE)
                        Fichero.WriteLine("#AtendidoPor#" + Arreglo(8).ToString)
                        Fichero.WriteLine("#BoletosCantidad#1")
                        ''---------DETALLE DE LOS BOLETOS QUE SE DESEAN IMPRIMIR---------------''
                        For i = 0 To 1
                            NBoleto = PadL(NBoleto, 9, "0")
                            Fichero.WriteLine("#NumeroBoleto#" + NBoleto.ToString)
                            Fichero.WriteLine("#Origen#" + NemonicoOrigen)
                            Fichero.WriteLine("#FechaViaje#" + StrFechaViaje)
                            Fichero.WriteLine("#HoraViaje#" + Arreglo(14).ToString)
                            Fichero.WriteLine("#Bus#" + Bus)
                            Fichero.WriteLine("#Pasajero#-")
                            Fichero.WriteLine("#Destino#" + NemonicoDestino)
                            Fichero.WriteLine("#NombreDestino#" + Arreglo(13).ToString)
                            Fichero.WriteLine("#FechaEmision#" + StrFecha)
                            Fichero.WriteLine("#Butaca#" + asientonuevo.ToString)
                            Fichero.WriteLine("#ValorSinSeguro#" + (Val(Arreglo(18)) - 3).ToString)
                            Fichero.WriteLine("#FinBoleto#SI")
                            ''Fichero.WriteLine("#DestinoAgencia#-")
                            ''Fichero.WriteLine("#DestinoAgenciaNombre#-")
                            ''Fichero.WriteLine("#TipoServicio#" + Servicio)


                            ''Fichero.WriteLine("#Servicio#" + Servicio)
                            ''Fichero.WriteLine("#Destinatario#-")
                            ''Fichero.WriteLine("#Remitente#-")
                            ''Fichero.WriteLine("#TelefonoDestinatario#-")
                            ''Fichero.WriteLine("#DireccionDestinatario#-")
                            ''Fichero.WriteLine("#TelefonoCliente#-")
                            ''Fichero.WriteLine("#TipoPago#CONTADO")
                            ''Fichero.WriteLine("#Paquetes#-")
                            ''Fichero.WriteLine("#Sobres#-")
                            ''Fichero.WriteLine("#CantidadDePiezas#-")
                            ''Fichero.WriteLine("#ValorGuia#-")


                        Next
                        ''---------FIN DETALLE DE LOS BOLETOS QUE SE DESEAN IMPRIMIR---------------''
                        Fichero.WriteLine(" ")
                        Fichero.Close()
                        '******************************FIN CREACION DEL ARCHIVO POS*******************************'

                        Return "downloadfiles.aspx?archivo=" & ArchivoPos & ""

                    Else
                        '****************************************Si el wsgface no retorna face hacemos un transaction.rollback ***************
                        transaccion.Rollback()
                        Return "ERROR:" + "outStr"
                    End If

                Else
                    Return "ERROR. No posee caja de cobro asignada"
                End If
            Catch ex As MySqlException
                transaccion.Rollback()
                SQLInserta = "ERROR." + ex.Message
                Return SQLInserta
                'Catch ex As System.IndexOutOfRangeException
                'Return "ERROR." + ex.Message
            Finally
                connection.Close()
                comando.Dispose()
                transaccion.Dispose()
            End Try
        End Using
    End Function

    <WebMethod()> _
    Public Function datosBoletocomprado(ByVal numerobutaca As Integer, ByVal idhorario As Integer, _
                                        ByVal idorigen As Integer, ByVal iddestino As Integer, _
                                        ByVal idempresa As Integer, ByVal idservicio As Integer, _
                                        ByVal fecha As String) As List(Of [Boleto])

        Dim SqlBusqueda As String = "select (select precio from Boletos.configuracion where idconfiguracion=1) as preciorecargo," +
        "b.id_boleto idboleto," +
        "DATE_FORMAT(b.fechaviaje,'%d/%m/%Y') fechaviaje,b.origen idorigen,b.destino iddestino,b.nitcliente,b.numerobutaca,b.estadobutaca," +
        "b.empresa,b.agencia,b.estadoboleto,b.total,b.horario idhorario,b.ruta, " +
        "o.nom_destino origen,d.nom_destino destino,s.idservicio," +
        "s.descripcion servicio,CAST(h.hora as char(10)) hora " +
        "from Boletos.boleto b " +
        "LEFT JOIN Encomiendas.destino o ON b.origen=o.id_destino " +
        "LEFT JOIN Encomiendas.destino d ON b.destino=d.id_destino " +
        "LEFT JOIN Boletos.servicio s ON b.idservicio=s.idservicio " +
        "LEFT JOIN Encomiendas.hora h ON b.horario=h.id_horario " +
        "where b.numerobutaca=" & numerobutaca & " AND " +
        "b.horario=" & idhorario & " AND b.origen=" & idorigen & " AND " +
        "b.destino=" & iddestino & " AND b.empresa=" & idempresa & " AND " +
        "b.fechaviaje='" & fecha & "'"
        'b.idservicio=" & idservicio & " AND 

        Dim result As List(Of [Boleto]) = New List(Of Boleto)()
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            Try
                connection.Open()
                Dim cmd As New MySqlCommand(String.Format(SqlBusqueda), connection)
                Dim reader As MySqlDataReader = cmd.ExecuteReader()
                reader.Read()
                Dim Elemento As New Boleto
                If reader.HasRows Then
                    Elemento.preciorecargo = reader("preciorecargo")
                    Elemento.idboleto = reader("idboleto")
                    Elemento.fechaviaje = reader("fechaviaje")
                    Elemento.idorigen = reader("idorigen")
                    Elemento.iddestino = reader("iddestino")
                    Elemento.origen = reader("origen")
                    Elemento.destino = reader("destino")
                    Elemento.idservicio = reader("idservicio")
                    Elemento.servicio = reader("servicio")
                    Elemento.nitcliente = reader("nitcliente")
                    Elemento.numerobutaca = reader("numerobutaca")
                    Elemento.estadobutaca = reader("estadobutaca")
                    Elemento.idempresa = reader("empresa")
                    Elemento.idagencia = reader("agencia")
                    Elemento.estadoboleto = reader("estadoboleto")
                    Elemento.total = reader("total")
                    Elemento.idhorario = reader("idhorario")
                    Elemento.horario = reader("hora")
                    Elemento.idruta = reader("ruta")
                    result.Add(Elemento)
                Else
                    Elemento.estadoboleto = "Numero de boleto invalido"
                    result.Add(Elemento)
                End If

            Catch ex As Exception
                Dim Elemento As New Boleto
                Elemento.estadoboleto = "Numero de boleto invalido"
            End Try
        End Using

        Return result

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


    Public Function NumeroAleatorio() As Integer
        Dim LimiteMinimo As Integer = 1
        Dim LimiteMaximo As Integer = 9999
        Dim Semilla As Integer = CInt(Date.Now.Millisecond)
        Dim NumberRandom As New Random(Semilla)
        Dim Aleatorio As Integer = NumberRandom.Next(LimiteMinimo, LimiteMaximo)
        Return Aleatorio
    End Function

    Public Class Boleto
        Public preciorecargo As Double
        Public idboleto As Integer
        Public fechaviaje As String
        Public idhorario As Integer
        Public horario As String
        Public idservicio As Integer
        Public servicio As String
        Public total As Double
        Public idorigen As Integer
        Public iddestino As Integer
        Public origen As String
        Public destino As String
        Public nitcliente As String
        Public estadobutaca As String
        Public numerobutaca As Integer
        Public idempresa As Integer
        Public idagencia As Integer
        Public estadoboleto As String
        Public idruta As Integer

    End Class

End Class