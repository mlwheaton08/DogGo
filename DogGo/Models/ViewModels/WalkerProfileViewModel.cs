namespace DogGo.Models.ViewModels;

public class WalkerProfileViewModel
{
    public Walker Walker { get; set; }
    public List<Walks> Walks { get; set; }
    public int WalkTime { get; set; }
}