using System.Globalization;

namespace IconExtractor;

public static class Language
{
    public static string[] Get() => CultureInfo.CurrentUICulture.Name switch
    {
        "zh-CN" =>
        [
            "图标提取工具",
            "大小：",
            "拖拽 .dll 或者 .exe 文件到这里\n显示的图标可点击保存为 .png 和 .ico 文件"
        ],
        _ =>
        [
            "IconExtractor",
            "Size:",
            "Drag and drop a .dll or .exe file into this.\nThe displayed icons can be clicked to save as .png and .ico files."
        ]
    };
}