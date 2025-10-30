namespace Spikescape.Leaderboard
{
    public static class NameValidator
    {
        public static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Name cannot be empty.";

            if (NameFilter.Instance == null || !NameFilter.Instance.IsReady)
                return "Internal system error. Try again.";

            if (!NameFilter.Instance.IsNameValid(name))
                return "Innapropriate name detected. Please try something else.";

            return null;
        } 
    }
}