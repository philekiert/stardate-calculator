﻿using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xaml;
using System.Xml.Schema;

public class Stardate
{
    /************************************************************

    TOS    Starts  2266  0193.29  +1496.5725
           Ends    2270  6179.58    
    TMP    Starts  2267  5701.73  +133.07789
           Ends    2286  8230.21
    Films  Starts  2283  7697.87  +188.116
           Ends    2294  9767.15
    TNG    Starts  2323  0.0      +1000.0


    Some key stardates for reference
    --------------------------------

    1512.0    2266    The Corbomite Maneuvre
    5693.0    2269    Tholian Web, assume that this is 4 years on

    7412.0    2279    Star Trek: The Motion Picture
    8130.0    2285    The Wrath of Khan
    8210.0    2285    The Search for Spock
    8390.0    2286    The Voyage Home 
    8454.0    2287    The Final Frontier
    9521.0    2292    The Undiscovered country
    9715.0    2293    Generations

    54868.6   23--    868.6 along the year is a few days after First Contact Day.

    ************************************************************/

    private const double TOS_ROOT = 2265.1893d; // Changed from 2265.8709 for aesthetic reasons
    private const double TOS_INCREMENT = 1496.2162d; // Changed from 1496.5725d
    private const double TMP_ROOT = 2224.155d;
    private const double TMP_INCREMENT = 133.07789d;
    private const double FILMS_ROOT = 2242.08d;
    private const double FILMS_INCREMENT = 188.116d;
    private const double TNG_ROOT = 2323.3981d; // __868.6 should be a few days after First Contact day (April 5th).
    private const double TNG_INCREMENT = 1000d;

    private DateTime earthDate;
    private double tosMetric;
    private double tmpMetric;
    private double filmsMetric;
    private double tngMetric;

    public string FormatStardate(object stardate)
    {
        if (stardate is DateTime dt)
            return dt.ToString("dd/MM/yyyy HH:mm");
        if (stardate is double d)
            return ((int)(d * 100d) / 100d).ToString();
        else
            return "Error";
    }

    public DateTime EarthDate { get { return earthDate; } }
    public double TOS { get { return tosMetric; } }
    public double TMP { get { return tmpMetric; } }
    public double Films { get { return filmsMetric; } }
    public double TNG { get { return tngMetric; } }
    public object Primary
    {
        get
        {
            if (tosMetric < 0)
                return earthDate;
            if (earthDate.Year < 2280)
                return tosMetric < tmpMetric ? tosMetric : tmpMetric;
            if (earthDate.Year < 2300)
                return tmpMetric > filmsMetric ? tmpMetric : filmsMetric;
            return filmsMetric > tngMetric ? filmsMetric : tngMetric;
        }
    }

    public Stardate(DateTime dateTime)
    {
        Calculate(dateTime);
    }
    public Stardate(double stardate)
    {
        // Truncation somewhere in the back and forth of things can lead to stardates of, say, 5000 getting
        // read back as 4999.99.
        stardate += .0000001d;
        if (stardate < 7000)
        {
            double realTOS = (stardate / TOS_INCREMENT) + TOS_ROOT;
            double realTMP = (stardate / TMP_INCREMENT) + TMP_ROOT;
            Calculate(realTOS > realTMP ? realTOS : realTMP);
        }
        else if (stardate < 10000)
        {
            double realTMP = (stardate / TMP_INCREMENT) + TMP_ROOT;
            double realFilms = (stardate / FILMS_INCREMENT) + FILMS_ROOT;
            Calculate(realTMP < realFilms ? realTMP : realFilms);
        }
        else
        {
            double realFilms = (stardate / FILMS_INCREMENT) + FILMS_ROOT;
            double realTNG = (stardate / TNG_INCREMENT) + TNG_ROOT;
            Calculate(realFilms < realTNG ? realFilms : realTNG);
        }
    }

    public void Calculate(double dateReal)
    {
        // Convert the double to a DateTime.
        DateTime dt = new DateTime((int)dateReal, 1, 1);
        Calculate(dateReal, dt.AddMilliseconds((DateTime.IsLeapYear(dt.Year) ? 31_622_400_000 : 31_536_000_000) *
                                               (dateReal % 1)));
    }
    public void Calculate(DateTime dateTime)
    {
        // generate and return a double representing the year and fraction through the year.
        double secondsAlongYear = ((dateTime.DayOfYear - 1) * 86_400d) +
                                  (dateTime.Hour * 3_600) +
                                  (dateTime.Minute * 60) +
                                  (dateTime.Second) +
                                  (dateTime.Millisecond / 1000d);
        Calculate(dateTime.Year + (secondsAlongYear / (DateTime.IsLeapYear(dateTime.Year) ? 31_622_400 : 31_536_000)),
                  dateTime);
    }
    public void Calculate(double dateReal, DateTime dateTime)
    {
        earthDate = dateTime;
        tosMetric = (dateReal - TOS_ROOT) * TOS_INCREMENT;
        tmpMetric = (dateReal - TMP_ROOT) * TMP_INCREMENT;
        filmsMetric = (dateReal - FILMS_ROOT) * FILMS_INCREMENT;
        tngMetric = (dateReal - TNG_ROOT) * TNG_INCREMENT;
    }
}
