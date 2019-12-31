using System;
using System.Collections.Generic;
using System.Threading;

using SentenceAPI.ApplicationFeatures.Loggers;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;
using SentenceAPI.ApplicationFeatures.Loggers.Configuration;


using System.Collections.Concurrent;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.FactoriesManager;

namespace SentenceAPI.ApplicationFeatures.DefferedExecution
{
    /// <summary>
    /// The deffered tasks manager is used when we want to perform an action, but this action is not so critical,
    /// and can be done after some time after the client got a resposne from the server. This manager has a queue for
    /// such deffered actions, and a thread, which always monitors the state of the queue. if the queue is not empty,
    /// then the Action from the queue is invoked. From any part of the programm such actions can be inserted in the queue.
    /// </summary>
    public static class DefferedTasksManager
    {
        private static readonly object locker = new object();

        #region Factories
        private static readonly IFactoriesManager factoriesManager = 
            ManagersDictionary.Instance.GetManager(Startup.ApiName);
        #endregion

        private static ILogger<ApplicationError> exceptionLogger;
        private static ConcurrentQueue<Action> actions;

        public static void Initialize()
        {
            actions = new ConcurrentQueue<Action>();
            
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
        }

        /// <summary>
        /// Adds the task to the tasks list
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If the task is NULL
        /// </exception>
        public static void AddTask(Action task)
        { 
            if (task == null)
            { 
                throw new ArgumentNullException("The task can not be null");
            }

            actions.Enqueue(task);
        }

        /// <summary>
        /// Starts the work of the Deffered task manager, by creating the thread and checking the tasks stack.
        /// if the stack 
        /// </summary>
        public static void Start()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Action action;

                    if (actions.TryDequeue(out action))
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error);
                        }
                    }
                    else 
                    {
                        Thread.Yield();
                    }
                }
            }).Start();
        }
    }
}
