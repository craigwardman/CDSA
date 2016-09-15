Namespace OrderByClauseWrapper
    Public Class OrderByClause
        Private _ClauseList As New Generic.List(Of OrderByClauseElement)

        Public Property ClauseList() As Generic.List(Of OrderByClauseElement)
            Get
                Return _ClauseList
            End Get
            Set(ByVal value As Generic.List(Of OrderByClauseElement))
                _ClauseList = value
            End Set
        End Property

        Public Sub New()

        End Sub

        Public Sub New(ByVal elements As OrderByClauseElement())
            _ClauseList.AddRange(elements)
        End Sub

        Public Sub New(ByVal element As OrderByClauseElement)
            _ClauseList.Add(element)
        End Sub

        Public Sub New(ByVal fieldName As String, ByVal direction As Direction)
            MyClass.New(New OrderByClauseElement(fieldName, direction))
        End Sub
    End Class
End Namespace
