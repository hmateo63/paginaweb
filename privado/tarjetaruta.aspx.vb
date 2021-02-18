Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Configuration
Imports MySql.Data.MySqlClient

Partial Class privado_tarjetaruta
    Inherits System.Web.UI.Page

    Dim Accion As Integer = 0
    Dim codigopagina As Integer = 3

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Accion = 1

        If Session("id_usr") = "" Then
            FormsAuthentication.RedirectToLoginPage()
        Else
            husuario.Value = Session("id_usr")
            hagencia.Value = Session("id_agc")

            'MsgBox(Session("FacturaElectronica"))



            Session("Especial") = "sin bus"

            If DatosSql.permiso(husuario.Value, Accion, codigopagina) = 0 Then
                Response.Redirect("error.html")
            End If
        End If
    End Sub

End Class