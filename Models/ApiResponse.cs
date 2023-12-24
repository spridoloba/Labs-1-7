public class ApiResponse
{
    public string imageLink { get; set; }
    public string url { get; set; }
    public string title { get; set; }

    public string campaignId { get; set; }


    public string language { get; set; }
    

    public CategoryInfo Category { get; set; }

    public class CategoryInfo
    {
        public int Id { get; set; }
        
    }

}