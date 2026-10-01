using BuildingBlocks.Persistence;
using MyTasks.Domain;

namespace MyTaska.Data;

 public class EfMyTaskRepository : IMyTaskRepository
{
    private readonly AppContext _context;
    public EfMyTaskRepository(AppContext context)
    {
        _context=context;
    }

    public async Task<MyTaskItemModel> CreateAsync(MyTaskItemModel myTask, CancellationToken cancellationToken = default)
    {
        var item=myTask with
        {
            Id = myTask.Id==Guid.Empty?Guid.NewGuid() : myTask.Id,
            CreateAt=myTask.CreateAt==default? DateTime.UtcNow:myTask.CreateAt
        };

        await _context.MyTasks.AddAsync(item, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }
    public async Task<MyTaskItemModel> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _context.MyTasks.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id ==id,cancellationToken=default);

        return item;
        
    }

    public async Task<IEnumerable<MyTaskItemModel>> ListAsync(CancellationToken cancellationToken)
    { var tasks=await _context.MyTasks.AsNoTracking()
                .OrderByDescending(x=x.CreateAt)
                .ToListAsync(cancellationToken);

                return tasks;
        
    }

    public async Task<MyTaskItemModel> UpdateAsync(Guid id, MyTaskItemModel myTask, CancellationToken cancellationToken = default)
    {
        var existing = await _context.MyTasks.FindAsync(new object[]{id},cancellationToken);
        if (existing is null)
        {
            return null;
            
        }
        _context.Entity(existing).State=EntityState.Detached;

        
    }




}