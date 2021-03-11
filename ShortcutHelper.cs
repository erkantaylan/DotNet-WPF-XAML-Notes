internal static class ShortcutHelper {    
    /// <summary>
    ///     windows styles
    /// </summary>
    public enum ShortcutWindowStyles {
        /// <summary>
        ///     Hide
        /// </summary>
        WshHide = 0,

        /// <summary>
        ///     NormalFocus
        /// </summary>
        WshNormalFocus = 1,

        /// <summary>
        ///     MinimizedFocus
        /// </summary>
        WshMinimizedFocus = 2,

        /// <summary>
        ///     MaximizedFocus
        /// </summary>
        WshMaximizedFocus = 3,

        /// <summary>
        ///     NormalNoFocus
        /// </summary>
        WshNormalNoFocus = 4,

        /// <summary>
        ///     MinimizedNoFocus
        /// </summary>
        WshMinimizedNoFocus = 6
    }
    /// <summary>
    ///     Create shortcut in current FilePath.
    /// </summary>
    /// <param name="linkFileName">shortcut name(include .lnk extension.)</param>
    /// <param name="targetPath">target FilePath</param>
    /// <param name="workingDirectory">working FilePath</param>
    /// <param name="arguments">arguments</param>
    /// <param name="hotkey">hot key(ex: Ctrl+Shift+Alt+A)</param>
    /// <param name="shortcutWindowStyle">window style</param>
    /// <param name="description">shortcut description</param>
    /// <param name="iconNumber">icon index(start of 0)</param>
    /// <returns>shortcut file FilePath.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string CreateShortcut(
        string linkFileName,
        string targetPath,
        string workingDirectory = "",
        string arguments = "",
        string hotkey = "",
        ShortcutWindowStyles shortcutWindowStyle = ShortcutWindowStyles.WshNormalFocus,
        string description = "",
        int iconNumber = 0) {
        if (linkFileName.Contains(DefaultShortcutExtension) == false) {
            linkFileName = $"{linkFileName}{DefaultShortcutExtension}";
        }
        if (!File.Exists(targetPath)) {
            throw new FileNotFoundException(targetPath);
        }
        if (workingDirectory == string.Empty) {
            workingDirectory = Path.GetDirectoryName(targetPath);
        }
        string iconLocation = $"{targetPath},{iconNumber}";

        if (Environment.Version.Major >= 4) {
            Type shellType = Type.GetTypeFromProgID(WscriptShellName);
            dynamic shell = Activator.CreateInstance(shellType);
            dynamic shortcut = shell.CreateShortcut(linkFileName);

            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = workingDirectory;
            shortcut.Arguments = arguments;
            shortcut.Hotkey = hotkey;
            shortcut.WindowStyle = shortcutWindowStyle;
            shortcut.Description = description;
            shortcut.IconLocation = iconLocation;

            shortcut.Save();
        } else {
            Type shellType = Type.GetTypeFromProgID(WscriptShellName);
            object shell = Activator.CreateInstance(shellType);
            object shortcut = shellType.InvokeMethod("CreateShortcut", shell, linkFileName);
            Type shortcutType = shortcut.GetType();

            shortcutType.InvokeSetMember("TargetPath", shortcut, targetPath);
            shortcutType.InvokeSetMember("WorkingDirectory", shortcut, workingDirectory);
            shortcutType.InvokeSetMember("Arguments", shortcut, arguments);
            shortcutType.InvokeSetMember("Hotkey", shortcut, hotkey);
            shortcutType.InvokeSetMember("WindowStyle", shortcut, shortcutWindowStyle);
            shortcutType.InvokeSetMember("Description", shortcut, description);
            shortcutType.InvokeSetMember("IconLocation", shortcut, iconLocation);

            shortcutType.InvokeMethod("Save", shortcut);
        }
        return Path.Combine(Environment.CurrentDirectory, linkFileName);
    }
    private static object InvokeSetMember(
        this Type type,
        string methodName,
        object targetInstance,
        params object[] arguments) {
        return type.InvokeMember(
            methodName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty,
            null,
            targetInstance,
            arguments);
    }
    private static object InvokeMethod(
        this Type type,
        string methodName,
        object targetInstance,
        params object[] arguments) {
        return type.InvokeMember(
            methodName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
            null,
            targetInstance,
            arguments);
    }
    public static void CreateDesktopShortcut(string programPath) {
        const string shortcutName = "ILAC TARIF.lnk";
        if (File.Exists($@"{Constants.DesktopLocation}\{shortcutName}")) {
            File.Delete($@"{Constants.DesktopLocation}\{shortcutName}");
        }
        CreateShortcut(shortcutName, programPath, "", "-m");
        File.Move(shortcutName, $@"{Constants.DesktopLocation}\{shortcutName}");
    }
    #region Constants

    /// <summary>
    ///     Default shortcut extension
    /// </summary>
    private const string DefaultShortcutExtension = ".lnk";

    private const string WscriptShellName = "WScript.Shell";

    #endregion
}
