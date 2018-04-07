//https://msdn.microsoft.com/en-us/library/windows/desktop/aa372675%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396

namespace PowerStateManagement
{
    public enum InformationLevel
    {
        SystemPowerPolicyAc = 0,
        SystemPowerPolicyDc = 1,
        VerifySystemPolicyAc = 2,
        VerifySystemPolicyDc = 3,
        SystemPowerCapabilities = 4,
        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a SYSTEM_BATTERY_STATE structure containing information about the current system battery.
        /// </summary>
        SystemBatteryState = 5,
        SystemPowerPolicyCurrent = 8,
        AdministratorPowerPolicy = 9,
        SystemReserveHiberFile = 10,
        ProcessorInformation = 11,
        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a SYSTEM_POWER_INFORMATION structure.
        /// Applications can use this level to retrieve information about the idleness of the system.
        /// </summary>
        SystemPowerInformation = 12,
        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a ULONGLONG that specifies the interrupt-time count, in 100-nanosecond units, at the last system wake time.
        /// </summary>
        LastWakeTime = 14,
        /// <summary>
        /// The lpInBuffer parameter must be NULL; otherwise, the function returns ERROR_INVALID_PARAMETER.
        /// The lpOutputBuffer buffer receives a ULONGLONG that specifies the interrupt-time count, in 100-nanosecond units, at the last system sleep time.
        /// </summary>
        LastSleepTime = 15,
        SystemExecutionState = 16,
        ProcessorPowerPolicyAc = 18,
        ProcessorPowerPolicyDc = 19,
        VerifyProcessorPowerPolicyAc = 20,
        VerifyProcessorPowerPolicyDc = 21,
        ProcessorPowerPolicyCurrent = 22,
    }
}
