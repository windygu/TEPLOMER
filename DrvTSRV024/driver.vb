﻿
Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports STKTVMain
Imports System.IO
Imports System.Threading

Public Structure MArchive
    Public DateArch As DateTime
    Public HC As Int32
    Public MsgHC As String

    Public HCtv1 As Long
    Public MsgHCtv1 As String

    Public HCtv2 As Long
    Public MsgHCtv2 As String

    Public G1 As Single
    Public G2 As Single
    Public G3 As Single
    Public G4 As Single
    Public G5 As Single
    Public G6 As Single

    Public t1 As Single
    Public t2 As Single
    Public t3 As Single
    Public t4 As Single
    Public t5 As Single
    Public t6 As Single

    Public p1 As Single
    Public p2 As Single
    Public p3 As Single
    Public p4 As Single
    Public p5 As Single
    Public p6 As Single

    Public m1 As Single
    Public m2 As Single
    Public m3 As Single
    Public m4 As Single
    Public m5 As Single
    Public m6 As Single

    Public v1 As Single
    Public v2 As Single
    Public v3 As Single
    Public v4 As Single
    Public v5 As Single
    Public v6 As Single
    
    Public dt12 As Single
    Public dt45 As Single

    Public tx1 As Single
    Public tx2 As Single

    Public tair1 As Single
    Public tair2 As Single

    Public SP As Long
    Public SPtv1 As Long
    Public SPtv2 As Long

    Public dQ1 As Single
    Public dQ2 As Single

   
    Public archType As Short
End Structure

Public Structure Archive
    Public DateArch As DateTime

    Public oktime As Long
    Public Errtime As Long
    Public oktime2 As Long
    Public Errtime2 As Long
    Public ErrtimeH As Long
    Public HC As Long
    Public MsgHC As String

    Public HCtv1 As Long
    Public MsgHCtv1 As String

    Public HCtv2 As Long
    Public MsgHCtv2 As String

    Public Tw1 As Single
    Public Tw2 As Single

    Public P1 As Single
    Public T1 As Single
    Public M2 As Single
    Public V1 As Single

    Public P2 As Single
    Public T2 As Single
    Public M3 As Single
    Public V2 As Single

    Public V3 As Single
    Public M1 As Single

    Public Q1 As Single
    Public Q2 As Single
    Public Q3 As Single
    Public Q4 As Single
    Public Q5 As Single
    Public Q6 As Single


    Public QG1 As Single
    Public QG2 As Single

    Public SP As Long
    Public SPtv1 As Long
    Public SPtv2 As Long

    Public tx1 As Long
    Public tx2 As Long
    Public tair1 As Long
    Public tair2 As Long

    Public T3 As Single
    Public T4 As Single
    Public T5 As Single
    Public T6 As Single

    Public P3 As Single
    Public P4 As Single
    Public P5 As Single
    Public P6 As Single

    Public v4 As Single
    Public v5 As Single
    Public v6 As Single
    Public M4 As Single
    Public M5 As Single
    Public M6 As Single
    Public V1h As Double
    Public V2h As Double
    Public V3h As Double
    Public V4h As Double
    Public Q1H As Double
    Public Q2H As Double

    Public archType As Short
End Structure

Public Structure TArchive
    Public DateArch As DateTime


    Public V1 As Double
    Public V2 As Double
    Public V3 As Double
    Public V4 As Double
    Public V5 As Double
    Public V6 As Double

    Public M1 As Double
    Public M2 As Double
    Public M3 As Double
    Public M4 As Double
    Public M5 As Double
    Public M6 As Double
    Public Q1 As Double
    Public Q2 As Double

    Public TW1 As Double
    Public TW2 As Double
    Public Q3 As Double
    Public Q4 As Double
    Public Q5 As Double
    Public Q6 As Double

    Public HC As Int32
    Public Errtime As Long
    Public oktime As Long
    Public Errtime2 As Long
    Public oktime2 As Long

    Public archType As Short
End Structure


'Смещение	Размер поля (в байтах)	Название	Тип значения	Диапазон	Единицы измерения	Примечание
'0х00	1	День	Беззнаковое целое	1 – 31	-	-
'0х01	1	Месяц	Беззнаковое целое	1 – 12	-	-
'0х02	1	Год	Беззнаковое целое	0 – 99	-	-
'0х03	1	Час	Беззнаковое целое	0 – 23	час	-
'0х04	4	Тепло по 1 теплосистеме	32-битный IEEE-754 формат	0 – 999999999	МДж	Накопительный счетчик
'0х08	4	Тепло по 2 теплосистеме	32-битный IEEE-754 формат	0 – 999999999	МДж	Накопительный счетчик
'0х0С	4	Общий расход по 1 трубопроводу	32-битный IEEE-754 формат	0 – 999999	В установленных единицах (т, м3)	Накопительный счетчик
'0х10	4	Общий расход по 2 трубопроводу	32-битный IEEE-754 формат	0 – 999999	В установленных единицах (т, м3)	Накопительный счетчик
'0х14	4	Общий расход по 3 трубопроводу	32-битный IEEE-754 формат	0 – 999999	В установленных единицах (т, м3)	Накопительный счетчик
'0х18	4	Общий расход по 4 трубопроводу	32-битный IEEE-754 формат	0 – 999999	В установленных единицах (т, м3)	Накопительный счетчик
'0х1С	2	Температура по 1 трубопроводу	Беззнаковое целое	0 – 25000	10-2 ºС	Среднее значение за 1 час
'0х1E	2	Температура по 3 трубопроводу	Беззнаковое целое	0 – 25000	10-2 ºС	Среднее значение за 1 час
'0х20	2	Температура по 2 трубопроводу	Беззнаковое целое	0 – 25000	10-2 ºС	Среднее значение за 1 час
'0х22	2	Температура по 4 трубопроводу	Беззнаковое целое	0 – 25000	10-2 ºС	Среднее значение за 1 час
'0х24	2	Слово состояния	Беззнаковое целое	0 – 65535	-	Смотри таблицу 4
'0х26	4	Аварийное время	Беззнаковое целое	0 – 999999999	мин	Накопительный счетчик

Public Structure HArch
    Public day As Byte
    Public month As Byte
    Public year As Byte
    Public hour As Byte
    Public Q1 As Single
    Public Q2 As Single
    Public V1 As Single
    Public V2 As Single
    Public V3 As Single
    Public V4 As Single
    Public T1 As UInteger
    Public T2 As UInteger
    Public T3 As UInteger
    Public T4 As UInteger
    Public Status As UInteger
    Public CrashTime As UInteger
    Public P1 As Single
    Public P2 As Single
    Public P3 As Single
    Public P4 As Single
    Public OKTime As UInteger
    Public OK As Boolean
    Public V1h As Single
    Public V2h As Single
    Public V3h As Single
    Public V4h As Single
    Public Q1H As Single
    Public Q2H As Single

End Structure


Public Structure DArch
    Public day As Byte
    Public month As Byte
    Public year As Byte
    Public hour As Byte
    Public Q1 As Single
    Public Q2 As Single
    Public V1 As Single
    Public V2 As Single
    Public V3 As Single
    Public V4 As Single
    Public T1 As UInteger
    Public T2 As UInteger
    Public T3 As UInteger
    Public T4 As UInteger
    Public CrashTime As UInteger
    Public P1 As Single
    Public P2 As Single
    Public P3 As Single
    Public P4 As Single
    Public OKTime As UInteger
    Public OK As Boolean
    Public V1h As Single
    Public V2h As Single
    Public V3h As Single
    Public V4h As Single
    Public Q1H As Single
    Public Q2H As Single
End Structure

'Public Class DataPass
'    Public passdata(100) As Byte
'    Public bDup As Boolean
'    Public Cnt As Int16
'    Public Size As Long

'    Function GetHash() As String
'        ' Create a new instance of the MD5 object.
'        Dim md5Hasher As MD5 = MD5.Create()

'        ' Convert the input string to a byte array and compute the hash.
'        Dim data As Byte() = md5Hasher.ComputeHash(passdata)

'        ' Create a new Stringbuilder to collect the bytes
'        ' and create a string.
'        Dim sBuilder As New StringBuilder()

'        ' Loop through each byte of the hashed data 
'        ' and NanFormat each one as a hexadecimal string.
'        Dim i As Integer
'        For i = 0 To data.Length - 1
'            sBuilder.Append(data(i).ToString("x2"))
'        Next i

'        ' Return the hexadecimal string.
'        Return sBuilder.ToString()

'    End Function


'    ' Verify a hash against a string.
'    Function VerifyHash(ByVal hash As String) As Boolean
'        ' Hash the input.
'        Dim hashOfInput As String = GetHash()

'        ' Create a StringComparer an comare the hashes.
'        Dim comparer As StringComparer = StringComparer.OrdinalIgnoreCase

'        If 0 = comparer.Compare(hashOfInput, hash) Then
'            Return True
'        Else
'            Return False
'        End If

'    End Function

'End Class

