create or replace force view v_logcall as
select logcall."ID_BD",logcall."ID_PTYPE",logcall."CPORT",logcall."NSESSION",logcall."DBEG",logcall."DBEG" as "TSBEG",logcall."DURATION",logcall."CEXAMINE",logcall."CRESULT",bbuildings.cshort,paramtype.CTYPE  from logcall
join bdevices on logcall.id_bd=bdevices.id_bd
join bbuildings on bdevices.id_bu=bbuildings.id_bu
left join paramtype on logcall.id_ptype=paramtype.id_type;

