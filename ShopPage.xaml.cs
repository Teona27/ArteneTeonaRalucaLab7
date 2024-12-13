using ArteneTeonaRalucaLab7.Models;
using Plugin.LocalNotification;
using Microsoft.Maui.Devices.Sensors;
using Plugin.LocalNotification;

namespace ArteneTeonaRalucaLab7;

public partial class ShopPage : ContentPage
{
    public ShopPage()
    {
        InitializeComponent();
    }

    async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        await App.Database.SaveShopAsync(shop);
        await Navigation.PopAsync();
    }

    async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;

        if (shop != null)
        {
            bool confirm = await DisplayAlert("Confirm Delete",
                                              $"Are you sure you want to delete {shop.ShopName}?",
                                              "Yes", "No");
            if (confirm)
            {
                await App.Database.DeleteShopAsync(shop);
                await Navigation.PopAsync();
            }
        }
    }

        async void OnShowMapButtonClicked(object sender, EventArgs e)
    {
        var shop = (Shop)BindingContext;
        var address = shop.Adress;
        var locations = await Geocoding.GetLocationsAsync(address);

        var options = new MapLaunchOptions
        {
            Name = "Magazinul meu preferat"
        };

        var shoplocation = locations?.FirstOrDefault();
        var myLocation = await Geolocation.GetLocationAsync();
        /* var shoplocation = new Location(46.7492379, 23.5745597); // pentru Windows Machine */
        var distance = myLocation.CalculateDistance(shoplocation, DistanceUnits.Kilometers);

        if (distance < 5)
        {
            var request = new NotificationRequest
            {
                Title = "Ai de facut cumparaturi în apropiere!",
                Description = address,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1)
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }


        await Map.OpenAsync(shoplocation, options);
    }
}
