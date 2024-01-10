using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Interface;
using Core.Model.ResponseModel;
using Core.ViewModel;
using Infrastructure.Data;
using Infrastructure.Entities; 
using Microsoft.EntityFrameworkCore;
using System.Net; 

namespace Core.Services;

public class StudentService : IStudentService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper; 
    public StudentService(DataContext dataContext,IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper; 
    }
    
     
    public async Task<Response<StudentCreateRequest>> CreateStudent(StudentCreateRequest request)
    {
        var existingGroup = await _dataContext.Groups.FirstOrDefaultAsync(x=>x.Id == request.GroupId);
        if (existingGroup == null) return new Response<StudentCreateRequest>(HttpStatusCode.NotFound,new List<string>(){"Group not found"});
        var mapped = _mapper.Map<Student>(request);
        await _dataContext.Students.AddAsync(mapped);
        await _dataContext.SaveChangesAsync();
        return new Response<StudentCreateRequest>(request);
    }

    public async Task<Response<StudentUpdateRequest>> UpdateStudent(StudentUpdateRequest request)
    {
        var existingStudent = await _dataContext.Students.FirstOrDefaultAsync(x=>x.Id == request.Id);
        if (existingStudent == null) return new Response<StudentUpdateRequest>(HttpStatusCode.NotFound,new List<string>(){"Student not found"});

        var existingGroup = await _dataContext.Groups.FirstOrDefaultAsync(x=>x.Id == request.GroupId);
        if (existingGroup == null) return new Response<StudentUpdateRequest>(HttpStatusCode.NotFound,new List<string>() { "Group not found" });
        
        _mapper.Map(request,existingStudent);
        await _dataContext.SaveChangesAsync();
        return new Response<StudentUpdateRequest>(request);
    }

    public async Task<Response<List<StudentListResponse>>> GetStudent()
    { 
        var query = await _dataContext.Students.ProjectTo<StudentListResponse>(_mapper.ConfigurationProvider).Where(x=>x.FullName.Contains("Anushervon")).Take(50).ToListAsync();
        
        return new Response<List<StudentListResponse>>(query);
    }

}
