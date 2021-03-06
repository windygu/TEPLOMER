﻿Imports STKTVMain
Imports System.IO
Imports System.Threading



Public Class driver
    Inherits STKTVMain.TVDriver

    Private Enum TegM4
        teg_OCTET_STRING = &H4 'Строка октетов
        teg_NULL = &H5 'Нет данных
        teg_ASCIIString = &H16 'Строка ASCII-символов
        teg_SEQUENCE = &H30 'Последовательность
        teg_IntU = &H41 'Беззнаковое целое (unsigned int)
        teg_IntS = &H42 'Целое со знаком (int)
        teg_IEEFloat = &H43 'Число с плавающей точкой IEEE 754 Float
        teg_MIXED = &H44 'Параметр с комбинированным значением int+float
        teg_Operative = &H45 'Оперативный параметр настроечной БД
        teg_ACK = &H46 'Подтверждение
        teg_TIME = &H47 'Текущее время
        teg_DATE = &H48 'Текущая календарная дата
        teg_ARCHDATE = &H49 'Дата архивной записи
        teg_PNUM = &H4A 'Номер параметра
        teg_FLAGS = &H4B 'Сборка флагов
        teg_ERR = &H55 'Ошибка
    End Enum

    Private Enum cmdM4
        cmd_ERROR = &H21 'Ошибка
        cmd_CONNECT = &H3F 'Запрос сеанса связи
        cmd_CHANGE_SPEED = &H42 'Запрос изменения скорости обмена
        cmd_COUNT_CONTROL = &H4F 'Запрос управления счетом
        cmd_ARCHIV = &H61 'Запрос поиска архивной записи
        cmd_PARAM = &H72 'Запрос чтения параметра
        cmd_WRITEPARAM = &H77 'Запрос записи параметра
    End Enum


    Private Class blockM4
        Public teg As TegM4
        Public dl As Integer
        Public data(1) As Byte
        Public Sub New(ByVal _teg As TegM4, _dl As Integer)
            teg = _teg
            dl = _dl
            ReDim data(dl)
        End Sub
    End Class

    Private Class messageM4
        Public ID As Byte
        Public cmd As cmdM4
        Public size As Integer
        Public Tegs As List(Of blockM4)

        Public Sub New(_cmd As Integer, _size As Integer)
            cmd = _cmd
            size = _size
            Tegs = New List(Of blockM4)
        End Sub

        Public Function BuildMessage(ByVal Id As Byte) As Byte()
            Dim bArr(4096) As Byte
            Dim res() As Byte
            Dim sz As Integer
            Dim pos As Integer
            Dim i As Integer
            Dim crc As UShort
            bArr(0) = &H10 ' not in crc !
            bArr(1) = &HFF
            bArr(2) = &H90
            bArr(3) = Id
            bArr(4) = &H0

            ' data size
            bArr(5) = &H0
            bArr(6) = &H0

            'data
            bArr(7) = cmd
            sz = 1
            pos = 7
            For Each b As blockM4 In Tegs
                pos += 1
                sz += 1
                bArr(pos) = b.teg
                pos += 1
                sz += 1
                bArr(pos) = b.dl 
                For i = 0 To b.dl - 1
                    pos += 1
                    sz += 1
                    bArr(pos) = b.data(i)
                Next
            Next

            bArr(5) = sz Mod 256
            bArr(6) = sz \ 256


            'crc
            crc = M4CRC(bArr, 1, pos)
            bArr(pos + 1) = crc \ 256
            bArr(pos + 2) = crc Mod 256
            ReDim res(0 To pos + 2)
            For i = 0 To pos + 2
                res(i) = bArr(i)
            Next
            Return res
        End Function

    End Class

    Private Function ParseM4Sequence(buf() As Byte, sz As Integer) As List(Of blockM4)

        Dim Tegs As List(Of blockM4)
        Dim ok As Boolean = True
        Dim pos As Integer
        Dim blen As Integer
        Dim block As blockM4
        blen = buf.Length - 1

        Tegs = New List(Of blockM4)()

        pos = 0
        Dim t As TegM4
        Dim dl As Integer
        Dim i As Integer
        Dim q As Integer
        While pos < sz
            t = CType(buf(pos), TegM4)
            pos += 1

            If (buf(pos) And &H80) = &H80 Then
                q = (buf(pos) And &H7F)
                dl = 0
                pos += 1

                If q = 1 Then
                    dl = buf(pos)
                End If
                If q = 2 Then
                    dl = buf(pos) * 256 + buf(pos + 1)
                End If
                If q = 3 Then
                    dl = buf(pos) * 256 * 256 + buf(pos + 1) * 256 + buf(pos + 2)
                End If

                pos += q
            Else
                dl = buf(pos)
                pos = pos + 1
            End If


            If pos + dl < sz Then
                block = New blockM4(t, dl)
                'pos = pos + 2
                For i = 0 To dl - 1
                    block.data(i) = buf(pos + i)
                Next
                pos = pos + dl
                Tegs.Add(block)
            Else
                Exit While
            End If

        End While

        Return Tegs

    End Function

    Private Function CheckHeader(buf() As Byte) As Boolean
        If buf(0) <> &H10 Then Return False
        If buf(1) <> &HFF Then Return False
        If buf(2) <> &H90 Then Return False


        If buf(4) <> &H0 Then Return vbFalse
        Return True

        
    End Function

    Private Function ParseM4Message(buf() As Byte) As messageM4

        Dim msg As messageM4
        Dim sz As Integer
        Dim ID As Byte
        Dim ok As Boolean = True
        Dim cmd As cmdM4
        Dim crc As UShort
        Dim pos As Integer
        Dim blen As Integer
        Dim block As blockM4
        blen = buf.Length

        If blen < 9 Then Return Nothing
        If buf(0) <> &H10 Then Return Nothing

        If buf(1) <> &HFF Then Return Nothing
        If buf(2) <> &H90 Then Return Nothing

        ID = buf(3)
        If buf(4) <> &H0 Then Return Nothing

        ' data size
        sz = buf(6) * 256 + buf(5)


        'command
        cmd = CType(buf(7), cmdM4)


        
        'crc
        'crc = M4CRC(buf, 1, buf.Length - 3)
        'If buf(buf.Length - 2) <> crc \ 256 Then Return Nothing
        'If buf(buf.Length - 1) <> crc Mod 256 Then Return Nothing

        msg = New messageM4(cmd, sz)
        msg.ID = ID
        If cmd = cmdM4.cmd_CONNECT Or cmd = cmdM4.cmd_ERROR Then
            Return msg
        End If


        pos = 8
        Dim t As TegM4
        Dim dl As Integer
        Dim q As Integer
        Dim i As Integer
        While pos < sz + 7

            t = CType(buf(pos), TegM4)
            pos = pos + 1
            If (buf(pos) And &H80) = &H80 Then
                q = (buf(pos) And &H7F)
                dl = 0
                pos += 1

                If q = 1 Then
                    dl = buf(pos)
                End If
                If q = 2 Then
                    dl = buf(pos) * 256 + buf(pos + 1)
                End If

                If q = 3 Then
                    dl = buf(pos) * 256 * 256 + buf(pos + 1) * 256 + buf(pos + 2)
                End If

                pos += q
            Else
                dl = buf(pos)
                pos = pos + 1
            End If

            If pos + dl < blen + 2 Then
                block = New blockM4(t, dl)

                For i = 0 To dl - 1
                    block.data(i) = buf(pos + i)
                Next
                pos = pos + dl
                msg.Tegs.Add(block)
            End If

        End While

        Return msg

    End Function

    Private Function GetDeviceDate() As Date
        Dim d As Date
        Dim barr() As Byte
        Dim inbuf(1024) As Byte
        Dim msg As messageM4
        Dim block As blockM4
        Dim i As Integer
        Dim mID As Byte
        d = DateTime.Now
        msg = New messageM4(cmdM4.cmd_PARAM, 0)
        block = New blockM4(TegM4.teg_PNUM, 3)
        block.data(0) = 0  ' chanel=0
        block.data(1) = 1025 Mod 256
        block.data(2) = 1025 \ 256

        msg.Tegs.Add(block)

        block = New blockM4(TegM4.teg_PNUM, 3)
        block.data(0) = 0 ' chanel=0
        block.data(1) = 1024 Mod 256
        block.data(2) = 1024 \ 256

        msg.Tegs.Add(block)

        mID = NextID()

        barr = msg.BuildMessage(mID)



        write(barr, barr.Length)


        WaitForData()



        i = MyRead(inbuf, 0, 22, 200)
        If i > 0 Then
        If CheckCRC16(inbuf, 1, i - 3) Then

            msg = ParseM4Message(inbuf)
            If msg.cmd = cmdM4.cmd_PARAM Then 'And msg.ID = mID Then
                block = msg.Tegs(0)
                d = DateSerial(block.data(2) + 2000, block.data(1), block.data(0))
                block = msg.Tegs(1)
                d = d.AddTicks(TimeSerial(block.data(3), block.data(2), block.data(1)).Ticks)
            End If

        End If
        End If


        Return d

    End Function

    Private mIsConnected As Boolean

    Private PacketID As Byte = 0

    Private Function NextID() As Byte
        PacketID += 1
        If PacketID = 255 Then
            PacketID = 1
        End If
        Return PacketID
    End Function




    Private Structure MArchive
        Public DateArch As DateTime
        Public HC As Long
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

        Public M1 As Single
        Public M2 As Single
        Public M3 As Single

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

        Public dt12 As Single
        Public dt45 As Single

        Public tx1 As Single
        Public tx2 As Single

        Public tair1 As Single
        Public tair2 As Single

        Public SP As Long
        Public SPtv1 As Long
        Public SPtv2 As Long


        Public archType As Short
    End Structure

    Private Structure Archive
        Public DateArch As DateTime

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
        Public v4 As Single
        Public v5 As Single
        Public v6 As Single
        Public M4 As Single
        Public M5 As Single
        Public M6 As Single
        Public WORKTIME1 As Single
        Public ERRTIME1 As Single
        Public OKTIME1 As Single
        Public archType As Short
    End Structure

    Private Structure TArchive
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
        Public TE1 As Double
        Public TE2 As Double
        Public TSUM1 As Double
        Public TSUM2 As Double
        Public Q3 As Double
        Public Q4 As Double

        Public archType As Short
    End Structure

    Dim tArch As TArchive
    Dim IsTArchToRead As Boolean = False
    ' Dim WithEvents tim As System.Timers.Timer

    Dim tv As Short

    Public Const archType_moment As Integer = 1
    Public Const archType_total As Integer = 2
    Public Const archType_hour As Integer = 3
    Public Const archType_day As Integer = 4


    Dim Arch As Archive
    Dim mArch As MArchive

    Dim WillCountToRead As Short = 0
    Dim IsBytesToRead As Boolean = False
    Dim pagesToRead As Short = 0
    Dim curtime As DateTime
    Dim IsmArchToRead As Boolean = False
    Dim ispackageError As Boolean = False

    Dim buffer(0 To 32000) As Byte
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
    Public Overrides Property isMArchToDBWrite() As Boolean

        Get
            Return m_isMArchToDBWrite
        End Get
        Set(ByVal value As Boolean)
            m_isMArchToDBWrite = value
        End Set
    End Property
    Dim m_isTArchToDBWrite As Boolean = False
    Public Overrides Property isTArchToDBWrite() As Boolean
        Get
            Return m_isTArchToDBWrite
        End Get
        Set(ByVal value As Boolean)
            m_isTArchToDBWrite = value
        End Set
    End Property

    'Public inputbuffer(69) As Byte

    Public Overrides Function CounterName() As String
        Return "SPT943"
    End Function

    Private m_serverip As String



    Private Sub Set9600()
        Dim bArr(0 To 20) As Byte
        Dim crc As UShort
        Dim i As Integer

        '10 FF 90 00 00 05 00 42 03 00 00 00 7E 39

        bArr(0) = &H10 ' not in crc !
        bArr(1) = &HFF
        bArr(2) = &H90
        bArr(3) = NextID()
        bArr(4) = &H0

        ' data size
        bArr(5) = &H5
        bArr(6) = &H0

        'data
        bArr(7) = &H42
        bArr(8) = &H2
        bArr(9) = &H0
        bArr(10) = &H0
        bArr(11) = &H0

        'crc
        crc = M4CRC(bArr, 1, 4 + 2 + 5)
        bArr(12) = crc \ 256
        bArr(13) = crc Mod 256

        EraseInputQueue()
        WillCountToRead = 13
        IsBytesToRead = True

        write(bArr, 14)

       

        WaitForData()



        i = MyRead(bArr, 0, 10, 200)
     

    End Sub

    Private Function TryConnect() As Boolean
        EraseInputQueue()
        Dim inbuf(64) As Byte
        Dim startBytes(0 To 30) As Byte
        Dim i As Int16




        For i = 0 To 20
            startBytes(i) = &HFF
        Next


        write(startBytes, 16)
        System.Threading.Thread.Sleep(CalcInterval(16))
        System.Threading.Thread.Sleep(1000)

        Dim bArr(0 To 20) As Byte
        Try

            bArr(0) = &H10
            bArr(1) = &HFF
            bArr(2) = &H3F
            bArr(3) = &H0
            bArr(4) = &H0
            bArr(5) = &H0
            bArr(6) = &H0
            'bArr(7) = &HC1
            bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
            bArr(8) = &H16
            EraseInputQueue()
            WillCountToRead = 8
            IsBytesToRead = True

            write(bArr, 9)
            tv = 1
        Catch exc As Exception
        End Try

        WaitForData()
        i = MyRead(inbuf, 0, 8, 200)
        If i = 8 Then
            If CheckCRC8(inbuf, 1, 5) = False Then
                Return False
            End If
        Else
            If i = 0 Then
                DriverTransport.SendEvent(UnitransportAction.LowLevelStop, "Данные не получены")
            End If

            Return False

        End If


        For i = 0 To 20
            startBytes(i) = &HFF
        Next


        write(startBytes, 16)
        System.Threading.Thread.Sleep(CalcInterval(16))
        System.Threading.Thread.Sleep(1000)
        Dim crc As UShort

        '10 FF 90 00 00 05 00 3F 00 00 00 00 D9 19 
        Try
            ' header
            bArr(0) = &H10 ' not in crc !
            bArr(1) = &HFF
            bArr(2) = &H90
            bArr(3) = NextID()
            bArr(4) = &H0

            ' data size
            bArr(5) = &H5
            bArr(6) = &H0

            'data
            bArr(7) = &H3F
            bArr(8) = &H0
            bArr(9) = &H0
            bArr(10) = &H0
            bArr(11) = &H0

            'crc
            crc = M4CRC(bArr, 1, 4 + 2 + 5)
            bArr(12) = crc \ 256
            bArr(13) = crc Mod 256

            EraseInputQueue()
            WillCountToRead = 13
            IsBytesToRead = True

            write(bArr, 14)

            tv = 0



            WaitForData()



            i = MyRead(inbuf, 0, 13, 200)
            If i = 13 Then
                If CheckCRC16(inbuf, 1, 10) = False Then
                    Return False
                End If
            Else
                If i = 0 Then
                    DriverTransport.SendEvent(UnitransportAction.LowLevelStop, "Данные не получены")
                End If

                Return False

            End If


            mIsConnected = True
            Return True


          
        Catch exc As Exception
            Return False
        End Try

    End Function


    Private m_readRAMByteCount As Short

    Public Overrides Function ReadArch(ByVal ArchType As Short, ByVal ArchYear As Short, _
    ByVal ArchMonth As Short, ByVal ArchDay As Short, ByVal ArchHour As Short) As String
        Dim mID As Byte
        Dim barr() As Byte
        Dim inbuf(1024) As Byte
        Dim msg As messageM4
        Dim block As blockM4
        Dim i As Integer
        Dim ok As Boolean = False
        Dim Seq As List(Of blockM4)
        Dim dok As Boolean
        Dim sz As Integer
        Dim trycnt As Integer

        cleararchive(Arch)

        Dim d As Date

        d = GetDeviceDate()

        EraseInputQueue()

        msg = New messageM4(cmdM4.cmd_ARCHIV, 0)

        block = New blockM4(TegM4.teg_OCTET_STRING, 5)
        block.data(0) = &HFF
        block.data(1) = &HFF
        block.data(2) = 0 ' chanel 1
        If (ArchType = archType_hour) Then
            block.data(3) = 0
        Else
            block.data(3) = 1
        End If

        block.data(4) = 1

        msg.Tegs.Add(block)


        block = New blockM4(TegM4.teg_ARCHDATE, 8)


        If (ArchType = archType_hour) Then
            block.data(0) = ArchYear - 2000
            block.data(1) = ArchMonth Mod 13
            block.data(2) = ArchDay Mod 32
            block.data(3) = ArchHour Mod 24
            block.data(4) = 0
            block.data(5) = 0
            block.data(6) = 0
            block.data(7) = 0
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)
            'Arch.DateArch = Arch.DateArch.AddSeconds(-1)

            Arch.archType = archType_hour

            If Arch.DateArch > d Then
                isArchToDBWrite = False
                Return "Ошибка даты архива"

            End If
        End If

        If (ArchType = archType_day) Then
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, 0, 0, 0)
            'Arch.DateArch = Arch.DateArch.AddSeconds(-1)
            block.data(0) = ArchYear - 2000
            block.data(1) = ArchMonth Mod 13
            block.data(2) = ArchDay Mod 32
            block.data(3) = 0
            block.data(4) = 0
            block.data(5) = 0
            block.data(6) = 0
            block.data(7) = 0
       
            Arch.archType = archType_day

            If Arch.DateArch > d Then
                isArchToDBWrite = False
                Return "Ошибка даты архива"

            End If
        End If

        msg.Tegs.Add(block)

        trycnt = 5
