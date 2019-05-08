﻿using System;

namespace Wyam.Common.Shortcodes
{
    public class ShortcodeArgumentException : ArgumentException
    {
        public ShortcodeArgumentException(string message)
            : base(message)
        {
        }

        public ShortcodeArgumentException(string message, string paramName)
            : base(message, paramName)
        {
        }
    }
}
