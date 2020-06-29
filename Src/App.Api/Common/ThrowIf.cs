using System;

namespace App.Api.Common
{
    internal static class ThrowIf
    {
        public static class Argument
        {
            public static void IsNull<T>(T argument)
            {
                if (argument is null)
                {
                    throw new ArgumentNullException(typeof(T).Name);
                }
            }
        }
    }
}
