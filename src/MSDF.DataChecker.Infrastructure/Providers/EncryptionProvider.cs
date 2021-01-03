// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MSDF.DataChecker.Domain.Providers
{
    public interface IEncryptionProvider
    {
        Task<string> EncryptStringAsync(string plainText);

        Task<string> DecryptStringAsync(string cipherText);
    }

    public class EncryptionProvider : IEncryptionProvider
    {
        private const string Plaintext = "PlainText";
        private readonly string _key;

        public EncryptionProvider(IConfiguration configuration)
        {
            _key = configuration.GetValue<string>("EncryptionKey") ?? Plaintext;
        }

        public async Task<string> EncryptStringAsync(string plainText)
        {
            if (_key.Equals(Plaintext))
            {
                return plainText;
            }

            using Aes aes = Aes.Create();

            SetAes(aes);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            await using MemoryStream memoryStream = new MemoryStream();

            await using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            await using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
            {
                await streamWriter.WriteAsync(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public async Task<string> DecryptStringAsync(string cipherText)
        {
            if (_key.Equals(Plaintext) || string.IsNullOrEmpty(cipherText))
            {
                return cipherText;
            }

            byte[] buffer = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();

            SetAes(aes);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            await using var memoryStream = new MemoryStream(buffer);

            await using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            return await new StreamReader(cryptoStream).ReadToEndAsync();
        }

        private void SetAes(Aes aes)
        {
            aes.Key = Encoding.UTF8.GetBytes(_key);
            aes.IV = new byte[16];
        }
    }
}
