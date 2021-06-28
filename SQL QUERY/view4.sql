--------------------------------------------------------
--  File created - Thursday-June-24-2021   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for View EXP_POST_V
--------------------------------------------------------

  CREATE OR REPLACE FORCE EDITIONABLE VIEW "FMIS_DBA"."EXP_POST_V" ("EXP_POST_LINE_EPOL_EPOL_ID", "EXP_POST_LINE_EPOL_ID", "EXP_POST_LINE_EPOL_TYPE", "EXP_POST_LINE_EPOL_STAT", "EXP_POST_LINE_CODE_DISP", "EXP_POST_LINE_CODE_TARH", "EXP_POST_LINE_CURE_MOJA", "EXP_POST_LINE_EXPL_DATE", "EXP_POST_LINE_INCO_TYPE", "EXP_POST_LINE_LINE_LENT", "EXP_POST_LINE_POST_TYPE", "EXP_POST_LINE_STAC_DYNM", "EXP_POST_LINE_ORGA_CODE", "EXP_POST_LINE_ORGA_MANA_CODE", "EXP_POST_LINE_ORGA_MANA_ASTA_C", "EXP_POST_LINE_EUNL_EUNL_ID", "EXP_POST_LINE_ELRD_ELRD_ID", "EXP_POST_LINE_EOTY_EOTY_ID", "EXP_POST_LINE_GEOL_G_CODE", "EXP_POST_LINE_EARD_EARD_ID", "EXP_POST_LINE_EDIC_EDIC_ID", "EXP_POST_LINE_EOCO_EOCO_ID", "EXP_POST_LINE_EPOL_NAME", "PAY_ASSISTANT_CODE", "PAY_ASSISTANT_ASTA_DESC", "PAY_MANAGEMENT_CODE", "PAY_MANAGEMENT_MANA_DESC", "PAY_ORGAN_CODE", "PAY_ORGAN_ORGA_DESC", "EXP_UNIT_LEVEL_EUNL_ID", "EXP_UNIT_LEVEL_EUNL_DESC", "EXP_UNIT_LEVEL_EUNL_NUM", "EXP_AOC_RDC_EARD_ID", "EXP_AOC_RDC_EARD_NAME", "EXP_LOCATION_RDC_ELRD_ID", "EXP_LOCATION_RDC_ELRD_DESC", "EXP_OWENER_COMPANY_EOCO_ID", "EXP_OWENER_COMPANY_EOCO_DESC", "EXP_OWENER_TYPE_EOTY_ID", "EXP_OWENER_TYPE_EOTY_DESC", "EXP_DISTRIBUTE_COMPANY_EDIC_ID", "EXP_DISTRIBUTE_COMPANY_EDIC_DE", "BKP_GEOGH_LOC_G_CODE", "BKP_GEOGH_LOC_G_DESC") AS 
  SELECT EXP_POST_LINE.EPOL_EPOL_ID,
  EXP_POST_LINE.EPOL_ID,
  EXP_POST_LINE.EPOL_TYPE,
  EXP_POST_LINE.EPOL_STAT,
  EXP_POST_LINE.CODE_DISP,
  EXP_POST_LINE.CODE_TARH,
  EXP_POST_LINE.CURE_MOJA,
  EXP_POST_LINE.EXPL_DATE,
  EXP_POST_LINE.INCO_TYPE,
  EXP_POST_LINE.LINE_LENT,
  EXP_POST_LINE.POST_TYPE,
  EXP_POST_LINE.STAC_DYNM,
  EXP_POST_LINE.ORGA_CODE,
  EXP_POST_LINE.ORGA_MANA_CODE,
  EXP_POST_LINE.ORGA_MANA_ASTA_CODE,
  EXP_POST_LINE.EUNL_EUNL_ID,
  EXP_POST_LINE.ELRD_ELRD_ID,
  EXP_POST_LINE.EOTY_EOTY_ID,
  EXP_POST_LINE.GEOL_G_CODE,
  EXP_POST_LINE.EARD_EARD_ID,
  EXP_POST_LINE.EDIC_EDIC_ID,
  EXP_POST_LINE.EOCO_EOCO_ID,
  EXP_POST_LINE.EPOL_NAME,
  PAY_ASSISTANT.CODE,
  PAY_ASSISTANT.ASTA_DESC,
  PAY_MANAGEMENT.CODE,
  PAY_MANAGEMENT.MANA_DESC,
  PAY_ORGAN.CODE,
  PAY_ORGAN.ORGA_DESC,
  EXP_UNIT_LEVEL.EUNL_ID,
  EXP_UNIT_LEVEL.EUNL_DESC,
  EXP_UNIT_LEVEL.EUNL_NUM,
  EXP_AOC_RDC.EARD_ID,
  EXP_AOC_RDC.EARD_NAME,
  EXP_LOCATION_RDC.ELRD_ID,
  EXP_LOCATION_RDC.ELRD_DESC,
  EXP_OWENER_COMPANY.EOCO_ID,
  EXP_OWENER_COMPANY.EOCO_DESC,
  EXP_OWENER_TYPE.EOTY_ID,
  EXP_OWENER_TYPE.EOTY_DESC,
  EXP_DISTRIBUTE_COMPANY.EDIC_ID,
  EXP_DISTRIBUTE_COMPANY.EDIC_DESC,
  BKP_GEOGH_LOC.G_CODE,
  BKP_GEOGH_LOC.G_DESC
FROM EXP_POST_LINE,
  PAY_ORGAN,
  PAY_MANAGEMENT,
  PAY_ASSISTANT,
  EXP_UNIT_LEVEL,
  EXP_AOC_RDC ,
  EXP_OWENER_COMPANY,
  EXP_OWENER_TYPE,
  EXP_LOCATION_RDC ,
  EXP_DISTRIBUTE_COMPANY,
  BKP_GEOGH_LOC
WHERE PAY_ORGAN.MANA_CODE     =PAY_MANAGEMENT.CODE
AND PAY_ORGAN.MANA_ASTA_CODE  =PAY_MANAGEMENT.ASTA_CODE
AND PAY_MANAGEMENT.ASTA_CODE  =PAY_ASSISTANT.CODE
AND PAY_ORGAN.MANA_ASTA_CODE  =EXP_POST_LINE.ORGA_MANA_ASTA_CODE
AND PAY_ORGAN.MANA_CODE       =EXP_POST_LINE.ORGA_MANA_CODE
AND PAY_ORGAN.CODE(+)         =EXP_POST_LINE.ORGA_CODE
AND EXP_POST_LINE.EUNL_EUNL_ID=EXP_UNIT_LEVEL.EUNL_ID(+)
AND EXP_POST_LINE.ELRD_ELRD_ID=EXP_LOCATION_RDC.ELRD_ID(+)
AND EXP_POST_LINE.EARD_EARD_ID=EXP_AOC_RDC.EARD_ID(+)
AND EXP_POST_LINE.EOTY_EOTY_ID=EXP_OWENER_TYPE.EOTY_ID(+)
AND EXP_POST_LINE.EOCO_EOCO_ID=EXP_OWENER_COMPANY.EOCO_ID(+)
AND EXP_POST_LINE.GEOL_G_CODE =BKP_GEOGH_LOC.G_CODE(+)
AND EXP_POST_LINE.EDIC_EDIC_ID=EXP_DISTRIBUTE_COMPANY.EDIC_ID(+)
 
;
