Imports System.ComponentModel
Imports System.Threading
Imports System.IO
Imports System.Xml
Imports System.Net

Public Class Main_Screen
    Dim exitlaunch As Boolean = True
    Dim apptorun As String = """" & (Application.StartupPath & "\7za.exe""").Replace("\\", "\")


    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "", Optional ByVal silent As Boolean = True)
        Try
            If silent = False Then
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.Message.ToString
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
            End If
            If My.Computer.FileSystem.DirectoryExists((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs") = False Then
                My.Computer.FileSystem.CreateDirectory((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs")
            End If
            Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
            filewriter.Flush()
            filewriter.Close()
            filewriter = Nothing
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Error_Handler(ByVal ex As String, Optional ByVal identifier_msg As String = "", Optional ByVal silent As Boolean = True)
        Try
            If silent = False Then
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ": " & ex.ToString
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
            End If
            If My.Computer.FileSystem.DirectoryExists((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs") = False Then
                My.Computer.FileSystem.CreateDirectory((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs")
            End If
            Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
            filewriter.Flush()
            filewriter.Close()
            filewriter = Nothing
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Log_Handler(ByVal message As String)
        Try
            If My.Computer.FileSystem.DirectoryExists((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs") = False Then
                My.Computer.FileSystem.CreateDirectory((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs")
            End If
            Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Update Logs\" & Format(Now(), "yyyyMMdd") & "_Update_Log.txt", True)
            filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & message)
            filewriter.Flush()
            filewriter.Close()
            filewriter = Nothing
        Catch ex As Exception
            Error_Handler(ex, "Log Handler")
        End Try
    End Sub


  
    Private Sub Main_Function()
        Try
            Log_Handler("**************************************************" & vbCrLf)
            Log_Handler("AutoUpdate now running check..." & vbCrLf)
            Dim filereader As StreamReader
            Dim manifest As String = ""
            Dim temp As String

            If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "config.aup").Replace("\\", "\")) = True Then
                filereader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "config.aup").Replace("\\", "\"))

                While filereader.Peek <> -1
                    temp = filereader.ReadLine()
                    If temp.ToUpper.StartsWith("APPLICATION_NAME=") = True Then
                        manifest = temp
                    End If
                End While
                filereader.Close()
                filereader = Nothing
                If manifest.Length > 17 Then
                    Label1.Text = "Updating " & manifest.Remove(0, 17)
                End If

                Try
                    If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "config.aup").Replace("\\", "\")) = True Then
                        filereader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "config.aup").Replace("\\", "\"))

                        While filereader.Peek <> -1
                            temp = filereader.ReadLine()
                            If temp.ToUpper.StartsWith("MANIFEST=") = True Then
                                manifest = temp
                            End If
                        End While
                        filereader.Close()
                        filereader = Nothing
                        If manifest.Length > 9 Then
                            manifest = manifest.Remove(0, 9)
                            If manifest.Length > 0 Then
                                Label2.Text = "Manifest: " & manifest
                                ToolTip1.SetToolTip(Label2, manifest)
                                Try
                                    If My.Computer.FileSystem.FileExists((Application.StartupPath & "\7za.exe").Replace("\\", "\")) = False Then
                                        Dim za As String = manifest
                                        za = za.Remove(za.LastIndexOf("/"), za.Length - za.LastIndexOf("/"))
                                        za = za.Remove(za.LastIndexOf("/"), za.Length - za.LastIndexOf("/"))
                                        za = za & "/7za.exe"
                                        If My.Computer.Network.IsAvailable() = True Then
                                            My.Computer.Network.DownloadFile(za, (Application.StartupPath & "\" & "7za.exe").Replace("\\", "\"), "", "", False, 100000, True)
                                        End If
                                    End If
                                Catch ex As Exception
                                    Error_Handler(ex, "7za Application Download")
                                End Try
                                Try
                                    If My.Computer.Network.IsAvailable() = True Then
                                        My.Computer.Network.DownloadFile(manifest, (Application.StartupPath & "\" & "manifest.xml").Replace("\\", "\"), "", "", False, 100000, True)
                                    End If
                                Catch ex As Exception
                                    Error_Handler(ex, "File Download")
                                End Try

                                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "manifest.xml").Replace("\\", "\")) = True Then
                                    Dim re As StreamReader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "manifest.xml").Replace("\\", "\"))
                                    Dim cont As Boolean = False
                                    Dim newmanifestflag As Boolean = True
                                    If re.Peek <> -1 Then
                                        If re.ReadLine.ToLower.StartsWith("<?xml") = True Then
                                            cont = True
                                        End If
                                    End If
                                    re.Close()
                                    re.Dispose()
                                    re = Nothing
                                    If cont = True Then


                                        Dim m_xmld As XmlDocument
                                        Dim m_nodelist As XmlNodeList
                                        Dim m_node As XmlNode
                                        m_xmld = New XmlDocument()
                                        m_xmld.Load((Application.StartupPath & "\" & "manifest.xml").Replace("\\", "\"))
                                        m_nodelist = m_xmld.SelectNodes("/application/manifestUpdated")
                                        For Each m_node In m_nodelist
                                            If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "AutoUpdate.rec").Replace("\\", "\")) Then
                                                Dim quickread As StreamReader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "AutoUpdate.rec").Replace("\\", "\"))
                                                If m_node.ChildNodes.Item(0).InnerText > quickread.ReadLine Then
                                                    'just remembered that what i'm doing is stupid. autoupdate must always run in case a file is missing
                                                    'manifest number is irrelevant
                                                End If
                                                quickread.Close()
                                                quickread = Nothing
                                                My.Computer.FileSystem.DeleteFile((Application.StartupPath & "\" & "AutoUpdate.rec").Replace("\\", "\"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                            End If
                                            Dim quickwrite As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter((Application.StartupPath & "\" & "AutoUpdate.rec").Replace("\\", "\"), False)
                                            quickwrite.Write(m_node.ChildNodes.Item(0).InnerText)
                                            quickwrite.Close()
                                            quickwrite = Nothing
                                        Next
                                        m_nodelist = m_xmld.SelectNodes("/application/file")
                                        Dim filename, filepath, filesize, filelastmodified, dfilepath, filepathclear As String
                                        Dim downloadfile As Boolean
                                        Dim finfo As FileInfo
                                        Dim counter As Integer = 0
                                        For Each m_node In m_nodelist
                                            counter = counter + 1
                                        Next
                                        Dim percentComplete As Integer
                                        Dim counter2 As Integer = 0
                                        For Each m_node In m_nodelist
                                            counter2 = counter2 + 1
                                            Label4.Text = counter2 & "/" & counter
                                            downloadfile = False
                                            If m_node.ChildNodes.Count = 5 Then
                                                filename = m_node.ChildNodes.Item(0).InnerText
                                                Label2.Text = "File: " & filename
                                                ToolTip1.SetToolTip(Label2, filename)
                                                Dim tempfilepath As String
                                                filepath = m_node.ChildNodes.Item(1).InnerText
                                                tempfilepath = filepath
                                                dfilepath = filepath
                                                filepathclear = m_node.ChildNodes.Item(2).InnerText
                                                filesize = m_node.ChildNodes.Item(3).InnerText
                                                filelastmodified = m_node.ChildNodes.Item(4).InnerText
                                                filepath = (Application.StartupPath & "\" & filepath).Replace("\\", "\")
                                                filepathclear = (Application.StartupPath & "\" & filepathclear).Replace("\\", "\")

                                                If My.Computer.FileSystem.FileExists(filepathclear) = False Then
                                                    downloadfile = True
                                                Else
                                                    finfo = New FileInfo(filepathclear)
                                                    If Format(finfo.LastWriteTime, "yyyyMMddHHmm") < filelastmodified Then
                                                        downloadfile = True
                                                    End If
                                                    finfo = Nothing
                                                End If

                                                'If filename = "AutoUpdate.exe" Then
                                                'downloadfile = False
                                                'End If

                                                If downloadfile = True Then
                                                    If Me.WindowState = FormWindowState.Minimized Then
                                                        Me.WindowState = FormWindowState.Normal
                                                    End If
                                                    Try
                                                        If My.Computer.Network.IsAvailable() = True Then
                                                            My.Computer.Network.DownloadFile((manifest.Replace("/manifest.xml", "") & "/" & filename).Replace("\", "/"), filepath, "", "", False, 100000, True)
                                                            Log_Handler("**************************************************" & vbCrLf)
                                                            Log_Handler("Downloaded " & (manifest.Replace("/manifest.xml", "") & "/" & filename).Replace("\", "/") & " to " & filepath & vbCrLf)
                                                            If My.Computer.FileSystem.FileExists(filepath) = True Then
                                                                Log_Handler("Running: DosShellCommand(" & apptorun & " e """ & filepath & """ -o""" & (Application.StartupPath & tempfilepath.Remove(tempfilepath.Length - filename.Length, filename.Length)).Replace("\\", "\") & """ -y" & ")" & vbCrLf)

                                                                DosShellCommand(apptorun, "e """ & filepath & """ -o""" & (Application.StartupPath & tempfilepath.Remove(tempfilepath.Length - filename.Length, filename.Length)).Replace("\\", "\") & """ -y")

                                                                My.Computer.FileSystem.DeleteFile(filepath, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)
                                                            End If
                                                        End If
                                                    Catch ex As Exception
                                                        Error_Handler(ex, "File Download")
                                                    End Try
                                                End If
                                            End If

                                            If counter2 > 0 Then
                                                percentComplete = CSng(counter2) / CSng(counter) * 100
                                            Else
                                                percentComplete = 100
                                            End If
                                            If percentComplete < 100 Then
                                                ProgressBar1.Value = percentComplete
                                            Else
                                                ProgressBar1.Value = 100
                                            End If

                                        Next
                                        m_xmld = Nothing
                                    Else
                                        Error_Handler("Error: Manifest doesn't appear to be a valid XML document", "Main Function")
                                        Label2.Text = "Error: Manifest doesn't appear to be a valid XML document"
                                        'ToolTip1.SetToolTip(Label2, "Error: Manifest doesn't appear to be a valid XML document")
                                        Me.ControlBox = True
                                        Me.Refresh()
                                        Me.WindowState = FormWindowState.Normal
                                    End If

                                    My.Computer.FileSystem.DeleteFile((Application.StartupPath & "\" & "manifest.xml").Replace("\\", "\"), FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                                Else
                                    Error_Handler("Error: Manifest cannot be downloaded", "Main Function")
                                    Label2.Text = "Error: Manifest cannot be downloaded"
                                    'ToolTip1.SetToolTip(Label2, "Error: Manifest cannot be downloaded")
                                    Me.ControlBox = True
                                    Me.Refresh()
                                    Me.WindowState = FormWindowState.Normal
                                End If
                            End If
                        Else
                            Error_Handler("Error: Manifest entry not located", "Main Function")
                            Label2.Text = "Error: Manifest entry not located"
                            'ToolTip1.SetToolTip(Label2, "Error: Manifest entry not located")
                            Me.ControlBox = True
                            Me.Refresh()
                            Me.WindowState = FormWindowState.Normal
                        End If
                    Else
                        Error_Handler("Error: Config doesn't exist", "Main Function")
                        Label2.Text = "Error: Config doesn't exist"
                        'ToolTip1.SetToolTip(Label2, "Error: Config doesn't exist")
                        Me.ControlBox = True
                        Me.Refresh()
                        Me.WindowState = FormWindowState.Normal
                    End If

                Catch ex As Exception
                    Error_Handler(ex, "Main Function")
                    'Main_Function()
                End Try

            Else
                Error_Handler("Error: Config doesn't exist", "Main Function")
                Label2.Text = "Error: Config doesn't exist"
                'ToolTip1.SetToolTip(Label2, "Error: Config doesn't exist")
                Me.ControlBox = True
                Me.Refresh()
                Me.WindowState = FormWindowState.Normal
            End If
        Catch ex As Exception
            Error_Handler(ex, "Main Function")
        End Try

    End Sub

    Private Sub Main_Screen_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Control.CheckForIllegalCrossThreadCalls = False
            Me.Text = "AutoUpdate " & Format(My.Application.Info.Version.Major, "0000") & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & Format(My.Application.Info.Version.Revision, "00")

            Dim runApplication As Boolean = True

            Dim filereader As StreamReader
            Dim lineread As String = ""

            'MsgBox("File Exists: " & My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "config_lastRun.aup").Replace("\\", "\")))
            If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "config_lastRun.aup").Replace("\\", "\")) = True Then
                filereader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "config_lastRun.aup").Replace("\\", "\"))
                While filereader.Peek <> -1
                    lineread = filereader.ReadLine
                    'MsgBox("Date Check: " & Format(Now(), "yyyyMMdd") & " = " & lineread)
                    If Format(Now(), "yyyyMMdd") = lineread Then
                        runApplication = False
                        Exit While
                    End If
                End While
                filereader.Close()
                filereader = Nothing
            End If

            'MsgBox("Before Command Line Check: " & runApplication)

            If My.Application.CommandLineArgs.Count > 0 Then
                For Each fil As String In My.Application.CommandLineArgs
                    If fil.Length > 0 Then
                        If fil.ToLower = "force" Or fil.ToLower = "f" Then
                            runApplication = True
                        End If
                    End If
                Next
            End If

            'MsgBox("After Command Line Check: " & runApplication)

            If runApplication = True Then
                If Application.ExecutablePath.ToLower.EndsWith("autoupdate.exe") = True Then
                    exitlaunch = False
                    My.Computer.FileSystem.CopyFile(Application.ExecutablePath, Application.ExecutablePath.Replace("AutoUpdate.exe", "AutoUpdate_Launcher.exe"), True)
                    If runApplication = True Then
                        Shell(Application.ExecutablePath.Replace("AutoUpdate.exe", "AutoUpdate_Launcher.exe") & " force", AppWinStyle.NormalFocus, False)
                    Else
                        Shell(Application.ExecutablePath.Replace("AutoUpdate.exe", "AutoUpdate_Launcher.exe"), AppWinStyle.NormalFocus, False)
                    End If
                    Me.Close()
                Else
                    exitlaunch = True
                    BackgroundWorker1.RunWorkerAsync()
                End If
            Else
                exitlaunch = True
                Me.Close()
            End If

        Catch ex As Exception
            Error_Handler(ex, "Main Screen Load")
            BackgroundWorker1.RunWorkerAsync()
        End Try
    End Sub


    Private Sub Main_Screen_Closed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        Try
            If exitlaunch = True Then
                Dim filewriter As StreamWriter
                filewriter = My.Computer.FileSystem.OpenTextFileWriter((Application.StartupPath & "\" & "config_lastRun.aup").Replace("\\", "\"), False)
                filewriter.WriteLine(Format(Now(), "yyyyMMdd"))
                filewriter.Close()
                filewriter = Nothing

                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\" & "config.aup").Replace("\\", "\")) = True Then
                    Dim filereader As StreamReader = My.Computer.FileSystem.OpenTextFileReader((Application.StartupPath & "\" & "config.aup").Replace("\\", "\"))
                    Dim manifest As String = ""
                    Dim temp As String
                    While filereader.Peek <> -1
                        temp = filereader.ReadLine()
                        If temp.ToUpper.StartsWith("EXECUTABLE=") = True Then
                            manifest = temp
                        End If
                    End While
                    filereader.Close()
                    filereader = Nothing
                    If manifest.Length > 11 Then
                        Shell("""" & (Application.StartupPath & "\" & manifest.Remove(0, 11)).Replace("\\", "\") & """", AppWinStyle.NormalFocus, False)
                    End If
                Else
                    Error_Handler("Error: Config doesn't exist", "Main Function")
                    Label2.Text = "Error: Config doesn't exist"
                    'ToolTip1.SetToolTip(Label2, "Error: Config doesn't exist")
                    Me.ControlBox = True
                    Me.Refresh()
                    Me.WindowState = FormWindowState.Normal
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Main_Screen_Closed")
        End Try
    End Sub


    Private Function DosShellCommand(ByVal AppToRun As String, ByVal AppToRunArgs As String) As String
        Dim s As String = ""
        Try
            Dim myProcess As Process = New Process

            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.UseShellExecute = False

            Dim sErr As StreamReader
            Dim sOut As StreamReader
            Dim sIn As StreamWriter


            myProcess.StartInfo.CreateNoWindow = True

            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True

            myProcess.StartInfo.FileName = AppToRun
            myProcess.StartInfo.Arguments = AppToRunArgs

            myProcess.Start()
            sIn = myProcess.StandardInput
            sIn.AutoFlush = True

            sOut = myProcess.StandardOutput()
            sErr = myProcess.StandardError

            sIn.Write(AppToRun & System.Environment.NewLine)
            sIn.Write("exit" & System.Environment.NewLine)
            s = sOut.ReadToEnd()
            'MsgBox(s)
            Log_Handler(s)
            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If



            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()


        Catch ex As Exception
            Error_Handler(ex, "DOS Shell Command")
        End Try
        Return s
    End Function

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Main_Function()
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Label2.Text = "Update Procedure Completed"
        Me.Close()
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged

    End Sub
End Class
