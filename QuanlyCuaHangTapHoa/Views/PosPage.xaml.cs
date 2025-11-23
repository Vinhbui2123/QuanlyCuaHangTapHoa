using QuanlyCuaHangTapHoa.ViewModels;

namespace QuanlyCuaHangTapHoa.Views;

public partial class PosPage : ContentPage
{
    public PosPage(PosViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
