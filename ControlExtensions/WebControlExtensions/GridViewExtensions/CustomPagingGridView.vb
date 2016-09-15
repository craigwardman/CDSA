Imports System.Web.UI.WebControls

Namespace WebControlExtensions.GridViewExtensions
    Public Class CustomPagingGridView
        Inherits GridView

        'remember the virtual item count
        Public Property VirtualItemCount() As Integer
            Get
                If ViewState("gv_virtualItemCount") Is Nothing Then ViewState("gv_virtualItemCount") = 0

                Return CInt(ViewState("gv_virtualItemCount"))
            End Get
            Set(ByVal value As Integer)
                ViewState("gv_virtualItemCount") = value
            End Set
        End Property

        'remember the current page
        Private Property CurrentPageIndex() As Integer
            Get
                If ViewState("gv_CurrentPageIndex") Is Nothing Then ViewState("gv_CurrentPageIndex") = 0
                Return CInt(ViewState("gv_CurrentPageIndex"))
            End Get
            Set(ByVal value As Integer)
                ViewState("gv_CurrentPageIndex") = value
            End Set
        End Property

        Public Overrides Property PageIndex() As Integer
            Get
                Return CurrentPageIndex
            End Get
            Set(ByVal value As Integer)
                MyBase.PageIndex = value
                CurrentPageIndex = value
            End Set
        End Property

        Public ReadOnly Property TotalPages() As Integer
            Get
                Return CInt(Math.Ceiling(VirtualItemCount / PageSize))
            End Get
        End Property

        Public Overrides Property DataSource() As Object
            Get
                Return MyBase.DataSource
            End Get
            Set(ByVal value As Object)
                MyBase.DataSource = value

                'save the page index for when the pager is redrawn for the new datasource
                CurrentPageIndex = PageIndex
            End Set
        End Property

        'make this read/write so it can be set/handled by the client app
        Public Shadows Property SortExpression() As String
            Get
                If ViewState("gv_SortExpression") Is Nothing Then ViewState("gv_SortExpression") = String.Empty
                Return CStr(ViewState("gv_SortExpression"))
            End Get
            Set(ByVal value As String)
                ViewState("gv_SortExpression") = value
            End Set
        End Property

        'make this read/write so it can be set/handled by the client app
        Public Shadows Property SortDirection() As System.Web.UI.WebControls.SortDirection
            Get
                If ViewState("gv_SortDirection") Is Nothing Then ViewState("gv_SortDirection") = SortDirection.Ascending
                Return CType(ViewState("gv_SortDirection"), SortDirection)
            End Get
            Set(ByVal value As System.Web.UI.WebControls.SortDirection)
                ViewState("gv_SortDirection") = value
            End Set
        End Property

        Protected Overrides Sub InitializePager(ByVal row As System.Web.UI.WebControls.GridViewRow, ByVal columnSpan As Integer, ByVal pagedDataSource As System.Web.UI.WebControls.PagedDataSource)
            'make the grid provide paging
            pagedDataSource.AllowCustomPaging = True
            pagedDataSource.VirtualCount = VirtualItemCount
            pagedDataSource.CurrentPageIndex = CurrentPageIndex

            If Not SeoPagingEnabled Then
                MyBase.InitializePager(row, columnSpan, pagedDataSource)
            Else
                'render the seo paging stuff into the row
                DrawSeoPager(row, columnSpan, pagedDataSource)
            End If
        End Sub

        Protected Overrides Sub OnSorting(ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs)
            'dont use the base class sorting
            e.Cancel = True

            'invert the sortdirection if its a sort on the same column
            If MyClass.SortExpression = e.SortExpression Then
                Select Case MyClass.SortDirection
                    Case SortDirection.Ascending
                        e.SortDirection = SortDirection.Descending
                    Case SortDirection.Descending
                        e.SortDirection = SortDirection.Ascending
                End Select
            End If

            'remember the current page after the sort completes
            MyBase.PageIndex = CurrentPageIndex

            'raise the event to client app
            MyBase.OnSorting(e)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            'if its the last page and the page isnt full, make sure blank items dont appear
            If PageSize > 0 Then
                If CurrentPageIndex + 1 = Math.Ceiling(VirtualItemCount / PageSize) And VirtualItemCount Mod PageSize <> 0 Then
                    Dim maxItems As Integer = VirtualItemCount Mod PageSize
                    For Each row As GridViewRow In MyClass.Rows
                        If row.DataItemIndex > maxItems Then row.Visible = False
                    Next
                End If
            End If
        End Sub

