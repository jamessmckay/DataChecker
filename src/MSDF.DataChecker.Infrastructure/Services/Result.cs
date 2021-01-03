// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

namespace MSDF.DataChecker.Domain.Services
{
    public class Result<T>
    {
        private Result(string reason) => FailureReason = reason;

        private Result(T payload) => Payload = payload;

        public T Payload { get; }

        public string FailureReason { get; }

        public bool IsSuccess
        {
            get => FailureReason == null;
        }

        public bool IsUpdated { get; set; }

        public static Result<T> Fail(string reason) => new Result<T>(reason);

        public static Result<T> Success(T payload) => new Result<T>(payload);

        public static implicit operator bool(Result<T> result) => result.IsSuccess;
    }
}