tv1_get:
        trycnt -= 1
        mID = NextID()

        barr = msg.BuildMessage(mID)

        EraseInputQueue()

        write(barr, barr.Length)
        WaitForData()


        i = MyRead(inbuf, 0, 7, 1000)

        If i = 7 Then
            If CheckHeader(inbuf) Then
                sz = inbuf(5) + inbuf(6) * 256
                i = MyRead(inbuf, 7, sz + 2, 3000)
            Else
                EraseInputQueue()
                i = 0
            End If
        End If



        dok = False
        If i > 0 Then
            If CheckCRC16(inbuf, 1, i + 7 - 3) Then
                msg = ParseM4Message(inbuf)
                If msg.cmd = cmdM4.cmd_ARCHIV Then 'msg.ID = mID And
                    block = msg.Tegs(0)
                    If block.teg = TegM4.teg_ARCHDATE And block.dl >= 3 Then
                        If (ArchType = archType_hour) Then
                            If block.data(0) = ArchYear - 2000 And block.data(1) = ArchMonth And block.data(2) = ArchDay And block.data(3) = ArchHour Then
                                dok = True
                            End If
                        Else
                            If block.data(0) = ArchYear - 2000 And block.data(1) = ArchMonth And block.data(2) = ArchDay Then
                                dok = True
                            End If
                        End If

                    End If
                    block = msg.Tegs(1)
                    If block.teg = TegM4.teg_SEQUENCE And dok Then
                        Seq = ParseM4Sequence(block.data, block.dl)
                        If Seq.Count > 41 Then



                            block = Seq(3)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.T1 = BToSingle(block.data, 0)
                            block = Seq(4)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.T2 = BToSingle(block.data, 0)
                            block = Seq(5)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.T3 = BToSingle(block.data, 0)
                            block = Seq(6)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.T4 = BToSingle(block.data, 0)
                            block = Seq(7)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.tx1 = BToSingle(block.data, 0)
                            block = Seq(8)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.tair1 = BToSingle(block.data, 0)

                            block = Seq(9)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.P1 = BToSingle(block.data, 0)
                            block = Seq(10)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.P2 = BToSingle(block.data, 0)
                            block = Seq(11)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.P3 = BToSingle(block.data, 0)
                            block = Seq(12)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.P4 = BToSingle(block.data, 0)
                            block = Seq(13)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.P5 = BToSingle(block.data, 0)

                            block = Seq(14)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.V1 = BToSingle(block.data, 0)
                            block = Seq(15)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.V2 = BToSingle(block.data, 0)
                            block = Seq(16)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.V3 = BToSingle(block.data, 0)
                      
                            block = Seq(17)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.M1 = BToSingle(block.data, 0)
                            block = Seq(18)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.M2 = BToSingle(block.data, 0)
                            block = Seq(19)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.M3 = BToSingle(block.data, 0)

                            block = Seq(20)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.Q1 = BToSingle(block.data, 0)
                            block = Seq(21)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.Q2 = BToSingle(block.data, 0)


                            block = Seq(22)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.WORKTIME1 = BToSingle(block.data, 0)
                            block = Seq(23)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.ERRTIME1 = BToSingle(block.data, 0)
                            block = Seq(24)
                            If block.teg = TegM4.teg_IEEFloat Then Arch.OKTIME1 = BToSingle(block.data, 0)



                            'If Seq.Count = 42 Then
                            block = Seq(Seq.Count - 1)
                                If block.teg = TegM4.teg_FLAGS Then Arch.HC = block.data(1) * 256 + block.data(0)
                                Arch.MsgHC = DeCodeHC(Arch.HC)
                            'End If

        End If

                            ok = True
                        GoTo arch_final
        End If


                            End If
                        End If
                    End If
        If trycnt > 0 Then GoTo tv1_get



