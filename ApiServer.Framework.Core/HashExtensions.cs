﻿using System;
using System.Collections.Generic;
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

        /// <summary>
        /// md5
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Md5(this string input, string salt="") {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
