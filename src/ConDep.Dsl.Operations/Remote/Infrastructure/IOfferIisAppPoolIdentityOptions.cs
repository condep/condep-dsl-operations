namespace ConDep.Dsl
{
    public interface IOfferIisAppPoolIdentityOptions
    {
        /// <summary>
        /// Configures the application pool to run as the built-in account NetworkService.
        /// In IIS 7, this is the default (not IIS 7.5 and above, which has  ApplicationPoolIdentity
        /// as default).
        /// </summary>
        /// <returns></returns>
        IOfferIisAppPoolOptions NetworkService();

        /// <summary>
        /// Configures the application pool to run as the built-in account LocalService. 
        /// </summary>
        /// <returns></returns>
        IOfferIisAppPoolOptions LocalService();

        /// <summary>
        /// Configures the application pool to run as the built-in account LocalSystem. 
        /// </summary>
        /// <returns></returns>
        IOfferIisAppPoolOptions LocalSystem();

        /// <summary>
        /// Configures the application pool to run as the built-in account ApplicationPoolIdentity.
        /// In IIS 7.5 and above this is the default. 
        /// </summary>
        /// <returns></returns>
        IOfferIisAppPoolOptions ApplicationPoolIdentity();

        /// <summary>
        /// Configures the application pool to run as a custom account. Provide a local or domain user, 
        /// together with the password.
        /// </summary>
        /// <returns></returns>
        IOfferIisAppPoolOptions SpecificUser(string username, string password);
    }
}