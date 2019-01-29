// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Class for providing a safe way to manipulating directory permissions.
    /// </summary>
    public class DirectoryPermissionsRevoker : IDisposable
    {
        private readonly List<FileSystemAccessRule> appliedFileSystemAccessRules = new List<FileSystemAccessRule>();
        private readonly string folderPath;
        private readonly DirectoryInfo directoryInfo;
        private bool disposed;

        /// <summary>
        /// Creates an instance of <see cref="DirectoryPermissionsRevoker"/>.
        /// Adds a <paramref name="rights"/> of type <see cref="AccessControlType.Deny"/> to the access
        /// rule set for the folder at <paramref name="folderPath"/>.
        /// </summary>
        /// <param name="folderPath">The path of the file to change the right for.</param>
        /// <param name="rights">The right to deny.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="folderPath"/> is <c>null</c> 
        /// or empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when <paramref name="folderPath"/> 
        /// does not exist.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="rights"/> is 
        /// not supported to set on the folder.</exception>
        /// <exception cref="PathTooLongException">Thrown when <paramref name="folderPath"/> exceed the 
        /// system-defined maximum length.</exception>
        /// <exception cref="SecurityException">Thrown when the caller does not have the required permissions.</exception>
        public DirectoryPermissionsRevoker(string folderPath, FileSystemRights rights)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException(@"folderPath must have a valid value.", nameof(folderPath));
            }

            this.folderPath = folderPath;
            directoryInfo = new DirectoryInfo(Path.GetFullPath(folderPath));
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(@"folderPath does not exist.");
            }

            if ((rights ^ FileSystemRights.Synchronize) == 0)
            {
                // The FileSystemRights Synchronize by itself cannot be set.
                throw new NotSupportedException($"Setting the right {rights} is not supported.");
            }

            AddDenyDirectoryInfoRight(GetSupportedFileSystemRights(rights));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (Directory.Exists(folderPath))
            {
                foreach (FileSystemAccessRule appliedFileSystemAccessRule in appliedFileSystemAccessRules)
                {
                    TryRevertDenyDirectoryInfoRight(appliedFileSystemAccessRule);
                }
            }

            disposed = true;
        }

        private void AddDenyDirectoryInfoRight(FileSystemRights rights)
        {
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            if (IsFileSystemAccessRuleSet(rights, directorySecurity, AccessControlType.Deny))
            {
                return;
            }

            SecurityIdentifier sid = GetSecurityIdentifier();
            var fileSystemAccessRule = new FileSystemAccessRule(sid, rights, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                                                                PropagationFlags.None, AccessControlType.Deny);
            directorySecurity.AddAccessRule(fileSystemAccessRule);

            directoryInfo.SetAccessControl(directorySecurity);
            appliedFileSystemAccessRules.Add(fileSystemAccessRule);
        }

        private static bool IsFileSystemAccessRuleSet(FileSystemRights rights, CommonObjectSecurity commonObjectSecurity, AccessControlType accessControlType)
        {
            AuthorizationRuleCollection rules = commonObjectSecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
            return rules.OfType<FileSystemAccessRule>().Any(fs => fs.FileSystemRights.HasFlag(rights) && fs.AccessControlType == accessControlType);
        }

        private static FileSystemRights GetSupportedFileSystemRights(FileSystemRights rights)
        {
            return rights & ~FileSystemRights.Synchronize;
        }

        private void TryRevertDenyDirectoryInfoRight(FileSystemAccessRule rule)
        {
            try
            {
                DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

                directorySecurity.RemoveAccessRule(rule);

                directoryInfo.SetAccessControl(directorySecurity);
            }
            catch (SystemException)
            {
                // Ignored
            }
        }

        private static SecurityIdentifier GetSecurityIdentifier()
        {
            SecurityIdentifier id = WindowsIdentity.GetCurrent().User.AccountDomainSid;
            return new SecurityIdentifier(WellKnownSidType.WorldSid, id);
        }

        ~DirectoryPermissionsRevoker()
        {
            Dispose(false);
        }
    }
}