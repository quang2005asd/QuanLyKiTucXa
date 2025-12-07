using System.ComponentModel.DataAnnotations;
using QuanLyKiTucXa.API.DTOs;
using Xunit;

namespace QuanLyKiTucXa.Tests.DTOs;

public class StudentDtoValidationTests
{
    [Fact]
    public void CreateStudentDto_WithValidData_ShouldPass()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            StudentCode = "STU001",
            FullName = "Nguyễn Văn A",
            Email = "student@example.com",
            PhoneNumber = "0123456789"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    [Fact]
    public void CreateStudentDto_WithEmptyStudentCode_ShouldFail()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            StudentCode = "",
            FullName = "Nguyễn Văn A",
            Email = "student@example.com",
            PhoneNumber = "0123456789"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateStudentDto.StudentCode)));
    }

    [Fact]
    public void CreateStudentDto_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var dto = new CreateStudentDto
        {
            StudentCode = "STU001",
            FullName = "Nguyễn Văn A",
            Email = "invalid-email",
            PhoneNumber = "0123456789"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(CreateStudentDto.Email)));
    }

    [Fact]
    public void UpdateStudentDto_WithInvalidStatus_ShouldFail()
    {
        // Arrange
        var dto = new UpdateStudentDto
        {
            Status = "InvalidStatus"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.NotEmpty(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(UpdateStudentDto.Status)));
    }

    [Fact]
    public void UpdateStudentDto_WithValidStatus_ShouldPass()
    {
        // Arrange
        var dto = new UpdateStudentDto
        {
            Status = "Active"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.Empty(validationResults);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
