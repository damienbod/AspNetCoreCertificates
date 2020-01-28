using System;
using System.Text;

namespace CertificateManager
{
    /// <summary>
    /// src:  Mathias Raacke
    /// https://github.com/oocx/ReadX509CertificateFromPem
    /// </summary>
    public static class PemDecoder
    {
        public static byte[] DecodeSection(string data, string type)
        {
            var lines = data.Replace("\r", "").Split("\n");
            bool inSection = false;
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!inSection)
                {
                    if (line == $"-----BEGIN {type}-----")
                    {
                        inSection = true;
                    }
                }
                else
                {
                    if (line == $"-----END {type}-----")
                    {
                        break;
                    }
                    else
                    {
                        sb.Append(line);
                    }

                }
            }
            return Convert.FromBase64String(sb.ToString());
        }

        public static string GetBegin(string type)
        {
            return $"-----BEGIN {type}-----";
        }

        public static string GetEnd(string type)
        {
            return $"-----END {type}-----";
        }
    }
}
