using Newtonsoft.Json;

namespace DSP.Service
{
    public class CampaignResponse
    {
        [JsonProperty("campaignId")]
        public Campaign Campaign { get; set; }
    }

    public class Campaign
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

       
    }

}
