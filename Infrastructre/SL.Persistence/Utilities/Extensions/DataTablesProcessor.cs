using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using SL.Application.Models.Request;
using SL.Application.Models.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SL.Persistence.Extensions
{
    public static class DataTablesProcessor
    {
        public static async Task<DataTablesResponse<TViewModel>> ToDataTablesResponseAsync<TEntity, TViewModel>(
            this IQueryable<TEntity> query,
            DataTablesRequest dataTableRequest,
            IMapper mapper) where TEntity : class
        {
            int recordsTotal = await query.CountAsync();
            ExpressionStarter<TEntity> predicate = PredicateBuilder.New<TEntity>(true);


            if (!string.IsNullOrWhiteSpace(dataTableRequest.Search?.Value))
            {
                ExpressionStarter<TEntity> globalSearchPredicate = PredicateBuilder.New<TEntity>(false);
                string globalSearchValue = dataTableRequest.Search.Value.ToLower();

                foreach (ColumnRequest? columnRequest in dataTableRequest.Columns.Where(column => column.Searchable && !string.IsNullOrWhiteSpace(column.Name)))
                {
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "e");


                    Expression propertyExpression = parameterExpression;
                    foreach (string member in columnRequest.Name.Split('.'))
                    {
                        propertyExpression = Expression.PropertyOrField(propertyExpression, member);
                    }

                    if (propertyExpression.Type == typeof(string))
                    {
                        MethodInfo? toLowerMethodInfo = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                        MethodInfo? containsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                        MethodCallExpression memberLowerExpression = Expression.Call(propertyExpression, toLowerMethodInfo);
                        ConstantExpression constantValue = Expression.Constant(globalSearchValue, typeof(string));
                        MethodCallExpression containsExpression = Expression.Call(memberLowerExpression, containsMethodInfo, constantValue);

                        Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameterExpression);
                        globalSearchPredicate = globalSearchPredicate.Or(lambda);
                    }
                }
                predicate = predicate.And(globalSearchPredicate);
            }


            ExpressionStarter<TEntity> columnFilterPredicate = PredicateBuilder.New<TEntity>(true);
            foreach (ColumnRequest? columnRequest in dataTableRequest.Columns.Where(column => column.Searchable && !string.IsNullOrWhiteSpace(column.Search?.Value) && !string.IsNullOrWhiteSpace(column.Name)))
            {
                try
                {
                    ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "e");
                    MemberExpression propertyExpression = Expression.Property(parameterExpression, columnRequest.Name);

                    object constantValue;
                    Type propertyType = propertyExpression.Type;

                    Type? underlyingType = Nullable.GetUnderlyingType(propertyType);
                    if (underlyingType != null)
                    {
                        propertyType = underlyingType;
                    }

                    if (propertyType.IsEnum)
                    {
                        if (Enum.TryParse(propertyType, columnRequest.Search.Value, out object? parsedEnum))
                        {
                            constantValue = parsedEnum;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(propertyExpression.Type);
                        if (!converter.CanConvertFrom(typeof(string)) || !converter.IsValid(columnRequest.Search.Value))
                        {
                            continue;
                        }
                        constantValue = converter.ConvertFromInvariantString(columnRequest.Search.Value);
                    }

                    ConstantExpression constantExpression = Expression.Constant(constantValue, propertyExpression.Type);
                    BinaryExpression comparisonExpression = Expression.Equal(propertyExpression, constantExpression);

                    Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(comparisonExpression, parameterExpression);
                    columnFilterPredicate = columnFilterPredicate.And(lambda);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Column filter error for '{columnRequest.Name}': {ex.Message}");
                }
            }
            predicate = predicate.And(columnFilterPredicate);

            query = query.Where(predicate);
            int recordsFiltered = await query.CountAsync();


            if (dataTableRequest.Order != null && dataTableRequest.Order.Any())
            {
                List<string> orderingClauses = new List<string>();
                foreach (OrderRequest orderRequest in dataTableRequest.Order)
                {
                    string columnName = dataTableRequest.Columns[orderRequest.Column].Name;
                    if (!string.IsNullOrWhiteSpace(columnName))
                    {
                        orderingClauses.Add($"{columnName} {orderRequest.Dir}");
                    }
                }
                if (orderingClauses.Any())
                {
                    query = query.OrderBy(string.Join(", ", orderingClauses));
                }
            }


            List<TViewModel> pagedData = await query
                .Skip(dataTableRequest.Start)
                .Take(dataTableRequest.Length)
                .ProjectTo<TViewModel>(mapper.ConfigurationProvider)
                .ToListAsync();

            return new DataTablesResponse<TViewModel>
            {
                Draw = dataTableRequest.Draw,
                RecordsTotal = recordsTotal,
                RecordsFiltered = recordsFiltered,
                Data = pagedData
            };
        }
    }
}