using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMSVotingSystem.Application.Common
{
    public static class TOTPUtility
    {
        private static readonly int[] DIGITS_POWER = { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000 };

        public static string GenerateRandomSecret()
        {
            byte[] secret = new byte[20]; // 160 bits as recommended for SHA1
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(secret);
            }

            // Convert to Base32 for easier SMS transmission
            return Base32Encode(secret);
        }

        public static string GenerateTOTP(string base32Secret, long timeCounter, int digits = 6)
        {
            byte[] secret = Base32Decode(base32Secret);
            byte[] message = BitConverter.GetBytes(timeCounter);

            // Ensure big-endian format
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(message);
            }

            // Get the HMAC-SHA1 hash
            using (var hmac = new HMACSHA1(secret))
            {
                byte[] hash = hmac.ComputeHash(message);

                // Dynamic truncation
                int offset = hash[hash.Length - 1] & 0xf;
                int binary = ((hash[offset] & 0x7f) << 24) |
                           ((hash[offset + 1] & 0xff) << 16) |
                           ((hash[offset + 2] & 0xff) << 8) |
                           (hash[offset + 3] & 0xff);

                int otp = binary % DIGITS_POWER[digits];
                string result = otp.ToString().PadLeft(digits, '0');

                return result;
            }
        }

        public static string GenerateCurrentTOTP(string base32Secret, int digits = 6, int timeStepSeconds = 300)
        {
            long timeCounter = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / timeStepSeconds;
            return GenerateTOTP(base32Secret, timeCounter, digits);
        }

        public static string GenerateHOTP(string base32Secret, long counter, int digits = 6)
        {
            // HOTP is the same algorithm as TOTP but with counter instead of time
            return GenerateTOTP(base32Secret, counter, digits);
        }

        public static bool VerifyTOTP(string base32Secret, string totpCode, int digits = 6, int timeStepSeconds = 300, int windowSize = 1)
        {
            long currentTimeCounter = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / timeStepSeconds;

            // Check current and adjacent time windows
            for (int i = -windowSize; i <= windowSize; i++)
            {
                string generatedCode = GenerateTOTP(base32Secret, currentTimeCounter + i, digits);
                if (generatedCode == totpCode)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool VerifyHOTP(string base32Secret, string hotpCode, long counter, int digits = 6, int lookAheadWindow = 10)
        {
            // Check current counter and look ahead window for counter drift
            for (long i = 0; i < lookAheadWindow; i++)
            {
                string generatedCode = GenerateHOTP(base32Secret, counter + i, digits);
                if (generatedCode == hotpCode)
                {
                    return true;
                }
            }

            return false;
        }


        public static int GetRemainingSecondsForCurrentTimeStep()
        {
            long currentTimeStepNumber = GetCurrentTimeStepNumber();
            long secondsIntoCurrentTimeStep = (long)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % 300);
            return 300 - (int)secondsIntoCurrentTimeStep;
        }

        private static long GetCurrentTimeStepNumber()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 60;
        }
        // Base32 encoding/decoding for human-readable secrets via SMS
        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            StringBuilder result = new StringBuilder();

            int bitsLeft = 0;
            int buffer = 0;

            foreach (byte b in data)
            {
                buffer = (buffer << 8) | b;
                bitsLeft += 8;

                while (bitsLeft >= 5)
                {
                    bitsLeft -= 5;
                    result.Append(alphabet[(buffer >> bitsLeft) & 31]);
                }
            }

            if (bitsLeft > 0)
            {
                buffer = buffer << (5 - bitsLeft);
                result.Append(alphabet[buffer & 31]);
            }

            return result.ToString();
        }

        private static byte[] Base32Decode(string input)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            // Convert to uppercase and remove any padding
            input = input.TrimEnd('=').ToUpper();

            // Prepare output byte array
            int byteCount = input.Length * 5 / 8;
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0;
            int bitsRemaining = 8;
            int arrayIndex = 0;

            foreach (char c in input)
            {
                int value = alphabet.IndexOf(c);
                if (value < 0)
                    throw new ArgumentException("Invalid Base32 character: " + c);

                if (bitsRemaining > 5)
                {
                    bitsRemaining -= 5;
                    curByte |= (byte)(value << bitsRemaining);

                    if (bitsRemaining == 0)
                    {
                        returnArray[arrayIndex++] = curByte;
                        curByte = 0;
                        bitsRemaining = 8;
                    }
                }
                else
                {
                    int bitsToAdd = 5 - bitsRemaining;
                    curByte |= (byte)(value >> bitsToAdd);
                    returnArray[arrayIndex++] = curByte;

                    curByte = (byte)((value << (8 - bitsToAdd)) & 0xFF);
                    bitsRemaining = 8 - bitsToAdd;
                }
            }

            // If we didn't end on a full byte
            if (arrayIndex < byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }
    }
}