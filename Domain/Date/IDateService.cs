using System;

using Domain.KernelInterfaces;


namespace Domain.Date
{
    public interface IDateService : IService
    {
        DateTime Now => DateTime.Now;
    }
}