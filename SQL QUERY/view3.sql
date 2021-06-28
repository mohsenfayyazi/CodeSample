--------------------------------------------------------
--  File created - Thursday-June-24-2021   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for View EXP_GROP_V
--------------------------------------------------------

  CREATE OR REPLACE FORCE EDITIONABLE VIEW "FMIS_DBA"."EXP_GROP_V" ("MVAR", "A", "MW", "INFV_TIME", "VAR_DATE", "GROP_ID", "POGI_STAT") AS 
  SELECT 
SUM(nvl(mvar, 0)) mvar,SUM(nvl(A, 0)) A,
SUM(nvl(mw*pogi_stat, 0)) mw,INFV_TIME,VAR_DATE,grop_GROP_ID GROP_ID,0 POGI_STAT
FROM EXP_POST_GROUP a,
  EXP_POST_GROUP_INSTRU b,
  EXP_POST_VAR_INSTRU c,  EXP_POST_VAR_HEAD e
WHERE 
a.POGR_ID=b.POGR_POGR_ID
and b.EPIU_EPIU_ID=c.EPIU_EPIU_ID
and e.EPVH_ID=c.EPVH_EPVH_ID
and infv_time is not null
and a.EPOL_EPOL_ID=e.EPOL_EPOL_ID
and e.epvh_type!=6
and  (b.pogi_date>=var_date or b.pogi_date is null)
and  (b.pogi_time>=infv_time or b.pogi_time is null)
group by c.INFV_TIME,var_DATE,grop_GROP_ID
union
SELECT
SUM(nvl(mvar, 0)) mvar,SUM(nvl(a, 0)) a,
    SUM(nvl(mw, 0)) mw,
       
    infv_time,
    var_date,
    grop_grop_id grop_id--,
    ,0 POGI_STAT
FROM
    exp_group_setting          a,
    exp_post_line_instru   b,
    exp_post_var_instru     c,    exp_post_var_head       e
WHERE
   a.eins_eins_id=b.eins_eins_id
   and a.eunl_eunl_id=b.eunl_eunl_id
   and b.epiu_id = c.epiu_epiu_id
    AND e.epvh_id = c.epvh_epvh_id
    AND infv_time IS NOT NULL
    
    AND a.grse_stat = 0
    AND e.epvh_type != 6

GROUP BY
    c.infv_time,
    var_date,
    grop_grop_id--,
/*
union


SELECT 
SUM(nvl(mvar, 0)) mvar,SUM(nvl(A, 0)) A,
SUM(nvl(mw*pogi_stat, 0)) mw,INFV_TIME,VAR_DATE,f.grop_GROP_ID GROP_ID,0 POGI_STAT
FROM EXP_POST_GROUP a,
  EXP_POST_GROUP_INSTRU b,
  EXP_POST_VAR_INSTRU c,  EXP_POST_VAR_HEAD e,
  exp_groups_groups f
WHERE 
a.POGR_ID=b.POGR_POGR_ID
and b.EPIU_EPIU_ID=c.EPIU_EPIU_ID
and e.EPVH_ID=c.EPVH_EPVH_ID
and infv_time is not null
and a.EPOL_EPOL_ID=e.EPOL_EPOL_ID
and e.epvh_type!=6
and f.grop_grop_id_r=a.grop_grop_id


group by c.INFV_TIME,var_DATE,f.grop_GROP_ID
*/
order by mw desc
;
