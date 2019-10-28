using System.Collections.Generic;
using System;

using SharedLibrary.FactoriesManager.Interfaces;

namespace SharedLibrary.FactoriesManager
{ 
    public class ManagersDictionary
    { 
        #region Fields
        private Dictionary<string, IFactoriesManager> managers;
        #endregion

        #region Singleton
        static ManagersDictionary() { }
        private ManagersDictionary() { }

        private static ManagersDictionary managersDictionary;
        public static ManagersDictionary Instance 
        {
            get
            { 
                if (managersDictionary is null)
                { 
                    managersDictionary = new ManagersDictionary();
                }

                return managersDictionary;
            }
        }
        #endregion

        public ManagersDictionary AddFactoriesManager(string name)
        { 
            if (name is null)
            {
                throw new ArgumentNullException("None of the arguments can be null");
            }

            if (managers.ContainsKey(name)) 
            { 
                throw new ArgumentException("The factories manager with such name already added");
            }

            managers.Add(name, new FactoriesManager());

            return this;
        }

        public ManagersDictionary RemoveManager(string name)
        { 
            if (managers.ContainsKey(name))
            { 
                managers.Remove(name);
            }

            return this;
        }

        public IFactoriesManager GetManager(string name)
        { 
            if (!managers.ContainsKey(name))
            {
                throw new ArgumentException($"There is no manager with the name: ${name}");
            }

            return managers[name];
        }
    }
}