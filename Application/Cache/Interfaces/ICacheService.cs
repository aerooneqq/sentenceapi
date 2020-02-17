﻿using Domain.KernelInterfaces;

namespace Application.Caching.Interfaces
{
    public interface ICacheService : IService
    {
        bool Contains(string key);
        object GetValue(string key);
        bool TryInsert(string key, object obj);
    }
}
