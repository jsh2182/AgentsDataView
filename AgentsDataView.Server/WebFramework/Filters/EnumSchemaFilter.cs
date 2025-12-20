using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class EnumSchemaFilter : ISchemaFilter
{
    private readonly XDocument _xmlComments;

    public EnumSchemaFilter(string xmlPath)
    {
        if (System.IO.File.Exists(xmlPath))
            _xmlComments = XDocument.Load(xmlPath);
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum)
            return;

        var enumType = context.Type;
        var members = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static);

        var sb = new StringBuilder();
        sb.AppendLine("**مقادیر Enum:**  ");
        sb.AppendLine("| مقدار | عدد | توضیح |");
        sb.AppendLine("|:------|:----:|:------|");

        foreach (var member in members)
        {
            var value = Convert.ToInt32(Enum.Parse(enumType, member.Name));
            var display = member.GetCustomAttribute<DisplayAttribute>()?.Name ?? member.Name;

            // درست کردن نام برای XML lookup (تبدیل + به .)
            var fullName = $"F:{member.DeclaringType?.FullName?.Replace('+', '.')}.{member.Name}";
            var summary = _xmlComments?
                .Descendants("member")
                .FirstOrDefault(x => x.Attribute("name")?.Value == fullName)?
                .Descendants("summary")
                .FirstOrDefault()?
                .Value.Trim();

            sb.AppendLine($"| {display} | {value} | {(summary ?? "-")} |");
        }

        if (!string.IsNullOrEmpty(schema.Description))
            schema.Description += "\n\n";

        schema.Description += sb.ToString();
    }
}
