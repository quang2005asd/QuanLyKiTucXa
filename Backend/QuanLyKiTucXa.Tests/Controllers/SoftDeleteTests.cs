using Microsoft.EntityFrameworkCore;
using QuanLyKiTucXa.API.Data;
using QuanLyKiTucXa.API.Infrastructure;
using QuanLyKiTucXa.API.Models;
using AutoMapper;
using Xunit;
using QuanLyKiTucXa.API.Controllers;

namespace QuanLyKiTucXa.Tests.Controllers;

public class SoftDeleteFilterTests
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SoftDeleteFilterTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async Task SoftDelete_ShouldExcludeDeletedRecords_InNormalQueries()
    {
        // Arrange
        var student = new Student
        {
            StudentCode = "STU001",
            FullName = "Test Student",
            Email = "test@example.com",
            Status = "Active"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var studentId = student.Id;

        // Act - Soft delete
        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        _context.Students.Update(student);
        await _context.SaveChangesAsync();

        // Assert - Normal query should not return deleted record
        var normalQuery = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
        Assert.Null(normalQuery);

        // But IgnoreQueryFilters should return it
        var filteredQuery = await _context.Students.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == studentId);
        Assert.NotNull(filteredQuery);
        Assert.True(filteredQuery.IsDeleted);
    }

    [Fact]
    public async Task SoftDelete_ShouldWorkForAllEntities()
    {
        // Arrange
        var building = new Building
        {
            Name = "Building A",
            Address = "123 Main St",
            TotalFloors = 5
        };
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();
        var buildingId = building.Id;

        // Act - Soft delete
        building.IsDeleted = true;
        building.DeletedAt = DateTime.UtcNow;
        _context.Buildings.Update(building);
        await _context.SaveChangesAsync();

        // Assert
        var normalQuery = await _context.Buildings.FirstOrDefaultAsync(b => b.Id == buildingId);
        Assert.Null(normalQuery);

        var filteredQuery = await _context.Buildings.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == buildingId);
        Assert.NotNull(filteredQuery);
    }

    [Fact]
    public async Task PaginationWithSoftDelete_ShouldOnlyCountActiveRecords()
    {
        // Arrange
        var students = new List<Student>
        {
            new Student { StudentCode = "STU001", FullName = "Student 1", Email = "s1@example.com", Status = "Active" },
            new Student { StudentCode = "STU002", FullName = "Student 2", Email = "s2@example.com", Status = "Active" },
            new Student { StudentCode = "STU003", FullName = "Student 3", Email = "s3@example.com", Status = "Active" }
        };
        _context.Students.AddRange(students);
        await _context.SaveChangesAsync();

        // Act - Soft delete one student
        var deletedStudent = students.First();
        deletedStudent.IsDeleted = true;
        deletedStudent.DeletedAt = DateTime.UtcNow;
        _context.Students.Update(deletedStudent);
        await _context.SaveChangesAsync();

        // Assert
        var totalCount = await _context.Students.CountAsync();
        Assert.Equal(2, totalCount);

        var allCount = await _context.Students.IgnoreQueryFilters().CountAsync();
        Assert.Equal(3, allCount);
    }
}
