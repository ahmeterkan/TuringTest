
// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client
// Install-Package Newtonsoft.Json

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TuringTest
{
    class Program
    {


        static void Main(string[] args)
        {
            InvokeRequestResponseService().Wait();
            Console.ReadKey();
        }


        static async Task InvokeRequestResponseService()
        {
            string answer;
            Console.WriteLine("Answer gir:");
            answer = Console.ReadLine().ToString();
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "Answer", answer
                                            },
                                            {
                                                "Class", ""
                                            },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "fBvNzAu7y/ZWIiO9dSBORiGt/A4h5N3nkl1LNVOAo6slULtVixIe3fDXuqgEiUpbIrPOTbh33VlxWsx1koOqFQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/30c360b67da74a6b9cd8f824654164b4/services/1132923a147f43c1ab14097f24ce3d5b/execute?api-version=2.0&format=swagger");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    JObject azureobj = JObject.Parse(result);
                    Console.WriteLine("\n\n\n");
                    string Answer = (string)azureobj["Results"]["output1"][0]["Answer"];
                    string Class = (string)azureobj["Results"]["output1"][0]["Class"];
                    string PreprocessedAnswer = (string)azureobj["Results"]["output1"][0]["Preprocessed Answer"];
                    string ScoredLabels = (string)azureobj["Results"]["output1"][0]["Scored Labels"];
                    float ScoredProbabilities = (float)azureobj["Results"]["output1"][0]["Scored Probabilities"] * 100;
                    Console.WriteLine("Answer: " + Answer + "\nPreprocessed Answer: " + PreprocessedAnswer +
                        "\nScored Labels: " + ScoredLabels + "\nScored Probabilities: %" + ScoredProbabilities);

                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }

            }
        }
    }
}