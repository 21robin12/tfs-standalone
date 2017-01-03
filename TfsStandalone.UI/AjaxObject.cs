using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TfsStandalone.UI.Controllers;

namespace TfsStandalone.UI
{
    public class AjaxObject
    {
        public void Execute(string guid, string controllerName, string methodName, IList<object> data = null)
        {
            var type = GetControllerType(controllerName);
            var controller = Activator.CreateInstance(type);
            var method = type.GetMethod(methodName);
            Task.Factory
                .StartNew(() => method.Invoke(controller, data?.ToArray()))
                .ContinueWith((e) =>
                {
                    BrowserForm.Instance.ExecuteJsFunction("ajax.resolve", guid, e.Result?.ToString());
                });
        }

        private Type GetControllerType(string controllerName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var controllerType = types.FirstOrDefault(x => typeof(Controller).IsAssignableFrom(x) && x.Name == controllerName);
            if (controllerType == null)
            {
                throw new Exception($"Class extending Controller with name {controllerName} was not found in {assembly.FullName}");
            }

            return controllerType;
        }
    }
}
