using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Entropedia {
    public class SetLocation : MonoBehaviour 
    {
        private Sun sun;

        public string country = "Singapore";
        public bool randomMode = true;

        public void Start()
        {
            // Get Sun object
            sun = GetComponent<Sun>();

            if (randomMode)
            {
                // Read csv file
                string path = @"Assets/Scripts/latlong.csv";
                using(var reader = new StreamReader(path))
                {
                    List<string> dataList = new List<string>();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        dataList.Add(line);                            
                    }
                    
                    // Get random country
                    int id = UnityEngine.Random.Range(1, 241);
                    var countryData = dataList[id].Split("|");
                    country = countryData[0];
                    float latitude = float.Parse(countryData[1]);
                    float longitude = float.Parse(countryData[2]);

                    // Set location by latitude and longitude
                    sun.SetLocation(latitude, longitude);
                }
            }
        }
    }
}