Imports STKTVMain
Imports System.IO
Imports System.Threading

Public Class Driver
    Inherits STKTVMain.TVDriver



    Private mIsConnected As Boolean


    Private Structure MArchive
        Public DateArch As DateTime
        Public 用 As Long
        Public Msg用 As String

        Public 用tv1 As Long
        Public Msg用tv1 As String

        Public 用tv2 As Long
        Public Msg用tv2 As String

        Public G1 As Single
        Public G2 As Single
        Public G3 As Single
        Public G4 As Single
        Public G5 As Single
        Public G6 As Single

        Public t1 As Single
        Public t2 As Single
        Public t4 As Single
        Public t5 As Single

        Public p1 As Single
        Public p2 As Single
        Public p3 As Single
        Public p4 As Single

        Public dt12 As Single
        Public dt45 As Single

        Public SP As Long
        Public SPtv1 As Long
        Public SPtv2 As Long


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

        Public archType As Short
    End Structure

    Private Structure Archive
        Public DateArch As DateTime

        Public 用 As Long
        Public Msg用 As String

        Public 用tv1 As Long
        Public Msg用tv1 As String

        Public 用tv2 As Long
        Public Msg用tv2 As String

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

        Public SP As Long
        Public SPtv1 As Long
        Public SPtv2 As Long

        Public T3 As Single
        Public T4 As Single
        Public P3 As Single
        Public P4 As Single
        Public v4 As Single
        Public v5 As Single
        Public v6 As Single
        Public M4 As Single
        Public M5 As Single
        Public M6 As Single

        Public archType As Short
    End Structure




    Dim V54 As Boolean = False
    Dim archType_hour = 3
    Dim archType_day = 4


    Dim Arch As Archive
    Dim mArch As MArchive
    Dim tArch As TArchive

    Dim WillCountToRead As Short = 0
    Dim IsBytesToRead As Boolean = False
    Dim pagesToRead As Short = 0
    Dim curtime As DateTime
    Dim IsmArchToRead As Boolean = False
    Dim ispackageError As Boolean = False
    Dim IsTArchToRead As Boolean = False

    Dim buffer(32767) As Byte
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
        Return "SPT941"
    End Function

  



    Public Function GetAndProcessData() As String
        Dim buf(69) As Byte
        Dim i As Int16
        For i = 0 To 69
            buf(i) = 0
        Next

        Dim ret As Long

        If (IsBytesToRead = False) Then
            Return ""
        End If

        Try
            ret = MyRead(buf, 0, WillCountToRead, 100)
            If (buf(2) = &H21) Then
                '
                EraseInputQueue()
                Return "恋葹袱. 菩� 銹葹褂:" + Hex(buf(3))
            End If
            If (ret > 0) Then
                If (ret = WillCountToRead) Then
                    If (ispackageError = True) Then

                        For i = bufferindex + 1 To bufferindex + ret
                            buffer(i) = buf(i - bufferindex - 1)
                        Next
                        If (pagesToRead < 2) Then IsBytesToRead = False
                        bufferindex = 0
                        For i = 0 To 69
                            buffer(i) = 0
                        Next
                        If (pagesToRead < 2) Then EraseInputQueue()
                        ispackageError = False
                        If V54 Then
                            Return writeMessage54(buffer, bufferindex)
                        End If
                        Return writeMessage(buffer, bufferindex)
                    End If
                    If (pagesToRead > 1) Then
                        pagesToRead = pagesToRead - 1
                        If V54 Then
                            Return writeMessage54(buf, ret)
                        End If
                        Return writeMessage(buf, ret)
                    End If
                    'tim.Stop()
                    IsBytesToRead = False
                    EraseInputQueue()
                    If V54 Then
                        Return writeMessage54(buf, ret)
                    End If
                    Return writeMessage(buf, ret)
                End If
                If (ret < WillCountToRead) Then
                    For i = bufferindex To bufferindex + ret - 1
                        buffer(i) = buf(i)
                    Next
                    ispackageError = True
                    WillCountToRead = WillCountToRead - ret
                    bufferindex = bufferindex + ret - 1
                End If
            End If
        Catch ex As Exception
            Return "恋葹袱." + ex.Message
        End Try
        Return ""
    End Function
    Public Overrides Sub Connect()



        Dim i As Integer

        For i = 0 To 5
            If TryConnect() Then
                Return ' True
            End If
        Next
        Return 'False
    End Sub

    Private Function TryConnect() As Boolean
        EraseInputQueue()

        Dim startBytes(0 To 20) As Byte
        Dim i As Int16


        If (IsBytesToRead = True) Then
            Return False
        End If
        For i = 0 To 18
            startBytes(i) = &HFF
        Next
        startBytes(19) = 0
        startBytes(20) = 0

        write(startBytes, 21)
        WaitForData()
        RaiseIdle()

        Dim bArr(0 To 8) As Byte
        Try

            bArr(0) = &H10
            bArr(1) = &HFF
            bArr(2) = &H3F
            bArr(3) = &H0
            bArr(4) = &H0
            bArr(5) = &H0
            bArr(6) = &H0
            bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
            bArr(8) = &H16


            EraseInputQueue()
            WillCountToRead = 8
            IsBytesToRead = True
            write(bArr, 9)


            Dim sret As String
            WaitForData()
            sret = GetAndProcessData()
            If (sret.Length > 5) Then
                If (sret.Substring(0, 6) = "恋葹袱") Then
                    EraseInputQueue()
                    Return False
                End If
                If sret.IndexOf(" 54 ") >= 8 Then
                    V54 = True
                    Debug.Print("old version 941")
                End If
                If sret.IndexOf(" 92 ") >= 8 Then
                    V54 = False
                    Debug.Print("new version 941")
                End If


                mIsConnected = True
                Return True
            End If

        Catch exc As Exception
            Return False
        End Try

    End Function

    Public Function ReadFlashSync(ByVal fistpage As Integer, ByVal ReadPageCount As Integer) As String
        pagesToRead = 0
        Dim bArr(0 To 8) As Byte
        Dim buf(8000) As Byte



        Try
            If (fistpage < 0 Or fistpage > 3071) Then
                'MsgBox("羅矼齏�� 邇跂� 閻鞨鉗 髣蓿�矗繻鉗 髓鞐辷��", MsgBoxStyle.OkOnly, "ReadFlash")
                Return ""
            End If
            If (ReadPageCount < 1 Or ReadPageCount > 64) Then
                'MsgBox("羅矼齏鈬 褌謌�繿鰲� 髣蓿�矗繻�� 髓鞐辷�", MsgBoxStyle.OkOnly, "ReadFlash")
                Return ""
            End If
        Catch ew As Exception
            'MsgBox("羅矼齏�� 閠鞐跂鴃� �鱚辷� FLASH-閠��鱶", MsgBoxStyle.OkOnly, "ReadFlash")
            Return ""
        End Try

        EraseInputQueue()

        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(2) = &H45
        bArr(3) = fistpage Mod 256
        bArr(4) = fistpage \ 256
        bArr(5) = ReadPageCount
        bArr(6) = &H0
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16

        WillCountToRead = ReadPageCount * 64

        write(bArr, 9)
        WaitForData()


        Dim T As DateTime
        Dim ret As Integer
        Dim i As Integer
        T = DateTime.Now

        bufferindex = 0
        While (bufferindex < ReadPageCount * 64 + 5)
            RaiseIdle()
            ret = MyRead(buf, 0, ReadPageCount * 64 + 5 - bufferindex, 100)
            For i = 0 To ret - 1
                buffer(bufferindex + i) = buf(i)
            Next
            If T.AddSeconds(2) < DateTime.Now Then
                Return ""
            End If
            bufferindex = bufferindex + ret
        End While



        Dim sout As String

        sout = ""
        For i = 3 To bufferindex - 3
            sout = sout & Chr(buffer(i))
        Next
        Return sout


    End Function

   
    Private m_readRAMByteCount As Short

    Public Function ReadRAMSync(ByVal fistbyte As Integer, ByVal byteCount As Integer) As String
        Dim buf(8000) As Byte
        Dim bArr(0 To 8) As Byte
        'm_readRAMByteCount = byteCount
        Try
            If (fistbyte < 0 Or fistbyte > 1023) Then
                'MsgBox("羅矼齏�� 珞鞳� 閻鞨釿� 髣蓿�矗繻釿� �諷跂迺�", MsgBoxStyle.OkOnly, "ReadRAM")
                Return ""
            End If
            If (byteCount < 1 Or byteCount > 64) Then
                'MsgBox("羅矼齏鈬 褌謌�繿鰲� 髣蓿�矗繻�� 痼蜥鈞", MsgBoxStyle.OkOnly, "ReadRAM")
                Return ""
            End If
        Catch ew As Exception
            'MsgBox("羅矼齏�� 閠鞐跂鴃� �鱚辷� 稜�", MsgBoxStyle.OkOnly, "ReadRAM")
            Return ""
        End Try

        EraseInputQueue()

        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(2) = &H52
        bArr(3) = fistbyte Mod 256
        bArr(4) = fistbyte \ 256
        bArr(5) = byteCount
        bArr(6) = &H0
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16


        write(bArr, 9)

        WaitForData()

        Dim T As DateTime
        Dim ret As Integer
        Dim i As Integer
        T = DateTime.Now

        bufferindex = 0
        While (bufferindex < byteCount + 5)
            ret = MyRead(buf, (0), byteCount + 5 - bufferindex, 100)
            For i = 0 To ret - 1
                buffer(bufferindex + i) = buf(i)
            Next
            If T.AddSeconds(2) < DateTime.Now Then
                Return ""
            End If
            bufferindex = bufferindex + ret
        End While
        Dim sout As String

        sout = ""
        For i = 3 To bufferindex - 3
            sout = sout & Chr(buffer(i))
        Next
        Return sout

    End Function


    Public Sub ReadRAM(ByVal fistbyte As Integer, ByVal byteCount As Integer)
        If (IsBytesToRead = True) Then
            Return
        End If
        Dim bArr(0 To 8) As Byte
        m_readRAMByteCount = byteCount
        Try
            If (fistbyte < 0 Or fistbyte > 1023) Then
                'MsgBox("羅矼齏�� 珞鞳� 閻鞨釿� 髣蓿�矗繻釿� �諷跂迺�", MsgBoxStyle.OkOnly, "ReadRAM")
                Return
            End If
            If (byteCount < 1 Or byteCount > 64) Then
                'MsgBox("羅矼齏鈬 褌謌�繿鰲� 髣蓿�矗繻�� 痼蜥鈞", MsgBoxStyle.OkOnly, "ReadRAM")
                Return
            End If
        Catch ew As Exception
            'MsgBox("羅矼齏�� 閠鞐跂鴃� �鱚辷� 稜�", MsgBoxStyle.OkOnly, "ReadRAM")
            Return
        End Try
        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(2) = &H52
        bArr(3) = fistbyte Mod 256
        bArr(4) = fistbyte \ 256
        bArr(5) = byteCount
        bArr(6) = &H0
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16

        WillCountToRead = m_readRAMByteCount + 5
        IsBytesToRead = True

        write(bArr, 9)
    End Sub
    Public Overrides Function ReadArch(ByVal ArchType As Short, ByVal ArchYear As Short, _
    ByVal ArchMonth As Short, ByVal ArchDay As Short, ByVal ArchHour As Short) As String
        If V54 Then Return ReadArch54(ArchType, ArchYear, ArchMonth, ArchDay, ArchHour)
        If (IsBytesToRead = True) Then
            Return ""
        End If
        cleararchive(Arch)
        EraseInputQueue()
        Dim bArr(0 To 8) As Byte
        Dim ret As String = ""
        Dim retsum As String = ""
        Dim trycnt As Int32
        Dim tv1OK As Boolean
        Arch.Msg用 = ""



        trycnt = 5
