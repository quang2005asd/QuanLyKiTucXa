using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QuanLyKiTucXa.API.Infrastructure;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Quản Lý Ký Túc Xá API",
            Version = "v1.0",
            Description = "API for dormitory management system",
            Contact = new OpenApiContact
            {
                Name = "Development Team",
                Email = "support@quantucxa.local"
            },
            License = new OpenApiLicense
            {
                Name = "MIT License"
            }
        });

        // Add JWT Authentication to Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Configure response type handling
        options.OperationFilter<SwaggerOperationFilter>();
        options.SchemaFilter<SwaggerSchemaFilter>();
    }
}

public class SwaggerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common response codes
        if (!operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad Request - Invalid input data"
            });
        }

        if (!operation.Responses.ContainsKey("401"))
        {
            operation.Responses.Add("401", new OpenApiResponse
            {
                Description = "Unauthorized - Invalid or missing authentication token"
            });
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal Server Error"
            });
        }

        // Add pagination parameters documentation for List endpoints
        if (operation.OperationId?.Contains("Get") == true && operation.OperationId?.Contains("s", StringComparison.OrdinalIgnoreCase) == true)
        {
            var pageNumberParam = operation.Parameters?.FirstOrDefault(p => p.Name == "pageNumber");
            var pageSizeParam = operation.Parameters?.FirstOrDefault(p => p.Name == "pageSize");

            if (pageNumberParam != null)
            {
                pageNumberParam.Description = "Page number (1-based indexing). Default: 1";
            }
            if (pageSizeParam != null)
            {
                pageSizeParam.Description = "Number of items per page. Default: 10";
            }
        }
    }
}

public class SwaggerSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Add examples for common DTOs
        var type = context.Type;

        if (type?.Name == "PaginatedResponseDto`1")
        {
            schema.Description = "Paginated response wrapper containing list of items with pagination metadata";
        }
        else if (type?.Name == "ApiResponseDto`1")
        {
            schema.Description = "Standard API response wrapper containing success status, message, data, and optional error details";
        }
        else if (type?.Name == "PaginationMetaDto")
        {
            schema.Description = "Pagination metadata including total count, page numbers, and navigation flags";
        }
    }
}
