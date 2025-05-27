using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;


public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileParameter = context.MethodInfo.GetParameters()
            .FirstOrDefault(p => p.ParameterType == typeof(IFormFile));
        if (fileParameter != null)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = fileParameter.Name,
                //In = ParameterLocation.FormData,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                }
            });
        }
    }
}
