using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SharedLibrary.FactoriesManager;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.KernelInterfaces;
using SharedLibrary.FactoriesManager.Models;

namespace SentenceApiTests.FactoriesManagerTest
{
    [TestFixture]
    public class FactoriesManagerTest
    {
        private readonly IFactoriesManager factoriesManager =
            ManagersDictionary.Instance.GetManager("TestSentenceAPI");

        /// <summary>
        /// Add the Service factory which provides access to the interfaces IServiceA, IServiceB.
        /// Also add the factory which provides acces to the IServiceC interface
        /// The factory manager's method GetService should return the services IServiceA, IServiceB, IServiceC when
        /// asked to do so.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            factoriesManager.AddFactory(new FactoryInfo(new ServiceABFactory(), typeof(IServiceABFactory)));
            factoriesManager.AddFactory(new FactoryInfo(new ServiceCFactory(), typeof(IServiceCFactory)));
        }

        [Test]
        public void TestAddingServices()
        {
            factoriesManager.GetService<IServiceA>().TryGetTarget(out IServiceA serviceA);
            factoriesManager.GetService<IServiceB>().TryGetTarget(out IServiceB serviceB);
            factoriesManager.GetService<IServiceC>().TryGetTarget(out IServiceC serviceC);

            if (!(serviceA is IServiceA) || !(serviceA is ServiceA))
            {
                Assert.Fail("The serviceA is a wrong type");
            }

            if (!(serviceB is IServiceB) || !(serviceB is ServiceB))
            {
                Assert.Fail("The serviceB is a wrong type");
            }

            if (!(serviceC is IServiceC) || !(serviceC is ServiceC))
            {
                Assert.Fail("The serviceC is a wrong type");
            }
        }
    }

    interface IServiceA : IService { }
    interface IServiceB : IService { }
    interface IServiceC : IService { }

    class ServiceA : IServiceA { }
    class ServiceB : IServiceB { }
    class ServiceC : IServiceC { }

    interface IServiceABFactory : IServiceFactory
    {
        IServiceA GetServiceA();
        IServiceB GetServiceB();
    }


    class ServiceABFactory : IServiceABFactory
    {
        public IServiceA GetServiceA()
        {
            return new ServiceA();
        }

        public IServiceB GetServiceB()
        {
            return new ServiceB();
        }
    }

    interface IServiceCFactory : IServiceFactory
    {
        IServiceC GetServiceC();
    }

    class ServiceCFactory : IServiceCFactory
    {
        public IServiceC GetServiceC()
        {
            return new ServiceC();
        }
    }
}
