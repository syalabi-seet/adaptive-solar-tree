using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entropedia
{
    public class SetDate : MonoBehaviour
    {
        private Sun sun;

        public bool random = true;

        [Range(2022, 2025)]
        public int year = 2022;

        [Range(1, 12)]
        public int month = 1;

        [Range(0, 31)]
        public int day = 1;       

        public void Start()
        {
            sun = GetComponent<Sun>();
            if (random == true)
            {
                DateTime d = getRandomDate();
                sun.SetDate(d);
            }
        }

        private DateTime getRandomDate()
        {
            year = UnityEngine.Random.Range(2022, 2025);
            month = UnityEngine.Random.Range(1, 12);
            day = UnityEngine.Random.Range(0, 31);
            DateTime d = new DateTime(year, month, day, 0, 0, 0);
            return d;
        }
    }
}