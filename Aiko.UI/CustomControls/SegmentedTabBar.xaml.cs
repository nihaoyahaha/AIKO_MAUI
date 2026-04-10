namespace Aiko.UI.CustomControls;

public partial class SegmentedTabBar : ContentView
{
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable<SegmentedTabItem>),
        typeof(SegmentedTabBar),
        default(IEnumerable<SegmentedTabItem>));

    public IEnumerable<SegmentedTabItem>? ItemsSource
    {
        get => (IEnumerable<SegmentedTabItem>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public SegmentedTabBar()
    {
        InitializeComponent();
    }
}
