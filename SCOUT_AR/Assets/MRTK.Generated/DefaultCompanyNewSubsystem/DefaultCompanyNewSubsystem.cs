// TODO: [Optional] Add copyright and license statement(s).

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.Scripting;

namespace DefaultCompany.MRTK3.Subsystems
{
    [Preserve]
    [MRTKSubsystem(
        Name = "defaultcompany.mrtk3.subsystems",
        DisplayName = "DefaultCompany NewSubsystem",
        Author = "DefaultCompany",
        ProviderType = typeof(DefaultCompanyNewSubsystemProvider),
        SubsystemTypeOverride = typeof(DefaultCompanyNewSubsystem),
        ConfigType = typeof(BaseSubsystemConfig))]
    public class DefaultCompanyNewSubsystem : NewSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            // Fetch subsystem metadata from the attribute.
            var cinfo = XRSubsystemHelpers.ConstructCinfo<DefaultCompanyNewSubsystem, NewSubsystemCinfo>();

            if (!DefaultCompanyNewSubsystem.Register(cinfo))
            {
                Debug.LogError($"Failed to register the {cinfo.Name} subsystem.");
            }
        }

        [Preserve]
        class DefaultCompanyNewSubsystemProvider : Provider
        {

            #region INewSubsystem implementation

            // TODO: Add the provider implementation.

            #endregion NewSubsystem implementation
        }
    }
}
