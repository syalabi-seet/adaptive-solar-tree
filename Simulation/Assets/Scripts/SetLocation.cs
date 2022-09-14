using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entropedia {
    public class SetLocation : MonoBehaviour 
    {
        private Sun sun;
        public bool random = true;
        public string country = "Singapore";
        public float latitude = 1.3521f;
        public float longitude = 103.8198f;

        public void Awake()
        {
            // Get Sun object
            sun = GetComponent<Sun>();

            // Read csv file
            string path = @"Assets/Scripts/Locations.csv";
            using(var reader = new StreamReader(path))
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
                    float latitude_add = UnityEngine.Random.Range(-1.0f, 1.0f);
                    float longitude_add = UnityEngine.Random.Range(-1.0f, 1.0f);
                    latitude = float.Parse(countryData[1]) + latitude_add;
                    longitude = float.Parse(countryData[2]) + longitude_add;
                }               

                // Set location by latitude and longitude
                sun.SetLocation(latitude, longitude);
            }
        }
    }
}