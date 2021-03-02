public static class WebBrowserSilencer
{    public static void ShutUp(this WebBrowser browser)
    {
        browser.Navigated += (_, _) => { SetSilent(browser, true); };
    }
    private static void SetSilent(WebBrowser browser, bool silent)
    {
        if (browser == null)
        {
            throw new ArgumentNullException(nameof(browser));
        }
        // get an IWebBrowser2 from the document
        if (browser.Document is not IOleServiceProvider sp)
        {
            return;
        }
        var iidIWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
        var iidIWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

        sp.QueryService(ref iidIWebBrowserApp, ref iidIWebBrowser2, out object webBrowser);
        Type type = webBrowser?.GetType();
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty;
        type?.InvokeMember(
            "Silent",
            bindingFlags,
            null,
            webBrowser,
            new object[]
            {
                silent
            });
    }
    [ComImport]
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IOleServiceProvider
    {
        [PreserveSig]
        int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
    }
}
