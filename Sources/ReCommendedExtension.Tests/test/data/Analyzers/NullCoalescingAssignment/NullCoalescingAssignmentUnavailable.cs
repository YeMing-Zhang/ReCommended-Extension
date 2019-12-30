﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class Sample
    {
        void EqualityComparison(string value)
        {
            if (value == null)
            {
                value = "one";
            }
        }

        void EqualityComparison_Reverse(string value)
        {
            if (null == value)
            {
                value = "one";
            }
        }

        void IsComparison(string value)
        {
            if (value is null)
            {
                value = "one";
            }
        }
    }
}