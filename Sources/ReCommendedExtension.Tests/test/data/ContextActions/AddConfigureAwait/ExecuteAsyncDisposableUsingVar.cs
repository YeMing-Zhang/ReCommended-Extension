﻿using System.Threading.Tasks;

namespace Test
{
    internal class Execute
    {
        async Task AsyncDisposable_UsingVar()
        {
            aw{caret}ait using var m1 = new MemoryStream();
        }
    }
}