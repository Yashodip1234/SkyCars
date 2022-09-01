using SkyCars.Core;
using SkyCars.Core.DomainEntity.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SkyCars.Data.Extensions
{
    public enum OperatorComparer
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals = ExpressionType.Equal,
        GreaterThan = ExpressionType.GreaterThan,
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,
        LessThan = ExpressionType.LessThan,
        LessThanOrEqual = ExpressionType.LessThanOrEqual,
        NotEqual = ExpressionType.NotEqual
    }
    public static class DynamicLinqExpressionBuilder
    {
        public static async Task<IPagedList<T>> BuildPredicate<T>(this IQueryable<T> query, GridRequestModel objGrid)
        {
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name);
            //for change between to greater and less (one item to two item)
            var datefilters = objGrid.SearchParams.Where(x => x.OpType.ToUpper() == "BETWEEN").ToList();
            foreach (var objFilter in datefilters)
            {
                objGrid.SearchParams.Add(new SearchGrid() { FieldName = objFilter.FieldName, FieldValue = objFilter.FieldValue.Split('-')[0], OpType = "GreaterThanOrEqual" });
                objFilter.FieldValue = objFilter.FieldValue.Split('-')[1];
                objFilter.OpType = "LessThanOrEqual";
            }
            foreach (var objFilter in objGrid.SearchParams)
            {
                var predicate = (Expression<Func<T, bool>>)BuildNavigationExpression(parameterExpression, (OperatorComparer)Enum.Parse(typeof(OperatorComparer), objFilter.OpType), objFilter.FieldValue, objFilter.FieldName);
                query = query.Where(predicate);
            }
            #region :: check delete column exist or not ::
            if (!string.IsNullOrEmpty(parameterExpression.Type.GetProperty("IsDeleted")?.Name ?? ""))
            {
                var r = (Expression<Func<T, bool>>)BuildCondition(parameterExpression, "IsDeleted", OperatorComparer.Equals, "false");
                query = query.Where(r);
            }
            #endregion
            if ((objGrid?.Order?.Count ?? 0) > 0)
            {
                query = query.OrderBy(objGrid.Columns[objGrid.Order[0].Column].Data, objGrid.Order[0].Dir == "desc");
            }
            return await query.ToPagedListAsync(objGrid.Start, objGrid.Length,(int)objGrid.ExportType!=0 ? true:false);
        }
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc)
        {
            string command = desc ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
        private static Expression BuildNavigationExpression(Expression parameter, OperatorComparer comparer, object value, params string[] properties)
        {
            Expression childParameter, predicate;
            Type childType = null;

            Expression resultExpression;
            if (properties.Length > 1)
            {
                //build path
                parameter = Expression.Property(parameter, properties[0]);
                var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.Type);
                //if it´s a collection we later need to use the predicate in the methodexpressioncall
                if (isCollection)
                {
                    childType = parameter.Type.GetGenericArguments()[0];
                    childParameter = Expression.Parameter(childType, childType.Name);
                }
                else
                {
                    childParameter = parameter;
                }
                //skip current property and get navigation property expression recursivly
                var innerProperties = properties.Skip(1).ToArray();
                predicate = BuildNavigationExpression(childParameter, comparer, value, innerProperties);
                if (isCollection)
                {
                    //build subquery
                    resultExpression = BuildSubQuery(parameter, childType, predicate);
                }
                else
                {
                    resultExpression = predicate;
                }
            }
            else
            {
                //build final predicate
                resultExpression = BuildCondition(parameter, properties[0], comparer, value);
            }
            return resultExpression;
        }

        private static Expression BuildSubQuery(Expression parameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            predicate = Expression.Call(anyMethod, parameter, predicate);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildCondition(Expression parameter, string property, OperatorComparer comparer, object value)
        {
            var childProperty = parameter.Type.GetProperty(property);
            var left = Expression.Property(parameter, childProperty);
            var right = Expression.Constant(value);
            var predicate = BuildComparsion(left, comparer, right);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildComparsion(Expression left, OperatorComparer comparer, Expression right)
        {
            var mask = new List<OperatorComparer> {
                OperatorComparer.Contains,
                OperatorComparer.StartsWith,
                OperatorComparer.EndsWith
            };
            if (mask.Contains(comparer) && left.Type != typeof(string))
            {
                comparer = OperatorComparer.Equals;
            }
            if (!mask.Contains(comparer))
            {
                if (left.Type.Name.ToLower() == "string")
                {
                    return Expression.MakeBinary((ExpressionType)comparer, left, Expression.Convert(right, left.Type));
                }
                else if (left.Type.BaseType.Name.ToLower() == "enum" || left.Type.Name == "Nullable`1")
                {
                    return Expression.MakeBinary((ExpressionType)comparer, Expression.Call(Expression.Convert(left, typeof(int)), typeof(object).GetMethod("ToString")), right);
                }
                else
                {
                    return Expression.MakeBinary((ExpressionType)comparer, left, Expression.Call(left.Type, "Parse", null, right));
                }
            }
            return BuildStringCondition(left, comparer, right);
        }


        private static Expression BuildStringCondition(Expression left, OperatorComparer comparer, Expression right)
        {
            var compareMethod = typeof(string).GetMethods().FirstOrDefault(m => m.Name.Equals(Enum.GetName(typeof(OperatorComparer), comparer)) && m.GetParameters().Length == 1);
            //we assume ignoreCase, so call ToLower on paramter and memberexpression
            var toLowerMethod = typeof(string).GetMethods().FirstOrDefault(m => m.Name.Equals("ToLower") && m.GetParameters().Length == 0);
            left = Expression.Call(left, toLowerMethod);
            right = Expression.Call(right, toLowerMethod);
            return Expression.Call(left, compareMethod, right);
        }

        private static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }
    }
}
