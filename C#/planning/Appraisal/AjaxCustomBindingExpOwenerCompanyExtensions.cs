using Asr.Base;
using Equipment.Models;
using Kendo.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Equipment.Controllers.Planning.Appraisal
{
    [Developer("A.Saffari")]
    public static class AjaxCustomBindingExpOwenerCompanyExtensions
    {
        //EAR_EARTH_CHECK_LIST
        public static IQueryable<EXP_OWENER_COMPANY> ApplyExpOwenerCompanyPaging(this IQueryable<EXP_OWENER_COMPANY> data, int page, int pageSize)
        {
            if (pageSize > 0 && page > 0)
            {
                data = data.Skip((page - 1) * pageSize);
            }

            data = data.Take(pageSize);

            return data;
        }

        public static IQueryable<EXP_OWENER_COMPANY> ApplyExpOwenerCompanySorting(this IQueryable<EXP_OWENER_COMPANY> data,
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

        public static IQueryable<EXP_OWENER_COMPANY> ApplyExpOwenerCompanyFiltering(this IQueryable<EXP_OWENER_COMPANY> data,
            IList<IFilterDescriptor> filterDescriptors)
        {
            if (filterDescriptors.Any())
            {
                data = data.Where(ExpressionBuilder.Expression<EXP_OWENER_COMPANY>(filterDescriptors));
            }
            return data;
        }

        private static IQueryable<EXP_OWENER_COMPANY> AddSortExpression(IQueryable<EXP_OWENER_COMPANY> data, ListSortDirection sortDirection, string memberName)
        {
            if (sortDirection == ListSortDirection.Ascending)
            {
                switch (memberName)
                {
                    case "EOCO_ID":
                        data = data.OrderBy(chklst => chklst.EOCO_ID);
                        break;
                    case "EOCO_DESC":
                        data = data.OrderBy(chklst => chklst.EOCO_DESC);
                        break;
                    case "EOTY_EOTY_ID":
                        data = data.OrderBy(exp => exp.EOTY_EOTY_ID);
                        break;
                    case "GEOL_G_CODE":
                        data = data.OrderBy(exp => exp.GEOL_G_CODE);
                        break;
                    case "ACTV_TYPE":
                        data = data.OrderBy(exp => exp.ACTV_TYPE);
                        break;
                }
            }
            else
            {
                switch (memberName)
                {
                    case "EOCO_ID":
                        data = data.OrderByDescending(chklst => chklst.EOCO_ID);
                        break;
                    case "EOCO_DESC":
                        data = data.OrderByDescending(chklst => chklst.EOCO_DESC);
                        break;
                    case "EOTY_EOTY_ID":
                        data = data.OrderByDescending(exp => exp.EOTY_EOTY_ID);
                        break;
                    case "GEOL_G_CODE":
                        data = data.OrderByDescending(exp => exp.GEOL_G_CODE);
                        break;
                    case "ACTV_TYPE":
                        data = data.OrderByDescending(exp => exp.ACTV_TYPE);
                        break;
                }
            }
            return data;
        }
    }
}