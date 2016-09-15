'Copyright 2008 Craig Wardman <lgpl@craigwardman.com>
'This file is part of SqlHelper

'SqlHelper is free software: you can redistribute it and/or modify
'it under the terms of the GNU Lesser General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'SqlHelper is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU Lesser General Public License for more details.

'You should have received a copy of the GNU Lesser General Public License
'along with SqlHelper.  If not, see <http://www.gnu.org/licenses/>.

''' <summary>
''' Provides a wrapper for SQL Server communication.
''' </summary>
''' <remarks></remarks>
Public Class SqlHelper
    Implements ClauseWrappers.WhereClauseWrapper.IWhereClauseHandler
    Implements ClauseWrappers.OrderByClauseWrapper.IOrderByClauseHandler
    Implements ClauseWrappers.JoinClauseWrapper.IJoinClauseHandler

    Private _sqlConnString As String

    ''' <summary>
    ''' Instanciate the class using the given connection string for SQL
    ''' </summary>
    ''' <param name="connectionString">SQL Connection String</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal connectionString As String)
        _sqlConnString = connectionString
    End Sub

    ''' <summary>
    ''' Tests the current connection string by opening and closing a connection to the database
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsValidConnection() As Boolean
        Try
            Using _sqlConn As New SqlClient.SqlConnection(_sqlConnString)
                _sqlConn.Open()
                _sqlConn.Close()
            End Using

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Executes a piece of SQL code against the server.
    ''' </summary>
    ''' <param name="commandText">The SQL command to execute.</param>
    ''' <param name="commandType">The type of the command being passed.</param>
    ''' <param name="inputParams">Any input parameters used in the command or SPROC.</param>
    ''' <param name="outputParams">Any output parameters you want to capture from the SPROC.</param>
    ''' <returns>A dataset containing the result of the query, or nothing for non-query commands.</returns>
    ''' <remarks></remarks>
    Public Function Execute(ByVal commandText As String, ByVal commandType As CommandType, ByVal inputParams As Generic.Dictionary(Of String, Object), ByVal outputParams As Generic.Dictionary(Of String, Object)) As DataSet
        Using cmd As SqlClient.SqlCommand = GetSqlCommand(commandText, commandType, inputParams, outputParams)
            Return Execute(cmd, outputParams)
        End Using
    End Function

    Public Function Execute(ByVal cmd As SqlClient.SqlCommand, ByVal outputParams As Generic.Dictionary(Of String, Object)) As DataSet
        Using _sqlConn As New SqlClient.SqlConnection(_sqlConnString)
            _sqlConn.Open()

            cmd.Connection = _sqlConn
            cmd.CommandTimeout = _sqlConn.ConnectionTimeout

            'fill the results from the query
            Dim result As New DataSet()
            Using dataAdapter As New SqlClient.SqlDataAdapter(cmd)
                dataAdapter.Fill(result)
            End Using

            'get any output parameter results
            If outputParams IsNot Nothing AndAlso outputParams.Count > 0 Then
                Dim outputParamResults As New Generic.Dictionary(Of String, Object)

                For Each origParam As KeyValuePair(Of String, Object) In outputParams
                    outputParamResults.Add(origParam.Key, cmd.Parameters(origParam.Key).Value)
                Next

                'output back to original object
                outputParams = outputParamResults
            End If

            Return result

            _sqlConn.Close()
        End Using
    End Function

    Public Sub ExecuteReader(ByVal cmd As SqlClient.SqlCommand, ByVal outputParams As Generic.Dictionary(Of String, Object), rowCallback As Action(Of IDataReader))
        Using _sqlConn As New SqlClient.SqlConnection(_sqlConnString)
            _sqlConn.Open()

            cmd.Connection = _sqlConn
            cmd.CommandTimeout = _sqlConn.ConnectionTimeout

            'create a data reader for the query
            Using sqlReader As SqlClient.SqlDataReader = cmd.ExecuteReader()

                While sqlReader.Read()
                    rowCallback(sqlReader)
                End While


                'get any output parameter results
                If outputParams IsNot Nothing AndAlso outputParams.Count > 0 Then
                    Dim outputParamResults As New Generic.Dictionary(Of String, Object)

                    For Each origParam As KeyValuePair(Of String, Object) In outputParams
                        outputParamResults.Add(origParam.Key, cmd.Parameters(origParam.Key).Value)
                    Next

                    'output back to original object
                    outputParams = outputParamResults
                End If
            End Using

            _sqlConn.Close()
        End Using
    End Sub

    Public Function Execute(ByVal commandText As String) As DataSet
        Return Execute(commandText, CommandType.Text, Nothing, Nothing)
    End Function

    Public Function Execute(ByVal commandText As String, ByVal commandType As CommandType) As DataSet
        Return Execute(commandText, commandType, Nothing, Nothing)
    End Function

    Public Function Execute(ByVal commandText As String, ByVal commandType As CommandType, ByVal inputParams As Generic.Dictionary(Of String, Object)) As DataSet
        Return Execute(commandText, commandType, inputParams, Nothing)
    End Function

    Public Function GetSqlCommand(ByVal commandText As String, ByVal commandType As CommandType, ByVal inputParams As Generic.Dictionary(Of String, Object), ByVal outputParams As Generic.Dictionary(Of String, Object)) As SqlClient.SqlCommand
        Dim cmd As New SqlClient.SqlCommand(commandText)
        cmd.CommandType = commandType


        'add the parameters as SqlParameters
        If inputParams IsNot Nothing Then
            For Each param As KeyValuePair(Of String, Object) In inputParams
                cmd.Parameters.Add(New SqlClient.SqlParameter(param.Key, IIf(param.Value IsNot Nothing, param.Value, DBNull.Value)))
            Next
        End If

        If outputParams IsNot Nothing Then
            For Each param As KeyValuePair(Of String, Object) In outputParams
                Dim outputSqlParam As New SqlClient.SqlParameter(param.Key, IIf(param.Value IsNot Nothing, param.Value, DBNull.Value))
                outputSqlParam.Direction = ParameterDirection.Output
                cmd.Parameters.Add(outputSqlParam)
            Next
        End If

        Return cmd
    End Function

    ''' <summary>
    ''' Wraps the functionality of performing an SQL SELECT statement, with paging enabled
    ''' </summary>
    ''' <param name="tableName">Name of the table to select from</param>
    ''' <param name="fields">Fields to be selected</param>
    ''' <param name="whereClause">Conditions to match</param>
    ''' <param name="orderByClause">Order of the output data</param>
    ''' <param name="pageNumber">Page number to return</param>
    ''' <param name="pageSize">Page size (how many records do you consider a page)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [Select](ByVal tableName As String, ByVal fields As String, ByVal whereClause As SqlClauses.SqlWhereClause, ByVal orderByClause As SqlClauses.SqlOrderByClause, ByVal pageNumber As Integer, ByVal pageSize As Integer) As DataSet
        Return [Select](tableName, fields, whereClause, orderByClause, pageNumber, pageSize, True)
    End Function

    ''' <summary>
    ''' Wraps the functionality of performing an SQL SELECT statement, with paging enabled
    ''' </summary>
    ''' <param name="tableName">Name of the table to select from</param>
    ''' <param name="fields">Fields to be selected</param>
    ''' <param name="whereClause">Conditions to match</param>
    ''' <param name="orderByClause">Order of the output data</param>
    ''' <param name="pageNumber">Page number to return</param>
    ''' <param name="pageSize">Page size (how many records do you consider a page)</param>
    ''' <param name="getTotalCount">Indicates whether to run a second query to get the total count (for paged queries)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [Select](ByVal tableName As String, ByVal fields As String, ByVal whereClause As SqlClauses.SqlWhereClause, ByVal orderByClause As SqlClauses.SqlOrderByClause, ByVal pageNumber As Integer, ByVal pageSize As Integer, ByVal getTotalCount As Boolean) As DataSet
        Return Execute(GetSelectCommand(tableName, fields, whereClause, orderByClause, pageNumber, pageSize, getTotalCount), Nothing)
    End Function

    ''' <summary>
    ''' Wraps the functionality of performing an SQL SELECT statement
    ''' </summary>
    ''' <param name="tableName">The table to perform the query against.</param>
    ''' <param name="fields">The fields, or * for all.</param>
    ''' <param name="whereClause">A list of whereClause specifying (if any) the conditions to match.</param>
    ''' <param name="orderByClause">orderByClause specifying the sort order of the results</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function [Select](ByVal tableName As String, ByVal fields As String, ByVal whereClause As SqlClauses.SqlWhereClause, ByVal orderByClause As SqlClauses.SqlOrderByClause) As DataSet
        Return [Select](tableName, fields, whereClause, orderByClause, -1, -1)
    End Function


    Public Function [Select](ByVal tableName As String, ByVal fields As String, ByVal whereClause As SqlClauses.SqlWhereClause) As DataSet
        Return [Select](tableName, fields, whereClause, Nothing)
    End Function

    Public Function [Select](ByVal tableName As String, ByVal fields As String) As DataSet
        Return [Select](tableName, fields, Nothing, Nothing)
    End Function

    Public Function GetSelectCommand(ByVal tableName As String, ByVal fields As String, ByVal whereClause As SqlClauses.SqlWhereClause, ByVal orderByClause As SqlClauses.SqlOrderByClause, ByVal pageNumber As Integer, ByVal pageSize As Integer, ByVal getTotalCount As Boolean) As SqlClient.SqlCommand
        Dim sSql As String = ""
        Dim sSqlJoins As String = ""
        Dim sWhere As String = ""
        Dim sOrderBy As String = ""

        Dim params As Generic.Dictionary(Of String, Object) = Nothing

        If whereClause IsNot Nothing Then

            'get the joined tabled
            For Each jc As SqlClauses.SqlJoinClause In whereClause.RecursiveJoinList
                sSqlJoins &= jc.ToString()

                'any params?
                If jc.JoinWhere IsNot Nothing AndAlso jc.JoinWhere.RecursiveClauseList.Count > 0 Then
                    If params Is Nothing Then params = New Generic.Dictionary(Of String, Object)
                    For Each clause As SqlClauses.SqlWhereClauseElement In jc.JoinWhere.RecursiveClauseList
                        If clause.ParamName <> "" Then
                            params.Add(clause.ParamName, clause.ParamValue)
                        End If
                    Next
                End If
            Next

            Dim allClauses As Generic.List(Of SqlClauses.SqlWhereClauseElement) = whereClause.RecursiveClauseList

            If allClauses.Count > 0 Then
                sWhere = "WHERE " & whereClause.ToString()

                'now get the params
                If params Is Nothing Then params = New Generic.Dictionary(Of String, Object)
                For Each clause As SqlClauses.SqlWhereClauseElement In allClauses
                    If clause.ParamName <> "" Then
                        params.Add(clause.ParamName, clause.ParamValue)
                    End If
                Next
            End If
        End If

        If orderByClause IsNot Nothing AndAlso orderByClause.ClauseList.Count > 0 Then
            sOrderBy = "ORDER BY " & orderByClause.ToString()
        End If


        If pageNumber > 0 And pageSize > 0 Then
            'if we are paging data then we will be using WITH(NOLOCK) for speed
            sSqlJoins = Text.RegularExpressions.Regex.Replace(sSqlJoins, "(JOIN \[.*?\]\s(As \[.*?\])?)", "$1 WITH(NOLOCK) ")

            'we are using a page query, wrap the inner statement in a pageing statement
            Dim innerQuery As String = String.Format("SELECT {0}, ROW_NUMBER() OVER({1}) as RowNum FROM [{2}] WITH(NOLOCK){3} {4}", _
                                Text.RegularExpressions.Regex.Replace(fields, "(^|(?<comma>,)\s?)(?<dist>DISTINCT\s)?(?<fieldname>[^,]*?)", "$2$3[" + RemoveSquareBrackets(tableName) + "].$4"), _
                                sOrderBy, RemoveSquareBrackets(tableName), sSqlJoins, sWhere)

            Dim fullQuery As String = String.Format("SELECT {0} FROM ", fields.Replace("DISTINCT ", ""))
            fullQuery &= "(" & innerQuery & ")" & String.Format(" As [Paged" & RemoveSquareBrackets(tableName) & "] WHERE RowNum BETWEEN {0} AND {1}", ((pageNumber - 1) * pageSize) + 1, ((pageNumber - 1) * pageSize) + pageSize)

            sSql = fullQuery & " ORDER BY RowNum ASC;"

            If getTotalCount Then
                sSql &= String.Format("SELECT Count(*) AS TotalCount FROM [{0}] WITH(NOLOCK){1} {2};", RemoveSquareBrackets(tableName), sSqlJoins, sWhere)
            End If
        Else
            'no paging, normal select statement
            sSql = String.Format("SELECT {0} FROM [{1}]{2} {3} {4}", fields, RemoveSquareBrackets(tableName), sSqlJoins, sWhere, sOrderBy)
        End If

        Return GetSqlCommand(sSql, CommandType.Text, params, Nothing)
    End Function

    ''' <summary>
    ''' Wraps the functionality of performing an SQL INSERT statement
    ''' </summary>
    ''' <param name="tableName">The table to perform the insert against.</param>
    ''' <param name="values">The values to insert, in column order.</param>
    ''' <returns>Returns the ID of the newly inserted record, or -1</returns>
    ''' <remarks></remarks>
    Public Function Insert(ByVal tableName As String, ByVal values As Generic.List(Of Object)) As Integer
        Dim params As New Generic.Dictionary(Of String, Object)

        'basic sql
        Dim sSql As String = "INSERT INTO [" & RemoveSquareBrackets(tableName) & "] VALUES("

        'append the params
        Dim paramIndex As Integer = 1
        For Each value As Object In values
            params.Add("@Param" & paramIndex, value)
            sSql &= "@Param" & paramIndex & ","
            paramIndex += 1
        Next
        'remove the last comma
        sSql = sSql.Substring(0, sSql.Length - 1)
        sSql &= "); SELECT @@IDENTITY As NewId;"

        'exec
        Dim result As DataSet = Execute(sSql, CommandType.Text, params)

        If result IsNot Nothing AndAlso _
                result.Tables.Count > 0 AndAlso result.Tables(0).Rows.Count > 0 Then
            Return CInt(result.Tables(0).Rows(0).Item("NewId"))
        Else
            Return -1
        End If

    End Function

    ''' <summary>
    ''' Wraps the function of performing an SQL INSERT statement
    ''' </summary>
    ''' <param name="tableName">The table to perform the insert against.</param>
    ''' <param name="columnValuePairs">A list of values and the corresponding column name, for passing values in any order.</param>
    ''' <returns>Returns the ID of the newly inserted record, or -1</returns>
    ''' <remarks></remarks>
    Public Function Insert(ByVal tableName As String, ByVal columnValuePairs As Generic.List(Of KeyValuePair(Of String, Object))) As Integer
        Dim fieldNames As New Generic.List(Of String)
        Dim paramNames As New Generic.List(Of String)
        Dim params As New Generic.Dictionary(Of String, Object)

        For Each cvp As KeyValuePair(Of String, Object) In columnValuePairs
            fieldNames.Add("[" & RemoveSquareBrackets(cvp.Key) & "]")
            Dim paramName As String = ColumnNameToParamName(cvp.Key)

            If paramNames.Contains(paramName) Then
                Dim i As Integer = 1
                While paramNames.Contains(paramName & i)
                    i += 1
                End While

                paramName &= i
            End If

            paramNames.Add(paramName)

            params.Add(paramName, cvp.Value)
        Next

        Dim sSql As String = String.Format("INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @@IDENTITY As NewId;", RemoveSquareBrackets(tableName), String.Join(",", fieldNames.ToArray), String.Join(",", paramNames.ToArray))
        Dim result As DataSet = Execute(sSql, CommandType.Text, params)

        If result IsNot Nothing AndAlso _
                result.Tables.Count > 0 AndAlso result.Tables(0).Rows.Count > 0 Then
            Return CInt(result.Tables(0).Rows(0).Item("NewId"))
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' Wraps the functionality of the SQL UPDATE statement.
    ''' </summary>
    ''' <param name="tableName">The table to update</param>
    ''' <param name="columnValuePairs">The columns and values to update</param>
    ''' <param name="whereClause">The clauses for the records to update</param>
    ''' <remarks></remarks>
    Public Sub Update(ByVal tableName As String, ByVal columnValuePairs As Generic.List(Of KeyValuePair(Of String, Object)), ByVal whereClause As SqlClauses.SqlWhereClause)
        Dim fieldParamPairs As String = ""
        Dim paramNames As New Generic.List(Of String)
        Dim params As New Generic.Dictionary(Of String, Object)

        'work out the params for the SET
        For Each cvp As KeyValuePair(Of String, Object) In columnValuePairs
            Dim paramName As String = ColumnNameToParamName(cvp.Key)

            If paramNames.Contains(paramName) Then
                Dim i As Integer = 1
                While paramNames.Contains(paramName & i)
                    i += 1
                End While

                paramName &= i
            End If

            paramNames.Add(paramName)

            fieldParamPairs &= "[" & RemoveSquareBrackets(cvp.Key) & "]" & "=" & paramName & ","
            params.Add(paramName, cvp.Value)
        Next

        'remove the last comma
        fieldParamPairs = fieldParamPairs.Substring(0, fieldParamPairs.Length - 1)

        Dim sSql As String = String.Format("UPDATE [{0}]", RemoveSquareBrackets(tableName))

        'now work out the params for the WHERE
        If whereClause IsNot Nothing Then
            'get the joined tabled
            For Each jc As SqlClauses.SqlJoinClause In whereClause.RecursiveJoinList
                sSql &= jc.ToString()
            Next
            sSql += String.Format(" SET {0}", fieldParamPairs)

            Dim allClauses As Generic.List(Of SqlClauses.SqlWhereClauseElement) = whereClause.RecursiveClauseList
            If allClauses.Count > 0 Then
                sSql &= " WHERE " & whereClause.ToString()

                'now get the params
                For Each clause As SqlClauses.SqlWhereClauseElement In allClauses
                    If clause.ParamName <> "" Then
                        params.Add(clause.ParamName, clause.ParamValue)
                    End If
                Next
            End If
        Else
            sSql += String.Format(" SET {0}", fieldParamPairs)
        End If

        Execute(sSql, CommandType.Text, params)
    End Sub

    ''' <summary>
    ''' Wraps the functionality of the SQL DELETE statement.
    ''' </summary>
    ''' <param name="tableName">The name of the table to delete from</param>
    ''' <param name="whereClause">The clauses to match for the records to delete.</param>
    ''' <remarks></remarks>
    Public Sub Delete(ByVal tableName As String, ByVal whereClause As SqlClauses.SqlWhereClause)
        Dim sSql As String = String.Format("DELETE FROM [{0}]", RemoveSquareBrackets(tableName))
        Dim params As Generic.Dictionary(Of String, Object) = Nothing

        If whereClause IsNot Nothing Then
            'get the joined tabled
            For Each jc As SqlClauses.SqlJoinClause In whereClause.RecursiveJoinList
                sSql &= jc.ToString()
            Next

            Dim allClauses As Generic.List(Of SqlClauses.SqlWhereClauseElement) = whereClause.RecursiveClauseList
            If allClauses.Count > 0 Then
                sSql &= " WHERE " & whereClause.ToString()

                'now get the params
                params = New Generic.Dictionary(Of String, Object)
                For Each clause As SqlClauses.SqlWhereClauseElement In allClauses
                    If clause.ParamName <> "" Then
                        params.Add(clause.ParamName, clause.ParamValue)
                    End If
                Next
            End If

        End If

        Execute(sSql, CommandType.Text, params)
    End Sub

