//2015 Apache2, WinterDev
using System;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;

namespace MongoDB.Util
{
    public static class SeqCompare
    {
        public static bool CompareByteSeq(byte[] s1, byte[] s2)
        {
            int j = s1.Length;
            if (j != s2.Length)
            {
                return false;
            }
            for (int i = 0; i < j; ++i)
            {
                if (s1[i] != s2[i])
                {
                    return false;
                }
            }
            return true;
        }

    }


}