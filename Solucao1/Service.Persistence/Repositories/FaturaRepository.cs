using Microsoft.EntityFrameworkCore;
using Service.Entities.Models;
using Service.Persistence.Contexts;
using Service.Persistence.Repositories.Interfaces;

namespace Service.Persistence.Repositories;

public class FaturaRepository(
    AppDbContext context
) : GenericRepository<FaturaModel>(context),
    IFaturaRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<FaturaModel>> GetAllWithItemsAsync()
    {
        return await _context.Faturas.Include(x => x.Items).ToListAsync();
    }

    public async Task<FaturaModel?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Faturas.Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<FaturaModel>> Get10MoreExpensiveAsync()
    {
        return await _context.Faturas.OrderByDescending(x => x.Total).Take(10).ToListAsync();
    }

    public async Task<double> GetTotalByClientAsync(string client)
    {
        return await _context.Faturas.Where(x => x.Client == client).Select(x => x.Total).SumAsync();
    }

    public async Task<double> GetTotalByYearAndMonth(int year, int month)
    {
        return await _context.Faturas
            .Where(f => f.DataCriacao.Year == year && f.DataCriacao.Month == month)
            .SumAsync(f => f.Total);
    }
}
