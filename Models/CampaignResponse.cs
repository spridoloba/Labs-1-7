using Newtonsoft.Json;

namespace DSP.Models
{
    public class CampaignResponse
    {
        
        [JsonProperty("_advertising_campaign_id_")]
        public Campaign Campaign { get; set; }
    }

    public class Campaign
    {
        public int Id { get; set; }
        public string Language { get; set; }
        public string Name { get; set; }

        
    }

}
