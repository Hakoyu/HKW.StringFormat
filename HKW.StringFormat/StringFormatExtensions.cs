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
    #region FormatS

    ///// <summary>
    ///// 最简单的动态格式化, 不会获取任何成员特性
    ///// <para>
    ///// 根据成员名格式化对应的占位符, 支持 <see langword="object"/> 和 <see cref="IDictionary"/>
    ///// </para>
    ///// <para>
    ///// 若对象有相同名称的成员,则以入参顺序高的对象为主
    ///// </para>
    ///// </summary>
    ///// <param name="format">格式化字符串</param>
    ///// <param name="arg0">参数0</param>
    ///// <param name="arg1">参数1</param>
    ///// <param name="arg2">参数2</param>
    ///// <returns>格式化字符串</returns>
    //public static string FormatS(
    //    this string format,
    //    object arg0,
    //    object arg1 = null!,
    //    object arg2 = null!
    //)
    //{
    //    var sb = new StringBuilder(format);
    //    FormatSAction(sb, arg0);
    //    if (arg1 is not null)
    //        FormatSAction(sb, arg1);
    //    if (arg2 is not null)
    //        FormatSAction(sb, arg2);

    //    return sb.ToString();
    //}

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
        var argDatas = GetArgDatas(options, args);
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
                for (var j = 0; j < argDatas.Length; j++)
                {
                    var argData = argDatas[j];
                    if (argData.Arg is IDictionary dictionary)
                    {
                        if (dictionary.Contains(name))
                            formatNames.Add(
                                new(name, i - name.Length - 1, dictionary[name]!.ToString()!)
                            );
                    }
                    else
                    {
                        var formatName = name;
                        if (argData.MemberByFormatName!.TryGetValue(name, out var member) is false)
                            continue;

                        if (
                            argData.Accessor!.TryGetValue(argData.Arg, member.Name, out var value)
                            is false
                        )
                            continue;
                        formatNames.Add(new(name, i - name.Length - 1, value.ToString()!));
                    }
                }
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
            sb.Append(formatName.Value);
            lastIndex += dataLength + formatName.Name.Length + 2;
        }

        if (lastIndex < format.Length)
        {
            sb.Append(format, lastIndex, format.Length - lastIndex);
        }
        return sb.ToString();
    }

    private static ArgData[] GetArgDatas(StringFormatOptions options, object[] args)
    {
        if (options.OnlyHasNameMembers)
        {
            return args.Select(static a =>
                {
                    var accessor = a is IDictionary ? null : TypeAccessor.Create(a.GetType());
                    return new ArgData(
                        a,
                        accessor,
                        accessor
                            ?.GetMembers()
                            .Where(static m =>
                                m.IsDefined(typeof(FormatIgnoreAttribute)) is false
                                && m.IsDefined(typeof(FormatNameAttribute))
                            )
                            .ToImmutableDictionary(
                                static m =>
                                {
                                    var attribute = (FormatNameAttribute)
                                        m.GetAttribute(typeof(FormatNameAttribute), false)!;
                                    if (string.IsNullOrWhiteSpace(attribute.Name) is false)
                                        return attribute.Name;
                                    return m.Name;
                                },
                                m => m.MemberInfo
                            )
                    );
                })
                .ToArray();
        }
        else
        {
            return args.Select(static a =>
                {
                    var accessor = a is IDictionary ? null : TypeAccessor.Create(a.GetType());
                    return new ArgData(
                        a,
                        accessor,
                        accessor
                            ?.GetMembers()
                            .Where(static m => m.IsDefined(typeof(FormatIgnoreAttribute)) is false)
                            .ToImmutableDictionary(
                                static m =>
                                {
                                    if (m.IsDefined(typeof(FormatNameAttribute)))
                                    {
                                        var attribute = (FormatNameAttribute)
                                            m.GetAttribute(typeof(FormatNameAttribute), false)!;
                                        if (string.IsNullOrWhiteSpace(attribute.Name) is false)
                                            return attribute.Name;
                                    }
                                    return m.Name;
                                },
                                m => m.MemberInfo
                            )
                    );
                })
                .ToArray();
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

/// <summary>
/// 格式化信息
/// </summary>
/// <param name="Name">名称</param>
/// <param name="Index">索引值</param>
/// <param name="Value">格式化的值</param>
public readonly record struct FormatName(string Name, int Index, string Value);

/// <summary>
/// 参数信息
/// </summary>
/// <param name="Arg">参数</param>
/// <param name="Accessor">类型访问器</param>
/// <param name="MemberByFormatName">成员和格式化名称</param>
public readonly record struct ArgData(
    object Arg,
    TypeAccessor? Accessor,
    ImmutableDictionary<string, MemberInfo>? MemberByFormatName
);
