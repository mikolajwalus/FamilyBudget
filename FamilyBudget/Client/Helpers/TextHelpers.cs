namespace FamilyBudget.Client.Helpers
{
    public static class TextHelpers
    {
        public static string FormatMoneyToDisplay(decimal moneyAmount)
        {
            return String.Format(new System.Globalization.CultureInfo("en-US"), "{0:C}", moneyAmount);
        }

        public static string GetTextColorBasedOnValue(decimal moneyAmount)
        {
            return moneyAmount > 0 ? "text-success" : moneyAmount < 0 ? "text-danger" : "text-secondary";
        }
    }
}
