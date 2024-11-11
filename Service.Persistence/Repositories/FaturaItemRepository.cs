using Microsoft.EntityFrameworkCore;
using Service.Entities.Models;
using Service.Persistence.Contexts;
using Service.Persistence.Repositories.Interfaces;

namespace Service.Persistence.Repositories;

public class FaturaItemRepository(
    AppDbContext context
) : GenericRepository<FaturaItemModel>(context),
    IFaturaItemRepository
{
    private readonly AppDbContext _context = context;

    public async Task<FaturaItemModel?> GetByIdAndFaturaId(int id, int faturaId)
    {
        return await _context.FaturaItems.Include(x => x.Fatura).SingleOrDefaultAsync(x => x.Id == id && x.FaturaId == faturaId);
    }

    public async Task<IEnumerable<FaturaItemModel>> Get10MoreExpensives()
    {
        return await _context.FaturaItems.OrderByDescending(x => x.Valor).Take(10).ToListAsync();
    }
}
