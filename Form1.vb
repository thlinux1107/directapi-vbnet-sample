Imports Newtonsoft.Json
Imports System.Security.Cryptography
Imports System.Text
Imports System.Net
Imports System.IO



Public Class Main
        Private Sub btProcess_Click(sender As Object, e As EventArgs) Handles btProcess.Click

        'TH - Set the variables
        Dim jsonRequest As String = txtJSONrequest.Text
        Dim merchantId As String = txtMerchantId.Text
        Dim merchantKey As String = txtMerchantKey.Text
        Dim clientId As String = txtClientId.Text
        Dim clientSecret As String = txtClientSecret.Text
        Dim url_sale As String = "https://api-cert.sagepayments.com/bankcard/v1/charges?type=Sale"
        'Dim url_sale As String = "http://requestb.in/1c4m1691"
        Dim verb As String = "POST"
        Dim contentType As String = "application/json"
        Dim hash_authToken As Byte()
        Dim hash64_authToken As String

        'TH - Building timestamp and nonce. Needs to be epoch in seconds.
        Dim t As TimeSpan = (Date.Now - New DateTime(1970, 1, 1))
        Dim timestamp As Integer = t.TotalSeconds.ToString
        'TH - Added a static timestamp to check hash against php sample.
        'Dim timestamp As Integer = "1477496782"
        Dim nonce As String = timestamp




        'TH - Build the Authorization
        Dim authToken As String = verb & url_sale & jsonRequest & merchantId & nonce & timestamp
        Using hmac As HMACSHA512 = New HMACSHA512(Encoding.ASCII.GetBytes(clientSecret))
            hash_authToken = hmac.ComputeHash(Encoding.ASCII.GetBytes(authToken))
        End Using
        hash64_authToken = Convert.ToBase64String(hash_authToken)

        'TH - Initiate a Web Request
        Dim web_request As HttpWebRequest = CType(HttpWebRequest.Create(url_sale), HttpWebRequest)

        'TH - Set the headers and details
        Dim web_request_headers As WebHeaderCollection = web_request.Headers
        Console.WriteLine("Configuring web_request to included headers for Sage Direct API")
        web_request_headers.Add("clientId: " & clientId)
        web_request_headers.Add("merchantId: " & merchantId)
        web_request_headers.Add("merchantKey: " & merchantKey)
        web_request_headers.Add("nonce: " & nonce)
        web_request_headers.Add("timestamp: " & timestamp)
        web_request_headers.Add("authorization: " & hash64_authToken)
        web_request.Method = verb

        'TH - Format the request
        Dim byteArray As Byte() = Encoding.ASCII.GetBytes(jsonRequest)

        'TH - Send the data
        web_request.ContentType = contentType
        web_request.ContentLength = byteArray.Length
        Dim datastream As Stream = web_request.GetRequestStream()
        datastream.Write(byteArray, 0, byteArray.Length)
        datastream.Close()

        'TH - Get the Response
        Try
            Dim web_response As WebResponse = web_request.GetResponse()
            Console.WriteLine(CType(web_response, HttpWebResponse).StatusDescription)
            datastream = web_response.GetResponseStream()


            Dim reader As New StreamReader(datastream)
            Dim responseFromServer As String = reader.ReadToEnd()
            Console.WriteLine(responseFromServer)
            reader.Close()
            datastream.Close()
            web_response.Close()
            MsgBox("Response: " & responseFromServer)
        Catch ex As Exception
            MsgBox("Response: " & ex.Message)
        End Try



    End Sub

    Private Sub btLoad_Click(sender As Object, e As EventArgs) Handles btLoad.Click
        'TH - Test Data
        Dim merchantId As String = "819741936545"
        Dim merchantKey As String = "E5D6V3I2K5P1"
        Dim clientId As String = "dTkDb4GWxrjw35jfKcNSB6qs5jJjoTqa"
        Dim clientSecret As String = "EzHUFdK3Egr4MI3x"

        txtMerchantId.Text = merchantId
        txtMerchantKey.Text = merchantKey
        txtClientId.Text = clientId
        txtClientSecret.Text = clientSecret

    End Sub
End Class