arch_final:
        EraseInputQueue()
        If ok = False Then


            isArchToDBWrite = False
            Return "Ошибка чтения архива"

        Else
            isArchToDBWrite = True
            Return "Архива прочитан"
        End If


    End Function


    Private Function CheckCRC8(ByVal buf() As Byte, ByVal offset As Integer, ByVal len As Integer) As Boolean
        Dim KC As Long, i As Integer
        KC = 0
        For i = offset To offset + len - 1
            KC = KC + Int(buf(i))
        Next
        KC = 255 - (KC Mod 256)
        If KC = buf(offset + len) Then
            Return True
        Else
            Return False
        End If
    End Function


    Private Function CheckCRC16(ByVal buf() As Byte, ByVal offset As Integer, ByVal len As Integer) As Boolean
        Dim crc As UShort
        Try
        crc = M4CRC(buf, offset, len)
        If (buf(offset + len) = crc \ 256) And (buf(offset + len + 1) = crc Mod 256) Then
            Return True
        Else
            Return False
        End If
        Catch ex As Exception
            Debug.Print(ex.Message & vbCrLf & ex.StackTrace)
        End Try

    End Function



    Public Function DeCodeHCNumber(ByVal CodeHC As Long, ByVal tv As Int32) As String

        DeCodeHCNumber = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHCNumber = "TB" + tv.ToString + ":НС00" & ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС01" & ";"
        End If

        If CodeHC And 2 ^ 2 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС02" + ";"
        End If
        If CodeHC And 2 ^ 3 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС03" + ";"
        End If
        If CodeHC And 2 ^ 4 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС04" + ";"
        End If
        If CodeHC And 2 ^ 5 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС05" + ";"
        End If
        If CodeHC And 2 ^ 6 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС06" + ";"
        End If
        If CodeHC And 2 ^ 7 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":НС07" + ";"
        End If



        If CodeHC And 2 ^ 8 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС08" & ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС09" & ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС10" & ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС11 " & ";"
        End If

        If CodeHC And 2 ^ 12 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС12" & ";"
        End If

        If CodeHC And 2 ^ 13 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС13" & ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС14" & ";"
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС15" & ";"
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС16" & ";"
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС17 " & ";"
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС18" & ";"
        End If

        If CodeHC And 2 ^ 19 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС19" & ";"
        End If
        If CodeHC And 2 ^ 20 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС20" & ";"
        End If
        If CodeHC And 2 ^ 21 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС21" & ";"
        End If
        If CodeHC And 2 ^ 22 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС22" & ";"
        End If
        If CodeHC And 2 ^ 23 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString() + ":НС23" & ";"
        End If
    End Function
    Public Function DeCodeHCText(ByVal CodeHC As Long) As String

        DeCodeHCText = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHCText = DeCodeHCText _
                    & "Разряд батареи" & ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHCText = DeCodeHCText _
                    & "Перегрузка по цепям питания датчика объема" & ";"
        End If


        If CodeHC And 2 ^ 2 Then
            DeCodeHCText = DeCodeHCText _
                    & "Изменение сигнала на дискретном входе" & ";"
        End If

        If CodeHC And 2 ^ 3 Then
            DeCodeHCText = DeCodeHCText _
                    & "Tхв вне диапазона" & ";"
        End If

        If CodeHC And 2 ^ 4 Then
            DeCodeHCText = DeCodeHCText _
                    & "Выход контролируемого параметра за границы диапазона Ун -Ув " & ";"
        End If

        If CodeHC And 2 ^ 5 Then
            DeCodeHCText = DeCodeHCText _
                    & "нет расшифровки  " & ";"
        End If

        If CodeHC And 2 ^ 6 Then
            DeCodeHCText = DeCodeHCText _
                    & "нет расшифровки " & ";"
        End If

        If CodeHC And 2 ^ 7 Then
            DeCodeHCText = DeCodeHCText _
                    & "нет расшифровки " & ";"
        End If

        If CodeHC And 2 ^ 8 Then
            DeCodeHCText = DeCodeHCText _
                    & "P1 вне 0-1.1ВП1" & ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHCText = DeCodeHCText _
                    & "P2 вне 0-1.1ВП1" & ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHCText = DeCodeHCText _
                    & "T1 вне 0-176гр.С" & ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHCText = DeCodeHCText _
                    & "T2 вне 0-176гр.С" & ";"
        End If
        If CodeHC And 2 ^ 12 Then
            DeCodeHCText = DeCodeHCText _
                    & "T3 вне 0-176гр.С" & ";"
        End If


        If CodeHC And 2 ^ 13 Then
            DeCodeHCText = DeCodeHCText _
                    & "G1>Gв1" & ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G1<Gн1" & ";"
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHCText = DeCodeHCText _
                    & "G2>Gв2" & ";"
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G2<Gн2" & ";"
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHCText = DeCodeHCText _
                    & "G3>Gв3" & ";"
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G3<Gн3" & ";"
        End If


        If CodeHC And 2 ^ 19 Then
            DeCodeHCText = DeCodeHCText _
                    & "M1ч-М2ч  < (-HM)* M1ч" & ";"
        End If
        If CodeHC And 2 ^ 20 Then
            DeCodeHCText = DeCodeHCText _
                    & "Qч < 0 " & ";"
        End If
        If CodeHC And 2 ^ 21 Then
            DeCodeHCText = DeCodeHCText _
                    & "M1ч-М2ч  < 0" & ";"
        End If
        If CodeHC And 2 ^ 22 Then
            DeCodeHCText = DeCodeHCText _
                    & "НС22" & ";"
        End If
        If CodeHC And 2 ^ 23 Then
            DeCodeHCText = DeCodeHCText _
                    & "НС23" & ";"
        End If
    End Function
    Public Function DeCodeHC(ByVal CodeHC As Long) As String

        DeCodeHC = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHC = DeCodeHC _
                    & "HC:0 - Разряд батареи" & ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHC = DeCodeHC _
                    & "HC:1 - Перегрузка по цепям питания датчика объема" & ";"
        End If


        If CodeHC And 2 ^ 2 Then
            DeCodeHC = DeCodeHC _
                    & "HC:2 - Изменение сигнала на дисретном входе" & ";"
        End If

        If CodeHC And 2 ^ 3 Then
            DeCodeHC = DeCodeHC _
                    & "HC:3 - Tхв вне диапазона" & ";"
        End If

        If CodeHC And 2 ^ 4 Then
            DeCodeHC = DeCodeHC _
                    & "HC:4 - Выход контролируемого параметра за границы диапазона Ун -Ув " & ";"
        End If

        If CodeHC And 2 ^ 5 Then
            DeCodeHC = DeCodeHC _
                    & "HC:5 - нет расшифровки  " & ";"
        End If

        If CodeHC And 2 ^ 6 Then
            DeCodeHC = DeCodeHC _
                    & "HC:6 - нет расшифровки " & ";"
        End If

        If CodeHC And 2 ^ 7 Then
            DeCodeHC = DeCodeHC _
                    & "HC:7 - нет расшифровки " & ";"
        End If

        If CodeHC And 2 ^ 8 Then
            DeCodeHC = DeCodeHC _
                    & "HC:8 - P1 вне 0-1.1ВП1" & ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHC = DeCodeHC _
                    & "HC:9 - P2 вне 0-1.1ВП1" & ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHC = DeCodeHC _
                    & "HC:10 - T1 вне 0-176гр.С" & ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHC = DeCodeHC _
                    & "HC:11 - T2 вне 0-176гр.С" & ";"
        End If
        If CodeHC And 2 ^ 12 Then
            DeCodeHC = DeCodeHC _
                    & "HC:12 - T3 вне 0-176гр.С" & ";"
        End If


        If CodeHC And 2 ^ 13 Then
            DeCodeHC = DeCodeHC _
                    & "HC:13 - G1>Gв1" & ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHC = DeCodeHC _
                    & "HC:14 - 0<G1<Gн1" & ";"
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHC = DeCodeHC _
                    & "HC:15 - G2>Gв2" & ";"
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHC = DeCodeHC _
                    & "HC:16 - 0<G2<Gн2" & ";"
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHC = DeCodeHC _
                    & "HC:17 - G3>Gв3" & ";"
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHC = DeCodeHC _
                    & "HC:18 - 0<G3<Gн3" & ";"
        End If


        If CodeHC And 2 ^ 19 Then
            DeCodeHC = DeCodeHC _
                    & "HC:19 - M1ч-М2ч  < (-HM)* M1ч" & ";"
        End If
        If CodeHC And 2 ^ 20 Then
            DeCodeHC = DeCodeHC _
                    & "HC:20 - Qч < 0 " & ";"
        End If
        If CodeHC And 2 ^ 21 Then
            DeCodeHC = DeCodeHC _
                    & "HC:21 -M1ч-М2ч  < 0" & ";"
        End If
        If CodeHC And 2 ^ 22 Then
            DeCodeHC = DeCodeHC _
                    & "HC:22" & ";"
        End If
        If CodeHC And 2 ^ 23 Then
            DeCodeHC = DeCodeHC _
                    & "HC:23" & ";"
        End If
    End Function


    Private Function GetLng(ByVal SI() As Byte, ByVal Pos As Integer) As Long

        Dim h As ULong
        h = 0
        Dim b1 As Integer, b2 As Integer, b3 As Integer, b0 As Integer
        Try
            b0 = SI(Pos + 3)
            b1 = SI(Pos + 2)
            b2 = SI(Pos + 1)
            b3 = SI(Pos + 0)
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

    Public Function FloatExt(ByVal floatStr As String) As Single
        Dim tmpStr As String = ""
        Dim E As Long
        Dim Mantissa As Long
        Dim s As Long
        Dim f As Single
        Dim i As Long
        If floatStr = "" Then Exit Function
        If floatStr.Length <> 4 Then Exit Function
        ' If floatStr = String(4, 0) Then Exit Function
        If floatStr = Chr(0) + Chr(0) + Chr(0) + Chr(0) Then
            Return 0.0
        End If
        For i = 1 To 4
            tmpStr = Chr(Asc(Mid(floatStr, i, 1))) & tmpStr
        Next i


        floatStr = tmpStr
        '================ Float число========================
        'ст.байт                                 младший байт
        '====================================================
        'двоич.порядок |ст.байт                  младший байт
        '----------------------------------------------------
        ' xxxx xxxx     | sxxx xxxx | xxxx xxxx | xxxx xxxx |

        ' A = (-1)^s * f * 2^(e-127)
        ' f= сумма от 0 до 23 a(k)*2^(-k), где a(k) бит мантисы с номером k


        E = Asc(Mid(floatStr, 1, 1))
        If Asc(Mid(floatStr, 2, 1)) And (2 ^ 7) Then
            s = 1
        Else
            s = 0
        End If
        Mantissa = ((Asc(Mid(floatStr, 2, 1)) And &H7F) << 16) _
                     + (Asc(Mid(floatStr, 3, 1)) << 8) _
                     + (Asc(Mid(floatStr, 4, 1)))

        'Mantissa = (Asc(Mid(floatStr, 2, 1)) And &H7F) * (2 ^ 16) _
        '                     + Asc(Mid(floatStr, 3, 1)) * (2 ^ 8) _
        '                     + Asc(Mid(floatStr, 4, 1))

        f = 2 ^ 0
        For i = 22 To 0 Step -1
            If Mantissa And 2& ^ i Then
                f = f + 2 ^ (i - 23)
            End If
        Next i
        FloatExt = ((-1) ^ s) * f * (2.0! ^ (E - 127))
    End Function
    Private Function OracleDate(ByVal d As Date) As String
        Return "to_date('" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + _
            " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS')"
    End Function
    Public Overrides Function WriteArchToDB() As String
        '!!!!!!!!!!!!!!! 943 -  xx.59.59-> xx+1.00.00 !!!!
        'If Arch.archType <> 4 Then
        '    Arch.DateArch = Arch.DateArch.AddSeconds(1)
        'End If

        WriteArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,t1,t2,t3,t4,t5,t6,tce1,tce2,tair1,tair2,p1,p2,p3,p4,v1,v2,v3,v4,v5,v6,m1,m2,m3,m4,m5,m6,sp_TB1,sp_TB2,q1,q2,q4,q5,TSUM1,oktime,errtime,hc_code,hc,hc_1,hc_2) values ("
        WriteArchToDB = WriteArchToDB + DeviceID.ToString() + ","
        WriteArchToDB = WriteArchToDB + "SYSDATE" + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + Arch.archType.ToString + ","
        WriteArchToDB = WriteArchToDB + Arch.T1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T3.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T4.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T5.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T6.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.tx1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.tx2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.tair1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.tair2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.P1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.P2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.P3.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.P4.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.V1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.V2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.V3.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.v4.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.v5.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.v6.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M3.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M4.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M5.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.M6.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.SPtv1.ToString + ","
        WriteArchToDB = WriteArchToDB + Arch.SPtv2.ToString + ","
        WriteArchToDB = WriteArchToDB + Arch.Q1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.QG1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.Q2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.QG2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.WORKTIME1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.OKTIME1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.ERRTIME1.ToString.Replace(",", ".") + ","



        If DeCodeHCNumber(Arch.HCtv1, 1) = "" And DeCodeHCNumber(Arch.HCtv2, 2) = "" Then
            WriteArchToDB = WriteArchToDB + "'-','Нет НС',"
        ElseIf DeCodeHCNumber(Arch.HCtv1, 1) = "" Then
            WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.HCtv2, 2) + "','" + S180("Счетчик: кан2:" + DeCodeHCText(Arch.HCtv2)) + "',"
        ElseIf DeCodeHCNumber(Arch.HCtv2, 2) = "" Then
            WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.HCtv1, 1) + "','" + S180("Счетчик: кан1:" + DeCodeHCText(Arch.HCtv1)) + "',"
        Else
            WriteArchToDB = WriteArchToDB + "'" + S180(DeCodeHCNumber(Arch.HCtv1, 1) + DeCodeHCNumber(Arch.HCtv2, 2)) + "','" + S180("Счетчик: кан1:" + DeCodeHCText(Arch.HCtv1) + "кан2:" + DeCodeHCText(Arch.HCtv2)) + "',"
        End If

        'WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.HCtv1, 1) + ";" + DeCodeHCNumber(Arch.HCtv2, 2) + "',"
        WriteArchToDB = WriteArchToDB + "'" + S180(DeCodeHCText(Arch.HCtv1)) + "',"
        WriteArchToDB = WriteArchToDB + "'" + S180(DeCodeHCText(Arch.HCtv2)) + "'"
        WriteArchToDB = WriteArchToDB + ")"
    End Function
    Public Overrides Function WriteMArchToDB() As String
        WriteMArchToDB = ""
        Try
            WriteMArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,t1,t2,t3,t4,t5,t6,p1,p2,p3,p4,g1,g2,g3,g4,g5,g6,dt12,dt45,sp_TB1,sp_TB2,tce1,tce2,tair1,tair2,hc_code,hc,hc_1,hc_2) values ("
            WriteMArchToDB = WriteMArchToDB + DeviceID.ToString() + ","
            WriteMArchToDB = WriteMArchToDB + "SYSDATE" + ","

            WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
            WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
            WriteMArchToDB = WriteMArchToDB + mArch.archType.ToString + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t1.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t2.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t3.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t4.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t5.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.t6.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.p1.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.p2.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.p3.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.p4.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G1.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G2.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G3.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G4.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G5.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.G6.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.dt12.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.dt45.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.SPtv1.ToString + ","
            WriteMArchToDB = WriteMArchToDB + mArch.SPtv2.ToString + ","
            WriteMArchToDB = WriteMArchToDB + mArch.tx1.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.tx2.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.tair1.ToString.Replace(",", ".") + ","
            WriteMArchToDB = WriteMArchToDB + mArch.tair2.ToString.Replace(",", ".") + ","



            If DeCodeHCNumber(mArch.HCtv1, 1) = "" And DeCodeHCNumber(mArch.HCtv2, 2) = "" Then
                WriteMArchToDB = WriteMArchToDB + "'-','Нет НС',"
            ElseIf DeCodeHCNumber(mArch.HCtv1, 1) = "" Then
                WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.HCtv2, 2) + "','" + S180("Счетчик: кан2:" + DeCodeHCText(mArch.HCtv2)) + "',"
            ElseIf DeCodeHCNumber(mArch.HCtv2, 2) = "" Then
                WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.HCtv1, 1) + "','" + S180("Счетчик: кан1:" + DeCodeHCText(mArch.HCtv1)) + "',"
            Else
                WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.HCtv1, 1) + DeCodeHCNumber(mArch.HCtv2, 2) + "','" + S180("Счетчик: кан1:" + DeCodeHCText(mArch.HCtv1) + "кан2:" + DeCodeHCText(mArch.HCtv2)) + "',"
            End If

            WriteMArchToDB = WriteMArchToDB + "'" + S180(DeCodeHCText(mArch.HCtv1)) + "',"
            WriteMArchToDB = WriteMArchToDB + "'" + S180(DeCodeHCText(mArch.HCtv2)) + "'"
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

        MyTransport.CleanPort()
        System.Threading.Thread.Sleep(150)
        Dim buffer(256) As Byte
        Dim sz As Integer
        While MyTransport.BytesToRead
            sz = MyTransport.BytesToRead
            If sz > 256 Then sz = 256
            MyTransport.Read(buffer, 0, sz)
            System.Threading.Thread.Sleep(100)
        End While

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


        marc.archType = 0
        isMArchToDBWrite = False
    End Sub
    Public Overrides Function ReadMArch() As String

        Dim d As Date
        d = GetDeviceDate()

        Dim mID As Byte
        Dim barr() As Byte
        Dim inbuf(1024) As Byte
        Dim msg As messageM4
        Dim block As blockM4
        Dim i As Integer
        Dim ok As Boolean = False
        Dim sz As Integer
        Dim trycnt As Integer

        clearMarchive(mArch)
        mArch.archType = 1
        mArch.DateArch = d


        msg = New messageM4(cmdM4.cmd_PARAM, 0)
        For i = 1027 To 1045
            block = New blockM4(TegM4.teg_PNUM, 3)
            block.data(0) = 0  ' chanel=1
            block.data(1) = i Mod 256
            block.data(2) = i \ 256
            msg.Tegs.Add(block)
        Next

        trycnt = 5

