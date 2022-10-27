using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module
{
    [AddComponentMenu("ML Agents/Sun Controller")]
    public class SunController : MonoBehaviour
    {   
        double earthMeanRadius = 6371.01;
        double astronomicalUnit = 149597890.0;
        public double elapsedJulianDays;
        public double decimalHours;
        public double eclipticLongitude;
        public double eclipticObliquity;
        public double rightAscension;
        public double declination;
        public double solarAzimuth;
        public double solarZenith;
        public double solarAltitude;
        public DateTime sunRise;
        public DateTime sunSet;

        double rad = Math.PI / 180.0;
        double deg = 180.0 / Math.PI;

        public void SetPosition(DateTime dateTime, double latitude, double longitude)
        {
            CalculateSunPosition(dateTime, latitude, longitude, out solarAzimuth, out solarZenith);
            solarAltitude = 90 - solarZenith;
            Vector3 angles = new Vector3((float)solarAltitude, (float)solarAzimuth, 0);                                      
            transform.localRotation = Quaternion.Euler(angles);
            GetComponent<Light>().intensity = Mathf.InverseLerp(-12, 0, angles.x);
        }

        private void CalculateSunPosition(
            DateTime dateTime, double latitude, double longitude,
            out double solarAzimuth, out double solarZenith)
        {
            // Calculate elapsed julian days
            CalculateElapsedJulianDay(
                dateTime, out elapsedJulianDays, out decimalHours);

            // Calculate ecliptic coordinates
            CalculateEclipticCoordinates(
                elapsedJulianDays, 
                out eclipticLongitude, out eclipticObliquity);

            // Calculate celestial coordinates
            CalculateCelestialCoordinates(
                eclipticLongitude, eclipticObliquity, 
                out rightAscension, out declination);

            // Calculate local coordinates
            CalculateLocalCoordinates(
                dateTime, latitude, longitude, elapsedJulianDays, decimalHours,
                rightAscension, declination, 
                out solarAzimuth, out solarZenith);            
        }

        public void CalculateSunHours(
            DateTime dateTime, double latitude, double longitude,
            out DateTime sunRise, out DateTime sunSet)
        {
            // Calculate elapsed julian days
            CalculateElapsedJulianDay(
                dateTime, out elapsedJulianDays, out decimalHours);

            // Calculate ecliptic coordinates
            CalculateEclipticCoordinates(
                elapsedJulianDays, 
                out eclipticLongitude, out eclipticObliquity);

            // Calculate celestial coordinates
            CalculateCelestialCoordinates(
                eclipticLongitude, eclipticObliquity, 
                out rightAscension, out declination);

            // Calculate Hour angle at Sunrise/sunset
            double fractionalYear = CalculateFractionalYear(dateTime);
            double equationOfTime = CalculateEquationOfTime(fractionalYear);
            double hourAngleSunrise = CalculateHourAngleSunrise(latitude, declination);
 
            sunRise = GetSunLimits(
                dateTime, longitude, hourAngleSunrise, equationOfTime);
            sunSet = GetSunLimits(
                dateTime, longitude, -hourAngleSunrise, equationOfTime);  
        } 

        private void CalculateElapsedJulianDay(
            DateTime dateTime, out double elapsedJulianDays, 
            out double decimalHours)
        {
            // Calculate time of day in UT decimal hours
            decimalHours = dateTime.Hour + (dateTime.Minute + dateTime.Second / 60.0) / 60.0;

            // Calculate current Julian Day
            double Aux = (dateTime.Month - 14.0) / 12.0;
            double julianDayNumber = (
                (1461.0 * (dateTime.Year + 4800.0 + Aux)) / 4.0 + 
                (367.0 * (dateTime.Month - 2.0 - 12.0 * Aux)) / 12.0 -
                (3.0 * ((dateTime.Year + 4900.0 + Aux) / 100.0)) / 4.0 +
                dateTime.Day - 32075.0);
            double julianDate = julianDayNumber - 0.5 + decimalHours / 24.0;
            elapsedJulianDays = julianDate - 2451545.0;
        }

        private void CalculateEclipticCoordinates(
            double elapsedJulianDays, out double eclipticLongitude, 
            out double eclipticObliquity)
        {
            double omega = 2.1429 - 0.0010394594 * elapsedJulianDays;
            double meanLongitude = 4.8950630 + 0.017202791698 * elapsedJulianDays;
            double meanAnomaly = 6.2400600 + 0.0172019699 * elapsedJulianDays;
            eclipticLongitude = (
                meanLongitude +
                0.03341607 * Math.Sin(meanAnomaly) +
                0.00034894 * Math.Sin(2.0 * meanAnomaly) - 
                0.0001134 - 0.0000203 * Math.Sin(omega));
            eclipticObliquity = (
                0.4090928 - 6.2140E-9 * elapsedJulianDays +
                0.0000396 * Math.Cos(omega));
        }

        private void CalculateCelestialCoordinates(
            double eclipticLongitude, double eclipticObliquity,
            out double rightAscension, out double declination)
        {
            double rightAscensionY = Math.Cos(eclipticObliquity) * Math.Sin(eclipticLongitude);
            double rightAscensionX = Math.Cos(eclipticLongitude);
            rightAscension = Math.Atan2(rightAscensionY, rightAscensionX);
            if (rightAscension < 0.0)
            {
                rightAscension = rightAscension + Math.PI * 2.0;
            }
            declination = Math.Asin(Math.Sin(eclipticObliquity) * Math.Sin(eclipticLongitude));
        }

        private void CalculateLocalCoordinates(
            DateTime dateTime, double latitude, double longitude,
            double elapsedJulianDays, double decimalHours, 
            double rightAscension, double declination,
            out double solarAzimuth, out double solarZenith)
        {
            double greenwichMeanSideRealTime = (
                6.6974243242 + 0.0657098283 * elapsedJulianDays + decimalHours);
            double localMeanSideRealTime = (
                greenwichMeanSideRealTime * 15.0 + longitude) * rad;
            double hourAngle = localMeanSideRealTime - rightAscension;
            solarZenith = Math.Acos(
                Math.Cos(latitude * rad) * Math.Cos(hourAngle) * Math.Cos(declination)
                + Math.Sin(declination) * Math.Sin(latitude * rad));
            double azimuthY = -Math.Sin(hourAngle);
            double azimuthX = (
                Math.Tan(declination) * Math.Cos(latitude * rad) -
                Math.Sin(latitude * rad) * Math.Cos(hourAngle));
            solarAzimuth = Math.Atan2(azimuthY, azimuthX);
            if (solarAzimuth < 0.0)
            {
                solarAzimuth = solarAzimuth + Math.PI * 2.0;
            }
            solarAzimuth = solarAzimuth / rad;
            double parallaxError = (
                (earthMeanRadius / astronomicalUnit) * Math.Sin(solarZenith));
            solarZenith = (solarZenith + parallaxError) / rad;
        }

        private double CalculateFractionalYear(DateTime dateTime)
        {
            return (
                ((Math.PI * 2) / 365) *
                (dateTime.DayOfYear - 1 + (dateTime.Hour - 12) / 24));
        }

        private double CalculateEquationOfTime(double fractionalYear)
        {
            return (229.18 * (0.000075 + 
                0.001868 * Math.Cos(fractionalYear) -
                0.032077 * Math.Sin(fractionalYear) -
                0.014615 * Math.Cos(2 * fractionalYear) -
                0.040849 * Math.Sin(2 * fractionalYear)));
        }

        private double CalculateHourAngleSunrise(double latitude, double declination)
        {
            double latitudeRad = latitude * rad;
            double zenithRad = 90.833 * rad;

            double hourAngleX = (
                Math.Cos(zenithRad) /
                (Math.Cos(latitudeRad) * Math.Cos(declination)));
            double hourAngleY = (Math.Tan(latitudeRad) * Math.Tan(declination));
            double hourAngleRad = Math.Acos(hourAngleX - hourAngleY);
            return hourAngleRad * deg;            
        }

        private DateTime GetSunLimits(
            DateTime dateTime, double longitude, double hourAngle, 
            double equationOfTime)
        {
            double sunLimitMinutes = 720 - 4 * (longitude + hourAngle) - equationOfTime;
            double hours = sunLimitMinutes / 60;
            double minutes = sunLimitMinutes % 60;
            DateTime sunDateTime = dateTime.AddHours(hours);
            sunDateTime = sunDateTime.AddMinutes(minutes);
            return sunDateTime;
        }
    }
}