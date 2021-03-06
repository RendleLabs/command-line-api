﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Environment;

namespace System.CommandLine.Suggest
{
    public class FileSuggestionRegistration : ISuggestionRegistration
    {
        public const string ResgistrationFileName = ".dotnet-suggest-registration.txt";
        public const string TestDirectroyOverride = "INTERNAL_TEST_DOTNET_SUGGEST_HOME";
        private readonly string _registrationConfigurationFilePath;

        public FileSuggestionRegistration(string registrationsConfigurationFilePath = null)
        {
            if (!string.IsNullOrWhiteSpace(registrationsConfigurationFilePath))
            {
                _registrationConfigurationFilePath = registrationsConfigurationFilePath;
                return;
            }

            var testDirectoryOverride = GetEnvironmentVariable(TestDirectroyOverride);
            if (!string.IsNullOrWhiteSpace(testDirectoryOverride))
            {
                _registrationConfigurationFilePath = Path.Combine(testDirectoryOverride, ResgistrationFileName);
                return;
            }

            var userProfile = GetFolderPath(SpecialFolder.UserProfile);
            if (userProfile != null)
            {
                _registrationConfigurationFilePath = Path.Combine(userProfile, ResgistrationFileName);
            }
        }

        public Registration FindRegistration(FileInfo soughtExecutable)
        {
            if (soughtExecutable == null)
            {
                return null;
            }

            if (_registrationConfigurationFilePath == null
                || !File.Exists(_registrationConfigurationFilePath))
            {
                return null;
            }

            string completionTarget = File.ReadAllLines(_registrationConfigurationFilePath).LastOrDefault(line =>
                line.StartsWith(soughtExecutable.FullName, StringComparison.OrdinalIgnoreCase));

            if (completionTarget == null)
            {
                // Completion provider not found!
                return null;
            }

            return new Registration(completionTarget);
        }

        public IEnumerable<Registration> FindAllRegistrations()
        {
            var allRegistration = new List<Registration>();

            if (_registrationConfigurationFilePath != null && File.Exists(_registrationConfigurationFilePath))
            {
                allRegistration
                    .AddRange(File
                        .ReadAllLines(_registrationConfigurationFilePath)
                        .Select(l => l.Trim())
                        .Where(l => l.Any())
                        .Select(item => new Registration(item))
                    );
            }

            return allRegistration;
        }

        public void AddSuggestionRegistration(Registration registration)
        {
            using (var writer = new StreamWriter(_registrationConfigurationFilePath, true))
            {
                writer.WriteLine(registration.ExecutablePath);
            }
        }
    }
}
