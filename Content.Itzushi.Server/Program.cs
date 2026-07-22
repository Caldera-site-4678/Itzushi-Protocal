// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Server;

namespace Content.Itzushi.Server;

internal static class Program
{
    public static void Main(string[] args)
    {
        // c# doing this randomly breaks replay saving, linux doesn't need file locking
        AppContext.SetSwitch("System.IO.DisableFileLocking", true);
        ContentStart.Start(args);
    }
}