tryagain1:

        If (ArchType = archType_hour) Then
            bArr(2) = &H48
            bArr(3) = ArchYear - 1900
            bArr(4) = ArchMonth Mod 13
            bArr(5) = ArchDay Mod 32
            bArr(6) = ArchHour Mod 24
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)
            Arch.DateArch = Arch.DateArch.AddSeconds(-1)
        End If

        If (ArchType = archType_day) Then
            bArr(2) = &H59
            bArr(3) = ArchYear - 1900
            bArr(4) = ArchMonth Mod 13
            bArr(5) = ArchDay Mod 32
            bArr(6) = &H0
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, 0, 0, 0)
            Arch.DateArch = Arch.DateArch.AddSeconds(-1)
        End If



        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16
        tv1OK = True

        WillCountToRead = 69
        IsBytesToRead = True


        write(bArr, 9)
        WaitForData()
        ret = GetAndProcessData()
        If (ret.Length > 5) Then
            If (ret.Substring(0, 6) = "恋葹袱") Then
                retsum = retsum + ret
                If trycnt = 0 Then
                    'Return retsum
                    trycnt = 5
                    GoTo finalRet
                Else
                    trycnt -= 1
                    GoTo tryagain1
                End If
            Else
                tv1OK = True
            End If
        End If
        If (ret.Length = 0) Then
            EraseInputQueue()
            retsum = retsum & vbCrLf & "恋葹袱 �鱚辷� 瑁�萵�"
            tv1OK = False

        End If




finalRet:
        If tv1OK Then
            retsum = "扇�萵 關鋏蓿琿" & vbCrLf & retsum
            retsum = retsum & vbCrLf
            EraseInputQueue()
            isArchToDBWrite = True
            Return retsum
        Else
            retsum = "恋葹袱 �鱚辷�" & vbCrLf & retsum
            retsum = retsum & vbCrLf
            EraseInputQueue()
            Return retsum
        End If

    End Function



    Private Function ReadArch54(ByVal ArchType As Short, ByVal ArchYear As Short, _
  ByVal ArchMonth As Short, ByVal ArchDay As Short, ByVal ArchHour As Short) As String
        If (IsBytesToRead = True) Then
            Return ""
        End If
        cleararchive(Arch)
        EraseInputQueue()
        Dim bArr(0 To 8) As Byte
        Dim ret As String = ""
        Dim retsum As String = ""
        Dim trycnt As Int32
        Dim tv1OK As Boolean
        Arch.Msg用 = ""



        trycnt = 5
tryagain1:

        If (ArchType = archType_hour) Then
            bArr(2) = &H48
            bArr(3) = ArchYear - 1900
            bArr(4) = ArchMonth Mod 13
            bArr(5) = ArchDay Mod 32
            bArr(6) = ArchHour Mod 24
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, ArchHour, 0, 0)
            Arch.DateArch = Arch.DateArch.AddSeconds(-1)
        End If

        If (ArchType = archType_day) Then
            bArr(2) = &H59
            bArr(3) = ArchYear - 1900
            bArr(4) = ArchMonth Mod 13
            bArr(5) = ArchDay Mod 32
            bArr(6) = &H0
            Arch.DateArch = New DateTime(ArchYear, ArchMonth, ArchDay, 0, 0, 0)
            Arch.DateArch = Arch.DateArch.AddSeconds(-1)
        End If



        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16
        tv1OK = True

        WillCountToRead = 69
        IsBytesToRead = True


        write(bArr, 9)
        WaitForData()
        ret = GetAndProcessData()
        If (ret.Length > 5) Then
            If (ret.Substring(0, 6) = "恋葹袱") Then
                retsum = retsum + ret
                If trycnt = 0 Then
                    'Return retsum
                    trycnt = 5
                    GoTo finalRet
                Else
                    trycnt -= 1
                    GoTo tryagain1
                End If
            Else
                tv1OK = True
            End If
        End If
        If (ret.Length = 0) Then
            EraseInputQueue()
            retsum = retsum & vbCrLf & "恋葹袱 �鱚辷� 瑁�萵�"
            tv1OK = False

        End If







