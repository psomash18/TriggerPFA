using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace TriggerPFA
{
	public static class TriggerPFA
	{
		[FunctionName("TriggerPFA")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "TriggerPFA")] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");
			string flowUrl = "https://prod-58.westus.logic.azure.com:443/workflows/a5ab879f5f0a4c33b2af47d8812ee41a/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=tbjzwDG5Ki7BKcF17gnCNU3DMTwSaKe1htfmitHijS8";

			var data = new DataMembrs
			{
				FirstName = "Final",
				LastName = "withoutmail",
				Email = "pradeep.somas@hcl.com",
				StartDate = new DateTime(2024, 8, 13, 10, 00, 00).ToLocalTime(),
				EndDate = new DateTime(2024, 8, 13, 10, 30, 00).ToLocalTime(),
				OrganizerId = "fbe46d9a-55cd-ee11-907a-002248243577",
				//OrganizerId = "e083b994-b9cd-ee11-907a-002248243577",
				Phone = "7259266918",
				Subject="All None Final"
			};

			string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);



			using (var httpClient = new HttpClient())
			{
				var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

				var response = await httpClient.PostAsync(flowUrl, content);
				

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					var jsonString = JsonConvert.DeserializeObject(responseBody);
					JObject jsonObject = JObject.Parse(jsonString.ToString());
					string keyValue = (string)jsonObject["activityid"];
					

					log.LogInformation($"Response from Power Automate flow: {keyValue}");
					return new OkObjectResult("Appointment id is :"+keyValue);
				}
				else
				{
					return new StatusCodeResult((int)response.StatusCode);
				}
			}

		}
	}
}
