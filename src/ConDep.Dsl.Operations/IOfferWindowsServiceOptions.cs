using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace ConDep.Dsl
{
    public interface IOfferWindowsServiceOptions
    {
        /// <summary>
        /// To run the Windows Service under a custom account, provide domain\user or .\user for local user. For built-in accounts like NetworkService, use .\NetworkService. If no user is provided, LocalSystem will be used. 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions UserName(string username);

        /// <summary>
        /// Password for the custom account if UserName is set.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions Password(string password);

        /// <summary>
        /// A description for the Windows Service.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions Description(string description);

        /// <summary>
        /// A service group for the Windows Service. Windows Services within the same Service Group and auto start set, will start together when a computer starts up. There are also other reasons for using Service Groups.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions ServiceGroup(string group);
        
        /// <summary>
        /// Provide any parameters you want to be sent in to the Windows Service executable on startup. 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions ExeParams(string parameters);

        /// <summary>
        /// During installation or removal, will ignore any errors during start/stop of the Windows Service.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions IgnoreFailureOnServiceStartStop(bool value);

        /// <summary>
        /// Defines the startup type for the Windows Service.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions StartupType(ServiceStartMode type);

        /// <summary>
        /// If used, will not start the Windows Service after installation.
        /// </summary>
        /// <returns></returns>
        IOfferWindowsServiceOptions DoNotStartAfterInstall();

        /// <summary>
        /// Defines actions for first, second and subsequent failures of the Windows Service.
        /// </summary>
        /// <param name="serviceFailureResetInterval"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        IOfferWindowsServiceOptions OnServiceFailure(int serviceFailureResetInterval, Action<IOfferWindowsServiceFailureOptions> options);

        /// <summary>
        /// Timeout for how long ConDep will wait on start or stop for the Windows Service.
        /// </summary>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns></returns>
        IOfferWindowsServiceOptions TimeoutInSeconds(int timeout);

        /// <summary>
        /// List of other services that this Windows Service depend on.
        /// </summary>
        /// <param name="dependencies">List of service names</param>
        /// <returns></returns>
        IOfferWindowsServiceOptions Dependencies(List<string> dependencies);

        /// <summary>
        /// If the <see cref="StartupType"/> is set to <see cref="ServiceStartMode.Automatic"/>, this method will enable delayed start for the Windows Service.
        /// </summary>
        /// <returns></returns>
        IOfferWindowsServiceOptions EnableDelayedAutoStart();
    }
}