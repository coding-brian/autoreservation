namespace Service.WordProcess
{
    public interface IWordPrcoessFactory
    {
        IWordProcess Create(string word, string userId = "");
    }
}