using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HKW.HKWStringFormat;

/// <summary>
/// 格式化名称
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class FormatNameAttribute : Attribute
{
    /// <inheritdoc/>
    public FormatNameAttribute(string Name)
    {
        this.Name = Name;
    }

    /// <summary>
    /// 格式化名称
    /// </summary>
    public string Name { get; set; }
}
