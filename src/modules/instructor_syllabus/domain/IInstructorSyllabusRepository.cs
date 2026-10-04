namespace InstructorSyllabus.Domain;

public interface IInstructorSyllabusRepository
{
    Task<IEnumerable<SyllabusSummaryModel>> GetSyllabiAsync(CancellationToken cancellationToken = default);
    Task<object> GetStudentSyllabusAsync(int studentId, int syllabusId, CancellationToken cancellationToken = default);
    Task<StudentSyllabusItemModel> UpdateStudentSyllabusAsync(UpdateStudentSyllabusDto dto, CancellationToken cancellationToken = default);
}
