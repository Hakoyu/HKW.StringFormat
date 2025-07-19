using System.Collections;
using System.Reflection;
using System.Text;
using HKW.FastMember;

namespace HKW.HKWStringFormat;

/// <summary>
/// 字符串格式化扩展
/// </summary>
public static class StringFormatExtensions
{
    #region FormatS

    /// <summary>
    /// 最简单的动态格式化, 不会获取任何成员特性
    /// <para>
    /// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    /// </para>
    /// <para>
    /// 若对象有相同名称的成员,则以入参顺序高的对象为主
    /// </para>
    /// </summary>
    /// <param name="format">格式化字符串</param>
    /// <param name="arg0">参数0</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <returns>格式化字符串</returns>
    public static string FormatS(
        this string format,
        object arg0,
        object arg1 = null!,
        object arg2 = null!
    )
    {
        var sb = new StringBuilder(format);
        FormatSAction(sb, arg0);
        if (arg1 is not null)
            FormatSAction(sb, arg1);
        if (arg2 is not null)
            FormatSAction(sb, arg2);

        return sb.ToString();
    }

    /// <summary>
    /// 最简单的动态格式化, 不会获取任何成员特性
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
    public static string FormatS(this string format, params object[] args)
    {
        var sb = new StringBuilder(format);
        foreach (var arg in args)
        {
            FormatSAction(sb, arg);
        }
        return sb.ToString();
    }

    private static void FormatSAction(StringBuilder sb, object obj)
    {
        if (obj is IDictionary dictionary)
        {
            FormatDictionaryAction(sb, dictionary);
        }
        else
        {
            var accessor = TypeAccessor.Create(obj.GetType());
            foreach (var member in accessor.GetMembers())
            {
                sb.Replace($"{{{member.Name}}}", accessor[obj, member.Name].ToString());
            }
        }
    }

    #endregion

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
    /// <param name="arg0">参数0</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(
        this string format,
        object arg0,
        object arg1 = null!,
        object arg2 = null!
    )
    {
        var options = StringFormatOptions.Default;
        var sb = new StringBuilder(format);
        FormatXAction(options, format, sb, arg0);
        if (arg1 is not null)
            FormatXAction(options, format, sb, arg1);
        if (arg2 is not null)
            FormatXAction(options, format, sb, arg2);

        return sb.ToString();
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
        var options = StringFormatOptions.Default;
        var sb = new StringBuilder(format);
        foreach (var arg in args)
        {
            FormatXAction(options, format, sb, arg);
        }
        return sb.ToString();
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
    /// <param name="arg0">参数0</param>
    /// <param name="arg1">参数1</param>
    /// <param name="arg2">参数2</param>
    /// <returns>格式化字符串</returns>
    public static string FormatX(
        this string format,
        StringFormatOptions options,
        object arg0,
        object arg1 = null!,
        object arg2 = null!
    )
    {
        var sb = new StringBuilder(format);
        FormatXAction(options, format, sb, arg0);
        if (arg1 is not null)
            FormatXAction(options, format, sb, arg1);
        if (arg2 is not null)
            FormatXAction(options, format, sb, arg2);

        return sb.ToString();
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
        var sb = new StringBuilder(format);
        foreach (var arg in args)
        {
            FormatXAction(options, format, sb, arg);
        }
        return sb.ToString();
    }

    private static StringBuilder FormatXAction(
        StringFormatOptions options,
        string format,
        StringBuilder sb,
        object obj
    )
    {
        if (obj is IDictionary dictionary)
        {
            FormatDictionaryAction(sb, dictionary);
        }
        else
        {
            FormatXObjectAction(options, format, sb, obj);
        }
        return sb;
    }

    private static void FormatXObjectAction(
        StringFormatOptions options,
        string format,
        StringBuilder sb,
        object obj
    )
    {
        var accessor = TypeAccessor.Create(obj.GetType());
        Span<char> nameSpan = stackalloc char[options.MaximumMemberNameLength];
        var nameMaxIndex = options.MaximumMemberNameLength - 1;
        var nameIndex = -1;
        for (var i = format.Length - 1; i >= 0; i--)
        {
            var c = format[i];
            if (c == '}')
            {
                nameIndex = nameMaxIndex;
            }
            else if (c == '{')
            {
                var name = nameSpan[(nameIndex + 1)..^0].ToString();
                var formatName = name;
                if (accessor.GetMemberDictionary().TryGetValue(name, out var member) is false)
                    continue;

                if (Attribute.IsDefined(member.MemberInfo, typeof(FormatIgnoreAttribute)))
                    continue;

                if (Attribute.IsDefined(member.MemberInfo, typeof(FormatNameAttribute)))
                {
                    var attribute = (FormatNameAttribute)
                        Attribute.GetCustomAttribute(
                            member.MemberInfo,
                            typeof(FormatNameAttribute)
                        )!;
                    if (string.IsNullOrWhiteSpace(attribute.Name))
                        formatName = attribute.Name;
                }
                else if (options.OnlyHasNameMembers)
                {
                    continue;
                }

                sb.Replace(
                    $"{{{formatName}}}",
                    accessor[obj, name].ToString(),
                    i,
                    nameMaxIndex - nameIndex + 2
                );
                nameIndex = 0;
            }
            else if (nameIndex >= 0)
            {
                if (c == '\n' || c == '\r')
                {
                    nameIndex = -1;
                    continue;
                }
                nameSpan[nameIndex--] = c;
            }
        }
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
