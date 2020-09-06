using System;

namespace WoWAddonUpdater
{
    public static class Logger
    {
        public static void Exception(Exception exception)
        {
            Console.WriteLine(exception.ToString());
        }
    }
}