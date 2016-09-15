Namespace SqlClauses
    Public Class SqlJoinClause
        Private _joinTypeText As String
        Private _tableAlias As String
        Private _targetTable As String
        Private _targetColumn As String
        Private _srcTable As String
        Private _srcColumn As String

        Private _joinWhereConjunctionText As String
        Private _joinWhere As SqlWhereClause

        Public Property JoinWhereConjunctionText() As String
            Get
                Return _joinWhereConjunctionText
            End Get
            Set(ByVal value As String)
                _joinWhereConjunctionText = value
            End Set
        End Property

        Public Property JoinWhere() As SqlWhereClause
            Get
                Return _joinWhere
            End Get
            Set(ByVal value As SqlWhereClause)
                _joinWhere = value
            End Set
        End Property

        Public Property JoinTypeText() As String
            Get
                Return _joinTypeText
            End Get
            Set(ByVal value As String)
                _joinTypeText = value
            End Set
        End Property
        Public Property TableAlias() As String
            Get
                Return _tableAlias
            End Get
            Set(ByVal value As String)
                _tableAlias = value
            End Set
        End Property

        Public Property TargetTable() As String
            Get
                Return _targetTable
            End Get
            Set(ByVal value As String)
                _targetTable = value
            End Set
        End Property

        Public Property TargetColumn() As String
            Get
                Return _targetColumn
            End Get
            Set(ByVal value As String)
                _targetColumn = value
            End Set
        End Property

        Public Property SourceTable() As String
            Get
                Return _srcTable
            End Get
            Set(ByVal value As String)
                _srcTable = value
            End Set
        End Property

        Public Property SourceColumn() As String
            Get
                Return _srcColumn
            End Get
            Set(ByVal value As String)
                _srcColumn = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Dim tableName As String = TargetTable
            Dim strQry As String = String.Format("{0}[{1}]", _joinTypeText, _targetTable.Replace(".", "].["))

            If _tableAlias <> "" Then
                strQry &= " As [" & _tableAlias & "]"
                tableName = _tableAlias
            End If


            strQry &= String.Format(" ON [{0}].[{1}]=[{2}].[{3}]", _srcTable, _srcColumn, tableName, _targetColumn)

            If _joinWhere IsNot Nothing AndAlso _joinWhere.RecursiveClauseList.Count > 0 Then
                strQry += " " + _joinWhereConjunctionText + " (" + _joinWhere.ToString() + ")"
            End If

            Return strQry
        End Function
    End Class
End Namespace
