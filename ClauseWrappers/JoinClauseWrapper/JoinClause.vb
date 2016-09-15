Imports ClauseWrappers.WhereClauseWrapper

Namespace JoinClauseWrapper
    <Serializable()> _
    Public Class JoinClause
        Private _joinType As JoinType

        Private _targetTable As String
        Private _targetColumn As String
        Private _srcTable As String
        Private _srcColumn As String
        Private _tableAlias As String

        Private _joinWhere As WhereClause
        Private _joinWhereConjunctionOperator As ConjunctionOperator


        Public Sub New()

        End Sub

        Public Sub New(ByVal joinType As JoinType, ByVal targetTable As String, ByVal tableAlias As String, ByVal targetColumn As String, ByVal srcTable As String, ByVal srcColumn As String)
            _joinType = joinType
            _targetTable = targetTable
            _targetColumn = targetColumn
            _srcTable = srcTable
            _srcColumn = srcColumn
            _tableAlias = tableAlias
        End Sub

        Public Sub New(ByVal joinType As JoinType, ByVal targetTable As String, ByVal targetColumn As String, ByVal srcTable As String, ByVal srcColumn As String)
            MyClass.New(joinType, targetTable, "", targetColumn, srcTable, srcColumn)
        End Sub

#Region "Properties"

        Public Property JoinWhere() As WhereClause
            Get
                Return _joinWhere
            End Get
            Set(ByVal value As WhereClause)
                _joinWhere = value
            End Set
        End Property

        Public Property JoinWhereConjunctionOperator() As ConjunctionOperator
            Get
                Return _joinWhereConjunctionOperator
            End Get
            Set(ByVal value As ConjunctionOperator)
                _joinWhereConjunctionOperator = value
            End Set
        End Property

        Public Property JoinType() As JoinType
            Get
                Return _joinType
            End Get
            Set(ByVal value As JoinType)
                _joinType = value
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
#End Region

    End Class
End Namespace
