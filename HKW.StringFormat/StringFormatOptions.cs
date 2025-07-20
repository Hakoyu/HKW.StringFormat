using System.ComponentModel;

namespace HKW.HKWStringFormat;

/// <summary>
/// 字符串格式化设置
/// </summary>
public class StringFormatOptions
{
    private static readonly Lazy<StringFormatOptions> _lazy = new(() => new StringFormatOptions());

    /// <summary>
    /// 默认 StringFormatOptions
    /// </summary>
    public static StringFormatOptions Default => _lazy.Value;

    /// <summary>
    /// 成员名称的最大长度
    /// </summary>
    [DefaultValue(1024)]
    public int MaximumMemberNameLength { get; set; } = 1024;

    /// <summary>
    /// 仅限有名称的成员
    /// </summary>
    [DefaultValue(true)]
    public bool OnlyHasNameMembers { get; set; } = true;

    /// <summary>
    /// 格式化 <see langword="null"/> 值
    /// <para><c>{FormatName}</c>是格式化名称</para>
    /// </summary>
    [DefaultValue("{{FormatName}.NULL}")]
    public string FormatNullValue { get; set; } = "{{FormatName}.NULL}";
}
