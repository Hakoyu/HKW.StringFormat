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
}
