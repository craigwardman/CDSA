Namespace WhereClauseWrapper
    <Serializable()> _
    Public Class WhereClause
        Private _joinList As New Generic.List(Of JoinClauseWrapper.JoinClause)
        Private _ClauseList As New Generic.List(Of WhereClauseElement)
        Private _subGroups As New Generic.List(Of WhereClause)
        Private _conjunctionOperator As ConjunctionOperator

        Public Sub New()

        End Sub

        Public Sub New(ByVal elements() As WhereClauseElement, ByVal conjunctionOperator As ConjunctionOperator)
            _ClauseList.AddRange(elements)
            _conjunctionOperator = conjunctionOperator
        End Sub

        Public Sub New(ByVal element As WhereClauseElement)
            _ClauseList.Add(element)
            _conjunctionOperator = WhereClauseWrapper.ConjunctionOperator.And
        End Sub

        Public Sub New(ByVal compareItem As String, ByVal [operator] As [Operator], ByVal compareValue As Object)
            MyClass.New(New WhereClauseElement(compareItem, [operator], compareValue))
        End Sub

#Region "Properties"
        Public Property JoinList() As Generic.List(Of JoinClauseWrapper.JoinClause)
            Get
                Return _joinList
            End Get
            Set(ByVal value As Generic.List(Of JoinClauseWrapper.JoinClause))
                _joinList = value
            End Set
        End Property

        Public Property ClauseList() As Generic.List(Of WhereClauseElement)
            Get
                Return _ClauseList
            End Get
            Set(ByVal value As Generic.List(Of WhereClauseElement))
                _ClauseList = value
            End Set
        End Property

        Public Property SubGroups() As Generic.List(Of WhereClause)
            Get
                Return _subGroups
            End Get
            Set(ByVal value As Generic.List(Of WhereClause))
                _subGroups = value
            End Set
        End Property

        Public Property ConjunctionOperator() As ConjunctionOperator
            Get
                Return _conjunctionOperator
            End Get
            Set(ByVal value As ConjunctionOperator)
                _conjunctionOperator = value
            End Set
        End Property
#End Region

        Public ReadOnly Property RecursiveClauseList() As Generic.List(Of WhereClauseElement)
            Get
                Dim allClauses As New Generic.List(Of WhereClauseElement)

                If _ClauseList IsNot Nothing AndAlso _ClauseList.Count > 0 Then
                    allClauses.AddRange(_ClauseList)
                End If

                If _subGroups IsNot Nothing AndAlso _subGroups.Count > 0 Then
                    For Each subgroup As WhereClause In _subGroups
                        allClauses.AddRange(subgroup.RecursiveClauseList)
                    Next
                End If

                Return allClauses
            End Get
        End Property
    End Class
End Namespace
