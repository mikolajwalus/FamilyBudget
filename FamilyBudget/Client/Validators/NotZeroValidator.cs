using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace FamilyBudget.Client.Validators
{
    public class NotZeroValidator : ValidatorBase
    {
        [Parameter]
        public override string Text { get; set; } = "Amount cannot be zero, you can remove instead";

        protected override bool Validate(IRadzenFormComponent component)
        {
            var amount = component.GetValue();

            return amount != null && (decimal)amount != 0;
        }
    }
}
