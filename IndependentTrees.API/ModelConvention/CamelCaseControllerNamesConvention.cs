using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace IndependentTrees.API.ModelConvention
{
    public class CamelCaseControllerNamesConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.ControllerName = ToCamelCase(controller.ControllerName);
            foreach (var action in controller.Actions)
                action.ActionName = ToCamelCase(action.ActionName);
        }

        private string ToCamelCase(string input) =>
            string.IsNullOrEmpty(input)
                ? input
                : char.ToLowerInvariant(input[0]) + input.Substring(1);
    }
}
