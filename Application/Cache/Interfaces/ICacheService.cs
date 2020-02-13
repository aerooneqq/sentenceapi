﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Caching.Interfaces
{
    public interface ICacheService
    {
        bool Contains(string key);
        object GetValue(string key);
        bool TryInsert(string key, object obj);
    }
}
