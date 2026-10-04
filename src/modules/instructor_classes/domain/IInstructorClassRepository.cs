namespace InstructorClasses.Domain;

public interface IInstructorClassRepository
{
    Task<IEnumerable<InstructorClassModel>> GetClassesAsync(string? date, CancellationToken cancellationToken = default);
    Task<InstructorClassModel?> GetClassByIdAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<UpdateAttendanceResult?> UpdateAttendanceAsync(int lessonId, int studentId, CancellationToken cancellationToken = default);
    Task<object?> GetClassStudentDetailsAsync(int lessonId, int studentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RepeatPatternModel>> GetRepeatPatternsAsync(CancellationToken cancellationToken = default);
    Task<InstructorClassModel> CreateClassAsync(CreateClassDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteClassAsync(int classId, CancellationToken cancellationToken = default);
    Task<bool> AddInstructorAsync(AddInstructorDto dto, CancellationToken cancellationToken = default);
    Task<bool> AddStudentAsync(AddStudentDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassUserSummary>> SearchStudentsAsync(string? q, CancellationToken cancellationToken = default);
    Task<IEnumerable<ClassUserSummary>> SearchInstructorsAsync(string? q, CancellationToken cancellationToken = default);
    Task<IEnumerable<StudioSummary>> SearchStudiosAsync(string? q, CancellationToken cancellationToken = default);
}