Public Class driver
    Inherits STKTVMain.TVDriver

    Public Const c_lng256 As Long = 256&
    Private SequenceErrorCount As Integer = 0
    Private mIsConnected As Boolean
    Public TSRV24M As Boolean = False
    Public TSRV24MP As Boolean = False

    'Private MyManager As LATIR.Manager
    Private isTCP As Boolean
    Private SleepTime As Long

    'Public PT(3) As Double
    'Public MV(3) As Integer



    Dim tArch As TArchive
    Dim IsTArchToRead As Boolean = False
    ' Dim WithEvents tim As System.Timers.Timer

    Dim tv As Short

    Dim archType_hour = 3
    Dim archType_day = 4


    Dim Arch As Archive
    Dim mArch As MArchive

    Dim WillCountToRead As Short = 0
    Dim IsBytesToRead As Boolean = False
    Dim pagesToRead As Short = 0
    Dim curtime As DateTime
    Dim IsmArchToRead As Boolean = False
    Dim ispackageError As Boolean = False

    Dim buffer(0 To 73) As Byte
    Dim bufferindex As Short = 0


    Dim m_isArchToDBWrite As Boolean = False
    Public Overrides Property isArchToDBWrite() As Boolean

        Get
            Return m_isArchToDBWrite
        End Get
        Set(ByVal value As Boolean)
            m_isArchToDBWrite = value
        End Set
    End Property
    Dim m_isMArchToDBWrite As Boolean = False



    'Public inputbuffer(69) As Byte

    Public Overrides Function CounterName() As String
        Return "TSRV20"
    End Function







    Private Function ChSum(ByVal buf() As Byte, ByVal sz As Long) As Long

        Return Crc16(buf, 0, sz)
    End Function

    Function Crc16(ByVal buf() As Byte, ByVal Start As Long, ByVal Size As Long) As Long
        Dim i As Long
        Dim fcs As Long
        Dim sh As Integer
        Dim up As Integer



        ' The initial FCS value
        fcs = &HFFFF&
        up = 1

        ' evaluate the FCS
        For i = Start To Start + Size - 1
            If up Then
                fcs = fcs Xor (buf(i) << 8)
            Else
                fcs = fcs Xor buf(i)
            End If
            up = 1 - up

            sh = 8
            While sh > 0
                If ((fcs And &H1) = &H1) And sh > 0 Then
                    fcs = fcs >> 1
                    fcs = fcs Xor &HA001
                    sh -= 1
                Else
                    fcs = fcs >> 1
                    sh -= 1
                End If
            End While




        Next i

        ' return the result
        Crc16 = fcs
    End Function







    Public Function VerifySum(ByVal buf() As Byte, ByVal sz As Int16) As Boolean
        Dim crc As Long
        crc = ChSum(buf, sz - 2)
        If (buf(sz - 2) = (crc And &HFF)) And (buf(sz - 1) = ((crc And &HFF00) >> 8)) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function Bytes2Single(ByVal hexValue() As Byte, ByVal index As Int16, ByVal Revers As Boolean) As Single

        Try

            Dim i As Integer = 0
            Dim bArray(0 To 3) As Byte

            For i = 0 To 3
                bArray(i) = hexValue(index + i)
            Next
            If Revers Then
                Array.Reverse(bArray)
            End If

            Return BitConverter.ToSingle(bArray, 0)

        Catch ex As Exception
            Return 0.0
        End Try
    End Function

    Private Function GetFlt(ByVal SI() As Byte, ByVal Pos As Integer) As Single
        Dim l As Single
        l = Bytes2Single(SI, Pos, True)
        Return l
    End Function
    Private Function GetLng(ByVal SI() As Byte, ByVal Pos As Integer) As Long

        Dim h As ULong
        h = 0
        Dim b1 As Integer, b2 As Integer, b3 As Integer, b0 As Integer
        Try
            b0 = SI(pos)
            b1 = SI(pos + 1)
            b2 = SI(pos + 2)
            b3 = SI(pos + 3)
            h = (b0 << 24) + (b1 << 16) + (b2 << 8) + b3
        Catch ex As Exception

            h = 0
        End Try
        Return h
    End Function
    Private Function GetInt(ByVal SI() As Byte, ByVal Pos As Integer) As Integer
        Dim h As Integer
        Dim b1 As Integer, b0 As Integer
        b0 = SI(pos)
        b1 = SI(pos + 1)
        h = (b0 << 8) + b1
        Return h
    End Function

    Private Function GetUInt(ByVal SI() As Byte, ByVal Pos As Integer) As UInteger
        Dim h As UInteger
        Dim b1 As UInteger, b0 As UInteger
        b0 = SI(Pos)
        b1 = SI(Pos + 1)
        h = (b0 << 8) + b1
        Return h
    End Function

    Private Function GetTotal(ByVal SI() As Byte, ByVal Pos As Integer) As Double
        Dim l1 As Long
        Dim l2 As Single
        l1 = GetLng(SI, Pos)
        l2 = Bytes2Single(SI, Pos + 4, True)
        Return l1 + l2
    End Function


    Public Function TryConnect() As Boolean

        MyTransport.CleanPort()
        EraseInputQueue()


        Dim buf(1000) As Byte

        buf(0) = 1
        buf(1) = &H11
        Dim crc As Long

        crc = Crc16(buf, 0, 2)
        buf(3) = (crc And &HFF00) >> 8
        buf(2) = crc And &HFF
        MyTransport.CleanPort()
        MyTransport.Write(buf, 0, 4)

        Dim j As Int16




        Dim btr As Long
        Dim sz As Long = 0
        Dim sz1 As Long = 0
        Dim bufStr As String = ""

        WaitForData()
