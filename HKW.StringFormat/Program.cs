namespace HKW.HKWStringFormat;

internal class Program
{
    static void Main(string[] args)
    {
        var obj = new FormatSampleClass();
        Console.WriteLine(FormatSampleData.FormatX(new(), obj));
    }

    public const string FormatSampleData = """
        AAAAAAAAAA{Content0}BBBBBBBBBB{Content0}CCCCCCCCCC{Content0}DDDDDDDDDD{Content0}EEEEEEEEEE{Content0}FFFFFFFFFF{Content0}GGGGGGGGGG{Content0}HHHHHHHHHH{Content0}IIIIIIIIII{Content0}JJJJJJJJJJ{Content0}
        AAAAAAAAAA{Content1}BBBBBBBBBB{Content1}CCCCCCCCCC{Content1}DDDDDDDDDD{Content1}EEEEEEEEEE{Content1}FFFFFFFFFF{Content1}GGGGGGGGGG{Content1}HHHHHHHHHH{Content1}IIIIIIIIII{Content1}JJJJJJJJJJ{Content1}
        AAAAAAAAAA{Content2}BBBBBBBBBB{Content2}CCCCCCCCCC{Content2}DDDDDDDDDD{Content2}EEEEEEEEEE{Content2}FFFFFFFFFF{Content2}GGGGGGGGGG{Content2}HHHHHHHHHH{Content2}IIIIIIIIII{Content2}JJJJJJJJJJ{Content2}
        AAAAAAAAAA{Content3}BBBBBBBBBB{Content3}CCCCCCCCCC{Content3}DDDDDDDDDD{Content3}EEEEEEEEEE{Content3}FFFFFFFFFF{Content3}GGGGGGGGGG{Content3}HHHHHHHHHH{Content3}IIIIIIIIII{Content3}JJJJJJJJJJ{Content3}
        AAAAAAAAAA{Content4}BBBBBBBBBB{Content4}CCCCCCCCCC{Content4}DDDDDDDDDD{Content4}EEEEEEEEEE{Content4}FFFFFFFFFF{Content4}GGGGGGGGGG{Content4}HHHHHHHHHH{Content4}IIIIIIIIII{Content4}JJJJJJJJJJ{Content4}
        AAAAAAAAAA{Content5}BBBBBBBBBB{Content5}CCCCCCCCCC{Content5}DDDDDDDDDD{Content5}EEEEEEEEEE{Content5}FFFFFFFFFF{Content5}GGGGGGGGGG{Content5}HHHHHHHHHH{Content5}IIIIIIIIII{Content5}JJJJJJJJJJ{Content5}
        AAAAAAAAAA{Content6}BBBBBBBBBB{Content6}CCCCCCCCCC{Content6}DDDDDDDDDD{Content6}EEEEEEEEEE{Content6}FFFFFFFFFF{Content6}GGGGGGGGGG{Content6}HHHHHHHHHH{Content6}IIIIIIIIII{Content6}JJJJJJJJJJ{Content6}
        AAAAAAAAAA{Content7}BBBBBBBBBB{Content7}CCCCCCCCCC{Content7}DDDDDDDDDD{Content7}EEEEEEEEEE{Content7}FFFFFFFFFF{Content7}GGGGGGGGGG{Content7}HHHHHHHHHH{Content7}IIIIIIIIII{Content7}JJJJJJJJJJ{Content7}
        AAAAAAAAAA{Content8}BBBBBBBBBB{Content8}CCCCCCCCCC{Content8}DDDDDDDDDD{Content8}EEEEEEEEEE{Content8}FFFFFFFFFF{Content8}GGGGGGGGGG{Content8}HHHHHHHHHH{Content8}IIIIIIIIII{Content8}JJJJJJJJJJ{Content8}
        AAAAAAAAAA{Content9}BBBBBBBBBB{Content9}CCCCCCCCCC{Content9}DDDDDDDDDD{Content9}EEEEEEEEEE{Content9}FFFFFFFFFF{Content9}GGGGGGGGGG{Content9}HHHHHHHHHH{Content9}IIIIIIIIII{Content9}JJJJJJJJJJ{Content9}
        """;
}

internal class FormatSampleClass
{
    public string Content0 { get; } = $" {nameof(Content0)} ";

    public string Content1 { get; } = $" {nameof(Content1)} ";

    public string Content2 { get; } = $" {nameof(Content2)} ";

    public string Content3 { get; } = $" {nameof(Content3)} ";

    public string Content4 { get; } = $" {nameof(Content4)} ";

    public string Content5 { get; } = $" {nameof(Content5)} ";

    public string Content6 { get; } = $" {nameof(Content6)} ";

    public string Content7 { get; } = $" {nameof(Content7)} ";

    public string Content8 { get; } = $" {nameof(Content8)} ";

    public string Content9 { get; } = $" {nameof(Content9)} ";
}
