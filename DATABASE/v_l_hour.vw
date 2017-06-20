create or replace force view v_l_hour as
select datacurr."ID",datacurr."ID_BD",datacurr."ID_DU",datacurr."ID_PTYPE",datacurr."DCALL",datacurr."DCOUNTER",datacurr."Q1",datacurr."Q2",datacurr."T1",datacurr."T2",datacurr."DT12",datacurr."T3",datacurr."T4",datacurr."T5",datacurr."DT45",datacurr."T6",datacurr."V1",datacurr."V2",datacurr."DV12",datacurr."V3",datacurr."V4",datacurr."V5",datacurr."DV45",datacurr."V6",datacurr."M1",datacurr."M2",datacurr."DM12",datacurr."M3",datacurr."M4",datacurr."M5",datacurr."DM45",datacurr."M6",datacurr."P1",datacurr."P2",datacurr."P3",datacurr."P4",datacurr."P5",datacurr."P6",datacurr."G1",datacurr."G2",datacurr."G3",datacurr."G4",datacurr."G5",datacurr."G6",datacurr."TCOOL",datacurr."TCE1",datacurr."TCE2",datacurr."TSUM1",datacurr."TSUM2",datacurr."Q1H",datacurr."Q2H",datacurr."V1H",datacurr."V2H",datacurr."V4H",datacurr."V5H",datacurr."ERRTIME",datacurr."ERRTIMEH",datacurr."HC",datacurr."SP",datacurr."SP_TB1",datacurr."SP_TB2",datacurr."DATECOUNTER",datacurr."DG12",datacurr."DG45",datacurr."DP12",datacurr."DP45",datacurr."UNITSR",datacurr."Q3",datacurr."Q4",datacurr."PATM",datacurr."Q5",datacurr."DQ12",datacurr."DQ45",datacurr."PXB",datacurr."DQ",datacurr."HC_1",datacurr."HC_2",datacurr."THOT",datacurr."DANS1",datacurr."DANS2",datacurr."DANS3",datacurr."DANS4",datacurr."DANS5",datacurr."DANS6",datacurr."CHECK_A",datacurr."OKTIME",datacurr."WORKTIME",datacurr."TAIR1",datacurr."TAIR2",datacurr."HC_CODE",datacurr."ERRTIME2",datacurr."OKTIME2",datacurr."Q6",datacurr."D_EQL_24",datacurr."HCRAW1",datacurr."HCRAW2",datacurr."HCRAW"
,datacurr."MG1",datacurr."MG2"
,dans.tair1   as L_tair1  ,dans.t1   as L_t1  ,dans.t2   as L_t2  ,dans.t3   as L_t3  ,dans.t4   as L_t4  ,dans.t5   as L_t5  ,dans.t6   as L_t6             ,dans.G1   as L_G1  ,dans.G2   as L_G2  ,dans.G3   as L_G3  ,dans.G4   as L_G4  ,dans.G5   as L_G5  ,dans.G6   as L_G6                        ,dans.v1   as L_v1  ,dans.v2   as L_v2  ,dans.v3   as L_v3  ,dans.v4   as L_v4  ,dans.v5   as L_v5  ,dans.v6   as L_v6             ,dans.Q1   as L_Q1  ,dans.Q2   as L_Q2  ,dans.Q3   as L_Q3  ,dans.Q4   as L_Q4                        ,dans.p1   as L_p1  ,dans.p2   as L_p2  ,dans.p3   as L_p3  ,dans.p4   as L_p4  ,dans.p5   as L_p5  ,dans.p6   as L_p6
  from datacurr
join bdevices on datacurr.id_bd=bdevices.ID_BD
left join datacurr dans on dans.id_bd=bdevices.Linked_id_bd and dans.id_ptype=1 and dans.dcounter >=datacurr.dcounter and dans.dcounter <=(datacurr.dcounter +15/24/60);