READMORE:
        sz1 = sz
        btr = MyTransport.BytesToRead
        While btr > 0
            MyTransport.Read(buf, sz, btr)
            System.Threading.Thread.Sleep(CalcInterval(50))
            sz += btr
            btr = MyTransport.BytesToRead
        End While

        If sz > 0 Then
            If VerifySum(buf, sz) Then

                For j = 0 To sz - 1
                    If buf(j) >= &H20 Then
                        bufStr = bufStr + " j=" + j.ToString + "->" + Chr(buf(j)) + " V=" + buf(j).ToString
                    Else
                        bufStr = bufStr + " j=" + j.ToString + "->V=" + buf(j).ToString
                    End If
                Next

                Debug.Print(bufStr)

                bufStr = ""
                For j = 0 To sz - 1
                    If buf(j) >= &H20 Then
                        bufStr = bufStr + Chr(buf(j))
                    End If
                Next
                Debug.Print(bufStr)
                'VZLJOT

                'Vzljot TSR 62.10.03.09



                If buf(3) = Asc("V") And buf(4) = Asc("Z") And buf(5) = Asc("L") And buf(6) = Asc("J") And buf(7) = Asc("O") And buf(8) = Asc("T") And buf(9) = Asc(" ") Then
                    If bufStr.Contains(" 76.30.02.") Then
                        TSRV24M = False
                        Return True
                    End If

                    If bufStr.Contains(" 76.30.03.") Then
                        TSRV24M = True
                        Return True
                    End If

                    If bufStr.Contains(" 76.30.04.") Then
                        TSRV24MP = True
                        Return True
                    End If

                    'If (((Int()) fwVer & -256) == 0x76300400)
                    'Return slaveId.Substring(0, 15) == "VZLJOT TSRV-043" ? TsrBase.VzljotModel.Tsrv043 : TsrBase.VzljotModel.Tsrv024Mplus;
                    'If (((Int()) fwVer & -256) == 0x76300200)
                    '  Return TsrBase.VzljotModel.Tsrv024;
                    'If (((Int()) fwVer & -256) == 0x76300300)
                    '  Return TsrBase.VzljotModel.Tsrv024M;


                End If

            Else
                If sz1 <> sz Then
                    GoTo READMORE
                End If

            End If
        Else

            DriverTransport.SendEvent(UnitransportAction.LowLevelStop, "Данные не получены")

        End If
        Return False

    End Function

    Public Overrides Sub Connect()

        mIsConnected = False
        Dim cnt As Integer = 5
        While cnt > 0
            mIsConnected = TryConnect()
            If mIsConnected Then

                'Dim i As Integer
                'For i = 1 To 3
                '    Dim k As Double
                '    k = ReadFloat(49189UL - 1 + (i - 1) * 2)
                '    If k > 0 Then k = -k
                '    PT(i) = 1.0 + k
                '    If PT(i) <= 0 Then PT(i) = 1.0
                '    If IsOld Then
                '        MV(i) = ReadChar(60 - 1 + i - 1)
                '    Else
                '        MV(i) = ReadChar(84 - 1 + i - 1)
                '    End If

                'Next
                Return
            End If

            cnt = cnt - 1
        End While




    End Sub




    Private Function EncodeError(ByVal e As Byte) As String
        Select Case e

            Case 1
                Return "ILLEGAL FUNCTION"
            Case 2
                Return "ILLEGAL DATA ADDRESS"
            Case 3
                Return "ILLEGAL DATA VALUE"
            Case 4
                Return "FAILURE IN ASSOCIATED DEVICE"
            Case 5
                Return "ACKNOWLEDGE"
            Case 6
                Return "BUSY, REJECTED MESSAGE"
            Case 7
                Return "NAK-NEGATIVE ACKNOWLEDGMENT"
            Case Else
                Return "UNKNOWN ERROR"
        End Select

    End Function



    Private m_readRAMByteCount As Short

    Private Function ReadArchHalf(ByVal ArchType As Short, ByVal ArchDate As DateTime, ByRef Arch As Archive) As Boolean

        If TSRV24MP Then
            Return ReadArchHalfPlus(ArchType, ArchDate, Arch)
        End If

        Dim retsum As String
        Dim ok As Boolean
        Dim buf(1000) As Byte
        Dim HC As UInteger


        Try


            cleararchive(Arch)
            EraseInputQueue()
            Dim dt2 As Date
            Dim devdate As Date
            devdate = GetDeviceDate()
            devdate = devdate.AddMinutes(-1)
            If ArchType = archType_hour Then
                dt2 = ArchDate

            End If
            If ArchType = archType_day Then
                dt2 = ArchDate
            End If

            If dt2 <= devdate Then
                Arch.archType = ArchType
                If ArchType = archType_hour Then


                    buf(0) = 1
                    buf(1) = 65

                    buf(2) = 0  ' часовой архив
                    buf(3) = 0

                    buf(4) = 0 ' 1 запись
                    buf(5) = 1

                    buf(6) = 1 ' по времени
                    buf(7) = dt2.Second
                    buf(8) = dt2.Minute
                    buf(9) = dt2.Hour
                    buf(10) = dt2.Day
                    buf(11) = dt2.Month
                    buf(12) = (dt2.Year Mod 100)

                    Dim crc As Long
                    crc = Crc16(buf, 0, 13)
                    buf(13) = crc And &HFF
                    buf(14) = (crc And &HFF00) >> 8


                    MyTransport.CleanPort()
                    MyTransport.Write(buf, 0, 15)

                    WaitForData()

                    Dim btr As Long
                    Dim sz As Long = 0
                    btr = MyTransport.BytesToRead
                    While btr > 0
                        MyTransport.Read(buf, sz, btr)
                        System.Threading.Thread.Sleep(CalcInterval(50))
                        sz += btr
                        btr = MyTransport.BytesToRead
                    End While
                    If sz > 0 Then
                        If VerifySum(buf, sz) Then
                            SequenceErrorCount = 0
                            If sz = 5 Then
                                ok = False
                                retsum = EncodeError(buf(2))
                            Else


                                With Arch

                                    .Q1 = GetFlt(buf, 3 + 96)
                                    .Q2 = GetFlt(buf, 3 + 96 + 4)
                                    .Q4 = GetFlt(buf, 3 + 96 + 8)
                                    .Q5 = GetFlt(buf, 3 + 96 + 12)

                                    .M1 = GetFlt(buf, 3 + 112)
                                    .M2 = GetFlt(buf, 3 + 112 + 4)
                                    .M4 = GetFlt(buf, 3 + 112 + 8)
                                    .M5 = GetFlt(buf, 3 + 112 + 12)

                                    .V1 = GetFlt(buf, 3 + 128)
                                    .V2 = GetFlt(buf, 3 + 128 + 4)
                                    .v4 = GetFlt(buf, 3 + 128 + 8)
                                    .v5 = GetFlt(buf, 3 + 128 + 12)

                                    .T1 = GetInt(buf, 3 + 152) * 0.01
                                    .T2 = GetInt(buf, 3 + 152 + 2) * 0.01
                                    .T4 = GetInt(buf, 3 + 152 + 4) * 0.01
                                    .T5 = GetInt(buf, 3 + 152 + 6) * 0.01


                                    .P1 = GetUInt(buf, 3 + 152) / 1000
                                    .P2 = GetUInt(buf, 3 + 152 + 2) / 1000
                                    .P4 = GetUInt(buf, 3 + 152 + 4) / 1000
                                    .P5 = GetUInt(buf, 3 + 152 + 6) / 1000


                                    .oktime = GetUInt(buf, 3 + 4)

                                    .Errtime = GetUInt(buf, 3 + 10)

                                    .HC = GetLng(buf, 3 + 80)
                                End With
                                Arch.DateArch = dt2
                                ok = True
                            End If
                        Else
                            SequenceErrorCount += 1
                        End If
                    Else
                        SequenceErrorCount += 1
                    End If
                End If

                If ArchType = archType_day Then

                    buf(0) = 1
                    buf(1) = 65

                    buf(2) = 0  ' суточный архив
                    buf(3) = 1
                    buf(4) = 0 ' 1 запись
                    buf(5) = 1
                    buf(6) = 1 ' по времени
                    buf(7) = dt2.Second
                    buf(8) = dt2.Minute
                    buf(9) = dt2.Hour
                    buf(10) = dt2.Day
                    buf(11) = dt2.Month
                    buf(12) = (dt2.Year Mod 100)
                    Dim crc As Long
                    crc = Crc16(buf, 0, 13)
                    buf(13) = crc And &HFF
                    buf(14) = (crc And &HFF00) >> 8


                    MyTransport.Write(buf, 0, 15)

                    'Dim i As Int16
                    'Dim j As Int16

                    WaitForData()


                    Dim btr As Long
                    Dim sz As Long = 0
                    btr = MyTransport.BytesToRead
                    While btr > 0
                        MyTransport.Read(buf, sz, btr)
                        System.Threading.Thread.Sleep(CalcInterval(50))
                        sz += btr
                        btr = MyTransport.BytesToRead
                    End While


                    If sz > 0 Then
                        If VerifySum(buf, sz) Then
                            SequenceErrorCount = 0
                            If sz = 5 Then
                                ok = False
                                retsum = EncodeError(buf(2))
                            Else


                                With Arch
                                    .Q1 = GetFlt(buf, 3 + 96)
                                    .Q2 = GetFlt(buf, 3 + 96 + 4)
                                    .Q4 = GetFlt(buf, 3 + 96 + 8)
                                    .Q5 = GetFlt(buf, 3 + 96 + 12)

                                    .M1 = GetFlt(buf, 3 + 112)
                                    .M2 = GetFlt(buf, 3 + 112 + 4)
                                    .M4 = GetFlt(buf, 3 + 112 + 8)
                                    .M5 = GetFlt(buf, 3 + 112 + 12)

                                    .V1 = GetFlt(buf, 3 + 128)
                                    .V2 = GetFlt(buf, 3 + 128 + 4)
                                    .v4 = GetFlt(buf, 3 + 128 + 8)
                                    .v5 = GetFlt(buf, 3 + 128 + 12)

                                    .T1 = GetInt(buf, 3 + 152) * 0.01
                                    .T2 = GetInt(buf, 3 + 152 + 2) * 0.01
                                    .T4 = GetInt(buf, 3 + 152 + 4) * 0.01
                                    .T5 = GetInt(buf, 3 + 152 + 6) * 0.01


                                    .P1 = GetUInt(buf, 3 + 152) / 1000
                                    .P2 = GetUInt(buf, 3 + 152 + 2) / 1000
                                    .P4 = GetUInt(buf, 3 + 152 + 4) / 1000
                                    .P5 = GetUInt(buf, 3 + 152 + 6) / 1000


                                    .oktime = GetUInt(buf, 3 + 4)

                                    .Errtime = GetUInt(buf, 3 + 10)

                                    .HC = GetLng(buf, 3 + 80)
                                End With
                                ok = True
                                Arch.DateArch = dt2
                            End If
                        Else
                            SequenceErrorCount += 1
                        End If
                    Else
                        SequenceErrorCount += 1
                    End If


                End If
            End If



            Return ok
        Catch ex As System.Exception
            Return False
        End Try
    End Function



    Private Function ReadArchHalfPlus(ByVal ArchType As Short, ByVal ArchDate As DateTime, ByRef Arch As Archive) As Boolean

        Dim retsum As String
        Dim ok As Boolean
        Dim buf(1000) As Byte
        Dim HC As UInteger


        Try


            cleararchive(Arch)
            EraseInputQueue()
            Dim dt2 As Date
            Dim devdate As Date
            devdate = GetDeviceDate()
            devdate = devdate.AddMinutes(-1)
            If ArchType = archType_hour Then
                dt2 = ArchDate

            End If
            If ArchType = archType_day Then
                dt2 = ArchDate
            End If

            If dt2 <= devdate Then
                Arch.archType = ArchType
                If ArchType = archType_hour Then


                    buf(0) = 1
                    buf(1) = 65

                    buf(2) = 0  ' часовой архив
                    buf(3) = 0  ' 0,3,6 -  разные системы

                    buf(4) = 0 ' 1 запись
                    buf(5) = 1

                    buf(6) = 1 ' по времени
                    buf(7) = dt2.Second
                    buf(8) = dt2.Minute
                    buf(9) = dt2.Hour
                    buf(10) = dt2.Day
                    buf(11) = dt2.Month
                    buf(12) = (dt2.Year Mod 100)

                    Dim crc As Long
                    crc = Crc16(buf, 0, 13)
                    buf(13) = crc And &HFF
                    buf(14) = (crc And &HFF00) >> 8


                    MyTransport.CleanPort()
                    MyTransport.Write(buf, 0, 15)

                    WaitForData()

                    Dim btr As Long
                    Dim sz As Long = 0
                    btr = MyTransport.BytesToRead
                    While btr > 0
                        MyTransport.Read(buf, sz, btr)
                        System.Threading.Thread.Sleep(CalcInterval(50))
                        sz += btr
                        btr = MyTransport.BytesToRead
                    End While
                    If sz > 0 Then
                        If VerifySum(buf, sz) Then
                            SequenceErrorCount = 0
                            If sz = 5 Then
                                ok = False
                                retsum = EncodeError(buf(2))
                            Else


                                With Arch

                                    .Q1 = GetTotal(buf, 3 + 60)
                                    .Q2 = GetTotal(buf, 3 + 68 + 4)

                                    .M1 = GetTotal(buf, 3 + 84)
                                    .M2 = GetTotal(buf, 3 + 84 + 8)
                                    .M4 = GetTotal(buf, 3 + 84 + 16)
                                    .M5 = GetTotal(buf, 3 + 84 + 24)

                                    .V1 = GetTotal(buf, 3 + 116)
                                    .V2 = GetTotal(buf, 3 + 116 + 8)
                                    .v4 = GetTotal(buf, 3 + 116 + 16)
                                    .v5 = GetTotal(buf, 3 + 116 + 24)

                                    .T1 = GetInt(buf, 3 + 156) * 0.01
                                    .T2 = GetInt(buf, 3 + 156 + 2) * 0.01
                                    .T4 = GetInt(buf, 3 + 156 + 4) * 0.01
                                    .T5 = GetInt(buf, 3 + 156 + 6) * 0.01


                                    .P1 = GetUInt(buf, 3 + 164) / 1000
                                    .P2 = GetUInt(buf, 3 + 164 + 2) / 1000
                                    .P4 = GetUInt(buf, 3 + 164 + 4) / 1000
                                    .P5 = GetUInt(buf, 3 + 164 + 6) / 1000


                                    .oktime = GetLng(buf, 3 + 4)

                                    .Errtime = GetLng(buf, 3 + 16)

                                    .HC = GetLng(buf, 3 + 56)
                                End With
                                Arch.DateArch = dt2
                                ok = True
                            End If
                        Else
                            SequenceErrorCount += 1
                        End If
                    Else
                        SequenceErrorCount += 1
                    End If
                End If

                If ArchType = archType_day Then

                    buf(0) = 1
                    buf(1) = 65

                    buf(2) = 0  ' суточный архив
                    buf(3) = 1  ' 1,4,5 -  разные системы ???
                    buf(4) = 0 ' 1 запись
                    buf(5) = 1
                    buf(6) = 1 ' по времени
                    buf(7) = dt2.Second
                    buf(8) = dt2.Minute
                    buf(9) = dt2.Hour
                    buf(10) = dt2.Day
                    buf(11) = dt2.Month
                    buf(12) = (dt2.Year Mod 100)
                    Dim crc As Long
                    crc = Crc16(buf, 0, 13)
                    buf(13) = crc And &HFF
                    buf(14) = (crc And &HFF00) >> 8


                    MyTransport.Write(buf, 0, 15)

                    'Dim i As Int16
                    'Dim j As Int16

                    WaitForData()


                    Dim btr As Long
                    Dim sz As Long = 0
                    btr = MyTransport.BytesToRead
                    While btr > 0
                        MyTransport.Read(buf, sz, btr)
                        System.Threading.Thread.Sleep(CalcInterval(50))
                        sz += btr
                        btr = MyTransport.BytesToRead
                    End While


                    If sz > 0 Then
                        If VerifySum(buf, sz) Then
                            SequenceErrorCount = 0
                            If sz = 5 Then
                                ok = False
                                retsum = EncodeError(buf(2))
                            Else


                                With Arch
                                    .Q1 = GetTotal(buf, 3 + 60)
                                    .Q2 = GetTotal(buf, 3 + 68 + 4)

                                    .M1 = GetTotal(buf, 3 + 84)
                                    .M2 = GetTotal(buf, 3 + 84 + 8)
                                    .M4 = GetTotal(buf, 3 + 84 + 16)
                                    .M5 = GetTotal(buf, 3 + 84 + 24)

                                    .V1 = GetTotal(buf, 3 + 116)
                                    .V2 = GetTotal(buf, 3 + 116 + 8)
                                    .v4 = GetTotal(buf, 3 + 116 + 16)
                                    .v5 = GetTotal(buf, 3 + 116 + 24)

                                    .T1 = GetInt(buf, 3 + 156) * 0.01
                                    .T2 = GetInt(buf, 3 + 156 + 2) * 0.01
                                    .T4 = GetInt(buf, 3 + 156 + 4) * 0.01
                                    .T5 = GetInt(buf, 3 + 156 + 6) * 0.01


                                    .P1 = GetUInt(buf, 3 + 164) / 1000
                                    .P2 = GetUInt(buf, 3 + 164 + 2) / 1000
                                    .P4 = GetUInt(buf, 3 + 164 + 4) / 1000
                                    .P5 = GetUInt(buf, 3 + 164 + 6) / 1000


                                    .oktime = GetLng(buf, 3 + 4) / 60

                                    .Errtime = GetLng(buf, 3 + 16) / 60

                                    .HC = GetLng(buf, 3 + 56)
                                End With
                                ok = True
                                Arch.DateArch = dt2
                            End If
                        Else
                            SequenceErrorCount += 1
                        End If
                    Else
                        SequenceErrorCount += 1
                    End If


                End If
            End If



            Return ok
        Catch ex As System.Exception
            Return False
        End Try
    End Function


    Public Overrides Function ReadArch(ByVal ArchType As Short, ByVal ArchYear As Short,
   ByVal ArchMonth As Short, ByVal ArchDay As Short, ByVal ArchHour As Short) As String

        Dim retsum As String
        Dim ok As Boolean = False
        Dim buf(1000) As Byte
        Dim tryCnt As Integer
        tryCnt = 3
        While Not ok And tryCnt > 0

            tryCnt -= 1
            SequenceErrorCount = 0
            Try


                cleararchive(Arch)
                EraseInputQueue()
                Dim dt1 As Date, dt2 As Date
                Dim devdate As Date
                devdate = GetDeviceDate()
                devdate = devdate.AddMinutes(-1)
                If ArchType = archType_hour Then
                    dt2 = New Date(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)
                    'dt2 = dt2.AddHours(-1)
                    dt1 = dt2.AddHours(-1)

                End If
                If ArchType = archType_day Then
                    dt2 = New Date(ArchYear, ArchMonth, ArchDay, 0, 0, 0)
                    'dt2 = dt2.AddDays(-1)
                    dt1 = dt2.AddDays(-1)
                End If
                Arch.archType = ArchType

                If (ArchType = 3 And dt2 <= devdate.AddHours(-1)) Or (ArchType = 4 And dt2 <= devdate.AddHours(-25)) Then


                    Dim Arch1 As Archive
                    Dim Arch2 As Archive

                    Arch2 = New Archive
                    ok = ReadArchHalf(ArchType, dt2, Arch2)
                    If SequenceErrorCount > 0 Then ok = False

                    If ok Then
                        Arch1 = New Archive
                        ok = ReadArchHalf(ArchType, dt1, Arch1)
                        If SequenceErrorCount > 0 Then ok = False
                    End If

                    If ok Then
                        With Arch
                            .T1 = Arch2.T1
                            .T2 = Arch2.T2
                            .T3 = Arch2.T3
                            .T4 = Arch2.T4
                            .T5 = Arch2.T5
                            .T6 = Arch2.T6
                            .P1 = Arch2.P1
                            .P2 = Arch2.P2
                            .P3 = Arch2.P3
                            .P4 = Arch2.P4
                            .P5 = Arch2.P5
                            .P6 = Arch2.P6
                            .M1 = Arch2.M1 - Arch1.M1
                            .M2 = Arch2.M2 - Arch1.M2
                            .M3 = Arch2.M3 - Arch1.M3
                            .M4 = Arch2.M4 - Arch1.M4
                            .M5 = Arch2.M5 - Arch1.M5
                            .M6 = Arch2.M6 - Arch1.M6


                            .Q1 = Arch2.Q1 - Arch1.Q1
                            .Q2 = Arch2.Q2 - Arch1.Q2

                            .Q3 = Arch2.Q3 - Arch1.Q3
                            .Q4 = Arch2.Q4 - Arch1.Q4
                            .Q5 = Arch2.Q5 - Arch1.Q5

                            .Q6 = Arch2.Q6 - Arch1.Q6




                            .Errtime = Arch2.ErrtimeH - Arch1.ErrtimeH
                            .ErrtimeH = Arch2.ErrtimeH
                            .oktime = Arch2.oktime - Arch1.oktime
                            .Errtime2 = Arch2.Errtime2 - Arch1.Errtime2
                            .oktime2 = Arch2.oktime2 - Arch1.oktime2
                            .V1 = Arch2.V1 - Arch1.V1
                            .V2 = Arch2.V2 - Arch1.V2
                            .V3 = Arch2.V3 - Arch1.V3
                            .v4 = Arch2.v4 - Arch1.v4
                            .v5 = Arch2.v5 - Arch1.v5
                            .v6 = Arch2.v6 - Arch1.v6


                            .Q1H = Arch2.Q1
                            .Q2H = Arch2.Q2
                            .V1h = Arch2.V1
                            .V2h = Arch2.V2
                            .V3h = Arch2.V3
                            .HC = Arch2.HC
                            .MsgHC = Arch2.MsgHC
                            .oktime = Arch2.oktime
                            .Errtime = Arch2.Errtime
                            .ErrtimeH = Arch2.ErrtimeH
                        End With

                        If ArchType = archType_hour Then
                            dt2 = New Date(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)

                        End If
                        If ArchType = archType_day Then
                            dt2 = New Date(ArchYear, ArchMonth, ArchDay, 0, 0, 0)

                        End If
                        Arch.DateArch = dt2
                    End If
                End If
            Catch ex As System.Exception
                Return "Ошибка:" & ex.Message
            End Try
        End While
        If ok Then
            retsum = "Архив прочитан"
            retsum = retsum & vbCrLf
            EraseInputQueue()
            isArchToDBWrite = True
        Else
            retsum = "Ошибка: не удалось получить архив"
            retsum = retsum & vbCrLf
            EraseInputQueue()
            isArchToDBWrite = False
        End If

        Return retsum

    End Function

    Public Function DeCodeHCNumber(ByVal CodeHC As Long) As String

        Return Convert.ToString(CodeHC, 2)


    End Function
    Public Function DeCodeHCText(ByVal CodeHC As Long) As String
        DeCodeHCText = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHCText = DeCodeHCText + "Отказ первого преобразователя расхода для W1" & ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHCText = DeCodeHCText + "Отказ второго преобразователя расхода для W1" + ";"
        End If
        If CodeHC And 2 ^ 2 Then
            DeCodeHCText = DeCodeHCText + "Отказ первого преобразователя расхода для W2" + ";"
        End If
        If CodeHC And 2 ^ 3 Then
            DeCodeHCText = DeCodeHCText + "Отказ второго преобразователя расхода для W2" + ";"
        End If
        If CodeHC And 2 ^ 4 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя температуры для первой энтальпии в формуле W1" + ";"
        End If
        If CodeHC And 2 ^ 5 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя температуры для второй энтальпии в формуле W1" + ";"
        End If
        If CodeHC And 2 ^ 6 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя температуры для первой энтальпии в формуле W2" + ";"
        End If

        If CodeHC And 2 ^ 7 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя температуры для второй энтальпии в формуле W2" + ";"
        End If

        If CodeHC And 2 ^ 8 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя давления для первой энтальпии в формуле W1" + ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя давления для второй энтальпии в формуле W1" + ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя давления для первой энтальпии в формуле W2" + ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHCText = DeCodeHCText + "Отказ преобразователя давления для второй энтальпии в формуле W2" + ";"
        End If

        If CodeHC And 2 ^ 12 Then
            DeCodeHCText = DeCodeHCText + "НС1" + ";"
        End If

        If CodeHC And 2 ^ 13 Then
            DeCodeHCText = DeCodeHCText + "НС2" + ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHCText = DeCodeHCText + "НС3" + ";"
        End If
        If CodeHC And 2 ^ 15 Then
            DeCodeHCText = DeCodeHCText + "Отказ(EEPROM)" + ";"
        End If

    End Function
    Public Function DeCodeHC(ByVal CodeHC As Long) As String
        Return Convert.ToString(CodeHC, 2)
    End Function

    Public Function Bytes2Float(ByVal fbytes() As Byte) As Single
        If UBound(fbytes) < 3 Then
            Return 0
        End If
        Return System.BitConverter.ToSingle(fbytes, 0)
    End Function


    Private Function BToSingle(ByVal hexValue() As Byte, ByVal index As Int16) As Single

        Try

            Dim iInputIndex As Integer = 0

            Dim iOutputIndex As Integer = 0

            Dim bArray(3) As Byte



            For iInputIndex = 0 To 3

                bArray(iOutputIndex) = hexValue(index + iInputIndex)

                iOutputIndex += 1

            Next
            'Array.Reverse(bArray)
            Return BitConverter.ToSingle(bArray, 0)

        Catch ex As Exception

        End Try
    End Function







    Private Sub Flip4(ByRef b() As Byte, ByVal index As Int16)
        Dim t(4) As Byte
        Dim i As Int16
        For i = 0 To 3
            t(i) = b(index + i)
        Next
        For i = 0 To 3
            b(index + i) = t(3 - i)
        Next
    End Sub

    Private Sub Flip2(ByRef b() As Byte, ByVal index As Int16)
        Dim t(2) As Byte
        Dim i As Int16
        For i = 0 To 1
            t(i) = b(index + i)
        Next
        For i = 0 To 1
            b(index + i) = t(1 - i)
        Next
    End Sub
    Public Overrides Function WriteArchToDB() As String

        'If Arch.archType <> 4 Then
        '    Arch.DateArch = Arch.DateArch.AddSeconds(1)
        'End If

        WriteArchToDB = "INSERT INTO DATACURR(id_bd, id_ptype,DCALL,DCOUNTER,DATECOUNTER,t1,t2,t3,t4,t5,t6,p1,p2,p3,p4,p5,p6,v1,v2,v3,v4,v5,v6,m1,m2,m3,m4,m5,m6,q1,q2,q3,q4,q5,q6,hc_code,hc,errtime, errtimeh,worktime,oktime,oktime2,errtime2,hcraw) values ("
        WriteArchToDB = WriteArchToDB + "'" + DeviceID.ToString() + "',"
        WriteArchToDB = WriteArchToDB + "'" + Arch.archType.ToString() + "',"
        WriteArchToDB = WriteArchToDB + "SYSDATE" + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T1, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T2, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T3, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T4, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T5, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.T6, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P1, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P2, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P3, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P4, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P5, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.P6, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.V1, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.V2, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.V3, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.v4, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.v5, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.v6, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M1, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M2, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M3, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M4, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M5, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.M6, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q1, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q2, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q3, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q4, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q5, "##############0.000000").Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + NanFormat(Arch.Q6, "##############0.000000").Replace(",", ".") + ","
        If DeCodeHCNumber(Arch.HC) = "0000000000000000" Then
            WriteArchToDB = WriteArchToDB + "'','Нет НС'"
        Else
            WriteArchToDB = WriteArchToDB + "'" + S180(DeCodeHCNumber(Arch.HC)) + "','" + Arch.MsgHC + "'"
        End If
        WriteArchToDB = WriteArchToDB + "," + Format(Arch.Errtime, "##############0").Replace(",", ".")

        WriteArchToDB = WriteArchToDB + "," + Format(Arch.ErrtimeH, "##############0").Replace(",", ".")
        WriteArchToDB = WriteArchToDB + "," + NanFormat(Arch.oktime, "##############0.000000").Replace(",", ".")
        WriteArchToDB = WriteArchToDB + "," + NanFormat(Arch.oktime, "##############0.000000").Replace(",", ".")
        WriteArchToDB = WriteArchToDB + "," + NanFormat(Arch.oktime2, "##############0.000000").Replace(",", ".")
        WriteArchToDB = WriteArchToDB + "," + NanFormat(Arch.Errtime2, "##############0.000000").Replace(",", ".")
        WriteArchToDB = WriteArchToDB + "," + "'" + Arch.HC.ToString().Replace(",", ".") + "'"
        WriteArchToDB = WriteArchToDB + ")"
        Debug.Print(WriteArchToDB)
    End Function

    Private Function OracleDate(ByVal d As Date) As String
        Return "to_date('" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() +
            " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS')"
    End Function
    Public Overrides Function WriteMArchToDB() As String
        WriteMArchToDB = ""
        Try
            WriteMArchToDB = "INSERT INTO DATACURR(id_bd, id_ptype,DCALL,DCOUNTER,DATECOUNTER,t1,t2,t3,t4,t5,t6,v1,v2,v3,v4,v5,v6,M1,M2,M3,M4,M5,M6,P1,P2,P3,P4,P5,P6,q1,q2,q3,q4,q5,q6,hc_code,hc,hcraw) values ("
            WriteMArchToDB = WriteMArchToDB + DeviceID.ToString() + ","
            WriteMArchToDB = WriteMArchToDB + "1,"
            WriteMArchToDB = WriteMArchToDB + "SYSDATE" + ","
            WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
            WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t1, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t2, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t3, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t4, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t5, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.t6, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v1, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v2, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v3, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v4, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v5, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.v6, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m1, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m2, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m3, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m4, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m5, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.m6, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p1, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p2, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p3, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p4, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p5, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.p6, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G1, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G2, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G3, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G4, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G5, "##############0.000000").Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + NanFormat(mArch.G6, "##############0.000000").Replace(",", ".") + ","



            If DeCodeHCNumber(mArch.HC) = "0000000000000000" Then
                WriteMArchToDB = WriteMArchToDB + "'-','Нет НС',"

            Else
                WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.HC) + "','" + S180(mArch.MsgHC) + "',"



            End If

            WriteMArchToDB = WriteMArchToDB + "'" + mArch.HC.ToString().Replace(",", ".") + "'"
            WriteMArchToDB = WriteMArchToDB + ")"
        Catch
        End Try
        'Return WriteMArchToDB
    End Function




    Public Overrides Sub EraseInputQueue()
        If (IsBytesToRead = True) Then
            IsBytesToRead = False
        End If
        bufferindex = 0
        System.Threading.Thread.Sleep(150)
        MyTransport.CleanPort()
    End Sub
    Private Sub cleararchive(ByRef arc As Archive)
        arc.DateArch = DateTime.MinValue

        arc.HC = 0
        arc.MsgHC = ""

        arc.HCtv1 = 0
        arc.MsgHCtv1 = ""

        arc.HCtv2 = 0
        arc.MsgHCtv2 = ""

        arc.Tw1 = 0
        arc.Tw2 = 0

        arc.P1 = 0
        arc.T1 = 0
        arc.M2 = 0
        arc.V1 = 0

        arc.P2 = 0
        arc.T2 = 0
        arc.M3 = 0
        arc.V2 = 0

        arc.V3 = 0
        arc.M1 = 0

        arc.Q1 = 0
        arc.Q2 = 0

        arc.QG1 = 0
        arc.QG2 = 0

        arc.SP = 0
        arc.SPtv1 = 0
        arc.SPtv2 = 0

        arc.tx1 = 0
        arc.tx2 = 0
        arc.tair1 = 0
        arc.tair2 = 0

        arc.T3 = 0
        arc.T4 = 0
        arc.T5 = 0
        arc.T6 = 0
        arc.P3 = 0
        arc.P4 = 0
        arc.v4 = 0
        arc.v5 = 0
        arc.v6 = 0
        arc.M4 = 0
        arc.M5 = 0
        arc.M6 = 0

        arc.archType = 0
        isArchToDBWrite = False
    End Sub
    Private Sub clearMarchive(ByRef marc As MArchive)
        marc.DateArch = DateTime.MinValue
        marc.HC = 0
        marc.MsgHC = ""

        marc.HCtv1 = 0
        marc.MsgHCtv1 = ""

        marc.HCtv2 = 0
        marc.MsgHCtv2 = ""

        marc.G1 = 0
        marc.G2 = 0
        marc.G3 = 0
        marc.G4 = 0
        marc.G5 = 0
        marc.G6 = 0

        marc.t1 = 0
        marc.t2 = 0
        marc.t3 = 0
        marc.t4 = 0
        marc.t5 = 0
        marc.t6 = 0

        marc.p1 = 0
        marc.p2 = 0
        marc.p3 = 0
        marc.p4 = 0

        marc.dt12 = 0
        marc.dt45 = 0

        marc.tx1 = 0
        marc.tx2 = 0

        marc.tair1 = 0
        marc.tair2 = 0

        marc.SP = 0
        marc.SPtv1 = 0
        marc.SPtv2 = 0


        marc.archType = 1
        isMArchToDBWrite = False
    End Sub

    Private Function ChanelToByte(ByVal ch As Byte) As Byte
        '1 – первый канал,
        '0 – второй канал,
        '2 – холодная вода,
        '4 – четвертый канал,
        '3 – пятый канал.

        Select Case ch
            Case 1
                Return 1
            Case 2
                Return 0
            Case 3
                Return 2
            Case 4
                Return 4
            Case 5
                Return 3
            Case Else
                Return ch
        End Select
    End Function


    Private Function ReadFloat(ByVal addr As ULong) As Single
        Dim buf(30) As Byte
        Dim res As Single = 0.0
        Try
            buf(0) = 1
            buf(1) = &H4

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H2

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF
            MyTransport.CleanPort()

            MyTransport.Write(buf, 0, 8)




            WaitForData()


            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                System.Threading.Thread.Sleep(CalcInterval(50))
                sz += btr
                btr = MyTransport.BytesToRead
            End While
            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    If sz = 5 Then
                        Dim es As String
                        es = EncodeError(buf(2))
                        Debug.Print(es)
                    Else
                        res = GetFlt(buf, 3)
                    End If
                    SequenceErrorCount = 0
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
            SequenceErrorCount += 1
        End Try
        Return res

    End Function

    Private Function ReadLong(ByVal addr As ULong) As Long
        Dim buf(30) As Byte
        Dim res As Long = 0
        Try
            buf(0) = 1
            buf(1) = &H3

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H2

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF

            MyTransport.CleanPort()
            MyTransport.Write(buf, 0, 8)



            WaitForData()



            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                System.Threading.Thread.Sleep(CalcInterval(50))
                sz += btr
                btr = MyTransport.BytesToRead
            End While
            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    If sz = 5 Then
                        Dim es As String
                        es = EncodeError(buf(2))
                        Debug.Print(es)
                    Else
                        res = GetLng(buf, 3)
                    End If

                    SequenceErrorCount = 0
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
            SequenceErrorCount += 1
        End Try
        Return res

    End Function

    Private Function ReadInt(ByVal addr As ULong) As Int16
        Dim buf(30) As Byte
        Dim res As Int16 = 0
        Try
            buf(0) = 1
            buf(1) = &H3

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H1

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF
            MyTransport.CleanPort()

            MyTransport.Write(buf, 0, 8)

            WaitForData()


            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                System.Threading.Thread.Sleep(CalcInterval(20))
                sz += btr
                btr = MyTransport.BytesToRead
            End While

            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    If sz = 5 Then
                        Dim es As String
                        es = EncodeError(buf(2))
                        Debug.Print(es)
                    Else
                        res = GetInt(buf, 3)
                    End If
                    SequenceErrorCount = 0
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
            SequenceErrorCount += 1
        End Try
        Return res

    End Function



    Private Function ReadUInt(ByVal addr As ULong) As UInt16
        Dim buf(30) As Byte
        Dim res As UInt16 = 0
        Try
            buf(0) = 1
            buf(1) = &H3

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H1

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF
            MyTransport.CleanPort()

            MyTransport.Write(buf, 0, 8)

            WaitForData()


            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                System.Threading.Thread.Sleep(CalcInterval(20))
                sz += btr
                btr = MyTransport.BytesToRead
            End While

            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    If sz = 5 Then
                        Dim es As String
                        es = EncodeError(buf(2))
                        Debug.Print(es)
                    Else
                        res = GetUInt(buf, 3)
                    End If
                    SequenceErrorCount = 0
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
            SequenceErrorCount += 1
        End Try
        Return res

    End Function

    Private LastDate As DateTime = DateTime.MinValue

    Public Function GetDeviceDate() As Date

        If LastDate <> DateTime.MinValue Then
            Return LastDate
        End If

        Dim DateArch As Date
        Dim buf(1000) As Byte
        Dim Seconds As Long = 0
        Dim addr As Long
        Try


            addr = 32769 - 1


            buf(0) = 1
            buf(1) = &H3

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H2

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF

            MyTransport.CleanPort()
            MyTransport.Write(buf, 0, 8)

            WaitForData()

            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                Thread.Sleep(CalcInterval(20))
                sz += btr
                btr = MyTransport.BytesToRead
            End While

            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    SequenceErrorCount = 0
                    Seconds = GetLng(buf, 3)
                    DateArch = New Date(1970, 1, 1, 0, 0, 0)
                    DateArch = DateArch.AddSeconds(Seconds)
                    LastDate = DateArch
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
        End Try
        Return DateArch
    End Function

    Public Overrides Function ReadMArch() As String


        clearMarchive(mArch)
        EraseInputQueue()
        SequenceErrorCount = 0

        mArch.DateArch = GetDeviceDate()


        mArch.t1 = ReadFloat(49225UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.t2 = ReadFloat(49227UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr

        mArch.t4 = ReadFloat(49231UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr

        mArch.t5 = ReadFloat(49233UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr



        mArch.G1 = ReadFloat(49717UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.m1 = ReadFloat(49721UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.v1 = ReadFloat(49725UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr


        mArch.G2 = ReadFloat(49777UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.m2 = ReadFloat(49781UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.v2 = ReadFloat(49785UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr

        mArch.G4 = ReadFloat(49837UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.m4 = ReadFloat(49841UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.v4 = ReadFloat(49845UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr

        mArch.G4 = ReadFloat(49897UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.m5 = ReadFloat(49901UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr
        mArch.v5 = ReadFloat(49905UL - 1)
        If SequenceErrorCount > 5 Then GoTo archErr


        Dim hc As Long
        mArch.MsgHC = ""
        mArch.HC = 0
        'hc = ReadInt(16385UL - 1)
        'If SequenceErrorCount > 5 Then GoTo ArchErr
        'If hc > 0 Then
        '    mArch.MsgHC = mArch.MsgHC & "ТС1:" & DeCodeHCText(hc)
        '    mArch.HC = mArch.HC Or hc
        'End If
        'hc = ReadInt(16386UL - 1)
        'If SequenceErrorCount > 5 Then GoTo ArchErr
        'If hc > 0 Then
        '    mArch.MsgHC = mArch.MsgHC & "ТС2:" & DeCodeHCText(hc)
        '    mArch.HC = mArch.HC Or hc
        'End If

        'hc = ReadInt(16387UL - 1)
        'If SequenceErrorCount > 5 Then GoTo ArchErr
        'If hc > 0 Then
        '    mArch.MsgHC = mArch.MsgHC & "ТС3:" & DeCodeHCText(hc)
        '    mArch.HC = mArch.HC Or hc
        'End If



        isMArchToDBWrite = True
        Return "Мгновенный архив прочитан"

archErr:
        isMArchToDBWrite = False
        Return "Ошибка чтения мгновенного архива"
    End Function
    Dim m_isTArchToDBWrite As Boolean = False
    Public Overrides Property isTArchToDBWrite() As Boolean
        Get
            Return m_isTArchToDBWrite
        End Get
        Set(ByVal value As Boolean)
            m_isTArchToDBWrite = value
        End Set
    End Property



    Private Sub clearTarchive(ByRef marc As TArchive)
        marc.DateArch = DateTime.MinValue
        marc.V1 = 0
        marc.V2 = 0
        marc.V3 = 0
        marc.V4 = 0
        marc.V5 = 0
        marc.V6 = 0
        marc.M1 = 0
        marc.M2 = 0
        marc.M3 = 0
        marc.M4 = 0
        marc.M5 = 0
        marc.M6 = 0
        marc.Q1 = 0
        marc.Q2 = 0
        marc.TW1 = 0
        marc.TW2 = 0
        marc.archType = 2
        isTArchToDBWrite = False
    End Sub

    Public Overrides Function ReadTArch() As String


        Dim bArr(0 To 8) As Byte
        Dim temptv As Short
        temptv = tv
        clearTarchive(tArch)
        EraseInputQueue()
        SequenceErrorCount = 0



        tArch.DateArch = GetDeviceDate()

        Dim half As Archive
        half = New Archive
        ReadArchHalf(archType_hour, tArch.DateArch.AddMinutes(-59), half)
        If SequenceErrorCount > 5 Then GoTo ArchErr

        tArch.M1 = half.M1
        tArch.M2 = half.M2
        tArch.M3 = half.M3
        tArch.M4 = half.M4
        tArch.M5 = half.M5
        tArch.M6 = half.M6

        tArch.V1 = half.V1
        tArch.V2 = half.V2
        tArch.V3 = half.V3

        tArch.Q1 = half.Q1
        tArch.Q2 = half.Q2
        tArch.Q3 = half.Q3
        tArch.Q4 = half.Q4
        tArch.Q5 = half.Q5
        tArch.Q6 = half.Q6

        tArch.Errtime = half.Errtime ' ReadLong(32775UL - 1) / 60
        tArch.oktime = half.oktime ' ReadLong(32773UL - 1) / 60

        tArch.Errtime2 = half.Errtime2  'ReadLong(32783UL - 1) / 60
        tArch.oktime2 = half.oktime2 'ReadLong(32785UL - 1) / 60

        isTArchToDBWrite = True

        Return "Итоговый архив прочитан"
ArchErr:
        isTArchToDBWrite = False

        Return "Ошибка чтения итогового архива"
    End Function

    Public Overrides Function WriteTArchToDB() As String
        WriteTArchToDB = "INSERT INTO DATACURR(id_bd,id_ptype,DCALL,DCOUNTER,DATECOUNTER,Q1,Q2,Q3,Q4,Q5,Q6,M1,M2,M3,M4,M5,M6,v1,v2,v3,v4,v5,v6,TSUM1,TSUM2,worktime,oktime,ERRTIME,oktime2,ERRTIME2) values ("
        WriteTArchToDB = WriteTArchToDB + DeviceID.ToString() + ","
        WriteTArchToDB = WriteTArchToDB + tArch.archType.ToString() + ","
        WriteTArchToDB = WriteTArchToDB + "SYSDATE" + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","

        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q1, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q2, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q3, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q4, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q5, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Q5, "##############0.000000").Replace(",", ".") + ","


        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M1, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M2, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M3, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M4, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M5, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.M6, "##############0.000000").Replace(",", ".") + ","

        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V1, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V2, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V3, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V4, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V5, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.V6, "##############0.000000").Replace(",", ".") + ","

        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.TW1, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.TW2, "##############0.000000").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.oktime, "##############0").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.oktime, "##############0").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Errtime, "##############0").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.oktime2, "##############0").Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + NanFormat(tArch.Errtime2, "##############0").Replace(",", ".")
        WriteTArchToDB = WriteTArchToDB + ")"
    End Function

    Private Function ExtLong4(ByVal extStr As String) As Double
        Dim i As Long
        On Error Resume Next
        ExtLong4 = 0
        For i = 0 To 3
            ExtLong4 = ExtLong4 + Asc(Mid(extStr, 1 + i, 1)) * (256 ^ (i))
        Next i
    End Function


    Public Overrides Function IsConnected() As Boolean
        If MyTransport Is Nothing Then Return False
        Return mIsConnected And MyTransport.IsConnected
    End Function
    Private Function S180(ByVal s As String) As String

        Dim outs As String
        outs = s
        If outs.Length <= 180 Then
            Return outs
        End If
        outs = outs.Substring(0, 180)
        Return outs
    End Function




    Private Function TableForArch(ByVal ArchType As Short) As String
        Dim tName As String = ""
        If ArchType = 1 Then
            tName = "TPLC_M"
        End If

        If ArchType = 3 Then
            tName = "TPLC_H"
        End If
        If ArchType = 4 Then
            tName = "TPLC_D"
        End If
        If ArchType = 2 Then
            tName = "TPLC_T"
        End If
        Return tName
    End Function

    Public Overrides Function ReadSystemParameters() As DataTable
        Dim dt As DataTable
        dt = New DataTable
        dt.Columns.Add("Название")
        dt.Columns.Add("Значение")

        Dim dr As DataRow

        'Запрос на функцию 17:
        'Длина(поля, байт)  Содержание поля
        '1:      Адрес устройства
        '1:      Номер функции (=17)

        'Ответ:
        'Длина(поля, байт)        Содержание поля
        '1:      Адрес устройства
        '1:      Номер функции (=17)
        '1:      Длина данных
        'До вер. 63.01.03.31: строка: "Взлёт ТСРВ-030 63.01.03.XX" с нулём на конце, где "XX" - модификация версии (кодировка: Win1251)
        'С вер. 63.01.03.31: строка: "VZLJOT 63.01.03.XX " с нулём на конце, где "XX" - модификация версии (кодировка: Win1251)
        '2:      Количество битовых ячеек ввода (=0)
        '2:      Количество битовых ячеек хранения (=0)
        '2:      Количество регистров ввода типа целое значение 1 байт
        '2:      Количество регистров ввода типа целое значение 2 байта
        '2:      Количество регистров ввода типа целое значение 4 байта
        '2:      Количество регистров ввода типа вещественное значение
        '2:      Количество регистров хранения типа целое значение 1 байт
        '2:      Количество регистров хранения типа целое значение 2 байта
        '2:      Количество регистров хранения типа целое значение 4 байта
        '2:      Количество регистров хранения типа вещественное значение

        Dim buf(1000) As Byte
        EraseInputQueue()

        buf(0) = 1
        buf(1) = 17
        Dim crc As Long
        crc = Crc16(buf, 0, 2)
        buf(2) = crc And &HFF
        buf(3) = (crc And &HFF00) >> 8

        MyTransport.Write(buf, 0, 4)

        WaitForData()

        Dim b(4096) As Byte
        Dim cnt As Integer
        cnt = MyTransport.BytesToRead
        Dim sz As Integer
        Dim ptr As Integer
        ptr = 0
        sz = 0
        If cnt > 0 Then
            While cnt > 0
                MyTransport.Read(b, ptr, cnt)
                Thread.Sleep(CalcInterval(20))
                ptr += cnt
                sz += cnt

                RaiseIdle()
                cnt = MyTransport.BytesToRead
            End While
        End If
        Dim i As Integer = 0
        Dim dev As String = ""

        For i = 0 To sz - 23  '18
            dev += Chr(b(3 + i))
        Next i

        If sz > 0 Then
            dr = dt.NewRow
            dr("Название") = "Устройство"
            dr("Значение") = dev
            dt.Rows.Add(dr)

            Dim indReg As Integer
            indReg = sz - 20
            dr = dt.NewRow
            dr("Название") = "Количество битовых ячеек ввода (=0)"
            dr("Значение") = GetInt(b, indReg + 0)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество битовых ячеек хранения (=0)"
            dr("Значение") = GetInt(b, indReg + 2)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров ввода типа целое значение 1 байт"
            dr("Значение") = GetInt(b, indReg + 4)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров ввода типа целое значение 2 байта"
            dr("Значение") = GetInt(b, indReg + 5)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров ввода типа целое значение 4 байта"
            dr("Значение") = GetInt(b, indReg + 8)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров ввода типа вещественное значение"
            dr("Значение") = GetInt(b, indReg + 10)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров хранения типа целое значение 1 байт"
            dr("Значение") = GetInt(b, indReg + 12)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров хранения типа целое значение 2 байта"
            dr("Значение") = GetInt(b, indReg + 14)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров хранения типа целое значение 4 байта"
            dr("Значение") = GetInt(b, indReg + 16)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Количество регистров хранения типа вещественное значение"
            dr("Значение") = GetInt(b, indReg + 18)
            dt.Rows.Add(dr)


            '432769: Заводской номер ТВ, unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ТВ"
            dr("Значение") = ReadLong(32769UL - 1)
            dt.Rows.Add(dr)

            '432771: Код(объекта) unsigned long
            dr = dt.NewRow
            dr("Название") = "Код объекта"
            dr("Значение") = ReadLong(32771UL - 1)
            dt.Rows.Add(dr)

            '432773: Заводской номер ПР 1,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПР 1"
            dr("Значение") = ReadLong(32773UL - 1)
            dt.Rows.Add(dr)

            '432775: Заводской номер ПР 2,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПР 2"
            dr("Значение") = ReadLong(32775UL - 1)
            dt.Rows.Add(dr)

            '432777: Заводской номер ПР 3,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПР 3"
            dr("Значение") = ReadLong(32777UL - 1)
            dt.Rows.Add(dr)

            '432779: Заводской номер ПТ 1,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПТ 1"
            dr("Значение") = ReadLong(32779UL - 1)
            dt.Rows.Add(dr)

            '432781: Заводской номер ПТ 2,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПТ 2"
            dr("Значение") = ReadLong(32781UL - 1)
            dt.Rows.Add(dr)

            '432783: Заводской номер ПТ 3,unsigned long
            dr = dt.NewRow
            dr("Название") = "Заводской номер ПТ 3"
            dr("Значение") = ReadLong(32783UL - 1)
            dt.Rows.Add(dr)

            '400001: Адрес в сети Modbus unsigned char

            dr = dt.NewRow
            dr("Название") = "Адрес в сети Modbus"
            dr("Значение") = ReadChar(1 - 1)
            dt.Rows.Add(dr)


            '400002 Индекс скорости RS232, unsigned char

            If ReadChar(2UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Индекс скорости RS232"
                dr("Значение") = "1200 бод"
                dt.Rows.Add(dr)
            ElseIf ReadChar(2UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Индекс скорости RS232"
                dr("Значение") = "2400 бод"
                dt.Rows.Add(dr)
            ElseIf ReadChar(2UL - 1) = 2 Then
                dr = dt.NewRow
                dr("Название") = "Индекс скорости RS232"
                dr("Значение") = "4800 бод"
                dt.Rows.Add(dr)
            ElseIf ReadChar(2UL - 1) > 2 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If

            '400003: Дополнительный байтовый тайм-аут при приёме, 10 мс:с вер. 63.01.03.02 / Задержка ответа по Modbus, мс С вер. 63.01.03.31
            dr = dt.NewRow
            dr("Название") = "Доп. байтовый тайм-аут при приёме, 10 мс"
            dr("Значение") = ReadChar(3UL - 1)
            dt.Rows.Add(dr)

            dr = dt.NewRow
            dr("Название") = "Задержка ответа по Modbus, мс"
            dr("Значение") = ReadChar(3UL - 1)
            dt.Rows.Add(dr)

            '400007: Единицы отображения тепла, unsigned char

            If ReadChar(7UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Единицы отображения тепла"
                dr("Значение") = "Дж"
                dt.Rows.Add(dr)
            ElseIf ReadChar(7UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Единицы отображения тепла"
                dr("Значение") = "кал"
                dt.Rows.Add(dr)
            ElseIf ReadChar(7UL - 1) > 1 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If

            '400008: Разрешение перевода часов на летнее/зимнее время,unsigned char

            If ReadChar(8UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Разрешение перевода часов на летнее/зимнее время"
                dr("Значение") = "Запрещено"
                dt.Rows.Add(dr)
            ElseIf ReadChar(8UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Разрешение перевода часов на летнее/зимнее время"
                dr("Значение") = "Разрешено"
                dt.Rows.Add(dr)
            ElseIf ReadChar(8UL - 1) > 1 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If

            '400012 Использование ПT1,unsigned char
            If ReadChar(12UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 1"
                dr("Значение") = "Не используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(12UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 1"
                dr("Значение") = "Используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(12UL - 1) > 1 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If

            '400013 Использование ПT2,unsigned char
            If ReadChar(13UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 2"
                dr("Значение") = "Не используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(13UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 2"
                dr("Значение") = "Используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(13UL - 1) > 1 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If
            '400014 Использование ПT3,unsigned char
            If ReadChar(14UL - 1) = 0 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 3"
                dr("Значение") = "Не используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(14UL - 1) = 1 Then
                dr = dt.NewRow
                dr("Название") = "Использование ПТ 3"
                dr("Значение") = "Используется"
                dt.Rows.Add(dr)
            ElseIf ReadChar(14UL - 1) > 1 Then
                dr = dt.NewRow
                dr("Название") = "Ошибка: "
                dr("Значение") = "Неверное значение параметра"
                dt.Rows.Add(dr)
            End If
        End If

        'dr = dt.NewRow
        'dr("Название") = "Плотность 1: "
        'dr("Значение") = PT(1).ToString
        'dt.Rows.Add(dr)

        'dr = dt.NewRow
        'dr("Название") = "Плотность 2: "
        'dr("Значение") = PT(2).ToString
        'dt.Rows.Add(dr)
        'dr = dt.NewRow

        'dr("Название") = "Плотность 3: "
        'dr("Значение") = PT(3).ToString
        'dt.Rows.Add(dr)


        'dr = dt.NewRow
        'dr("Название") = "Масса \ Объем  1: "
        'dr("Значение") = IIf(MV(1) = 0, "МАССА", "ОБЪЕМ")
        'dt.Rows.Add(dr)

        'dr = dt.NewRow
        'dr("Название") = "Масса \ Объем 2: "
        'dr("Значение") = IIf(MV(2) = 0, "МАССА", "ОБЪЕМ")
        'dt.Rows.Add(dr)
        'dr = dt.NewRow

        'dr("Название") = "Масса \ Объем 3: "
        'dr("Значение") = IIf(MV(3) = 0, "МАССА", "ОБЪЕМ")
        'dt.Rows.Add(dr)




        Return dt
    End Function

    Private Function ReadChar(ByVal addr As ULong) As Byte
        'Dim b1 As UInteger
        ''Dim b0 As UInteger
        'b1 = ReadInt(addr)
        ''b0 = b1 >> 8
        'Return b1 And &HFF


        Dim buf(30) As Byte
        Dim res As Byte = 0
        Try
            buf(0) = 1
            buf(1) = &H3

            buf(2) = (addr And &HFF00) >> 8
            buf(3) = addr And &HFF
            buf(4) = &H0
            buf(5) = &H1

            Dim crc As Long
            crc = Crc16(buf, 0, 6)
            buf(7) = (crc And &HFF00) >> 8
            buf(6) = crc And &HFF
            MyTransport.CleanPort()

            MyTransport.Write(buf, 0, 8)

            WaitForData()


            Dim btr As Long
            Dim sz As Long = 0
            btr = MyTransport.BytesToRead
            While btr > 0
                MyTransport.Read(buf, sz, btr)
                System.Threading.Thread.Sleep(CalcInterval(20))
                sz += btr
                btr = MyTransport.BytesToRead
            End While

            If sz > 0 Then
                If VerifySum(buf, sz) Then
                    If sz = 5 Then
                        Dim es As String
                        es = EncodeError(buf(2))
                        Debug.Print(es)

                    Else
                        res = buf(4)

                    End If
                    SequenceErrorCount = 0
                Else
                    SequenceErrorCount += 1
                End If
            Else
                SequenceErrorCount += 1
            End If
        Catch
            SequenceErrorCount += 1
        End Try
        Return res


    End Function
    Public Overrides Property isMArchToDBWrite() As Boolean
        Get
            Return m_isMArchToDBWrite
        End Get
        Set(ByVal value As Boolean)
            m_isMArchToDBWrite = value
        End Set
    End Property


End Class
