namespace PaymentLink.Domain.ProductReadModels.Entities;

public class ProductReadModel
{
    public string Id { get; private set; }
    public IEnumerable<PriceReadModel> Prices { get; private set; }
    public bool LiveMode { get; private set; }
    public string UserId { get; private set; }

    public ProductReadModel(string id, string userId, bool liveMode)
    {
        Id = id;
        LiveMode = liveMode;
        UserId = userId;
    }

    public void SetPrices(IEnumerable<PriceReadModel> prices)
    {
        Prices = prices;
    }
    
    private ProductReadModel()
    {}
}