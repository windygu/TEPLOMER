﻿Imports System
Imports System.Runtime.InteropServices
Module Module1
    Public Enum ShowWindowConstants

        SW_HIDE = 0

        SW_SHOWNORMAL = 1

        SW_NORMAL = 1

        SW_SHOWMINIMIZED = 2

        SW_SHOWMAXIMIZED = 3

        SW_MAXIMIZE = 3

        SW_SHOWNOACTIVATE = 4

        SW_SHOW = 5

        SW_MINIMIZE = 6

        SW_SHOWMINNOACTIVE = 7

        SW_SHOWNA = 8

        SW_RESTORE = 9

        SW_SHOWDEFAULT = 10

        SW_FORCEMINIMIZE = 11

        SW_MAX = 11

    End Enum
    <DllImport("User32.dll")> _
    Public Function ShowWindowAsync(ByVal hWnd As IntPtr, ByVal swCommand As Integer) As Integer

    End Function

    Public Sub Main()
        Dim cc As maker
        cc = New maker
        Dim s As String
        s = Command()
        cc.Go(s)
    End Sub

End Module
