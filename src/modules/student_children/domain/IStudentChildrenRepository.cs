namespace StudentChildren.Domain;

public interface IStudentChildrenRepository
{
    Task<IEnumerable<StudentKidModel>> GetChildrenAsync(CancellationToken cancellationToken = default);
    Task<StudentKidModel?> GetChildByIdAsync(int childId, CancellationToken cancellationToken = default);
    Task<StudentKidModel> CreateChildAsync(CreateStudentKidDto dto, CancellationToken cancellationToken = default);
    Task<StudentKidModel?> UpdateChildAsync(int childId, UpdateStudentKidDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteChildAsync(int childId, CancellationToken cancellationToken = default);
}
