Namespace SqlClauses
    Public Class SqlOrderByClauseElement
        Private _columnName As String
        Private _directionText As String

        Public Property ColumnName() As String
            Get
                Return _columnName
            End Get
            Set(ByVal value As String)
                _columnName = value
            End Set
        End Property

        Public Property DirectionText() As String
            Get
                Return _directionText
            End Get
            Set(ByVal value As String)
                _directionText = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal columnName As String, ByVal directionText As String)
            _columnName = columnName
            _directionText = directionText
        End Sub

        Public Overrides Function ToString() As String
            Dim qry As String = ""
            If Not ColumnName.Contains("(") Then
                If ColumnName.Contains(".") Then
                    Dim pathParts() As String = ColumnName.Split("."c)
                    For Each pp As String In pathParts
                        qry &= String.Format("[{0}].", Text.RegularExpressions.Regex.Replace(pp, "[\[\]]", ""))
                    Next
                    qry = qry.Remove(qry.Length - 1, 1)
                    qry &= _directionText
                Else
                    qry = String.Format("[{0}] {1}", Text.RegularExpressions.Regex.Replace(_columnName, "[\[\]]", ""), _directionText)
                End If
            Else
                qry = String.Format("{0}{1}", Text.RegularExpressions.Regex.Replace(ColumnName, "[\[\]]", ""), _directionText)
            End If

            Return qry
        End Function
    End Class
End Namespace
