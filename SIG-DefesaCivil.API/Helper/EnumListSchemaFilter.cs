using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SIG_DefesaCivil.API.ValidationAttributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace SIG_DefesaCivil.API.Helper
{
    public class EnumListSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Se não houver informações do membro (propriedade), não faz nada
            if (context.MemberInfo == null) return;

            // Busca o atributo [EnumList] na propriedade
            var enumListAttribute = context.MemberInfo.GetCustomAttribute<EnumListAttribute>();

            if (enumListAttribute != null)
            {
                var enumType = enumListAttribute._enumType;

                if (enumType != null && enumType.IsEnum)
                {
                    // 1. Recupera os nomes do Enum
                    var enumNames = Enum.GetNames(enumType);

                    if (schema.Type == "array" && schema.Items != null)
                    {
                        schema.Items.Enum = enumNames
                            .Select(name => new OpenApiString(name))
                            .ToList<IOpenApiAny>();
                    }
                }
            }
        }
    }
}