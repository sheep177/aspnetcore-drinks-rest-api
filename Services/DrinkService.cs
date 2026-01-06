using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Helpers;
using Drinks.API.Models;
using Drinks.API.ResourceParameters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Drinks.API.Services;

public class DrinkService : IDrinkService
{
    private readonly IDrinkRepo _repo;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    private const string DrinksListCacheKey = "drinks:list";

    public DrinkService(
        IDrinkRepo repo,
        IMapper mapper,
        IMemoryCache cache)
    {
        _repo = repo;
        _mapper = mapper;
        _cache = cache;
    }

    // ============================
    // GET all
    // ============================
    public async Task<PagedList<DrinksDto>> GetAllDrinksAsync(
        DrinksResourceParameters parameters)
    {
        if (_cache.TryGetValue(DrinksListCacheKey, out PagedList<DrinksDto> cached))
        {
            return cached;
        }

        // 1️⃣ Repo 返回的是 PagedList<Drink>
        var entities = await _repo.GetAllDrinksAsync(parameters);

        // 2️⃣ 在 PagedList 内部完成映射（关键）
        var result = entities.Map(d => _mapper.Map<DrinksDto>(d));

        // 3️⃣ cache
        _cache.Set(
            DrinksListCacheKey,
            result,
            TimeSpan.FromSeconds(60));

        return result;
    }

    // ============================
    // GET by id + ETag
    // ============================
    public async Task<(DrinksDto? drink, string etag)>
        GetDrinkWithETagAsync(int id)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null) return (null, string.Empty);

        var dto = _mapper.Map<DrinksDto>(entity);
        var etag = GenerateETag(entity.RowVersion);

        return (dto, etag);
    }

    public async Task<bool> DrinkExistsAsync(int id)
    {
        return await _repo.GetDrinkByIdAsync(id) != null;
    }

    // ============================
    // CREATE
    // ============================
    public async Task<DrinksDto> CreateDrinkAsync(
        DrinksForCreationDto input)
    {
        var entity = _mapper.Map<Drink>(input);
        _repo.CreateDrink(entity);
        await _repo.SaveDrinkAsync();

        InvalidateCache();
        return _mapper.Map<DrinksDto>(entity);
    }

    // ============================
    // UPDATE (PUT)
    // ============================
    public async Task<bool> UpdateDrinkAsync(
        int id,
        DrinksForUpdateDto input,
        string? ifMatch)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null) return false;

        var currentEtag = GenerateETag(entity.RowVersion);
        if (ifMatch == null || ifMatch != currentEtag)
        {
            throw new DbUpdateConcurrencyException();
        }

        _mapper.Map(input, entity);
        await _repo.SaveDrinkAsync();

        InvalidateCache(id);
        return true;
    }

    // ============================
    // DELETE
    // ============================
    public async Task<bool> DeleteDrinkAsync(int id)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null) return false;

        _repo.DeleteDrink(entity);
        await _repo.SaveDrinkAsync();

        InvalidateCache(id);
        return true;
    }
    
    public async Task<bool> PatchDrinkAsync(
        int id,
        DrinksPatchDto patchDto,
        string? ifMatch)
    {
        var entity = await _repo.GetDrinkByIdAsync(id);
        if (entity == null) return false;

        // 并发控制（必须）
        var currentEtag = GenerateETag(entity.RowVersion);
        if (string.IsNullOrWhiteSpace(ifMatch) || ifMatch != currentEtag)
        {
            throw new DbUpdateConcurrencyException();
        }

        // 应用 patch
        if (patchDto.Name != null)
            entity.Name = patchDto.Name;

        if (patchDto.Brand != null)
            entity.Brand = patchDto.Brand;

        if (patchDto.Price.HasValue)
            entity.Price = patchDto.Price.Value;

        await _repo.SaveDrinkAsync();

        InvalidateCache(id);
        return true;
    }

    // ============================
    // Helpers
    // ============================
    private static string GenerateETag(byte[] rowVersion)
        => $"\"{Convert.ToBase64String(rowVersion)}\"";

    private void InvalidateCache(int? id = null)
    {
        _cache.Remove(DrinksListCacheKey);
        if (id.HasValue)
        {
            _cache.Remove($"drink:{id}");
        }
    }
    
    
}