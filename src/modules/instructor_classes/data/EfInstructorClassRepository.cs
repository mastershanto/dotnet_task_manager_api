using System.Collections.Concurrent;
using InstructorClasses.Domain;

namespace InstructorClasses.Data;

public class EfInstructorClassRepository : IInstructorClassRepository
{
    private static readonly ConcurrentDictionary<int, InstructorClassModel> _classes = new();
    private static int _nextId = 10;

    static EfInstructorClassRepository()
    {
        SeedDefaults();
    }

    private static void SeedDefaults()
    {
        var class1 = new InstructorClassModel
        {
            Id = 1,
            Title = "Intermediate Salsa & Bachata",
            Type = "group",
            StartTime = "18:00:00",
            EndTime = "19:00:00",
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            IsCompleted = false,
            StudioId = 1,
            StudentCount = 6,
            Studio = new(1, "Main Ballroom - Sunset Dance", "123 Sunset Blvd", "Los Angeles"),
            Program = new(1, "Latin Fusion Program", "Comprehensive Salsa, Bachata, and Merengue", 199.99m),
            Category = new(1, "Group Classes", "#3B82F6"),
            Users = new List<ClassUserSummary>
            {
                new(16, "Sarah Jenkins", "sarah@example.com", "https://sunsetdance.thewarriors.team/assets/images/user1.png", true),
                new(4, "Amara Chen", "amara@example.com", "https://sunsetdance.thewarriors.team/assets/images/user2.png", false),
                new(9, "Michael Scott", "michael@example.com", "https://sunsetdance.thewarriors.team/assets/images/user3.png", true)
            },
            Instructors = new List<ClassUserSummary>
            {
                new(1, "Alex Rivera", "alex@sunsetdance.com", "https://sunsetdance.thewarriors.team/assets/images/alex.png", null)
            }
        };

        var class2 = new InstructorClassModel
        {
            Id = 2,
            Title = "Advanced Argentine Tango",
            Type = "private",
            StartTime = "19:30:00",
            EndTime = "20:30:00",
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            IsCompleted = true,
            StudioId = 2,
            StudentCount = 2,
            Studio = new(2, "Studio B - Tango Room", "123 Sunset Blvd", "Los Angeles"),
            Program = new(2, "Private Masterclass", "1-on-1 private dance coaching", 299.99m),
            Category = new(2, "Private Classes", "#10B981"),
            Users = new List<ClassUserSummary>
            {
                new(4, "Amara Chen", "amara@example.com", "https://sunsetdance.thewarriors.team/assets/images/user2.png", true)
            },
            Instructors = new List<ClassUserSummary>
            {
                new(1, "Alex Rivera", "alex@sunsetdance.com", "https://sunsetdance.thewarriors.team/assets/images/alex.png", null)
            }
        };

        _classes.TryAdd(class1.Id, class1);
        _classes.TryAdd(class2.Id, class2);
    }

