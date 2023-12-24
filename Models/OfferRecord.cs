using DSP.Service;

namespace DSP.Models
{
    public class OfferRecord
    {
        public int ID { get; set; }
        public int PeerclickSourceId { get; set; } = 0;
        public string Country { get; set; }
        public int OnOff { get; set; } = 1;
        public int ScheduleEnabled { get; set; } = 0;
        public string Schedule { get; set; } = "null";
        public int Mode { get; set; } = 30;
        public string RoiForSources { get; set; }
        public string RoiForUsers { get; set; }
        public int Turbo { get; set; } = 0;
        public int NoLeads { get; set; } = 1;
        public int CLimit { get; set; } = 0;
        public int StartMcpm { get; set; } = 0;
        public int SpendLimitDaily { get; set; } = 0;
        public int LeadLimitDaily { get; set; } = 0;
        public string Cat { get; set; }
        public async Task SetCategoryAsync(int categoryId, DatabaseService databaseService)
        {
            this.Cat = await databaseService.GetCategoryFromIdAsync(categoryId);
        }
    
        public int Adult { get; set; } = 0;
        public int AdFormat { get; set; } = 0;
        public int N2S { get; set; } = 1;
        public int RetargetMode { get; set; } = 1;
        public string Lang { get; set; }
        public string LangFilter { get; set; }
        public string OsFilter { get; set; } = ""; 
        public string RegionFilter { get; set; } = "";  
        public string CityFilter { get; set; } = ""; 
        public string AdType { get; set; }
        public string LandingType { get; set; } = "r";
        public string Dep { get; set; } = "salt";
        public int Team { get; set; } = 0;

        public int Approve { get; set; } = 100;
        public string Offer { get; set; }
        public string Payout { get; set; }
        public string ButtonText { get; set; }
        public string Pp { get; set; } = "t";
        public string Crid { get; set; } = ""; 
        public string Owner { get; set; }
        public int MgidId { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = ""; 
        public string Image { get; set; }
        public string Brand { get; set; } = ""; 
        public int ShowAnim { get; set; } = 0;
        public string Tags { get; set; }
        public int AbtestOn { get; set; } = 0;
        public string Pp2 { get; set; } = ""; 
        public string Crid2 { get; set; } = ""; 
        public string Owner2 { get; set; } = ""; 
        public int MgidId2 { get; set; } = 0;
        public string Link2 { get; set; } = ""; 
        public string Title2 { get; set; } = ""; 
        public string Description2 { get; set; } = ""; 
        public string Image2 { get; set; } = ""; 
        public string Brand2 { get; set; } = ""; 
        public int ShowAnim2 { get; set; } = 0;
        public string Tags2 { get; set; } = ""; 
        public int StasDone { get; set; } = 0;
        public string Comment { get; set; } = ""; 
        public string MetaData { get; set; } = "null";
    }

}
