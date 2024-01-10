using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interface;
using Core.Model.ResponseModel;
using Core.ViewModel;
using Infrastructure.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;
[Route("api/[controller]/[action]")]
[EnableCors("AllowSpecificOrigin")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [Authorize(Permissions.Students.GetStudent)] 
    public async Task<Response<List<StudentListResponse>>> GetStudent()
    {
        return await _studentService.GetStudent();
    }

    [HttpPut]
    [Authorize(Permissions.Students.UpdateStudent)]
    public async Task<Response<StudentUpdateRequest>> UpdateStudent(StudentUpdateRequest request)
    {
        return await _studentService.UpdateStudent(request);
    }

    [HttpPost]
    [Authorize(Permissions.Students.CreateStudent)]
    public async Task<Response<StudentCreateRequest>> CreateStudent(StudentCreateRequest request)
    {
        return await _studentService.CreateStudent(request);
    }


    [HttpDelete]
    [Authorize(Permissions.Students.DeleteStudent)]
    public Response<string> DeleteStudent(string id)
    {
        return new Response<string>("Студент удалён");
    }
}
