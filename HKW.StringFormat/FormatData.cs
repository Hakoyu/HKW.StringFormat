namespace HKW.HKWStringFormat;

/// <summary>
/// 格式化数据
/// </summary>
public readonly struct FormatData(string name, object value)
{
    /// <summary>
    /// 格式化名称
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// 格式化值
    /// </summary>
    public readonly object Value = value;
}
