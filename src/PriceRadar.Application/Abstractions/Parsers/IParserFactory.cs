namespace PriceRadar.Application.Abstractions.Parsers;

public interface IParserFactory
{
    public IParser CreateParser(StoreType storeType);
}