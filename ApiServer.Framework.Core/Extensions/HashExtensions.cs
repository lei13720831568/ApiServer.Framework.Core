using ApiServer.Framework.Core.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ApiServer.Framework.Core
{
    /// <summary>
    /// Extension methods for hashing strings
    /// </summary>
    public static class HashExtensions
    {
        /// <summary>
        /// HMACSHA256
        /// </summary>
        /// <param name="input"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static string Hs256(this string input,string secret) {
            if (String.IsNullOrEmpty(input)) return string.Empty;
            if (String.IsNullOrEmpty(secret)) secret = "";
            byte[] keybyte = Encoding.UTF8.GetBytes(secret);
            using (var sha = new HMACSHA256(keybyte))
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// HMACSHA256
        /// </summary>
        /// <param name="input"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static byte[] Hs256(this byte[] input, string secret)
        {
            if (input == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(secret)) secret = "";
            byte[] keybyte = Encoding.UTF8.GetBytes(secret);
            using (var sha = new HMACSHA256(keybyte))
            {
                return sha.ComputeHash(input);
            }
        }

        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha256(this string input)
        {
            if (String.IsNullOrEmpty(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        /// <summary>
        /// Creates a SHA256 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash.</returns>
        public static byte[] Sha256(this byte[] input)
        {
            if (input == null)
            {
                return null;
            }
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(input);
            }
        }
        /// <summary>
        /// Creates a SHA512 hash of the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A hash</returns>
        public static string Sha512(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using (var sha = SHA512.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// base64
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Base64(this string input) {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        public static string Base64(this byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        /// <summary>
        /// md5
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Md5(this string input, string salt="") {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input+ salt));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// md5
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Md5(this byte[] input, string salt = "")
        {
            using (var md5 = MD5.Create())
            {
                var inputSalt = input.Concat(Encoding.ASCII.GetBytes(salt)).ToArray();
                var result = md5.ComputeHash(inputSalt);
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// sm3
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SM3(this string input) {
            using var sm3 = SM3Algorithm.Create();
            var result = sm3.ComputeHash(Encoding.ASCII.GetBytes(input));
            var strResult = BitConverter.ToString(result).ToLower();
            return strResult.Replace("-", "");
        }
    }
}
