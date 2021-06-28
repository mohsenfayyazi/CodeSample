using Asr.Base;
using Asr.Map.Core;
using Equipment.Models.CoustomModel;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Equipment.Controllers.Map
{
    [Authorize]
    [Developer("A.Saffari")]
    public class MapController : Controller
    {
        public ActionResult Index()
        {
            return this.RedirectToAction("Index", "Home");
        }

        public ActionResult BaseDefinition()
        {
            return this.View();
        }

        public ActionResult ManagePoints()
        {
            return this.View();
        }

        public ActionResult SavePoint(MapPoint newPoint)
        {
            if (string.IsNullOrEmpty(newPoint.LocationX) || string.IsNullOrEmpty(newPoint.LocationY))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Point location cannot be null or empty." }.ToJson();
            }
            if (string.IsNullOrEmpty(newPoint.KeyValue))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Point key value cannot be null or empty." }.ToJson();
            }
            if (newPoint.MapTypeRowNo == null || newPoint.MapTypeRowNo <= 0)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Point Type must be choose." }.ToJson();
            }
            try
            {
                using (var cntx = new MapEntities(true))
                {
                    cntx.MAP_POINT.Add(newPoint);
                    cntx.SaveChanges();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "sucssed" }.ToJson();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        public ActionResult LoadPoints()
        {
            using (var mp = new MapEntities(true))
            {
                var query = mp.MAP_POINT.AsEnumerable().Select(x => new { x.RowNo, x.LocationX, x.LocationY, TypeName = x.MapType.Description, x.KeyValue }).ToList();
                return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetTableFieldsName(string queryText)
        {
            try
            {
                if (string.IsNullOrEmpty(queryText))
                {
                    return null;
                }
                MapType mType = new MapType();
                mType.TableName = queryText;
                return this.Json(mType.GetFieldsName().Select(b => new { fldName = b }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return this.Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveMapType(MapType pointType)
        {
            if (!string.IsNullOrEmpty(pointType.TableName))
            {
                if (!DbHelper.CheckQueryValidation(pointType.TableName))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Your Query Not Valid" }.ToJson();
                }
            }
            if (!string.IsNullOrEmpty(pointType.StatusQuery))
            {
                if (!DbHelper.CheckQueryValidation(pointType.StatusQuery))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Your Status Query Not Valid" }.ToJson();
                }
            }
            if (!string.IsNullOrEmpty(pointType.DisplayQuery))
            {
                if (!DbHelper.CheckQueryValidation(pointType.DisplayQuery))
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Your DisplayQuery Not Valid" }.ToJson();
                }
            }
            try
            {
                using (var cntx = new MapEntities(true))
                {
                    cntx.MAP_TYPE.Add(pointType);
                    cntx.SaveChanges();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "sucssed" }.ToJson();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        public ActionResult Types()
        {
            MapEntities cntx = new MapEntities(true);
            return View(cntx.MAP_TYPE.AsEnumerable());
        }

        public ActionResult SaveMapShape(MapShape shape)
        {
            if (string.IsNullOrEmpty(shape.ShapeName))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Shape name cannot be null or empty." }.ToJson();
            }
            if (string.IsNullOrEmpty(shape.Color))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Shape color cannot be null or empty." }.ToJson();
            }
            try
            {
                using (var cntx = new MapEntities(true))
                {
                    cntx.MAP_SHAPE.Add(shape);
                    cntx.SaveChanges();
                }
                return new ServerMessages(ServerOprationType.Success) { Message = "sucssed" }.ToJson();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.ToString() }.ToJson();
            }
        }

        public ActionResult GetPointInfo(int id)
        {
            PostInfo model = new PostInfo(id);
            return PartialView(model);
        }

        public ActionResult GetData(string id)
        {
            return Json(new { data = "salam" });
//            OsmSharp.Data.SQLite.Osm.SQLiteDataSource
        }



        public JsonResult SaveTestPoint(string lng, string lat)
        {
           //using (Oracle.ManagedDataAccess.Client.OracleConnection conn = new Oracle.ManagedDataAccess.Client.OracleConnection(GlobalConst.ConnectionString))
           //{
           //    conn.Open();
           //    using (Oracle.ManagedDataAccess.Client.OracleCommand cmd = conn.CreateCommand())
           //    {
           //        cmd.CommandText = string.Format("INSERT INTO MAP_LOCATIONS_TEST (LOC_ID,LAT,LNG,CONTENT) VALUES((SELECT (NVL(MAX(LOC_ID),0))+1 from MAP_LOCATIONS_TEST),'{0}','{1}','test')", lat.Trim(), lng.Trim());
           //        cmd.ExecuteNonQuery();
           //    }
           //
           //}
           //string s = string.Format("{0} - {1}", lng, lat);

            return new ServerMessages(ServerOprationType.Success) { Message = " save Success" }.ToJson();
        }
    }
}