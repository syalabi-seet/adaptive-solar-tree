using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module
{
    public enum trackingFrequencies
    {
        everySecond,
        everyMinute,
        everyHalfHour,
        everyHour,
        every2Hours,
        every4Hours,
        every24Hours
    }

    [AddComponentMenu("ML Agents/Environment Controller")]
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField]
        public bool randomizeCountry = false; 

        [SerializeField]
        public trackingFrequencies trackingFrequency = trackingFrequencies.everySecond; 

        [SerializeField]
        public SunController sunController;     

        [SerializeField]     
        public float shadowRatioLimit = 0.2f;
        
        [SerializeField]
        public float incidenceAngleLimit = 5f;       

        public DateTime startTime;
        public DateTime endTime;
        public double latitude;
        public double longitude;
        public string country;
        public double timeOffset;

        private GUIStyle guiStyle = new GUIStyle();

        AgentController agentController;

        void Start()
        {
            agentController = GameObject.FindWithTag("Module").GetComponent<AgentController>();
        }

        public void SetLocation()
        {
            var textAssetData = Resources.Load<TextAsset>(@"Locations");
            string[] data = textAssetData.text.Split("\n");

            if (randomizeCountry == true)
            {
                int id = UnityEngine.Random.Range(1, data.Length - 1);
                var countryData = data[id].Split("|");
                country = countryData[0];
                latitude = float.Parse(countryData[1]);
                longitude = float.Parse(countryData[2]);
                timeOffset = float.Parse(countryData[3]);                            
            } else
            {
                // Singapore as default
                country = "Singapore";
                latitude = 1.3521;
                longitude = 103.8198;
                timeOffset = 8.0;
            }
            latitude += UnityEngine.Random.Range(-0.5f, 0.5f);
            longitude += UnityEngine.Random.Range(-0.5f, 0.5f);   
        }

        public void SetDate()
        {
            int year = UnityEngine.Random.Range(2022, 2030);
            int month = UnityEngine.Random.Range(1, 12);
            int day = UnityEngine.Random.Range(1, 29);
            startTime = new DateTime(year, month, day, 0, 0, 0);
        }

        public void SetTime()
        {
            DateTime sunRise;
            DateTime sunSet;
            sunController.CalculateSunHours(startTime, latitude, longitude, out sunRise, out sunSet);
            int sunRiseHour = sunRise.Hour;
            int sunSetHour = sunSet.Hour;
            if ((sunRise - startTime).TotalMinutes < 0)
            {
                sunRiseHour = 24 - sunRiseHour;
            }

            if ((sunSet - startTime).TotalMinutes < 0)
            {
                sunSetHour = 24 - sunSetHour;
            }

            int hour = UnityEngine.Random.Range(sunRiseHour, sunSetHour);
            int minute = UnityEngine.Random.Range(sunRise.Minute, sunSet.Minute);
            startTime = startTime.AddHours(hour);
            startTime = startTime.AddMinutes(minute);
            endTime = startTime.AddMinutes(30);
        }

        private void Update()
        {
            startTime = startTime.AddSeconds(1 * Time.deltaTime);
            sunController.SetPosition(startTime, latitude, longitude);
        }

        public void Reset()
        {
            SetLocation();
            SetDate();  
            SetTime();          
        }

        private void OnGUI()
        {
            guiStyle.fontSize = 20;
            guiStyle.normal.textColor = Color.red;

            // Print world settings on game scene
            string country_str = String.Concat("Country: ", country);
            GUI.Label(new Rect(10, 0, 300, 20), country_str, guiStyle);

            string location = String.Concat("Coordinates: (",  (float)latitude, ", ", (float)longitude, ")");
            GUI.Label(new Rect(10, 20, 300, 20), location, guiStyle);

            string date = String.Concat("Date/Time: ", startTime.AddHours(timeOffset));
            GUI.Label(new Rect(10, 40, 300, 20), date, guiStyle);

            string episodeCount = String.Concat("Episode: ", agentController.CompletedEpisodes);
            GUI.Label(new Rect(10, 60, 300, 20), episodeCount, guiStyle);

            string currentReward = String.Concat("Cumulative reward: ", agentController.GetCumulativeReward());
            GUI.Label(new Rect(10, 80, 300, 20), currentReward, guiStyle);
        }
    }
}