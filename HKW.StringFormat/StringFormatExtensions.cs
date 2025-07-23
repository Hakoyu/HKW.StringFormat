using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using HKW.FastMember;

namespace HKW.HKWStringFormat;

/// <summary>
/// 字符串格式化扩展
/// </summary>
public static class StringFormatExtensions
{
    #region FormatX
    /// <summary>
    /// 动态格式化, 会获取成员特性, 使用默认设置
    /// <para>
    /// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    /// </para>
    /// <para>
    /// 若对象有相同名称的成员,则以入参顺序高的对象为主
    /// </para>
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="arg">参数</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(this string format, object arg)
    {
        return FormatX(format, StringFormatOptions.Default, arg);
    }

    /// <summary>
    /// 动态格式化, 会获取成员特性
    /// <para>
    /// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    /// </para>
    /// <para>
    /// 若对象有相同名称的成员,则以入参顺序高的对象为主
    /// </para>
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="options">格式化设置</param>
    /// <param name="arg">参数</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(this string format, StringFormatOptions options, object arg)
    {
        if (arg is FormatData formatData)
        {
            return format.Replace(formatData.Name, formatData.Value.ToString());
        }
        else
        {
            return FormatX(format, options, args: arg);
        }
    }

    /// <summary>
    /// 动态格式化, 会获取成员特性, 使用默认设置
    /// <para>
    /// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    /// </para>
    /// <para>
    /// 若对象有相同名称的成员,则以入参顺序高的对象为主
    /// </para>
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="args">参数</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(this string format, params object[] args)
    {
        return FormatX(format, StringFormatOptions.Default, args);
    }

    /// <summary>
    /// 动态格式化, 会获取成员特性
    /// <para>
    /// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    /// </para>
    /// <para>
    /// 若对象有相同名称的成员,则以入参顺序高的对象为主
    /// </para>
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="options">格式化设置</param>
    /// <param name="args">参数</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(
        this string format,
        StringFormatOptions options,
        params object[] args
    )
    {
        var formatNames = new List<FormatName>();
        Span<char> nameSpan = stackalloc char[options.MaximumMemberNameLength];
        var nameMaxIndex = nameSpan.Length - 1;
        var nameIndex = nameMaxIndex;
        var valueByFormatName = GetValueByFormatName(options, args);
        for (var i = 0; i < format.Length; i++)
        {
            var c = format[i];
            if (c == '{')
            {
                nameIndex = 0;
            }
            else if (c == '}')
            {
                var name = new string(nameSpan[..nameIndex]);
                if (valueByFormatName.TryGetValue(name, out var value))
                    formatNames.Add(new(name, i - name.Length - 1, value));
                nameIndex = nameMaxIndex;
            }
            else if (nameIndex < nameMaxIndex)
            {
                if (c == '\n' || c == '\r')
                {
                    nameIndex = nameMaxIndex;
                    continue;
                }
                nameSpan[nameIndex++] = c;
            }
        }

        var sb = new StringBuilder();
        var lastIndex = 0;
        for (var i = 0; i < formatNames.Count; i++)
        {
            var formatName = formatNames[i];
            var dataLength = formatName.Index - lastIndex;
            sb.Append(format, lastIndex, dataLength);
            // 添加值
            sb.Append(
                formatName.Value?.ToString()
                    ?? options.FormatNullValue.FormatX(
                        options,
                        new FormatData("{FormatName}", formatName.Name)
                    )
            );
            lastIndex += dataLength + formatName.Name.Length + 2;
        }

        if (lastIndex < format.Length)
        {
            sb.Append(format, lastIndex, format.Length - lastIndex);
        }
        return sb.ToString();
    }

    private static Dictionary<string, object?> GetValueByFormatName(
        StringFormatOptions options,
        params object[] args
    )
    {
        var valueByFormatName = new Dictionary<string, object?>();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg is IDictionary dictionary)
            {
                foreach (DictionaryEntry de in dictionary)
                {
                    valueByFormatName.TryAdd(de.Key.ToString()!, de.Value);
                }
            }
            else if (arg is FormatData formatData)
            {
                valueByFormatName.TryAdd(formatData.Name, formatData.Value);
            }
            else if (arg is IEnumerable<FormatData> formatDatas)
            {
                foreach (var data in formatDatas)
                {
                    valueByFormatName.TryAdd(data.Name, data.Value);
                }
            }
            else
            {
                var accessor = TypeAccessor.Create(arg.GetType());
                var members = accessor.GetMembers();
                for (var j = 0; j < members.Length; j++)
                {
                    var member = members[j];
                    var formatName = member.Name;
                    if (member.IsDefined<FormatIgnoreAttribute>())
                        continue;
                    if (member.IsDefined<FormatNameAttribute>())
                    {
                        var attribute = (FormatNameAttribute)
                            member.GetAttribute<FormatNameAttribute>()!;
                        if (string.IsNullOrWhiteSpace(attribute.Name) is false)
                            formatName = attribute.Name;
                    }
                    else if (options.OnlyHasNameMembers)
                    {
                        continue;
                    }
                    valueByFormatName.TryAdd(formatName, accessor[arg, member.Name]);
                }
            }
        }

        return valueByFormatName;
    }

    private static KeyValuePair<string, object?>? GetFormatNameAndValue(
        StringFormatOptions options,
        object arg
    )
    {
        if (arg is IDictionary dictionary)
        {
            foreach (DictionaryEntry de in dictionary)
            {
                return new(de.Key.ToString()!, de.Value!);
            }
        }
        else if (arg is FormatData formatData)
        {
            return new(formatData.Name, formatData.Value);
        }
        else if (arg is IEnumerable<FormatData> formatDatas)
        {
            foreach (var data in formatDatas)
            {
                return new(data.Name, data.Value);
            }
        }
        else
        {
            var accessor = TypeAccessor.Create(arg.GetType());
            var members = accessor.GetMembers();
            for (var j = 0; j < members.Length; j++)
            {
                var member = members[j];
                var formatName = member.Name;
                if (member.IsDefined<FormatIgnoreAttribute>())
                    continue;
                if (member.IsDefined<FormatNameAttribute>())
                {
                    var attribute = (FormatNameAttribute)
                        member.GetAttribute<FormatNameAttribute>()!;
                    if (string.IsNullOrWhiteSpace(attribute.Name) is false)
                        formatName = attribute.Name;
                }
                else if (options.OnlyHasNameMembers)
                {
                    continue;
                }
                return new(formatName, accessor[arg, member.Name]);
            }
        }
        return null;
    }
    #endregion

    private static void FormatDictionaryAction(StringBuilder sb, IDictionary dictionary)
    {
        foreach (var key in dictionary.Keys)
        {
            sb.Replace($"{{{key}}}", dictionary[key]!.ToString());
        }
    }
}

/// <summary>
/// 格式化信息
/// </summary>
/// <param name="Name">名称</param>
/// <param name="Index">索引值</param>
/// <param name="Value">格式化的值</param>
internal readonly record struct FormatName(string Name, int Index, object? Value);
