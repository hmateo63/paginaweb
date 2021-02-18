Imports System.Data
Imports MySql.Data.MySqlClient
Partial Class login
    Inherits System.Web.UI.Page



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Clear()
        TxtUser.Focus()
    End Sub



    Protected Sub BtnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnLogin.Click
        Try
            Dim str As String

            Dim Adaptador As New MySqlDataAdapter
            Dim TablaUsuario As New DataTable
            Using conexion As New MySqlConnection()
                conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
                conexion.Open()
                Dim comando As MySqlCommand
                comando = conexion.CreateCommand
                comando.CommandType = Data.CommandType.Text
                comando.Parameters.Add("@id_usr", MySqlDbType.VarChar, 15).Value = TxtUser.Value
                comando.Parameters.Add("@pass_usr", MySqlDbType.VarChar, 15).Value = TxtPass.Value

                ''str = "select * from Facturacion.usuario where id_usr = @id_usr and pass_usr=md5(md5(@pass_usr)) and status_usr=1"

                str = "call Facturacion.sp_usuarios_acceso('" & TxtUser.Value & "' , '" & TxtPass.Value & "');"


                comando.CommandText = str
                Adaptador.SelectCommand = comando
                Adaptador.Fill(TablaUsuario)
                If TablaUsuario.Rows.Count > 0 Then

                    Session("id_usr") = TablaUsuario.Rows(0).Item("id_usr")
                    Session("usuario") = TablaUsuario.Rows(0).Item("nom_usr")
                    Session("id_agc") = TablaUsuario.Rows(0).Item("id_agc")
                    Session("id_empsa") = TablaUsuario.Rows(0).Item("id_empsa")
                    Session("perfil") = TablaUsuario.Rows(0).Item("id_tipoU")

                    Session("FacturaElectronica") = TablaUsuario.Rows(0).Item("FacturaElectronica")

                    If TablaUsuario.Rows(0).Item("id_empsa") = 1 Then
                        Session("FacturaElectronica") = False


                    End If

                    'MsgBox(Session("FacturaElectronica"))

                    Dim TestString As String = TablaUsuario.Rows(0).Item("UrlImagen")

                    Dim aString As String = Replace(TestString, "imagenes", "../images")

                    Session("UrlImagen") = aString

                    Session("carouselImagen") = "img_litegua.jpg"
                    Session("carouselEmpresa") = "Lineas Terrestres Guatemaltecas"
                    Session("carouselMensaje") = "Guatemala, Puerto Barrios, Santo Tomas de Castilla, Rio Dulce"


                    If Session("id_empsa") = 4 Then
                        Session("carouselImagen") = "img_vilma.jpg"
                        Session("carouselEmpresa") = "Transportes Vilma"
                        Session("carouselMensaje") = "Chiquimula, Jocotan, El Florido"
                    End If









                    comando.Dispose()

                    Dim tablaAgencia, tablaLogueo As DataTable
                    tablaAgencia = DatosSql.cargar_datatable("select nom_agc,nombre_comercial,dire_agc,id_dispositivo as dispositivo from Facturacion.agencia " +
                    "where id_agc = " & Session("id_agc") & "")

                    tablaLogueo = DatosSql.cargar_datatable("SELECT id_usr,id_agc,fecha,hora FROM Encomiendas.log_registrologueos " +
                    "where id_usr='" & Session("id_usr") & "' and fecha=curdate()")
                    comando.CommandText = "insert into log_registrologueos(id_usr,id_agc,fecha,hora) values('" & Session("id_usr") & "','" & Session("id_agc") & "',curdate(),curtime())"
                    comando.ExecuteScalar()
                    comando.Dispose()


                    If tablaAgencia.Rows.Count > 0 Then
                        Session("agencia") = tablaAgencia.Rows(0).Item("nombre_comercial")
                        Session("dire_agc") = tablaAgencia.Rows(0).Item("dire_agc")
                        Session("dispositivo") = tablaAgencia.Rows(0).Item("dispositivo")
                    End If

                    Dim tablaEmpresa As DataTable
                    tablaEmpresa = DatosSql.cargar_datatable("select nom_empsa from Facturacion.empresa where id_empsa = " & Session("id_empsa") & "")

                    If tablaEmpresa.Rows.Count > 0 Then
                        Session("empresa") = tablaEmpresa.Rows(0).Item("nom_empsa")
                    End If
                    Dim TablaCajaAsignada As DataTable
                    'ESTO ES PARA DETERMINAR SI EL Facturacion.usuario TIENE CAJA ASIGNADA
                    Dim StrCajaAsignada As String = "select ac.resolucion,ac.serie,ac.id_agenciacaja," +
                    "a.nom_agc, ac.desc_agenciacaja,DATE_FORMAT(fecha_resolucion,'%Y%m%d') AS fecha_resol " +
                    "from Facturacion.agenciacaja ac " +
                    "INNER JOIN Facturacion.agencia a ON ac.id_agc=a.id_agc " +
                    "INNER JOIN Facturacion.agencia_asig_caja acc ON acc.id_agenciacaja= ac.id_agenciacaja " +
                    "WHERE id_usr='" & TxtUser.Value & "'"
                    TablaCajaAsignada = DatosSql.cargar_datatable(StrCajaAsignada)
                    If TablaCajaAsignada.Rows.Count > 0 Then
                        Session("resolucion") = TablaCajaAsignada.Rows(0).Item("resolucion")
                        Session("serie") = TablaCajaAsignada.Rows(0).Item("serie")
                        Session("fecha_resolucion") = TablaCajaAsignada.Rows(0).Item("fecha_resol")
                        Session("caja_agencia") = TablaCajaAsignada.Rows(0).Item("id_agenciacaja").ToString
                    Else
                        Session("resolucion") = ""
                        Session("serie") = ""
                        Session("fecha_resolucion") = ""
                        Session("caja_agencia") = 0

                    End If
                    'INTEGRADO PARA EVITAR ERROR DE INSERT CUANDO SE CREA GUIA AL CREDITO


                    Dim TablaFecha As DataTable = DatosSql.cargar_datatable("select DATE_FORMAT(curdate(),'%Y%m%d') AS FechaActual")
                    If TablaFecha.Rows.Count > 0 Then
                        Session("FechaActual") = TablaFecha.Rows(0).Item("FechaActual")
                    End If
                    FormsAuthentication.RedirectFromLoginPage(TxtUser.Value, False)
                Else
                    LblEstado.Text = "Datos incorrectos"
                End If
            End Using
        Catch ex As MySqlException
            LblEstado.Text = "Sucedio un error: " + ex.Message
        Finally
            'conexion.Close()
            'conexion = Nothing
        End Try
    End Sub

End Class
