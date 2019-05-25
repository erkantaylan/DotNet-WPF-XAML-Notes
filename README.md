# CSharp-Notebook
My C# journey notes




# MahappsMetro and Prism dialogs comes together to make life easier

`MAhApps.Metro 1.6.5` and `Prism.Unity v7.2.0.1233-pre` 

```C#
using System;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;

public static class DialogExtensions
{
    public static async Task<IDialogParameters> ShowDialogAsync(this CustomDialog dialog, MetroDialogSettings settings = null)
    {
        if (!(dialog.DataContext is DialogViewModel vm))
        {
            return null;
        }

        if (settings == null)
        {
            settings = MetroDialogSettingsFactory.Create();
        }

        MetroWindow window = Application.Current.Windows.OfType<MetroWindow>().First();
        await window.ShowMetroDialogAsync(dialog, settings);
        IDialogParameters parameters = await vm.Task;
        await window.HideMetroDialogAsync(dialog);
        return parameters;
    }
}

public abstract class DialogViewModel : BindableBase, IDialogViewModel
{
    private readonly TaskCompletionSource<IDialogParameters> tcs;

    protected DialogViewModel()
    {
        tcs = new TaskCompletionSource<IDialogParameters>();
    }

    public Task<IDialogParameters> Task => tcs.Task;

    public event EventHandler Closed;

    protected void Close(IDialogParameters parameters = null)
    {
        tcs.SetResult(parameters);
        Closed?.Invoke(this, EventArgs.Empty);
    }
}

public interface IDialogViewModel
{
    event EventHandler Closed;
}

public static class MetroDialogSettingsFactory
{
    public static MetroDialogSettings Create()
    {
        return new MetroDialogSettings
        {
            AnimateHide = true
          , AnimateShow = false
        };
    }
}
```

## How to use it

### ViewModel

```C#
class MyDialogViewModel : DialogViewModel {
  private void MyMethodtoEndDialog() { //submit or close does not matter
      this.Close(new DialogParameters(/*my parameters*/))
  }
}
```

### Initialazing dialog

```C#
DialogParameters result = await container.Resolve<ServerConnectionDialog>().ShowDialogAsync();
if(result != null)
{
    //yeeey
}
```
yes it is one single line

you can just say `Close()` at viewmodel, it will return `null` as *result* and check if result is `null` 

at dialogs i check only data is whether valid or not, i don't do any work with them, just return data to put all same logic in one place...
