---------------------------------------------
-- Export file for user COUNTERS@29F       --
-- Created by bami on 20.09.2016, 15:37:30 --
---------------------------------------------

set define off
spool all.log

prompt
prompt Creating table ANALIZER
prompt =======================
prompt
@@analizer.tab
prompt
prompt Creating table ANALIZER_CFG
prompt ===========================
prompt
@@analizer_cfg.tab
prompt
prompt Creating table BGROUPS
prompt ======================
prompt
@@bgroups.tab
prompt
prompt Creating table BBUILDINGS
prompt =========================
prompt
@@bbuildings.tab
prompt
prompt Creating table DEVCLASSES
prompt =========================
prompt
@@devclasses.tab
prompt
prompt Creating table DEVICES
prompt ======================
prompt
@@devices.tab
prompt
prompt Creating table PARAMTYPE
prompt ========================
prompt
@@paramtype.tab
prompt
prompt Creating table MASKS
prompt ====================
prompt
@@masks.tab
prompt
prompt Creating table BDEVICES
prompt =======================
prompt
@@bdevices.tab
prompt
prompt Creating table BMODEMS
prompt ======================
prompt
@@bmodems.tab
prompt
prompt Creating table CHARTSETTINGS
prompt ============================
prompt
@@chartsettings.tab
prompt
prompt Creating table COLORS
prompt =====================
prompt
@@colors.tab
prompt
prompt Creating table COMMONPARAM
prompt ==========================
prompt
@@commonparam.tab
prompt
prompt Creating table IPADDR
prompt =====================
prompt
@@ipaddr.tab
prompt
prompt Creating table COMPORTS
prompt =======================
prompt
@@comports.tab
prompt
prompt Creating table MODEMS
prompt =====================
prompt
@@modems.tab
prompt
prompt Creating table CONNECTIONS
prompt ==========================
prompt
@@connections.tab
prompt
prompt Creating table CONTRACT
prompt =======================
prompt
@@contract.tab
prompt
prompt Creating table DAN310CFG
prompt ========================
prompt
@@dan310cfg.tab
prompt
prompt Creating table DAN310SCHEMA
prompt ===========================
prompt
@@dan310schema.tab
prompt
prompt Creating table DATACURR
prompt =======================
prompt
@@datacurr.tab
prompt
prompt Creating table DATATYPE
prompt =======================
prompt
@@datatype.tab
prompt
prompt Creating table DEVSCHEMA
prompt ========================
prompt
@@devschema.tab
prompt
prompt Creating table DEVSCHEMAPARAM
prompt =============================
prompt
@@devschemaparam.tab
prompt
prompt Creating table DEVUNITS
prompt =======================
prompt
@@devunits.tab
prompt
prompt Creating table HCMESSAGES
prompt =========================
prompt
@@hcmessages.tab
prompt
prompt Creating table LOGCALL
prompt ======================
prompt
@@logcall.tab
prompt
prompt Creating table MASKSLINE
prompt ========================
prompt
@@masksline.tab
prompt
prompt Creating table MISSINGARCH
prompt ==========================
prompt
@@missingarch.tab
prompt
prompt Creating table MISSINGPASS
prompt ==========================
prompt
@@missingpass.tab
prompt
prompt Creating table PLANCALL
prompt =======================
prompt
@@plancall.tab
prompt
prompt Creating table QLIST
prompt ====================
prompt
@@qlist.tab
prompt
prompt Creating table SETUPHAN
prompt =======================
prompt
@@setuphan.tab
prompt
prompt Creating table SETUPHANLINE
prompt ===========================
prompt
@@setuphanline.tab
prompt
prompt Creating table TDENSITY
prompt =======================
prompt
@@tdensity.tab
prompt
prompt Creating table TEMP_
prompt ====================
prompt
@@temp_.tab
prompt
prompt Creating table USERGROUP
prompt ========================
prompt
@@usergroup.tab
prompt
prompt Creating table USERS
prompt ====================
prompt
@@users.tab
prompt
prompt Creating table VALUEBOUNDS
prompt ==========================
prompt
@@valuebounds.tab
prompt
prompt Creating table WEBREPORT
prompt ========================
prompt
@@webreport.tab
prompt
prompt Creating table WEBREQEST
prompt ========================
prompt
@@webreqest.tab
prompt
prompt Creating table WEBTEMPLATE
prompt ==========================
prompt
@@webtemplate.tab
prompt
prompt Creating table WHOGIVE
prompt ======================
prompt
@@whogive.tab
prompt
prompt Creating table WHOGIVETOP
prompt =========================
prompt
@@whogivetop.tab
prompt
prompt Creating sequence BBUILDINGS_SEQ
prompt ================================
prompt
@@bbuildings_seq.seq
prompt
prompt Creating sequence BDEVICES_SEQ
prompt ==============================
prompt
@@bdevices_seq.seq
prompt
prompt Creating sequence BGROUPS_SEQ
prompt =============================
prompt
@@bgroups_seq.seq
prompt
prompt Creating sequence BMODEMS_SEQ
prompt =============================
prompt
@@bmodems_seq.seq
prompt
prompt Creating sequence COLORS_SEQ
prompt ============================
prompt
@@colors_seq.seq
prompt
prompt Creating sequence COMPORTS_SEQ
prompt ==============================
prompt
@@comports_seq.seq
prompt
prompt Creating sequence CONNECTIONS_SEQ
prompt =================================
prompt
@@connections_seq.seq
prompt
prompt Creating sequence CONTRACT_SEQ
prompt ==============================
prompt
@@contract_seq.seq
prompt
prompt Creating sequence DANSCHEMA_SEQ
prompt ===============================
prompt
@@danschema_seq.seq
prompt
prompt Creating sequence DATACURR_SEQ
prompt ==============================
prompt
@@datacurr_seq.seq
prompt
prompt Creating sequence DEVSCHEMADATA_SEQ
prompt ===================================
prompt
@@devschemadata_seq.seq
prompt
prompt Creating sequence DEVSCHEMA_SEQ
prompt ===============================
prompt
@@devschema_seq.seq
prompt
prompt Creating sequence HCMESSAGES_SEQ
prompt ================================
prompt
@@hcmessages_seq.seq
prompt
prompt Creating sequence IPADDR_SEQ
prompt ============================
prompt
@@ipaddr_seq.seq
prompt
prompt Creating sequence MASKSLINE_SEQ
prompt ===============================
prompt
@@masksline_seq.seq
prompt
prompt Creating sequence MASKS_SEQ
prompt ===========================
prompt
@@masks_seq.seq
prompt
prompt Creating sequence MODEMS_SEQ
prompt ============================
prompt
@@modems_seq.seq
prompt
prompt Creating sequence PLANCALL_SEQ
prompt ==============================
prompt
@@plancall_seq.seq
prompt
prompt Creating sequence QLIST_SEQ
prompt ===========================
prompt
@@qlist_seq.seq
prompt
prompt Creating sequence SETUPHANLINE_SEQ
prompt ==================================
prompt
@@setuphanline_seq.seq
prompt
prompt Creating sequence SETUPHAN_SEQ
prompt ==============================
prompt
@@setuphan_seq.seq
prompt
prompt Creating sequence USERS_SEQ
prompt ===========================
prompt
@@users_seq.seq
prompt
prompt Creating sequence WEBREPORT_SEQ
prompt ===============================
prompt
@@webreport_seq.seq
prompt
prompt Creating sequence WEBREQUEST_SEQ
prompt ================================
prompt
@@webrequest_seq.seq
prompt
prompt Creating sequence WEBTEMPLATE_SEQ
prompt =================================
prompt
@@webtemplate_seq.seq
prompt
prompt Creating sequence WHOGIVE_SEQ
prompt =============================
prompt
@@whogive_seq.seq
prompt
prompt Creating sequence WHOGIVETOP_SEQ
prompt ================================
prompt
@@whogivetop_seq.seq
prompt
prompt Creating view ID_BY_NAME
prompt ========================
prompt
@@id_by_name.vw
prompt
prompt Creating view NODE_INFO
prompt =======================
prompt
@@node_info.vw
prompt
prompt Creating view TAB_COL_COMMENT
prompt =============================
prompt
@@tab_col_comment.vw
prompt
prompt Creating view V_DANSD
prompt =====================
prompt
@@v_dansd.vw
prompt
prompt Creating view V_DANSH
prompt =====================
prompt
@@v_dansh.vw
prompt
prompt Creating view V_DATACURR
prompt ========================
prompt
@@v_datacurr.vw
prompt
prompt Creating view V_DEVSCHEMA_IMAGE
prompt ===============================
prompt
@@v_devschema_image.vw
prompt
prompt Creating view V_DEVSHEMA
prompt ========================
prompt
@@v_devshema.vw
prompt
prompt Creating view V_DEV2
prompt ====================
prompt
@@v_dev2.vw
prompt
prompt Creating view V_DEV2_ALL
prompt ========================
prompt
@@v_dev2_all.vw
prompt
prompt Creating view V_GRP2
prompt ====================
prompt
@@v_grp2.vw
prompt
prompt Creating view V_GRP2_ALL
prompt ========================
prompt
@@v_grp2_all.vw
prompt
prompt Creating view V_L_HOUR
prompt ======================
prompt
@@v_l_hour.vw
prompt
prompt Creating view V_LINKED_DAY
prompt ==========================
prompt
@@v_linked_day.vw
prompt
prompt Creating view V_LINKED_HOUR
prompt ===========================
prompt
@@v_linked_hour.vw
prompt
prompt Creating view V_LINKED_MOMENT
prompt =============================
prompt
@@v_linked_moment.vw
prompt
prompt Creating view V_LOGCALL
prompt =======================
prompt
@@v_logcall.vw
prompt
prompt Creating view V_PROBLEM_CONNECTIONS
prompt ===================================
prompt
@@v_problem_connections.vw
prompt
prompt Creating view V_STATUS
prompt ======================
prompt
@@v_status.vw
prompt
prompt Creating view V_T106
prompt ====================
prompt
@@v_t106.vw
prompt
prompt Creating view V$USER
prompt ====================
prompt
@@v$user.vw
prompt
prompt Creating function TONUMBER
prompt ==========================
prompt
@@tonumber.fnc
prompt
prompt Creating function COUNTEXPEND
prompt =============================
prompt
@@countexpend.fnc
prompt
prompt Creating procedure SI2SPT942
prompt ============================
prompt
@@si2spt942.prc
prompt
prompt Creating procedure LST_SPT942SI2
prompt ================================
prompt
@@lst_spt942si2.prc
prompt
prompt Creating procedure LST_FULL
prompt ===========================
prompt
@@lst_full.prc
prompt
prompt Creating procedure AUTO_LST_FULL
prompt ================================
prompt
@@auto_lst_full.prc
prompt
prompt Creating procedure AUTO_LST_MO
prompt ==============================
prompt
@@auto_lst_mo.prc
prompt
prompt Creating procedure BAMI_SI1SPT943
prompt =================================
prompt
@@bami_si1spt943.prc
prompt
prompt Creating procedure BAMI_SI2MT200
prompt ================================
prompt
@@bami_si2mt200.prc
prompt
prompt Creating procedure BAMI_SI2SPT942
prompt =================================
prompt
@@bami_si2spt942.prc
prompt
prompt Creating procedure BAMI_SI2SPT943
prompt =================================
prompt
@@bami_si2spt943.prc
prompt
prompt Creating procedure BAMI_SI2SPT960
prompt =================================
prompt
@@bami_si2spt960.prc
prompt
prompt Creating procedure BAMI_SI2TSR
prompt ==============================
prompt
@@bami_si2tsr.prc
prompt
prompt Creating procedure BAMI_SI2TV2SPT943
prompt ====================================
prompt
@@bami_si2tv2spt943.prc
prompt
prompt Creating procedure BAMI_SI5SPT943
prompt =================================
prompt
@@bami_si5spt943.prc
prompt
prompt Creating procedure BAMI_SI5TSR
prompt ==============================
prompt
@@bami_si5tsr.prc
prompt
prompt Creating procedure BAMI_SI6SPT942
prompt =================================
prompt
@@bami_si6spt942.prc
prompt
prompt Creating procedure BAMI_SI6TSR
prompt ==============================
prompt
@@bami_si6tsr.prc
prompt
prompt Creating procedure CHECK24
prompt ==========================
prompt
@@check24.prc
prompt
prompt Creating procedure CHECK24LST
prompt =============================
prompt
@@check24lst.prc
prompt
prompt Creating procedure CHEK_DAY_ARCHIVE
prompt ===================================
prompt
@@chek_day_archive.prc
prompt
prompt Creating procedure EXPTOTEXT
prompt ============================
prompt
@@exptotext.prc
prompt
prompt Creating procedure INITCHARTS
prompt =============================
prompt
@@initcharts.prc
prompt
prompt Creating procedure INSEMPTY
prompt ===========================
prompt
@@insempty.prc
prompt
prompt Creating procedure INSEMPTYW
prompt ============================
prompt
@@insemptyw.prc
prompt
prompt Creating procedure SI2MT200
prompt ===========================
prompt
@@si2mt200.prc
prompt
prompt Creating procedure LST_MT200SI2
prompt ===============================
prompt
@@lst_mt200si2.prc
prompt
prompt Creating procedure SI2SPT942GV4
prompt ===============================
prompt
@@si2spt942gv4.prc
prompt
prompt Creating procedure LST_SPT942SI2GV4
prompt ===================================
prompt
@@lst_spt942si2gv4.prc
prompt
prompt Creating procedure SI2SPT942GV5
prompt ===============================
prompt
@@si2spt942gv5.prc
prompt
prompt Creating procedure LST_SPT942SI2GV5
prompt ===================================
prompt
@@lst_spt942si2gv5.prc
prompt
prompt Creating procedure SI6SPT942
prompt ============================
prompt
@@si6spt942.prc
prompt
prompt Creating procedure LST_SPT942SI6
prompt ================================
prompt
@@lst_spt942si6.prc
prompt
prompt Creating procedure SI1SPT943
prompt ============================
prompt
@@si1spt943.prc
prompt
prompt Creating procedure LST_SPT943SI1
prompt ================================
prompt
@@lst_spt943si1.prc
prompt
prompt Creating procedure SI2SPT943
prompt ============================
prompt
@@si2spt943.prc
prompt
prompt Creating procedure LST_SPT943SI2
prompt ================================
prompt
@@lst_spt943si2.prc
prompt
prompt Creating procedure SI2SPT943TV2
prompt ===============================
prompt
@@si2spt943tv2.prc
prompt
prompt Creating procedure LST_SPT943SI2TV2
prompt ===================================
prompt
@@lst_spt943si2tv2.prc
prompt
prompt Creating procedure SI5SPT943
prompt ============================
prompt
@@si5spt943.prc
prompt
prompt Creating procedure LST_SPT943SI5
prompt ================================
prompt
@@lst_spt943si5.prc
prompt
prompt Creating procedure SI2SPT960
prompt ============================
prompt
@@si2spt960.prc
prompt
prompt Creating procedure LST_SPT960SI2
prompt ================================
prompt
@@lst_spt960si2.prc
prompt
prompt Creating procedure SI2TSR
prompt =========================
prompt
@@si2tsr.prc
prompt
prompt Creating procedure LST_TSRSI2
prompt =============================
prompt
@@lst_tsrsi2.prc
prompt
prompt Creating procedure SI5TSR
prompt =========================
prompt
@@si5tsr.prc
prompt
prompt Creating procedure LST_TSRSI5
prompt =============================
prompt
@@lst_tsrsi5.prc
prompt
prompt Creating procedure SI6TSR
prompt =========================
prompt
@@si6tsr.prc
prompt
prompt Creating procedure LST_TSRSI6
prompt =============================
prompt
@@lst_tsrsi6.prc
prompt
prompt Creating procedure SI2VKT7
prompt ==========================
prompt
@@si2vkt7.prc
prompt
prompt Creating procedure LST_VKT7SI2
prompt ==============================
prompt
@@lst_vkt7si2.prc
prompt
prompt Creating procedure SAVEMT2
prompt ==========================
prompt
@@savemt2.prc
prompt
prompt Creating procedure SMT2WORK
prompt ===========================
prompt
@@smt2work.prc
prompt
prompt Creating procedure SSPTWORK
prompt ===========================
prompt
@@ssptwork.prc
prompt
prompt Creating procedure SSPT942
prompt ==========================
prompt
@@sspt942.prc
prompt
prompt Creating procedure TEST
prompt =======================
prompt
@@test.prc
prompt
prompt Creating procedure VKT24
prompt ========================
prompt
@@vkt24.prc
prompt
prompt Creating procedure VKTSAVE
prompt ==========================
prompt
@@vktsave.prc
prompt
prompt Creating procedure VKTWORK
prompt ==========================
prompt
@@vktwork.prc
prompt
prompt Creating procedure WRITELOG
prompt ===========================
prompt
@@writelog.prc
prompt
prompt Creating procedure XML_SI1SPT942TV1
prompt ===================================
prompt
@@xml_si1spt942tv1.prc
prompt
prompt Creating procedure XML_LST_FULL
prompt ===============================
prompt
@@xml_lst_full.prc
prompt
prompt Creating trigger DATACURR_NOW
prompt =============================
prompt
@@datacurr_now.trg

spool off
