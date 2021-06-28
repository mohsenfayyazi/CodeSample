using Asr.Base;
using Equipment.Models;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Equipment.Controllers.Planning.Earth
{
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingExtensionsEarCheckListRow
    {
        public static IQueryable<EAR_CHECK_LIST_ROW> ApplyEAR_CHECK_LIST_ROWPaging(this IQueryable<EAR_CHECK_LIST_ROW> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IEnumerable ApplyEAR_CHECK_LIST_ROWGrouping(this IQueryable<EAR_CHECK_LIST_ROW> data, IList<GroupDescriptor> groupDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                Func<IEnumerable<EAR_CHECK_LIST_ROW>, IEnumerable<AggregateFunctionsGroup>> selector = null;
                foreach (var group in groupDescriptors.Reverse())
                {
                    if (selector == null)
                    {
                        if (group.Member == "CHLR_DESC")
                        {
                            selector = Earths => BuildInnerGroup(Earths, o => o.CHLR_DESC);
                        }
                        else if (group.Member == "CHLR_WEIGHT")
                        {
                            selector = Earths => BuildInnerGroup(Earths, o => o.CHLR_WEIGHT);
                        }
                    }
                    else
                    {
                        if (group.Member == "CHLR_DESC")
                        {
                            selector = BuildGroup(o => o.CHLR_DESC, selector);
                        }
                        else if (group.Member == "CHLR_WEIGHT")
                        {
                            selector = BuildGroup(o => o.CHLR_WEIGHT, selector);
                        }
                    }
                }

                return selector.Invoke(data).ToList();
            }

            return data.ToList();
        }

        public static IQueryable<EAR_CHECK_LIST_ROW> ApplyEAR_CHECK_LIST_ROWSorting(this IQueryable<EAR_CHECK_LIST_ROW> data,
            IList<GroupDescriptor> groupDescriptors, IList<SortDescriptor> sortDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                foreach (var groupDescriptor in groupDescriptors.Reverse())
                {
                    data = AddSortExpression(data, groupDescriptor.SortDirection, groupDescriptor.Member);
                }
            }

            if (sortDescriptors != null && sortDescriptors.Any())
            {
                foreach (SortDescriptor sortDescriptor in sortDescriptors)
                {
                    data = AddSortExpression(data, sortDescriptor.SortDirection, sortDescriptor.Member);
                }
            }

            return data;
        }

        public static IQueryable<EAR_CHECK_LIST_ROW> ApplyEAR_CHECK_LIST_ROWFiltering(this IQueryable<EAR_CHECK_LIST_ROW> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EAR_CHECK_LIST_ROW>(filterDescriptors));
            }
            return data;
        }

        private static Func<IEnumerable<EAR_CHECK_LIST_ROW>, IEnumerable<AggregateFunctionsGroup>>
        BuildGroup<T>(Expression<Func<EAR_CHECK_LIST_ROW, T>> groupSelector, Func<IEnumerable<EAR_CHECK_LIST_ROW>,
            IEnumerable<AggregateFunctionsGroup>> selectorBuilder)
        {
            var tempSelector = selectorBuilder;
            return g => g.GroupBy(groupSelector.Compile())
                         .Select(c => new AggregateFunctionsGroup
                                {
                                    Key = c.Key,
                                    HasSubgroups = true,
                                    Member = groupSelector.MemberWithoutInstance(),
                                    Items = tempSelector.Invoke(c).ToList()
                                });
        }

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<EAR_CHECK_LIST_ROW> group, Expression<Func<EAR_CHECK_LIST_ROW, T>> groupSelector)
        {
            return group.GroupBy(groupSelector.Compile())
                        .Select(i => new AggregateFunctionsGroup
                               {
                                   Key = i.Key,
                                   Member = groupSelector.MemberWithoutInstance(),
                                   Items = i.ToList()
                               });
        }

        private static IQueryable<EAR_CHECK_LIST_ROW> AddSortExpression(IQueryable<EAR_CHECK_LIST_ROW> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "CHLR_ROW":
                        data = data.OrderBy(chklst => chklst.CHLR_ROW);
                        break;
                    case "CHLR_DESC":
                        data = data.OrderBy(chklst => chklst.CHLR_DESC);
                        break;
                    case "CHLR_WEIGHT":
                        data = data.OrderBy(chklst => chklst.CHLR_WEIGHT);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "CHLR_ROW":
                        data = data.OrderByDescending(chklst => chklst.CHLR_ROW);
                        break;
                    case "CHLR_DESC":
                        data = data.OrderByDescending(chklst => chklst.CHLR_DESC);
                        break;
                    case "CHLR_WEIGHT":
                        data = data.OrderByDescending(chklst => chklst.CHLR_WEIGHT);
                        break;
                }
            }
            return data;
        }
    }
}