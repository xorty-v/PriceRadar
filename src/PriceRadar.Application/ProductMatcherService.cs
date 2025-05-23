using FuzzySharp;
using PriceRadar.Domain.Entities;
using PriceRadar.Domain.Interfaces;

namespace PriceRadar.Application;

internal sealed class ProductMatcherService : IProductMatcherService
{
    private const int SimilarityThreshold = 90;
    private readonly ICategoryRepository _categoryRepository;

    private readonly IOfferRepository _offerRepository;
    private readonly IProductRepository _productRepository;

    public ProductMatcherService(
        IOfferRepository offerRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _offerRepository = offerRepository;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task MatchOffersToProductsAsync()
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync();

        foreach (var category in categories)
        {
            var offers = await _offerRepository.GetOffersByCategoryAsync(category.Id);
            var products = await _productRepository.GetProductsByCategoryAsync(category.Id);

            var newProducts = new List<Product>();

            foreach (var offer in offers)
            {
                var bestMatch = products
                    .Select(p => new { Product = p, Score = Fuzz.Ratio(p.Name, offer.Name) })
                    .OrderByDescending(m => m.Score)
                    .FirstOrDefault();

                if (bestMatch != null && bestMatch.Score >= SimilarityThreshold)
                {
                    offer.ProductId = bestMatch.Product.Id;
                }
                else
                {
                    var newProduct = new Product
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = offer.CategoryId,
                        Name = offer.Name
                    };

                    newProducts.Add(newProduct);
                    offer.ProductId = newProduct.Id;
                    products.Add(newProduct);
                }
            }

            if (newProducts.Any())
                await _productRepository.AddProductsAsync(newProducts);

            await _offerRepository.UpdateOffersAsync(offers);
        }
    }
}