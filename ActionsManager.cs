using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Threading;

namespace ButtonsController
{
    /// <summary>
    /// Provides a way to interact with a dynamically linked library, which will handle all the actions.
    /// </summary>
    public class ActionsManager : IDisposable
    {
        public string AssemblyName;
        public Assembly LoadedAssembly;
        public Type ActionsClass;
        public MethodInfo Initializer;
        public MethodInfo ActionsExecuter;
        public MethodInfo ExitMethod;

        /// <summary>
        /// Initializes a new instance of AcionsManager.
        /// </summary>
        /// <param name="dllName">The name of the Dynamically Linked Library to import</param>
        public ActionsManager(string dllName)
        {
            AssemblyName = dllName;
            LoadedAssembly = Assembly.Load(dllName);
            ActionsClass = LoadedAssembly.GetType("CustomActions.Actions");
            Initializer = ActionsClass.GetMethod("Initialize");
            ActionsExecuter = ActionsClass.GetMethod("ExecuteActions");
            ExitMethod = ActionsClass.GetMethod("Exit");
        }

        /// <summary>
        /// Calls the static method Actions.Initialize() on the DLL (on a new thread)
        /// </summary>
        public void InitializeLibrary()
        {
            Initializer.Invoke(null, null);
        }

        /// <summary>
        /// Calls the static method Actions.ExecuteAction(object[] {byte}) on the DLL (on a new thread)
        /// </summary>
        /// <param name="actionNumber">The number of the action. Max 256 actions is enough.</param>
        public void ExecuteAction(byte actionCode)
        {
            if (Program.Config.RunActionsOnNewThread)
            {
                ThreadStart s = new ThreadStart(() => ActionsExecuter.Invoke(null, new object[] { actionCode }));
                Thread t = new Thread(s, 0);
                t.Name = "ActionsManager Action Thread";
                t.Start();
            }
            else
            {
                ActionsExecuter.Invoke(null, new object[] { actionCode });
            }
        }

        /// <summary>
        /// Calls the static method Actions.Exit() on the DLL (on a new thread)
        /// Actions.Exit() NEEDS to dispose all the objects
        /// </summary>
        public void Dispose()
        {
            ExitMethod.Invoke(null, null);
        }
    }
}
