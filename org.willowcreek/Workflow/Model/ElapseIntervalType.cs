namespace org.willowcreek.Workflow.Model
{
    public enum ElapseIntervalType
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2,
        Days = 3
    }

    /// <summary>
    /// I am constructing this class so that I can add these values to a Rock CustomRadioButtonField.
    /// They should be kept in sync with the above enumeration.
    /// </summary>
    public static class ElapseIntervalTypeHelper
    {
        public const string ElapseIntervalOptions = "Seconds,Minutes,Hours,Days";
    }
}