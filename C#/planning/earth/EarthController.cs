using Asr.Cartable;
using Asr.Security;
using Equipment.Codes.Security;
using Equipment.DAL;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.Planning.Earth
{
    [Authorize]
    public partial class EarthController
    {
        BandarEntities cntx;
        AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
        public EarthController()
            : base()
        {
            cntx = this.DB();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cntx.Dispose();
            }
            base.Dispose(disposing);
        }


        //
        // GET: /Earth/
        [MenuAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CHK_ROW_TMP(int id)
        {
            //ViewData["cntr"] = cntx.CNT_CONTRACT_V.Select(o => new { o.TITL,o.CNTR_NO }).AsEnumerable();
            ViewData["id"] = id;
            return View();
        }

        public ActionResult EAR_CHK_ROW_TMP(int id)
        {


            ViewData["id"] = id;
            return View();
        }

        public ActionResult SendCheckListRow()
        {
            List<string> success = new List<string>();
            List<string> error = new List<string>();
            int NoteId = 0;
            int row = int.Parse(Request.Form["row"]);
            for (int i = 1; i <= row; i++)
            {
                try
                {

                    if (!string.IsNullOrEmpty(Request.Form["C_" + i]))
                    {
                        string Username = Request.Form["C_" + i].Split(',')[1];
                        NoteId = int.Parse(Request.Form["C_" + i].Split(',')[0]);
                        if (NoteId != 0)
                        {

                            wp = new AsrWorkFlowProcess(NoteId);
                            wp.Approve(new string[] { Username }, "چک لیست زمین جهت امتیاز دهی ارسال شد");

                            success.Add("اطلاعات با موفقیت جهت امتیاز دهی به " + Username + " ارسال شد");
                        }
                        else
                        {
                            error.Add("اطلاعات از قبل جهت امتیاز دهی به " + Username + " ارسال شده است");

                        }

                    }


                }
                catch (Exception ex)
                {
                    error.Add(" خطا در ارسال اطلاعات به    " + ex.ToString());


                }
            }
            string successMessages = string.Join("<br />", success.ToArray());
            string errorMessages = string.Join("<br />", error.ToArray());
            if (success.Count() != 0)
            {
                return new ServerMessages(ServerOprationType.Success) { Message = successMessages }.ToJson();
            }
            else
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = errorMessages }.ToJson();
            }
        }

        public ActionResult ViewFormCheckList(string id, decimal notId)
        {
            ViewData["ECHL_ID"] = id;
            ViewData["notId"] = notId;
            return View();
        }


        public ActionResult ViewForm(string id, decimal notId)
        {
            ViewData["ERTH_ID"] = id;
            ViewData["notId"] = notId;

            return View();
        }
        public ActionResult UpdateEarthCheckList(EAR_EARTH_CHECK_LIST ObjTemp)
        {
            //objecttemp.GROP_GROP_ID = 62;
            try
            {
                //if (!cntx.EAR_EARTH_CHECK_LIST.Select(xx => xx.ERTH_ERTH_ID == ObjTemp.ERTH_ERTH_ID && xx.ECHL_ID == ObjTemp.ECHL_ID).Any())
                //{
                string Sql = string.Format("update EAR_EARTH_CHECK_LIST set ERTH_ERTH_ID={0} where ECHL_ID={1}", ObjTemp.ERTH_ERTH_ID, ObjTemp.ECHL_ID);
                cntx.Database.ExecuteSqlCommand(Sql);
                var Query = (from b in cntx.EAR_CHECK_LIST_ROW where b.ECHL_ECHL_ID == ObjTemp.ECHL_ID select new { b.PRSN_EMP_NUMB }).ToList().Distinct();
                foreach (var Row in Query)
                {
                    wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name },
                        cntx.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == 603).Select(xx => xx.ETDO_DESC).FirstOrDefault(),
                        "چک لیست انتخاب زمین با شماره ردیف   " + ObjTemp.ECHL_ID + "  ثبت شد ", 603,
                        ObjTemp.ECHL_ID + "-" + Row.PRSN_EMP_NUMB);
                }



                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد ") }.ToJson();
                //}
                //else { return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("اطلاعات وارد شده تکراری است") }.ToJson(); }
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("خطا در ثبت اطلاعات  " + ex.ToString()) }.ToJson();

            }
        }

        public ActionResult UpdateEarthCheckListValue(EAR_CHECK_LIST_ROW ObjTemp)
        {
            //objecttemp.GROP_GROP_ID = 62;
            List<string> success = new List<string>();
            List<string> error = new List<string>();
            int row = int.Parse(Request.Form["row"]);
            for (int i = 1; i <= row; i++)
            {
                try
                {

                    if (!string.IsNullOrEmpty(Request.Form["ID_" + i]))
                    {
                        string Sql = string.Format("update EAR_CHECK_LIST_ROW set CHLR_VALUE={0} where CHLR_ID={1}", Request.Form["Val_" + i], Request.Form["ID_" + i]);
                        cntx.Database.ExecuteSqlCommand(Sql);

                    }
                    success.Add(" امتیاز ردیف " + Request.Form["DESC_" + i] + " با موفقیت ثبت شد ");



                }
                catch (Exception ex)
                {
                    success.Add(" خطا در ثبت امتیاز ردیف" + Request.Form["DESC_" + i]);


                }
            }
            string successMessages = string.Join("<br />", success.ToArray());
            return new ServerMessages(ServerOprationType.Success) { Message = successMessages }.ToJson();
        }

        public ActionResult EAR_EARTH_TMP_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = (from p in cntx.EAR_EARTH_CHK_TMP

                        orderby p.ECHT_ID descending

                        select new
                        {
                            p.ECHT_ID,
                            p.ECHT_DESC
                        }).ToList();



            return Json(data.ToDataSourceResult(request));
        }




        public ActionResult EAR_EARTH_CHECK_LIST_Read([DataSourceRequest] DataSourceRequest request, int? earthId)
        {

            var data = EarthRepository.Get_EAR_EARTH_CHECK_LIST()
                                      .Where(b => b.ERTH_ERTH_ID == earthId || earthId == null)
                                      .Select(b => new
                                      {
                                          b.ECHL_ID,
                                          b.ECHL_DESC,
                                          coustomFeildEarhName = b.EAR_EARTH.ERTH_NAME,
                                          Plan = b.CGT_PRO.CGT_PLAN.PLN_DESC,
                                          Proj = b.CGT_PRO.PRJ_DESC,
                                          Template = b.EAR_EARTH_CHK_TMP.ECHT_DESC
                                      })
                                      .ToDataSourceResult(request);

            return Json(data, JsonRequestBehavior.AllowGet);

        }




        public ActionResult EAR_EARTH_Read([DataSourceRequest] DataSourceRequest request)
        {

            var data = cntx.EAR_EARTH

                                      .Select(b => new
                                      {
                                          b.ERTH_ID,
                                          b.ERTH_NAME,
                                          b.ERTH_ADDRESS,
                                          b.ERTH_OWNERSHIP,
                                          b.ERTH_AREA,
                                          b.ERTH_COMM,
                                          chkListCount = b.EAR_EARTH_CHECK_LIST.Count,

                                      })
                                     .ToList().AsEnumerable();


            var FinalQuery = data.Select(b => new
            {
                b.ERTH_ID,
                b.ERTH_NAME,
                b.ERTH_ADDRESS,
                b.ERTH_OWNERSHIP,
                b.ERTH_AREA,
                b.ERTH_COMM,
                b.chkListCount,
                Not_id = return_noteid(b.ERTH_ID)
            }).ToList();
            return Json(FinalQuery.ToDataSourceResult(request));
        }
        public decimal return_noteid(int id)
        {

            string sql = "SELECT WF_NOTE_V.NOT_ID as m FROM WF_NOTE_V where stat='OPEN' and MESSAGE_NAME='CREATOR' and upper(RECIPIENT_ROLE)='" + this.User.Identity.Name.ToUpper() + "' and  WF_NOTE_V.ITEM_KEY='FLW_EART.PFLW_EARTH^" + id + "'";
            //and stat='OPEN'

            decimal not_id = cntx.Database.SqlQuery<decimal>(sql).FirstOrDefault();
            return not_id;
        }
        public JsonResult SendStartNode(int notId, string MessageName, string MessageType)
        {

            AsrWorkFlowProcess wp = new AsrWorkFlowProcess(notId);


            AsrJobProvider jp = new AsrJobProvider(MessageName, MessageType);

            return this.Json(new { Success = true, data = jp.AllUsers }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Get_Checklist_DD()
        {
            try
            {
                var RetVal = from b in cntx.EAR_EARTH_CHECK_LIST
                             where b.ERTH_ERTH_ID == null
                             select new { b.ECHL_ID, ECHL_DESC = b.ECHL_DESC + "-" + b.CGT_PRO.CGT_PLAN.PLN_DESC + "-" + b.CGT_PRO.PRJ_DESC };
                return Json(RetVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
        }

        public ActionResult InsertCheckList(EAR_EARTH_CHECK_LIST ObjTemp)
        {
            try
            {
                var newInstance = new EAR_EARTH_CHECK_LIST()
                {
                    ECHL_DESC = ObjTemp.ECHL_DESC,
                    ECHT_ECHT_ID = ObjTemp.ECHT_ECHT_ID,
                    CPRO_CPLA_PLN_CODE = ObjTemp.CPRO_CPLA_PLN_CODE,
                    CPRO_PRJ_CODE = ObjTemp.CPRO_PRJ_CODE
                };
                newInstance.SaveToDataBase();
                var checkListTemplate = (from b in EarthRepository.cntx.EAR_EARTH_CHK_TMP where b.ECHT_ID == ObjTemp.ECHT_ECHT_ID select b).FirstOrDefault();
                foreach (var row in checkListTemplate.EAR_CHECK_LIST_ROW_TMP)
                {
                    newInstance.EAR_CHECK_LIST_ROW.Add(new EAR_CHECK_LIST_ROW()
                    {
                        CHLR_DESC = row.CHTR_DESC,
                        CHLR_ROW = row.CHTR_ROW.ToString(),
                        CHLR_VALUE = row.CHTR_VALUE,
                        CHLR_WEIGHT = row.CHTR_WEIGHT,
                        PRSN_EMP_NUMB = row.PRSN_EMP_NUMB
                    });
                }
                newInstance.Update();

                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();

            }
            catch (Exception Ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "خطا در ثبت اطلاعات:" + Ex.ToString() }.ToJson();
            }

        }

        [EntityAuthorize("EAR_EARTH > select,insert")]
        public ActionResult EAR_EARTH_Add(EAR_EARTH model)
        {
            //  try
            //  {
            if (PublicRepository.ExistModel("EAR_EARTH", "ERTH_NAME='{0}'", model.ERTH_NAME.Trim()))
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = string.Format("[{0}] قبلا ثبت شده است", model.ERTH_NAME) }.ToJson();
            }
            //model.SaveToDataBase();

            Dbase.Add(model);
            //AsrWorkFlowProcess wp = new AsrWorkFlowProcess();
            // wp.StartProcess(HttpContext.User.Identity.Name, new string[] { }, "AddEarthTest", "Msg Body", 102, model.ERTH_ID);

            wp.StartProcess(this.HttpContext.User.Identity.Name, new string[] { this.HttpContext.User.Identity.Name },
                           cntx.EXP_TYPE_DOC.Where(xx => xx.ETDO_ID == 102).Select(xx => xx.ETDO_DESC).FirstOrDefault(),
                           "فرم انتخاب زمین با شماره ردیف   " + model.ERTH_ID + "  ثبت شد ", 102,
                           model.ERTH_ID);
            // var context = GlobalHost.ConnectionManager.GetHubContext<Equipment.Hubs.CartableHub>();
            // context.Clients.All.getBroadCastMessage(model.ERTH_NAME);

            //wp.FinishProccess();
            //wp.Approve(new string[] { });
            //wp.Reject(new string[] { });, wp.NoteId

            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("اطلاعات با موفقیت ثبت شد") }.ToJson();
            //  }
            //   catch (Exception ex)
            //   {
            //       this.LogError(ex);
            //      return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            //   }
        }


        [HttpPost]
        public ActionResult EAR_CHECK_LIST_Add(string newInstanceName, int selectedTemplateID = 0, short selectedPlanCode = 0, short selectedProjCode = 0, int selectedEarthCode = 0)
        {
            if (string.IsNullOrEmpty(newInstanceName) || selectedEarthCode == 0 || selectedPlanCode == 0 || selectedProjCode == 0)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات را کامل وارد کنید." }.ToJson();
            }
            if (PublicRepository.ExistModel("EAR_EARTH_CHECK_LIST", "ECHL_DESC='{0}' AND ECHT_ECHT_ID={1} AND CPRO_CPLA_PLN_CODE={2} AND CPRO_PRJ_CODE={3}", newInstanceName, selectedTemplateID, selectedPlanCode, selectedProjCode))
            {
                string msg = "از این قالب برای زمین [{0}] و پروژه [{1}]\n مربوط به طرح [{2}]\n چک لیستی با این عنوان ایجاد شده است.";
                var pln = (from b in cntx.CGT_PLAN where b.PLN_CODE == selectedPlanCode select b).FirstOrDefault().PLN_DESC;
                var prj = (from b in cntx.CGT_PRO where b.PRJ_CODE == selectedProjCode && b.CPLA_PLN_CODE == selectedPlanCode select b).FirstOrDefault().PRJ_DESC;
                var ear = (from b in EarthRepository.Get_EAR_EARTH() where b.ERTH_ID == selectedEarthCode select b).FirstOrDefault().ERTH_NAME;
                msg = string.Format(msg, ear, prj, pln);
                //string.Format("[{0}] قبلا ثبت شده است.", newInstanceName)
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = msg }.ToJson();
            }

            var newInstance = new EAR_EARTH_CHECK_LIST()
            {
                ECHL_DESC = newInstanceName,
                ECHT_ECHT_ID = selectedTemplateID,
                CPRO_CPLA_PLN_CODE = selectedPlanCode,
                CPRO_PRJ_CODE = selectedProjCode,
                ERTH_ERTH_ID = selectedEarthCode
            };
            newInstance.SaveToDataBase();
            //__________________Get Temp List____________________________
            var checkListTemplate = (from b in EarthRepository.cntx.EAR_EARTH_CHK_TMP where b.ECHT_ID == selectedTemplateID select b).FirstOrDefault();
            foreach (var row in checkListTemplate.EAR_CHECK_LIST_ROW_TMP)
            {
                newInstance.EAR_CHECK_LIST_ROW.Add(new EAR_CHECK_LIST_ROW()
                {
                    CHLR_DESC = row.CHTR_DESC,
                    CHLR_ROW = row.CHTR_ROW.ToString(),
                    CHLR_VALUE = row.CHTR_VALUE,
                    //CHLR_WEIGHT = row.CHTR_WEIGHT,
                    PRSN_EMP_NUMB = row.PRSN_EMP_NUMB
                });
            }
            newInstance.Update();
            Session["89765EQ_CurrentEARCHKLST"] = newInstance.ECHL_ID;
            return new ServerMessages(ServerOprationType.Success)
            {
                Message = string.Format("[{0}] ایجاد شد .", newInstanceName),
                CoustomData = new { NewCheckListId = newInstance.ECHL_ID }
            }.ToJson();
            //return Json(new { Success = true, NewCheckListId = newInstance.ECHL_ID }, JsonRequestBehavior.AllowGet);
        }


    }
}