Option Strict Off

Namespace USR_USP_CONSTITUENT_GETPHOTOS

    '*****************************************************************
    'Generated by SPCodeGen © 2003 Blackbaud CoreTech
    '*****************************************************************
#Region "Parameter Specification"
    'Name                         Type                Size        Direction
    '@RETURN_VALUE                int                             Return value
    '@IDSETREGISTERID             uniqueidentifier                In
#End Region

    'SP TYPE: Stored Procedure - multiple resultsets

    <System.CodeDom.Compiler.GeneratedCodeAttribute("SPCodeGen", "2.0.0.0")>
    <System.Serializable> Public NotInheritable Class SPResultCollection
        Inherits SPResultSetCollection

        Friend Sub New()
            MyBase.New(1)

        End Sub
        Public ReadOnly Property ResultSet As ResultRow()
            Get
                Return _results(0)
            End Get
        End Property
        Public ReadOnly Property ResultSet1 As ResultRow1()
            Get
                Return _results(1)
            End Get
        End Property
        Public Function Clone() As SPResultCollection
            Return CType(MyBase.CloneResult, SPResultCollection)
        End Function
    End Class

    <System.CodeDom.Compiler.GeneratedCodeAttribute("SPCodeGen", "2.0.0.0")>
    <System.Serializable()> Public NotInheritable Class ResultRow
        Inherits SPResultSetDataRow

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Public Enum ColumnList
            [ID]
            [PICTURE]
        End Enum

        Private Const MAX_COLUMN_INDEX As Integer = 1

        Public Sub New()
            Me.New(True)
        End Sub

        Friend Sub New(ByVal initDBNull As Boolean)
            MyBase.New(MAX_COLUMN_INDEX, initDBNull)
        End Sub

        Public Sub New(ByVal rdr As System.Data.SqlClient.SqlDataReader)
            ReDim _data(MAX_COLUMN_INDEX)
            rdr.GetValues(_data)
        End Sub

        Public ReadOnly Property ColumnIsDBNull(ByVal col As ColumnList) As Boolean
            Get
                Return ObjIsNull(_data(col))
            End Get
        End Property

        <System.Xml.Serialization.XmlAttribute()> Overloads Property [ID]() As System.Guid
            Get
                Dim o As Object = _data(ColumnList.ID)
                If (o Is DBNull.Value) Then Return System.Guid.Empty
                Return DirectCast(o, System.Guid)
            End Get

            Set(ByVal Value As System.Guid)
                _data(ColumnList.ID) = Value
            End Set
        End Property

        Overloads ReadOnly Property [ID](ByVal valueIfNull As System.Guid) As System.Guid
            Get
                Return MyBase.GetValueIfNull(ColumnList.ID, valueIfNull)
            End Get
        End Property


        Overloads Property [PICTURE]() As System.Byte()
            Get
                Dim o As Object = _data(ColumnList.PICTURE)
                If (o Is DBNull.Value) Then Return Nothing
                Return DirectCast(o, System.Byte())
            End Get

            Set(ByVal Value As System.Byte())
                _data(ColumnList.PICTURE) = Value
            End Set
        End Property

        Overloads ReadOnly Property [PICTURE](ByVal valueIfNull As System.Byte()) As System.Byte()
            Get
                Return MyBase.GetValueIfNull(ColumnList.PICTURE, valueIfNull)
            End Get
        End Property


    End Class
    <System.CodeDom.Compiler.GeneratedCodeAttribute("SPCodeGen", "2.0.0.0")>
    <System.Serializable()> Public NotInheritable Class ResultRow1
        Inherits SPResultSetDataRow

        <System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
        Public Enum ColumnList
            [PICTURE]
        End Enum

        Private Const MAX_COLUMN_INDEX As Integer = 0

        Public Sub New()
            Me.New(True)
        End Sub

        Friend Sub New(ByVal initDBNull As Boolean)
            MyBase.New(MAX_COLUMN_INDEX, initDBNull)
        End Sub

        Public Sub New(ByVal rdr As System.Data.SqlClient.SqlDataReader)
            ReDim _data(MAX_COLUMN_INDEX)
            rdr.GetValues(_data)
        End Sub

        Public ReadOnly Property ColumnIsDBNull(ByVal col As ColumnList) As Boolean
            Get
                Return ObjIsNull(_data(col))
            End Get
        End Property

        Overloads Property [PICTURE]() As System.Byte()
            Get
                Dim o As Object = _data(ColumnList.PICTURE)
                If (o Is DBNull.Value) Then Return Nothing
                Return DirectCast(o, System.Byte())
            End Get

            Set(ByVal Value As System.Byte())
                _data(ColumnList.PICTURE) = Value
            End Set
        End Property

        Overloads ReadOnly Property [PICTURE](ByVal valueIfNull As System.Byte()) As System.Byte()
            Get
                Return MyBase.GetValueIfNull(ColumnList.PICTURE, valueIfNull)
            End Get
        End Property


    End Class
    <System.CodeDom.Compiler.GeneratedCodeAttribute("SPCodeGen", "2.0.0.0")>
    Public Module WrapperRoutines

        Public Function CreateSQLCommand(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), Optional ByVal timeout As Integer = 0) As System.Data.SqlClient.SqlCommand

            Dim cmd As System.Data.SqlClient.SqlCommand = conn.CreateCommand
            If timeout = 0 Then
                cmd.CommandTimeout = Config.DefaultResultSetTimeout
            Else
                cmd.CommandTimeout = timeout
            End If
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "dbo.USR_USP_CONSTITUENT_GETPHOTOS"

            Dim p As System.Data.SqlClient.SqlParameter

            '@RETURN_VALUE
            p = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
            p.Direction = ParameterDirection.ReturnValue
            p.Value = 0


            '@IDSETREGISTERID
            If [IDSETREGISTERID].HasValue Then
                cmd.Parameters.AddWithValue("@IDSETREGISTERID", [IDSETREGISTERID].Value)
            Else
                cmd.Parameters.AddWithValue("@IDSETREGISTERID", System.DBNull.Value).SqlDbType = SqlDbType.uniqueidentifier
            End If

            Return cmd
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid)) As ResultRow
            Return ExecuteRow(conn, [IDSETREGISTERID], 0, 0, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), ByRef returnIntValue As Integer) As ResultRow
            Return ExecuteRow(conn, [IDSETREGISTERID], returnIntValue, 0, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), ByRef returnIntValue As Integer, ByVal timeout As Integer) As ResultRow
            Return ExecuteRow(conn, [IDSETREGISTERID], returnIntValue, timeout, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), Optional ByRef returnIntValue As Integer = 0, Optional ByVal timeout As Integer = 0, Optional ByVal tran As SQLClient.SQLTransaction = Nothing) As ResultRow

            Dim cmd As System.Data.SqlClient.SqlCommand = CreateSQLCommand(conn, [IDSETREGISTERID], timeout)

            If tran IsNot Nothing Then
                cmd.Transaction = tran
            End If

            Dim rdr As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader(CommandBehavior.SingleRow)

            If rdr.Read Then
                ExecuteRow = New ResultRow(rdr)
            Else
                ExecuteRow = Nothing
            End If

            rdr.Close()

            returnIntValue = DirectCast(cmd.Parameters.Item(0).Value, Integer)



        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid)) As SPResultCollection
            Return ExecuteSP(conn, [IDSETREGISTERID], 0, 0, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), ByRef returnIntValue As Integer) As SPResultCollection
            Return ExecuteSP(conn, [IDSETREGISTERID], returnIntValue, 0, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), ByRef returnIntValue As Integer, ByVal timeout As Integer) As SPResultCollection
            Return ExecuteSP(conn, [IDSETREGISTERID], returnIntValue, timeout, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection, ByVal [IDSETREGISTERID] As Nullable(Of System.Guid), Optional ByRef returnIntValue As Integer = 0, Optional ByVal timeout As Integer = 0, Optional ByVal tran As SQLClient.SQLTransaction = Nothing) As SPResultCollection

            Dim cmd As System.Data.SqlClient.SqlCommand
            cmd = CreateSQLCommand(conn, [IDSETREGISTERID], timeout)
            If tran IsNot Nothing Then
                cmd.Transaction = tran
            End If
            Dim rdr As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader

            Dim list As New System.Collections.ArrayList
            Dim res As New SPResultCollection

            Try

                'BEGIN ResultSet 0
                Do While rdr.Read()
                    list.Add(New ResultRow(rdr))
                Loop
                Dim a(list.Count - 1) As ResultRow
                list.CopyTo(a)
                res.InitResultSet(0, a)
                'END ResultSet 0


                'BEGIN ResultSet 1
                list.Clear()
                If rdr.NextResult Then
                    Do While rdr.Read()
                        list.Add(New ResultRow1(rdr))
                    Loop
                End If

                Dim a1(list.Count - 1) As ResultRow1
                list.CopyTo(a1)
                res.InitResultSet(1, a1)
                'END ResultSet 1



            Catch ex As Exception


                Throw

            Finally

                rdr.Close()

            End Try

            Dim returnObjectValue As Object = cmd.Parameters.Item(0).Value

            If Not IsDBNull(returnObjectValue) Then
                returnIntValue = CInt(returnObjectValue)
            Else
                returnIntValue = 0
            End If






            Return res
        End Function



        'Overload step 2
        'ParmCount=1

        Public Function CreateSQLCommand(ByVal conn As System.Data.SqlClient.SqlConnection, Optional ByVal timeout As Integer = 0) As System.Data.SqlClient.SqlCommand

            Dim cmd As System.Data.SqlClient.SqlCommand = conn.CreateCommand
            If timeout = 0 Then
                cmd.CommandTimeout = Config.DefaultResultSetTimeout
            Else
                cmd.CommandTimeout = timeout
            End If
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandText = "dbo.USR_USP_CONSTITUENT_GETPHOTOS"

            Dim p As System.Data.SqlClient.SqlParameter

            '@RETURN_VALUE
            p = cmd.Parameters.Add("@RETURN_VALUE", SqlDbType.Int)
            p.Direction = ParameterDirection.ReturnValue
            p.Value = 0

            Return cmd
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection ) As ResultRow
            Return ExecuteRow(conn , 0, 0, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection , ByRef returnIntValue As Integer) As ResultRow
            Return ExecuteRow(conn , returnIntValue, 0, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection , ByRef returnIntValue As Integer, ByVal timeout as Integer) As ResultRow
            Return ExecuteRow(conn , returnIntValue, timeout, Nothing)
        End Function

        Public Function ExecuteRow(ByVal conn As System.Data.SqlClient.SqlConnection , Optional ByRef returnIntValue As Integer = 0, Optional ByVal timeout as Integer = 0, Optional ByVal tran as SQLClient.SQLTransaction = Nothing) As ResultRow

            Dim cmd As System.Data.SqlClient.SqlCommand = CreateSQLCommand(conn , timeOut)

            If tran IsNot Nothing Then
                cmd.Transaction = tran
            End If

            Dim rdr As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader(CommandBehavior.SingleRow)

            If rdr.Read Then
                ExecuteRow = New ResultRow(rdr)
            Else
                ExecuteRow = Nothing
            End If

            rdr.Close()

            returnIntValue = DirectCast(cmd.Parameters.Item(0).Value, Integer)



        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection ) As SPResultCollection
            Return ExecuteSP(conn , 0, 0, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection , ByRef returnIntValue As Integer) As SPResultCollection
            Return ExecuteSP(conn , returnIntValue, 0, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection , ByRef returnIntValue As Integer, ByVal timeout as Integer) As SPResultCollection
            Return ExecuteSP(conn , returnIntValue, timeout, Nothing)
        End Function

        Public Function ExecuteSP(ByVal conn As System.Data.SqlClient.SqlConnection , Optional ByRef returnIntValue As Integer = 0, Optional ByVal timeout as Integer = 0, Optional ByVal tran as SQLClient.SQLTransaction = Nothing) As SPResultCollection

            Dim cmd As System.Data.SqlClient.SqlCommand
            cmd = CreateSQLCommand(conn , timeOut)
            If tran IsNot Nothing Then
                cmd.Transaction = tran
            End If
            Dim rdr As System.Data.SqlClient.SqlDataReader = cmd.ExecuteReader

            Dim list As New System.Collections.ArrayList
            Dim res As New SPResultCollection

            Try

                'BEGIN ResultSet 0
                Do While rdr.Read()
                    list.Add(New ResultRow(rdr))
                Loop
                Dim a(list.Count-1) As ResultRow
                list.CopyTo(a)
                res.InitResultSet(0, a)
                'END ResultSet 0
                
                
                'BEGIN ResultSet 1
                list.Clear()
                If rdr.NextResult Then
                    Do While rdr.Read()
                        list.Add(New ResultRow1(rdr))
                    Loop
                End If

                Dim a1(list.Count-1) As ResultRow1
                list.CopyTo(a1)
                res.InitResultSet(1, a1)
                'END ResultSet 1



            Catch ex As Exception


                Throw

            Finally

                rdr.Close()

            End Try

            Dim returnObjectValue As Object = cmd.Parameters.Item(0).Value

            If Not IsDBNull(returnObjectValue) Then
                returnIntValue = CInt(returnObjectValue)
            Else
                returnIntValue = 0
            End If


          



            Return res
        End Function


    End Module
End Namespace

