namespace bidService.Models;

public class Bid
{

    public int BidID { get; set; }   
    public int ProductID {get; set;}
    
    public int BidUserID { get; set; }
    public int BidPrice {get; set;}
}