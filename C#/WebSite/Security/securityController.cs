using Asr.Security;
using Equipment.Codes.Security;
using Equipment.Models;
using Equipment.Models.CoustomModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Equipment.Controllers.WebSite.Security
{
    [Authorize]
    public class securityController : DbController
    {

        public class ViewModel
        {
            public List<Asr.Security.SEC_USERS> AvailableProducts { get; set; }
            public List<Asr.Security.WF_LOCAL_USER_ROLES> RequestedProducts { get; set; }
            public int[] AvailableSelected { get; set; }
            public int[] RequestedSelected { get; set; }
            public string SavedRequested { get; set; }
        }

        //
        // GET: /security/
        public ActionResult Index()
        {
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            ViewBag.ORGA = Db.PAY_ORGAN.Select(c => new { c.CODE, c.ORGA_DESC });
            ViewBag.puser = Db.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
            ViewBag.puser1 = Db.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult userUpdate([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Asr.Security.SEC_USERS> SEC_USERS)
        {
            using (var Security = new SecurityEntities(true))
            {
                Asr.Security.SEC_USERS s = new Asr.Security.SEC_USERS();
                // Asr.Security.SEC_USERS.updateUser(s, row.ROW_NO);

                // var db = new BandarEntities();
                if (SEC_USERS != null && ModelState.IsValid)
                {
                    foreach (Asr.Security.SEC_USERS row in SEC_USERS)
                    {
                        Asr.Security.SEC_USERS.updateUser(s, row.ROW_NO);
                    }
                }
                return Json(SEC_USERS.ToDataSourceResult(request, ModelState));
            }
        }

        public ActionResult user_New(int? id)
        {
            Session["userid"] = id;
            //var Db = new BandarEntities();
            if (id != 0)
            {
                Equipment.Models.SEC_USERS EXPNEW = (from b in Db.SEC_USERS where b.ROW_NO == id select b).FirstOrDefault();
                return View(EXPNEW);
            }
            return View();
        }

        public ActionResult accesroleuser_New(int? id)
        {
            //int idd = 2; //Convert.ToInt32(Request.Form["ETDO_ID1"].ToString());
            //Session["typedocitem"] = idd;
            Session["Roleaccessitem"] = id;
            return View();
        }

        public ActionResult userRole_New(int? id)
        {
            var model = new Asr.Security.SEC_USERS();
            var selectedUsers = new List<Asr.Security.SEC_USERS>();
            Session["userRole"] = id;

            using (var Security = new SecurityEntities(true))
            {
                var q = (from b in Security.SEC_USERS select b);
                model.AvailableUsers = q.ToList();
                model.SelectedUsers = selectedUsers;
            }
            return View(model);
        }

        public ActionResult menuRole_New(int? id)
        {
            Session["menuRole"] = id;
            return View();
        }

        public ActionResult roleuser_New(int? id)
        {
            Session["Roleuser"] = id;
            return View();
        }

        public ActionResult siguser_New(int? id)
        {
            Session["siguser"] = id;
            return View();
        }

        [MenuAuthorize]
        public ActionResult postuser_New(int? id)
        {
            //using (var Db = this.DB())
            //{
            Session["postuser"] = id;
            ViewBag.ORGA = Db.PAY_ORGAN.Select(c => new { c.CODE, c.ORGA_DESC });
            ViewBag.post = Db.EXP_POST_LINE.Select(c => new { c.EPOL_ID, c.EPOL_NAME });
            return View();
            //}
        }

        public ActionResult receuser_New(int? id)
        {
            //var Db = new BandarEntities();
            Session["receuser"] = id;
            using (var Security = new SecurityEntities(true))
            {
                ViewBag.puser = Security.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
                ViewBag.prole = Security.SEC_JOB_TYPE_DOC.Select(c => new { c.JDTY_ID, c.ACTIV_FNAM });
                return View();
            }
        }

        public ActionResult taffuser_New(int? id)
        {
            //var Db = new BandarEntities();
            ViewBag.puser = Db.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
            Session["taffuser"] = id;
            return View();
        }

        public ActionResult accitem_New(int? id)
        {
            //var db = new BandarEntities();
            ViewBag.puser = Db.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
            Session["accitem"] = id;
            return View();
        }

        public ActionResult addtaffuser_New(int? id)
        {
            Session["taffadd"] = id;
            if (id != 0)
            {
                //using (var db = new BandarEntities())
                //{
                ViewBag.puser1 = Db.SEC_USERS.Select(c => new { c.ROW_NO, c.ORCL_NAME });
                //}
                using (var Security = new SecurityEntities(true))
                {
                    Asr.Security.SEC_TAFIEZ taff = (from b in Security.SEC_TAFIEZ where b.TAFZ_ID == id select b).FirstOrDefault();
                    return View(taff);
                }
            }
            else
                return View();
        }


        public ActionResult Getmainmenu()
        {
            //var db = new BandarEntities();
            var query = (from b in Db.SEC_MENU
                         where b.SEC_MENU2 == null
                         select new { b.MENU_ID, b.MENU_TITL });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInfomenu([DataSourceRequest] DataSourceRequest request, int id)
        {
            //using (var db = new BandarEntities())
            //{
            var query = from b in Db.SEC_MENU
                        where b.MENU_MENU_ID == id
                        select new
                        {
                            b.MENU_ID,
                            b.MENU_TITL


                        };
            return Json(query.ToDataSourceResult(request));
            //}
        }


        public ActionResult GetInforolemenu([DataSourceRequest] DataSourceRequest request, int sid)
        {
            //using (var db = new BandarEntities())
            //{
            var query = from b in Db.SEC_JOB_MENU
                        join k in Db.SEC_MENU on b.MENU_MENU_ID equals k.MENU_ID
                        where b.JDTY_JDTY_ID == sid
                        select new
                        {
                            k.MENU_ID,
                            k.MENU_TITL
                        };
            return Json(query.ToDataSourceResult(request));
            //}
        }


        public ActionResult GetInforeceiuser([DataSourceRequest] DataSourceRequest request, string idrole, string flowid)
        {
            // var db = new BandarEntities();
            using (var Security = new SecurityEntities(true))
            {
                //string str = null;
                var query = from b in Asr.Security.SEC_USER_ROLE.serachnextrole(flowid, idrole) select b;
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult GetInfouserrefrence([DataSourceRequest] DataSourceRequest request, string idrole, string flowid)
        {
            // var db = new BandarEntities();
            //   using (var Security = new SecurityEntities(true))
            //  //  using (var db = new BandarEntities())
            //    {
            //        var query = from b in Security.WF_LOCAL_USER_ROLES 
            //                            where b.PARENT_ORIG_SYSTEM == flowid && b.ROLE_NAME==idrole
            //                            select new
            //                    {
            //                        b.USER_NAME,
            //                        b.ROLE_NAME,
            //                        b.OWNER_TAG,
            //                        b.ROLE_ORIG_SYSTEM,
            //                        b.PARENT_ORIG_SYSTEM
            //
            //                    };
            //        return Json(query.ToDataSourceResult(request));
            //
            //
            //    }

            return Json(null);
        }


        public ActionResult GetInfouserr([DataSourceRequest] DataSourceRequest request, int sid)
        {
            string cm = string.Empty;
            //using (var cntx = new BandarEntities())
            //{
            cm = (Db.Database.SqlQuery<string>("SELECT SEC_USERS.ORCL_NAME as m FROM SEC_USERS, OWF_MGR.WF_NOTE_V WHERE (OWF_MGR.WF_NOTE_V.RECIPIENT_ROLE=SEC_USERS.ORCL_NAME) and SEC_USERS.ROW_NO=" + sid).FirstOrDefault());
            //}

            string str = null;
            using (var Security = new SecurityEntities(true))
            {
                var q = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME);
                int i = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME).Count();
                if (i != 0)
                    str = q.FirstOrDefault().ToString();
                else
                    str = "";
            }

            //  using (var Security = new SecurityEntities(true))
            //  {
            //      
            //      //var query = Security.SEC_USERS.Select(x => new { x.ROW_NO, x.ORCL_NAME, x.ORCL_PASS, x.USER_TYPE }).ToList();
            //      //return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);
            // 
            //       var  query = from b in Security.WF_LOCAL_USER_ROLES join K in Security.SEC_JOB_TYPE_DOC on b.ROLE_NAME equals K.ACTIV_NAME
            //                    where ((b.USER_NAME == str) || (cm != null && b.PARENT_ORIG_SYSTEM == "FLW_RESP" && b.ROLE_NAME == "CRATEOR")) && K.ACTI_TYPE==1
            //                      select new
            //                      {
            //                          b.USER_NAME,
            //                          b.ROLE_NAME,
            //                          b.OWNER_TAG,
            //                          b.ROLE_ORIG_SYSTEM,
            //                          b.PARENT_ORIG_SYSTEM
            //
            //
            //
            //                      };
            //          return Json(query.ToDataSourceResult(request));
            //     
            //
            //
            //  }

            return Json(null);
        }

        public ActionResult GetInforoleaccess([DataSourceRequest] DataSourceRequest request, int id)
        {
            // var db = new BandarEntities();
            string str = null;
            string itt = null;
            int sid = Convert.ToInt32(Session["accitem"]);
            string cm = string.Empty;
            //using (var cntx = new BandarEntities())
            //{
            cm = (Db.Database.SqlQuery<string>("SELECT SEC_USERS.ORCL_NAME as m FROM SEC_USERS, OWF_MGR.WF_NOTE_V WHERE (OWF_MGR.WF_NOTE_V.RECIPIENT_ROLE=SEC_USERS.ORCL_NAME) and SEC_USERS.ROW_NO=" + sid).FirstOrDefault());
            //}

            using (var Security = new SecurityEntities(true))
            {
                var q = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME);
                int i = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME).Count();
                if (i != 0)
                    str = q.FirstOrDefault().ToString();
                else
                    str = "";

                itt = (from b in Security.EXP_TYDO_V where b.ETDO_ETDO_ID == id select b.ETDO_ETDO_DESC).FirstOrDefault().ToString();
            }

            // using (var Security = new SecurityEntities(true))
            // {
            //                       
            //      var  query = from b in Security.WF_LOCAL_USER_ROLES join K in Security.SEC_JOB_TYPE_DOC on b.ROLE_NAME equals K.ACTIV_NAME
            //                   where ((b.USER_NAME == str) || (cm != null && b.PARENT_ORIG_SYSTEM == "FLW_RESP" && b.ROLE_NAME == "CRATEOR")) && K.ACTI_TYPE == 1
            //                   && K.ETDO_ETDO_ID == id && b.ROLE_ORIG_SYSTEM==itt
            //                     select new
            //                     {
            //                         b.USER_NAME,
            //                         b.ROLE_NAME,
            //                         b.OWNER_TAG,
            //                         b.ROLE_ORIG_SYSTEM,
            //                         b.PARENT_ORIG_SYSTEM
            //
            //
            //
            //                     };
            //         return Json(query.ToDataSourceResult(request));
            //    
            //
            //
            // }

            return Json(null);
        }

        public ActionResult GetInfoitemdocrole([DataSourceRequest] DataSourceRequest request, int id, string idrole, string flowid)
        {
            //var Db = new BandarEntities();

            //int id = Convert.ToInt32(Session["typedocitem"]);
            //int idd = Convert.ToInt32(Session["Roleaccessitem"]);

            var q = (from b in Db.SEC_JOB_TYPE_DOC
                     join k in Db.EXP_TYPE_DOC on b.ETDO_ETDO_ID equals k.ETDO_ID
                     where b.ACTIV_NAME == idrole && k.FLOW_TYPE == flowid
                     select b.JDTY_ID);

            int idd = Convert.ToInt16(q.FirstOrDefault());

            var query = from b in Db.EXP_ITEM_TYPE_DOC
                        join k in Db.SEC_JOB_TYPE_ITEM on b.EITY_ID equals k.EITY_EITY_ID
                        join j in Db.SEC_JOB_TYPE_DOC on k.JDTY_JDTY_ID equals j.JDTY_ID
                        join t in Db.EXP_TYPE_DOC on j.ETDO_ETDO_ID equals t.ETDO_ID
                        where b.ETDO_ETDO_ID == id && j.ETDO_ETDO_ID == id && k.ACTI_TYPE == 1 && j.ACTI_TYPE == 1 && j.JDTY_ID == idd //&& j.ACTIV_NAME == idrole && t.FLOW_TYPE == flowid && k.ACTI_TYPE == 1
                        select new
                        {
                            b.EITY_ID,
                            b.EITY_DESC
                        };
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfoitemdocrole0([DataSourceRequest] DataSourceRequest request, int id, string idrole, string flowid)
        {
            //var Db = new BandarEntities();
            //int id = Convert.ToInt32(Session["typedocitem"]);
            //int idd = Convert.ToInt32(Session["Roleaccessitem"]);

            var q = (from b in Db.SEC_JOB_TYPE_DOC
                     join k in Db.EXP_TYPE_DOC on b.ETDO_ETDO_ID equals k.ETDO_ID
                     where b.ACTIV_NAME == idrole && k.FLOW_TYPE == flowid
                     select b.JDTY_ID);

            int idd = Convert.ToInt16(q.FirstOrDefault());

            var query = from b in Db.EXP_ITEM_TYPE_DOC
                        join k in Db.SEC_JOB_TYPE_ITEM on b.EITY_ID equals k.EITY_EITY_ID
                        join j in Db.SEC_JOB_TYPE_DOC on k.JDTY_JDTY_ID equals j.JDTY_ID
                        join t in Db.EXP_TYPE_DOC on j.ETDO_ETDO_ID equals t.ETDO_ID
                        where b.ETDO_ETDO_ID == id && j.ETDO_ETDO_ID == id && j.ACTI_TYPE == 1 && k.ACTI_TYPE == 0 && j.JDTY_ID == idd// && j.ACTIV_NAME == idrole && t.FLOW_TYPE == flowid && k.ACTI_TYPE == 0
                        select new
                        {
                            b.EITY_ID,
                            b.EITY_DESC
                        };
            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult GetInfodocrole([DataSourceRequest] DataSourceRequest request)
        {
            //var Db = new BandarEntities();

            //int idd = Convert.ToInt32(Session["Roleaccessitem"]);
            int idd = Convert.ToInt32(Session["typedocitem"]);

            var query1 = from b in Db.SEC_JOB_TYPE_ITEM
                         select new
                         {
                             b.EITY_EITY_ID,
                         };

            var query = from b in Db.EXP_ITEM_TYPE_DOC
                        where b.ETDO_ETDO_ID == idd
                        select new
                        {
                            b.EITY_DESC,
                            b.EITY_ID
                        };

            //var query = from b in cntx.PRN_INQUIRY_CONTRACTORS where (b.INQY_ID == id) select b;
            //foreach (var item in query)
            //{
            //    var query2 = from b in cntx.PRN_INQUIRY_ROWS where (b.INQY_ID == id) select b;

            //     foreach (var item2 in query2)

            //     {

            //         var query3 = from b in cntx.PRN_INQUIRY_RESPONDS 
            //                      where (b.INQY_ID == id && b.INCR_ID==item.ID && b.INRW_ID==item2.ID) select b;
            //         if (!query3.Any())
            //         {
            //             respond.INCR_ID = item.ID;
            //             respond.INQY_ID = id;
            //             respond.INRW_ID = item2.ID;
            //             cntx.PRN_INQUIRY_RESPONDS.Add(respond);
            //             cntx.SaveChanges();

            //         }

            //     }

            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfoitemdoc([DataSourceRequest] DataSourceRequest request, int id)
        {
            //var Db = new BandarEntities();

            var query = from b in Db.EXP_ITEM_TYPE_DOC
                        where b.ETDO_ETDO_ID == id
                        select new
                        {
                            b.EITY_DESC,
                            b.EITY_ID
                        };
            return Json(query.ToDataSourceResult(request));
        }


        public ActionResult GetInfouserrolesec([DataSourceRequest] DataSourceRequest request, int sid, int etdoid)
        {
            // var db = new BandarEntities();
            string str = null;
            string itt = null;
            string cm = string.Empty;
            //using (var cntx = new BandarEntities())
            //{
            cm = (Db.Database.SqlQuery<string>("SELECT SEC_USERS.ORCL_NAME as m FROM SEC_USERS, OWF_MGR.WF_NOTE_V WHERE (OWF_MGR.WF_NOTE_V.RECIPIENT_ROLE=SEC_USERS.ORCL_NAME) and SEC_USERS.ROW_NO=" + sid).FirstOrDefault());
            //}

            using (var Security = new SecurityEntities(true))
            {
                var q = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME);
                int i = (from b in Security.SEC_USERS where b.ROW_NO == sid select b.ORCL_NAME).Count();
                if (i != 0)
                    str = q.FirstOrDefault().ToString();
                else
                    str = "";

                itt = (from b in Security.EXP_TYDO_V where b.ETDO_ETDO_ID == etdoid select b.ETDO_ETDO_DESC).FirstOrDefault().ToString();
            }

            return Json(null);
        }


        public ActionResult GetInforoleuser([DataSourceRequest] DataSourceRequest request, int sid)
        {
            // var db = new BandarEntities();
            string str = null;
            string tedo = null;
            string flowt = null;
            using (var Security = new SecurityEntities(true))
            {
                var q = (from b in Security.SEC_JOB_TYPE_DOC where b.JDTY_ID == sid select b);
                int i = (from b in Security.SEC_JOB_TYPE_DOC where b.JDTY_ID == sid select b).Count();
                if (i != 0)
                {
                    str = q.FirstOrDefault().ACTIV_NAME.ToString();
                    tedo = q.FirstOrDefault().ETDO_ETDO_ID.ToString();
                    int itedo = Convert.ToInt32(tedo);
                    flowt = (from b in Security.EXP_TYDO_V where b.ETDO_ETDO_ID == itedo select b.ETDO_FLOW_TYPE).FirstOrDefault().ToString();
                }
                else
                {
                    str = "";
                    tedo = "";
                    flowt = "";
                }
            }
            //var query = Security.SEC_USERS.Select(x => new { x.ROW_NO, x.ORCL_NAME, x.ORCL_PASS, x.USER_TYPE }).ToList();
            //return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);


            //  using (var Security = new SecurityEntities(true))
            //  {
            //  var query = from b in Security.WF_LOCAL_USER_ROLES
            //              where b.ROLE_NAME == str && b.PARENT_ORIG_SYSTEM == flowt
            //                  select new
            //                  {
            //                      b.USER_NAME,
            //                      b.ROLE_NAME,                                     
            //               b.OWNER_TAG ,
            //               b.PARENT_ORIG_SYSTEM 
            //             
            //
            //
            //
            //                  };
            //      return Json(query.ToDataSourceResult(request));
            //
            //
            //  }
            return Json(null);
        }

        public ActionResult GetInfouserrolenew([DataSourceRequest] DataSourceRequest request)
        {
            //var db = new BandarEntities();

            var query = from b in Db.SEC_USERS
                        orderby b.ROW_NO
                        select new
                        {
                            b.ROW_NO,
                            b.USER_NAME,
                            b.ORCL_NAME
                        };
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfouser([DataSourceRequest] DataSourceRequest request)
        {
            // var db = new BandarEntities();

            using (var Security = new SecurityEntities(true))
            {
                var query = from b in Security.SEC_USERS
                            where b.SYS_TYPE == "1"
                            orderby b.ORCL_NAME
                            select new
                            {
                                b.ROW_NO,
                                b.ORCL_NAME,
                                b.ORCL_PASS,
                                b.USER_TYPE,
                                b.USER_STATE,
                                b.FIRS_NAME,
                                b.FAML_NAME,
                                b.USER_NAME,
                                b.USER_PASS
                            };
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult GetInfouserecive([DataSourceRequest] DataSourceRequest request, string idrole, string flowid, int id)
        {
            // var db = new BandarEntities();

            using (var Security = new SecurityEntities(true))
            {
                int name = 0;

                if (id == -1)
                    name = Convert.ToInt32(Session["receuser"]);
                else
                    name = id;

                var q = (from b in Security.SEC_JOB_TYPE_DOC
                         join k in Security.EXP_TYDO_V on b.ETDO_ETDO_ID equals k.ETDO_ETDO_ID
                         where b.ACTIV_NAME == idrole && k.ETDO_FLOW_TYPE == flowid
                         select b);
                var q1 = from b in q
                         join k in Security.SEC_USER_ROLE on b.JDTY_ID equals k.JDTY_JDTY_ID
                         where k.SCSU_ROW_NO == name
                         select k;

                var query = from b in Security.SEC_USERS
                            join k in q1 on b.ROW_NO equals k.SCSU_ROW_NO_R
                            select new
                            {
                                b.ROW_NO,
                                b.ORCL_NAME,
                                b.ORCL_PASS,
                                b.USER_TYPE,
                                b.USER_STATE,
                                b.FIRS_NAME,
                                b.FAML_NAME,
                                b.USER_NAME,
                                b.USER_PASS
                            };
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult GetInfotaff([DataSourceRequest] DataSourceRequest request, int sid)
        {
            // var db = new BandarEntities();

            using (var Security = new SecurityEntities(true))
            {
                int name = 0;

                if (sid != -1)
                    name = sid;
                else
                    name = Convert.ToInt32(Session["taffuser"]);

                var query = from b in Security.SEC_TAFIEZ
                            where b.SCSU_ROW_NO == name
                            select new
                            {
                                b.TAFZ_ID,
                                b.TAFZ_STATUS,
                                b.SCSU_ROW_NO,
                                b.SCSU_ROW_NO_R,
                                b.TAFZ_STDA,
                                b.TAFZ_ENDA
                            };
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult GetInfousertaffadd([DataSourceRequest] DataSourceRequest request)
        {
            //var db = new Security();

            int id = Convert.ToInt32(Session["taffadd"]);
            using (var Security = new SecurityEntities(true))
            {
                var query = from b in Security.SEC_USERS
                            join k in Security.SEC_TAFIEZ on b.ROW_NO equals k.SCSU_ROW_NO_R
                            where k.SCSU_ROW_NO == id
                            orderby b.ROW_NO
                            select new
                            {
                                b.ROW_NO,
                                b.USER_NAME,
                                b.ORCL_NAME
                            };
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult GetInforole([DataSourceRequest] DataSourceRequest request, int id)
        {
            // var db = new BandarEntities();
            Session["typedocitem"] = id;
            //   using (var Security = new SecurityEntities(true))
            //using (var Security = new BandarEntities())
            //{
            //var query = Security.SEC_USERS.Select(x => new { x.ROW_NO, x.ORCL_NAME, x.ORCL_PASS, x.USER_TYPE }).ToList();
            //return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);
            var query = from b in Db.SEC_JOB_TYPE_DOC
                        where b.ETDO_ETDO_ID == id && b.ACTI_TYPE == 1
                        select new
                        {
                            b.JDTY_ID,
                            b.ACTIV_NAME,
                            b.ACTIV_FNAM,
                            b.ACTI_TYPE,
                            b.WIND_HIGH,
                            b.WIND_WIDE
                        };
            return Json(query.ToDataSourceResult(request));
            //}
        }

        public ActionResult highw_New(int? id)
        {
            Session["roleedit"] = id;

            //using (var Security = new BandarEntities())
            //{
            if (id != 0)
            {
                Equipment.Models.SEC_JOB_TYPE_DOC taff = (from b in Db.SEC_JOB_TYPE_DOC where b.JDTY_ID == id select b).FirstOrDefault();
                return View(taff);
            }
            else
            { return View(); }
            //}
        }


        [HttpPost]
        public ActionResult confirm_rolehw(Equipment.Models.SEC_JOB_TYPE_DOC NewItem)
        {
            int u = Convert.ToInt32(Session["roleedit"]);

            //using (var Db = new BandarEntities())
            //{
            try
            {
                var chkl = (from b in Db.SEC_JOB_TYPE_DOC where b.JDTY_ID == u select b).FirstOrDefault();
                chkl.WIND_HIGH = NewItem.WIND_HIGH;
                chkl.WIND_WIDE = NewItem.WIND_WIDE;
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", u) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }


        public ActionResult GetInforoleid([DataSourceRequest] DataSourceRequest request, int id)
        {
            // var db = new BandarEntities();

            using (var Security = new SecurityEntities(true))
            {
                //var query = Security.SEC_USERS.Select(x => new { x.ROW_NO, x.ORCL_NAME, x.ORCL_PASS, x.USER_TYPE }).ToList();
                //return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);
                var query = from b in Security.SEC_JOB_TYPE_DOC
                            where (b.ETDO_ETDO_ID == id || id == 0) && b.ACTIV_NAME != null && b.ACTI_TYPE == 1
                            select new
                            {
                                b.JDTY_ID,
                                b.ACTIV_NAME,
                                b.ACTIV_FNAM,
                                b.ACTI_TYPE
                            };
                return Json(query.ToDataSourceResult(request));
            }
        }

        public ActionResult Getpostuser([DataSourceRequest] DataSourceRequest request, int etdoid, string id)
        {
            //var Db = new BandarEntities();
            int userid = 0;
            if (id == "0")
                userid = Convert.ToInt32(Session["postuser"]);
            else
                userid = int.Parse(((from b in Db.SEC_USERS where b.ORCL_NAME == id select b.ROW_NO).FirstOrDefault().ToString()));

            var query = from b in Db.SEC_USER_TYPE_POST
                        where b.ETDO_ETDO_ID == etdoid && b.SCSU_ROW_NO == userid
                        select new
                        {
                            b.EURP_ID,
                            b.EPOL_EPOL_ID
                        };
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult Getpost([DataSourceRequest] DataSourceRequest request)
        {
            //var db = new BandarEntities();

            var query = from b in Db.EXP_POST_LINE
                        where b.EPOL_EPOL_ID == null
                        select new
                        {
                            b.EPOL_ID,
                            b.EPOL_NAME,
                            b.CODE_DISP,
                            b.ORGA_CODE
                        };
            return Json(query.ToDataSourceResult(request));
        }

        public ActionResult GetInfouserta()
        {
            using (var Security = new SecurityEntities(true))
            {
                //var query = Security.SEC_USERS.Select(x => new { x.ROW_NO, x.ORCL_NAME, x.ORCL_PASS, x.USER_TYPE }).ToList();
                //return this.Json(new { Success = true, data = query }, JsonRequestBehavior.AllowGet);
                var query = from b in Security.SEC_USERS

                            select new
                            {
                                b.ROW_NO,
                                b.ORCL_NAME
                            };
                return Json(query, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Getuser()
        {
            //var db = new BandarEntities();

            var query = (from b in Db.SEC_USERS
                         orderby b.ORCL_NAME
                         select new { b.ROW_NO, b.ORCL_NAME });

            return Json(query, JsonRequestBehavior.AllowGet);
        }


        public ActionResult getuserchoise(int? id)
        {
            //var Db = new BandarEntities();

            Equipment.Models.SEC_USERS taff = (from b in Db.SEC_USERS where b.ROW_NO == id select b).FirstOrDefault();
            //SEC_USERS query = (from b in db.SEC_USERS
            //             where b.ROW_NO == id 
            //             select b);
            return Json(new
            {
                Success = true,
                taff.FAML_NAME,
                taff.FIRS_NAME,
                taff.ORCL_NAME,
                taff.ORCL_PASS,
                taff.OUTP_OUTP_ID,
                taff.PRSN_EMP_NUMB,
                taff.USER_NAME,
                taff.USER_PASS,
                taff.USER_STATE,
                taff.USER_TYPE
            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Getpersonel()
        {
            //var db = new BandarEntities();

            var query = (from b in Db.PAY_PERSONEL select new { b.EMP_NUMB, b.FAML_NAME, b.FIRS_NAME });
            return Json(query, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Getoutp()
        {
            //var db = new BandarEntities();

            var query = (from b in Db.EXP_OUT_PERSONEL select new { b.OUTP_ID, b.OUTP_FNAME, b.OUTP_LNAME });
            return Json(query, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult confirm_postuser(Equipment.Models.SEC_USER_TYPE_POST NewItem)
        {
            int rowuser = Convert.ToInt32(Session["postuser"]);
            int idpost = 0;
            if (Request.Form["rolee"] != "")
                idpost = int.Parse(Request.Form["rolee"].ToString());

            int edt = 0;
            if (Request.Form["ETDO_ID2"] != "")
                edt = int.Parse(Request.Form["ETDO_ID2"].ToString());

            //using (var Db = new BandarEntities())
            //{
            try
            {
                NewItem.EPOL_EPOL_ID = idpost;
                NewItem.SCSU_ROW_NO = rowuser;
                NewItem.ETDO_ETDO_ID = edt;
                Db.SEC_USER_TYPE_POST.Add(NewItem);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", idpost) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }

        [HttpPost]
        public ActionResult confirm_userrecive(Asr.Security.SEC_USER_ROLE NewItem)
        {
            string rolen = Request.Form["chosei"].ToString();
            string flown = Request.Form["flow"].ToString();
            string name = Request.Form["userrec"].ToString();

            int rowuser = Convert.ToInt32(Session["receuser"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    int rolejobid = int.Parse(((from b in Security.SEC_JOB_TYPE_DOC
                                                join k in Security.EXP_TYDO_V on b.ETDO_ETDO_ID equals k.ETDO_ETDO_ID
                                                where b.ACTIV_NAME == rolen && k.ETDO_FLOW_TYPE == flown
                                                select new { b.JDTY_ID }).FirstOrDefault().JDTY_ID).ToString());

                    int rowyserdep = (from b in Security.SEC_USERS
                                      where b.ORCL_NAME == name
                                      select new { b.ROW_NO }).FirstOrDefault().ROW_NO;

                    var newUser = new Asr.Security.SEC_USER_ROLE();

                    Asr.Security.SEC_USER_ROLE.CreatUserDepen(newUser, rolejobid, rowuser, rowyserdep);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        [HttpPost]
        public ActionResult confirm_addusertaff(Asr.Security.SEC_TAFIEZ NewItem)
        {
            string sdate = Request.Form["azDate"].ToString();
            string edate = Request.Form["taDate1"].ToString();

            //string sdate = NewItem.TAFZ_STDA;
            //string edate = NewItem.TAFZ_ENDA;

            int u = Convert.ToInt32(Session["taffuser"]);
            int id = Convert.ToInt32(Session["taffadd"]);
            int r = int.Parse(NewItem.SCSU_ROW_NO_R.ToString());

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    if (id == 0)
                    {
                        Asr.Security.SEC_TAFIEZ.CreatUserTAFF(u, r, sdate, edate);
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", u) }.ToJson();
                    }
                    else
                    {
                        Asr.Security.SEC_TAFIEZ.UpdateUserTAFF(id, r, sdate, edate);
                        return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] بررسی شد.", u) }.ToJson();
                    }
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        [HttpPost]
        public ActionResult confirm_userrole(Asr.Security.SEC_USERS NewItem)
        {
            string name = Request.Form["Edinid"].ToString();
            int u = Convert.ToInt32(Session["userRole"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    var q = (from b in Security.SEC_JOB_TYPE_DOC
                             join j in Security.EXP_TYDO_V on b.ETDO_ETDO_ID equals j.ETDO_ETDO_ID
                             where b.JDTY_ID == u
                             select new { b.ACTIV_FNAM, j.ETDO_ETDO_DESC, j.ETDO_FLOW_TYPE, b.ACTIV_NAME }).FirstOrDefault();

                    string str = q.ACTIV_NAME;
                    string strf = q.ACTIV_FNAM;
                    string flow = q.ETDO_FLOW_TYPE;
                    string fname = q.ETDO_ETDO_DESC;
                    var model = new Asr.Security.WF_LOCAL_USER_ROLES();
                    model.USER_NAME = name;
                    model.OWNER_TAG = strf;
                    model.PARENT_ORIG_SYSTEM = flow;
                    model.ROLE_NAME = str;
                    model.ROLE_ORIG_SYSTEM = fname;
                    Asr.Security.WF_LOCAL_USER_ROLES.insertuserroles(model);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        [HttpPost]
        public ActionResult confirm_roleuser(Asr.Security.WF_LOCAL_USER_ROLES NewItem)
        {
            int jdtyid = 0;
            if (Request.Form["rolee"] != "")
                jdtyid = int.Parse(Request.Form["rolee"].ToString());

            int u = Convert.ToInt32(Session["Roleuser"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    var q1 = (from b in Security.SEC_USERS where b.ROW_NO == u select new { b.ORCL_NAME });
                    string name = q1.FirstOrDefault().ORCL_NAME;
                    var q = (from b in Security.SEC_JOB_TYPE_DOC
                             join j in Security.EXP_TYDO_V on b.ETDO_ETDO_ID equals j.ETDO_ETDO_ID
                             where b.JDTY_ID == jdtyid
                             select new { b.ACTIV_FNAM, j.ETDO_ETDO_DESC, j.ETDO_FLOW_TYPE, b.ACTIV_NAME }).FirstOrDefault();
                    string str = q.ACTIV_NAME;
                    string strf = q.ACTIV_FNAM;
                    string flow = q.ETDO_FLOW_TYPE;
                    string fname = q.ETDO_ETDO_DESC;
                    var model = new Asr.Security.WF_LOCAL_USER_ROLES();
                    model.USER_NAME = name;
                    model.OWNER_TAG = strf;
                    model.PARENT_ORIG_SYSTEM = flow;
                    model.ROLE_NAME = str;
                    model.ROLE_ORIG_SYSTEM = fname;
                    Asr.Security.WF_LOCAL_USER_ROLES.insertuserroles(model);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }


        public ActionResult confirm_itemuser(SEC_JOB_TYPE_ITEM NewItem)
        {
            //int jdtyid = 0;
            string rolen = Request.Form["chosei"].ToString();
            int typedoc = Convert.ToInt32(Request.Form["ETDO_IDacc"].ToString());
            string flown = Request.Form["flow"].ToString();
            int type = Convert.ToInt32(Request.Form["type"].ToString());
            int u = Convert.ToInt32(Session["accitem"]);
            int id = Convert.ToInt32(Request.Form["rolee"].ToString());
            //using (var Db = new BandarEntities())
            //{
            try
            {
                string myobj = (from b in Db.SEC_JOB_TYPE_DOC where b.ETDO_ETDO_ID == typedoc && b.ACTIV_NAME == rolen && b.ACTI_TYPE == 1 select b.JDTY_ID).FirstOrDefault().ToString();
                NewItem.JDTY_JDTY_ID = Convert.ToInt32(myobj);
                NewItem.EITY_EITY_ID = id;
                NewItem.ACTI_TYPE = type;
                Db.SEC_JOB_TYPE_ITEM.Add(NewItem);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", id) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }



        public ActionResult confirm_itemuserrole(SEC_JOB_TYPE_ITEM NewItem)
        {
            //int jdtyid = 0;
            //string rolen = Request.Form["chosei"].ToString();
            //int typedoc = Convert.ToInt32(Request.Form["ETDO_IDacc"].ToString());

            //string flown = Request.Form["flow"].ToString();
            int type = Convert.ToInt32(Request.Form["type"].ToString());
            //int u = Convert.ToInt32(Session["accitem"]);
            int id = Convert.ToInt32(Request.Form["rolee"].ToString());
            string myobj = Session["Roleaccessitem"].ToString();
            //using (var Db = new BandarEntities())
            //{
            try
            {
                //string myobj = (from b in db.SEC_JOB_TYPE_DOC where b.ETDO_ETDO_ID == typedoc && b.ACTIV_NAME == rolen && b.ACTI_TYPE == 1 select b.JDTY_ID).FirstOrDefault().ToString();
                NewItem.JDTY_JDTY_ID = Convert.ToInt32(myobj);
                NewItem.EITY_EITY_ID = id;
                NewItem.ACTI_TYPE = type;
                Db.SEC_JOB_TYPE_ITEM.Add(NewItem);
                //var myobj = (from b in db.EXP_ITEM_TYPE_DOC where b.EITY_ID == id select b).FirstOrDefault();
                // myobj.MESN_TYPE = flown;
                //myobj.ROWI_CART = rolen;
                ////myobj.ETDO_ETDO_ID = u;
                //myobj.ROWI_TYPE = type;
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", id) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }

        [HttpPost]
        public ActionResult confirm_menurole(Equipment.Models.SEC_JOB_MENU NewItem)
        {
            string name = Request.Form["Edinid"].ToString();
            int u = Convert.ToInt32(Session["menuRole"]);

            //using (var Db = new BandarEntities())
            //{
            try
            {
                NewItem.MENU_MENU_ID = int.Parse(name);
                NewItem.JDTY_JDTY_ID = u;
                Db.SEC_JOB_MENU.Add(NewItem);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }




        //[HttpPost]
        //public ActionResult confirm_menurole(Asr.Security.SEC_JOB_MENU  NewItem)
        //{
        //    string name = Request.Form["Edinid"].ToString();
        //    int u = Convert.ToInt32(Session["menuRole"]);
        //    using (var Context = new SecurityEntities())
        //    {
        //        try
        //        {
        //            NewItem.MENU_MENU_ID = int.Parse(name);
        //            NewItem.JDTY_JDTY_ID = u;
        //            Context.SEC_JOB_MENU.Add(NewItem);
        //            Context.SaveChanges();
        //            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name), CoustomData = 1 }.ToJson();
        //        }
        //        catch (Exception ex)
        //        {
        //            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
        //        }
        //    }
        //}

        [HttpPost]
        public ActionResult delete_itemrole(int id)
        {
            // int u = Convert.ToInt32(Session["menuRole"]);
            //using (var Db = new BandarEntities())
            //{
            try
            {
                var myobj = (from b in Db.SEC_JOB_TYPE_ITEM
                             where b.EITY_EITY_ID == id
                             select b).FirstOrDefault();
                Db.SEC_JOB_TYPE_ITEM.Remove(myobj);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] حذف شد.", id) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }

        [HttpPost]
        public ActionResult delete_menurole(int id)
        {
            int u = Convert.ToInt32(Session["menuRole"]);

            //using (var Db = new BandarEntities())
            //{
            try
            {
                var myobj = (from b in Db.SEC_JOB_MENU where b.MENU_MENU_ID == id && b.JDTY_JDTY_ID == u select b).FirstOrDefault();
                Db.SEC_JOB_MENU.Remove(myobj);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] حذف شد.", id) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }

        [HttpPost]
        public ActionResult delete_userrole(string name)
        {
            //string name = Request.Form["Edinid"].ToString();
            int u = Convert.ToInt32(Session["userRole"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    string str = (from b in Security.SEC_JOB_TYPE_DOC where b.JDTY_ID == u select b.ACTIV_NAME).FirstOrDefault().ToString();
                    var model = new Asr.Security.WF_LOCAL_USER_ROLES();
                    model.USER_NAME = name;
                    model.ROLE_NAME = str;
                    Asr.Security.WF_LOCAL_USER_ROLES.deleteuserroles(str, name);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        [HttpPost]
        public ActionResult delete_userrolerefr(string name, string rolen, string flown)
        {
            //string rolen = Request.Form["chosei"].ToString();
            //string flown = Request.Form["flow"].ToString();
            name = Request.Form["name"].ToString();

            int u = Convert.ToInt32(Session["receuser"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    int rolejobid = int.Parse(((from b in Security.SEC_JOB_TYPE_DOC
                                                join k in Security.EXP_TYDO_V on b.ETDO_ETDO_ID equals k.ETDO_ETDO_ID
                                                where b.ACTIV_NAME == rolen && k.ETDO_FLOW_TYPE == flown
                                                select new { b.JDTY_ID }).FirstOrDefault().JDTY_ID).ToString());

                    int rowyserdep = (from b in Security.SEC_USERS
                                      where b.ORCL_NAME == name
                                      select new { b.ROW_NO }).FirstOrDefault().ROW_NO;

                    Asr.Security.SEC_USER_ROLE.DeleteUserDepen(rolejobid, u, rowyserdep);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        [HttpPost]
        public ActionResult delete_postuser(int id)
        {
            //string name = Request.Form["Edinid"].ToString();
            //int u = Convert.ToInt32(Session["userRole"]);

            //using (var Db = new BandarEntities())
            //{
            try
            {
                var myobj = (from b in Db.SEC_USER_TYPE_POST where b.EURP_ID == id select b).FirstOrDefault();
                Db.SEC_USER_TYPE_POST.Remove(myobj);
                Db.SaveChanges();
                return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] حذف شد.", id) }.ToJson();
            }
            catch (Exception ex)
            {
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
            }
            //}
        }

        [HttpPost]
        public ActionResult delete_roleuser(string str)
        {
            //string name = Request.Form["Edinid"].ToString();
            int u = Convert.ToInt32(Session["Roleuser"]);

            using (var Security = new SecurityEntities(true))
            {
                try
                {
                    var q1 = (from b in Security.SEC_USERS where b.ROW_NO == u select new { b.ORCL_NAME });
                    string name = q1.FirstOrDefault().ORCL_NAME;
                    var model = new Asr.Security.WF_LOCAL_USER_ROLES();
                    model.USER_NAME = name;
                    model.ROLE_NAME = str;
                    Asr.Security.WF_LOCAL_USER_ROLES.deleteuserroles(str, name);
                    return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", name) }.ToJson();
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }


        public ActionResult userInsert(Asr.Security.SEC_USERS NewItem)
        {
            if (NewItem == null)
                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "اطلاعات را به شکل کامل پر کنید" }.ToJson();
            if (string.IsNullOrEmpty(NewItem.USER_NAME))
                return new ServerMessages(ServerOprationType.Success) { ExceptionMessage = "نام کاربری را وارد کنید" }.ToJson();

            int u = Convert.ToInt32(Session["userid"]);
            using (var security = new SecurityEntities(true))
            {
                try
                {
                    if (u == 0)
                    {
                        if ((new AsrMembershipProvider()).AllUsers.Any(x => x.USER_NAME.ToUpper().Trim() == NewItem.USER_NAME.ToUpper().Trim()))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام کاربری که وارد کرده اید قبلا ثبت شده است!" }.ToJson();

                        if ((new AsrMembershipProvider()).AllUsers.Any(x => x.ORCL_NAME.ToUpper().Trim() == NewItem.ORCL_NAME.ToUpper().Trim()))
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "نام اراکلی که وارد کرده اید قبلا ثبت شده است!" }.ToJson();

                        if ((NewItem.PRSN_EMP_NUMB.HasValue) && (NewItem.PRSN_EMP_NUMB > 0))
                            if ((new AsrMembershipProvider()).AllUsers.Any(x => x.PRSN_EMP_NUMB == NewItem.PRSN_EMP_NUMB))
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "برای پرسنل مورد نظر قبلا یک حساب کاربری ایجاد شده است" }.ToJson();

                        if ((NewItem.OUTP_OUTP_ID.HasValue) && (NewItem.PRSN_EMP_NUMB > 0))
                            if ((new AsrMembershipProvider()).AllUsers.Any(x => x.PRSN_EMP_NUMB == NewItem.PRSN_EMP_NUMB))
                                return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "برای پرسنل بیرون سازمانی مورد نظر قبلا یک حساب کاربری ایجاد شده است" }.ToJson();

                        if (Asr.Security.SEC_USERS.CreateNewUser(NewItem))
                            return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", NewItem.ORCL_NAME) }.ToJson();
                        else
                            return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = "Error" }.ToJson();

                        //return new ServerMessages(ServerOprationType.Success) { Message = string.Format("[{0}] ثبت شد.", NewItem.ORCL_NAME) }.ToJson();
                    }
                    else
                    {
                        Asr.Security.SEC_USERS.updateUser(NewItem, u);
                        return new ServerMessages(ServerOprationType.Success) { Message = "بروز رسانی شد." }.ToJson();
                    }
                }
                catch (Exception ex)
                {
                    return new ServerMessages(ServerOprationType.Failure) { ExceptionMessage = ex.PersianMessage() }.ToJson();
                }
            }
        }

        public string confirm_role(Asr.Security.WF_LOCAL_ROLES NewItem)
        {
            int f1 = int.Parse(Request.Form["ETDO_ID1"].ToString());
            using (var Security = new SecurityEntities(true))
            {
                Asr.Security.WF_LOCAL_ROLES.refreshroles(NewItem, f1);
            }
            return "[{0}] ثبت شد.";
        }

    }

}