    public Task<IEnumerable<InstructorClassModel>> GetClassesAsync(string? date, CancellationToken cancellationToken = default)
    {
        var items = _classes.Values.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(date))
        {
            items = items.Where(c => c.Date == date);
        }
        return Task.FromResult<IEnumerable<InstructorClassModel>>(items.OrderBy(c => c.StartTime).ToList());
    }

    public Task<InstructorClassModel?> GetClassByIdAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        _classes.TryGetValue(lessonId, out var cls);
        return Task.FromResult(cls);
    }

    public Task<UpdateAttendanceResult?> UpdateAttendanceAsync(int lessonId, int studentId, CancellationToken cancellationToken = default)
    {
        if (_classes.TryGetValue(lessonId, out var cls))
        {
            var userIndex = cls.Users.FindIndex(u => u.Id == studentId);
            bool newAttended = true;
            if (userIndex >= 0)
            {
                newAttended = !(cls.Users[userIndex].IsAttended ?? false);
                cls.Users[userIndex] = cls.Users[userIndex] with { IsAttended = newAttended };
            }
            return Task.FromResult<UpdateAttendanceResult?>(new UpdateAttendanceResult(lessonId, studentId, newAttended));
        }
        return Task.FromResult<UpdateAttendanceResult?>(null);
    }

    public Task<object?> GetClassStudentDetailsAsync(int lessonId, int studentId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object?>(new
        {
            lesson_id = lessonId,
            student_id = studentId,
            name = "Sarah Jenkins",
            email = "sarah@example.com",
            phone = "+1 555-0192",
            avatar = "https://sunsetdance.thewarriors.team/assets/images/user1.png",
            level = "Silver 1",
            classes_remaining = 14,
            is_attended = true,
            notes = "Strong posture, needs polish on foot turnouts."
        });
    }

    public Task<IEnumerable<RepeatPatternModel>> GetRepeatPatternsAsync(CancellationToken cancellationToken = default)
    {
        var patterns = new List<RepeatPatternModel>
        {
            new("none", "Does Not Repeat"),
            new("daily", "Every Day"),
            new("weekly", "Every Week"),
            new("biweekly", "Every 2 Weeks"),
            new("monthly", "Every Month")
        };
        return Task.FromResult<IEnumerable<RepeatPatternModel>>(patterns);
    }

    public Task<InstructorClassModel> CreateClassAsync(CreateClassDto dto, CancellationToken cancellationToken = default)
    {
        var id = Interlocked.Increment(ref _nextId);
        var cls = new InstructorClassModel
        {
            Id = id,
            Title = dto.Title,
            Type = dto.Type,
            Date = dto.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            StudioId = dto.StudioId,
            StudentCount = dto.StudentIds?.Count ?? 0,
            Studio = new(dto.StudioId, "Studio " + dto.StudioId, "123 Sunset Blvd", "Los Angeles"),
            Category = new(dto.CategoryId ?? 1, "Dance", "#3B82F6")
        };
        _classes[id] = cls;
        return Task.FromResult(cls);
    }

    public Task<bool> DeleteClassAsync(int classId, CancellationToken cancellationToken = default)
    {
        _classes.TryRemove(classId, out _);
        return Task.FromResult(true);
    }

    public Task<bool> AddInstructorAsync(AddInstructorDto dto, CancellationToken cancellationToken = default)
    {
        if (_classes.TryGetValue(dto.LessonId, out var cls))
        {
            if (!cls.Instructors.Any(i => i.Id == dto.InstructorId))
            {
                cls.Instructors.Add(new(dto.InstructorId, "Instructor " + dto.InstructorId, null, null, null));
            }
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> AddStudentAsync(AddStudentDto dto, CancellationToken cancellationToken = default)
    {
        if (_classes.TryGetValue(dto.LessonId, out var cls))
        {
            if (!cls.Users.Any(u => u.Id == dto.StudentId))
            {
                cls.Users.Add(new(dto.StudentId, "Student " + dto.StudentId, null, null, false));
            }
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<IEnumerable<ClassUserSummary>> SearchStudentsAsync(string? q, CancellationToken cancellationToken = default)
    {
        var list = new List<ClassUserSummary>
        {
            new(4, "Amara Chen", "amara@example.com", "https://sunsetdance.thewarriors.team/assets/images/user2.png", null),
            new(16, "Sarah Jenkins", "sarah@example.com", "https://sunsetdance.thewarriors.team/assets/images/user1.png", null),
            new(9, "Michael Scott", "michael@example.com", "https://sunsetdance.thewarriors.team/assets/images/user3.png", null)
        };
        if (!string.IsNullOrWhiteSpace(q))
        {
            list = list.Where(s => s.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return Task.FromResult<IEnumerable<ClassUserSummary>>(list);
    }

    public Task<IEnumerable<ClassUserSummary>> SearchInstructorsAsync(string? q, CancellationToken cancellationToken = default)
    {
        var list = new List<ClassUserSummary>
        {
            new(1, "Alex Rivera", "alex@sunsetdance.com", "https://sunsetdance.thewarriors.team/assets/images/alex.png", null),
            new(2, "Carlos Santana", "carlos@sunsetdance.com", "https://sunsetdance.thewarriors.team/assets/images/carlos.png", null)
        };
        if (!string.IsNullOrWhiteSpace(q))
        {
            list = list.Where(s => s.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return Task.FromResult<IEnumerable<ClassUserSummary>>(list);
    }

    public Task<IEnumerable<StudioSummary>> SearchStudiosAsync(string? q, CancellationToken cancellationToken = default)
    {
        var list = new List<StudioSummary>
        {
            new(1, "Main Ballroom - Sunset Dance", "123 Sunset Blvd", "Los Angeles"),
            new(2, "Studio B - Tango Room", "123 Sunset Blvd", "Los Angeles"),
            new(3, "River Studio", "456 River Rd", "Los Angeles")
        };
        if (!string.IsNullOrWhiteSpace(q))
        {
            list = list.Where(s => s.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return Task.FromResult<IEnumerable<StudioSummary>>(list);
    }
}
