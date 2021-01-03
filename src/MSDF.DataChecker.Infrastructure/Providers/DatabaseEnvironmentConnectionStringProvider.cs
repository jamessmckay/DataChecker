// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;
using MSDF.DataChecker.Domain.Entities;

namespace MSDF.DataChecker.Domain.Providers
{
    public interface IDatabaseEnvironmentConnectionStringProvider
    {
        Task<string> GetConnectionString(DatabaseEnvironment environment);
    }

    public class DatabaseEnvironmentDatabaseEnvironmentConnectionStringProvider : IDatabaseEnvironmentConnectionStringProvider
    {
        private readonly IEncryptionProvider _encryptionProvider;

        public DatabaseEnvironmentDatabaseEnvironmentConnectionStringProvider(IEncryptionProvider encryptionProvider)
        {
            _encryptionProvider = encryptionProvider;
        }

        public async Task<string> GetConnectionString(DatabaseEnvironment environment)
        {
            if (environment.SecurityIntegrated != null && environment.SecurityIntegrated.Value)
            {
                return
                    $"Data Source={environment.DataSource};Database={environment.Database};Integrated Security=true;{environment.ExtraData}";
            }

            string password = await _encryptionProvider.DecryptStringAsync(environment.Password);

            return
                $"Data Source={environment.DataSource};Database={environment.Database};User Id={environment.User};Password={password};{environment.ExtraData}";
        }
    }
}
