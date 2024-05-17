using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game.Util
{
    public class Helper
    {
        public static void ReparentAllChildren(GameObject source, GameObject newParent)
        {
            // Get the number of children
            int childrenCount = source.transform.childCount;

            // Use a temporary array to store children as the hierarchy changes dynamically when reparenting
            Transform[] children = new Transform[childrenCount];

            // Populate the array with children
            for (int i = 0; i < childrenCount; i++)
            {
                children[i] = source.transform.GetChild(i);
            }

            // Set the new parent for each child
            foreach (Transform child in children)
            {
                child.SetParent(newParent.transform, true); // 'true' keeps the world position, 'false' would adapt to new parent's transform
            }
        }
        
        
        public static bool IsCurrentDateInRange(string dateRange)
        {
            string[] dates = dateRange.Split('-');
            if (dates.Length != 2)
            {
                throw new ArgumentException("Date range should be in the format 'MM/dd/yyyy-MM/dd/yyyy'");
            }

            DateTime startDate = DateTime.ParseExact(dates[0], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(dates[1], "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            DateTime currentDate = DateTime.Now.Date; // Using Date to ignore time component

            return (currentDate >= startDate && currentDate <= endDate);
        }
        
    }

}
