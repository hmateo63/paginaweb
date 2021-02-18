Imports Microsoft.VisualBasic
Imports System.Data.Odbc
Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class DatosSql

    Shared Function Sql() As String
        Return System.Configuration.ConfigurationManager.ConnectionStrings("ConeccionSQL").ConnectionString
    End Function

    Public Shared Function permiso(ByVal usuario As String, ByVal accion As Integer, ByVal codigopagina As Integer) As Integer
        Dim resultado As Integer = 0
        Using connection As New MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString)

            Dim comando As MySqlCommand
            Dim reader As MySqlDataReader

            Try
                connection.Open()
                comando = New MySqlCommand("select fn_permiso('" & usuario & "'," & accion & "," & codigopagina & ") valor", connection)
                reader = comando.ExecuteReader
                reader.Read()
                If reader.HasRows Then
                    If reader("valor") = 0 Then
                        reader.Close()
                        resultado = 0
                    ElseIf reader("valor") = 1 Then
                        reader.Close()
                        resultado = 1
                    End If
                Else
                    resultado = 0
                End If
            Catch ex As MySqlException
                resultado = 1
            End Try
        End Using
        Return resultado
    End Function

    Public Shared Function cargar_datatable(ByVal strsql As String) As System.Data.DataTable
        Using conexion As New MySqlConnection
            conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
            Dim ds As New System.Data.DataTable
            Try
                conexion.Open()
                Dim comando As New MySqlCommand(strsql, conexion)
                'comando.CommandTimeout = 500
                Dim da As New MySqlDataAdapter(comando)

                da.Fill(ds)
                comando.Dispose()
                da.Dispose()


            Catch ex As MySqlException

            End Try
            Return ds
        End Using
    End Function

    Public Shared Function cargar_cuadrecaja(ByVal strsql As String) As System.Data.DataTable

        Using conexion As New MySqlConnection
            conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
            Dim ds As New System.Data.DataTable
            Try
                conexion.Open()
                Dim comando As New MySqlCommand(strsql, conexion)
                comando.CommandTimeout = 100
                Dim da As New MySqlDataAdapter(comando)

                da.Fill(ds)
                comando.Dispose()
                da.Dispose()

            Catch ex As MySqlException

            End Try
            Return ds
        End Using
    End Function

    Public Shared Sub cargar_Grid(ByVal Datagridview As GridView, ByVal strsql As String)
        Using conexion As New MySqlConnection
            'conexion = New MySqlConnection()
            conexion.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString
            Try
                conexion.Open()
                Dim comando As New MySqlCommand(strsql, conexion)
                Dim da As New MySqlDataAdapter(comando)
                Dim ds As New System.Data.DataSet
                da.Fill(ds)
                Datagridview.DataSource = ds.Tables(0)
                Datagridview.AutoGenerateColumns = True
                Datagridview.DataBind()
                comando.Dispose()
                da.Dispose()

            Catch ex As MySqlException

            Finally
                'conexion.Close()
            End Try
        End Using
    End Sub

    Public Shared Function EjecutarSQL(ByVal strsql As String) As Boolean
        Using conexion As New SqlConnection
            Dim comando As SqlCommand
            Dim estado As String
            conexion.ConnectionString = Sql()
            Try
                conexion.Open()
                comando = New SqlCommand(strsql, conexion)
                estado = comando.ExecuteNonQuery

                If estado < 1 Then
                    Return False
                Else
                    Return True
                End If

            Catch ex As SqlException
                Return False
            Finally
                conexion.Close()
            End Try
        End Using
    End Function


    Public Shared Function BuscaSap(ByVal strSql As String) As Boolean
        Using conexion As New SqlConnection(Sql)
            'Dim comando As MySqlCommand
            Dim lector As SqlDataReader
            'conexion = New MySqlConnection()
            'conexion.ConnectionString = Sql()
            Try
                conexion.Open()
                Using comando As New SqlCommand(strSql, conexion)
                    lector = comando.ExecuteReader
                    While lector.Read()
                        Dim result As Boolean
                        'result = lector("Nit")
                        If result = False Then
                            BuscaSap = False
                        Else
                            BuscaSap = True
                        End If
                    End While
                    comando.Dispose()
                    lector.Close()
                End Using
            Catch ex As SqlException
                BuscaSap = False
            Finally
            End Try
        End Using
    End Function

    ''********************************************


    Shared Function cadenaCon() As String
        'Return "Data Source=unions1-pc\sql2008r2;Initial Catalog=dbLitegua;Integrated Security=True"

        '--------------Conexion Final-------------------------

        'Return "server=192.168.3.16;user id=UnionSystems;password=union01;database=Encomiendas;Connection Lifetime=120;"

        Return System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConString").ConnectionString


        '--------------Conexion De Pruebas al localhost-------------------------
        'Return "server=127.0.0.1;user id=UnionSystems;password=union01;database=Encomiendas;Connection Lifetime=120;"



        'Return "server=127.0.0.1;user id=root;password=root;database=Encomiendas;Connection Lifetime=120;"

        'Return "server=192.168.3.16;user id=UnionSystems;password=union01;database=Encomiendas;Max Pool Size=2;Min Pool Size=1;Pooling=true;Connection Lifetime=120;"


        'Return "server=192.168.3.16;user id=UnionSystems;password=union01;database=Encomiendas;Max Pool Size=2;Min Pool Size=1;Pooling=true;Connection Lifetime=120;"
        'Return "driver={SQL Server};" & _
        '"server=SOFTSERV9;database=DbRec;uid=moytho;pwd=ponchito1986"
    End Function

    'Shared Function cadenaConReplica() As String
    '    Return System.Configuration.ConfigurationManager.ConnectionStrings("EjemploConStringReplica").ConnectionString
    'End Function

    ''Shared Function Sql() As String
    ''    Return System.Configuration.ConfigurationManager.ConnectionStrings("ConeccionSQL").ConnectionString
    ''End Function





    'Public Shared Function EjecutaTransaccion(ByVal str1 As String, ByVal str2 As String) As Boolean
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion
    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            'ejecuto segundo comando sql
    '            comando.CommandText = str2
    '            comando.ExecuteNonQuery()
    '            transaccion.Commit()
    '            EjecutaTransaccion = True
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            EjecutaTransaccion = False
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function EjecutaTransaccionCliente(ByVal str1 As String, ByVal str2 As String) As Long
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion

    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            'ejecuto segundo comando sql
    '            comando.CommandText = str2
    '            comando.ExecuteNonQuery()
    '            comando.CommandText = "SELECT @@IDENTITY"
    '            Dim Valor As Long
    '            Valor = comando.ExecuteScalar
    '            transaccion.Commit()
    '            Return Valor
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            Return 0
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function EjecutaRemitente(ByVal str1 As String) As Long
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion
    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            comando.CommandText = "SELECT @@IDENTITY"
    '            Dim Valor As Long
    '            Valor = comando.ExecuteScalar
    '            transaccion.Commit()
    '            Return Valor
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            Return 0
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function Ejecuta3Transacciones(ByVal str1 As String, ByVal str2 As String, ByVal str3 As String) As Boolean
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion
    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            'ejecuto segundo comando sql
    '            comando.CommandText = str2
    '            comando.ExecuteNonQuery()
    '            comando.CommandText = str3
    '            comando.ExecuteNonQuery()
    '            transaccion.Commit()
    '            Ejecuta3Transacciones = True
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            Ejecuta3Transacciones = False
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function InsertaBodega(ByVal str1 As String) As Boolean
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion
    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            transaccion.Commit()
    '            InsertaBodega = True
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            InsertaBodega = False
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function ActualizaDatos(ByVal str1 As String) As Boolean
    '    Using conexion As New MySqlConnection
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Dim comando As New MySqlCommand
    '        Dim transaccion As MySqlTransaction
    '        transaccion = conexion.BeginTransaction
    '        comando.Connection = conexion
    '        comando.Transaction = transaccion
    '        Try
    '            'ejecuto primer comando sql
    '            comando.CommandText = str1
    '            comando.ExecuteNonQuery()
    '            transaccion.Commit()
    '            ActualizaDatos = True
    '        Catch ex As Exception
    '            transaccion.Rollback()
    '            ActualizaDatos = False
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '            transaccion.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function EjecutaSP(ByVal VarSale As Integer, ByVal VarLlega As Integer, ByVal VarSalida As Integer, ByVal VarDestino As Integer) As Long
    '    Using conexion As New MySqlConnection(cadenaCon())
    '        Dim comando As New MySqlCommand
    '        comando.Connection = conexion
    '        comando.CommandType = System.Data.CommandType.StoredProcedure
    '        comando.CommandText = "TotalKms"
    '        Dim ParamSale As New MySqlParameter
    '        ParamSale.ParameterName = "@id_salep"
    '        'cambio, le coloque 64
    '        ParamSale.MySqlDbType = MySqlDbType.Int64
    '        ParamSale.Value = VarSale
    '        comando.Parameters.Add(ParamSale)
    '        Dim ParamLlega As New MySqlParameter
    '        ParamLlega.ParameterName = "@id_llegap"
    '        ParamLlega.MySqlDbType = MySqlDbType.Int64
    '        ParamLlega.Value = VarLlega
    '        comando.Parameters.Add(ParamLlega)
    '        Dim ParamSalida As New MySqlParameter
    '        ParamSalida.ParameterName = "@id_salidap"
    '        ParamSalida.MySqlDbType = MySqlDbType.Int64
    '        ParamSalida.Value = VarSalida
    '        comando.Parameters.Add(ParamSalida)
    '        Dim ParamDestino As New MySqlParameter
    '        ParamDestino.ParameterName = "@id_destinop"
    '        ParamDestino.MySqlDbType = MySqlDbType.Int64
    '        ParamDestino.Value = VarDestino
    '        comando.Parameters.Add(ParamDestino)
    '        Dim da = New MySqlDataAdapter(comando)
    '        Dim ds As New System.Data.DataTable
    '        da.Fill(ds)
    '        Dim total As Long

    '        Try
    '            Dim strTotal As String
    '            strTotal = ds.Rows(0).Item("KmsTotal")
    '            If strTotal = "" Then total = 0 Else total = Val(strTotal)
    '            'total = ds.Rows(0).Item("KmsTotal")
    '            Return total
    '        Catch ex As Exception
    '            Return total = 0
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function EjecutaSql(ByVal strsql As String) As Boolean
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim estado As String
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            comando = New MySqlCommand(strsql, conexion)
    '            estado = comando.ExecuteNonQuery

    '            If estado < 1 Then
    '                Return False
    '            Else
    '                Return True
    '            End If

    '        Catch ex As MySqlException
    '            Return False
    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function


    ''Public Shared Function EjecutarSQL(ByVal strsql As String) As Boolean
    ''    Using conexion As New SqlConnection
    ''        Dim comando As SqlCommand
    ''        Dim estado As String
    ''        conexion.ConnectionString = Sql()
    ''        Try
    ''            conexion.Open()
    ''            comando = New SqlCommand(strsql, conexion)
    ''            estado = comando.ExecuteNonQuery

    ''            If estado < 1 Then
    ''                Return False
    ''            Else
    ''                Return True
    ''            End If

    ''        Catch ex As SqlException
    ''            Return False
    ''        Finally
    ''            conexion.Close()
    ''        End Try
    ''    End Using
    ''End Function


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
    ''Public Shared Function cargar_datatable(ByVal strsql As String) As System.Data.DataTable
    ''    Using conexion As New MySqlConnection
    ''        conexion.ConnectionString = cadenaCon()
    ''        Dim ds As New System.Data.DataTable
    ''        Try
    ''            conexion.Open()
    ''            Dim comando As New MySqlCommand(strsql, conexion)
    ''            'comando.CommandTimeout = 500
    ''            Dim da As New MySqlDataAdapter(comando)

    ''            da.Fill(ds)
    ''            comando.Dispose()
    ''            da.Dispose()

    ''        Catch ex As MySqlException
    ''        Finally
    ''            conexion.Close()
    ''        End Try
    ''        Return ds
    ''    End Using
    ''End Function
    ''Public Shared Function cargar_cuadrecaja(ByVal strsql As String) As System.Data.DataTable

    ''    Using conexion As New MySqlConnection
    ''        conexion.ConnectionString = cadenaConReplica()
    ''        Dim ds As New System.Data.DataTable
    ''        Try
    ''            strsql = " " + strsql
    ''            If strsql.Length > 2 Then

    ''                conexion.Open()
    ''                Dim comando As New MySqlCommand(strsql, conexion)
    ''                comando.CommandTimeout = 100
    ''                Dim da As New MySqlDataAdapter(comando)

    ''                da.Fill(ds)
    ''                comando.Dispose()
    ''                da.Dispose()

    ''            End If

    ''        Catch ex As MySqlException
    ''        Finally
    ''            conexion.Close()
    ''        End Try
    ''        Return ds
    ''    End Using
    ''End Function
    ''Public Shared Sub cargar_Grid(ByVal Datagridview As GridView, ByVal strsql As String)
    ''    Using conexion As New MySqlConnection
    ''        'conexion = New MySqlConnection()
    ''        conexion.ConnectionString = cadenaCon()
    ''        Try
    ''            conexion.Open()
    ''            Dim comando As New MySqlCommand(strsql, conexion)
    ''            Dim da As New MySqlDataAdapter(comando)
    ''            Dim ds As New System.Data.DataSet
    ''            da.Fill(ds)
    ''            Datagridview.DataSource = ds.Tables(0)
    ''            Datagridview.AutoGenerateColumns = True
    ''            Datagridview.DataBind()
    ''            comando.Dispose()
    ''            da.Dispose()

    ''        Catch ex As SqlException

    ''        Finally
    ''            conexion.Close()
    ''        End Try
    ''    End Using
    ''End Sub
    Public Shared Function busca(ByVal strSql As String) As Boolean
        Using conexion As New MySqlConnection(cadenaCon)
            'Dim comando As MySqlCommand
            Dim lector As MySqlDataReader
            'conexion = New MySqlConnection()
            conexion.ConnectionString = cadenaCon()
            Try
                conexion.Open()
                Using comando As New MySqlCommand(strSql, conexion)
                    lector = comando.ExecuteReader
                    lector.Read()
                    If lector.HasRows = False Then
                        busca = False
                    Else
                        busca = True
                    End If
                    comando.Dispose()
                    lector.Close()
                End Using
            Catch ex As MySqlException
                busca = False

            Finally
                conexion.Close()
            End Try
        End Using
    End Function

    'Public Shared Function busca2(ByVal strSql As String) As Boolean
    '    Using conexion As New MySqlConnection(cadenaCon)
    '        'Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            Using comando As New MySqlCommand(strSql, conexion)
    '                lector = comando.ExecuteReader
    '                While lector.Read()
    '                    Dim result As Integer
    '                    result = lector("uso_tarjeta_credito")
    '                    If result = 0 Then
    '                        busca2 = False
    '                    Else
    '                        busca2 = True
    '                    End If
    '                End While
    '                comando.Dispose()
    '                lector.Close()
    '            End Using
    '        Catch ex As MySqlException
    '            busca2 = False
    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    '    Return True
    'End Function

    ''Public Shared Function BuscaSap(ByVal strSql As String) As Boolean

    ''    Dim siBuscaSap As Boolean
    ''    siBuscaSap = False




    ''    Using conexion As New SqlConnection(Sql)
    ''        'Dim comando As MySqlCommand
    ''        Dim lector As SqlDataReader
    ''        'conexion = New MySqlConnection()
    ''        conexion.ConnectionString = Sql()
    ''        Try
    ''            conexion.Open()
    ''            Using comando As New SqlCommand(strSql, conexion)
    ''                lector = comando.ExecuteReader
    ''                While lector.Read()
    ''                    Dim result As Boolean
    ''                    'result = lector("Nit")
    ''                    If result = False Then
    ''                        BuscaSap = False
    ''                        siBuscaSap = False

    ''                    Else
    ''                        BuscaSap = True
    ''                        siBuscaSap = True

    ''                    End If
    ''                End While
    ''                comando.Dispose()
    ''                lector.Close()
    ''            End Using
    ''        Catch ex As SqlException
    ''            BuscaSap = False
    ''        Finally
    ''            conexion.Close()
    ''        End Try
    ''    End Using
    ''    Return siBuscaSap

    ''End Function


    'Public Shared Function RetornaClienteSap(ByVal strSql As String) As String
    '    Using coneccion As New SqlConnection
    '        Dim comando As SqlCommand
    '        Dim resultado As SqlDataReader
    '        coneccion.ConnectionString = Sql()
    '        Try
    '            coneccion.Open()
    '            comando = New SqlCommand(strSql, coneccion)
    '            resultado = comando.ExecuteReader
    '            resultado.Read()
    '            If resultado.HasRows = True Then
    '                RetornaClienteSap = resultado("Nit")
    '            Else
    '                RetornaClienteSap = "C/F"
    '            End If
    '            comando.Dispose()
    '            resultado.Close()
    '        Catch ex As Exception
    '            RetornaClienteSap = "C/F"
    '        Finally
    '            coneccion.Close()
    '        End Try
    '    End Using
    'End Function


    'Public Shared Function Retornatiposap(ByVal strSql As String) As String
    '    Using coneccion As New SqlConnection
    '        Dim comando As SqlCommand
    '        Dim resultado As SqlDataReader
    '        coneccion.ConnectionString = Sql()
    '        Try
    '            coneccion.Open()
    '            comando = New SqlCommand(strSql, coneccion)
    '            resultado = comando.ExecuteReader
    '            resultado.Read()
    '            Retornatiposap = resultado("tipo_cliente")

    '            comando.Dispose()
    '            resultado.Close()
    '        Catch ex As Exception
    '        Finally
    '            coneccion.Close()
    '        End Try
    '    End Using
    '    Return True
    'End Function


    Public Shared Function RetornaDataReader(ByVal strSql As String) As Integer
        Using conexion As New MySqlConnection

            Dim comando As MySqlCommand
            Dim lector As MySqlDataReader
            'conexion = New MySqlConnection()
            conexion.ConnectionString = cadenaCon()
            Try
                conexion.Open()
                comando = New MySqlCommand(strSql, conexion)
                lector = comando.ExecuteReader
                lector.Read()
                If lector.HasRows = True Then
                    RetornaDataReader = lector("id_webpagina")
                Else
                    RetornaDataReader = 0
                End If
                comando.Dispose()
                lector.Close()
            Catch ex As MySqlException
                RetornaDataReader = 0

            Finally
                conexion.Close()
            End Try
        End Using
    End Function






    'Public Shared Function RetornaProveedor(ByVal strSql As String) As Integer
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            comando = New MySqlCommand(strSql, conexion)
    '            lector = comando.ExecuteReader
    '            lector.Read()
    '            If lector.HasRows = True Then
    '                RetornaProveedor = lector("id_guia_proveedor")
    '            Else
    '                RetornaProveedor = 0
    '            End If
    '            comando.Dispose()
    '            lector.Close()
    '        Catch ex As MySqlException
    '            RetornaProveedor = 0

    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function



    'Public Shared Function RetornaMonto(ByVal strSql As String) As Integer
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            comando = New MySqlCommand(strSql, conexion)
    '            lector = comando.ExecuteReader
    '            lector.Read()
    '            If lector.HasRows = True Then
    '                RetornaMonto = lector("total_guia")
    '            Else
    '                RetornaMonto = 0
    '            End If
    '            comando.Dispose()
    '            lector.Close()
    '        Catch ex As MySqlException
    '            RetornaMonto = 0

    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function





    'Public Shared Function EjecutaStoredProcedure(ByVal strSql As String) As String
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            comando = New MySqlCommand(strSql, conexion)
    '            lector = comando.ExecuteReader
    '            lector.Read()
    '            If lector.HasRows = True Then
    '                EjecutaStoredProcedure = lector(0)
    '            Else
    '                EjecutaStoredProcedure = 0
    '            End If
    '            comando.Dispose()
    '            lector.Close()
    '        Catch ex As MySqlException
    '            EjecutaStoredProcedure = 0
    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function RetornaDataReaderDouble(ByVal strSql As String) As Double
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        Try
    '            conexion.Open()
    '            comando = New MySqlCommand(strSql, conexion)
    '            lector = comando.ExecuteReader
    '            lector.Read()
    '            If lector.HasRows = True Then
    '                RetornaDataReaderDouble = lector(0)
    '            Else
    '                RetornaDataReaderDouble = 0
    '            End If
    '            comando.Dispose()
    '            lector.Close()
    '        Catch ex As MySqlException
    '            RetornaDataReaderDouble = -1
    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function Maximo(ByVal strSql As String) As Integer
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        Dim lector As MySqlDataReader
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        Try
    '            comando = New MySqlCommand(strSql, conexion)
    '            lector = comando.ExecuteReader
    '            lector.Read()
    '            If lector.HasRows = False Then
    '                Maximo = 0
    '            Else
    '                Try
    '                    Maximo = lector.GetInt32(0) + 1
    '                Catch ex As Exception
    '                    Maximo = 1
    '                Finally
    '                    lector.Close()
    '                End Try
    '            End If
    '            comando.Dispose()
    '        Catch ex As MySqlException
    '            Maximo = 0
    '        Finally
    '            conexion.Close()
    '        End Try
    '    End Using
    'End Function
    'Public Shared Function ConvierteFecha(ByVal Fecha As String) As String
    '    Dim dia, mes, ano As String
    '    'RECIBE DE PRIMERO EL DIA, LUEGO EL MES Y POR ULTIMO EL AÑO
    '    Dim posicion, largo, otraposicion As Integer
    '    largo = Len(Fecha)
    '    posicion = InStr(1, Fecha, "/")
    '    otraposicion = InStr(posicion + 1, Fecha, "/")
    '    dia = Left(Fecha, posicion - 1)
    '    mes = Mid(Fecha, posicion + 1, otraposicion - posicion - 1)
    '    ano = Right(Fecha, 4)
    '    ConvierteFecha = ano + "-" + mes + "-" + dia
    '    Return ConvierteFecha
    'End Function
    'Public Shared Function FechaSinSignos(ByVal Fecha As String) As String
    '    'RECIBE DE PRIMERO EL DIA, LUEGO EL MES Y POR ULTIMO EL AÑO
    '    Dim dia, mes, ano As String
    '    Dim posicion, largo, otraposicion As Integer
    '    largo = Len(Fecha)
    '    posicion = InStr(1, Fecha, "/")
    '    otraposicion = InStr(posicion + 1, Fecha, "/")
    '    mes = Left(Fecha, posicion - 1)
    '    dia = Mid(Fecha, posicion + 1, otraposicion - posicion - 1)
    '    ano = Right(Fecha, 4)
    '    FechaSinSignos = ano + mes + dia
    '    Return FechaSinSignos
    'End Function
    'Public Shared Function ConvierteFechaDiaMesAno(ByVal Fecha As String) As String
    '    Dim dia, mes, ano As String
    '    Dim posicion, largo, otraposicion As Integer
    '    largo = Len(Fecha)
    '    posicion = InStr(1, Fecha, "/")
    '    otraposicion = InStr(posicion + 1, Fecha, "/")
    '    dia = Left(Fecha, posicion - 1)
    '    mes = Mid(Fecha, posicion + 1, otraposicion - posicion - 1)
    '    ano = Right(Fecha, 4)
    '    ConvierteFechaDiaMesAno = ano + "-" + mes + "-" + dia
    '    Return ConvierteFechaDiaMesAno
    'End Function
    'Public Shared Function EjecutaSqlYRegresa(ByVal strsql As String) As Long
    '    Using conexion As New MySqlConnection
    '        Dim comando As MySqlCommand
    '        'conexion = New MySqlConnection()
    '        conexion.ConnectionString = cadenaCon()
    '        conexion.Open()
    '        comando = New MySqlCommand(strsql, conexion)
    '        Try
    '            comando.ExecuteNonQuery()
    '            Dim Valor As Long
    '            comando.CommandText = "SELECT @@IDENTITY"
    '            Valor = comando.ExecuteScalar
    '            'cmd.CommandText = "SELECT @@IDENTITY";
    '            'int nId = (int)cmd.ExecuteScalar()
    '            Return Valor

    '        Catch ex As MySqlException
    '            Return 0
    '        Finally
    '            conexion.Close()
    '            comando.Dispose()
    '        End Try
    '    End Using
    'End Function

    Public Shared Function enteros(ByVal strSql As Object) As Integer
        Using conexion As New MySqlConnection
            Dim comando As MySqlCommand
            Dim numero As MySqlDataReader
            'conexion = New MySqlConnection()
            conexion.ConnectionString = cadenaCon()
            conexion.Open()
            Try
                comando = New MySqlCommand(strSql, conexion)
                numero = comando.ExecuteReader
                numero.Read()
                If numero.HasRows = True Then
                    enteros = numero.GetInt32(0)
                Else
                    enteros = 0
                End If
                comando.Dispose()
                numero.Close()
            Catch ex As MySqlException
                enteros = 0

            Finally
                conexion.Close()
            End Try
        End Using

    End Function









    ''**********************************************






















End Class