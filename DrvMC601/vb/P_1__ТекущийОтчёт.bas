Attribute VB_Name = "P_1__������������"
Option Explicit
'
' ������ �������� ��������� �������� ������.
' ��� �� �������� �� Q-���������
'
Global ��������� As Variant, ����������� As Variant
Global ������������� As Variant, ������������ As Variant

Global ��������� As Variant, ������������� As Variant
Global ��������� As Variant, ���������� As Variant
Global ����� As Variant, ���������� As Boolean
Global ���������� As Variant
' Global ������� As Variant

Global �������� As Boolean
Global ����� As Variant, ������� As Variant
Global ��������� As Variant, ���������� As Variant
Global ������� As Variant, ������� As Variant
Global ��������� As Variant, Datime As Variant

Global �������������� As Boolean


'   *************************
'   *** �����, ���������� ***
'   *************************
Public Sub �����������������_I()
    ��������� = ""
    ������������� = ""
    ���������� = ""
    ��������� = Null
    ���������� = Null
    ����� = Null
    ���������� = True
'    ������� = Null
End Sub

Public Sub �����������������_P()
    ��������� = ""
    ����������� = ""
    ������������� = ""
    ������������ = ""
End Sub

Public Sub �����������������_R()
    ��������� = ""
    ����� = Null
    �������� = True
    ��������� = Date
    ���������� = Time()
    ������� = Null
    ������� = Null
    ������� = Null
End Sub

Public Sub �����������������_���()
    �����������������_I
    �����������������_P
    �����������������_R
End Sub

Public Sub ��������������_I(ByVal ������ As Variant)
    �����������������_I
        If NoData(������) Then Exit Sub
    ���������� ������
' ������ I-��������� (����)
    ��������� = ���������(����_���)
    ��������� = ���������(����_���������)
    ���������� = ���������(����_����������)
    ����� = �����������(���������(����_�����), ����������)
    ������������� = ���������(����_���������)
    ���������� = ���������(����_�����)
End Sub

Public Sub ��������������_P(ByVal ������� As Variant)
    �����������������_P
        If NoData(�������) Then Exit Sub
    �������������� �������
' ������ P-��������� (����)
    ����������� = ��������������_P(����_�����)
    ������������� = ��������������_P(����_������)
    ������������ = ��������������_P(����_�����)
    ��������������
End Sub


'   *********************************
'   *** �������������� ���������� ***
'   *********************************
' ��� -- ��������� ���������� ������� � ������ �� ���������
'
Public Sub ���������������()
    If NoData(�����) Or (����� = 0) Then ����� = 101
    If NoData(�����) Or (����� = 0) Then
        ����� = ��������������(�����, ����������)
      End If
End Sub

Public Function ����������()
' ��������� ������� ������
    If NoData(����������) Or ���������� = 0 Then
        ���������� = Null
      Else
        ���������� = ����������������(������������(Date, ����������, �����))
      End If
End Function

Public Function �����������(ByVal ������ As Variant)
' ��������� ������� ����� ������
Dim d As Variant, Q As Variant
    ����������� = Null
        If NoData(���������) Or (��������� = 0) Then Exit Function
    d = �����������������(Date, �����, ���������)
    If Not NoData(������) Then
        Q = ������������(������, �����)
        Q = �����������������(Q, �����, ���������)
        If Q < d Then d = Q
      End If
    ����������� = d
End Function

Public Function ����������(ByVal ������ As Variant)
Dim d As Variant, X As Integer, P As Integer
    ���������� = Null
        If NoData(���������) Or (��������� = 0) Then Exit Function
    ' ��� ���� ����������� ������ ��� ����.
    If NoData(������) Then
        d = �����������������(Date, �����, ���������)
        d = ����������(d, �����������(����������, ���������))
      Else
        d = ����������������(������)
        d = ������������(d, �����)
        d = �����������������(d, �����, ���������)
      End If
    ���������� = ��������������(d, ���������)
End Function


