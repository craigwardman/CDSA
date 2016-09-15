Namespace SqlClauses
    Public Class SqlOrderByClause
        Private _ClauseList As New Generic.List(Of SqlOrderByClauseElement)

        Public Property ClauseList() As Generic.List(Of SqlOrderByClauseElement)
            Get
                Return _ClauseList
            End Get
            Set(ByVal value As Generic.List(Of SqlOrderByClauseElement))
                _ClauseList = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            If _ClauseList IsNot Nothing AndAlso _ClauseList.Count > 0 Then
                Dim sqlOrderBy As String = ""

                For Each clauseElement As SqlOrderByClauseElement In _ClauseList
                    sqlOrderBy &= clauseElement.ToString() & ","
                Next

                Return sqlOrderBy.Substring(0, sqlOrderBy.Length - 1)
            Else
                Return ""
            End If
        End Function
    End Class
End Namespace
