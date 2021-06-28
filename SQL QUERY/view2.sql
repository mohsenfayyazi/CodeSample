--------------------------------------------------------
--  File created - Thursday-June-24-2021   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for View EXP_GOSE_V
--------------------------------------------------------

  CREATE OR REPLACE FORCE EDITIONABLE VIEW "FMIS_DBA"."EXP_GOSE_V" ("CODE_NAME", "EPOL_EPOL_ID", "EPOL_EPOL_ID_INSLIN", "EPOL_EPOL_ID_LINE", "EPOL_NAME", "EUNL_NUM", "EUNL_DESC", "ORGA_CODE", "ORGA_MANA_ASTA_CODE", "ORGA_MANA_CODE", "EPIU_ID", "GROP_GROP_ID", "CODE_DISP", "BAPT_DATE", "EXPT_DATE", "POGR_ID", "EPIU_EPIU_ID_RT") AS 
  select "CODE_NAME","EPOL_EPOL_ID","EPOL_EPOL_ID_INSLIN","EPOL_EPOL_ID_LINE","EPOL_NAME","EUNL_NUM","EUNL_DESC","ORGA_CODE","ORGA_MANA_ASTA_CODE","ORGA_MANA_CODE","EPIU_ID","GROP_GROP_ID","CODE_DISP","BAPT_DATE","EXPT_DATE","POGR_ID","EPIU_EPIU_ID_RT" from (
SELECT
    b.code_name,
    b.epol_epol_id,
    nvl(b.epol_epol_id_inslin,b.epol_epol_id) epol_epol_id_inslin,
    b.epol_epol_id_line,
    c.epol_name,
    d.eunl_num,
    d.eunl_desc,
    c.orga_code,
    c.orga_mana_asta_code,
    c.orga_mana_code,
    b.epiu_id,
    a.grop_grop_id,
    b.code_disp,
    0 pogr_id,
    b.BAPT_DATE,
    b.EXPT_DATE,
    b.epiu_epiu_id_rt
    
FROM
    exp_group_setting      a,
    exp_post_line_instru   b,
    exp_post_line          c,
    exp_unit_level         d
WHERE
    a.eins_eins_id = b.eins_eins_id
    AND a.eunl_eunl_id = b.eunl_eunl_id
    AND ( b.epol_epol_id_inslin = c.epol_id
          OR b.epol_epol_id = c.epol_id )
    AND epol_type = '0'
    AND d.eunl_id = a.eunl_eunl_id
    AND ( a.orga_code = c.orga_code
           OR a.orga_code IS NULL)
UNION
SELECT
    c.code_name,
    c.epol_epol_id,
    nvl(c.epol_epol_id_inslin,c.epol_epol_id) epol_epol_id_inslin,
    c.epol_epol_id_line,
    d.epol_name,
    e.eunl_num,
    e.eunl_desc,
    d.orga_code,
    d.orga_mana_asta_code,
    d.orga_mana_code,
    c.epiu_id,
    a.grop_grop_id,
    c.code_disp,
    0 pogr_id,
    c.BAPT_DATE,
    c.EXPT_DATE,
    c.epiu_epiu_id_rt
FROM
    exp_post_group          a,
    exp_post_group_instru   b,
    exp_post_line_instru    c,
    exp_post_line           d,
    exp_unit_level          e
WHERE
    a.POGR_ID = b.POGR_POGR_ID
    AND b.EPIU_EPIU_ID = c.epiu_id
    and (d.EPOL_ID=c.EPOL_EPOL_ID
    or       c.epol_epol_id_inslin = d.epol_id
           )
    AND epol_type = '0'
    AND e.eunl_id = c.eunl_eunl_id
  )  
    
ORDER BY
   farsi_order_u( epol_name),
    code_name
;
