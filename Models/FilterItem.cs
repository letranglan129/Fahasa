﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fahasa.Models
{
    public class FilterItem<T>
    {
        public string type { get; set; }
        public T value { get; set; }
    }
}