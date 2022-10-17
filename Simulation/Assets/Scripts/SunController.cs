using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module
{
    [RequireComponent(typeof(Light)), AddComponentMenu("ML Agents/Sun Controller")]
    public class SunController : MonoBehaviour
    {
        public bool random = true;
        public float latitude = 1.3521f;
        public float longitude = 103.8198f;
        public string country = "Singapore";

        [Range(0, 24)]
        public int hour = 0;

        [Range(0, 60)]
        public int minute = 0;

        [Range(2022, 2025)]
        public int year = 2022;

        [Range(1, 12)]
        public int month = 1;

        [Range(1, 31)]
        public int day = 1;

        public DateTime time;
        new Light light;

        public float timeSpeed;

        public int frameSteps;
        int frameStep;

        DateTime date;
        DateTime[] dateRange;


        private void SetLocation()
        {
            // Read csv file
            string filename = @"Assets/Data/Locations.csv";
            using(var reader = new StreamReader(filename))
            {
                List<string> dataList = new List<string>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    dataList.Add(line);                            
                }
                
                // Get random country
                if (random == true)
                {
                    int id = UnityEngine.Random.Range(1, 241);
                    var countryData = dataList[id].Split("|");
                    country = countryData[0];
                    latitude = float.Parse(countryData[1]);
                    longitude = float.Parse(countryData[2]);
                }     

                latitude += UnityEngine.Random.Range(-1.0f, 1.0f);
                longitude += UnityEngine.Random.Range(-1.0f, 1.0f);   
            }
        }

        private void SetDate()
        {
            if (random == true)
            {
                year = UnityEngine.Random.Range(2022, 2025);
                month = UnityEngine.Random.Range(1, 12);
                if (month == 2)
                {
                    day = UnityEngine.Random.Range(0, 29);
                } else
                {
                    day = UnityEngine.Random.Range(0, 32);
                }                    
            }

            try
            {
                time = new DateTime(year, month, day, hour, minute, 0);  
            }
            catch (System.Exception)
            {
                Debug.Log("Retrying...");
                day = 30;
                time = new DateTime(year, month, day, hour, minute, 0);  
            }       
        }

        private void GenerateSolarData()
        {
            string filename = @"Assets/Data/SolarGeometricData.csv";
            StreamWriter sw = new StreamWriter(filename, false);
            sw.WriteLine("timeIndex, solarZenith, solarAzimuth");

            // Loop hour
            int timeIndex = 0;
            for (int m_hour = 0; m_hour <= 23; m_hour++)
            {
                // Loop minute
                for (int m_minute = 0; m_minute <= 59; m_minute++)
                {
                    DateTime m_time = new DateTime(year, month, day, m_hour, m_minute, 0);      
                    double m_alt;
                    double m_azi;
                    SunPosition.CalculateSunPosition(m_time, (double)latitude, (double)longitude, out m_azi, out m_alt);
                    double m_zen = 90.0f - m_alt;
                    string outString = string.Format("{0}, {1}, {2}", timeIndex, m_zen, m_azi);
                    sw.WriteLine(outString);
                    timeIndex++;
                }
            }
            sw.Close();
            Debug.Log("Solar geometric data generated");
        }

        private void SetUpdateSteps(int i) {
            frameSteps = i;
        }

        private void SetTimeSpeed(float speed) {
            timeSpeed = speed;
        }

        private void Start()
        {
            light = GetComponent<Light>();
            // GenerateSolarData();
        }

        public void SetSun(bool isRandomized)
        {
            if (isRandomized)
            {
                random = true;
            } else
            {
                random = false;
            }

            SetLocation();
            SetDate();      
        }

        private void Update()
        {
            time = time.AddSeconds(60 * timeSpeed * Time.deltaTime);
            if (frameStep==0) 
            {
                SetPosition();
            }
            frameStep = (frameStep + 1) % frameSteps;
        }

        private void SetPosition()
        {
            double alt;
            double azi;
            SunPosition.CalculateSunPosition(time, (double)latitude, (double)longitude, out azi, out alt);
            Vector3 angles = new Vector3((float)alt, (float)azi, 0);                                      
            transform.localRotation = Quaternion.Euler(angles);
            light.intensity = Mathf.InverseLerp(-12, 0, angles.x);
        }

        private void OnGUI()
        {
            GUI.contentColor = Color.green;

            // Print world settings on game scene
            string country_str = String.Concat("Country: ", country);
            GUI.Label(new Rect(10, 0, 300, 20), country_str);

            string location = String.Concat("Coordinates: (",  latitude, ", ", longitude, ")");
            GUI.Label(new Rect(10, 15, 300, 20), location);

            string date = String.Concat("Date/Time: ", time);
            GUI.Label(new Rect(10, 30, 300, 20), date);
        }
    }

    /*
     * The following source came from this blog:
     * http://guideving.blogspot.co.uk/2010/08/sun-position-in-c.html
     */
    public static class SunPosition
    {
        private const double Deg2Rad = Math.PI / 180.0;
        private const double Rad2Deg = 180.0 / Math.PI;

        /*! 
         * \brief Calculates the sun light. 
         * 
         * CalcSunPosition calculates the suns "position" based on a 
         * given date and time in local time, latitude and longitude 
         * expressed in decimal degrees. It is based on the method 
         * found here: 
         * http://www.astro.uio.no/~bgranslo/aares/calculate.html
         * The calculation is only satisfiably correct for dates in 
         * the range March 1 1900 to February 28 2100. 
         * \param dateTime Time and date in local time. 
         * \param latitude Latitude expressed in decimal degrees. 
         * \param longitude Longitude expressed in decimal degrees. 
         */
        public static void CalculateSunPosition(
            DateTime dateTime, 
            double latitude, 
            double longitude, 
            out double outAzimuth, 
            out double outAltitude)
        {
            // Convert to UTC  
            dateTime = dateTime.ToUniversalTime();

            // Number of days from J2000.0.  
            double julianDate = 367 * dateTime.Year -
                Math.Floor((7.0 / 4.0) * (dateTime.Year +
                Math.Floor((dateTime.Month + 9.0) / 12.0))) +
                Math.Floor((275.0 * dateTime.Month) / 9.0) +
                dateTime.Day - 730531.5;

            double julianCenturies = julianDate / 36525.0;

            // Sidereal Time  
            double siderealTimeHours = 6.6974 + 2400.0513 * julianCenturies;

            double siderealTimeUT = siderealTimeHours +
                (366.2422 / 365.2422) * (double)dateTime.TimeOfDay.TotalHours;

            double siderealTime = siderealTimeUT * 15 + longitude;

            // Refine to number of days (fractional) to specific time.  
            julianDate += (double)dateTime.TimeOfDay.TotalHours / 24.0;
            julianCenturies = julianDate / 36525.0;

            // Solar Coordinates  
            double meanLongitude = CorrectAngle(Deg2Rad *
                (280.466 + 36000.77 * julianCenturies));

            double meanAnomaly = CorrectAngle(Deg2Rad *
                (357.529 + 35999.05 * julianCenturies));

            double equationOfCenter = Deg2Rad * ((1.915 - 0.005 * julianCenturies) *
                Math.Sin(meanAnomaly) + 0.02 * Math.Sin(2 * meanAnomaly));

            double elipticalLongitude =
                CorrectAngle(meanLongitude + equationOfCenter);

            double obliquity = (23.439 - 0.013 * julianCenturies) * Deg2Rad;

            // Right Ascension  
            double rightAscension = Math.Atan2(
                Math.Cos(obliquity) * Math.Sin(elipticalLongitude),
                Math.Cos(elipticalLongitude));

            double declination = Math.Asin(
                Math.Sin(rightAscension) * Math.Sin(obliquity));

            // Horizontal Coordinates  
            double hourAngle = CorrectAngle(siderealTime * Deg2Rad) - rightAscension;

            if (hourAngle > Math.PI)
            {
                hourAngle -= 2 * Math.PI;
            }

            double altitude = Math.Asin(Math.Sin(latitude * Deg2Rad) *
                Math.Sin(declination) + Math.Cos(latitude * Deg2Rad) *
                Math.Cos(declination) * Math.Cos(hourAngle));

            // Nominator and denominator for calculating Azimuth  
            // angle. Needed to test which quadrant the angle is in.  
            double aziNom = -Math.Sin(hourAngle);
            double aziDenom =
                Math.Tan(declination) * Math.Cos(latitude * Deg2Rad) -
                Math.Sin(latitude * Deg2Rad) * Math.Cos(hourAngle);

            double azimuth = Math.Atan(aziNom / aziDenom);

            if (aziDenom < 0) // In 2nd or 3rd quadrant  
            {
                azimuth += Math.PI;
            }
            else if (aziNom < 0) // In 4th quadrant  
            {
                azimuth += 2 * Math.PI;
            }

            outAltitude = altitude * Mathf.Rad2Deg;
            outAzimuth = azimuth * Mathf.Rad2Deg;
        }

        /*! 
        * \brief Corrects an angle. 
        * 
        * \param angleInRadians An angle expressed in radians. 
        * \return An angle in the range 0 to 2*PI. 
        */
        private static double CorrectAngle(double angleInRadians)
        {
            if (angleInRadians < 0)
            {
                return 2 * Math.PI - (Math.Abs(angleInRadians) % (2 * Math.PI));
            }
            else if (angleInRadians > 2 * Math.PI)
            {
                return angleInRadians % (2 * Math.PI);
            }
            else
            {
                return angleInRadians;
            }
        }
    }

}