using Asr.Base;
using Equipment.Models;
using Kendo.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Equipment.Controllers.Planning.Earth
{
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingEarEarthCheckListExtensions
    {
        //EAR_EARTH_CHECK_LIST
        public static IQueryable<EAR_EARTH_CHECK_LIST> ApplyEarthsPaging(this IQueryable<EAR_EARTH_CHECK_LIST> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IQueryable<EAR_EARTH_CHECK_LIST> ApplyEarEarthCheckListSorting(this IQueryable<EAR_EARTH_CHECK_LIST> data,
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

        public static IQueryable<EAR_EARTH_CHECK_LIST> ApplyEarEarthCheckListFiltering(this IQueryable<EAR_EARTH_CHECK_LIST> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EAR_EARTH_CHECK_LIST>(filterDescriptors));
            }
            return data;
        }

        private static IQueryable<EAR_EARTH_CHECK_LIST> AddSortExpression(IQueryable<EAR_EARTH_CHECK_LIST> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "ECHL_ID":
                        data = data.OrderBy(chklst => chklst.ECHL_ID);
                        break;
                    case "ECHL_DESC":
                        data = data.OrderBy(chklst => chklst.ECHL_DESC);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "ERTH_ID":
                        data = data.OrderByDescending(chklst => chklst.ECHL_ID);
                        break;
                    case "ERTH_NAME":
                        data = data.OrderByDescending(chklst => chklst.ECHL_DESC);
                        break;
                }
            }
            return data;
        }
    }
}