Namespace SqlClauses
    Public Class SqlWhereClauseElement
        Private _columnName As String
        Private _paramName As String
        Private _paramValue As Object
        Private _operatorText As String
        Private _useRawValue As Boolean

        Public Property UseRawValue() As Boolean
            Get
                Return _useRawValue
            End Get
            Set(ByVal value As Boolean)
                _useRawValue = value
            End Set
        End Property
        Public Property ColumnName() As String
            Get
                Return _columnName
            End Get
            Set(ByVal value As String)
                _columnName = value
            End Set
        End Property

        Public Property ParamName() As String
            Get
                Return _paramName
            End Get
            Set(ByVal value As String)
                _paramName = value
            End Set
        End Property

        Public Property ParamValue() As Object
            Get
                Return _paramValue
            End Get
            Set(ByVal value As Object)
                _paramValue = value
            End Set
        End Property

        Public Property OperatorText() As String
            Get
                Return _operatorText
            End Get
            Set(ByVal value As String)
                _operatorText = value
            End Set
        End Property

        Public Sub New()

        End Sub


        Public Sub New(ByVal columnName As String, ByVal paramName As String, ByVal paramValue As Object, ByVal operatorText As String, ByVal useRawValue As Boolean)
            _columnName = columnName
            _paramName = paramName
            _paramValue = paramValue
            _operatorText = operatorText
            _useRawValue = useRawValue
        End Sub
        Public Sub New(ByVal columnName As String, ByVal paramName As String, ByVal paramValue As Object, ByVal operatorText As String)
            MyClass.New(columnName, paramName, paramValue, operatorText, False)
        End Sub

        Public Overrides Function ToString() As String
            'allow special formatting of operator syntax
            Dim qry As String = ""
            If Not ColumnName.Contains("(") Then
                If ColumnName.Contains(".") Then
                    Dim pathParts() As String = ColumnName.Split("."c)
                    For Each pp As String In pathParts
                        qry &= String.Format("[{0}].", Text.RegularExpressions.Regex.Replace(pp, "[\[\]]", ""))
                    Next
                    qry = qry.Remove(qry.Length - 1, 1)
                    qry &= OperatorText
                Else
                    qry = String.Format("[{0}]{1}", Text.RegularExpressions.Regex.Replace(ColumnName, "[\[\]]", ""), OperatorText)
                End If
            Else
                qry = String.Format("{0}{1}", Text.RegularExpressions.Regex.Replace(ColumnName, "[\[\]]", ""), OperatorText)
            End If


            Select Case _operatorText
                Case " IN "
                    qry += "(" + CStr(ParamValue) + ")"
                Case Else
                    If Not _useRawValue Then
                        qry += ParamName
                    Else
                        qry += ParamValue.ToString()
                    End If
            End Select



            Return qry
        End Function
    End Class

End Namespace
