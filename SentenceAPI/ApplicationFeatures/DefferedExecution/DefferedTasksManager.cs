using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using SentenceAPI.ApplicationFeatures.Loggers;
using SentenceAPI.ApplicationFeatures.Loggers.Interfaces;
using SentenceAPI.ApplicationFeatures.Loggers.Models;

namespace SentenceAPI.ApplicationFeatures.DefferedExecution
{
    /// <summary>
    /// The deffered tasks manager is used when we want to perform an action, but this action is not so critical,
    /// and can be done after some time after the client got a resposne from server. This manager has a queue for
    /// such deffered actions, and a thread, which always monitors the state of the queue. if the queue is not empty,
    /// then the Action from the queue is invoked. From any part of the programm such actions can be inserted in the queue.
    /// </summary>
    public static class DefferedTasksManager
    {
        private static readonly object locker = new object();

        private static ILogger<ApplicationError> exceptionLogger;
        private static Queue<Action> tasks;

        public static void Initialize()
        {
            tasks = new Queue<Action>();
            exceptionLogger = new ExceptionLogger();
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

            new Thread(() =>
            {
                while (true)
                {
                    lock (locker)
                    {
                        tasks.Enqueue(task);
                        break;
                    }
                }
            }).Start();
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

                    lock (locker)
                    {
                        tasks.TryDequeue(out action);
                    }

                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        exceptionLogger.Log(new ApplicationError(ex));
                    }

                    Thread.Sleep(100); 
                }
            }).Start();
        }
    }
}
