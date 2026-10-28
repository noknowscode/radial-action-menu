using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RadialActionMenuDemo.Controls;

namespace RadialActionMenuDemo;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Initialize the LEFT radial menu with 5 items (different icons)
        LeftRadialMenu.Items = new ObservableCollection<RadialMenuItemData>
        {
            new RadialMenuItemData
            {
                Glyph = "\uE77B", // Contact
                Label = "Contact"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE715", // Mail
                Label = "Mail"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE717", // Phone
                Label = "Call"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE8F3", // Calendar
                Label = "Schedule"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE721", // Search
                Label = "Search"
            }
        };

        // Initialize the RIGHT radial menu with 4 items (original icons)
        RightRadialMenu.Items = new ObservableCollection<RadialMenuItemData>
        {
            new RadialMenuItemData
            {
                Glyph = "\uE72D", // Share
                Label = "Share"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE70F", // Edit
                Label = "Edit"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE74D", // Delete
                Label = "Delete"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE734", // Favorite
                Label = "Favorite"
            }
        };

        // Initialize the CENTER radial menu with 4 items (arcs directly above)
        CenterRadialMenu.Items = new ObservableCollection<RadialMenuItemData>
        {
            new RadialMenuItemData
            {
                Glyph = "\uE8B7", // Folder
                Label = "Files"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE7C3", // Camera
                Label = "Camera"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE718", // Attach
                Label = "Attach"
            },
            new RadialMenuItemData
            {
                Glyph = "\uE786", // Microphone
                Label = "Record"
            }
        };
    }

    private void OnRadialMenuItemSelected(object sender, RadialMenuItemData e)
    {
        ActionFeedback.Text = $"> {e.Label.ToUpperInvariant()}_EXEC";
    }
}
