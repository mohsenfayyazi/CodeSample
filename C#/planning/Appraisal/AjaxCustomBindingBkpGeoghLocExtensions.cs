using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Equipment.Models;
using Kendo.Mvc;
using Asr.Base;

namespace Equipment.Controllers.Planning.Appraisal
{
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingBkpGeoghLocExtensions
    {
        //EAR_EARTH_CHECK_LIST
        public static IQueryable<BKP_GEOGH_LOC> ApplyBkpGeoghLocPaging(this IQueryable<BKP_GEOGH_LOC> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IQueryable<BKP_GEOGH_LOC> ApplyBkpGeoghLocSorting(this IQueryable<BKP_GEOGH_LOC> data,
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

        public static IQueryable<BKP_GEOGH_LOC> ApplyBkpGeoghLocFiltering(this IQueryable<BKP_GEOGH_LOC> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<BKP_GEOGH_LOC>(filterDescriptors));
            }
            return data;
        }

        private static IQueryable<BKP_GEOGH_LOC> AddSortExpression(IQueryable<BKP_GEOGH_LOC> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "G_CODE":
                        data = data.OrderBy(chklst => chklst.G_CODE);
                        break;
                    case "G_DESC":
                        data = data.OrderBy(chklst => chklst.G_DESC);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "G_CODE":
                        data = data.OrderByDescending(chklst => chklst.G_CODE);
                        break;
                    case "G_DESC":
                        data = data.OrderByDescending(chklst => chklst.G_DESC);
                        break;
                }
            }
            return data;
        }
    }
}
