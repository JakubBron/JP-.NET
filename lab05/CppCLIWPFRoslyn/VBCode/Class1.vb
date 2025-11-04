Imports Microsoft.VisualBasic.FileIO
Imports System.IO
Imports System.Text
Imports System

Public Class A
    Public Function f1(ByVal path As String) As String
        Using fs As FileStream = File.Create(path)
            Dim info As Byte() = New UTF8Encoding(True).GetBytes("Successful file saving and reading " + path + " file.")
            fs.Write(info, 0, info.Length)
        End Using
        Dim outInfo As String = FileSystem.ReadAllText(path)
        Console.WriteLine(outInfo)
        Return outInfo
    End Function

    Public Function f2(ByVal p As String) As String
        s = p
        Return "variable s value is now set to: '" + s + "'"
    End Function

    Public i As Integer
    Public s As String
    Public b As Boolean
End Class
