using ClauseWrappers.OrderByClauseWrapper;

namespace CDSA.Helpers
{
    public class StringToOrderByClause
    {
        public static OrderByClause GetOrderByClause(string sortExpression, DDL.Definitions.Schemas.ISchemaBase schema)
        {
            // work out the orderBy from the current expression
            if (!string.IsNullOrEmpty(sortExpression))
            {
                OrderByClause orderBy = new OrderByClause();

                if (sortExpression.Contains(","))
                {
                    string[] exprs = sortExpression.Split(',');

                    foreach (string expr in exprs)
                    {
                        OrderByClauseElement myOrderByClauseElem = new OrderByClauseElement();

                        if (expr.Contains(" ") && (expr.ToUpper().EndsWith("ASC") || expr.ToUpper().EndsWith("DESC")))
                        {
                            myOrderByClauseElem.FieldName = SchemaCatalog.GetFieldNameByPropertyName(schema, expr.Split(' ')[0]);

                            switch (expr.ToUpper().Split(' ')[1])
                            {
                                case "ASC":
                                    myOrderByClauseElem.Direction = Direction.Ascending;
                                    break;

                                case "DESC":
                                    myOrderByClauseElem.Direction = Direction.Descending;
                                    break;
                            }
                        }
                        else
                        {
                            myOrderByClauseElem.FieldName = SchemaCatalog.GetFieldNameByPropertyName(schema, expr);
                            myOrderByClauseElem.Direction = Direction.Ascending;
                        }

                        orderBy.ClauseList.Add(myOrderByClauseElem);
                    }
                }
                else
                {
                    OrderByClauseElement myOrderByClauseElem = new OrderByClauseElement();

                    if (sortExpression.Contains(" ") && (sortExpression.ToUpper().EndsWith("ASC") || sortExpression.ToUpper().EndsWith("DESC")))
                    {
                        myOrderByClauseElem.FieldName = SchemaCatalog.GetFieldNameByPropertyName(schema, sortExpression.Split(' ')[0]);

                        switch (sortExpression.ToUpper().Split(' ')[1])
                        {
                            case "ASC":
                                myOrderByClauseElem.Direction = Direction.Ascending;
                                break;

                            case "DESC":
                                myOrderByClauseElem.Direction = Direction.Descending;
                                break;
                        }
                    }
                    else
                    {
                        myOrderByClauseElem.FieldName = SchemaCatalog.GetFieldNameByPropertyName(schema, sortExpression);
                        myOrderByClauseElem.Direction = Direction.Ascending;
                    }

                    orderBy.ClauseList.Add(myOrderByClauseElem);
                }

                return orderBy;
            }
            else
            {
                return null;
            }
        }
    }
}