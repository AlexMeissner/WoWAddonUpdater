using System;

namespace WoWAddonUpdater
{
    public static class Logger
    {
        public static void Message(string message)
        {
            Properties.Settings.Default.Log = message + Environment.NewLine + Properties.Settings.Default.Log;
        }

        public static void Exception(Exception exception)
        {
            Message(exception.ToString());
        }
    }
}