tv1_again:
        trycnt -= 1

        mID = NextID()

        barr = msg.BuildMessage(mID)

        EraseInputQueue()

        write(bArr, bArr.Length)
        WaitForData()


        i = MyRead(inbuf, 0, 7, 1000)

        If i = 7 Then
            If CheckHeader(inbuf) Then
                sz = inbuf(5) + inbuf(6) * 256
                i = MyRead(inbuf, 7, sz + 2, 3000)
            Else
                i = 0
                EraseInputQueue()
            End If
        End If

        If i > 0 Then
            If CheckCRC16(inbuf, 1, i + 7 - 3) Then
                msg = ParseM4Message(inbuf)
                If msg.cmd = cmdM4.cmd_PARAM Then 'msg.ID = mID And 
                    block = msg.Tegs(0)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.G1 = BToSingle(block.data, 0)
                    block = msg.Tegs(1)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.G2 = BToSingle(block.data, 0)
                    block = msg.Tegs(2)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.G3 = BToSingle(block.data, 0)

                    block = msg.Tegs(3)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.M1 = BToSingle(block.data, 0)
                    block = msg.Tegs(4)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.M2 = BToSingle(block.data, 0)
                    block = msg.Tegs(5)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.M3 = BToSingle(block.data, 0)

                    block = msg.Tegs(6)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.t1 = BToSingle(block.data, 0)
                    block = msg.Tegs(7)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.t2 = BToSingle(block.data, 0)
                    block = msg.Tegs(8)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.t3 = BToSingle(block.data, 0)
                    block = msg.Tegs(9)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.t4 = BToSingle(block.data, 0)
                    block = msg.Tegs(10)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.t5 = BToSingle(block.data, 0)
                    block = msg.Tegs(11)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.tx1 = BToSingle(block.data, 0)




                    block = msg.Tegs(12)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.p1 = BToSingle(block.data, 0)
                    block = msg.Tegs(13)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.p2 = BToSingle(block.data, 0)
                    block = msg.Tegs(14)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.p3 = BToSingle(block.data, 0)
                    block = msg.Tegs(15)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.p4 = BToSingle(block.data, 0)
                    block = msg.Tegs(16)
                    If block.teg = TegM4.teg_IEEFloat Then mArch.p5 = BToSingle(block.data, 0)


                    block = msg.Tegs(18)
                    If block.teg = TegM4.teg_FLAGS Then mArch.HC = block.data(1) * 256 + block.data(0)
                    mArch.MsgHC = DeCodeHC(mArch.HC)
                    ok = True
                    GoTo march_final
                End If
            End If
        End If
        If trycnt > 0 Then GoTo tv1_again


