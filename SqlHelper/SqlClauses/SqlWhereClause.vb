Namespace SqlClauses
    Public Class SqlWhereClause
        Private _JoinList As New Generic.List(Of SqlJoinClause)
        Private _ClauseList As New Generic.List(Of SqlWhereClauseElement)
        Private _subgroups As New Generic.List(Of SqlWhereClause)
        Private _conjunctionText As String

        Public Property JoinList() As Generic.List(Of SqlJoinClause)
            Get
                Return _JoinList
            End Get
            Set(ByVal value As Generic.List(Of SqlJoinClause))
                _JoinList = value
            End Set
        End Property

        Public ReadOnly Property RecursiveJoinList() As Generic.List(Of SqlJoinClause)
            Get
                Dim allJoins As New Generic.List(Of SqlJoinClause)

                If _JoinList IsNot Nothing AndAlso _JoinList.Count > 0 Then
                    allJoins.AddRange(_JoinList)
                End If

                If _subgroups IsNot Nothing AndAlso _subgroups.Count > 0 Then
                    For Each subgroup As SqlWhereClause In _subgroups
                        allJoins.AddRange(subgroup.RecursiveJoinList)
                    Next
                End If

                Return allJoins
            End Get
        End Property

        Public Property ClauseList() As Generic.List(Of SqlWhereClauseElement)
            Get
                Return _ClauseList
            End Get
            Set(ByVal value As Generic.List(Of SqlWhereClauseElement))
                _ClauseList = value
            End Set
        End Property

        Public Property SubGroups() As Generic.List(Of SqlWhereClause)
            Get
                Return _subgroups
            End Get
            Set(ByVal value As Generic.List(Of SqlWhereClause))
                _subgroups = value
            End Set
        End Property

        Public Property ConjunctionText() As String
            Get
                Return _conjunctionText
            End Get
            Set(ByVal value As String)
                _conjunctionText = value
            End Set
        End Property

        Public ReadOnly Property RecursiveClauseList() As Generic.List(Of SqlWhereClauseElement)
            Get
                Dim allClauses As New Generic.List(Of SqlWhereClauseElement)

                If _ClauseList IsNot Nothing AndAlso _ClauseList.Count > 0 Then
                    allClauses.AddRange(_ClauseList)
                End If

                If _subgroups IsNot Nothing AndAlso _subgroups.Count > 0 Then
                    For Each subgroup As SqlWhereClause In _subgroups
                        allClauses.AddRange(subgroup.RecursiveClauseList)
                    Next
                End If

                Return allClauses
            End Get
        End Property

        Public Overrides Function ToString() As String
            If (_ClauseList IsNot Nothing AndAlso _ClauseList.Count > 0) Or (_subgroups IsNot Nothing AndAlso _subgroups.Count > 0) Then
                Dim sqlWhere As String = ""

                'add all the items with this groups conjuction text
                If _ClauseList IsNot Nothing AndAlso _ClauseList.Count > 0 Then
                    For Each clauseElement As SqlWhereClauseElement In _ClauseList
                        sqlWhere &= clauseElement.ToString() & " " & ConjunctionText & " "
                    Next
                End If

                'now add all the subgroups with the conjuction text
                If _subgroups IsNot Nothing AndAlso _subgroups.Count > 0 Then
                    For Each subgroup As SqlWhereClause In _subgroups
                        Dim allClauses As Generic.List(Of SqlWhereClauseElement) = subgroup.RecursiveClauseList
                        If allClauses IsNot Nothing AndAlso allClauses.Count > 0 Then
                            sqlWhere &= "(" & subgroup.ToString() & ") " & ConjunctionText & " "
                        End If
                    Next
                End If

                'remove the last conjuction text
                Return sqlWhere.Substring(0, sqlWhere.Length - (" " & ConjunctionText & " ").Length)

            Else
                Return ""
            End If
        End Function
    End Class
End Namespace
