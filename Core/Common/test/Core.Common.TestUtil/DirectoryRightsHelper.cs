// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using System.Security.AccessControl;
using System.Security.Principal;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Class for providing a safe way to manipulating directory permissions.
    /// </summary>
    public class DirectoryRightsHelper : IDisposable
    {
        private readonly IList<FileSystemAccessRule> appliedFileSystemAccessRules = new List<FileSystemAccessRule>();
        private readonly string filePath;
        private readonly DirectoryInfo directoryInfo;

        /// <summary>
        /// Creates an instance of <see cref="DirectoryRightsHelper"/>.
        /// Adds a <paramref name="rights"/> of type <see cref="AccessControlType.Deny"/> to the access
        /// rule set for the file at <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to change the right for.</param>
        /// <param name="rights">The right to deny.</param>
        public DirectoryRightsHelper(string filePath, FileSystemRights rights)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException(@"filePath must have a valid value.", "filePath");
            }

            this.filePath = filePath;
            directoryInfo = new DirectoryInfo(Path.GetFullPath(filePath));
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(@"filePath does not exist.");
            }
            AddDenyDirectoryInfoRight(rights);
        }

        public void Dispose()
        {
            if (!Directory.Exists(filePath))
            {
                return;
            }
            foreach (var appliedFileSystemAccessRule in appliedFileSystemAccessRules)
            {
                RevertDenyDirectoryInfoRight(appliedFileSystemAccessRule);
            }
        }

        private void AddDenyDirectoryInfoRight(FileSystemRights rights)
        {
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            if (IsFileSystemAccessRuleSet(rights, directorySecurity, AccessControlType.Deny))
            {
                return;
            }

            SecurityIdentifier sid = GetSecurityIdentifier();
            var fileSystemAccessRule = new FileSystemAccessRule(sid, rights, AccessControlType.Deny);
            directorySecurity.AddAccessRule(fileSystemAccessRule);

            directoryInfo.SetAccessControl(directorySecurity);
            appliedFileSystemAccessRules.Add(fileSystemAccessRule);
        }

        private static bool IsFileSystemAccessRuleSet(FileSystemRights rights, CommonObjectSecurity commonObjectSecurity, AccessControlType accessControlType)
        {
            AuthorizationRuleCollection rules = commonObjectSecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
            return rules.OfType<FileSystemAccessRule>().Any(fs => fs.FileSystemRights == rights && fs.AccessControlType == accessControlType);
        }

        private void RevertDenyDirectoryInfoRight(FileSystemAccessRule rule)
        {
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.RemoveAccessRule(rule);

            directoryInfo.SetAccessControl(directorySecurity);
        }

        private static SecurityIdentifier GetSecurityIdentifier()
        {
            SecurityIdentifier id = WindowsIdentity.GetCurrent().User.AccountDomainSid;
            return new SecurityIdentifier(WellKnownSidType.WorldSid, id);
        }
    }
}