using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace parking
{
    public class price
    {
        public int minutes;
        public string dayshoursmns;
        public int amount;

        public price(DBConnect db, string numbercar, string mns)
        {
            minutes = int.Parse(mns);
            string datenow = getBaseDateNow();
            string id_rate = "0";
            string req = "";
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0: // We check if there is a specific subscription for car number
                        req = "select * from subscription where ident = '" + numbercar + "' and from_dt <= '" + datenow + "' and to_dt >= '" + datenow + "' order by id desc";
                        break;
                    case 1: // We check if there is a specific subscription for car number
                        req = "select * from subscription where ident = '" + numbercar + "' and from_dt <= '" + datenow + "' order by id desc";
                        break;
                    case 2: 
                        req = "select * from subscription where ident = '' and from_dt <= '" + datenow + "' and to_dt >= '" + datenow + "' order by id desc";
                        break;
                    case 3: 
                        req = "select * from subscription where ident = '' and from_dt <= '" + datenow + "' order by id desc";
                        break;
                }

                ArrayList results = db.Select(req);
                if (results.Count != 0)
                {
                    NameValueCollection row = (NameValueCollection)results[0];
                    id_rate = row["id_rate"];
                }
                if (id_rate != "0") break;
            }
            // No price found
            if (id_rate == "0")
            {
                amount = 0;
                dayshoursmns = converMnsToDayshoursmns();
            }
            else
            {
                int unit = 30;
                // Rate infos
                req = "select * from rates where id = '" + id_rate + "'";
                ArrayList results = db.Select(req);
                if (results.Count != 0)
                {
                    NameValueCollection row = (NameValueCollection)results[0];
                    unit = Int32.Parse(row["unit"]);
                    // search price for segment
                    req = "select * from rate_details where id_rate='" + id_rate + "' and limitmns >= '" + minutes.ToString() + "' order by limitmns asc";
                    results = db.Select(req);
                    if (results.Count == 0)
                    {
                        req = "select * from rate_details where id_rate='" + id_rate + "' and limitmns <= '" + minutes.ToString() + "' order by limitmns desc";
                        results = db.Select(req);
                    }
                    if (results.Count != 0)
                    {
                        row = (NameValueCollection)results[0];
                        int nbsegment = minutes / unit;
                        int remainseg = minutes % unit;
                        if (remainseg > 0) nbsegment++;
                        amount = Int32.Parse(row["price"]) * nbsegment;
                    }
                    else amount = 0;
                }
                else amount = 0;
                dayshoursmns = converMnsToDayshoursmns();
            }
        }

        public string getBaseDateNow()
        {
            DateTime localDate = DateTime.Now;
            return localDate.Year.ToString() + "-" + localDate.Month.ToString() + "-" + localDate.Day.ToString();
        }

        public string converMnsToDayshoursmns()
        {
            string res = "";
            dayshoursmns = minutes.ToString();
            int nbhours = minutes / 60;
            int remainmns = minutes % 60;
            int nbdays = nbhours / 24;
            int remainhours = nbhours % 24;
            if (nbdays != 0)
                res += nbdays.ToString() + "d ";
            if (remainhours != 0)
            {
                if (remainhours < 10)
                    res += "0" + remainhours.ToString() + ":";
                else res += remainhours.ToString() + ":";
            }
            else res += "00:";
            if (remainmns != 0)
            {
                if (remainmns < 10)
                    res += "0" + remainmns.ToString();
                else res += remainmns.ToString();
            }
            else res += "00";
            dayshoursmns = res;
            return dayshoursmns;
        }
    }
}