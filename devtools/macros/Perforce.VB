Imports EnvDTE
Imports System.Diagnostics

Public Module Perforce

    Function FindInPath(ByVal exeName As String)
        Dim env = System.Environment.GetEnvironmentVariable("Path")
        Dim Paths = Split(env, ";")
        Dim Element
        Dim p4Filename = ""
        For Each Element In Paths
            If Right(Element, 1) <> "\" And Right(Element, 1) <> "/" Then
                Element = Element + "\"
            End If
            If System.IO.File.Exists(Element + exeName) Then
                Return Element + exeName
            End If
        Next
        FindInPath = ""
    End Function

    Function StripLastDir(ByVal curPath As String)
        Dim testPath = curPath.Replace("/", "\")
        Dim lastIndex = testPath.LastIndexOf("\")
        If lastIndex = -1 Then
            Return ""
        Else
            Return Left(testPath, lastIndex)
        End If
    End Function

    Function EZRunCommand(ByVal exeName As String, ByVal args As String, ByVal p4StartDir As String)
        ' Find the p4 command.
        Dim p4Filename = FindInPath(exeName)
        If p4Filename = "" Then
            MsgBox("Can't find " + exeName + " in path.")
            Return ""
        End If

        ' Run p4 edit <filename>
        Dim p = New System.Diagnostics.Process
        p.StartInfo.UseShellExecute = False
        p.StartInfo.RedirectStandardError = True
        p.StartInfo.Arguments = args
        p.StartInfo.FileName = p4Filename
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = ""
        p.Start()
        Dim outputStr = p.StandardError.ReadToEnd()
        p.WaitForExit()
        If p.ExitCode <> 0 Then
            MsgBox(outputStr)
            Return False
        Else
            Return True
        End If
    End Function

    Sub OutputDebugStr(ByVal str As String)
        Dim win As Window = DTE.Windows.Item(EnvDTE.Constants.vsWindowKindOutput)

        ' Create handles to the Output window and its panes.
        Dim OW As OutputWindow = win.Object

        Dim OWp As OutputWindowPane

        ' Add a new pane to the Output window.
        Try
            OWp = OW.OutputWindowPanes.Item("Build")
        Catch e As System.Exception
            OWp = OW.OutputWindowPanes.Add("Build")
        End Try

        ' Add a line of text to the new pane.
        OWp.OutputString(str + System.Environment.NewLine)
    End Sub

    Sub EditCurrentFile()
        If EZRunCommand("p4.exe", "edit " + Chr(34) + ActiveDocument.FullName + Chr(34), ActiveDocument.FullName) Then
            OutputDebugStr(ActiveDocument.FullName + " edited successfully.")
        End If
    End Sub

    Sub AddCurrentFile()
        If EZRunCommand("p4.exe", "add  " + Chr(34) + ActiveDocument.FullName + Chr(34), ActiveDocument.FullName) Then
            OutputDebugStr(ActiveDocument.FullName + " added successfully.")
        End If
    End Sub

    Sub RevertCurrentFile()
        If EZRunCommand("p4.exe", "revert  " + Chr(34) + ActiveDocument.FullName + Chr(34), ActiveDocument.FullName) Then
            OutputDebugStr(ActiveDocument.FullName + " reverted successfully.")
        End If
    End Sub

End Module
