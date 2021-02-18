
Partial Class privado_cerrarsesion
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        FormsAuthentication.SignOut()
        Session.Abandon()
        Response.Redirect("~/privado/tarjetaruta.aspx")
    End Sub
End Class
