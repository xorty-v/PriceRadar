using PriceRadar.Application.Abstractions.Parsers;
using PriceRadar.Domain;

namespace PriceRadar.Application;

public interface ICategoryMapperService
{
    public Guid? Map(StoreType store, string categoryName);
}

public class CategoryMapperService : ICategoryMapperService
{
    private readonly Dictionary<StoreType, Dictionary<string, Guid>> _map;

    public CategoryMapperService()
    {
        _map = new Dictionary<StoreType, Dictionary<string, Guid>>
        {
            [StoreType.Alta] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Notebooks"] = Constants.PredefinedIds.Categories.Laptops,
                ["Smartphones"] = Constants.PredefinedIds.Categories.Smartphones,
                ["Monitors"] = Constants.PredefinedIds.Categories.Monitors,
                ["Headphones"] = Constants.PredefinedIds.Categories.Headphones,
                ["Keyboards"] = Constants.PredefinedIds.Categories.Keyboards,
                ["Mouses"] = Constants.PredefinedIds.Categories.Mouses
            },
            [StoreType.EliteElectronic] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Notebook"] = Constants.PredefinedIds.Categories.Laptops,
                ["note-pc"] = Constants.PredefinedIds.Categories.Laptops,
                ["Mobile Phone"] = Constants.PredefinedIds.Categories.Smartphones,
                ["Mobile-Phone"] = Constants.PredefinedIds.Categories.Smartphones,
                ["Monitor"] = Constants.PredefinedIds.Categories.Monitors,
                ["monitor"] = Constants.PredefinedIds.Categories.Monitors,
                ["Headphones"] = Constants.PredefinedIds.Categories.Headphones,
                ["Keyboard"] = Constants.PredefinedIds.Categories.Keyboards,
                ["Mouse"] = Constants.PredefinedIds.Categories.Mouses
            },
            [StoreType.Zoommer] = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Laptop brands"] = Constants.PredefinedIds.Categories.Laptops,
                ["Mobile Phones"] = Constants.PredefinedIds.Categories.Smartphones,
                ["Monitors"] = Constants.PredefinedIds.Categories.Monitors,
                ["Headphones & Headsets"] = Constants.PredefinedIds.Categories.Headphones,
                ["Keyboards"] = Constants.PredefinedIds.Categories.Keyboards,
                ["Mouse"] = Constants.PredefinedIds.Categories.Mouses
            }
        };
    }

    public Guid? Map(StoreType store, string categoryName)
    {
        if (_map.TryGetValue(store, out var categoryDict) &&
            categoryDict.TryGetValue(categoryName.Trim(), out var category))
        {
            return category;
        }

        return null;
    }
}