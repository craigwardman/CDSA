Namespace WhereClauseWrapper
    Public Enum [Operator]
        Equals
        GreaterThan
        LessThan
        GreaterThanEqualTo
        LessThanEqualTo
        NotEqual
        IsNull
        IsNotNull
        [Like]
        [In]
    End Enum

    Public Enum [ConjunctionOperator]
        [And]
        [Or]
    End Enum
End Namespace
