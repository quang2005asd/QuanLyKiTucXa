using AutoMapper;
using QuanLyKiTucXa.API.DTOs;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;
using Xunit;

namespace QuanLyKiTucXa.Tests.Infrastructure;

public class MappingProfileTests
{
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void StudentToStudentDto_ShouldMapCorrectly()
    {
        // Arrange
        var student = new Student
        {
            Id = 1,
            StudentCode = "STU001",
            FullName = "Nguyễn Văn A",
            Email = "student@example.com",
            PhoneNumber = "0123456789",
            Status = "Active",
            ProfileImage = "https://example.com/image.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<StudentDto>(student);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(student.Id, dto.Id);
        Assert.Equal(student.StudentCode, dto.StudentCode);
        Assert.Equal(student.FullName, dto.FullName);
        Assert.Equal(student.Email, dto.Email);
        Assert.Equal(student.PhoneNumber, dto.PhoneNumber);
        Assert.Equal(student.Status, dto.Status);
    }

    [Fact]
    public void CreateStudentDtoToStudent_ShouldMapCorrectly()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            StudentCode = "STU002",
            FullName = "Nguyễn Văn B",
            Email = "studentb@example.com",
            PhoneNumber = "0987654321"
        };

        // Act
        var student = _mapper.Map<Student>(createDto);

        // Assert
        Assert.NotNull(student);
        Assert.Equal(createDto.StudentCode, student.StudentCode);
        Assert.Equal(createDto.FullName, student.FullName);
        Assert.Equal(createDto.Email, student.Email);
        Assert.Equal(createDto.PhoneNumber, student.PhoneNumber);
    }

    [Fact]
    public void UpdateStudentDtoToStudent_ShouldMapNonNullValues()
    {
        // Arrange
        var student = new Student
        {
            Id = 1,
            StudentCode = "STU001",
            FullName = "Original Name",
            Email = "original@example.com",
            PhoneNumber = "0123456789",
            Status = "Active"
        };

        var updateDto = new UpdateStudentDto
        {
            FullName = "Updated Name"
        };

        // Act
        _mapper.Map(updateDto, student);

        // Assert
        Assert.Equal("Updated Name", student.FullName);
    }

    [Fact]
    public void MappingProfile_ShouldBeValid()
    {
        // Arrange
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());

        // Act & Assert
        config.AssertConfigurationIsValid();
    }
}
