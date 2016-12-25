using System;
using Xamarin.Forms;

namespace Zen.Tracker.Client.Views
{
    public partial class CheckBox : ContentView
    {
        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(
                "FontSize", typeof(double), typeof(CheckBox),
                Device.GetNamedSize(NamedSize.Default, typeof(Label)),                
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    CheckBox checkbox = (CheckBox)bindable;
                    checkbox.CheckBoxLabel.FontSize = (double)newValue;
                });

        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(
                "IsChecked", typeof(bool), typeof(CheckBox),
                false,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    var boolValue = (bool) newValue;

                    // Set the graphic.
                    CheckBox checkbox = (CheckBox)bindable;
                    checkbox.CheckBoxLabel.Text = boolValue ? "\u2714" : "\u25FB";

                    // Fire the event.
                    checkbox.CheckedChanged?.Invoke(checkbox, boolValue);
                });

        public event EventHandler<bool> CheckedChanged;

        public CheckBox()
        {
            InitializeComponent();
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            set { SetValue(FontSizeProperty, value); }
            get { return (double)GetValue(FontSizeProperty); }
        }

        public bool IsChecked
        {
            set { SetValue(IsCheckedProperty, value); }
            get { return (bool)GetValue(IsCheckedProperty); }
        }

        // TapGestureRecognizer handler.
        private void OnCheckBoxTapped(object sender, EventArgs args)
        {
            IsChecked = !IsChecked;
        }
    }
}
