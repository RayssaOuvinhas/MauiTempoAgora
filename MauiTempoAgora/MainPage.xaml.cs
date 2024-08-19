using MauiTempoAgora.Models;
using MauiTempoAgora.Service;
using Microsoft.Maui.Devices.Sensors;
using System.Diagnostics;

namespace MauiTempoAgora
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource _cancelTokenSource;
        bool _isCheckingLocation;

        string? cidade;
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                _cancelTokenSource = new CancellationTokenSource();

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

                Location? location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    lbl_latitude.Text = location.Latitude.ToString();
                    lbl_longitude.Text = location.Longitude.ToString();

                    Debug.WriteLine("------------------------------");
                    Debug.WriteLine(location);
                    Debug.WriteLine("------------------------------");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fnsEx)
            {
                await DisplayAlert("Erro: Localização desabilitada", fnsEx.Message, "OK");
            }
            catch (PermissionException fnsEx)
            {
                await DisplayAlert("Erro: Permissão", fnsEx.Message, "OK");
            }
            catch (Exception ex) 
            { 
                await DisplayAlert("Erro:", ex.Message, "OK");
            }
        }

        private async Task<string> GetGeocodeReverseData(double latitude = 47.673988, double longitude = -122.121513)
        {
            IEnumerable<Placemark>placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

            Placemark? placemark = placemarks?.FirstOrDefault();

            Debug.WriteLine("------------------------------");
            Debug.WriteLine(placemark?.Locality);
            Debug.WriteLine("------------------------------");

            if (placemark != null) 
            {
                cidade = placemark.Locality;

                return
                $"AdminArea:         {placemark.AdminArea}\n" +
                $"CountryCode:       {placemark.CountryCode}\n" +
                $"CountryName:       {placemark.CountryName}\n" +
                $"FeatureName:       {placemark.FeatureName}\n" +
                $"Locality:          {placemark.Locality}\n" +
                $"PostalCode:        {placemark.PostalCode}\n" +
                $"SubAdminArea:      {placemark.SubAdminArea}\n" +
                $"SubLocality:       {placemark.SubLocality}\n" +
                $"SubThoroughfare:   {placemark.SubThoroughfare}\n" +
                $"Thoroughfare:      {placemark.Thoroughfare}\n";
            }
            return "Nada";
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            double latitude = Convert.ToDouble(lbl_latitude.Text);
            double longitude = Convert.ToDouble(lbl_longitude.Text);

            lbl_reverso.Text = await GetGeocodeReverseData(latitude, longitude);
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(cidade))
                {
                    Tempo? previsao = await DataService.GetPrevisaoDoTempo(cidade);

                    string dados_previsao = "";

                    if (previsao != null) 
                    {
                        dados_previsao =
                        $"Humidade:         {previsao.Humidity}\n" +
                        $"Nascer do sol:       {previsao.Sunrise}\n" +
                        $"Por do sol:       {previsao.Sunset}\n" +
                        $"Temperatura:       {previsao.Temperature}\n" +
                        $"Titulo:          {previsao.Title}\n" +
                        $"Visibilidade:        {previsao.Visibility}\n" +
                        $"Vento:      {previsao.Wind}\n" +
                        $"Previsao:       {previsao.Weather}\n" +
                        $"Descrição:   {previsao.WeatherDescription}\n";
                    }
                    else
                    {
                        dados_previsao = $"Sem dados, previsão nula.";
                    }

                    Debug.WriteLine("------------------------------");
                    Debug.WriteLine(dados_previsao);
                    Debug.WriteLine("------------------------------");

                    lbl_previsao.Text = dados_previsao;
                }
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Erro", ex.Message,"OK");
            }
        }
    }

}
