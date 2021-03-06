﻿Imports System.Data
Imports System.IO
Imports System.Reflection
Imports System.Xml
Imports Oracle.ManagedDataAccess.Client
Imports NLog

Public Class TVMain

    Private Inited As Boolean
    Private m_DBTableName As String = "datacurr"
    Private Shared Logger As Logger = LogManager.GetCurrentClassLogger()




    Public Overridable Property DBTableName() As String
        Get
            Return (m_DBTableName)
        End Get
        Set(ByVal value As String)
            m_DBTableName = value
        End Set
    End Property


    Private m_Status As String = "trial"

    Public ReadOnly Property Status() As String
        Get

            Dim d As Date = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location)
            If m_Status.ToLower() = "trial" Then
                If DateTime.Now() < d.AddDays(10) Then
                    Return "assv,ip,robustel,web"
                End If
            End If

            Return (m_Status)
        End Get
    End Property

    Private Mymodem As Integer = -1
    Private PhoneLineType As String = "T"

    Private mPortBusy As Boolean
    Public Property PortBusy() As Boolean
        Get
            Return mPortBusy
        End Get
        Set(ByVal value As Boolean)
            mPortBusy = value
        End Set
    End Property

    Public Event Idle()
    Public Event TransportStatus(ByVal Action As UnitransportAction, ByVal MSG As String)

    Public Function ReadSystemParameters() As DataTable
        If Not TVD Is Nothing Then
            If TVD.IsConnected Then
                Return TVD.ReadSystemParameters()
            Else
                Return Nothing
            End If


        Else
            Return Nothing
        End If

    End Function


    Public WithEvents TVD As TVDriver
    Dim connection As OracleConnection
    Dim command As OracleCommand
    Dim PortID As Short
    Dim m_RequestInterval As Integer
    Dim DeviceReady As Boolean
    Dim m_DataSourceName As String
    Dim m_ShowHidden As Boolean = False
    Dim m_LogEnabled As Boolean = False
    Dim m_SendToTGK As Boolean = True
    Dim m_ConnectStatus As String = ""
    Dim m_AutoOpen As String = "Данные"
    Public Function AutoOpen() As String
        Return m_AutoOpen
    End Function
    Public Function ConnectStatus() As String
        Return m_ConnectStatus
    End Function

    Public Property LogEnabled() As Boolean
        Get
            Return m_LogEnabled
        End Get
        Set(ByVal value As Boolean)
            m_LogEnabled = value
        End Set
    End Property
    Protected Shared Function GetMyDir() As String
        Dim s As String
        s = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)
        s = s.Substring(6)
        Return s
    End Function


    Protected Sub LOG(ByVal id_bd As Integer, ByVal s As String)
        If (id_bd = 0) Then
            NLog.GlobalDiagnosticsContext.Set("counter", "_system")
            NLog.GlobalDiagnosticsContext.Set("id", 0)
        Else
            NLog.GlobalDiagnosticsContext.Set("counter", "_" & id_bd.ToString())
            NLog.GlobalDiagnosticsContext.Set("id", id_bd)
        End If

        Logger.Info(s)

        'End If
    End Sub


    Public Property SendToTGK() As Boolean
        Get
            Return m_SendToTGK
        End Get
        Set(ByVal value As Boolean)
            m_SendToTGK = value
        End Set
    End Property
    Public Property ShowHidden() As Boolean
        Get
            Return m_ShowHidden
        End Get
        Set(ByVal value As Boolean)
            m_ShowHidden = value
        End Set
    End Property
    Public Property DataSourceName() As String
        Get
            Return m_DataSourceName
        End Get
        Set(ByVal value As String)
            m_DataSourceName = value
        End Set
    End Property

    Public Property RequestInterval() As Integer
        Get
            Return m_RequestInterval
        End Get
        Set(ByVal value As Integer)
            m_RequestInterval = value
        End Set
    End Property
    Public Function dbconnect() As OracleConnection
        Return connection
    End Function
    Private Structure ToolTipStruct
        Dim ip As String
        Dim schemeName As String
        Dim Caddress As String
        Dim device As String
    End Structure
    Public Structure ConfigStruct
        Dim ip As String
        Dim ipport As String
        Dim Transport As String
        Dim password As String
        Dim device As String
        Dim NPQuery As Integer
        Dim HideRow As Integer
    End Structure

    Public Function OracleDate(ByVal d As Date) As String
        Return "to_date('" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + _
            " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS')"
    End Function

    Public Function OracleTimeStamp(ByVal d As Date) As String
        Return "TO_TIMESTAMP('" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + _
            " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString() + "." + d.Millisecond.ToString().PadLeft(3, "0") + "','YYYY-MM-DD HH24:MI:SS.FF')"
    End Function

    Public Sub ClearDBArchString(ByVal ArchType As Int32, ByVal ArchYear As Int32, _
    ByVal ArchMonth As Int32, ByVal ArchDay As Int32, ByVal ArchHour As Int32, ByVal id_bd As Int32)
        Dim cmd As New OracleCommand()
        Dim datearch As DateTime
        Dim after As Date, befor As Date
        datearch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)



        after = datearch.AddSeconds(-1)
        befor = datearch.AddSeconds(1)
        cmd.Connection = connection
        cmd.CommandText = "delete from " & DBTableName & " where dcounter>=" + _
        OracleDate(after) + " and dcounter<=" + _
        OracleDate(befor) + " and id_ptype=" + ArchType.ToString() + _
        "and id_bd=" + id_bd.ToString()
        Debug.Print(cmd.CommandText)
        Try
            'SyncLock connection
            cmd.Connection = connection
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            'End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Public Sub PrepareDataWork(ByVal DateFrom As Date, ByVal DateTo As Date)
        Dim cmd As New OracleCommand()
        cmd.CommandText = "delete from datawork where dcounter>=" + _
        OracleDate(DateFrom) + " and dcounter<=" + _
        OracleDate(DateTo) + ""

        Try
            ''SyncLock connection
            cmd.Connection = connection
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        cmd.CommandText = "insert into datawork " & DBTableName & " where dcounter>=" + _
        OracleDate(DateFrom) + " and dcounter<=" + _
        OracleDate(DateTo) + ""


        Try
            '' SyncLock connection
            cmd.Connection = connection
            cmd.ExecuteNonQuery()
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try


    End Sub




    Public Function CheckForArch(ByVal ArchType As Int32, ByVal ArchYear As Int32, _
    ByVal ArchMonth As Int32, ByVal ArchDay As Int32, ByVal ArchHour As Int32, ByVal id_bd As Int32) As Boolean
        Dim cmd As New OracleCommand()
        Dim datearch As DateTime
        Dim after As Date, befor As Date
        datearch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)



        after = datearch.AddSeconds(-1)
        befor = datearch.AddSeconds(1)
        cmd.Connection = connection
        cmd.CommandText = "select count(*) CNT  from " & DBTableName & " where dcounter>=" + _
        OracleDate(after) + " and dcounter<=" + _
        OracleDate(befor) + " and id_ptype=" + ArchType.ToString() + _
        "and id_bd=" + id_bd.ToString() + ""



        Dim da As OracleDataAdapter
        Dim dt As DataTable = New DataTable
        cmd.Connection = connection
        da = New OracleDataAdapter(cmd)
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock

            da.Dispose()
            cmd.Dispose()
            If dt.Rows.Count > 0 Then

                If dt.Rows(0)("CNT") > 0 Then

                    Return True
                End If
            End If



        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return False
    End Function



    Public Function GetRealDateFromBase(ByVal ArchType As Int32, ByVal ArchYear As Int32, _
    ByVal ArchMonth As Int32, ByVal ArchDay As Int32, ByVal ArchHour As Int32, ByVal id_bd As Int32) As DateTime
        Dim cmd As New OracleCommand()
        Dim datearch As DateTime
        Dim after As Date, befor As Date
        datearch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)


        after = datearch.AddSeconds(-1)
        befor = datearch.AddSeconds(1)
        cmd.Connection = connection
        cmd.CommandText = "select dcounter  from " & DBTableName & " where dcounter>=" + _
        OracleDate(after) + " and dcounter<=" + _
        OracleDate(befor) + " and id_ptype=" + ArchType.ToString() + _
        "and id_bd=" + id_bd.ToString() + " "

        Dim da As OracleDataAdapter
        Dim dt As DataTable = New DataTable
        cmd.Connection = connection
        da = New OracleDataAdapter(cmd)
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
            If dt.Rows.Count > 0 Then

                Return dt.Rows(0)("dcounter")


            End If


        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return DateTime.Now
    End Function

    Public Function GetDBRecords(ByVal DeviceID As Int32) As DataTable
        Dim dt As DataTable = Nothing
        Dim cmd As New OracleCommand()

        cmd.CommandText = "select * from  v_" & DBTableName & " where dcounter >sysdate-40" + "  "

        Dim da As OracleDataAdapter
        da = New OracleDataAdapter

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
            Return dt

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return dt
    End Function

   


    Public Function GetDBRecords2(ByVal id_bd As String, ByVal after As Date, ByVal befor As Date, ByVal archtype As Short) As DataTable
        Dim dt As DataTable = Nothing
        Dim cmd As New OracleCommand()
        Try
            after = after.AddSeconds(-1)
        Catch
        End Try
        befor = befor.AddSeconds(1)

        cmd.CommandText = "select * from v_" & DBTableName & " where dcounter>=" + _
        OracleDate(after) + " and dcounter<=" + _
        OracleDate(befor) + "and id_ptype=" + archtype.ToString + _
        " and id_bd=" + id_bd + " "
        'dt = New DataTable
        cmd.Connection = connection


        Dim da As OracleDataAdapter
        da = New OracleDataAdapter

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
            Return dt

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        Return dt
    End Function

    Public Function GetDBDevicePlanListAll() As DataTable
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        'cmd.CommandText = "select * from plancall where cstatus=0"
        cmd.CommandText = "select plancall.*,npip,nppassword,ipport,transport,sysdate ServerDate from bdevices join plancall on bdevices.id_bd=plancall.id_bd where  transport in (0,2,3) and npquery=1 and( nplock is null or nplock < sysdate) " & _
            " and (plancall.dlock is null or plancall.dlock + plancall.MINREPEAT/24/60 < sysdate ) and plancall.ncall < plancall.nmaxcall " & _
            "and ((plancall.chour=1 and nvl(plancall.DNEXTHOUR,sysdate-1)<=sysdate) or (plancall.ccurr=1 and nvl(plancall.dnextcurr,sysdate-1) <=sysdate) or (plancall.c24=1 and nvl(plancall.dnext24 ,sysdate-1)<=sysdate)  or (plancall.csum=1 and nvl(plancall.dnextsum ,sysdate-1)<=sysdate)) " + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
        Catch
        End Try

        Return dt
    End Function

    Public Function GetDBDevicePlanListIP() As DataTable
        'Dim da As System.Data.Common.DbDataAdapter
        Dim MyName As String
        MyName = Environment.MachineName
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        'cmd.CommandText = "select * from plancall where cstatus=0"
        cmd.CommandText = "select plancall.*,npip,nppassword,ipport,transport,sysdate ServerDate from bdevices" + _
         " join ipaddr on bdevices.srv_ip_id=ipaddr.id_ip  and ipaddr.terminal='" + MyName + "' " + _
        " join plancall on bdevices.id_bd=plancall.id_bd where  transport in (2,3) and npquery=1 and( nplock is null or nplock < sysdate) " & _
        " and (plancall.dlock is null or plancall.dlock + plancall.MINREPEAT/24/60 < sysdate ) and plancall.ncall < plancall.nmaxcall " & _
        " and ((plancall.chour=1 and nvl(plancall.DNEXTHOUR,sysdate-1)<=sysdate) or (plancall.ccurr=1 and nvl(plancall.dnextcurr,sysdate-1) <=sysdate) or (plancall.c24=1 and nvl(plancall.dnext24 ,sysdate-1)<=sysdate)  or (plancall.csum=1 and nvl(plancall.dnextsum ,sysdate-1)<=sysdate)) " + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
        Catch
        End Try

        Return dt
    End Function

    Public Function GetDBDevicePlanListModem() As DataTable
        'Dim da As System.Data.Common.DbDataAdapter

        Dim MyName As String
        MyName = Environment.MachineName


        'query = "select ID_CP,CIPADDR,TERMINAL,MACHINE,COMPORT,CTYPECALL ,useduntil from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where ( UsedUntil is null or UsedUntil < sysdate) and TERMINAL='" & MyName & "'" + "  order by COMPORT"

        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        'cmd.CommandText = "select * from plancall where cstatus=0"
        cmd.CommandText = "select plancall.*,npip,nppassword,ipport,transport,sysdate ServerDate from bdevices" + _
            " join ipaddr on bdevices.srv_ip_id=ipaddr.id_ip  and ipaddr.terminal='" + MyName + "' " + _
            " join plancall on bdevices.id_bd=plancall.id_bd where  transport in (0,4) and npquery=1 and( nplock is null or nplock < sysdate) " & _
            " and (plancall.dlock is null or plancall.dlock + plancall.MINREPEAT/24/60 < sysdate ) and plancall.ncall < plancall.nmaxcall " & _
            " and ((plancall.chour=1 and nvl(plancall.DNEXTHOUR,sysdate-1)<=sysdate) or (plancall.ccurr=1 and nvl(plancall.dnextcurr,sysdate-1) <=sysdate) or (plancall.c24=1 and nvl(plancall.dnext24 ,sysdate-1)<=sysdate)  or (plancall.csum=1 and nvl(plancall.dnextsum ,sysdate-1)<=sysdate)) "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
        Catch
        End Try

        Return dt
    End Function


    Public Function GetDBOneDevicePlanList(ByVal DevID As Integer) As DataTable
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        'cmd.CommandText = "select * from plancall where cstatus=0"
        cmd.CommandText = "select plancall.*,npip,nppassword,ipport,transport,sysdate ServerDate from bdevices join plancall on bdevices.id_bd=plancall.id_bd where  ( nplock is null or nplock <sysdate ) and npquery=1 " & _
        " and (plancall.dlock is null or plancall.dlock + plancall.MINREPEAT/24/60 < sysdate ) and plancall.ncall < plancall.nmaxcall " & _
        " and ((plancall.chour=1 and nvl(plancall.DNEXTHOUR,sysdate-1)<=sysdate) or (plancall.ccurr=1 and nvl(plancall.dnextcurr,sysdate-1) <=sysdate) or (plancall.c24=1 and nvl(plancall.dnext24 ,sysdate-1)<=sysdate)  or (plancall.csum=1 and nvl(plancall.dnextsum ,sysdate-1)<=sysdate)) " & _
        " and bdevices.id_bd=" & DevID.ToString() + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
        Catch
        End Try

        Return dt



    End Function



    Public Function GetRequestList(ByVal DevID As Integer) As DataTable
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection

        cmd.CommandText = "select WEBREQEST.*,npip,nppassword,ipport,transport,sysdate ServerDate from bdevices join WEBREQEST on bdevices.id_bd=WEBREQEST.id_bd where WEBREQEST.Processed=0 and ( nplock is null or nplock <sysdate ) and npquery=1 " & _
        " and ((WEBREQEST.chour=1 ) or (WEBREQEST.ccurr=1 ) or (WEBREQEST.c24=1 )  or (WEBREQEST.csum=1 )) " & _
        " and bdevices.id_bd=" & DevID.ToString() + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
            cmd.Dispose()
        Catch
        End Try

        Return dt



    End Function



    Public Function LockDevice(ByVal DevID As Integer, ByVal LockSeconds As Integer, ByVal NoCheck As Boolean) As Boolean
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection


        cmd.CommandText = "select npip,sysdate ServerDate from bdevices  where  ( nplock is null or nplock <sysdate ) /* and  transport in (0,1,2,3,4,5,6,7)  and npquery=1 */  and bdevices.id_bd=" & DevID.ToString() + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
        Catch
        End Try

        If NoCheck Or dt.Rows.Count > 0 Then
            cmd.CommandText = "update bdevices set nplock =sysdate + (0.00001*" + LockSeconds.ToString + ") where bdevices.id_bd=" + DevID.ToString()
            cmd.ExecuteNonQuery()
            SaveLog(DevID, 0, "??", 1, "Блокировка  устройства на " + LockSeconds.ToString + " сек.")
            cmd.Dispose()
            Return True
        End If
        Return False
    End Function

    Public Function UnLockDevice(ByVal DevID As Integer) As Boolean
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection


        cmd.CommandText = "select npip,sysdate ServerDate from bdevices  where  ( nplock >=sysdate ) /* and   transport in (0,1,2,3,4,5,6)  and npquery=1 */  and bdevices.id_bd=" + DevID.ToString() + " "

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock
            da.Dispose()
        Catch
        End Try

        If dt.Rows.Count > 0 Then
            cmd.CommandText = "update bdevices set nplock = null where bdevices.id_bd=" + DevID.ToString()
            cmd.ExecuteNonQuery()
            SaveLog(DevID, 0, "??", 1, "Снятие блокировки устройства ")
            cmd.Dispose()
            Return True
        End If
        Return False
    End Function



    Public Function GetDBDateTime() As DateTime
        'Dim da As System.Data.Common.DbDataAdapter
        Dim da As OracleDataAdapter
        Dim dt As DataTable
        da = New OracleDataAdapter

        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        'cmd.CommandText = "select * from plancall where cstatus=0"
        cmd.CommandText = "select sysdate d from dual"

        da.SelectCommand = cmd
        dt = New DataTable
        Try
            SyncLock connection
                da.Fill(dt)
            End SyncLock

            da.Dispose()
            cmd.Dispose()
            Return Convert.ToDateTime(dt.Rows(0)("d"))
        Catch
            da.Dispose()
            cmd.Dispose()
            Return System.DateTime.Now
        End Try

    End Function
    Public Function GetDBRecordsToListBox() As DataTable
        Dim dt As DataTable = Nothing
        Dim dr As OracleDataReader
        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        cmd.CommandText = "select id_bd,cshort from bdevices,bbuildings where bbuildings.id_bu=bdevices.id_bu" + " "

        dt = New DataTable
        dt.Columns.Add("id_bd", GetType(System.Int32))
        dt.Columns.Add("cshort", GetType(System.String))

        Dim drow As DataRow


        Try
            SyncLock connection
                dr = cmd.ExecuteReader()
                If dr.HasRows Then
                    While dr.Read()
                        drow = dt.NewRow
                        drow("id_bd") = dr.Item("id_bd")
                        drow("cshort") = dr.Item("cshort")
                        dt.Rows.Add(drow)
                    End While
                End If
                dr.Close()
            End SyncLock
            dr.Dispose()
            cmd.Dispose()

            Return dt

        Catch ex As Exception
            'MsgBox(ex.Message)
            Console.WriteLine(ex.Message)
            cmd.Dispose()
        End Try
        Return dt
    End Function
    Public Function CounterName() As String
        Return TVD.CounterName
    End Function
    Public Sub CloseDBConnection()
        Try
            If Not connection Is Nothing Then
                If (connection.State = ConnectionState.Open) Then connection.Close()
                connection = Nothing
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub
    Public Function DeviceInit(ByVal id_bd As Int32, Optional ByVal UsePort As String = "(Любой)", Optional ByVal aSocket As GRPSSocket = Nothing) As Boolean
        ClearDuration()
        Dim deviceid As Int32
        Dim IPstr As String = String.Empty
        Dim DrvStr As String = String.Empty
        Dim NPPassword As String = String.Empty
        m_ConnectStatus = ""

        Dim dr As OracleDataReader
        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        cmd.CommandText = "select devices.dllname, bdevices.npip,bdevices.nppassword, bdevices.ipport,bdevices.transport, bdevices.id_dev" + _
           ",bmodems.cspeed,bmodems.cphone,bmodems.cdatabit,bmodems.cstopbits,bmodems.cparity,bmodems.connectlimit from bdevices,bmodems,devices where devices.id_dev =bdevices.id_dev and bmodems.id_bd=bdevices.id_bd and bdevices.id_bd=" + id_bd.ToString + " "

        Dim cspeed As Long = 9600
        Dim cdatabit As Integer = 0
        Dim cstopbits As Integer = 0
        Dim connectlimit As Integer
        Dim cparity As String = "None"
        Dim ipport As String = ""
        Dim phone As String = ""
        Dim transport As Integer
        Dim HasRows As Boolean
        Try
            SyncLock connection
                dr = cmd.ExecuteReader()
                HasRows = dr.HasRows
                If dr.HasRows Then
                    dr.Read()
                    IPstr = dr("NPIP").ToString & ""
                    deviceid = Convert.ToInt32(dr("id_dev").ToString)
                    DrvStr = "" & dr("dllname")
                    NPPassword = dr("nppassword").ToString() & ""
                    Try
                        cspeed = dr("cspeed")
                        cdatabit = dr("cdatabit")
                        cstopbits = dr("cstopbits")
                        connectlimit = dr("connectlimit")
                    Catch ex As Exception

                    End Try
                    
                    If dr("cparity") = "N" Then
                        cparity = "None"
                    End If
                    If dr("cparity") = "E" Then
                        cparity = "Even"
                    End If
                    If dr("cparity") = "O" Then
                        cparity = "Odd"
                    End If
                    If dr("cparity") = "S" Then
                        cparity = "Space"
                    End If
                    If dr("cparity") = "M" Then
                        cparity = "Mark"
                    End If
                    ipport = dr("ipport").ToString() & ""
                    transport = dr("transport") & ""
                    phone = dr("cphone") & ""

                End If
                dr.Close()
                dr.Dispose()
                cmd.Dispose()
            End SyncLock
            If HasRows = False Then
                SaveLog(id_bd, 0, "??", 1, "Не обнаружена запись об устройстве в базе данных")
                m_ConnectStatus = "Не обнаружена запись об устройстве в базе данных"
                Return False
            End If
            'SaveLog(id_bd, 0, "??", 1, "Найдена запись об устройстве в базе данных")
            'm_ConnectStatus = "Найдена запись об устройстве в базе данных"


        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try


        If (DrvStr = "") Then
            m_ConnectStatus = "Неизвестный драйвер"
            SaveLog(id_bd, 0, "??", 1, "Неизвестный драйвер")
            Return False
        End If


        TVD = Nothing
        Try
            TVD = GetDoc(DrvStr)
        Catch
            m_ConnectStatus = "Ошибка загрузки драйвера"
            SaveLog(id_bd, 0, "??", 1, "Ошибка загрузки драйвера")
            Return False
        End Try

        If Not TVD Is Nothing Then

            SaveLog(id_bd, 0, "??", 1, "Загружен " + DrvStr)

            TVD.DB = Me

            TVD.DeviceID = id_bd



            TVD.ServerIp = IPstr
            TVD.BaudRate = cspeed 'Int(node.Attributes.GetNamedItem("BaudRate").Value)
            TVD.DataBits = cdatabit ' Int(node.Attributes.GetNamedItem("DataBits").Value)
            TVD.StopBits = cstopbits 'Int(node.Attributes.GetNamedItem("StopBits").Value)
            TVD.Parity = cparity 'node.Attributes.GetNamedItem("Parity").Value

            TVD.IPPort = ipport
            TVD.Transport = transport
            Select Case transport
                Case 0
                    'm_ConnectStatus += vbCrLf & "Транспорт: модем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: модем")
                Case 1
                    'm_ConnectStatus += vbCrLf & "Транспорт: нульмодем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: нульмодем")
                Case 2
                    'm_ConnectStatus += vbCrLf & "Транспорт: NPORT"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: NPORT")
                Case 3
                    'm_ConnectStatus += vbCrLf & "Транспорт: Vortex virtual serial"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт:  Vortex virtual serial")

                Case 4
                    'm_ConnectStatus += vbCrLf & "Транспорт: модем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: GSM модем")

                Case 5
                    'm_ConnectStatus += vbCrLf & "Транспорт: модем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: АССВ")

                Case 6
                    'm_ConnectStatus += vbCrLf & "Транспорт: модем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: ROBUSTEL")
                Case 7
                    'm_ConnectStatus += vbCrLf & "Транспорт: модем"
                    SaveLog(id_bd, 0, "??", 1, "Транспорт: ATM")

            End Select

            If transport = 0 Or transport = 4 Then
                If UsePort = "(Любой)" Then
                    If transport = 4 Then
                        TVD.ComPort = GetNextModem("G")
                        transport = 0
                        TVD.Transport = transport
                    Else
                        TVD.ComPort = GetNextModem("")
                    End If
                Else
                    TVD.ComPort = GetModemByName(UsePort)
                End If


                TVD.AtCommand = GetModemINIT()
                TVD.PhoneLineType = PhoneLineType
                TVD.Phone = phone
                TVD.ConnectLimit = connectlimit

                If TVD.ComPort = "" Then
                    m_ConnectStatus = "Все модемы заняты, либо не определены для терминала: " & Environment.MachineName
                    SaveLog(id_bd, 0, "??", 1, "Все модемы заняты, либо не определены для терминала: " & Environment.MachineName)
                    'TVD = Nothing
                    Return False
                End If



                SaveLog(id_bd, 0, "??", 1, "Выделен модем на порту: " & TVD.ComPort & " Телефон:" & TVD.Phone)
            ElseIf transport = 1 Then
                TVD.ComPort = UsePort
            ElseIf transport = 5 Or transport = 6 Or transport = 7 Then
                If Not aSocket Is Nothing Then
                    If transport = 5 Then SaveLog(id_bd, 0, "??", 1, "ASSV CALLER ID:" & aSocket.callerID)
                    If transport = 6 Then SaveLog(id_bd, 0, "??", 1, "ROBUSTEL CALLER ID:" & aSocket.callerID)
                    If transport = 7 Then SaveLog(id_bd, 0, "??", 1, "ATM CALLER ID:" & aSocket.callerID)
                End If
            Else

                SaveLog(id_bd, 0, "??", 1, "IP адрес:" & TVD.ServerIp)
                'm_ConnectStatus += vbCrLf & "IP адрес:" & TVD.ServerIp
            End If
            TVD.NPPassword = NPPassword




            ' таймаут
            TVD.TimeOut = 2000
            TVD.sleepInterval = 50


            'm_ConnectStatus += vbCrLf & "Параметры транспортного уровня загружены"


            If TVD.OpenPort(aSocket) = False Then
                Dim msg As String = ""

                msg = TVD.DriverTransport.GetError
                If TVD.Transport = 0 Then
                    PortBusy = TVD.DriverTransport.PortBusy
                    If TVD.DriverTransport.PortBusy Then
                        If msg <> "" Then
                            msg = msg + " "
                        End If
                        msg = msg + TVD.ComPort + " "
                        msg = msg + "порт занят "
                    End If
                End If

                TVD.CloseTransportConnect()
                FreeModem()
                UnLockDevice(id_bd)
                m_ConnectStatus = "Ошибка транспортa. " + msg
                SaveLog(id_bd, 0, "??", 1, "Ошибка транспорта. " + msg)
                ClearDuration()
                SaveLog(id_bd, 0, "??", 1, "Сеанс завершен")
                Return False
            End If

            SaveLog(id_bd, 0, "??", 1, "Транспортный уровень инициализирован")
            ClearDuration()

            cmd = Nothing
            DeviceReady = True


            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetEnvInfo() As String
        Dim out As String
        out = "Path:" & System.IO.Path.GetDirectoryName(Me.GetType().Assembly.Location()) + "\Config.xml"
        Try
            out = out & vbCrLf & connection.ConnectionString
        Catch
        End Try
        Return out
    End Function
    Public Function Init() As Boolean
        If Inited Then
            Return Inited
        End If

        DeviceReady = False

        Dim xml As XmlDocument
        xml = New XmlDocument
        xml.Load(System.IO.Path.GetDirectoryName(Me.GetType().Assembly.Location()) + "\Config.xml")
        Dim node As XmlElement
        node = xml.FirstChild()

        Dim builder As New OracleConnectionStringBuilder()
        Dim serviceName As String = ""
        Try
            serviceName = node.Attributes.GetNamedItem("Oracle").Value

        Catch ex As Exception
            serviceName = "//192.168.9.35:1521/ora11ape.astrum.local"
        End Try
        
            DataSourceName = node.Attributes.GetNamedItem("DataSource").Value
            Try
                m_AutoOpen = (node.Attributes.GetNamedItem("AutoOpen").Value)
            Catch ex As Exception

        End Try

        Try
            m_Status = (node.Attributes.GetNamedItem("Status").Value)
        Catch ex As Exception
            m_Status = "trial"
        End Try

        Try
            ShowHidden = (node.Attributes.GetNamedItem("ShowHidden").Value.ToLower() = "true")
        Catch
        End Try

        Try
            LogEnabled = (node.Attributes.GetNamedItem("LogEnabled").Value.ToLower() = "true")
        Catch
        End Try

            Try
            SendToTGK = (node.Attributes.GetNamedItem("SendToTGK").Value.ToLower() = "true")
        Catch
        End Try

        builder.DataSource = serviceName '  node.Attributes.GetNamedItem("DataSource").Value
        builder.UserID = node.Attributes.GetNamedItem("UserID").Value
        builder.Password = node.Attributes.GetNamedItem("Password").Value
        connection = New OracleConnection()
        connection.ConnectionString = builder.ConnectionString
        command = New OracleCommand()
        command.Connection = connection



        Try
            '' SyncLock connection
            connection.Open()
            '' End SyncLock
            If connection.State <> ConnectionState.Open Then
                Console.WriteLine("Ошибка соединения")
                Return False
            End If
            Dim SessionGlob As OracleGlobalization = connection.GetSessionInfo()
            SessionGlob.Language = "RUSSIAN"
            Try
                m_RequestInterval = Convert.ToDouble(node.Attributes.GetNamedItem("RequestInterval").Value)

            Catch
                m_RequestInterval = 59000
            End Try



            node = Nothing
            xml = Nothing
        Catch ex As Exception
            ' MsgBox(ex.Message, MsgBoxStyle.OkOnly)
            m_ConnectStatus += ex.Message
            Console.WriteLine(ex.Message)
            Return False
        End Try

        QueryExec("ALTER SESSION FORCE PARALLEL DML")


        Inited = True
        Return Inited
    End Function

    Public Sub DeviceClose()
        Try
            If Not TVD Is Nothing Then
                TVD.CloseTransportConnect()
                TVD = Nothing
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        FreeModem()

        DeviceReady = False



    End Sub

    Public Function GetToolTipStructFromId_BD(ByVal id_bd As Int32) As Object
        GetToolTipStructFromId_BD = New ToolTipStruct
        Dim dr As OracleDataReader
        Dim cmd As New OracleCommand()
        cmd.Connection = connection
        cmd.CommandText = "select bdevices.npip,bdevices.scheme_name,bbuildings.caddress," + _
           "devices.cdevdesc from bdevices,bbuildings,devices where bdevices.id_bd=" + id_bd.ToString + _
           "and bbuildings.id_bu = bdevices.id_bu and devices.id_dev=bdevices.id_dev" + " "
        Try
            SyncLock connection
                dr = cmd.ExecuteReader()
                If dr.HasRows Then
                    dr.Read()
                    GetToolTipStructFromId_BD.ip = dr("NPIP").ToString
                    GetToolTipStructFromId_BD.schemeName = dr("scheme_Name").ToString
                    GetToolTipStructFromId_BD.caddress = dr("caddress").ToString
                    GetToolTipStructFromId_BD.device = dr("cdevdesc").ToString
                End If
                dr.Close()
            End SyncLock
            Return GetToolTipStructFromId_BD
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Function
    Public Function GetConfigStructFromId_BD(ByVal id_bd As Int32) As ConfigStruct
        Dim mGetConfigStructFromId_BD
        mGetConfigStructFromId_BD = New ConfigStruct
        Dim dr As OracleDataReader
        Dim cmd As New OracleCommand()
        Dim HasRows As Boolean
        cmd.Connection = connection
        cmd.CommandText = "select bdevices.npip, bdevices.nppassword, bdevices.npquery,bdevices.hiderow,  devices.cdevname, bdevices.transport, bdevices.ipport" + _
           " from bdevices,devices where bdevices.id_bd=" + id_bd.ToString + " and devices.id_dev=bdevices.id_dev" + " "
        Try
            SyncLock connection
                dr = cmd.ExecuteReader()
                HasRows = dr.HasRows
                If HasRows Then
                    dr.Read()
                    mGetConfigStructFromId_BD.ipport = dr("IPPORT").ToString
                    If dr("transport") = 0 Then
                        mGetConfigStructFromId_BD.Transport = "MODEM"
                    End If

                    If dr("transport") = 1 Then
                        mGetConfigStructFromId_BD.Transport = "COM"
                    End If

                    If dr("transport") = 2 Then
                        mGetConfigStructFromId_BD.Transport = "NPORT"
                    End If

                    If dr("transport") = 3 Then
                        mGetConfigStructFromId_BD.Transport = "VSX"
                    End If
                    If dr("transport") = 4 Then
                        mGetConfigStructFromId_BD.Transport = "GSM Modem"
                    End If

                    If dr("transport") = 5 Then
                        mGetConfigStructFromId_BD.Transport = "ASSV"
                    End If


                    If dr("transport") = 6 Then
                        mGetConfigStructFromId_BD.Transport = "ROBUSTEL"
                    End If


                    If dr("transport") = 7 Then
                        mGetConfigStructFromId_BD.Transport = "ATM"
                    End If


                    mGetConfigStructFromId_BD.IP = dr("NPIP").ToString
                    mGetConfigStructFromId_BD.device = dr("cdevname").ToString
                    mGetConfigStructFromId_BD.Password = dr("nppassword").ToString
                    mGetConfigStructFromId_BD.NPQUERY = dr("npquery").ToString
                    mGetConfigStructFromId_BD.HideRow = dr("hiderow").ToString
                End If
                dr.Close()

            End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
        If HasRows Then
            Return mGetConfigStructFromId_BD
        Else
            Return Nothing
        End If

    End Function
    'Public Sub setConfigToDB(ByVal id_bd As Int32, ByVal ip As String, ByVal password As String, ByVal device As String, ByVal NPQuery As Boolean, ByVal HideRow As Boolean, ByVal Transport As String, ByVal IPPort As String)
    '    Dim cmd As New OracleCommand()
    '    cmd.Connection = connection
    '    Dim i As Integer
    '    Dim trId As Integer
    '    If NPQuery Then
    '        i = 1
    '    Else
    '        i = 0
    '    End If
    '    Dim j As Integer
    '    If HideRow Then
    '        j = 1
    '    Else
    '        j = 0
    '    End If

    '    If Transport = "MODEM" Then
    '        trId = 0
    '    End If

    '    If Transport = "COM" Then
    '        trId = 1
    '    End If

    '    If Transport = "NPORT" Then
    '        trId = 2
    '    End If

    '    If Transport = "VSX" Then
    '        trId = 3
    '    End If

    '    If Transport = "GSM Modem" Then
    '        trId = 4
    '    End If

    '    If Transport = "ASSV" Then
    '        trId = 5
    '    End If


    '    cmd.CommandText = "update bdevices set bdevices.hiderow=" & j.ToString & ", bdevices.npquery=" & i.ToString & ", bdevices.nppassword='" + password + "'" + _
    '    ", bdevices.npip='" + ip + "', bdevices.ipport='" + IPPort + "', bdevices.transport=" + trId.ToString + ", bdevices.id_dev=(select devices.id_dev from devices where devices.cdevname='" + device + _
    '    "') where bdevices.id_bd=" + id_bd.ToString + " "


    '    Try
    '        '' SyncLock connection
    '        cmd.ExecuteNonQuery()
    '        '' End SyncLock
    '    Catch ex As Exception
    '        Console.WriteLine(ex.Message)
    '    End Try
    'End Sub
    Public Sub ClearDBarch(ByVal after As Date, ByVal befor As Date, ByVal archtype As Short, ByVal id_bd As String)
        Dim cmd As New OracleCommand()
        after = after.AddSeconds(-1)
        befor = befor.AddSeconds(1)
        cmd.Connection = connection
        cmd.CommandText = "delete from " & DBTableName & " where dcounter>=" + _
        "to_date('" + after.Year.ToString() + "-" + after.Month.ToString() + "-" + after.Day.ToString() + _
        " " + after.Hour.ToString() + ":" + after.Minute.ToString() + ":" + after.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS') and dcounter<=" + _
        "to_date('" + befor.Year.ToString() + "-" + befor.Month.ToString() + "-" + befor.Day.ToString() + _
        " " + befor.Hour.ToString() + ":" + befor.Minute.ToString() + ":" + befor.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS') and id_ptype=" + archtype.ToString() + _
        "and id_bd=" + id_bd.ToString()
        Try
            '' SyncLock connection
            cmd.ExecuteNonQuery()
            '' End SyncLock
            cmd = Nothing
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub
    Protected Function GetDoc(ByVal name As String, Optional ByVal Root As String = "") As TVDriver
        Dim funcAssembly As TVDriver
        Dim asm As System.Reflection.Assembly
        funcAssembly = Nothing
        asm = Nothing
        Try
            If asm Is Nothing Then
                If Root <> "" Then
                    Try
                        asm = System.Reflection.Assembly.LoadFrom(Root + "\" & name & ".dll")
                    Catch
                    End Try
                End If
            End If
            If asm Is Nothing Then
                Dim FileName As String
                FileName = System.IO.Path.GetDirectoryName(Me.GetType().Assembly.Location) + "\" & name & ".dll"
                Try
                    If (File.Exists(FileName)) Then
                        asm = System.Reflection.Assembly.LoadFrom(FileName)
                    End If
                Catch ex As Exception
                    Console.WriteLine(ex.Message)
                End Try
                If (asm Is Nothing) Then
                    Try
                        FileName = AppDomain.CurrentDomain.DynamicDirectory + "\" & name & ".dll"
                        If (File.Exists(FileName)) Then
                            asm = System.Reflection.Assembly.LoadFrom(FileName)
                        Else
                            Try
                                funcAssembly = CType(System.Activator.CreateInstance(name, name & ".Driver").Unwrap(), TVDriver)
                            Catch e2 As System.Exception
                                Dim i As Integer = 0
                                Return Nothing
                            End Try
                        End If
                    Catch ex As Exception
                        Console.WriteLine(ex.Message)
                    End Try
                End If
                If (funcAssembly Is Nothing) Then
                    funcAssembly = CType(asm.CreateInstance(name & ".Driver", True), TVDriver)
                End If
            End If
        Catch
        End Try
        asm = Nothing
        Return funcAssembly
    End Function
    Public Sub CloseTransportConnect()

        Try
            If DeviceReady Then
                TVD.CloseTransportConnect()
                DeviceReady = False
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function write(ByRef buf() As Byte, ByVal len As Long)
        Dim ret As Long
        If Not TVD Is Nothing Then
            ret = TVD.write(buf, len)
            Return ret
        Else
            Return 0
        End If
    End Function
    Public Sub connect()
        If Not TVD Is Nothing Then
            TVD.Connect()
            If Not TVD.IsConnected Then
                m_ConnectStatus = "Ошибка протокола обмена. " + TVD.DriverTransport.GetError
                TVD.CloseTransportConnect()
                UnLockDevice(TVD.DeviceID)
            End If
        End If

    End Sub

    Public Function isConnected() As Boolean
        If Not TVD Is Nothing Then
            Return TVD.IsConnected
        Else
            Return False
        End If
    End Function

    Public Function readmarch() As String
        If Not TVD Is Nothing Then
            HoldLine()
            Return TVD.ReadMArch()
        Else
            m_ConnectStatus = "Драйвер устройства не создан"
            Return "Ошибка. Драйвер устройства не создан"
        End If
    End Function
    Public Function readtarch() As String
        If Not TVD Is Nothing Then
            HoldLine()
            Return TVD.ReadTArch()
        Else
            m_ConnectStatus = "Драйвер устройства не создан"
            Return "Ошибка. Драйвер устройства не создан"
        End If
    End Function
    Public Function readarch(ByVal ArchType As Int32, ByVal ArchYear As Int32, _
   ByVal ArchMonth As Int32, ByVal ArchDay As Int32, ByVal ArchHour As Int32) As String
        HoldLine()
        Return TVD.ReadArch(ArchType, ArchYear, ArchMonth, ArchDay, ArchHour)
    End Function
    Public Function bufcheck() As String
        Dim retstr As String = ""
        If Not TVD Is Nothing Then
            'retstr = TVD.bufcheck()
            If (TVD.isArchToDBWrite = True) Then
                Return WriteArchToDB()
                TVD.isArchToDBWrite = False
            End If
            If (TVD.isMArchToDBWrite = True) Then
                Return WritemArchToDB()
                TVD.isMArchToDBWrite = False
            End If
            If (TVD.isTArchToDBWrite = True) Then
                Return WriteTArchToDB()
                TVD.isTArchToDBWrite = False
            End If
            Return retstr
        Else
            m_ConnectStatus = "Драйвер устройства не создан"
            Return "Ошибка. Драйвер устройства не создан"
        End If


    End Function


    Public Sub SetTimeToPlanCall(ByVal id_bd As String, ByVal FieldName As String, ByVal time As DateTime)
        command.CommandText = "update PlanCall set " + FieldName + _
        "=" + "to_date('" + time.Year.ToString() + "-" + _
        time.Month.ToString() + "-" + time.Day.ToString() + " " + _
        time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString() + _
        "','YYYY-MM-DD HH24:MI:SS') where id_bd=" + id_bd.ToString
        Try
            '' SyncLock connection
            If connection.State = ConnectionState.Open Then
                command.ExecuteNonQuery()
            End If
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub SetNCALLToPlanCall(ByVal id_bd As String, ByVal NCALL As Integer)
        command.CommandText = "update PlanCall set NCALL=" + NCALL.ToString + _
        " where id_bd=" + id_bd.ToString
        Try
            '' SyncLock connection
            If connection.State = ConnectionState.Open Then
                command.ExecuteNonQuery()
            End If
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub AddHourToPlanCall(ByVal id_bd As String, ByVal FieldName As String, ByVal Hours As Integer)
        command.CommandText = "update PlanCall set " + FieldName + _
        "=" + FieldName + " + 1/24 * " + Hours.ToString() + " where id_bd=" + id_bd.ToString
        Try
            '' SyncLock connection
            If connection.State = ConnectionState.Open Then
                command.ExecuteNonQuery()
            End If
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub AddMinutesToPlanCall(ByVal id_bd As String, ByVal FieldName As String, ByVal Minutes As Integer)
        command.CommandText = "update PlanCall set " + FieldName + _
        "=" + FieldName + " + 1/24/60 * " + Minutes.ToString() + " where id_bd=" + id_bd.ToString
        Try
            '' SyncLock connection
            If connection.State = ConnectionState.Open Then
                command.ExecuteNonQuery()
            End If
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub

    Public Sub AddDaysToPlanCall(ByVal id_bd As String, ByVal FieldName As String, ByVal Days As Integer)
        command.CommandText = "update PlanCall set " + FieldName + _
        "=" + FieldName + " + " + Days.ToString() + " where id_bd=" + id_bd.ToString
        Try
            '' SyncLock connection
            If connection.State = ConnectionState.Open Then
                command.ExecuteNonQuery()
            End If
            '' End SyncLock
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
        End Try
    End Sub
    Public Function WriteArchToDB() As String
        command.CommandText = TVD.WriteArchToDB
        Dim ret As String
        ret = command.CommandText & " "
        Try
            '' SyncLock connection
            command.ExecuteNonQuery()
            '' End SyncLock
            TVD.isArchToDBWrite = False
            Return ret & " Архив добавлен в БД"
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
            Return ret & ex.Message
        End Try
    End Function


    Public Function WritemArchToDB() As String
        command.CommandText = TVD.WriteMArchToDB
        Dim ret As String
        ret = command.CommandText & " "

        Try
            TVD.isMArchToDBWrite = False
            '' SyncLock connection
            command.ExecuteNonQuery()
            '' End SyncLock
            Return ret & "Мгновенный архив добавлен в БД"
        Catch ex As Exception
            Debug.Print(ex.Message)
            Console.WriteLine(ex.Message)
            Return ret & ex.Message
        End Try
    End Function

    Public Function WriteTArchToDB() As String
        command.CommandText = TVD.WriteTArchToDB
        Dim ret As String
        ret = command.CommandText & " "

        Try
            TVD.isTArchToDBWrite = False
            '' SyncLock connection
            command.ExecuteNonQuery()
            '' End SyncLock
            Return ret & "Тотальный архив добавлен в БД"
        Catch ex As Exception
            Console.WriteLine(ex.Message)
            Debug.Print(ex.Message)
            Return ret & ex.Message
        End Try
    End Function

    Private Function GetLastMomentArchive(ByVal id_bd As Integer) As DataRow
        Dim dt As DataTable
        Dim dcall As Date

        dt = QuerySelect("select max(dcall) dcall from datacurr where id_bd=" + id_bd.ToString + "  and id_ptype=1")
        If dt.Rows.Count > 0 Then
            Try
                dcall = dt.Rows(0)("dcall")
            Catch ex As Exception
                Return Nothing
            End Try
        Else
            Return Nothing
        End If
        dt = QuerySelect("select * from datacurr where id_bd=" + id_bd.ToString + " and id_ptype=1 and dcall>=" & OracleDate(dcall))
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0)
        Else
            Return Nothing
        End If
    End Function

    'Public Function WriteErrToDB(ByVal DeviceID As Integer, ByVal ErrDate As Date, ByVal ErrMsg As String) As String
    '    Dim SSS As String

    '    Dim dr As DataRow
    '    Dim useIns As Boolean = True
    '    Dim DCOUNTER As Date

    '    dr = GetLastMomentArchive(DeviceID.ToString)
    '    If Not dr Is Nothing Then
    '        If dr("hc").ToString().Contains(ErrMsg) Then
    '            DCOUNTER = dr("DCOUNTER")
    '            If Math.Abs(DateDiff(Microsoft.VisualBasic.DateInterval.Minute, ErrDate, DCOUNTER)) > 59 Then
    '                useIns = True
    '            Else
    '                useIns = False
    '            End If
    '        End If
    '    End If

    '    If Not useIns Then
    '        Dim hc As String
    '        Dim passIdx As Integer
    '        hc = dr("hc").ToString()
    '        passIdx = hc.IndexOf(". Попытка № ")
    '        If passIdx > 0 Then
    '            If Not Integer.TryParse(hc.Substring(passIdx + 12).Trim(), passIdx) Then
    '                passIdx = 2
    '            Else
    '                passIdx += 1
    '            End If
    '        Else
    '            passIdx = 2
    '        End If

    '        SSS = "update datacurr set HC ='" + ErrMsg + ". Попытка № " + passIdx.ToString + "' ,dcall=sysdate  where id_bd=" + DeviceID.ToString + " and id_ptype=1 and dcall>=" & OracleDate(dr("dcall"))
    '        command.CommandText = SSS
    '        Dim ret As String
    '        ret = command.CommandText & " "
    '        Try
    '            '' SyncLock connection
    '            command.ExecuteNonQuery()
    '            '' End SyncLock
    '            TVD.isArchToDBWrite = False
    '            Return ret
    '        Catch ex As Exception
    '            Console.WriteLine(ex.Message)
    '            Debug.Print(ex.Message)
    '            Return ret & ex.Message
    '        End Try
    '    Else
    '        SSS = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,id_ptype,hc,hc_1) values ("
    '        SSS = SSS + DeviceID.ToString() + ","
    '        SSS = SSS + "SYSDATE" + ","
    '        SSS = SSS + OracleDate(ErrDate) + ","
    '        SSS = SSS + "1,"
    '        SSS = SSS + "'" & S180(ErrMsg) & "',"
    '        SSS = SSS + "'" & S180(ErrMsg) & "')"

    '        command.CommandText = SSS
    '        Dim ret As String
    '        ret = command.CommandText & " "
    '        Try
    '            '' SyncLock connection
    '            command.ExecuteNonQuery()
    '            '' End SyncLock
    '            TVD.isArchToDBWrite = False
    '            Return ret
    '        Catch ex As Exception
    '            Console.WriteLine(ex.Message)
    '            Debug.Print(ex.Message)
    '            Return ret & ex.Message
    '        End Try
    '    End If


    'End Function


    Public Function WriteErrToDB(ByVal DeviceID As Integer, ByVal ErrDate As Date, ByVal ErrMsg As String) As String
        Dim SSS As String

        Dim dr As DataRow
        Dim useIns As Boolean = True
        Dim DCALL As Date

        dr = GetLastMomentArchive(DeviceID.ToString)
        If Not dr Is Nothing Then
            If dr("hc").ToString().Contains(ErrMsg) Then
                DCALL = dr("DCALL")
                If Math.Abs(DateDiff(Microsoft.VisualBasic.DateInterval.Minute, ErrDate, DCALL)) > 59 Then
                    useIns = True
                Else
                    useIns = False
                End If
            End If
        End If

        If Not useIns Then
            Dim hc As String
            Dim passIdx As Integer
            hc = dr("hc").ToString()
            passIdx = hc.IndexOf(". Попытка № ")
            If passIdx > 0 Then
                If Not Integer.TryParse(hc.Substring(passIdx + 12).Trim(), passIdx) Then
                    passIdx = 2
                Else
                    passIdx += 1
                End If
            Else
                passIdx = 2
            End If

            SSS = "update datacurr set HC ='" + ErrMsg + ". Попытка № " + passIdx.ToString + "' ,DCOUNTER=sysdate  where id_bd=" + DeviceID.ToString + " and id_ptype=1 and DCOUNTER>=" & OracleDate(dr("DCOUNTER"))
            command.CommandText = SSS
            Dim ret As String
            ret = command.CommandText & " "
            Try
                '' SyncLock connection
                command.ExecuteNonQuery()
                '' End SyncLock
                TVD.isArchToDBWrite = False
                Return ret
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Debug.Print(ex.Message)
                Return ret & ex.Message
            End Try
        Else
            SSS = "INSERT INTO " & DBTableName & "(id_bd,DCOUNTER,DCALL,id_ptype,hc,hc_1) values ("
            SSS = SSS + DeviceID.ToString() + ","
            SSS = SSS + "SYSDATE" + ","
            SSS = SSS + OracleDate(ErrDate) + ","
            SSS = SSS + "1,"
            SSS = SSS + "'" & S180(ErrMsg) & "',"
            SSS = SSS + "'" & S180(ErrMsg) & "')"

            command.CommandText = SSS
            Dim ret As String
            ret = command.CommandText & " "
            Try
                '' SyncLock connection
                command.ExecuteNonQuery()
                '' End SyncLock
                TVD.isArchToDBWrite = False
                Return ret
            Catch ex As Exception
                Console.WriteLine(ex.Message)
                Debug.Print(ex.Message)
                Return ret & ex.Message
            End Try
        End If


    End Function

    Private Const selectCommandTemplate As String = "SELECT * FROM ({0}) {1}  "

    Public Function GetTable(ByVal tableName As String, Optional ByVal subquery As String = "") As DataTable
        Try
            Dim selectCommandText As String = String.Format(selectCommandTemplate, tableName, subquery)
            Dim dataAdapter As OracleDataAdapter = New OracleDataAdapter(selectCommandText, Me.dbconnect)
            Dim table As DataTable = New DataTable(tableName)
            dataAdapter.Fill(table)
            Return table
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Return Nothing
    End Function


    Public Function GetDeviceSet() As DataSet
        Dim ds As DataSet
        ds = New DataSet


        ds.Tables.Add(GetTable("v_GRP2"))
        ds.Tables.Add(GetTable("v_DEV2"))


        ' Create relationships between the tables. 
        ds.Relations.Add("g2bld_fk", ds.Tables("v_GRP2").Columns("id_grp"), ds.Tables("v_DEV2").Columns("id_grp"), False)
        'ds.Relations.Add("bld2dev_fk", ds.Tables("bbuildings").Columns("id_bu"), ds.Tables("bdevices").Columns("id_bu"), False)
        Return ds

    End Function

    Private Sub TVD_Idle() Handles TVD.Idle
        RaiseEvent Idle()
    End Sub

    Private Sub TVD_TransportStatus(ByVal Action As UnitransportAction, ByVal MSG As String) Handles TVD.TransportStatus
        Select Case Action
            Case STKTVMain.UnitransportAction.Connected

            Case STKTVMain.UnitransportAction.Connecting

            Case STKTVMain.UnitransportAction.Destroy

            Case STKTVMain.UnitransportAction.Disconnected

            Case STKTVMain.UnitransportAction.Disconnecting

            Case STKTVMain.UnitransportAction.ReceiveData

            Case STKTVMain.UnitransportAction.SendData

            Case STKTVMain.UnitransportAction.SettingUp

            Case STKTVMain.UnitransportAction.Wait

            Case STKTVMain.UnitransportAction.LowLevelStop
                SaveLog(TVD.DeviceID, 0, "0", 0, TVD.ComPort + "->" + MSG)

        End Select
        RaiseEvent TransportStatus(Action, MSG)
    End Sub

    Public Function QueryExec(ByVal s As String) As Boolean

        Dim cmd As OracleCommand
        cmd = New OracleCommand
        'LOG(s)
        Try

            cmd.CommandType = CommandType.Text
            cmd.CommandText = s
            cmd.Connection = dbconnect()
            Dim t As DateTime
            t = Now
            cmd.ExecuteNonQuery()

        Catch ex As Exception
            LOG(0, s + " err: " + ex.Message)
            Try
                cmd.Dispose()
            Catch ex1 As Exception

            End Try

            Return False
        End Try
        Try
            cmd.Dispose()
        Catch ex1 As Exception

        End Try
        Return True
    End Function

    Public Function QuerySelect(ByVal s As String) As DataTable
        Dim cmd As OracleCommand
        cmd = New OracleCommand
        cmd.CommandType = CommandType.Text
        cmd.CommandText = s
        cmd.Connection = dbconnect()
        Dim t As DateTime
        t = Now
        Dim dt As DataTable
        Dim da As OracleDataAdapter
        dt = New DataTable
        da = New OracleDataAdapter
        da.SelectCommand = cmd
        'LOG(s)
        Try
            da.Fill(dt)
        Catch ex As Exception
            LOG(0, s + " Err:" + ex.Message)
        End Try
        Try
            da.Dispose()
        Catch ex As Exception

        End Try
        Try
            cmd.Dispose()
        Catch ex As Exception

        End Try


        Return dt
    End Function

    Private Function TryGetPort(ByVal s As String) As Boolean
        Dim Port As System.IO.Ports.SerialPort
        Port = New System.IO.Ports.SerialPort
        Port.PortName = s
        Try
            Port.Open()
        Catch ex As Exception
            Logger.Error(ex)
            Return False
        End Try
        If Port.IsOpen Then
            Try
                Port.Close()
            Catch ex As Exception
                Logger.Error(ex)
                Return False
            End Try
            Return True
        Else
            Try
                Port.Close()
            Catch ex As Exception
                Logger.Error(ex)
            End Try
            Return False
        End If



    End Function

    Public Function GetNextModem(ByVal TYPECALL As String) As String

        Dim query As String
        Dim sPort As String
        Dim MyName As String
        MyName = Environment.MachineName

        If TYPECALL.ToUpper = "G" Then
            query = "select ID_CP,CIPADDR,TERMINAL,MACHINE,COMPORT,CTYPECALL ,useduntil from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where CTYPECALL='G' and  ( UsedUntil is null or UsedUntil < sysdate) and TERMINAL='" & MyName & "'" + "  order by UsedUntil"
        Else
            query = "select ID_CP,CIPADDR,TERMINAL,MACHINE,COMPORT,CTYPECALL ,useduntil from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where CTYPECALL<>'G' and  ( UsedUntil is null or UsedUntil < sysdate) and TERMINAL='" & MyName & "'" + "  order by UsedUntil"
        End If


        Dim dt As DataTable
        dt = QuerySelect(query)
        If dt.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                sPort = dt.Rows(i)("COMPORT")
                Mymodem = dt.Rows(i)("ID_CP")
                PhoneLineType = dt.Rows(i)("CTYPECALL")
                If TryGetPort(sPort) Then
                    HoldLine(20)
                    Return sPort
                End If
            Next
            Mymodem = 0
            sPort = ""
            Return ""
        Else
            Mymodem = 0
            sPort = ""
            Return ""
        End If

    End Function


    Private Function GetModemByName(ByVal Name As String) As String

        Dim query As String
        Dim sPort As String
        Dim MyName As String
        MyName = Environment.MachineName


        query = "select ID_CP,CIPADDR,TERMINAL,MACHINE,COMPORT,CTYPECALL ,useduntil from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where ( UsedUntil is null or UsedUntil < sysdate) and TERMINAL='" & MyName & "' and COMPORT='" + Name + "'  order by COMPORT"
        Dim dt As DataTable
        dt = QuerySelect(query)
        If dt.Rows.Count > 0 Then
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                sPort = dt.Rows(i)("COMPORT")
                Mymodem = dt.Rows(i)("ID_CP")
                PhoneLineType = dt.Rows(i)("CTYPECALL")
                If TryGetPort(sPort) Then
                    HoldLine(20)
                    Return sPort
                End If
            Next
            Mymodem = 0
            sPort = ""
            Return ""
        Else
            Mymodem = 0
            sPort = ""
            Return ""
        End If

    End Function


    Public Function GetStationModemCount() As Integer

        Dim query As String
        Dim MyName As String
        MyName = Environment.MachineName

        query = "select count(*) cnt from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where  TERMINAL='" & MyName & "'" + "  "
        Dim dt As DataTable
        dt = QuerySelect(query)
        If dt.Rows.Count > 0 Then
            Return dt.Rows(0)("cnt")
        Else
            Return 0
        End If

    End Function


    Public Function GetFreeModems() As DataTable

        Dim query As String
        Dim MyName As String
        MyName = Environment.MachineName

        query = "select ID_CP,CIPADDR,TERMINAL,MACHINE,COMPORT,CTYPECALL ,useduntil from ipaddr join comports on ipaddr.id_ip=comports.id_ip  where ( UsedUntil is null or UsedUntil < sysdate) and TERMINAL='" & MyName & "'" + " order by COMPORT  "
        Dim dt As DataTable
        dt = QuerySelect(query)
        If dt.Rows.Count > 0 Then
            Return dt
        Else
            Return Nothing
        End If

    End Function

    Public Function GetModemINIT() As String
        Dim MyName As String
        MyName = Environment.MachineName
        Dim query As String
        Dim dt As DataTable
        Try
            query = "select cinit from connections join comports on connections.id_cp=comports.id_cp left join modems on connections.id_modem = modems.id_modem join  ipaddr on comports.id_ip =ipaddr.id_ip where comports.id_cp=" & Mymodem.ToString & " and terminal='" & MyName & "'"
            dt = QuerySelect(query)
            If dt.Rows.Count > 0 Then
                Return dt.Rows(0)("cinit")
            End If
        Catch ex As Exception
            Logger.Error(ex)
            Return ""
        End Try

        Return ""
    End Function

    Public Sub FreeModem()
        Dim query As String
        If Mymodem > 0 Then
            query = "update comports set useduntil=sysdate where id_cp=" & Mymodem.ToString
            QueryExec(query)

        End If
        Mymodem = -1

    End Sub

    Public Sub HoldLine(Optional ByVal Minutes As Integer = 3)
        Dim query As String
        If Mymodem > 0 Then
            query = "update comports set useduntil=sysdate+" + Minutes.ToString + "/60/24 where id_cp=" & Mymodem.ToString
            QueryExec(query)
        End If
    End Sub

    Private LastLog As DateTime
    Public Sub ClearDuration()
        LastLog = DateTime.Now
    End Sub

    Private Function S180(ByVal s As String) As String

        Dim outs As String
        outs = s
        If outs.Length <= 180 Then
            Return outs
        End If
        outs = outs.Substring(0, 180)
        Return outs
    End Function

    Public Sub SaveLog(ByVal id_bd As Integer, ByVal id_ptype As Integer, ByVal cport As String, ByVal cEXAMINE As Integer, ByVal cresult As String)
        Dim query As String
        Dim duration As Integer = 0
        Try
            duration = DateDiff(DateInterval.Second, LastLog, DateTime.Now)
        Catch ex As Exception
            Logger.Error(ex)
        End Try

        LastLog = DateTime.Now
        'query = "insert into logcall( ID_BD,  ID_PTYPE ,  CPORT , NSESSION,  DBEG,  DURATION,  CEXAMINE ,  CRESULT) values(" & _
        'id_bd.ToString() & "," & id_ptype.ToString() & ",'" & cport & "',0,SYSTIMESTAMP," & duration.ToString() & "," & cEXAMINE.ToString & ",'" & S180(cresult) & "')"
        query = "insert into logcall( ID_BD,  ID_PTYPE ,  CPORT , NSESSION,  DBEG,  DURATION,  CEXAMINE ,  CRESULT) values(" &
        id_bd.ToString() & "," & id_ptype.ToString() & ",'" & cport & "',0," + OracleTimeStamp(LastLog) + "," & duration.ToString() & "," & cEXAMINE.ToString & ",'" & S180(Environment.MachineName + ": " +
         cresult) & "')"

        LOG(id_bd, cresult)
        QueryExec(query)

    End Sub


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Try
            If Not command Is Nothing Then
                command.Dispose()

            End If

            If Not connection Is Nothing Then
                connection.Close()
                connection.Dispose()

            End If
            
        Catch ex As Exception
            Logger.Error(ex)
        End Try
        connection = Nothing
        command = Nothing
    End Sub


    Public Sub New()
        NLog.GlobalDiagnosticsContext.Set("counter", "_system")
        Inited = Init()
    End Sub


End Class
