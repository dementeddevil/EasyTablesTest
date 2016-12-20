﻿using System.IO;

namespace ViewSliderSamples
{
    public static class StreamExtensions
    {
        #region Methods

        public static byte[] ToByte(this Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        #endregion
    }
}