using System.Collections.Concurrent;
using InstructorSyllabus.Domain;

namespace InstructorSyllabus.Data;

public class EfInstructorSyllabusRepository : IInstructorSyllabusRepository
{
    private static readonly ConcurrentDictionary<string, StudentSyllabusItemModel> _items = new();
    private static int _nextId = 1;

    static EfInstructorSyllabusRepository()
    {
        SeedDefaults();
    }

    private static void SeedDefaults()
    {
        var item1 = new StudentSyllabusItemModel
        {
            Id = 1,
            StudentId = 2,
            SyllabusId = 1,
            SyllabusItemId = 1,
            ItemName = "Basic Box Step",
            CategoryName = "Bronze Steps",
            IsIntroduction = true,
            IsCompleted = true
        };
        var item2 = new StudentSyllabusItemModel
        {
            Id = 2,
            StudentId = 2,
            SyllabusId = 1,
            SyllabusItemId = 2,
            ItemName = "Underarm Turn",
            CategoryName = "Bronze Steps",
            IsIntroduction = true,
            IsCompleted = false
        };

        _items.TryAdd($"{item1.StudentId}_{item1.SyllabusId}_{item1.SyllabusItemId}", item1);
        _items.TryAdd($"{item2.StudentId}_{item2.SyllabusId}_{item2.SyllabusItemId}", item2);
    }

    public Task<IEnumerable<SyllabusSummaryModel>> GetSyllabiAsync(CancellationToken cancellationToken = default)
    {
        var list = new List<SyllabusSummaryModel>
        {
            new(1, "Bronze Ballroom Syllabus", "Full beginner-to-intermediate standard steps", "Bronze"),
            new(2, "Silver Latin Syllabus", "Advanced Latin figures including Samba, Rumba, Cha Cha", "Silver"),
            new(3, "Gold Smooth Syllabus", "Mastery level Viennese Waltz, Foxtrot, Tango", "Gold")
        };
        return Task.FromResult<IEnumerable<SyllabusSummaryModel>>(list);
    }

    public Task<object> GetStudentSyllabusAsync(int studentId, int syllabusId, CancellationToken cancellationToken = default)
    {
        var matching = _items.Values
            .Where(i => i.StudentId == studentId && i.SyllabusId == syllabusId)
            .ToList();

        if (!matching.Any())
        {
            matching.Add(new StudentSyllabusItemModel
            {
                Id = 1,
                StudentId = studentId,
                SyllabusId = syllabusId,
                SyllabusItemId = 1,
                ItemName = "Basic Movement",
                CategoryName = "Foundation",
                IsIntroduction = true,
                IsCompleted = false
            });
        }

        var categories = matching
            .GroupBy(m => m.CategoryName)
            .Select(g => new
            {
                name = g.Key,
                items = g.Select(i => new
                {
                    id = i.SyllabusItemId,
                    title = i.ItemName,
                    is_introduction = i.IsIntroduction,
                    is_completed = i.IsCompleted
                }).ToList()
            }).ToList();

        var res = new
        {
            syllabus_id = syllabusId,
            syllabus_title = syllabusId == 1 ? "Bronze Ballroom Syllabus" : "Latin Syllabus",
            categories = categories,
            items = matching.Select(i => new
            {
                id = i.SyllabusItemId,
                title = i.ItemName,
                is_introduction = i.IsIntroduction,
                is_completed = i.IsCompleted
            }).ToList()
        };

        return Task.FromResult<object>(res);
    }

    public Task<StudentSyllabusItemModel> UpdateStudentSyllabusAsync(UpdateStudentSyllabusDto dto, CancellationToken cancellationToken = default)
    {
        var key = $"{dto.StudentId}_{dto.SyllabusId}_{dto.SyllabusItemId}";
        var model = new StudentSyllabusItemModel
        {
            Id = Interlocked.Increment(ref _nextId),
            StudentId = dto.StudentId,
            SyllabusId = dto.SyllabusId,
            SyllabusItemId = dto.SyllabusItemId,
            ItemName = "Syllabus Item " + dto.SyllabusItemId,
            IsIntroduction = dto.IsIntroduction,
            IsCompleted = dto.IsCompleted,
            UpdatedAt = DateTime.UtcNow
        };
        _items[key] = model;
        return Task.FromResult(model);
    }
}
