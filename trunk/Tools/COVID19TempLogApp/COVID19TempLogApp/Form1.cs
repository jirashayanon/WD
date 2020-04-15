using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;


namespace COVID19TempLogApp
{
    public partial class Form1 : Form
    {

        //static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "Google Sheets API .NET Quickstart";

        private Dictionary<string, string> _userColTable = new Dictionary<string, string>();
        private Dictionary<string, string> _dateRowTable = new Dictionary<string, string>();


        private SheetsService _service;
        private String _spreadsheetId = string.Empty;
        private void _initService()
        {

            string serviceAccountCredentialFilePath = "covid19templogapi-0c698e096d44.json";
            GoogleCredential credential;

            using (var stream = new FileStream(serviceAccountCredentialFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }





            /*
            const string strServiceAcctCredFileContents = @"{
                ""type"": ""service_account"",
                ""project_id"": ""covid19templogapi"",
                ""private_key_id"": ""0c698e096d44dd90a9771bd939e7eef2b5e5f6b0"",
                ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC7jQWt9YSuNByA\nafQAoq+HiEzrnrLLl8zjQAZR/sdx8WyUd8re56s5FqHySPxEhrMrsONgzV3Dt7T1\nK8l0rtOkiOEyqMC+5NACcMxpP8fWWTrtcBDx47z7HklmRA4ZXOCmaQ1un4ahG1zA\nHbM5hImrMRpa3ZBbi3xoieJDx/2TsrB5gJoEmCqq2uXj+Im2LyYG8zwDedkZZbYz\nY7Kf+IleMSMCmVl4MfXe9O+FnU6i7D0hbd7Mxa/3yUoXuwhbczmv8oZiAjhnQd0m\nx+9/8iwASvjWN6Y45HpPqJ1WsR7Y1Zre/pJKbm+oKz6qKYHgqNSo5tR4j1KcOOJT\nFqo5UxbvAgMBAAECggEAF+WnoBtE/SdhrFETyJLyF6xb1LfjY0Kj1h7dz0vN4OxY\nn/BFlrJhJESIfQUks5yQLtR6nGZ+eIYrVgPuxuHR0MiqHRjYelpQUMdLnGuGjH7R\nHdjmJ9QTS9H3m0K3oy6s5zdCgWJtNpD0d/wwznbfTcNTbtmFX8y9nNpYzLcqTcvC\nJ99Pg6stmFw+FyimkzDPKEjJ0k2nBVS7i86wjIJkHIENv+6Ba0yXmkqruDM67ryC\nEt+vaa3lsidHx02v83adSeO5Udq7yF+Y+ePbaoPx9zAgm0gqmySi5AqjzTHgMGBq\n4dysi8xlDR6QxhYFQ8jSmKJ1f+WObnJkXhs2rE+fAQKBgQD3CHdQDnuPZMAYKSI7\nK0c6dyciG4dqXliFM5cFASMXNCuD6dfvx5TjdoookEI+KHuLXC7C/YGKROpGGFjU\n6aazOmmhMcvHAaHIU2gNdRfpvkiuHwm/cioQidjug+QQ8Fi7H7rgNK/4/lWfWj3l\nEkJuNgJPQOl3w7DqXkTgzD6NAQKBgQDCW9KhmDgg2DRzGIw3pZS43iR7TM8PV6d9\nt3z1oJVDG+4Zkj2jk3VjvnpR4kX71PjBXPWVsKlB4tmPj3DwaKFp+pHbIWdd+Psq\n3ksiXUcbRgmYMCxrBTrlHXVPs0fDDT35Os9PS0m6E8/FJ0aTclHJgJwirOStD0on\nnMkt05Zz7wKBgGPnQJlXwhCTpjwfJoDXV88FMQZSK8mQrivwWTrk4tQXfSG0CGgo\nRsZWwjj1XlTU+mG5vg+Nhj3s8PG4FvTEKW6CSQWvpcGtn0fssz/+AtW85pfEZaTO\n/sQBe4G6RRm0ma3Tfzf9Fs2TzfzL+gwR/luj6/sxPTn6IiomRaTD9OgBAoGBAJAS\nCLyIXXLbUWXP2ICXWPsAkAJmGUbCLwHdtd9NvdcVKRYDdXV/pRDe7UUtebsHT63l\n/pWLcqvIQIVOSuWqaZxphRzuUfUyztwkE4XRpxfsfsg1TXe3VMTpZUBDIPrRIhNR\nKMMah/hLH0SBrKs3nrDDiPmbN5ehMdLTQb2ajng5AoGAEsbxtAKc6I509qeXmXJG\n3rMAY0uW3RkKjEJb3EHfLT+hDN9dOqu8xhHKuJM0Xz7Bs5Vw9mgh1ah2oGJJIq7D\nrsNvwrRLfrQf7LOQ5fpUx8YRoq4kUVWNaHW6k7HX94TLNxEngif3/1/f4lTQoId9\ns2H07rwhokd+9+fdyB9Ihm0=\n-----END PRIVATE KEY-----\n"",
                ""client_email"": ""covid19templogservice @covid19templogapi.iam.gserviceaccount.com"",
                ""client_id"": ""109922920355019400535"",
                ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                ""token_uri"": ""https://oauth2.googleapis.com/token"",
                ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/covid19templogservice%40covid19templogapi.iam.gserviceaccount.com""}";

            var credParams = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(strServiceAcctCredFileContents);

            var initializer = new ServiceAccountCredential.Initializer(credParams.ClientEmail);

            var cred = new ServiceAccountCredential(initializer.FromPrivateKey(credParams.PrivateKey));

            cred.GetAccessTokenForRequestAsync(@"https://www.googleapis.com/oauth2/v1/certs");
            */




            // Create Google Sheets API _service.
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                //HttpClientInitializer = cred,
                ApplicationName = ApplicationName,
            });



