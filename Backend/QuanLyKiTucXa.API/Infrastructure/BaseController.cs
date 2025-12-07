using Microsoft.AspNetCore.Mvc;
using QuanLyKiTucXa.API.DTOs;

namespace QuanLyKiTucXa.API.Infrastructure;

/// <summary>
/// Base controller with common error handling and response formatting
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Returns a standardized success response
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> OkResponse<T>(T data, string? message = null)
    {
        return Ok(ApiResponseDto<T>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Returns a standardized created response
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> CreatedResponse<T>(string actionName, object? routeValues, T data)
    {
        return CreatedAtAction(actionName, routeValues, ApiResponseDto<T>.SuccessResponse(data));
    }

    /// <summary>
    /// Returns a standardized paginated response
    /// </summary>
    protected ActionResult<PaginatedResponseDto<T>> OkPaginatedResponse<T>(
        List<T> data,
        int totalCount,
        int pageNumber,
        int pageSize,
        string? message = null)
    {
        return Ok(PaginatedResponseDto<T>.SuccessResponse(data, totalCount, pageNumber, pageSize, message));
    }

    /// <summary>
    /// Returns a standardized error response for paginated requests
    /// </summary>
    protected ActionResult<PaginatedResponseDto<T>> BadPaginatedRequestResponse<T>(string message)
    {
        return BadRequest(PaginatedResponseDto<T>.ErrorResponse(message));
    }

    /// <summary>
    /// Returns a standardized error response for not found (paginated)
    /// </summary>
    protected ActionResult<PaginatedResponseDto<T>> NotFoundPaginatedResponse<T>(string message)
    {
        return NotFound(PaginatedResponseDto<T>.ErrorResponse(message));
    }

    /// <summary>
    /// Returns a standardized error response for bad request
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> BadRequestResponse<T>(string message, Dictionary<string, string[]>? errors = null)
    {
        return BadRequest(ApiResponseDto<T>.ErrorResponse(message, errors));
    }

    /// <summary>
    /// Returns a standardized error response for not found
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> NotFoundResponse<T>(string message)
    {
        return NotFound(ApiResponseDto<T>.ErrorResponse(message));
    }

    /// <summary>
    /// Returns a standardized error response for internal server error
    /// </summary>
    protected ActionResult<ApiResponseDto<T>> InternalErrorResponse<T>(string message = "An internal error occurred")
    {
        return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<T>.ErrorResponse(message));
    }

    /// <summary>
    /// Validates ModelState and returns error response if invalid
    /// </summary>
    protected ActionResult<ApiResponseDto<T>>? ValidateModelState<T>()
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

            return BadRequest(ApiResponseDto<T>.ErrorResponse("Validation failed", errors));
        }
        return null;
    }
}
