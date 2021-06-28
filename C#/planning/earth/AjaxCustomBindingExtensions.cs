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
    public static class AjaxCustomBindingExtensions
    {
        public static IQueryable<EAR_EARTH> ApplyEarthsPaging(this IQueryable<EAR_EARTH> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IEnumerable ApplyEarthsGrouping(this IQueryable<EAR_EARTH> data, IList<GroupDescriptor> groupDescriptors)
        {
            if (groupDescriptors != null && groupDescriptors.Any())
            {
                Func<IEnumerable<EAR_EARTH>, IEnumerable<AggregateFunctionsGroup>> selector = null;
                foreach (var group in groupDescriptors.Reverse())
                {
                    if (selector == null)
                    {
                        if (group.Member == "ERTH_NAME")
                        {
                            selector = earths => BuildInnerGroup(earths, o => o.ERTH_NAME);
                        }
                        else if (group.Member == "ERTH_ADDRESS")
                        {
                            selector = earths => BuildInnerGroup(earths, o => o.ERTH_ADDRESS);
                        }
                        else if (group.Member == "ERTH_AREA")
                        {
                            selector = earths => BuildInnerGroup(earths, o => o.ERTH_AREA);
                        }
                        else if (group.Member == "ERTH_OWNERSHIP")
                        {
                            selector = earths => BuildInnerGroup(earths, o => o.ERTH_OWNERSHIP);
                        }
                    }
                    else
                    {
                        if (group.Member == "ERTH_NAME")
                        {
                            selector = BuildGroup(o => o.ERTH_NAME, selector);
                        }
                        else if (group.Member == "ERTH_ADDRESS")
                        {
                            selector = BuildGroup(o => o.ERTH_ADDRESS, selector);
                        }
                        else if (group.Member == "ERTH_AREA")
                        {
                            selector = BuildGroup(o => o.ERTH_AREA, selector);
                        }
                        else if (group.Member == "ERTH_OWNERSHIP")
                        {
                            selector = BuildGroup(o => o.ERTH_OWNERSHIP, selector);
                        }
                    }
                }

                return selector.Invoke(data).ToList();
            }

            return data.ToList();
        }

        public static IQueryable<EAR_EARTH> ApplyEarthSorting(this IQueryable<EAR_EARTH> data,
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

        public static IQueryable<EAR_EARTH> ApplyEarthFiltering(this IQueryable<EAR_EARTH> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EAR_EARTH>(filterDescriptors));
            }
            return data;
        }

        private static Func<IEnumerable<EAR_EARTH>, IEnumerable<AggregateFunctionsGroup>>
        BuildGroup<T>(Expression<Func<EAR_EARTH, T>> groupSelector, Func<IEnumerable<EAR_EARTH>,
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

        private static IEnumerable<AggregateFunctionsGroup> BuildInnerGroup<T>(IEnumerable<EAR_EARTH> group, Expression<Func<EAR_EARTH, T>> groupSelector)
        {
            return group.GroupBy(groupSelector.Compile())
                        .Select(i => new AggregateFunctionsGroup
                               {
                                   Key = i.Key,
                                   Member = groupSelector.MemberWithoutInstance(),
                                   Items = i.ToList()
                               });
        }

        private static IQueryable<EAR_EARTH> AddSortExpression(IQueryable<EAR_EARTH> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "ERTH_ID":
                        data = data.OrderBy(earth => earth.ERTH_ID);
                        break;
                    case "ERTH_NAME":
                        data = data.OrderBy(earth => earth.ERTH_NAME);
                        break;
                    case "ERTH_ADDRESS":
                        data = data.OrderBy(earth => earth.ERTH_ADDRESS);
                        break;
                    case "ERTH_AREA":
                        data = data.OrderBy(earth => earth.ERTH_AREA);
                        break;
                    case "ERTH_OWNERSHIP":
                        data = data.OrderBy(earth => earth.ERTH_OWNERSHIP);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "ERTH_ID":
                        data = data.OrderByDescending(earth => earth.ERTH_ID);
                        break;
                    case "ERTH_NAME":
                        data = data.OrderByDescending(earth => earth.ERTH_NAME);
                        break;
                    case "ERTH_ADDRESS":
                        data = data.OrderByDescending(earth => earth.ERTH_ADDRESS);
                        break;
                    case "ERTH_AREA":
                        data = data.OrderByDescending(earth => earth.ERTH_AREA);
                        break;
                    case "ERTH_OWNERSHIP":
                        data = data.OrderByDescending(earth => earth.ERTH_OWNERSHIP);
                        break;
                }
            }
            return data;
        }
    }
}