march_final:

        EraseInputQueue()
        If ok = False Then
            EraseInputQueue()
            isMArchToDBWrite = False
            Return "Ошибка чтения мгновенного архива "
        End If

        isMArchToDBWrite = True
        Return "Мгновенный архив прочитан"
    End Function




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
        Dim d As Date
        d = GetDeviceDate()

        Dim mID As Byte
        Dim barr() As Byte
        Dim inbuf(1024) As Byte
        Dim msg As messageM4
        Dim block As blockM4
        Dim i As Integer
        Dim ok As Boolean = False
        Dim sz As Integer
        Dim trycnt As Integer

        clearTarchive(tArch)
        tArch.archType = 2
        tArch.DateArch = d


        msg = New messageM4(cmdM4.cmd_PARAM, 0)
        For i = 2048 To 2058
            block = New blockM4(TegM4.teg_PNUM, 3)
            block.data(0) = 0 ' chanel=1
            block.data(1) = i Mod 256
            block.data(2) = i \ 256
            msg.Tegs.Add(block)
        Next
        'block = New blockM4(TegM4.teg_PNUM, 3)
        'block.data(0) = 1  ' chanel=1
        'block.data(1) = 2062 Mod 256
        'block.data(2) = 2062 \ 256
        'msg.Tegs.Add(block)

        'block = New blockM4(TegM4.teg_PNUM, 3)
        'block.data(0) = 1  ' chanel=1
        'block.data(1) = 2063 Mod 256
        'block.data(2) = 2063 \ 256
        'msg.Tegs.Add(block)

        'block = New blockM4(TegM4.teg_PNUM, 3)
        'block.data(0) = 1  ' chanel=1
        'block.data(1) = 2056 Mod 256
        'block.data(2) = 2056 \ 256
        'msg.Tegs.Add(block)


        trycnt = 5

