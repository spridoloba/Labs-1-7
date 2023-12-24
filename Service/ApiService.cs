using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSP.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, (string ClientID, string ApiToken)> _participantMapping;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _participantMapping = new Dictionary<string, (string ClientID, string ApiToken)>
            {
                { "pridoloba", ("6122", "f49c97a1ce36b") },
                
                

                
            };
        }

        public async Task<ApiResponse> FetchDataForParticipant(string teaserId, string participant, int limit = 0, int start = 0)
        {
            if (!_participantMapping.TryGetValue(participant, out var clientDetails))
            {
                throw new ArgumentException($"Invalid participant: {participant}");
            }

            string clientID = clientDetails.ClientID;
            string apiToken = clientDetails.ApiToken;

            var jsonResponse = await GetClientTeasers(apiToken, clientID, teaserId, limit, start);

            if (string.IsNullOrWhiteSpace(participant))
            {
                throw new ArgumentException("Participant cannot be null or empty.");
            }



            if (!string.IsNullOrEmpty(jsonResponse))
            {
                return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            }
            else
            {
                
                return null;
            }


        }



        private async Task<string> GetClientTeasers(string apiToken, string clientID, string teaserID, int limit, int start)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            var url = $"https://api.mgid.com/v1/goodhits/clients/{clientID}/teasers";
            if (!string.IsNullOrEmpty(teaserID))
            {
                url += $"/{teaserID}";
            }

            string queryParameters = "";
            if (limit > 0)
            {
                queryParameters += $"limit={limit}&";
            }
            if (start > 0)
            {
                queryParameters += $"start={start}&";
            }

            var query = new UriBuilder(url)
            {
                Query = queryParameters.TrimEnd('&')
            }.Uri;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(query);
               

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: Request not successful. Status Code: {response.StatusCode}");
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody; 
            }
            catch (Exception e)
            {
               
                throw; 
            }
        }

        public async Task<List<string>> FetchImageLinksForParticipant(string teaserId, string participant, int limit = 0, int start = 0)
        {
           
            var response = await FetchDataForParticipant(teaserId, participant, limit, start);
            var imageLinks = new List<string>();
           
            if (!string.IsNullOrEmpty(response?.imageLink))
            {
                imageLinks.Add(response.imageLink);
            }

            return imageLinks;
        }



        public async Task<(string Geo, string ButtonText)> GetLanguageDetails(string languageId, string filePath)
        {
            string[] lines = await File.ReadAllLinesAsync(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts[0] == languageId)
                {
                    return (parts[1], parts[2]);
                }
            }
            return (null, null); 
        }



        public async Task<string> FetchLanguageForParticipant(string teaserId, string participant, int limit = 0, int start = 0)
        {
            if (!_participantMapping.TryGetValue(participant, out var clientDetails))
            {
                throw new ArgumentException($"Invalid participant: {participant}");
            }

            string clientID = clientDetails.ClientID;
            string apiToken = clientDetails.ApiToken;


            var campainIdd = await FetchcampaignIdForParticipant(teaserId, participant, limit, start);
            var jsonResponse = await GetClientCampaigns(apiToken, clientID, campainIdd);



            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new Exception("No response received from API");
            }


            return await GetLanguageFromCampaignResponse(jsonResponse);


        }


        private async Task<string> GetLanguageFromCampaignResponse(string jsonResponse)
        {
            var jsonObject = JObject.Parse(jsonResponse);

            
            string campaignIdKey = jsonObject.Properties().First().Name;

           
            string language = jsonObject["language"].ToString();

            return language;
        }


        private async Task<string> GetClientCampaigns(string apiToken, string clientID, string campaignID)
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

            var url = $"https://api.mgid.com/v1/goodhits/clients/{clientID}/campaigns";
            if (!string.IsNullOrEmpty(campaignID))
            {
                url += $"/{campaignID}";
            }

            var query = new UriBuilder(url)
            {
              
            }.Uri;

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(query);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: Request not successful. Status Code: {response.StatusCode}");
                }

                string otvetka = await response.Content.ReadAsStringAsync();
                return otvetka; 
            }
            catch (Exception e)
            {
                
                throw;
            }
        }






        public async Task<string> FetchcampaignIdForParticipant(string teaserId, string participant, int limit = 0, int start = 0)
            {
         
            var response = await FetchDataForParticipant(teaserId, participant, limit, start);

           
            return response?.campaignId;
        }


    }
}
