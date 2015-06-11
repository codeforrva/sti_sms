using System;
using System.Collections;
using System.Collections.Generic;

namespace Cfrva.Sti.Sms.Models
{
    public class Clinic
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Categories { get; set; }
        public string Distance { get; set; }

        public int CategoryCount
        {
            get
            {
                try
                {
                    var categories = Categories.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    return categories.Length;
                }
                catch (Exception)
                {
                    //error parsing category count, return zero
                    return 0;
                }
            }
        }

        public decimal ParsedDistance
        {
            get
            {
                decimal parsedDistance = 20;
                if (decimal.TryParse(Distance, out parsedDistance))
                {
                    return parsedDistance;
                }
                return 20; //default to 20 miles
            }
        }
    }
}