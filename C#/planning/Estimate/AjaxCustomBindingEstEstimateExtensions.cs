using Asr.Base;
using Equipment.Models;
using Kendo.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Equipment.Controllers.Planning.Estimate
{
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingEstEstimateExtensions
    {
        public static IQueryable<EST_ESTIMATE> ApplyEstEstimatePaging(this IQueryable<EST_ESTIMATE> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IQueryable<EST_ESTIMATE> ApplyEstEstimateSorting(this IQueryable<EST_ESTIMATE> data,
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

        public static IQueryable<EST_ESTIMATE> ApplyEstEstimateFiltering(this IQueryable<EST_ESTIMATE> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EST_ESTIMATE>(filterDescriptors));
            }
            return data;
        }

        private static IQueryable<EST_ESTIMATE> AddSortExpression(IQueryable<EST_ESTIMATE> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "ESMT_ID":
                        data = data.OrderBy(earth => earth.ESMT_ID);
                        break;
                    case "EOCO_EOCO_ID":
                        data = data.OrderBy(earth => earth.EOCO_EOCO_ID);
                        break;
                    case "FINY_FINY_YEAR":
                        data = data.OrderBy(earth => earth.FINY_FINY_YEAR);
                        break;
                    case "ESMT_DIMAND":
                        data = data.OrderBy(earth => earth.ESMT_DIMAND);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "ESMT_ID":
                        data = data.OrderByDescending(earth => earth.ESMT_ID);
                        break;
                    case "EOCO_EOCO_ID":
                        data = data.OrderByDescending(earth => earth.EOCO_EOCO_ID);
                        break;
                    case "FINY_FINY_YEAR":
                        data = data.OrderByDescending(earth => earth.FINY_FINY_YEAR);
                        break;
                    case "ESMT_DIMAND":
                        data = data.OrderByDescending(earth => earth.ESMT_DIMAND);
                        break;
                }
            }
            return data;
        }
    }
}