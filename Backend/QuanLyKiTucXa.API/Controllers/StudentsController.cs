using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;

namespace QuanLyKiTucXa.API.Controllers;

public class StudentsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StudentsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<StudentDto>>> GetStudents(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadPaginatedRequestResponse<StudentDto>("Page number and page size must be greater than 0");

        var totalCount = await _context.Students.CountAsync();
        var students = await _context.Students
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var studentDtos = _mapper.Map<List<StudentDto>>(students);
        return OkPaginatedResponse(studentDtos, totalCount, pageNumber, pageSize);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFoundResponse<StudentDto>("Student not found");

        var studentDto = _mapper.Map<StudentDto>(student);
        return OkResponse(studentDto);
    }

    [HttpGet("code/{studentCode}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> GetStudentByCode(string studentCode)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
        if (student == null)
            return NotFoundResponse<StudentDto>("Student not found");

        var studentDto = _mapper.Map<StudentDto>(student);
        return OkResponse(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> PostStudent(CreateStudentDto createStudentDto)
    {
        var validationError = ValidateModelState<StudentDto>();
        if (validationError != null)
            return validationError;

        // Check if student code already exists
        if (await _context.Students.AnyAsync(s => s.StudentCode == createStudentDto.StudentCode))
            return BadRequestResponse<StudentDto>("Student code already exists");

        // Check if email already exists
        if (await _context.Students.AnyAsync(s => s.Email == createStudentDto.Email))
            return BadRequestResponse<StudentDto>("Email already exists");

        var student = _mapper.Map<Student>(createStudentDto);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var studentDto = _mapper.Map<StudentDto>(student);
        return CreatedResponse("GetStudent", new { id = student.Id }, studentDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<StudentDto>>> PutStudent(int id, UpdateStudentDto updateStudentDto)
    {
        var validationError = ValidateModelState<StudentDto>();
        if (validationError != null)
            return validationError;

        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFoundResponse<StudentDto>("Student not found");

        // Check if email is being updated and already exists
        if (!string.IsNullOrEmpty(updateStudentDto.Email) && updateStudentDto.Email != student.Email)
        {
            if (await _context.Students.AnyAsync(s => s.Email == updateStudentDto.Email))
                return BadRequestResponse<StudentDto>("Email already exists");
        }

        _mapper.Map(updateStudentDto, student);
        student.UpdatedAt = DateTime.UtcNow;

        try
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
                return NotFoundResponse<StudentDto>("Student not found");
            throw;
        }

        var studentDto = _mapper.Map<StudentDto>(student);
        return OkResponse(studentDto, "Student updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return NotFoundResponse<object>("Student not found");

        // Soft delete
        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        return OkResponse<object>(new { }, "Student deleted successfully");
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}