#Region "IOrderBy/WhereClauseHandler"
    'convert the generic whereclauseelement to sql syntaxed sqlwhereclauseelement
    Private Function ParseWhereClauseElement(ByVal whereClauseElement As ClauseWrappers.WhereClauseWrapper.WhereClauseElement) As SqlClauses.SqlWhereClauseElement
        Dim columnName, paramName, operatorText As String
        Dim paramValue As Object

        columnName = whereClauseElement.CompareItem
        paramName = ColumnNameToParamName(columnName)
        paramValue = whereClauseElement.CompareValue

        Select Case whereClauseElement.Operator
            Case ClauseWrappers.WhereClauseWrapper.Operator.Equals
                operatorText = "="
            Case ClauseWrappers.WhereClauseWrapper.Operator.GreaterThan
                operatorText = ">"
            Case ClauseWrappers.WhereClauseWrapper.Operator.GreaterThanEqualTo
                operatorText = ">="
            Case ClauseWrappers.WhereClauseWrapper.Operator.IsNotNull
                operatorText = " IS NOT NULL"
                paramValue = ""
                paramName = ""
            Case ClauseWrappers.WhereClauseWrapper.Operator.IsNull
                operatorText = " IS NULL"
                paramValue = ""
                paramName = ""
            Case ClauseWrappers.WhereClauseWrapper.Operator.LessThan
                operatorText = "<"
            Case ClauseWrappers.WhereClauseWrapper.Operator.LessThanEqualTo
                operatorText = "<="
            Case ClauseWrappers.WhereClauseWrapper.Operator.NotEqual
                operatorText = "!="
            Case ClauseWrappers.WhereClauseWrapper.Operator.Like
                operatorText = " LIKE "
            Case ClauseWrappers.WhereClauseWrapper.Operator.In
                operatorText = " IN "
                paramName = ""
            Case Else
                operatorText = ""
        End Select

        If whereClauseElement.UseRawValue Then paramName = ""

        Return New SqlClauses.SqlWhereClauseElement(columnName, paramName, paramValue, operatorText, whereClauseElement.UseRawValue)
    End Function

    ''' <summary>
    ''' Converts a generic whereClause to an Sql Where Clause object
    ''' </summary>
    ''' <param name="clause">The clause object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseWhereClause(ByVal clause As ClauseWrappers.WhereClauseWrapper.WhereClause) As Object Implements ClauseWrappers.WhereClauseWrapper.IWhereClauseHandler.ParseWhereClause
        If clause IsNot Nothing Then
            'start the recursive process of adding items and subgroups
            Return ParseWhereClause(clause, New Hashtable())
        Else
            'create an empty sqlWhere clause
            Return New SqlClauses.SqlWhereClause
        End If

    End Function

    'this is the recursively called version that converts generic where clause to sql where clause
    Private Function ParseWhereClause(ByVal currentClause As ClauseWrappers.WhereClauseWrapper.WhereClause, ByRef takenParamNames As Hashtable) As SqlClauses.SqlWhereClause

        Dim currentSqlWhere As New SqlClauses.SqlWhereClause
        Select Case currentClause.ConjunctionOperator
            Case ClauseWrappers.WhereClauseWrapper.ConjunctionOperator.And
                currentSqlWhere.ConjunctionText = "AND"
            Case ClauseWrappers.WhereClauseWrapper.ConjunctionOperator.Or
                currentSqlWhere.ConjunctionText = "OR"
            Case Else
                currentSqlWhere.ConjunctionText = ""
        End Select


        'for each element, create an sql element
        If currentClause IsNot Nothing AndAlso currentClause.ClauseList IsNot Nothing AndAlso currentClause.ClauseList.Count > 0 Then
            For Each whereClauseElement As ClauseWrappers.WhereClauseWrapper.WhereClauseElement In currentClause.ClauseList
                'get the sql element
                Dim sqlWhereElement As SqlClauses.SqlWhereClauseElement = ParseWhereClauseElement(whereClauseElement)
                If sqlWhereElement.ParamName <> "" Then
                    If takenParamNames.ContainsKey(sqlWhereElement.ParamName) Then
                        Dim paramNameNumber As Integer = 1
                        While takenParamNames.ContainsKey(sqlWhereElement.ParamName & paramNameNumber)
                            paramNameNumber += 1
                        End While

                        sqlWhereElement.ParamName &= paramNameNumber
                    End If

                    takenParamNames.Add(sqlWhereElement.ParamName, Nothing)
                End If
                currentSqlWhere.ClauseList.Add(sqlWhereElement)
            Next
        End If

        'add each sub group
        If currentClause IsNot Nothing AndAlso currentClause.SubGroups IsNot Nothing AndAlso currentClause.SubGroups.Count > 0 Then
            For Each subgroup As ClauseWrappers.WhereClauseWrapper.WhereClause In currentClause.SubGroups
                currentSqlWhere.SubGroups.Add(ParseWhereClause(subgroup, takenParamNames))
            Next
        End If

        'add any joins required for this where clause
        If currentClause IsNot Nothing AndAlso currentClause.JoinList.Count > 0 Then
            For Each jc As ClauseWrappers.JoinClauseWrapper.JoinClause In currentClause.JoinList
                currentSqlWhere.JoinList.Add(CType(ParseJoinClause(jc), SqlClauses.SqlJoinClause))
            Next
        End If

        Return currentSqlWhere
    End Function

    ''' <summary>
    ''' Converts a generic orderByClause to an Sql OrderBy Clause object
    ''' </summary>
    ''' <param name="clause">The clause object</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ParseOrderByClause(ByVal clause As ClauseWrappers.OrderByClauseWrapper.OrderByClause) As Object Implements ClauseWrappers.OrderByClauseWrapper.IOrderByClauseHandler.ParseOrderByClause
        'create an empty sqlOrderBy
        Dim orderBy As New SqlClauses.SqlOrderByClause

        'if the list is populated
        If clause IsNot Nothing AndAlso clause.ClauseList IsNot Nothing AndAlso clause.ClauseList.Count > 0 Then
            'loop through each order by clause and convert it to SQL style syntax
            For Each clauseElement As ClauseWrappers.OrderByClauseWrapper.OrderByClauseElement In clause.ClauseList
                Dim columnName, directionText As String

                columnName = clauseElement.FieldName

                Select Case clauseElement.Direction
                    Case ClauseWrappers.OrderByClauseWrapper.Direction.Ascending
                        directionText = "ASC"
                    Case ClauseWrappers.OrderByClauseWrapper.Direction.Descending
                        directionText = "DESC"
                    Case Else
                        directionText = ""
                End Select

                orderBy.ClauseList.Add(New SqlClauses.SqlOrderByClauseElement(columnName, directionText))
            Next
        End If

        Return orderBy
    End Function


    Public Function ParseJoinClause(ByVal clause As ClauseWrappers.JoinClauseWrapper.JoinClause) As Object Implements ClauseWrappers.JoinClauseWrapper.IJoinClauseHandler.ParseJoinClause
        Dim joinClause As New SqlClauses.SqlJoinClause

        If clause IsNot Nothing Then
            joinClause.TargetTable = RemoveSquareBrackets(clause.TargetTable)
            joinClause.TargetColumn = RemoveSquareBrackets(clause.TargetColumn)
            joinClause.SourceTable = RemoveSquareBrackets(clause.SourceTable)
            joinClause.SourceColumn = RemoveSquareBrackets(clause.SourceColumn)
            joinClause.TableAlias = RemoveSquareBrackets(clause.TableAlias)

            joinClause.JoinWhere = CType(ParseWhereClause(clause.JoinWhere), SqlClauses.SqlWhereClause)

            Select Case clause.JoinWhereConjunctionOperator
                Case ClauseWrappers.WhereClauseWrapper.ConjunctionOperator.And
                    joinClause.JoinWhereConjunctionText = "AND"
                Case ClauseWrappers.WhereClauseWrapper.ConjunctionOperator.Or
                    joinClause.JoinWhereConjunctionText = "OR"
                Case Else
                    joinClause.JoinWhereConjunctionText = ""
            End Select


            Select Case clause.JoinType
                Case ClauseWrappers.JoinClauseWrapper.JoinType.InnerJoin
                    joinClause.JoinTypeText = " INNER JOIN "
                Case ClauseWrappers.JoinClauseWrapper.JoinType.LeftJoin
                    joinClause.JoinTypeText = " LEFT JOIN "
                Case Else
                    joinClause.JoinTypeText = ""
            End Select
        End If

        Return joinClause
    End Function

#End Region

    Private Function RemoveSquareBrackets(ByVal name As String) As String
        Return Text.RegularExpressions.Regex.Replace(name, "[\[\]]", "")
    End Function

    Private Function ColumnNameToParamName(ByVal columnName As String) As String
        Return "@" & Text.RegularExpressions.Regex.Replace(columnName, "[^a-zA-Z0-9]", "")
    End Function


End Class
