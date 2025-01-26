using Newtonsoft.Json;

namespace RainChecker
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public MainPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();

            //UI
            SetBackground();

            cbLockCity.IsChecked = true;

            //If no preference is set default is Győr
            picker.SelectedIndex = Preferences.Get("City", 0);
        }

        public string getUrl()
        {
            string api = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={Latitude}" +
                $"&longitude={Longitude}" +
                "&current=temperature_2m,rain,weather_code&hourly=weather_code&daily=weather_code,temperature_2m_max,temperature_2m_min&timezone=Europe%2FBerlin&forecast_days=1";

            return api;
        }

        public async Task<ApiData.Rootobject> GetApiData()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(getUrl());
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                ApiData.Rootobject data = JsonConvert.DeserializeObject<ApiData.Rootobject>(jsonResponse);

                return data;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                return null;
            }
        }

        //gets the api data and sets the ui
        private async void InitializeDataAsync()
        {
            // Fetch weather data
            var data = await GetApiData();

            Dictionary<int, string> weatherCodes = new Dictionary<int, string>
            {
                { 0, "Clear sky" },
                { 1, "Mainly clear" },
                { 2, "Partly cloudy" },
                { 3, "Overcast" },
                { 45, "Fog" },
                { 48, "Depositing rime fog" },
                { 51, "Light drizzle" },
                { 53, "Moderate drizzle" },
                { 55, "Dense drizzle" },
                { 56, "Light freezing drizzle" },
                { 57, "Dense freezing drizzle" },
                { 61, "Slight rain" },
                { 63, "Moderate rain" },
                { 65, "Heavy rain" },
                { 66, "Light freezing rain" },
                { 67, "Heavy freezing rain" },
                { 71, "Slight snow fall" },
                { 73, "Moderate snow fall" },
                { 75, "Heavy snow fall" },
                { 77, "Snow grains" },
                { 80, "Slight rain showers" },
                { 81, "Moderate rain showers" },
                { 82, "Violent rain showers" },
                { 85, "Slight snow showers" },
                { 86, "Heavy snow showers" },
                { 95, "Thunderstorm" },
                { 96, "Thunderstorm with slight hail" },
                { 99, "Thunderstorm with heavy hail" }
            };

            List<int> rainCodes = new List<int>
            {
                51, 53, 55, 56, 57,
                61, 63, 65, 66, 67,
                80, 81, 82, 85, 86,
                95, 96, 99
            };


            if (data != null)
            {
                //from weathercodes
                foreach (var item in weatherCodes)
                {
                    //daily
                    if (item.Key == data.daily.weather_code[0])
                    {
                        lblWeatherToday.Text = item.Value;
                    }

                    //current
                    if (item.Key == data.current.weather_code)
                    {
                        lblWeatherRn.Text = item.Value;
                    }
                }

                SetDailyWeatherImage(data.daily.weather_code[0]);

                //min-max
                lblMin.Text = Math.Round(data.daily.temperature_2m_min[0]).ToString() + "°C"; 
                lblMax.Text = Math.Round(data.daily.temperature_2m_max[0]).ToString() + "°C";

                if (data.daily.temperature_2m_min[0] <= 10.0 && data.daily.temperature_2m_min[0] >= -10.0)
                {
                    lblMin.Padding = new Thickness(0, 0, 50, 0);
                }

                if (data.daily.temperature_2m_max[0] <= 10.0 && data.daily.temperature_2m_max[0] >= -10.0)
                {
                    lblMax.Padding = new Thickness(0, 0, 45, 0);
                }

                //rain
                bool willItRain = false;
                for (int i = 0; i < 24; i++)
                {
                    if (rainCodes.Contains(data.hourly.weather_code[i]))
                    {
                        lblBringUmbrella.Text = "Bring an umbrella";
                        imgUmbrella.Source = "umbrella_open.png";
                        willItRain = true;
                        break;
                    }
                }

                //next rain
                for (int i = 0; i < 24; i++)
                {
                    DateTime nextRainTime = DateTime.Parse(data.hourly.time[i]);
                    if (rainCodes.Contains(data.hourly.weather_code[i]) && nextRainTime.Hour > DateTime.Now.Hour)
                    {
                        lblNextRain.Text = $"{nextRainTime.Hour}:00";
                        lblNextRain.Padding = new Thickness(0, 0, 60, 0);
                        break;
                    }
                }

                if (!willItRain)
                {
                    lblBringUmbrella.Text = "No umbrella needed";
                    imgUmbrella.Source = "umbrella_closed.png";
                    lblNextRain.Text = "-";
                    lblNextRain.Padding = new Thickness(0, 0, 100, 0);
                }

                //current weather
                lblTempRn.Text = Math.Round(data.current.temperature_2m).ToString() + "°C";

                DateTime time = DateTime.Parse(data.current.time);

                lblTime.Text = time.ToString().Substring(0, time.ToString().Length - 3);
            }
        }

        private void SetBackground()
        {
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.Now);

            if (currentTime.IsBetween(new TimeOnly(5, 0), new TimeOnly(9, 0)))
            {
                BackgroundImageSource = "bg_morning.png";
            }

            if (currentTime.IsBetween(new TimeOnly(9, 0), new TimeOnly(17, 0)))
            {
                BackgroundImageSource = "bg_day.png";
            }

            if (currentTime.IsBetween(new TimeOnly(17, 0), new TimeOnly(20, 0)))
            {
                BackgroundImageSource = "bg_evening.png";
            }

            if (currentTime.IsBetween(new TimeOnly(20, 0), new TimeOnly(5, 0)))
            {
                BackgroundImageSource = "bg_night.png";
            }
        }

        private void SetDailyWeatherImage(int code)
        {
            if (code == 0) // Clear sky
            {
                imgWeatherToday.Source = "sunny.png";
            }
            else if (code == 1) // Mainly clear
            {
                imgWeatherToday.Source = "mostlysunny.png";
            }
            else if (code == 2) // Partly cloudy
            {
                imgWeatherToday.Source = "mostlycloudy.png";
            }
            else if (code == 3) // Overcast
            {
                imgWeatherToday.Source = "cloudy.png";
            }
            else if (code == 45 || code == 48) // Fog or Depositing rime fog
            {
                imgWeatherToday.Source = "fog.png";
            }
            else if (code == 51 || code == 53 || code == 55) // Drizzle (Light, Moderate, Dense)
            {
                imgWeatherToday.Source = "chancerain.png";
            }
            else if (code == 56 || code == 57) // Freezing drizzle (Light, Dense)
            {
                imgWeatherToday.Source = "chancesleet.png";
            }
            else if (code == 61 || code == 63 || code == 65) // Rain (Slight, Moderate, Heavy)
            {
                imgWeatherToday.Source = "rain.png";
            }
            else if (code == 66 || code == 67) // Freezing rain (Light, Heavy)
            {
                imgWeatherToday.Source = "sleet.png";
            }
            else if (code == 71 || code == 73 || code == 75) // Snowfall (Slight, Moderate, Heavy)
            {
                imgWeatherToday.Source = "snow.png";
            }
            else if (code == 77) // Snow grains
            {
                imgWeatherToday.Source = "flurries.png";
            }
            else if (code == 80 || code == 81 || code == 82) // Rain showers (Slight, Moderate, Violent)
            {
                imgWeatherToday.Source = "chancerain.png";
            }
            else if (code == 85 || code == 86) // Snow showers (Slight, Heavy)
            {
                imgWeatherToday.Source = "chanceflurries.png";
            }
            else if (code == 95) // Thunderstorm
            {
                imgWeatherToday.Source = "chancestorm.png";
            }
            else if (code == 96 || code == 99) // Thunderstorm with hail (Slight, Heavy)
            {
                imgWeatherToday.Source = "thunderstorm.png";
            }
            
        }

        private void cbLockCity_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (cbLockCity.IsChecked)
            {
                picker.IsEnabled = false;
            }
            else
            {
                picker.IsEnabled = true;
            }
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker.SelectedIndex == 0) //Győr
            {
                Preferences.Set("City", 0);
                Latitude = "47.66";
                Longitude = "17.62";
            }
            if (picker.SelectedIndex == 1) //Veszprém
            {
                Preferences.Set("City", 1);
                Latitude = "47.08";
                Longitude = "17.90";
            }

            cbLockCity.IsChecked = true;

            
            InitializeDataAsync();
        }
    }
}
