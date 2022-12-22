using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp
{
    internal static class Interactor
    {
        public static WeatherObject GetWeatherUsingCoordinates(LocationObject locationObject)
        {
            using (var client = new HttpClient())
            {
                string uri = "https://api.open-meteo.com/v1/forecast?latitude=" + locationObject.latitude + "&longitude=" + locationObject.longitude + "&hourly=temperature_2m&current_weather=1";
                var endpoint = new Uri(uri);
                var result = client.GetAsync(endpoint).Result;
                var json = result.Content.ReadAsStringAsync().Result;
                WeatherObject weatherObject = JsonConvert.DeserializeObject<WeatherObject>(json);
                return weatherObject;
            }
        }



        public static List<LocationObject> CreateDataList()
        {
            List<LocationObject> listToReturn = new List<LocationObject>();   
            
            string domain = AppDomain.CurrentDomain.BaseDirectory;
            string commaInCell = ", ";


            int counter = 0;

            // Read the csv line by line and split the data into strings to be attributed to a LocationObject which is then added to the List to be returned.
            foreach (string line in System.IO.File.ReadLines(domain + "\\worldcities.csv"))
            {
                if (!line.Contains(commaInCell))
                {
                    string[] splitLine = line.Split(',');

                    //if any of these are empty, set the property to null. Or just have an if statement to say if value not empty, assign value
                    LocationObject locationObject = new LocationObject();
                    if (splitLine[0].ToUpper() != "")
                    {
                        locationObject.cityName = splitLine[0].ToUpper();
                    }
                    else locationObject.cityName = "Error: City Name Not Found.";


                    if (splitLine[1] != "")
                    {
                        if (splitLine[1].Length > 5)
                        {
                            string trimmedFloat = "";
                            trimmedFloat += splitLine[1][0];
                            trimmedFloat += splitLine[1][1];
                            trimmedFloat += splitLine[1][2];
                            trimmedFloat += splitLine[1][3];
                            trimmedFloat += splitLine[1][4];
                            locationObject.latitude = float.Parse(trimmedFloat);
                        }
                        else locationObject.latitude = float.Parse(splitLine[1]);
                    }
                    else locationObject.latitude = 0;


                    if (splitLine[2] != "")
                    {
                        if (splitLine[2].Length > 5)
                        {
                            string trimmedFloat = "";
                            trimmedFloat += splitLine[2][0];
                            trimmedFloat += splitLine[2][1];
                            trimmedFloat += splitLine[2][2];
                            trimmedFloat += splitLine[2][3];
                            trimmedFloat += splitLine[2][4];
                            locationObject.longitude = float.Parse(trimmedFloat);
                        }
                        else locationObject.longitude = float.Parse(splitLine[2]);
                    }
                    else locationObject.longitude = 0;


                    if (splitLine[3] != "")
                    {
                        locationObject.country = splitLine[3].ToUpper();
                    }
                    else locationObject.country = "Error: Country Not Found.";


                    if (splitLine[4] != "")
                    {
                        locationObject.iso = splitLine[4].ToUpper();
                    }
                    else locationObject.iso = "Error: Locaton ISO Not Found.";


                    if (splitLine[5] != "")
                    {
                        locationObject.population = long.Parse(splitLine[5]);
                    }
                    else locationObject.population = 0;

                    locationObject.listingID = "";

                    listToReturn.Add(locationObject);
                    counter++;
                }

            }
            return listToReturn;
        }


        public static List<LocationObject> SelectLocationObject(List<LocationObject> locationList)
        {
            while (true)
            {
                Console.WriteLine("Enter the name of the city you'd like to look up.");
                string nameOfCity = Console.ReadLine().ToUpper();

                List<LocationObject> methodList = new List<LocationObject>();
                int counter = 0;
                foreach (LocationObject locationObject in locationList)
                {
                    if (locationObject.cityName == nameOfCity)
                    {
                        methodList.Add(locationObject);
                        counter++;
                    }
                }
                if (methodList.Count == 0)
                {
                    Console.WriteLine("Your search didn't match our database. Please ensure the city name is spelled correctly, without any special characters or accents.\n");
                }
                return methodList;
            }
        }



        public static void PresentWeatherObject(LocationObject locationObject, WeatherObject weatherObject)
        {
            Console.Clear();

            //location fields
            Console.WriteLine("LOCATION:");
            Console.WriteLine(" City Name: " + locationObject.cityName);
            Console.WriteLine(" Coordinates - Latitude: " + locationObject.latitude + "   Longitude: " + locationObject.longitude);
            Console.WriteLine(" Country : " + locationObject.country);
            Console.WriteLine(" Population : " + locationObject.population + "\n");

            //weather specifics at current time
            Console.WriteLine("CURRENT WEATHER:");
            Console.WriteLine(" Time: " + weatherObject.current_weather.time);
            Console.WriteLine(" Temperature: " + weatherObject.current_weather.temperature);
            Console.WriteLine(" Wind Speed: " + weatherObject.current_weather.windspeed);
            Console.WriteLine(" Wind Direction: " + weatherObject.current_weather.winddirection + "\n");

            //hourly weather
            Console.WriteLine("TEMPERATURE OVER THE NEXT 7 DAYS: ");
            int counter = 0;
            for (int i = 0; i < weatherObject.hourly.time.Length; i++)
            {
                Console.WriteLine(" Time: " + weatherObject.hourly.time[i]);
                //weatherObject.hourly.temperature_2m[i] -= 32;
                //weatherObject.hourly.temperature_2m[i] = weatherObject.hourly.temperature_2m[i] / 1.8f;
                Console.WriteLine(" Temperature: " + (int)weatherObject.hourly.temperature_2m[i] + " °C");
                Console.WriteLine();
                //if (i == 24)
                //{
                //    Console.WriteLine();
                //    return;
                //}
                Console.WriteLine();
                counter++;
            }
           
        }


        
        public static void TakeInputAndSearch()
        {
            while (true)
            {
                Console.WriteLine("Type the name of the city for which you would like to see the weather. Then press enter:");
                
                //any lists may need to be cleared at end of method, not sure
                List<LocationObject> locationList = CreateDataList();

                List<LocationObject> listings = SelectLocationObject(locationList);
                LocationObject selectedListing = SelectListing(listings);
                WeatherObject weatherObject = GetWeatherUsingCoordinates(selectedListing);

                PresentWeatherObject(selectedListing, weatherObject);
            }
        }



        public static LocationObject PresentSearchListing(LocationObject locationObject, int setPosition)
        {
            setPosition++;
            Console.WriteLine(setPosition + ": " + locationObject.cityName + " - " + locationObject.country + " - Lat: " + locationObject.latitude + " - Long: " + locationObject.longitude + "\n");
            LocationObject objectToReturn = locationObject;
            return objectToReturn;
        }



        public static LocationObject SelectListing(List<LocationObject> locationObjects)
        {
            Console.Clear();
            LocationObject objectToReturn = new LocationObject();

            int counter = 0;
            foreach (LocationObject locationObject in locationObjects)
            {
                LocationObject listing = PresentSearchListing(locationObject, counter);
                counter++;
            }

            Console.WriteLine("Select the listing of the city you would like to view by typing its corresponding number");
            char inputChar = Console.ReadKey().KeyChar;
            
            int inputNo = int.Parse(inputChar.ToString()) - 1;

            //need a block to ensure the correct thing is typed, or will throw exception
            
            objectToReturn = locationObjects[inputNo];
             
            return objectToReturn;
        }

    }



    public class LocationObject
    {
        public string cityName { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string country { get; set; }
        public string iso { get; set; }
        public long population { get; set; }
        public string listingID { get; set; }
    }




    public class WeatherObject
    {
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float elevation { get; set; }
        public float generationtime_ms { get; set; }
        public int utc_offset_seconds { get; set; }
        public string timezone { get; set; }
        public string timezone_abbreviation { get; set; }
        public Hourly hourly { get; set; }
        public Hourly_Units hourly_units { get; set; }
        public Current_Weather current_weather { get; set; }
    }

    public class Hourly
    {
        public string[] time { get; set; }
        public float[] temperature_2m { get; set; }
    }

    public class Hourly_Units
    {
        public string temperature_2m { get; set; }
    }

    public class Current_Weather
    {
        public string time { get; set; }
        public float temperature { get; set; }
        public int weathercode { get; set; }
        public float windspeed { get; set; }
        public float winddirection { get; set; }
    }

}

