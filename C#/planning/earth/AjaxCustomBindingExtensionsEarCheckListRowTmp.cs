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
using System.Web.Http;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingExtensionsEarCheckListRowTmp
    {
        public static IQueryable<EAR_CHECK_LIST_ROW_TMP> ApplyEAR_CHECK_LIST_ROW_TMPPaging(this IQueryable<EAR_CHECK_LIST_ROW_TMP> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IEnumerable ApplyEAR_CHECK_LIST_ROW_TMPGrouping(this IQueryable<EAR_CHECK_LIST_ROW_TMP> data, IList<GroupDescriptor> groupDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                Func<IEnumerable<EAR_CHECK_LIST_ROW_TMP>, IEnumerable<AggregateFunctionsGroup>> selector = null;
                foreach (var group in groupDescriptors.Reverse())
                {
                    if (selector == null)
                    {
                        if (group.Member == "CHTR_DESC")
                        {
                            selector = Earths => BuildInnerGroup(Earths, o => o.CHTR_DESC);
                        }
                        else if (group.Member == "CHTR_WEIGHT")
                        {
                            selector = Earths => BuildInnerGroup(Earths, o => o.CHTR_WEIGHT);
                        }
                    }
                    else
                    {
                        if (group.Member == "CHTR_DESC")
                        {
                            selector = BuildGroup(o => o.CHTR_DESC, selector);
                        }
                        else if (group.Member == "CHTR_WEIGHT")
                        {
                            selector = BuildGroup(o => o.CHTR_WEIGHT, selector);
                        }
                    }
                }

                return selector.Invoke(data).ToList();
            }

            return data.ToList();
        }

        public static IQueryable<EAR_CHECK_LIST_ROW_TMP> ApplyEAR_CHECK_LIST_ROW_TMPSorting(this IQueryable<EAR_CHECK_LIST_ROW_TMP> data,
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

        public static IQueryable<EAR_CHECK_LIST_ROW_TMP> ApplyEAR_CHECK_LIST_ROW_TMPFiltering(this IQueryable<EAR_CHECK_LIST_ROW_TMP> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EAR_CHECK_LIST_ROW_TMP>(filterDescriptors));
            }
            return data;
        }

        private static Func<IEnumerable<EAR_CHECK_LIST_ROW_TMP>, IEnumerable<AggregateFunctionsGroup>>
        BuildGroup<T>(Expression<Func<EAR_CHECK_LIST_ROW_TMP, T>> groupSelector, Func<IEnumerable<EAR_CHECK_LIST_ROW_TMP>,
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<EAR_CHECK_LIST_ROW_TMP> group, Expression<Func<EAR_CHECK_LIST_ROW_TMP, T>> groupSelector)
        {
            return group.GroupBy(groupSelector.Compile())
                        .Select(i => new AggregateFunctionsGroup
                               {
                                   Key = i.Key,
                                   Member = groupSelector.MemberWithoutInstance(),
                                   Items = i.ToList()
                               });
        }

        private static IQueryable<EAR_CHECK_LIST_ROW_TMP> AddSortExpression(IQueryable<EAR_CHECK_LIST_ROW_TMP> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "CHTR_ROW":
                        data = data.OrderBy(earth => earth.CHTR_ROW);
                        break;
                    case "CHTR_DESC":
                        data = data.OrderBy(earth => earth.CHTR_DESC);
                        break;
                    case "CHTR_WEIGHT":
                        data = data.OrderBy(earth => earth.CHTR_WEIGHT);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "CHTR_ROW":
                        data = data.OrderByDescending(earth => earth.CHTR_ROW);
                        break;
                    case "CHTR_DESC":
                        data = data.OrderByDescending(earth => earth.CHTR_DESC);
                        break;
                    case "CHTR_WEIGHT":
                        data = data.OrderByDescending(earth => earth.CHTR_WEIGHT);
                        break;
                }
            }
            return data;
        }
    }
}