            // Define request parameters.
            //String _spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit




            _spreadsheetId = @"1Q6IACnD-54d0dq8iEwgVjbT0-MuEvCwHOIGaS3sPSt8";  //Covid19 Temp Log
            //String range = "Sheet1!A2:E1000";
            String range = "Sheet1";

            SpreadsheetsResource.ValuesResource.GetRequest request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;



            List<string> lstCol = new List<string>();
            int r = 0;
            int c = 0;

            if (values != null && values.Count > 0)
            {
                //      ColA,   ColB
                //row1, Id
                //row2, Name
                //row3, Date

                r = 0;
                foreach (var row in values)
                {
                    // Print columns A and B, which correspond to indices 0 and 1.
                    //Console.WriteLine("{0}, {1}", row[0], row[1]);

                    if (r == 0) //construct list of column index
                    {
                        foreach (var elmnt in row)
                        {
                            lstCol.Add(elmnt.ToString());
                        }
                    }


                    if (row[0].ToString() == "Id")
                    {
                        foreach (var elmnt in row)
                        {
                            if (c > 0) //skip column#1
                            {
                                _userColTable.Add(elmnt.ToString(), lstCol[c]);
                            }

                            c++;
                        }
                    }


                    if (r > 2)
                    {
                        _dateRowTable.Add(row[0].ToString(), (r + 1).ToString());
                    }

                    r++;
                }

            }
            else
            {
                Console.WriteLine("No data found.");
            }

        }


        public Form1()
        {
            InitializeComponent();

            txtboxID.Text = "";
            txtboxTemp.Text = "";
            lblExceptionMessage.Text = "";

            _initService();
        }

        private string _employeeID = string.Empty;
        private void txtboxID_Validated(object sender, EventArgs e)
        {
            _employeeID = txtboxID.Text;
        }

        private string _temp = string.Empty;
        private void txtboxTemp_Validated(object sender, EventArgs e)
        {
            _temp = txtboxTemp.Text;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine(dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                Console.WriteLine(_dateRowTable[dateTimePicker1.Value.ToString("yyyy-MM-dd")]);
            }
            catch(Exception ex)
            {
                lblExceptionMessage.Text = "Please check the Date or contact admin.";
                return;
            }

            try
            {
                Console.WriteLine(_userColTable[_employeeID]);
            }
            catch(Exception ex)
            {
                //lblExceptionMessage.Text = ex.Message;
                lblExceptionMessage.Text = "Please check your Employee ID or contact admin.";
                return;
            }


            _update(txtboxTemp.Text);

        }

        private void _update(string temp)
        {

            try
            {
                List<object> newValArray = new List<object>();
                //Convert.ToInt32(temp)
                newValArray.Add(Convert.ToDouble(temp));

                ValueRange newValReq = new Google.Apis.Sheets.v4.Data.ValueRange();
                newValReq.Values = new List<IList<object>> { newValArray };


                //String range = "Sheet1!A2:E1000";
                String range = "Sheet1!";

                Console.WriteLine(_userColTable[_employeeID]);
                Console.WriteLine(_dateRowTable[dateTimePicker1.Value.ToString("yyyy-MM-dd")]);

                string newrange = String.Concat(range,
                        String.Concat(_userColTable[_employeeID], _dateRowTable[dateTimePicker1.Value.ToString("yyyy-MM-dd")]));

                //to update
                // How the input data should be interpreted.
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = (SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum)1;  // TODO: Update placeholder value.


                ValueRange requestBody = new ValueRange();
                //SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = sheetsService.Spreadsheets.Values.Update(requestBody, _spreadsheetId, range);
                SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest = _service.Spreadsheets.Values.Update(newValReq, _spreadsheetId, newrange);
                updateRequest.ValueInputOption = valueInputOption;

                // To execute asynchronously in an async method, replace `request.Execute()` as shown:
                UpdateValuesResponse updateResponse = updateRequest.Execute();

                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(updateResponse));
            }
            catch(Exception ex)
            {
                lblExceptionMessage.Text = ex.Message;
                return;
            }

            lblExceptionMessage.Text = "Successful! Thank you.";

        }

    }


    public class ServiceAccountJson
    {
        public string type { get; set; }
        public string project_id { get; set; }
        public string private_key_id { get; set; }
        public string private_key { get; set; }
        public string client_email { get; set; }
        public string client_id { get; set; }
        public string auth_uri { get; set; }
        public string token_uri { get; set; }
        public string auth_provider_x509_cert_url { get; set; }
        public string client_x509_cert_url { get; set; }
    };
}
