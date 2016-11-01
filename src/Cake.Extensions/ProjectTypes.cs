// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 

namespace Cake.Extensions
{
    using System;

    [Flags]
    public enum ProjectType
    {
        AspNetMvc1,
        AspNetMvc2,
        AspNetMvc3,
        AspNetMvc4,
        AspNetMvc5,
        CPlusplus,
        CSharp,
        Database,
        DatabaseOther,
        DeploymentCab,
        DeploymentMergeModule,
        DeploymentSetup,
        DeploymentSmartDeviceCab,
        DistributedSystem,
        Dynamics2012AxCsharpInAot,
        FSharp,
        JSharp,
        Legacy2003SmartDeviceCSharp,
        Legacy2003SmartDeviceVbNet,
        ModelViewControllerV2Mvc2,
        ModelViewControllerV3Mvc3,
        ModelViewControllerV4Mvc4,
        ModelViewControllerV5Mvc5,
        MonoForAndroid,
        Monotouch,
        MonotouchBinding,
        PortableClassLibrary,
        ProjectFolders,
        SharepointCSharp,
        SharepointVbNet,
        SharepointWorkflow,
        Silverlight,
        SmartDeviceCSharp,
        SmartDeviceVbNet,
        SolutionFolder,
        Test,
        VbNet,
        VisualDatabaseTools,
        VisualStudioToolsForApplicationsVsta,
        VisualStudioToolsForOfficeVsto,
        WebApplication,
        WebSite,
        WindowsCSharp,
        WindowsCommunicationFoundation,
        WindowsPhone881AppCSharp,
        WindowsPhone881AppVbNet,
        WindowsPhone881BlankHubWebviewApp,
        WindowsPresentationFoundation,
        WindowsStoreMetroAppsComponents,
        WindowsVbNet,
        WindowsVisualCPlusplus,
        WorkflowCSharp,
        WorkflowFoundation,
        WorkflowVbNet,
        XamarinAndroid,
        XamarinIos,
        XnaWindows,
        XnaXbox,
        XnaZune,
        Undefined
    }

    public static class ProjectTypes
    {
        public const string AspNetMvc1 = "{603C0E0B-DB56-11DC-BE95-000D561079B0}";
        public const string AspNetMvc2 = "{F85E285D-A4E0-4152-9332-AB1D724D3325}";
        public const string AspNetMvc3 = "{E53F8FEA-EAE0-44A6-8774-FFD645390401}";
        public const string AspNetMvc4 = "{E3E379DF-F4C6-4180-9B81-6769533ABE47}";
        public const string AspNetMvc5 = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string CSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string CPlusplus = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        public const string Database = "{A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124}";
        public const string DatabaseOther = "{4F174C21-8C12-11D0-8340-0000F80270F8}";
        public const string DeploymentCab = "{3EA9E505-35AC-4774-B492-AD1749C4943A}";
        public const string DeploymentMergeModule = "{06A35CCD-C46D-44D5-987B-CF40FF872267}";
        public const string DeploymentSetup = "{978C614F-708E-4E1A-B201-565925725DBA}";
        public const string DeploymentSmartDeviceCab = "{AB322303-2255-48EF-A496-5904EB18DA55}";
        public const string DistributedSystem = "{F135691A-BF7E-435D-8960-F99683D2D49C}";
        public const string Dynamics2012AxCsharpInAot = "{BF6F8E12-879D-49E7-ADF0-5503146B24B8}";
        public const string FSharp = "{F2A71F9B-5D33-465A-A702-920D77279786}";
        public const string JSharp = "{E6FDF86B-F3D1-11D4-8576-0002A516ECE8}";
        public const string Legacy2003SmartDeviceCSharp = "{20D4826A-C6FA-45DB-90F4-C717570B9F32}";
        public const string Legacy2003SmartDeviceVbNet = "{CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8}";
        public const string ModelViewControllerV2Mvc2 = "{F85E285D-A4E0-4152-9332-AB1D724D3325}";
        public const string ModelViewControllerV3Mvc3 = "{E53F8FEA-EAE0-44A6-8774-FFD645390401}";
        public const string ModelViewControllerV4Mvc4 = "{E3E379DF-F4C6-4180-9B81-6769533ABE47}";
        public const string ModelViewControllerV5Mvc5 = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string MonoForAndroid = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
        public const string Monotouch = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";
        public const string MonotouchBinding = "{F5B4F3BC-B597-4E2B-B552-EF5D8A32436F}";
        public const string PortableClassLibrary = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
        public const string ProjectFolders = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
        public const string SharepointCSharp = "{593B0543-81F6-4436-BA1E-4747859CAAE2}";
        public const string SharepointVbNet = "{EC05E597-79D4-47f3-ADA0-324C4F7C7484}";
        public const string SharepointWorkflow = "{F8810EC1-6754-47FC-A15F-DFABD2E3FA90}";
        public const string Silverlight = "{A1591282-1198-4647-A2B1-27E5FF5F6F3B}";
        public const string SmartDeviceCSharp = "{4D628B5B-2FBC-4AA6-8C16-197242AEB884}";
        public const string SmartDeviceVbNet = "{68B1623D-7FB9-47D8-8664-7ECEA3297D4F}";
        public const string SolutionFolder = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";
        public const string Test = "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";
        public const string VbNet = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string VisualDatabaseTools = "{C252FEB5-A946-4202-B1D4-9916A0590387}";
        public const string VisualStudioToolsForApplicationsVsta = "{A860303F-1F3F-4691-B57E-529FC101A107}";
        public const string VisualStudioToolsForOfficeVsto = "{BAA0C2D2-18E2-41B9-852F-F413020CAA33}";
        public const string WebApplication = "{349C5851-65DF-11DA-9384-00065B846F21}";
        public const string WebSite = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string WindowsCSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        public const string WindowsVbNet = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
        public const string WindowsVisualCPlusplus = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
        public const string WindowsCommunicationFoundation = "{3D9AD99F-2412-4246-B90B-4EAA41C64699}";
        public const string WindowsPhone881BlankHubWebviewApp = "{76F1466A-8B6D-4E39-A767-685A06062A39}";
        public const string WindowsPhone881AppCSharp = "{C089C8C0-30E0-4E22-80C0-CE093F111A43}";
        public const string WindowsPhone881AppVbNet = "{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}";
        public const string WindowsPresentationFoundation = "{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}";
        public const string WindowsStoreMetroAppsComponents = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
        public const string WorkflowCSharp = "{14822709-B5A1-4724-98CA-57A101D1B079}";
        public const string WorkflowVbNet = "{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}";
        public const string WorkflowFoundation = "{32F31D43-81CC-4C15-9DE6-3FC5453562B6}";
        public const string XamarinAndroid = "{EFBA0AD7-5A72-4C68-AF49-83D382785DCF}";
        public const string XamarinIos = "{6BC8ED88-2882-458C-8E55-DFD12B67127B}";
        public const string XnaWindows = "{6D335F3A-9D43-41b4-9D22-F6F17C4BE596}";
        public const string XnaXbox = "{2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}";
        public const string XnaZune = "{D399B71A-8929-442a-A9AC-8BEC78BB2433}";
    }
}