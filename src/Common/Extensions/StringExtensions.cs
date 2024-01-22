using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using DNTPersianUtils.Core;

namespace PortalCore.Common.Extensions
{
    public static class StringExtensions
    {
        [return: NotNullIfNotNull("mobileNumber")]
        public static string? FixMobileNumber(this string? mobileNumber)
        {
            if (mobileNumber is null)
            {
                return null;
            }

            mobileNumber = mobileNumber.ToEnglishNumbers();
            if (mobileNumber.StartsWith("+98", StringComparison.OrdinalIgnoreCase))
            {
                mobileNumber = mobileNumber.Replace("+98", "0", StringComparison.OrdinalIgnoreCase);
            }

            if (mobileNumber.StartsWith("98", StringComparison.OrdinalIgnoreCase))
            {
                mobileNumber = mobileNumber.Replace("98", "0", StringComparison.OrdinalIgnoreCase);
            }

            if (mobileNumber.StartsWith("0098", StringComparison.OrdinalIgnoreCase))
            {
                mobileNumber = mobileNumber.Replace("0098", "0", StringComparison.OrdinalIgnoreCase);
            }

            if (!mobileNumber.StartsWith("0", StringComparison.OrdinalIgnoreCase))
            {
                mobileNumber = "0" + mobileNumber;
            }

            return mobileNumber;
        }

        public static string GenerateRandomCode(int codeLenght = 4, bool useStaticCode = false, string staticCode = "1234")
        {
            if (useStaticCode)
            {
                return staticCode;
            }

            StringBuilder builder = new();

            var rand = new Random();

            for (int i = 0; i < codeLenght; i++)
            {
                var code = rand.Next(0, 9);

                builder.Append(code);
            }

            return builder.ToString();
        }

        public static string RemoveFromEnd(this string input, string removeString, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (input.EndsWith(removeString, stringComparison))
            {
                return input.Substring(0, input.Length - removeString.Length);
            }
            else
            {
                return input;
            }
        }
    }
}
