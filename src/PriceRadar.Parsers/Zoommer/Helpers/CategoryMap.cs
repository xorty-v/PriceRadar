namespace PriceRadar.Parsers.Zoommer.Helpers;

public static class CategoryMap
{
    private static Dictionary<(int categoryId, int subCategoryId), string> _categoryMap;

    static CategoryMap()
    {
        _categoryMap = new Dictionary<(int categoryId, int subCategoryId), string>
        {
            [(533, 846)] = "Headphones & Headsets",
            [(528, 811)] = "Headphones & Headsets",
            [(463, 956)] = "Headphones & Headsets",
            [(528, 791)] = "Headphones & Headsets",
            [(528, 810)] = "Headphones & Headsets",
            [(533, 908)] = "Headphones & Headsets",
            [(528, 964)] = "Headphones & Headsets",
            [(528, 792)] = "Headphones & Headsets",
            [(893, 1017)] = "Headphones & Headsets",
            [(533, 666)] = "Headphones & Headsets",
            [(528, 786)] = "Headphones & Headsets",
            [(887, 618)] = "Headphones & Headsets",
            [(887, 1237)] = "Headphones & Headsets",
            [(887, 1035)] = "Headphones & Headsets",
            [(528, 1226)] = "Headphones & Headsets",
            [(528, 783)] = "Headphones & Headsets",
            [(887, 617)] = "Headphones & Headsets",
            [(533, 906)] = "Headphones & Headsets",
            [(528, 798)] = "Headphones & Headsets",
            [(533, 457)] = "Headphones & Headsets",
            [(528, 963)] = "Headphones & Headsets",
            [(533, 905)] = "Headphones & Headsets",
            [(887, 458)] = "Headphones & Headsets",
            [(528, 747)] = "Headphones & Headsets",
            [(528, 795)] = "Headphones & Headsets",
            [(533, 903)] = "Headphones & Headsets",
            [(700, 695)] = "Keyboards",
            [(893, 663)] = "Keyboards",
            [(506, 635)] = "Keyboards",
            [(531, 563)] = "Laptop brands",
            [(531, 564)] = "Laptop brands",
            [(531, 708)] = "Laptop brands",
            [(463, 838)] = "Laptop brands",
            [(675, 894)] = "Laptop brands",
            [(717, 644)] = "Laptop brands",
            [(675, 669)] = "Laptop brands",
            [(675, 671)] = "Laptop brands",
            [(717, 721)] = "Laptop brands",
            [(855, 724)] = "Mobile Phones",
            [(855, 814)] = "Mobile Phones",
            [(855, 809)] = "Mobile Phones",
            [(855, 808)] = "Mobile Phones",
            [(855, 554)] = "Mobile Phones",
            [(855, 815)] = "Mobile Phones",
            [(855, 806)] = "Mobile Phones",
            [(855, 968)] = "Mobile Phones",
            [(855, 625)] = "Mobile Phones",
            [(855, 948)] = "Mobile Phones",
            [(855, 1243)] = "Mobile Phones",
            [(503, 550)] = "Monitors",
            [(503, 831)] = "Monitors",
            [(503, 825)] = "Monitors",
            [(503, 643)] = "Monitors",
            [(503, 1222)] = "Monitors",
            [(503, 817)] = "Monitors",
            [(503, 647)] = "Monitors",
            [(503, 829)] = "Monitors",
            [(503, 816)] = "Monitors",
            [(1140, 722)] = "Monitors",
            [(1140, 553)] = "Monitors",
            [(1140, 830)] = "Monitors",
            [(463, 668)] = "Monitors",
            [(700, 698)] = "Mouses",
            [(893, 1018)] = "Mouses",
        };
    }

    public static string GetCategoryName(int categoryId, int subCategoryId)
    {
        return _categoryMap.TryGetValue((categoryId, subCategoryId), out var name)
            ? name
            : "Unknown";
    }
}