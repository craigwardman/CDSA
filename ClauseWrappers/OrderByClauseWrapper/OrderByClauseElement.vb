Namespace OrderByClauseWrapper
    <Serializable()> _
    Public Class OrderByClauseElement
        Private _fieldName As String
        Private _direction As Direction

#Region "Properties"
        Public Property FieldName() As String
            Get
                Return _fieldName
            End Get
            Set(ByVal value As String)
                _fieldName = value
            End Set
        End Property

        Public Property Direction() As Direction
            Get
                Return _direction
            End Get
            Set(ByVal value As Direction)
                _direction = value
            End Set
        End Property

#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal fieldName As String, ByVal direction As Direction)
            _fieldName = fieldName
            _direction = direction
        End Sub
    End Class

End Namespace
