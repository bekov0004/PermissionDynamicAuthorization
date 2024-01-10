using Core.Model.ResponseModel;
using Core.ViewModel;

namespace Core.Interface;

public interface IStudentService
{
    Task<Response<StudentCreateRequest>> CreateStudent(StudentCreateRequest request);
    Task<Response<StudentUpdateRequest>> UpdateStudent(StudentUpdateRequest request);
    Task<Response<List<StudentListResponse>>> GetStudent();
}
