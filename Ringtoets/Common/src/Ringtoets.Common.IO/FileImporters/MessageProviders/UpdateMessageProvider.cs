// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Util.Extensions;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.FileImporters.MessageProviders
{
    /// <summary>
    /// This class provides the messages during the update actions of an importer.
    /// </summary>
    public class UpdateMessageProvider : IImporterMessageProvider
    {
        public string GetAddDataToModelProgressText()
        {
            return Resources.Importer_ProgressText_Updating_data_in_data_model;
        }

        public string GetCancelledLogMessageText(string typeDescriptor)
        {
            if (typeDescriptor == null)
            {
                throw new ArgumentNullException(nameof(typeDescriptor));
            }

            return string.Format(Resources.Importer_LogMessageText_Update_of_TypeDescriptor_0_cancelled_No_data_changed, typeDescriptor);
        }

        public string GetUpdateDataFailedLogMessageText(string typeDescriptor)
        {
            if (typeDescriptor == null)
            {
                throw new ArgumentNullException(nameof(typeDescriptor));
            }

            return string.Format(Resources.Importer_LogMessageText_Update_of_TypeDescriptor_0_failed_Reason__0__No_data_changed,
                                 typeDescriptor.FirstToLower());
        }
    }
}