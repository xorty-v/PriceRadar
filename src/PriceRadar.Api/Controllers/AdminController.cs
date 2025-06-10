using Microsoft.AspNetCore.Mvc;
using PriceRadar.Application;
using PriceRadar.Domain.Entities;

namespace PriceRadar.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IStoreCategoryService _storeCategoryService;
    private readonly IStoreService _storeService;

    public AdminController(
        ICategoryService categoryService,
        IStoreService storeService,
        IStoreCategoryService storeCategoryService)
    {
        _categoryService = categoryService;
        _storeService = storeService;
        _storeCategoryService = storeCategoryService;
    }

    [HttpGet("categories")]
    public async Task<IEnumerable<Category>> GetCategories() => await _categoryService.GetAllCategoriesAsync();

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory([FromQuery] string name)
    {
        await _categoryService.AddCategoryAsync(name);
        return Ok();
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromQuery] string name)
    {
        await _categoryService.UpdateCategoryAsync(id, name);
        return Ok();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return Ok();
    }

    [HttpPost("store-categories")]
    public async Task<IActionResult> AddStoreCategory(Guid storeId, Guid categoryId, string name, string url)
    {
        await _storeCategoryService.AddAsync(storeId, categoryId, name, url);
        return Ok();
    }

    [HttpPut("store-categories/{id}")]
    public async Task<IActionResult> UpdateStoreCategory(Guid id, Guid storeId, Guid categoryId, string name,
        string url)
    {
        await _storeCategoryService.UpdateAsync(id, storeId, categoryId, name, url);
        return Ok();
    }

    [HttpDelete("store-categories/{id}")]
    public async Task<IActionResult> DeleteStoreCategory(Guid id)
    {
        await _storeCategoryService.DeleteAsync(id);
        return Ok();
    }

    [HttpGet("stores")]
    public async Task<IEnumerable<Store>> GetAllStores() => await _storeService.GetAllAsync();

    [HttpPost("stores")]
    public async Task<IActionResult> AddStore(string name, string url)
    {
        await _storeService.AddAsync(name, url);
        return Ok();
    }

    [HttpPut("stores/{id}")]
    public async Task<IActionResult> UpdateStore(Guid id, string name, string url)
    {
        await _storeService.UpdateAsync(id, name, url);
        return Ok();
    }

    [HttpDelete("stores/{id}")]
    public async Task<IActionResult> DeleteStore(Guid id)
    {
        await _storeService.DeleteAsync(id);
        return Ok();
    }
}