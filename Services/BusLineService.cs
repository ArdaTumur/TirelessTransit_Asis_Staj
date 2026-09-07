using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.DTOs;
using MyAPI.Models;

namespace MyAPI.Services;

public class BusLineService
{
    private readonly AppDbContext _context;

    public BusLineService(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<BusLine>> GetAll() =>
        _context.BusLines
            .AsNoTracking()
            .Where(busLine => !busLine.IsDeleted)
            .OrderBy(busLine => busLine.Plate)
            .ToListAsync();

    public Task<BusLine?> GetById(int id) =>
        _context.BusLines
            .AsNoTracking()
            .FirstOrDefaultAsync(busLine => busLine.Id == id && !busLine.IsDeleted);

    public async Task<BusLine> Add(CreateBusLineDto dto, int createUserId)
    {
        var busLine = new BusLine
        {
            Driver = dto.Driver.Trim(),
            Plate = dto.Plate.Trim().ToUpperInvariant(),
            Hours = dto.Hours.Trim(),
            RouteStatus = dto.RouteStatus.Trim(),
            OriginCity = dto.OriginCity.Trim(),
            DestinationCity = dto.DestinationCity.Trim(),
            CreateUserId = createUserId
        };

        _context.BusLines.Add(busLine);
        await _context.SaveChangesAsync();

        return busLine;
    }

    public async Task<BusLine?> Update(int id, UpdateBusLineDto dto, int updateUserId)
    {
        var busLine = await _context.BusLines
            .FirstOrDefaultAsync(busLine => busLine.Id == id && !busLine.IsDeleted);

        if (busLine is null)
        {
            return null;
        }

        busLine.Driver = dto.Driver.Trim();
        busLine.Plate = dto.Plate.Trim().ToUpperInvariant();
        busLine.Hours = dto.Hours.Trim();
        busLine.RouteStatus = dto.RouteStatus.Trim();
        busLine.OriginCity = dto.OriginCity.Trim();
        busLine.DestinationCity = dto.DestinationCity.Trim();
        busLine.UpdatedDate = DateTime.UtcNow;
        busLine.UpdateUserId = updateUserId;
        await _context.SaveChangesAsync();

        return busLine;
    }

    public async Task<bool> Delete(int id)
    {
        var busLine = await _context.BusLines
            .FirstOrDefaultAsync(busLine => busLine.Id == id && !busLine.IsDeleted);

        if (busLine is null)
        {
            return false;
        }

        busLine.IsDeleted = true;
        await _context.SaveChangesAsync();

        return true;
    }
}
