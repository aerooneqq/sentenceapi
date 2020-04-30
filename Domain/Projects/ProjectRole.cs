using System;
using Domain.KernelModels;
using MongoDB.Bson;

namespace Domain.Projects
{
    public enum ProjectRole
    {
        Creator = 0,
        Participant,
        Observer,
    }
}