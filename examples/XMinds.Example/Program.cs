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

        const string AgeUserProp = "age";
        const string SubscriptionsUserProp = "subscriptions";
        const string ToBeDeletedUserProp = "to_be_deleted";

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

            await apiClient.CreateUserPropertyAsync(AgeUserProp, "uint8", false);
            await apiClient.CreateUserPropertyAsync(SubscriptionsUserProp, "unicode30", true);
            await apiClient.CreateUserPropertyAsync(ToBeDeletedUserProp, "float", false);

            var subsciptionsUserProperty = await apiClient.GetUserPropertyAsync(SubscriptionsUserProp);

            await apiClient.DeleteUserPropertyAsync(ToBeDeletedUserProp);

            var listAllUserPropertiesResult = await apiClient.ListAllUserPropertiesAsync();

            var user1 = new User(new Dictionary<string, object> 
            {
                { User.UserIdPropName, 1 },
                { AgeUserProp, 25 },
                { SubscriptionsUserProp, new List<string> { "chanel1", "chanel2" } }
            });

            await apiClient.CreateOrUpdateUserAsync(user1);

            var getUserResult = await apiClient.GetUserAsync(user1.UserId);

            user1[AgeUserProp] = 26;

            var user2 = new User(new Dictionary<string, object>
            {
                { User.UserIdPropName, 2 },
                { AgeUserProp, 45 },
                { SubscriptionsUserProp, new List<string> { "chanel1", "chanel3" } }
            });

            await apiClient.CreateOrUpdateUsersBulkAsync(new List<User> { user1, user2 });

            var listAllUsersBulkResult = await apiClient.ListAllUsersBulkAsync();

            var listUsersByIdsResult = await apiClient.ListUsersByIdsAsync(new List<object>() { user1.UserId, user2.UserId });

            await apiClient.DeleteCurrentDatabaseAsync();

            await apiClient.LoginRootAsync(XMindsApiRootEmail, XMindsApiRootPassword);

            await apiClient.DeleteIndividualAccountAsync(XMindsApiIndividualAccountEmail);

            await apiClient.DeleteServiceAccountAsync(XMindsApiServiceAccountName);

            //await apiClient.DeleteCurrentAccountAsync();

            apiClient.Dispose();
        }
    }
}