#Region "SEO Paging"

        Public Property SeoPagingEnabled() As Boolean
            Get
                Return CBool(ViewState("SeoPagingEnabled"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("SeoPagingEnabled") = value
            End Set
        End Property

        Public Property SeoPagingLinkUrl() As String
            Get
                If ViewState("SeoPagingLinkUrl") Is Nothing Then ViewState("SeoPagingLinkUrl") = ""
                Return CStr(ViewState("SeoPagingLinkUrl"))
            End Get
            Set(ByVal value As String)
                ViewState("SeoPagingLinkUrl") = value
            End Set
        End Property

        Public Property AppendQueryString() As String
            Get
                Return CStr(ViewState("AppendQuerySting"))
            End Get
            Set(ByVal value As String)
                ViewState("AppendQuerySting") = value
            End Set
        End Property

        Public Property SeoPagingLinkPage1Url() As String
            Get
                If ViewState("SeoPagingLinkPage1Url") Is Nothing Then ViewState("SeoPagingLinkPage1Url") = ""
                Return CStr(ViewState("SeoPagingLinkPage1Url"))
            End Get
            Set(ByVal value As String)
                ViewState("SeoPagingLinkPage1Url") = value
            End Set
        End Property

        Private Sub DrawSeoPager(ByVal row As System.Web.UI.WebControls.GridViewRow, ByVal columnSpan As Integer, ByVal pagedDataSource As System.Web.UI.WebControls.PagedDataSource)
            Dim currentPage As Integer = pagedDataSource.CurrentPageIndex + 1

            'show the pages in groups of PAGE_GROUP_SIZE
            Dim pageGroupStart As Integer = CInt(Math.Floor(currentPage / PagerSettings.PageButtonCount) * PagerSettings.PageButtonCount)

            'always put the current page number in the middle (if possible) of the page group size
            Dim middleButtonPos As Integer = CInt(Math.Floor(PagerSettings.PageButtonCount / 2))

            pageGroupStart += ((currentPage Mod PagerSettings.PageButtonCount) - middleButtonPos)
            If pageGroupStart < 1 Then pageGroupStart = 1
            If pageGroupStart + PagerSettings.PageButtonCount > Math.Ceiling(VirtualItemCount / PageSize) Then
                pageGroupStart = CInt(Math.Ceiling(VirtualItemCount / PageSize)) - PagerSettings.PageButtonCount
                If pageGroupStart < 1 Then pageGroupStart = 1

                While pageGroupStart + PagerSettings.PageButtonCount <= currentPage
                    pageGroupStart += 1
                End While
            End If


            'create the row cell
            row.Cells.Add(New TableCell)
            row.Cells(0).ColumnSpan = columnSpan

            'show a pagegroup back button
            'If pageGroupStart > 0 Then
            '    row.Cells(0).Text &= "<a style='color: #666;' href='" & SeoPagingLinkUrl.Replace(".aspx", "") & CStr(IIf(pageGroupStart - PagerSettings.PageButtonCount > 0, pageGroupStart - PagerSettings.PageButtonCount, 1)) & ".aspx" & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.FirstPageText & "</a> | "
            'Else
            '    row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.FirstPageText & "</span> | "
            'End If
            If currentPage > 1 Then
                row.Cells(0).Text &= "<a style='color: #666;' href='" & SeoPagingLinkPage1Url & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.FirstPageText & "</a> | "
            Else
                row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.FirstPageText & "</span> | "
            End If

            'always show the previous button
            If currentPage > 1 Then
                row.Cells(0).Text &= "<a style='color: #666;' href='" & CStr(IIf(currentPage - 1 > 1, SeoPagingLinkUrl.Replace(".aspx", "") & currentPage - 1 & ".aspx", SeoPagingLinkPage1Url)) & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.PreviousPageText & "</a> | "
            Else
                row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.PreviousPageText & "</span> | "
            End If

            'show the page numbers
            For i As Integer = pageGroupStart To pageGroupStart + (PagerSettings.PageButtonCount - 1)
                'stop when we get to the end
                If i > pagedDataSource.PageCount Then Exit For

                If i <> 0 Then
                    If i <> currentPage Then
                        'page 1 is generally the default page (/) and therefore dont duplicate the content (if we have the link)
                        If (i = 1) And SeoPagingLinkPage1Url <> "" Then
                            row.Cells(0).Text &= "<a href='" & SeoPagingLinkPage1Url & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & i.ToString().PadLeft(2, "0"c) & "</a> | "
                        Else
                            row.Cells(0).Text &= "<a href='" & SeoPagingLinkUrl.Replace(".aspx", "") & i & ".aspx" & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & i.ToString().PadLeft(2, "0"c) & "</a> | "
                        End If

                    Else
                        row.Cells(0).Text &= i.ToString().PadLeft(2, "0"c) & " | "
                    End If

                End If
            Next


            'always show the next button
            If (currentPage + 1) <= TotalPages Then
                row.Cells(0).Text &= "<a style='color: #666;' href='" & SeoPagingLinkUrl.Replace(".aspx", "") & currentPage + 1 & ".aspx" & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.NextPageText & "</a> | "
            Else
                row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.NextPageText & "</span> | "
            End If

            'show a page forward button
            If (currentPage + 1) <= TotalPages Then
                row.Cells(0).Text &= "<a style='color: #666;' href='" & SeoPagingLinkUrl.Replace(".aspx", "") & Math.Ceiling(VirtualItemCount / PageSize) & ".aspx" & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.LastPageText & "</a> | "
            Else
                row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.LastPageText & "</span>"
            End If
            'If (pageGroupStart + PagerSettings.PageButtonCount) <= TotalPages Then
            '    row.Cells(0).Text &= "<a style='color: #666;' href='" & SeoPagingLinkUrl.Replace(".aspx", "") & pageGroupStart + PagerSettings.PageButtonCount & ".aspx" & CStr(IIf(AppendQueryString <> "", "?" & AppendQueryString, "")) & "'>" & PagerSettings.LastPageText & "</a>"
            'Else
            '    row.Cells(0).Text &= "<span style='color: #CCC;'>" & PagerSettings.LastPageText & "</span>"
            'End If


        End Sub

#End Region

    End Class

End Namespace
