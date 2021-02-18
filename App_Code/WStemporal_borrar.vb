Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports MySql.Data.MySqlClient
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports AjaxControlToolkit

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WStemporal
    Inherits System.Web.Services.WebService
    <WebMethod(True)> _
    Public Function Tramos(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim StrTramos As String = "select rt.id_tramosunion,CONCAT(s.nom_destino,' | ',d.nom_destino) as destinos" +
                                    " from ruta_tramosunion rt" +
                                    " INNER JOIN destino s ON rt.id_sale=s.id_destino" +
                                    " INNER JOIN destino d ON rt.id_llega=d.id_destino" +
                                    " order by destinos"

            Dim comm As New MySqlCommand(StrTramos, conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("destinos").ToString(), dr("id_tramosunion").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Origen(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()

            Dim StrBusqueda As String = "(select a.id_destino as codigodestino,d.nom_destino as destino " +
                            "from Facturacion.agencia a " +
                            "INNER JOIN destino d ON a.id_destino=d.id_destino " +
                            "where a.id_agc=1 order by d.nom_destino limit 1) " +
                             "UNION ALL " +
                            "(select id_destino as codigodestino,nom_destino as destino " +
                            "from destino " +
                            "where id_destino!=(select a.id_destino " +
                            "from Facturacion.agencia a " +
                            "INNER JOIN destino d ON a.id_destino=d.id_destino " +
                            "where a.id_agc=1) " +
                            "order by nom_destino asc limit 100)"

            Dim comm As New MySqlCommand(StrBusqueda, conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("destino").ToString(), dr("codigodestino").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function
    <WebMethod(True)> _
    Public Function Empresa(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT id_empsa, nom_empsa FROM empresa order by nom_empsa", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_empsa").ToString(), dr("id_empsa").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Agencia(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_agc,nom_agc from Facturacion.agencia order by nom_agc", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_agc").ToString(), dr("id_agc").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Puesto(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_pso,desc_pso from puesto order by desc_pso", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("desc_pso").ToString(), dr("id_pso").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Piloto(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_emp,nom_emp from empleado where id_pso=1 order by nom_emp", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_emp").ToString(), dr("id_emp").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Asistente(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_emp,nom_emp from empleado where id_pso=2 order by nom_emp", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_emp").ToString(), dr("id_emp").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Departamento(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT id_deptoD, nom_deptoD FROM departamentod order by nom_deptoD", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_deptoD").ToString(), dr("id_deptoD").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Municipio(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Dim id_depto As Integer
        CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues)
        Dim kv As StringDictionary = CascadingDropDown.ParseKnownCategoryValuesString(knownCategoryValues)
        If Not kv.ContainsKey("Departamento") Or Not Int32.TryParse(kv("Departamento"), id_depto) Then
            Throw New ArgumentException("No se encontro ningun municipio")
        End If

        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT id_mpioD, nom_mpioD FROM municipioD where id_deptoD=" & id_depto & " order by nom_mpioD", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_mpioD").ToString(), dr("id_mpioD").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function HoraBoleto(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand("select h.id_detaHorario as codigohorario," +
                                         "hora.hora as hora " +
                                        "from horarios h " +
                                        "INNER JOIN hora ON h.id_horario=hora.id_horario " +
                                        "INNER JOIN ruta r ON h.id_ruta=r.id_ruta " +
                                        "where h.fecha_detaHorario = curdate() and r.id_agc=17", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)

            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("hora").ToString(), dr("codigohorario").ToString()))
            End While

            dr.Close()
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function FormaPago(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT id_tipoP, desc_tipoP FROM Facturacion.tipo_pago order by desc_tipoP", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("desc_tipoP").ToString(), dr("id_tipoP").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Destino(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "SELECT id_destino, nom_destino FROM destino order by nom_destino", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_destino").ToString(), dr("id_destino").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Ruta(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_ruta,nom_ruta from ruta where tipo_ruta=1 order by nom_ruta", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("nom_ruta").ToString(), dr("id_ruta").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

    <WebMethod(True)> _
    Public Function Horas(ByVal knownCategoryValues As String, ByVal category As String) As CascadingDropDownNameValue()
        Using conn As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)
            conn.Open()
            Dim comm As New MySqlCommand( _
            "select id_horario,hora from hora order by hora", conn)
            Dim dr As MySqlDataReader = comm.ExecuteReader()
            Dim l As New List(Of CascadingDropDownNameValue)
            While (dr.Read())
                l.Add(New CascadingDropDownNameValue(dr("hora").ToString(), dr("id_horario").ToString()))
            End While
            conn.Close()
            Return l.ToArray()
        End Using
    End Function

End Class