tv1_tagain:

        trycnt -= 1
        mID = NextID()

        barr = msg.BuildMessage(mID)

        EraseInputQueue()

        write(barr, barr.Length)
        WaitForData()



        i = MyRead(inbuf, 0, 7, 1000)

        If i = 7 Then
            If CheckHeader(inbuf) Then
                sz = inbuf(5) + inbuf(6) * 256
                i = MyRead(inbuf, 7, sz + 2, 3000)
            Else
                i = 0
                EraseInputQueue()
            End If
        End If

        If i > 0 Then
            If CheckCRC16(inbuf, 1, i + 7 - 3) Then
                msg = ParseM4Message(inbuf)
                If msg.cmd = cmdM4.cmd_PARAM Then ' msg.ID = mID And
                    block = msg.Tegs(0)
                    If block.teg = TegM4.teg_MIXED Then tArch.V1 = BToSingle(block.data, 4) + GetLng(block.data, 0)
                    block = msg.Tegs(1)
                    If block.teg = TegM4.teg_MIXED Then tArch.V2 = BToSingle(block.data, 4) + GetLng(block.data, 0)
                    block = msg.Tegs(2)
                    If block.teg = TegM4.teg_MIXED Then tArch.V3 = BToSingle(block.data, 4) + GetLng(block.data, 0)

                    block = msg.Tegs(3)
                    If block.teg = TegM4.teg_MIXED Then tArch.M1 = BToSingle(block.data, 4) + GetLng(block.data, 0)
                    block = msg.Tegs(4)
                    If block.teg = TegM4.teg_MIXED Then tArch.M2 = BToSingle(block.data, 4) + GetLng(block.data, 0)

                    block = msg.Tegs(5)
                    If block.teg = TegM4.teg_MIXED Then tArch.M3 = BToSingle(block.data, 4) + GetLng(block.data, 0)


                    block = msg.Tegs(6)
                    If block.teg = TegM4.teg_MIXED Then tArch.Q1 = BToSingle(block.data, 4) + GetLng(block.data, 0)
                    block = msg.Tegs(7)
                    If block.teg = TegM4.teg_MIXED Then tArch.Q2 = BToSingle(block.data, 4) + GetLng(block.data, 0)


                    block = msg.Tegs(8)
                    If block.teg = TegM4.teg_IEEFloat Then tArch.TE1 = BToSingle(block.data, 0)
                    block = msg.Tegs(9)
                    If block.teg = TegM4.teg_IEEFloat Then tArch.TW1 = BToSingle(block.data, 0)
                    block = msg.Tegs(10)
                    If block.teg = TegM4.teg_IEEFloat Then tArch.TSUM1 = BToSingle(block.data, 0)

                    ok = True
                    GoTo tarch_final
                End If
            End If
        End If
        If trycnt > 0 Then
            GoTo tv1_tagain
        End If





