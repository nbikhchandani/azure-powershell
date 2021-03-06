﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.Models;
using Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.ProviderModel;
using Microsoft.Azure.Commands.RecoveryServices.Backup.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets
{
    /// <summary>
    /// Get list of containers
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmRecoveryServicesBackupContainer"), 
    OutputType(typeof(ContainerBase), typeof(IList<ContainerBase>))]
    public class GetAzureRmRecoveryServicesBackupContainer : RecoveryServicesBackupCmdletBase
    {
        [Parameter(Mandatory = true, Position = 1, HelpMessage = ParamHelpMsgs.Container.ContainerType)]
        [ValidateNotNullOrEmpty]
        public ContainerType ContainerType { get; set; }

        [Parameter(Mandatory = false, Position = 2, 
            HelpMessage = ParamHelpMsgs.Container.BackupManagementType)]
        [ValidateNotNullOrEmpty]
        [ValidateSet("AzureVM", "MARS")]
        public string BackupManagementType { get; set; }

        [Parameter(Mandatory = false, Position = 3, 
            HelpMessage = ParamHelpMsgs.Container.Name)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = false, Position = 4, 
            HelpMessage = ParamHelpMsgs.Container.ResourceGroupName)]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(Mandatory = false, Position = 5, 
            HelpMessage = ParamHelpMsgs.Container.Status)]
        [ValidateNotNullOrEmpty]
        public ContainerRegistrationStatus Status { get; set; }

        public override void ExecuteCmdlet()
        {
            ExecutionBlock(() =>
            {
                base.ExecuteCmdlet();

                BackupManagementType? backupManagementTypeNullable = null;
                BackupManagementType backupManagementType;
                if (BackupManagementType != null)
                {
                    Enum.TryParse<BackupManagementType>(BackupManagementType, out backupManagementType);
                    backupManagementTypeNullable = backupManagementType;
                }

                PsBackupProviderManager providerManager = 
                    new PsBackupProviderManager(new Dictionary<System.Enum, object>()
                {  
                    {ContainerParams.ContainerType, ContainerType},
                    {ContainerParams.BackupManagementType, backupManagementTypeNullable},
                    {ContainerParams.Name, Name},
                    {ContainerParams.ResourceGroupName, ResourceGroupName},
                    {ContainerParams.Status, Status},
                }, ServiceClientAdapter);

                IPsBackupProvider psBackupProvider = 
                    providerManager.GetProviderInstance(ContainerType, backupManagementTypeNullable);
                var containerModels = psBackupProvider.ListProtectionContainers();
                WriteObject(containerModels, enumerateCollection: true);
            });
        }
    }
}
