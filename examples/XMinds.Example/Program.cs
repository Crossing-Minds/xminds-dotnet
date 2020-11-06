using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMinds.Example
{
    class Program
    {
        const string XMindsApiRootEmail = "your_root_email";
        const string XMindsApiRootPassword = "your_root_password";

        const string XMindsApiIndividualAccountEmail = "your_individual_account_email";
        const string XMindsApiIndividualAccountPassword = "your_individual_account_password";
        
        const string XMindsApiServiceAccountName = "your_service_account_name";
        const string XMindsApiServiceAccountPassword = "your_service_account_password";

        static void Main(string[] args)
        {
            RunExample().GetAwaiter().GetResult();
        }

        private static async Task RunExample()
        {
            var apiClient = new CrossingMindsApiClient();

            await apiClient.LoginRootAsync(XMindsApiRootEmail, XMindsApiRootPassword);

            var createdIndividualAccount = await apiClient.CreateIndividualAccountAsync(
                XMindsApiIndividualAccountEmail, XMindsApiIndividualAccountPassword, 
                Role.Manager, "John", "Doe");

            await apiClient.ResendVerificationCodeAsync(XMindsApiIndividualAccountEmail);

            try
            {
                await apiClient.VerifyAsync(XMindsApiIndividualAccountEmail, 
                    // Invalid verification code.
                    "00000000");
            }
            catch (AuthErrorException)
            {
                // This is expected as we provided invalid verification code.
            }

            var createdServiceAccount = await apiClient.CreateServiceAccountAsync(
                XMindsApiServiceAccountName, XMindsApiServiceAccountPassword,
                Role.Manager);

            var listAllAccountsResult = await apiClient.ListAllAccountsAsync();

            var createdDatabase = await apiClient.CreateDatabaseAsync(
                "ExampleDatabase", "Example Database created by .NET SDK Example",
                "uuid", "uint32");

            await apiClient.LoginServiceAsync(
                XMindsApiServiceAccountName, XMindsApiServiceAccountPassword, createdDatabase.Id);


            var allDatabases = await apiClient.ListAllDatabasesAsync(1, 10);

            var currentDatabase = await apiClient.GetCurrentDatabaseAsync();

            var currentDatabaseStatus = await apiClient.GetCurrentDatabaseStatusAsync();

            await apiClient.DeleteCurrentDatabaseAsync();


            await apiClient.LoginRootAsync(XMindsApiRootEmail, XMindsApiRootPassword);

            await apiClient.DeleteIndividualAccountAsync(XMindsApiIndividualAccountEmail);

            await apiClient.DeleteServiceAccountAsync(XMindsApiServiceAccountName);

            //await apiClient.DeleteCurrentAccountAsync();

            apiClient.Dispose();
        }
    }
}
