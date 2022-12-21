using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace FamilyBudget.Client.Validators
{
    public class UniqueValidator : ValidatorBase
    {
        [Parameter]
        public override string Text { get; set; } = "Value has to be unique";

        [Parameter]
        public IEnumerable<string> Values { get; set; }

        protected override bool Validate(IRadzenFormComponent component)
        {
            var value = component.GetValue();

            return value != null && Values != null && !Values.Contains(value.ToString());
        }
    }
}
