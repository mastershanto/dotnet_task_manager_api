using BuildingBlocks.Persistence;
using StudentChildren.Domain;

namespace StudentChildren.Data;

public class EfStudentChildrenRepository : IStudentChildrenRepository
{
    private readonly AppDbContext _db;
    private static readonly List<StudentKidModel> _inMemoryKids = new()
    {
        new StudentKidModel
        {
            Id = 29,
            Name = "Leo Rivera",
            Email = "leo.rivera@example.com",
            Avatar = "https://i.pravatar.cc/150?u=leo",
            Gender = "male",
            Birthday = "2016-04-12",
            DateOfBirth = "2016-04-12",
            Phone = "+1 555-0199",
            Address = "124 Sunset Blvd, Los Angeles, CA",
            Type = "child",
            ParentId = 1,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-3)
        },
        new StudentKidModel
        {
            Id = 30,
            Name = "Maya Rivera",
            Email = "maya.rivera@example.com",
            Avatar = "https://i.pravatar.cc/150?u=maya",
            Gender = "female",
            Birthday = "2018-09-24",
            DateOfBirth = "2018-09-24",
            Phone = "+1 555-0199",
            Address = "124 Sunset Blvd, Los Angeles, CA",
            Type = "child",
            ParentId = 1,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow.AddMonths(-2)
        }
    };

    public EfStudentChildrenRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<IEnumerable<StudentKidModel>> GetChildrenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<StudentKidModel>>(_inMemoryKids);
    }

    public Task<StudentKidModel?> GetChildByIdAsync(int childId, CancellationToken cancellationToken = default)
    {
        var found = _inMemoryKids.FirstOrDefault(k => k.Id == childId);
        return Task.FromResult(found);
    }

    public Task<StudentKidModel> CreateChildAsync(CreateStudentKidDto dto, CancellationToken cancellationToken = default)
    {
        var newId = _inMemoryKids.Count > 0 ? _inMemoryKids.Max(k => k.Id) + 1 : 1;
        var kid = new StudentKidModel
        {
            Id = newId,
            Name = dto.Name,
            Email = dto.Email,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            Birthday = dto.DateOfBirth,
            Phone = dto.Phone,
            Address = dto.Address,
            Type = "child",
            ParentId = 1,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };
        _inMemoryKids.Add(kid);
        return Task.FromResult(kid);
    }

    public Task<StudentKidModel?> UpdateChildAsync(int childId, UpdateStudentKidDto dto, CancellationToken cancellationToken = default)
    {
        var existing = _inMemoryKids.FirstOrDefault(k => k.Id == childId);
        if (existing == null) return Task.FromResult<StudentKidModel?>(null);

        var updated = existing with
        {
            Name = dto.Name ?? existing.Name,
            Email = dto.Email ?? existing.Email,
            Gender = dto.Gender ?? existing.Gender,
            DateOfBirth = dto.DateOfBirth ?? existing.DateOfBirth,
            Birthday = dto.DateOfBirth ?? existing.Birthday,
            Phone = dto.Phone ?? existing.Phone,
            Address = dto.Address ?? existing.Address
        };

        var index = _inMemoryKids.IndexOf(existing);
        _inMemoryKids[index] = updated;

        return Task.FromResult<StudentKidModel?>(updated);
    }

    public Task<bool> DeleteChildAsync(int childId, CancellationToken cancellationToken = default)
    {
        var existing = _inMemoryKids.FirstOrDefault(k => k.Id == childId);
        if (existing != null)
        {
            _inMemoryKids.Remove(existing);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