tarch_final:

        EraseInputQueue()
        If ok = False Then

            isTArchToDBWrite = False
            Return "Ошибка чтения итогового архива "
        End If

        isTArchToDBWrite = True
        Return "Итоговый архив прочитан"
       
    End Function

    Public Overrides Function WriteTArchToDB() As String
        WriteTArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,Q1,Q2,Q4,Q5,M1,M2,M3,M4,M5,M6,v1,v2,v3,v4,v5,v6,oktime,errtime,worktime ,TSUM1,TSUM2) values ("  ' 
        WriteTArchToDB = WriteTArchToDB + DeviceID.ToString() + ","
        WriteTArchToDB = WriteTArchToDB + "SYSDATE" + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","
        WriteTArchToDB = WriteTArchToDB + tArch.archType.ToString + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q2.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q3.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q4.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M2.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M3.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M4.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M5.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.M6.ToString.Replace(",", ".") + ","

        WriteTArchToDB = WriteTArchToDB + tArch.V1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.V2.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.V3.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.V4.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.V5.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.V6.ToString.Replace(",", ".") + ","

        WriteTArchToDB = WriteTArchToDB + tArch.TSUM1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.TE1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.TW1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.TSUM1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.TSUM2.ToString.Replace(",", ".") '+ ","
        'WriteTArchToDB = WriteTArchToDB + tArch.TW2.ToString.Replace(",", ".")
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

    Public Overrides Function ReadSystemParameters() As System.Data.DataTable

        TryConnect()
        EraseInputQueue()
        Dim dt As DataTable
        Dim dr As DataRow
        Dim cn(200) As String
    


        dt = New DataTable
        dt.Columns.Add("Название")
        dt.Columns.Add("Значение")

        Dim d As Date
        d = GetDeviceDate()

        Dim mID As Byte
        Dim barr() As Byte
        Dim inbuf(1024) As Byte
        Dim msg As messageM4
        Dim block As blockM4
        Dim i As Integer
        Dim ok As Boolean = False
        Dim s As String
        Dim j As Integer



        i = 0
        cn(i) = "СП"
        i = i + 1
        cn(i) = "ЕИ/P"
        i = i + 1
        cn(i) = "ЕИ/Q"
        i = i + 1
        cn(i) = "ТО"
        i = i + 1
        cn(i) = "ДО"
        i = i + 1
        cn(i) = "PКЧ"
        i = i + 1
        cn(i) = "СР"
        i = i + 1
        cn(i) = "ЧР"
        i = i + 1
        cn(i) = "ПЛ"
        i = i + 1
        cn(i) = "tхк"
        i = i + 1
        cn(i) = "Pxк"
        i = i + 1
        cn(i) = "ТС"
        i = i + 1
        cn(i) = "ТС1"
        i = i + 1
        cn(i) = "tк1"
        i = i + 1
        cn(i) = "ТС2"
        i = i + 1
        cn(i) = "tк2"
        i = i + 1
        cn(i) = "ТС3"
        i = i + 1
        cn(i) = "tк3"
        i = i + 1
        cn(i) = "ПД1"
        i = i + 1
        cn(i) = "ВП1"
        i = i + 1
        cn(i) = "Pк1"
        i = i + 1
        cn(i) = "ПД2"
        i = i + 1
        cn(i) = "ВП2"
        i = i + 1
        cn(i) = "Pк2"
        i = i + 1
        cn(i) = "ПД3"
        i = i + 1
        cn(i) = "ВП3"
        i = i + 1
        cn(i) = "Pк3"
        i = i + 1
        cn(i) = "С1"
        i = i + 1
        cn(i) = "Gв1"
        i = i + 1
        cn(i) = "Gн1"
        i = i + 1
        cn(i) = "Gкв1"
        i = i + 1
        cn(i) = "Gк"
        i = i + 1
        cn(i) = "н1"
        i = i + 1
        cn(i) = "Gотс1"
        i = i + 1
        cn(i) = "AGв1"
        i = i + 1
        cn(i) = "AGн1"
        i = i + 1
        cn(i) = "С2"
        i = i + 1
        cn(i) = "Gв"
        i = i + 1
        cn(i) = "Gн2"
        i = i + 1
        cn(i) = "Gкв2"
        i = i + 1
        cn(i) = "Gкн2"
        i = i + 1
        cn(i) = "Gотс2"
        i = i + 1
        cn(i) = "AGв2"
        i = i + 1
        cn(i) = "AGн2"
        i = i + 1
        cn(i) = "С3"
        i = i + 1
        cn(i) = "Gв3"
        i = i + 1
        cn(i) = "Gн3"
        i = i + 1
        cn(i) = "Gкв3"
        i = i + 1
        cn(i) = "Gкн3"
        i = i + 1
        cn(i) = "Gотс3"
        i = i + 1
        cn(i) = "AGв3"
        i = i + 1
        cn(i) = "Gн3"
        i = i + 1
        cn(i) = "НМ"
        i = i + 1
        cn(i) = "Mк"
        i = i + 1
        cn(i) = "АМк"
        i = i + 1
        cn(i) = "АrV"
        i = i + 1
        cn(i) = "Qк"
        i = i + 1
        cn(i) = "АQк"
        i = i + 1
        cn(i) = "ИД"
        i = i + 1
        cn(i) = "КИ1"
        i = i + 1
        cn(i) = "КИ2"
        i = i + 1
        cn(i) = "КИ3"
        i = i + 1
        cn(i) = "КД1"
        i = i + 1
        cn(i) = "AКД1"
        i = i + 1
        cn(i) = "КД2"
        i = i + 1
        cn(i) = "АСТ1"
        i = i + 1
        cn(i) = "АСТ2"
        i = i + 1
        cn(i) = "АСТ3"
        i = i + 1
        cn(i) = "АСТ4"
        i = i + 1
        cn(i) = "АСТ5"
        i = i + 1
        cn(i) = "АСТ6"
        i = i + 1
        cn(i) = "АСТ7"
        i = i + 1
        cn(i) = "АСТ8"
        i = i + 1
        cn(i) = "АСТ9"
        i = i + 1
        cn(i) = "АСТ10"
        i = i + 1
        cn(i) = "АСТ11"
        i = i + 1
        cn(i) = "АСТ12"
        i = i + 1
        cn(i) = "АСТ13"
        i = i + 1
        cn(i) = "АСТ14"
        i = i + 1
        cn(i) = "АСТ15"
        i = i + 1
        cn(i) = "АСТ16"
        i = i + 1
        cn(i) = "КТГ"
        i = i + 1
        cn(i) = "tп2"
        i = i + 1
        cn(i) = "tп3"
        i = i + 1
        cn(i) = "tп4"
        i = i + 1
        cn(i) = "tп5"
        i = i + 1
        cn(i) = "tо1"
        i = i + 1
        cn(i) = "tо2"
        i = i + 1
        cn(i) = "tо3"
        i = i + 1
        cn(i) = "tо4"
        i = i + 1
        cn(i) = "tо5"
        i = i + 1
        cn(i) = "КУ1"
        i = i + 1
        cn(i) = "УВ1"
        i = i + 1
        cn(i) = "УН1"
        i = i + 1
        cn(i) = "КУ2"
        i = i + 1
        cn(i) = "УВ2"
        i = i + 1
        cn(i) = "УН2"
        i = i + 1

        cn(i) = "КУ3"
        i = i + 1
        cn(i) = "УВ3"
        i = i + 1
        cn(i) = "УН3"
        i = i + 1
        cn(i) = "КУ4"
        i = i + 1
        cn(i) = "УВ4"
        i = i + 1
        cn(i) = "УН4"
        i = i + 1
        cn(i) = "КУ5"
        i = i + 1
        cn(i) = "УВ5"
        i = i + 1
        cn(i) = "УН5"
        i = i + 1
        cn(i) = "ПС"
        i = i + 1
        cn(i) = "ПМ"
        i = i + 1
        cn(i) = "PLG"
        i = i + 1
        cn(i) = "PPW"
        i = i + 1
        cn(i) = "AT1"
        i = i + 1
        cn(i) = "ОТВ1"
        i = i + 1
        cn(i) = "AT2"
        i = i + 1
        cn(i) = "ОТВ2"
        i = i + 1
        cn(i) = "AT3"
        i = i + 1
        cn(i) = "ОТВ3"
        i = i + 1
        cn(i) = "AT4"
        i = i + 1
        cn(i) = "ОТВ4"
        i = i + 1
        cn(i) = "AT5"
        i = i + 1
        cn(i) = "ОТВ5"
        i = i + 1
        cn(i) = "IP"
        i = i + 1
        cn(i) = "PORT"
        i = i + 1
        cn(i) = "SLG"
        i = i + 1
        cn(i) = "SPW"
        i = i + 1
        cn(i) = "Tka"



        dr = dt.NewRow
        dr("Название") = "Время прибора"
        dr("Значение") = d.ToString()
        dt.Rows.Add(dr)
      
        Dim lb As Integer
        Dim ub As Integer
        lb = 0
        ub = 30

        While lb + ub < 109
        msg = New messageM4(cmdM4.cmd_PARAM, 0)
            For i = lb To lb + ub
            block = New blockM4(TegM4.teg_PNUM, 3)
            block.data(0) = 0 ' chanel
            block.data(1) = i Mod 256
            block.data(2) = i \ 256
            msg.Tegs.Add(block)
        Next



        mID = NextID()

        barr = msg.BuildMessage(mID)

        EraseInputQueue()

        write(barr, barr.Length)
        WaitForData()


        i = MyRead(inbuf, 0, 1024, 2000)
        If i > 0 Then
            If CheckCRC16(inbuf, 1, i - 3) Then
                msg = ParseM4Message(inbuf)
                If msg.cmd = cmdM4.cmd_PARAM Then 'msg.ID = mID And
                        For i = 0 To msg.Tegs.Count - 1
                        block = msg.Tegs(i)
                        If block.teg = TegM4.teg_ASCIIString Then
                            dr = dt.NewRow
                                dr("Название") = cn(lb + i)

                            s = ""
                            For j = 0 To block.dl - 1
                                s = s + Chr(block.data(j))
                            Next
                            dr("Значение") = s
                            dt.Rows.Add(dr)
                        End If
                    Next
                End If
            End If
        End If

            lb = lb + ub + 1
            If lb > 109 Then Exit While

            If 109 - lb < 30 Then
                ub = 109 - lb
            Else
                ub = 30
            End If



        End While



        Return dt
    End Function

  




    Public Overrides Sub Connect()
        Dim i As Integer

        For i = 0 To 5
            If TryConnect() Then
                If BaudRate = 9600 Then
                    Set9600()
                End If
                Return ' True
            End If
        Next
        Return 'False

    End Sub



    Public Sub New()

    End Sub
End Class
