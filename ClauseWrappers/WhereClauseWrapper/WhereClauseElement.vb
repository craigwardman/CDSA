Namespace WhereClauseWrapper
    <Serializable()> _
    Public Class WhereClauseElement
        Private _compareItem As String
        Private _compareValue As Object
        Private _operator As [Operator]
        Private _useRawValue As Boolean = False

#Region "Properties"
        Public Property UseRawValue() As Boolean
            Get
                Return _useRawValue
            End Get
            Set(ByVal value As Boolean)
                _useRawValue = value
            End Set
        End Property
        Public Property CompareItem() As String
            Get
                Return _compareItem
            End Get
            Set(ByVal value As String)
                _compareItem = value
            End Set
        End Property

        Public Property CompareValue() As Object
            Get
                Return _compareValue
            End Get
            Set(ByVal value As Object)
                _compareValue = value
            End Set
        End Property

        Public Property [Operator]() As [Operator]
            Get
                Return _operator
            End Get
            Set(ByVal value As [Operator])
                _operator = value
            End Set
        End Property
#End Region

        Public Sub New()

        End Sub

        Public Sub New(ByVal compareItem As String, ByVal [operator] As [Operator], ByVal compareValue As Object)
            MyClass.New(compareItem, [operator], compareValue, False)
        End Sub

        Public Sub New(ByVal compareItem As String, ByVal [operator] As [Operator], ByVal compareValue As Object, ByVal useRawValue As Boolean)
            _compareItem = compareItem
            _compareValue = compareValue
            _operator = [operator]
            _useRawValue = useRawValue
        End Sub

    End Class
End Namespace

