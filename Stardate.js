class Stardate {
    #TOS_ROOT = 2265.1893; // Changed from 2265.8709 for aesthetic reasons
    #TOS_INCREMENT = 1496.2162; // Changed from 1496.5725d
    #TMP_ROOT = 2224.155;
    #TMP_INCREMENT = 133.07789;
    #FILMS_ROOT = 2242.08;
    #FILMS_INCREMENT = 188.116;
    #TNG_ROOT = 2323.3981; // __868.6 should be a few days after First Contact day (April 5th).
    #TNG_INCREMENT = 1000;

    #earthDate = null;
    #tosMetric = null;
    #tmpMetric = null;
    #filmsMetric = null;
    #tngMetric = null;
    
    #IsLeapYear(year) {
        return (year % 4 === 0 && year % 100 !== 0) || year % 400 === 0;
    }

    FormatStardate(stardate) {
        if (stardate instanceof Date) {
            let str = String(stardate.getDate()).padStart(2, '0')
            str += "/" + String(stardate.getMonth() + 1).padStart(2, '0')
            if (stardate.getFullYear() >= 0)
                str += "/" + String(stardate.getFullYear()).padStart(4, '0')
            else
                str += "/-" + String(Math.abs(stardate.getFullYear())).padStart(4, '0')
            str += " " + String(stardate.getHours()).padStart(2, '0')
            str += ":" + String(stardate.getMinutes()).padStart(2, '0')
            return str;
        }            
        if (typeof(stardate) === 'number')
            return String(stardate.toFixed(2));
        else
            return "Error";
    }

    EarthDate() { return this.#earthDate; }
    TOS() { return this.#tosMetric; }
    TMP() { return this.#tmpMetric; }
    Films() { return this.#filmsMetric; }
    TNG() { return this.#tngMetric; }
    Primary(forceStardate) {
        if (this.#earthDate == null)
            return null;
        if (this.#tosMetric < 0 && !forceStardate)
            return this.#earthDate;
        if (this.#tosMetric < 22160) // ~year 2080
            return this.#tosMetric < this.#tmpMetric ? this.#tosMetric : this.#tmpMetric;
        if (this.#tosMetric < 52084) // ~year 2300
            return this.#tmpMetric > this.#filmsMetric ? this.#tmpMetric : this.#filmsMetric;
        return this.#filmsMetric > this.#tngMetric ? this.#filmsMetric : this.#tngMetric;
    }

    Calculate(stardate)
    {
        if (typeof(stardate) === 'number') {
            // Truncation somewhere in the back and forth of things can lead to stardates of, say, 5000 getting
            // read back as 4999.99.
            stardate += .0000001;
            // Convert the stardate value into a number that represents the date in terms of year and fraction of year.
            if (stardate < 7000) {
                let realTOS = (stardate / this.#TOS_INCREMENT) + this.#TOS_ROOT;
                let realTMP = (stardate / this.#TMP_INCREMENT) + this.#TMP_ROOT;
                stardate = realTOS > realTMP ? realTOS : realTMP;
            }
            else if (stardate < 10000) {
                let realTMP = (stardate / this.#TMP_INCREMENT) + this.#TMP_ROOT;
                let realFilms = (stardate / this.#FILMS_INCREMENT) + this.#FILMS_ROOT;
                stardate = realTMP < realFilms ? realTMP : realFilms;
            }
            else {
                let realFilms = (stardate / this.#FILMS_INCREMENT) + this.#FILMS_ROOT;
                let realTNG = (stardate / this.#TNG_INCREMENT) + this.#TNG_ROOT;
                stardate = realFilms < realTNG ? realFilms : realTNG;
            }
            // Convert the double to a DateTime.
            let dt = new Date(Math.trunc(stardate), 0, 1);
            dt.setTime(dt.getTime() +
                       (this.#IsLeapYear(dt.getFullYear()) ? 31622400000 : 31536000000) * (stardate % 1));
            this.#Apply(stardate, dt);
        }
        else if (stardate instanceof Date) {
            // generate and return a double representing the year and fraction through the year.
            let secondsAlongYear = (stardate - new Date(stardate.getFullYear(), 0, 1)) / 1000;
            this.#Apply(stardate.getFullYear() + (secondsAlongYear /
                                                 (this.#IsLeapYear(stardate.getFullYear()) ? 31622400 : 31536000)),
                        stardate);
        }
        else {
            console.error("Invalid input: stardate must be a number or a Date object.");
        }
    }
    #Apply(dateReal, dateTime) {
        if (typeof(dateReal) != 'number' && !(dateTime instanceof Date)) {
            console.error("Invalid input: stardate must be a number or a Date object.");
        } else {
            this.#earthDate = dateTime;
            this.#tosMetric = (dateReal - this.#TOS_ROOT) * this.#TOS_INCREMENT;
            this.#tmpMetric = (dateReal - this.#TMP_ROOT) * this.#TMP_INCREMENT;
            this.#filmsMetric = (dateReal - this.#FILMS_ROOT) * this.#FILMS_INCREMENT;
            this.#tngMetric = (dateReal - this.#TNG_ROOT) * this.#TNG_INCREMENT;
        }
    }
}
