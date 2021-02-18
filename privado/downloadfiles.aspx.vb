
Partial Class downloadfiles
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim Archivo = Trim(Request.QueryString("archivo"))
        Dim Ruta As String = Trim(Request.QueryString("ruta"))

        If Archivo = "" Or Ruta = "" Then

            If Archivo = "" Then
                Archivo = "Vacio"
            End If
            If Ruta = "" Then
                Ruta = "Vacia"
            End If

            Response.Write("<p style=color:red>Error en la descarga del Archivo De impresion </p></br>")
            Response.Write("Ruta de Archivo:" + Ruta + "</br>")
            Response.Write("Nombre del Archivo:" + Archivo + "</br>")

        Else

            Dim rutaCompleta As String = Ruta + Archivo

            Dim strArchivo As Boolean = (System.IO.File.Exists(rutaCompleta))
            If strArchivo = True Then

                Response.ClearHeaders()
                Response.ClearContent()
                Response.ContentType = "application/pos"

                Dim DatoArchivo As String = "attachment; filename=" + Archivo
                Response.AddHeader("content-disposition", DatoArchivo)
                Response.WriteFile(rutaCompleta)

                ScriptManager.RegisterStartupScript(Me, Me.GetType, "cerrarventana", "window.close;", True)

            ElseIf strArchivo = False Then
                Response.Write("<p style=color:red>Error en la descarga del Archivo De impresion, El archivo no se creo correctamente</p>")
                Response.Write("El Archivo No Existe </br>")

            End If

        End If
        Dim rutaas As String = "cmd /C " + Ruta + "EscEliminar.bat " + Archivo
        Shell("cmd /C " + Ruta + "EscEliminar " + Archivo)

    End Sub
End Class
