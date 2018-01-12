using System;

namespace KanjiSeven.Utils
{
    public static class SystemEnviroment
    {
        public static bool IsUnix =>
            (int)Environment.OSVersion.Platform == 4 ||
            (int)Environment.OSVersion.Platform == 6 ||
            (int)Environment.OSVersion.Platform == 128;
    }
}