finalRet:
        If tv1OK Then
            retsum = "扇�萵 關鋏蓿琿" & vbCrLf & retsum
            retsum = retsum & vbCrLf
            EraseInputQueue()
            isArchToDBWrite = True
            Return retsum
        Else
            retsum = "恋葹袱 �鱚辷�" & vbCrLf & retsum
            retsum = retsum & vbCrLf
            EraseInputQueue()
            Return retsum
        End If

    End Function



    Public Function writeMessage(ByVal buf() As Byte, ByVal ret As Short) As String
        Dim retstring As String = ""
        Dim KC As Long = 0
        Try

            If (buf(2) = &H3F) Then '齣鰰邇礪� 驍�艾
                Dim i As Integer
                For i = 0 To 7
                    retstring += Hex(buf(i)) + " "
                Next
                KC = 0
                KC = 255 - ((Int(buf(1)) + Int(buf(2)) + _
                    Int(buf(3)) + Int(buf(4)) + Int(buf(5))) Mod 256)
                retstring += vbCrLf
                If (KC <> buf(6)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                Return retstring
            End If
            If (buf(2) = &H45) Then '�鱚辷� Flash-閠��鱶
                Dim i As Integer
                For i = 0 To 68
                    retstring += Hex(buf(i)) + " "
                Next
                retstring += vbCrLf
                KC = 0
                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                Return retstring
            End If

            If (buf(2) = &H52 And IsTArchToRead = True) Then '�鱚辷� 鴈鰰譛邇竡 瑁�萵�
                IsTArchToRead = False
                Dim i As Integer
                Dim str As String = ""
                'If (tv = 1 Or tv = 2) Then m_readRAMByteCount = 36
                'If (tv = 3) Then m_readRAMByteCount = 6
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                tArch.archType = 2




                Return "瑁�萵 關鋏蓿琿"
            End If



            If (buf(2) = &H52 And IsmArchToRead = True) Then '�鱚辷� 趁邇矼迯釿� 瑁�萵�
                IsmArchToRead = False
                Dim i As Integer
                Dim str As String = ""
                'If (tv = 1 Or tv = 2) Then
                m_readRAMByteCount = 36
                'End If

                'If (tv = 3) Then m_readRAMByteCount = 6
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                'If (tv = 1 Or tv = 2) Then
                For i = 3 To 39
                    str = str + Chr(buf(i))
                Next
                'End If
                mArch.archType = 1
                Dim Adr As Long
                Adr = 1


                mArch.用tv1 = Asc(Mid(str, Adr + 2, 1)) * 256& ^ 2 + Asc(Mid(str, Adr + 1, 1)) * 256& + Asc(Mid(str, Adr, 1))
                mArch.Msg用tv1 = DeCodeHC(mArch.用tv1)
                mArch.SPtv1 = Asc(Mid(str, Adr + 3, 1))
                mArch.G1 = FloatExt(Mid(str, Adr + 4, 4))
                mArch.G2 = FloatExt(Mid(str, Adr + 4 * 2, 4))
                mArch.G3 = FloatExt(Mid(str, Adr + 4 * 3, 4))
                mArch.t1 = FloatExt(Mid(str, Adr + 4 * 4, 4))
                mArch.t2 = FloatExt(Mid(str, Adr + 4 * 5, 4))
                mArch.dt12 = FloatExt(Mid(str, Adr + 4 * 6, 4))

                Return "瑁�萵 關鋏蓿琿"
            End If
            If (buf(2) = &H52 And IsmArchToRead = False) Then '�鱚辷� 稜�
                Dim i As Integer
                For i = 0 To 4 + m_readRAMByteCount
                    retstring += Hex(buf(i)) + " "
                Next
                retstring += vbCrLf
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                Return retstring
            End If
            If (buf(2) = &H48) Then '�瑜鈞鉗 瑁�萵
                'If (tv = 0) Then Return ""
                Dim hourstr As String = ""
                Dim i As Int32
                KC = 0
                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                For i = 3 To 64
                    hourstr = hourstr + Chr(buf(i))
                Next
                Arch.archType = archType_hour
                Dim Adr As Long
                Adr = 1

                Arch.用tv1 = Asc(Mid(hourstr, Adr + 2, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr, 1))
                Arch.Msg用tv1 = DeCodeHC(Arch.用tv1)
                Arch.SPtv1 = Asc(Mid(hourstr, Adr + 3, 1))
                Arch.T1 = FloatExt(Mid(hourstr, Adr + 4, 4))
                Arch.T2 = FloatExt(Mid(hourstr, Adr + 4 * 2, 4))
                Arch.V1 = FloatExt(Mid(hourstr, Adr + 4 * 3, 4))
                Arch.V2 = FloatExt(Mid(hourstr, Adr + 4 * 4, 4))
                Arch.V3 = FloatExt(Mid(hourstr, Adr + 4 * 5, 4))
                Arch.M1 = FloatExt(Mid(hourstr, Adr + 4 * 6, 4))
                Arch.M2 = FloatExt(Mid(hourstr, Adr + 4 * 7, 4))
                Arch.M3 = FloatExt(Mid(hourstr, Adr + 4 * 8, 4))
                Arch.Q1 = FloatExt(Mid(hourstr, Adr + 4 * 9, 4))
                Arch.Tw1 = FloatExt(Mid(hourstr, Adr + 4 * 10, 4))

                m_isArchToDBWrite = True
                'Arch.DateArch
                Return "瑁�萵 關鋏蓿琿"
            End If
            If (buf(2) = &H59) Then '體銜�逶� 瑁�萵
                'If (tv = 0) Then Return ""
                Dim hourstr As String = ""
                Dim i As Int32

                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                For i = 3 To 64
                    hourstr = hourstr + Chr(buf(i))
                Next

                'hourstr = buf.ToString
                Arch.archType = archType_day
                Dim Adr As Long
                Adr = 1
                'If (tv = 1) Then
                'Arch.用tv1 = Asc(Mid(hourstr, Adr, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr + 2, 1))
                Arch.用tv1 = Asc(Mid(hourstr, Adr + 2, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr, 1))
                Arch.Msg用tv1 = DeCodeHC(Arch.用tv1)
                Arch.SPtv1 = Asc(Mid(hourstr, Adr + 3, 1))
                Arch.T1 = FloatExt(Mid(hourstr, Adr + 4, 4))
                Arch.T2 = FloatExt(Mid(hourstr, Adr + 4 * 2, 4))
                Arch.V1 = FloatExt(Mid(hourstr, Adr + 4 * 3, 4))
                Arch.V2 = FloatExt(Mid(hourstr, Adr + 4 * 4, 4))
                Arch.V3 = FloatExt(Mid(hourstr, Adr + 4 * 5, 4))
                Arch.M1 = FloatExt(Mid(hourstr, Adr + 4 * 6, 4))
                Arch.M2 = FloatExt(Mid(hourstr, Adr + 4 * 7, 4))
                Arch.M3 = FloatExt(Mid(hourstr, Adr + 4 * 8, 4))
                Arch.Q1 = FloatExt(Mid(hourstr, Adr + 4 * 9, 4))
                Arch.Tw1 = FloatExt(Mid(hourstr, Adr + 4 * 10, 4))

                
                m_isArchToDBWrite = True
                'Arch.DateArch = DateTime.Now
                Return "扇�萵 關鋏蓿琿"
            End If
            'MsgBox("�琲纈 鞐髀鈑轢� 辣褌韶繩鴉�!", MsgBoxStyle.OkOnly, "恋葹袱")
            retstring = "恋葹袱"
            Return retstring
        Catch exc As Exception
        End Try
        Return "恋葹袱!�琲纈 辣 鞐髀鈑轢�"
    End Function



    Public Function writeMessage54(ByVal buf() As Byte, ByVal ret As Short) As String
        Dim retstring As String = ""
        Dim KC As Long = 0
        Try

            If (buf(2) = &H3F) Then '齣鰰邇礪� 驍�艾
                Dim i As Integer
                For i = 0 To 7
                    retstring += Hex(buf(i)) + " "
                Next
                KC = 0
                KC = 255 - ((Int(buf(1)) + Int(buf(2)) + _
                    Int(buf(3)) + Int(buf(4)) + Int(buf(5))) Mod 256)
                retstring += vbCrLf
                If (KC <> buf(6)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                Return retstring
            End If
            If (buf(2) = &H45) Then '�鱚辷� Flash-閠��鱶
                Dim i As Integer
                For i = 0 To 68
                    retstring += Hex(buf(i)) + " "
                Next
                retstring += vbCrLf
                KC = 0
                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                Return retstring
            End If

            If (buf(2) = &H52 And IsTArchToRead = True) Then '�鱚辷� 鴈鰰譛邇竡 瑁�萵�
                IsTArchToRead = False
                Dim i As Integer
                Dim str As String = ""
                'If (tv = 1 Or tv = 2) Then m_readRAMByteCount = 36
                'If (tv = 3) Then m_readRAMByteCount = 6
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                tArch.archType = 2




                Return "瑁�萵 關鋏蓿琿"
            End If



            If (buf(2) = &H52 And IsmArchToRead = True) Then '�鱚辷� 趁邇矼迯釿� 瑁�萵�
                IsmArchToRead = False
                Dim i As Integer
                Dim str As String = ""

                m_readRAMByteCount = 8
                
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                'If (tv = 1 Or tv = 2) Then
                For i = 3 To 11
                    str = str + Chr(buf(i))
                Next
                'End If
                mArch.archType = 1
                Dim Adr As Long
                Adr = 1

                mArch.t1 = FloatExt(Mid(str, Adr + 4 * 0, 4))
                mArch.t2 = FloatExt(Mid(str, Adr + 4 * 1, 4))


                Return "瑁�萵 關鋏蓿琿"
            End If
            If (buf(2) = &H52 And IsmArchToRead = False) Then '�鱚辷� 稜�
                Dim i As Integer
                For i = 0 To 4 + m_readRAMByteCount
                    retstring += Hex(buf(i)) + " "
                Next
                retstring += vbCrLf
                KC = 0
                For i = 1 To 2 + m_readRAMByteCount
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(3 + m_readRAMByteCount)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If
                Return retstring
            End If
            If (buf(2) = &H48) Then '�瑜鈞鉗 瑁�萵
                'If (tv = 0) Then Return ""
                Dim hourstr As String = ""
                Dim i As Int32
                KC = 0
                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                For i = 3 To 64
                    hourstr = hourstr + Chr(buf(i))
                Next
                Arch.archType = archType_hour
                Dim Adr As Long
                Adr = 1

                'Arch.用tv1 = Asc(Mid(hourstr, Adr + 2, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr, 1))
                'Arch.Msg用tv1 = DeCodeHC(Arch.用tv1)
                Arch.SPtv1 = Asc(Mid(hourstr, Adr, 1))
                Arch.用tv1 = Asc(Mid(hourstr, Adr + 1, 1))
                Arch.Msg用tv1 = DeCodeHC(Arch.用tv1)
                Arch.T1 = FloatExt(Mid(hourstr, Adr + 4 * 1, 4))
                Arch.T2 = FloatExt(Mid(hourstr, Adr + 4 * 2, 4))
                Arch.V1 = FloatExt(Mid(hourstr, Adr + 4 * 3, 4))
                Arch.V2 = FloatExt(Mid(hourstr, Adr + 4 * 4, 4))
                'Arch.V3 = FloatExt(Mid(hourstr, Adr + 4 * 5, 4))
                Arch.M1 = FloatExt(Mid(hourstr, Adr + 4 * 5, 4))
                Arch.M2 = FloatExt(Mid(hourstr, Adr + 4 * 6, 4))
                'Arch.M3 = FloatExt(Mid(hourstr, Adr + 4 * 8, 4))
                Arch.Q1 = FloatExt(Mid(hourstr, Adr + 4 * 7, 4))
                'Arch.Tw1 = FloatExt(Mid(hourstr, Adr + 4 * 10, 4))

                m_isArchToDBWrite = True
                'Arch.DateArch
                Return "瑁�萵 關鋏蓿琿"
            End If
            If (buf(2) = &H59) Then '體銜�逶� 瑁�萵
                'If (tv = 0) Then Return ""
                Dim hourstr As String = ""
                Dim i As Int32

                For i = 1 To 66
                    KC = (KC + Int(buf(i))) Mod 256
                Next
                KC = 255 - KC
                If (KC <> buf(67)) Then
                    Return "恋葹袱!菩迺韲譛轢� 體跛� 辣 骼硼琺�!" ', MsgBoxStyle.OkOnly, "菩迺韲譛轢� 體跛�")
                    'Return ""
                End If

                For i = 3 To 64
                    hourstr = hourstr + Chr(buf(i))
                Next

                'hourstr = buf.ToString
                Arch.archType = archType_day
                Dim Adr As Long
                Adr = 1
                'If (tv = 1) Then
                'Arch.用tv1 = Asc(Mid(hourstr, Adr, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr + 2, 1))
                Arch.用tv1 = Asc(Mid(hourstr, Adr + 2, 1)) * 256& ^ 2 + Asc(Mid(hourstr, Adr + 1, 1)) * 256& + Asc(Mid(hourstr, Adr, 1))
                Arch.Msg用tv1 = DeCodeHC(Arch.用tv1)
                Arch.SPtv1 = Asc(Mid(hourstr, Adr + 3, 1))
                Arch.T1 = FloatExt(Mid(hourstr, Adr + 4, 4))
                Arch.T2 = FloatExt(Mid(hourstr, Adr + 4 * 2, 4))
                Arch.V1 = FloatExt(Mid(hourstr, Adr + 4 * 3, 4))
                Arch.V2 = FloatExt(Mid(hourstr, Adr + 4 * 4, 4))
                Arch.V3 = FloatExt(Mid(hourstr, Adr + 4 * 5, 4))
                Arch.M1 = FloatExt(Mid(hourstr, Adr + 4 * 6, 4))
                Arch.M2 = FloatExt(Mid(hourstr, Adr + 4 * 7, 4))
                Arch.M3 = FloatExt(Mid(hourstr, Adr + 4 * 8, 4))
                Arch.Q1 = FloatExt(Mid(hourstr, Adr + 4 * 9, 4))
                Arch.Tw1 = FloatExt(Mid(hourstr, Adr + 4 * 10, 4))


                m_isArchToDBWrite = True
                'Arch.DateArch = DateTime.Now
                Return "扇�萵 關鋏蓿琿"
            End If
            'MsgBox("�琲纈 鞐髀鈑轢� 辣褌韶繩鴉�!", MsgBoxStyle.OkOnly, "恋葹袱")
            retstring = "恋葹袱"
            Return retstring
        Catch exc As Exception
        End Try
        Return "恋葹袱!�琲纈 辣 鞐髀鈑轢�"
    End Function


    'Public Function bufcheck() As String
    '    Dim buf(69) As Byte
    '    Dim i As Int16
    '    For i = 0 To 69
    '        buf(i) = 0
    '    Next

    '    Dim ret As Long

    '    If (IsBytesToRead = False) Then
    '        Return ""
    '    End If

    '    Try
    '        ret = nsio_read(m_RetPortID, buf(0), WillCountToRead)
    '        If (buf(2) = &H21) Then
    '            tim.Stop()
    '            EraseInputQueue()
    '            Return "恋葹袱. 菩� 銹葹褂:" + Hex(buf(3))
    '        End If
    '        If (ret > 0) Then
    '            If (ret = WillCountToRead) Then
    '                If (ispackageError = True) Then
    '                    tim.Stop()
    '                    For i = bufferindex + 1 To bufferindex + ret
    '                        buffer(i) = buf(i - bufferindex - 1)
    '                    Next
    '                    If (pagesToRead < 2) Then IsBytesToRead = False
    '                    bufferindex = 0
    '                    For i = 0 To 69
    '                        buffer(i) = 0
    '                    Next
    '                    If (pagesToRead < 2) Then EraseInputQueue()
    '                    ispackageError = False
    '                    Return writeMessage(buffer, bufferindex)
    '                End If
    '                If (pagesToRead > 1) Then
    '                    pagesToRead = pagesToRead - 1
    '                    Return writeMessage(buf, ret)
    '                End If
    '                tim.Stop()
    '                IsBytesToRead = False
    '                EraseInputQueue()
    '                Return writeMessage(buf, ret)
    '            End If
    '            If (ret < WillCountToRead) Then
    '                For i = bufferindex To bufferindex + ret - 1
    '                    buffer(i) = buf(i)
    '                Next
    '                ispackageError = True
    '                WillCountToRead = WillCountToRead - ret
    '                bufferindex = bufferindex + ret - 1
    '            End If
    '        End If
    '    Catch ex As Exception
    '        Return "恋葹袱." + ex.Message
    '    End Try
    '    Return ""
    'End Function


    Private Function ExtLong4(ByVal extStr As String) As Double
        Dim i As Long
        On Error Resume Next
        ExtLong4 = 0
        For i = 0 To 3
            ExtLong4 = ExtLong4 + Asc(Mid(extStr, 1 + i, 1)) * (256 ^ (i))
        Next i
    End Function
    'Public Function DeCodeHCNumber(ByVal CodeHC As Long, ByVal tv As Int32) As String

    '    DeCodeHCNumber = ""
    '    'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
    '    If CodeHC And 2 ^ 0 Then
    '        DeCodeHCNumber = "TB:" + tv.ToString + "用:0" + ";"
    '    End If

    '    If CodeHC And 2 ^ 1 Then
    '        DeCodeHCNumber = DeCodeHCNumber + "TB:" + tv.ToString + "用:1" + ";"
    '    End If


    '    '''''        If CodeHC And 2 ^ 2 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:2 - �辮繝頌芒� 闔 �繽�� 闊鰰辷� 籥鵄蒻鈞 籥硅纃�� (鴈譛褌 粳� 跪粤謌 02)" +";"
    '    '''''        End If
    '    '''''
    '    '''''        If CodeHC And 2 ^ 3 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:3 - 占鱶硴�� 齔鈞纃� 驤竝琺� 轢 粫驫鞳鴉鉤 碯鈔� D2" +";"
    '    '''''        End If
    '    '''''
    '    '''''        If CodeHC And 2 ^ 4 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:4 - 刪竝琺 Qp 闔 袱轢謫 �1 跂逵�� 辷肭繝� 關繖繼� " +";"
    '    '''''        End If
    '    '''''
    '    '''''        If CodeHC And 2 ^ 5 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:5 - 刪竝琺 Qp 闔 袱轢謫 �2 跂逵�� 辷肭繝� 關繖繼� " +";"
    '    '''''        End If
    '    '''''
    '    '''''        If CodeHC And 2 ^ 6 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:6 - 刪竝琺 Qp 闔 袱轢謫 �1 碚繧�驤� 矼頤辷� 關繖繼� " +";"
    '    '''''        End If
    '    '''''
    '    '''''        If CodeHC And 2 ^ 7 Then
    '    '''''            DeCodeHCNumber = DeCodeHCNumber _
    '    '''''                    & "用:7 - 刪竝琺 Qp 闔 袱轢謫 �2 碚繧�驤� 矼頤辷� 關繖繼� " +";"
    '    '''''        End If

    '    If CodeHC And 2 ^ 8 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:8" + ";"
    '    End If

    '    If CodeHC And 2 ^ 9 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:9" + ";"
    '    End If

    '    If CodeHC And 2 ^ 10 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:10" + ";"
    '    End If

    '    If CodeHC And 2 ^ 11 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:11 " + ";"
    '    End If

    '    If CodeHC And 2 ^ 12 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:12" + ";"
    '    End If

    '    If CodeHC And 2 ^ 13 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:13" + ";"
    '    End If

    '    If CodeHC And 2 ^ 14 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:14" + ";"
    '    End If

    '    If CodeHC And 2 ^ 15 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:15" + ";"
    '    End If

    '    If CodeHC And 2 ^ 16 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:16" + ";"
    '    End If

    '    If CodeHC And 2 ^ 17 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:17 " + ";"
    '    End If

    '    If CodeHC And 2 ^ 18 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:18" + ";"
    '    End If

    '    If CodeHC And 2 ^ 19 Then
    '        DeCodeHCNumber = DeCodeHCNumber _
    '                + "TB:" + tv.ToString + "用:19" + ";"
    '    End If
    'End Function

    Public Function DeCodeHCNumber(ByVal CodeHC As Long, ByVal tv As Int32) As String

        DeCodeHCNumber = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHCNumber = "TB" + tv.ToString + ":用00" + ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用01" + ";"
        End If

        If CodeHC And 2 ^ 2 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用02" + ";"
        End If
        If CodeHC And 2 ^ 3 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用03" + ";"
        End If
        If CodeHC And 2 ^ 4 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用04" + ";"
        End If
        If CodeHC And 2 ^ 5 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用05" + ";"
        End If
        If CodeHC And 2 ^ 6 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用06" + ";"
        End If
        If CodeHC And 2 ^ 7 Then
            DeCodeHCNumber = DeCodeHCNumber + "TB" + tv.ToString + ":用07" + ";"
        End If



        If CodeHC And 2 ^ 8 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用08" + ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用09" + ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用10" + ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用11 " + ";"
        End If

        If CodeHC And 2 ^ 12 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用12" + ";"
        End If

        If CodeHC And 2 ^ 13 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用13" + ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用14" + ";"
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用15" + ";"
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用16" + ";"
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用17 " + ";"
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用18" + ";"
        End If

        If CodeHC And 2 ^ 19 Then
            DeCodeHCNumber = DeCodeHCNumber _
                    + "TB" + tv.ToString + ":用19" + ";"
        End If
    End Function

    Public Function DeCodeHCText(ByVal CodeHC As Long) As String

        DeCodeHCText = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHCText = DeCodeHCText _
                   & "俥苣�� 痼鰰鞳�" + ";"
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHCText = DeCodeHCText _
                   & "�辮繝頌芒� 闊鰰辷�" + ";"
        End If


        If CodeHC And 2 ^ 3 Then
            DeCodeHCText = DeCodeHCText _
                    & "占鱶硴�� 齔鈞纃� 驤竝琺� 轢 粫驫鞳鴉鉤 碯鈔� D2" & ";"
        End If

        If CodeHC And 2 ^ 4 Then
            DeCodeHCText = DeCodeHCText _
                    & "刪竝琺 Qp 闔 袱轢謫 �1 跂逵�� 辷肭繝� 關繖繼� " & ";"
        End If

        If CodeHC And 2 ^ 5 Then
            DeCodeHCText = DeCodeHCText _
                    & "刪竝琺 Qp 闔 袱轢謫 �2 跂逵�� 辷肭繝� 關繖繼� " & ";"
        End If

        If CodeHC And 2 ^ 6 Then
            DeCodeHCText = DeCodeHCText _
                    & "刪竝琺 Qp 闔 袱轢謫 �1 碚繧�驤� 矼頤辷� 關繖繼� " & ";"
        End If

        If CodeHC And 2 ^ 7 Then
            DeCodeHCText = DeCodeHCText _
                    & "刪竝琺 Qp 闔 袱轢謫 �2 碚繧�驤� 矼頤辷� 關繖繼� " & ";"
        End If

        If CodeHC And 2 ^ 8 Then
            DeCodeHCText = DeCodeHCText _
                    & "P1 硴� 0-1.1堆1" + ";"
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHCText = DeCodeHCText _
                    & "P2 硴� 0-1.1堆1" + ";"
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHCText = DeCodeHCText _
                    & "T1 硴� 0-176竦.�" + ";"
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHCText = DeCodeHCText _
                    & "T2 硴� 0-176竦.�" + ";"
        End If

        If CodeHC And 2 ^ 12 Then
            DeCodeHCText = DeCodeHCText _
                    & "G1>G�1" + ";"
        End If

        If CodeHC And 2 ^ 13 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G1<G�1" + ";"
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHCText = DeCodeHCText _
                    & "G2>G�2" + ";"
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G2>G�2" + ";"
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHCText = DeCodeHCText _
                    & "G3>G�3" + ";"
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHCText = DeCodeHCText _
                    & "0<G3>G�3" + ";"
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHCText = DeCodeHCText _
                    & "M3� < -0.04M1" + ";"
        End If

        If CodeHC And 2 ^ 19 Then
            DeCodeHCText = DeCodeHCText _
                    & "Q� < 0" + ";"
        End If
        If DeCodeHCText = "" Then
            DeCodeHCText = "羅� 用"
        End If
    End Function
    Public Function DeCodeHC(ByVal CodeHC As Long) As String

        DeCodeHC = ""
        'CodeHC = CodeHC And ( 2 ^ 5 + 2 ^ 4 + 2 ^ 3 + 2 ^ 2 + 2 ^ 1 + 2 ^ 0)
        If CodeHC And 2 ^ 0 Then
            DeCodeHC = "用:0 - 俥苣�� 痼鰰鞳�" & vbCrLf
        End If

        If CodeHC And 2 ^ 1 Then
            DeCodeHC = DeCodeHC _
                    & "用:1 - �辮繝頌芒� 闊鰰辷�" & vbCrLf
        End If


        If CodeHC And 2 ^ 2 Then
            DeCodeHC = DeCodeHC _
                    & "用:2 - �辮繝頌芒� 闔 �繽�� 闊鰰辷� 籥鵄蒻鈞 籥硅纃�� (鴈譛褌 粳� 跪粤謌 02)" & vbCrLf
        End If

        If CodeHC And 2 ^ 3 Then
            DeCodeHC = DeCodeHC _
                    & "用:3 - 占鱶硴�� 齔鈞纃� 驤竝琺� 轢 粫驫鞳鴉鉤 碯鈔� D2" & vbCrLf
        End If

        If CodeHC And 2 ^ 4 Then
            DeCodeHC = DeCodeHC _
                    & "用:4 - 刪竝琺 Qp 闔 袱轢謫 �1 跂逵�� 辷肭繝� 關繖繼� " & vbCrLf
        End If

        If CodeHC And 2 ^ 5 Then
            DeCodeHC = DeCodeHC _
                    & "用:5 - 刪竝琺 Qp 闔 袱轢謫 �2 跂逵�� 辷肭繝� 關繖繼� " & vbCrLf
        End If

        If CodeHC And 2 ^ 6 Then
            DeCodeHC = DeCodeHC _
                    & "用:6 - 刪竝琺 Qp 闔 袱轢謫 �1 碚繧�驤� 矼頤辷� 關繖繼� " & vbCrLf
        End If

        If CodeHC And 2 ^ 7 Then
            DeCodeHC = DeCodeHC _
                    & "用:7 - 刪竝琺 Qp 闔 袱轢謫 �2 碚繧�驤� 矼頤辷� 關繖繼 " & vbCrLf
        End If

        If CodeHC And 2 ^ 8 Then
            DeCodeHC = DeCodeHC _
                    & "用:8 - P1 硴� 0-1.1堆1" & vbCrLf
        End If

        If CodeHC And 2 ^ 9 Then
            DeCodeHC = DeCodeHC _
                    & "用:9 -  - P2 硴� 0-1.1堆1" & vbCrLf
        End If

        If CodeHC And 2 ^ 10 Then
            DeCodeHC = DeCodeHC _
                    & "用:10 - T1 硴� 0-176竦.�" & vbCrLf
        End If

        If CodeHC And 2 ^ 11 Then
            DeCodeHC = DeCodeHC _
                    & "用:11 - T2 硴� 0-176竦.�" & vbCrLf
        End If

        If CodeHC And 2 ^ 12 Then
            DeCodeHC = DeCodeHC _
                    & "用:12 - G1>G�1" & vbCrLf
        End If

        If CodeHC And 2 ^ 13 Then
            DeCodeHC = DeCodeHC _
                    & "用:13 - 0<G1<G�1" & vbCrLf
        End If

        If CodeHC And 2 ^ 14 Then
            DeCodeHC = DeCodeHC _
                    & "用:14 - G2>G�2" & vbCrLf
        End If

        If CodeHC And 2 ^ 15 Then
            DeCodeHC = DeCodeHC _
                    & "用:15 - 0<G2>G�2" & vbCrLf
        End If

        If CodeHC And 2 ^ 16 Then
            DeCodeHC = DeCodeHC _
                    & "用:16 - G3>G�3" & vbCrLf
        End If

        If CodeHC And 2 ^ 17 Then
            DeCodeHC = DeCodeHC _
                    & "用:17 - 0<G3>G�3" & vbCrLf
        End If

        If CodeHC And 2 ^ 18 Then
            DeCodeHC = DeCodeHC _
                    & "用:18 - M3� < -0.04M1" & vbCrLf
        End If

        If CodeHC And 2 ^ 19 Then
            DeCodeHC = DeCodeHC _
                    & "用:19 - Q� < 0" & vbCrLf
        End If
    End Function
    Private Function FloatExt(ByVal floatStr As String) As Single
        Dim tmpStr As String = ""
        Dim E As Long
        Dim Mantissa As Long
        Dim s As Long
        Dim f As Single
        Dim i As Long
        'If floatStr = "" Then Exit Function
        If floatStr.Length <> 4 Then Exit Function
        ' If floatStr = String(4, 0) Then Exit Function
        If floatStr = Chr(0) + Chr(0) + Chr(0) + Chr(0) Then
            Return 0.0
        End If
        For i = 1 To 4
            tmpStr = Chr(Asc(Mid(floatStr, i, 1))) & tmpStr
        Next i


        floatStr = tmpStr
        '================ Float �蔡謗========================
        '髓.痼蜥                                 跌珞�蓍 痼蜥
        '====================================================
        '籵鉞�.闔��粮� |髓.痼蜥                  跌珞�蓍 痼蜥
        '----------------------------------------------------
        ' xxxx xxxx     | sxxx xxxx | xxxx xxxx | xxxx xxxx |

        ' A = (-1)^s * f * 2^(e-127)
        ' f= 體跛� 銜 0 粮 23 a(k)*2^(-k), 邃� a(k) 瘉� 赭迺蔡� � 邇跂韲� k


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
        FloatExt = (-1) ^ s * f * 2.0! ^ (E - 127)
    End Function



    Private Function OracleDate(ByVal d As Date) As String
        Return "to_date('" + d.Year.ToString() + "-" + d.Month.ToString() + "-" + d.Day.ToString() + _
            " " + d.Hour.ToString() + ":" + d.Minute.ToString() + ":" + d.Second.ToString() + "','YYYY-MM-DD HH24:MI:SS')"
    End Function
    Public Overrides Function WriteArchToDB() As String
        WriteArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,t1,t2,t4,t5,p1,p2,p3,p4,v1,v2,v3,v4,v5,v6,m1,m2,m3,m4,m5,m6,sp_TB1,sp_TB2,q1,q2,TSUM1,TSUM2,hc_code,hc,hc_1,hc_2) values ("
        WriteArchToDB = WriteArchToDB + DeviceID.ToString() + ","
        WriteArchToDB = WriteArchToDB + "SYSDATE" + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + OracleDate(Arch.DateArch) + ","
        WriteArchToDB = WriteArchToDB + Arch.archType.ToString + ","
        WriteArchToDB = WriteArchToDB + Arch.T1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T3.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.T4.ToString.Replace(",", ".") + ","
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
        WriteArchToDB = WriteArchToDB + Arch.Q2.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.Tw1.ToString.Replace(",", ".") + ","
        WriteArchToDB = WriteArchToDB + Arch.Tw2.ToString.Replace(",", ".") + ","

        'WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv1, 1) + ";" + DeCodeHCNumber(Arch.用tv2, 2) + "',"

        'If DeCodeHCNumber(Arch.用tv1, 1) = "" And DeCodeHCNumber(Arch.用tv2, 2) = "" Then
        '    WriteArchToDB = WriteArchToDB + "'-',"
        'ElseIf DeCodeHCNumber(Arch.用tv1, 1) = "" Then
        '    WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv2, 2) + "',"
        'Else
        '    WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv1, 1) + DeCodeHCNumber(Arch.用tv2, 2) + "',"
        'End If

        If DeCodeHCNumber(Arch.用tv1, 1) = "" And DeCodeHCNumber(Arch.用tv2, 2) = "" Then
            WriteArchToDB = WriteArchToDB + "'-','羅� 用',"
        ElseIf DeCodeHCNumber(Arch.用tv1, 1) = "" Then
            WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv2, 2) + "','" + S180("剽纈�蒻: 袱�2:" + DeCodeHCText(Arch.用tv2)) + "',"
        ElseIf DeCodeHCNumber(Arch.用tv2, 2) = "" Then
            WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv1, 1) + "','" + S180("剽纈�蒻: 袱�1:" + DeCodeHCText(Arch.用tv1)) + "',"
        Else
            WriteArchToDB = WriteArchToDB + "'" + DeCodeHCNumber(Arch.用tv1, 1) + DeCodeHCNumber(Arch.用tv2, 2) + "','" + S180("剽纈�蒻: 袱�1:" + DeCodeHCText(Arch.用tv1) + "袱�2:" + DeCodeHCText(Arch.用tv2)) + "',"
        End If

        WriteArchToDB = WriteArchToDB + "'" + DeCodeHCText(Arch.用tv1) + "',"
        WriteArchToDB = WriteArchToDB + "'" + DeCodeHCText(Arch.用tv2) + "'"
        WriteArchToDB = WriteArchToDB + ")"
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
    Public Overrides Function WriteMArchToDB() As String
        WriteMArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,t1,t2,t4,t5,p1,p2,p3,p4,g1,g2,g3,g4,g5,g6,dt12,dt45,sp_TB1,sp_TB2,hc_code,hc,hc_1,hc_2) values ("
        WriteMArchToDB = WriteMArchToDB + DeviceID.ToString() + ","
        WriteMArchToDB = WriteMArchToDB + "SYSDATE" + ","
        WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
        WriteMArchToDB = WriteMArchToDB + OracleDate(mArch.DateArch) + ","
        WriteMArchToDB = WriteMArchToDB + mArch.archType.ToString + ","
        WriteMArchToDB = WriteMArchToDB + mArch.t1.ToString.Replace(",", ".") + ","
        WriteMArchToDB = WriteMArchToDB + mArch.t2.ToString.Replace(",", ".") + ","
        WriteMArchToDB = WriteMArchToDB + mArch.t4.ToString.Replace(",", ".") + ","
        WriteMArchToDB = WriteMArchToDB + mArch.t5.ToString.Replace(",", ".") + ","
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

        ' 關釶諷赭 � 碼�蔡諷辷繻 dt 
        WriteMArchToDB = WriteMArchToDB + "null, null,"
        'WriteMArchToDB = WriteMArchToDB + mArch.dt12.ToString.Replace(",", ".") + ","
        'WriteMArchToDB = WriteMArchToDB + mArch.dt45.ToString.Replace(",", ".") + ","


        WriteMArchToDB = WriteMArchToDB + mArch.SPtv1.ToString + ","
        WriteMArchToDB = WriteMArchToDB + mArch.SPtv2.ToString + ","
        'WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.用tv1, 1) + ";" + DeCodeHCNumber(mArch.用tv2, 2) + "',"




        If DeCodeHCNumber(mArch.用tv1, 1) = "" And DeCodeHCNumber(mArch.用tv2, 2) = "" Then
            WriteMArchToDB = WriteMArchToDB + "'-','羅� 用',"
        ElseIf DeCodeHCNumber(mArch.用tv1, 1) = "" Then
            WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.用tv2, 2) + "','" + S180("剽纈�蒻: 袱�2:" + DeCodeHCText(mArch.用tv2)) + "',"
        ElseIf DeCodeHCNumber(mArch.用tv2, 2) = "" Then
            WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.用tv1, 1) + "','" + S180("剽纈�蒻: 袱�1:" + DeCodeHCText(mArch.用tv1)) + "',"
        Else
            WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCNumber(mArch.用tv1, 1) + DeCodeHCNumber(mArch.用tv2, 2) + "','" + S180("剽纈�蒻: 袱�1:" + DeCodeHCText(mArch.用tv1) + "袱�2:" + DeCodeHCText(mArch.用tv2)) + "',"
        End If

        WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCText(mArch.用tv1) + "',"
        WriteMArchToDB = WriteMArchToDB + "'" + DeCodeHCText(mArch.用tv2) + "'"
        WriteMArchToDB = WriteMArchToDB + ")"
    End Function





    Private Sub cleararchive(ByRef arc As Archive)
        arc.DateArch = DateTime.MinValue

        arc.用 = 0
        arc.Msg用 = ""

        arc.用tv1 = 0
        arc.Msg用tv1 = ""

        arc.用tv2 = 0
        arc.Msg用tv2 = ""

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

        arc.SP = 0
        arc.SPtv1 = 0
        arc.SPtv2 = 0

        arc.T3 = 0
        arc.T4 = 0
        arc.P3 = 0
        arc.P4 = 0
        arc.v4 = 0
        arc.v5 = 0
        arc.v6 = 0
        arc.M4 = 0
        arc.M5 = 0
        arc.M6 = 0

        arc.archType = 0
    End Sub
    Private Sub clearMarchive(ByRef marc As MArchive)
        marc.DateArch = DateTime.MinValue
        marc.用 = 0
        marc.Msg用 = ""

        marc.用tv1 = 0
        marc.Msg用tv1 = ""

        marc.用tv2 = 0
        marc.Msg用tv2 = ""

        marc.G1 = 0
        marc.G2 = 0
        marc.G3 = 0
        marc.G4 = 0
        marc.G5 = 0
        marc.G6 = 0

        marc.t1 = 0
        marc.t2 = 0
        marc.t4 = 0
        marc.t5 = 0

        marc.p1 = 0
        marc.p2 = 0
        marc.p3 = 0
        marc.p4 = 0

        marc.dt12 = 0
        marc.dt45 = 0

        marc.SP = 0
        marc.SPtv1 = 0
        marc.SPtv2 = 0


        marc.archType = 0
    End Sub


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
    End Sub


    Private Function ReadMArch54() As String
        If (IsBytesToRead = True) Then
            Return ""
        End If
        Dim ret As String
        Dim bArr(0 To 8) As Byte
        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(2) = &H52
        bArr(3) = &HE8 Mod 256
        bArr(4) = &HE8 \ 256
        bArr(5) = 8
        bArr(6) = &H0
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16
        IsmArchToRead = True
        clearMarchive(mArch)
        EraseInputQueue()

        WillCountToRead = 18
        IsBytesToRead = True



        write(bArr, 9)
        WaitForData()
        ret = GetAndProcessData()
        If (ret.Length > 5) Then
            If (ret.Substring(0, 6) = "恋葹袱") Then
                ret = ret
                ret = ret & vbCrLf
                ret = ret + "扇�萵 辣 關鋏蓿琿"
                ret = ret & vbCrLf
                EraseInputQueue()
                Return ret
            End If
        End If
        If (ret.Length = 0) Then
            EraseInputQueue()
            Return "恋葹袱 �鱚辷� 趁邇矼迯釿� 瑁�萵� 闔 丗1"
        End If



        Dim InpStrB As String
        InpStrB = ReadRAMSync(&H64, 6)
        If InpStrB <> "" Then
            Try
                mArch.DateArch = New DateTime(buffer(3) + 2000, buffer(4), buffer(5), buffer(6), buffer(7), buffer(8))
            Catch ex As Exception
                mArch.DateArch = DateTime.Now
            End Try

        End If

        If (ret.Length = 0) Then
            EraseInputQueue()
            Return "恋葹袱 �鱚辷� 籥鴿 趁邇矼迯釿� 瑁�萵� "
        End If
        m_isMArchToDBWrite = True
        Return "貰邇矼迯�� 瑁�萵 關鋏蓿琿"
    End Function

    Public Overrides Function ReadMArch() As String
        If V54 Then Return ReadMArch54()

        If (IsBytesToRead = True) Then
            Return ""
        End If
        Dim ret As String
        Dim bArr(0 To 8) As Byte
        bArr(0) = &H10
        bArr(1) = &HFF
        bArr(2) = &H52
        bArr(3) = &H200 Mod 256
        bArr(4) = &H200 \ 256
        bArr(5) = 36
        bArr(6) = &H0
        bArr(7) = 255 - ((Int(bArr(1)) + Int(bArr(2)) + Int(bArr(3)) + Int(bArr(4)) + Int(bArr(5)) + Int(bArr(6))) Mod 256)
        bArr(8) = &H16
        IsmArchToRead = True
        clearMarchive(mArch)
        EraseInputQueue()

        WillCountToRead = 41
        IsBytesToRead = True



        write(bArr, 9)
        WaitForData()
        ret = GetAndProcessData()
        If (ret.Length > 5) Then
            If (ret.Substring(0, 6) = "恋葹袱") Then
                ret = ret
                ret = ret & vbCrLf
                ret = ret + "扇�萵 辣 關鋏蓿琿"
                ret = ret & vbCrLf
                EraseInputQueue()
                Return ret
            End If
        End If
        If (ret.Length = 0) Then
            EraseInputQueue()
            Return "恋葹袱 �鱚辷� 趁邇矼迯釿� 瑁�萵� 闔 丗1"
        End If



        Dim InpStrB As String
        InpStrB = ReadRAMSync(&HF3, 6)
        If InpStrB <> "" Then
            Try
                mArch.DateArch = New DateTime(buffer(3) + 2000, buffer(4), buffer(5), buffer(6), buffer(7), buffer(8))
            Catch ex As Exception
                mArch.DateArch = DateTime.Now
            End Try

        End If

        If (ret.Length = 0) Then
            EraseInputQueue()
            Return "恋葹袱 �鱚辷� 籥鴿 趁邇矼迯釿� 瑁�萵� "
        End If
        m_isMArchToDBWrite = True
        Return "貰邇矼迯�� 瑁�萵 關鋏蓿琿"
    End Function

    Private Function ReadTArch54() As String


        Dim bArr(0 To 8) As Byte

        clearTarchive(tArch)
        EraseInputQueue()

        '========蓿釿鈞�� 籥迯�� 瘠鉅 =============
        Dim InpStrB As String
        InpStrB = ReadFlashSync(&HC3 \ 64, 1)


        InpStrB = InpStrB & ReadFlashSync(&HC3 \ 64 + 1, 1)
        If InpStrB <> "" Then

            InpStrB = ReadRAMSync(&HC3, 8 * 4)
            If InpStrB <> "" Then
                tArch.V1 = tArch.V1 + FloatExt(Mid(InpStrB, 1, 4))
                tArch.V2 = tArch.V2 + FloatExt(Mid(InpStrB, 1 + 4 * 1, 4))
                tArch.V3 = tArch.V3 + FloatExt(Mid(InpStrB, 1 + 4 * 2, 4))
                tArch.M1 = tArch.M1 + FloatExt(Mid(InpStrB, 1 + 4 * 3, 4))
                tArch.M2 = tArch.M2 + FloatExt(Mid(InpStrB, 1 + 4 * 4, 4))
                tArch.M3 = tArch.M3 + FloatExt(Mid(InpStrB, 1 + 4 * 5, 4))
                tArch.Q1 = tArch.Q1 + FloatExt(Mid(InpStrB, 1 + 4 * 6, 4))
                tArch.TW1 = tArch.TW1 + FloatExt(Mid(InpStrB, 1 + 4 * 7, 4))
            End If

        End If


        InpStrB = ReadRAMSync(&H64, 6) '籥鰰
        If InpStrB <> "" Then
            Try
                tArch.DateArch = New DateTime(buffer(3) + 2000, buffer(4), buffer(5), buffer(6), buffer(7), buffer(8))
            Catch ex As Exception
                tArch.DateArch = DateTime.Now
            End Try
        End If


        isTArchToDBWrite = True
        Return "呷鰰譛逶� 瑁�萵 關鋏蓿琿"
    End Function



    Public Overrides Function ReadTArch() As String
        If V54 Then Return ReadTArch54()

        Dim bArr(0 To 8) As Byte

        clearTarchive(tArch)
        EraseInputQueue()

        '========蓿釿鈞�� 籥迯�� 瘠鉅 =============
        Dim InpStrB As String
        InpStrB = ReadFlashSync(&H424A \ 64, 1)


        InpStrB = InpStrB & ReadFlashSync(&H424A \ 64 + 1, 1)
        If InpStrB <> "" Then
            InpStrB = Mid(InpStrB, (&H424A Mod 64) + 1)
            tArch.V1 = ExtLong4(Mid(InpStrB, 1 + 8 * 0, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 0 + 4, 4))
            tArch.V2 = ExtLong4(Mid(InpStrB, 1 + 8 * 1, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 1 + 4, 4))
            tArch.V3 = ExtLong4(Mid(InpStrB, 1 + 8 * 2, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 2 + 4, 4))
            tArch.M1 = ExtLong4(Mid(InpStrB, 1 + 8 * 3, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 3 + 4, 4))
            tArch.M2 = ExtLong4(Mid(InpStrB, 1 + 8 * 4, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 4 + 4, 4))
            tArch.M3 = ExtLong4(Mid(InpStrB, 1 + 8 * 5, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 5 + 4, 4))
            tArch.Q1 = ExtLong4(Mid(InpStrB, 1 + 8 * 6, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 6 + 4, 4))
            tArch.TW1 = ExtLong4(Mid(InpStrB, 1 + 8 * 7, 4)) + FloatExt(Mid(InpStrB, 1 + 8 * 7 + 4, 4))

            InpStrB = ReadRAMSync(&H520, 8 * 4)
            If InpStrB <> "" Then
                tArch.V1 = tArch.V1 + FloatExt(Mid(InpStrB, 1, 4))
                tArch.V2 = tArch.V2 + FloatExt(Mid(InpStrB, 1 + 4 * 1, 4))
                tArch.V3 = tArch.V3 + FloatExt(Mid(InpStrB, 1 + 4 * 2, 4))
                tArch.M1 = tArch.M1 + FloatExt(Mid(InpStrB, 1 + 4 * 3, 4))
                tArch.M2 = tArch.M2 + FloatExt(Mid(InpStrB, 1 + 4 * 4, 4))
                tArch.M3 = tArch.M3 + FloatExt(Mid(InpStrB, 1 + 4 * 5, 4))
                tArch.Q1 = tArch.Q1 + FloatExt(Mid(InpStrB, 1 + 4 * 6, 4))
                tArch.TW1 = tArch.TW1 + FloatExt(Mid(InpStrB, 1 + 4 * 7, 4))
            End If

        End If


        InpStrB = ReadRAMSync(&HF3, 6)
        If InpStrB <> "" Then
            Try
                tArch.DateArch = New DateTime(buffer(3) + 2000, buffer(4), buffer(5), buffer(6), buffer(7), buffer(8))
            Catch ex As Exception
                tArch.DateArch = DateTime.Now
            End Try
        End If


        isTArchToDBWrite = True
        Return "呷鰰譛逶� 瑁�萵 關鋏蓿琿"
    End Function

    Public Overrides Function WriteTArchToDB() As String
        WriteTArchToDB = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,Q1,Q2,M1,M2,M3,M4,M5,M6,v1,v2,v3,v4,v5,v6,TSUM1,TSUM2) values ("
        WriteTArchToDB = WriteTArchToDB + DeviceID.ToString() + ","
        WriteTArchToDB = WriteTArchToDB + "SYSDATE" + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","
        WriteTArchToDB = WriteTArchToDB + OracleDate(tArch.DateArch) + ","
        WriteTArchToDB = WriteTArchToDB + tArch.archType.ToString + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.Q2.ToString.Replace(",", ".") + ","
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
        WriteTArchToDB = WriteTArchToDB + tArch.TW1.ToString.Replace(",", ".") + ","
        WriteTArchToDB = WriteTArchToDB + tArch.TW2.ToString.Replace(",", ".")
        WriteTArchToDB = WriteTArchToDB + ")"
    End Function

    Public Overrides Function WriteErrToDB(ByVal ErrDate As Date, ByVal ErrMsg As String) As String
        Dim SSS As String
        SSS = "INSERT INTO " & DBTableName & "(id_bd,DCALL,DCOUNTER,DATECOUNTER,id_ptype,hc) values ("
        SSS = SSS + DeviceID.ToString() + ","
        SSS = SSS + "SYSDATE" + ","
        SSS = SSS + OracleDate(ErrDate) + ","
        SSS = SSS + OracleDate(ErrDate) + ","

        SSS = SSS + "1,"
        SSS = SSS + "'" & ErrMsg & "')"
        Return SSS
    End Function

    Public Overrides Function IsConnected() As Boolean
        If MyTransport Is Nothing Then Return False
        Return mIsConnected And MyTransport.IsConnected
    End Function



    Public Overrides Function ReadSystemParameters() As System.Data.DataTable
        Return New DataTable
    End Function



    Public Sub EraseInputQueue()
        If (IsBytesToRead = True) Then
            IsBytesToRead = False
        End If
        bufferindex = 0
        Dim i As Short

        i = 1
        While (i > 0 And MyTransport.IsConnected)
            i = MyTransport.Read(buffer, 0, 73)
        End While
        For i = 0 To 73
            buffer(i) = 0
        Next
    End Sub




End Class
