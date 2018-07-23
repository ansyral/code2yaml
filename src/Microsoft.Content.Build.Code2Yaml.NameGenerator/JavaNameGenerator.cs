﻿namespace Microsoft.Content.Build.Code2Yaml.NameGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using Microsoft.Content.Build.Code2Yaml.Constants;
    using Microsoft.Content.Build.Code2Yaml.Utility;

    public class JavaNameGenerator : INameGenerator
    {
        public override string GenerateTypeFullName(NameGeneratorContext context, XElement node, bool withTypeParameters)
        {
            string fullname = YamlUtility.RegularizeName(context.CurrentChange.Name, Constants.Dot);
            if (node != null && withTypeParameters)
            {
                fullname += GetTypeParameterString(node);
            }
            return fullname;
        }

        public override string GenerateTypeName(NameGeneratorContext context, XElement node, bool withTypeParameters)
        {
            string name = YamlUtility.RegularizeName(YamlUtility.ParseNameFromFullName(context.CurrentChange.Type, context.ParentChange?.Name, context.CurrentChange.Name), Constants.Dot);
            if (node != null && withTypeParameters)
            {
                name += GetTypeParameterString(node);
            }
            return name;
        }

        public override string GenerateMemberFullName(NameGeneratorContext context, XElement node)
        {
            string memberDefName = YamlUtility.RegularizeName(YamlUtility.ParseMemberName(node.NullableElement("definition").NullableValue(), node.NullableElement("argsstring").NullableValue()), Constants.Dot);
            string regularizedCompoundName = YamlUtility.RegularizeName(context.CurrentChange.Name, Constants.Dot);
            int indexOfMemberFullName = memberDefName.IndexOf(regularizedCompoundName);
            return String.IsNullOrEmpty(regularizedCompoundName) || String.IsNullOrEmpty(memberDefName) || indexOfMemberFullName == -1 ? memberDefName : memberDefName.Substring(indexOfMemberFullName);
        }

        public override string GenerateMemberName(NameGeneratorContext context, XElement node)
        {
            return YamlUtility.RegularizeName(YamlUtility.ParseMemberName(node.NullableElement("name").NullableValue(), node.NullableElement("argsstring").NullableValue()), Constants.Dot);
        }

        public override string GenerateMemberNameWithType(string memberName, string typeName)
        {
            return string.Join(Constants.Dot, typeName, memberName);
        }

        public override string GenerateId(NameGeneratorContext context, XElement node)
        {
            return YamlUtility.ParseIdFromUid(node.NullableAttribute("id").NullableValue(), Constants.Dot);
        }

        public override string GenerateLabel(NameGeneratorContext context, XElement node)
        {
            return YamlUtility.RegularizeName(node.NullableElement("label").NullableValue(), Constants.Dot);
        }

        private static string GetTypeParameterString(XElement node)
        {
            var builder = new StringBuilder();
            var templateParamList = node.NullableElement("templateparamlist").Elements("param").ToList();
            if (templateParamList.Count > 0)
            {
                builder.Append("<" + templateParamList[0].NullableElement("type").NullableValue());
                foreach (var param in templateParamList.Skip(1))
                {
                    builder.Append("," + param.NullableElement("type").NullableValue());
                }
                builder.Append(">");
            }
            return builder.ToString();
        }
    }
}
