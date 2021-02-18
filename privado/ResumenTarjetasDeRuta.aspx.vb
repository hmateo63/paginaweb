Imports System.Data
Imports iTextSharp.text
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Web.Services
Imports iTextSharp.text.pdf
Imports System.Configuration
Imports iTextSharp.text.html.simpleparser

Imports System.Web.UI.HtmlControls
Imports System.Text
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System
Imports System.Web.Security
Imports System.Web.UI.WebControls.WebParts

Partial Class privado_ResumenTarjetasDeRuta
    Inherits System.Web.UI.Page

    Dim Accion As Integer = 0
    Dim codigopagina As Integer = 3

    Dim filas As GridViewRow

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Accion = 1
        Dim strsql As String = ""
        If Session("id_usr") = "" Then
            FormsAuthentication.RedirectToLoginPage()
        Else
            husuario.Value = Session("id_usr")
            hagencia.Value = Session("id_agc")
            If DatosSql.permiso(husuario.Value, Accion, codigopagina) = 0 Then
                Response.Redirect("error.html")
            End If
        End If

        If (Not IsPostBack()) Then
            cargar_Combo(DropAgencia, "select 0 id_agencia,  'TODAS' nombre_agencia    union select id_agc as id_agencia,nom_agc as nombre_agencia from Facturacion.agencia where id_empsa=" & Session("id_empsa") & " order by id_agencia;")
            cargar_Combo(DropVehiculos, "select 0 id_vehiculo, 'TODOS' nombre_vehiculo union select id_veh as id_vehiculo,id_veh as nombre_vehiculo from Encomiendas.vehiculo where id_veh REGEXP '^[0-9]*[.]?[0-9]+$' and id_empsa=" & Session("id_empsa") & "   order by id_vehiculo;")
            cargar_Combo(DropPilotos, "select 0 as id_piloto, 'TODOS' nombre_piloto    union select id_emp as id_piloto, nom_emp as nombre_piloto from Encomiendas.empleado where id_pso=1  and id_empsa=" & Session("id_empsa") & " order by id_piloto;")
            cargar_Combo(DropServicios, "SELECT 0 as id_servicio ,'TODOS' nombre_servicio      union select idservicio as id_servicio,descripcion as nombre_servicio from Boletos.servicio where  id_empsa=" & Session("id_empsa") & " order by id_servicio;")

        End If

    End Sub

    Public Shared Sub cargar_Combo(ByVal ComboBox As DropDownList, ByVal strsql As String)
        Using conexion As New MySqlConnection
            'conexion = New MySqlConnection()
            conexion.ConnectionString = cadenaCon()
            Try
                conexion.Open()
                Dim comando As New MySqlCommand(strsql, conexion)
                Dim da As New MySqlDataAdapter(comando)
                Dim ds As New System.Data.DataSet
                da.Fill(ds)
                ComboBox.DataSource = ds.Tables(0)
                ComboBox.DataValueField = ds.Tables(0).Columns(0).Caption.ToString
                ComboBox.DataTextField = ds.Tables(0).Columns(1).Caption.ToString
                ComboBox.DataBind()
                ComboBox.SelectedIndex = -1
                comando.Dispose()
                da.Dispose()
            Catch ex As MySqlException
            Finally
                conexion.Close()
            End Try
        End Using
    End Sub
    Shared Function cadenaCon() As String
        Return System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
    End Function


End Class
