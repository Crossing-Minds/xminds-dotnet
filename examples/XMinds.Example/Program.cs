using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMinds.Example
{

    /// <summary>
    /// The goal of this example is just to demonstrate XMinds SDK using. It does 
    /// not give live sample, just shows how to call SDK methods.
    /// IMPORTANT: If the example fails, the test accounts and test database 
    /// can be left in your account.
    /// </summary>
    class Program
    {
        ////////////////////////////////////////////////////////////////////
        // Put your creadentials to run the example. 
        // IMPORTANT: If the example fails, the test accounts and test database 
        // can be left in your account.
        ////////////////////////////////////////////////////////////////////

        const string XMindsApiRootEmail = "your_root_email";
        const string XMindsApiRootPassword = "your_root_password";

        const string XMindsApiIndividualAccountEmail = "your_individual_account_email";
        const string XMindsApiIndividualAccountPassword = "your_individual_account_password";

        const string XMindsApiServiceAccountName = "your_service_account_name";
        const string XMindsApiServiceAccountPassword = "your_service_account_password";

        ////////////////////////////////////////////////////////////////////

        const string AgeUserProp = "age";
        const string SubscriptionsUserProp = "subscriptions";
        const string ToBeDeletedUserProp = "to_be_deleted";

        const string PriceItemProp = "price";
        const string TagsItemProp = "tags";
        const string ToBeDeletedItemProp = "to_be_deleted";

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

            #region Users Data and Properties

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

            #endregion

            #region Items Data and Properties

            await apiClient.CreateItemPropertyAsync(PriceItemProp, "float32", false);
            await apiClient.CreateItemPropertyAsync(TagsItemProp, "unicode32", true);
            await apiClient.CreateItemPropertyAsync(ToBeDeletedItemProp, "float", false);

            var tagsItemProperty = await apiClient.GetItemPropertyAsync(TagsItemProp);

            await apiClient.DeleteItemPropertyAsync(ToBeDeletedItemProp);

            var listAllItemPropertiesResult = await apiClient.ListAllItemPropertiesAsync();

            var item1 = new Item(new Dictionary<string, object>
            {
                { Item.ItemIdPropName, Guid.NewGuid() },
                { PriceItemProp, 9.99 },
                { TagsItemProp, new List<string> { "family", "sci-fi" } }
            });

            await apiClient.CreateOrUpdateItemAsync(item1);

            var getItemResult = await apiClient.GetItemAsync(item1.ItemId);

            item1[PriceItemProp] = 10.99;

            var item2 = new Item(new Dictionary<string, object>
            {
                { Item.ItemIdPropName, Guid.NewGuid() },
                { PriceItemProp, 4.49 },
                { TagsItemProp, new List<string> { "family" } }
            });

            await apiClient.CreateOrUpdateItemsBulkAsync(new List<Item> { item1, item2 });

            var listAllItemsBulkResult = await apiClient.ListAllItemsBulkAsync();

            var listItemsByIdsResult = await apiClient.ListItemsByIdsAsync(new List<object>() { item1.ItemId, item2.ItemId });

            #endregion

            #region User Ratings

            await apiClient.CreateOrUpdateRatingAsync(user1.UserId, item1.ItemId, 5);
            await apiClient.CreateOrUpdateRatingAsync(user1.UserId, item2.ItemId, 7);
            await apiClient.CreateOrUpdateRatingAsync(user2.UserId, item1.ItemId, 8);
            await apiClient.CreateOrUpdateRatingAsync(user2.UserId, item2.ItemId, 9);
            await apiClient.CreateOrUpdateRatingAsync(user1.UserId, item1.ItemId, 6);

            var listAllRatingsBulk = await apiClient.ListAllRatingsBulkAsync();

            await apiClient.DeleteRatingAsync(user1.UserId, item1.ItemId);

            var listUserRatingsResult = await apiClient.ListUserRatingsAsync(user1.UserId);

            await apiClient.CreateOrUpdateUserRatingsBulkAsync(user1.UserId, new List<ItemRating>
            {
                new ItemRating(item1.ItemId, 3, DateTime.Now),
                new ItemRating(item2.ItemId, 4, DateTime.Now),
            }); 

            var listAllRatingsBulk2 = await apiClient.ListAllRatingsBulkAsync();

            await apiClient.DeleteRatingAsync(user1.UserId);

            var listAllRatingsBulk3 = await apiClient.ListAllRatingsBulkAsync();

            await apiClient.CreateOrUpdateRatingsBulkAsync(new List<UserItemRating>
            {
                new UserItemRating(user1.UserId, item1.ItemId, 3, DateTime.Now),
                new UserItemRating(user1.UserId, item2.ItemId, 4, DateTime.Now),
                new UserItemRating(user2.UserId, item2.ItemId, 1, DateTime.Now),
            });

            var listAllRatingsBulk4 = await apiClient.ListAllRatingsBulkAsync();

            await apiClient.CreateOrUpdateRatingsBulkAsync(new List<UserItemRating>
            {
                new UserItemRating(user1.UserId, item1.ItemId, 3, DateTime.Now),
                new UserItemRating(user1.UserId, item2.ItemId, 4, DateTime.Now),
                new UserItemRating(user2.UserId, item2.ItemId, 1, DateTime.Now),
            });

            #endregion

            #region Recommendation and Background Tasks

            var random = new Random();
            var items = new List<Item>();
            var itemsRatings = new List<ItemRating>();
            for (int i = 0; i < 20; ++i)
            {
                var item = new Item(new Dictionary<string, object>
                {
                    { Item.ItemIdPropName, Guid.NewGuid() },
                    { PriceItemProp, random.NextDouble() * 10.0 },
                    { TagsItemProp, new List<string> { "family" } }
                });
                items.Add(item);

                var itemRating = new ItemRating(item.ItemId, random.Next(1, 10), DateTime.Now);
                itemsRatings.Add(itemRating);
            }

            await apiClient.CreateOrUpdateItemsBulkAsync(items);
            await apiClient.CreateOrUpdateUserRatingsBulkAsync(user1.UserId, itemsRatings);

            await apiClient.TriggerBackgroundTaskAsync(BackgroundTaskName.MlModelRetrain);

            // Just required to wait while training task completes to be able to continue the example.
            await Task.Delay(30000);

            var listRecentBackgroundTasks = await apiClient.ListRecentBackgroundTasksAsync(BackgroundTaskName.MlModelRetrain);

            var filters = new List<string> { $"{PriceItemProp}:{RecoFilterOperator.Gt}:5" };

            var getRecoItemToItemsResult = await apiClient.GetRecoItemToItemsAsync(item1.ItemId, filters: filters);

            var getRecoSessionToItemsResult = await apiClient.GetRecoSessionToItemsAsync(filters: filters);

            var getRecoUserToItemsResult = await apiClient.GetRecoUserToItemsAsync(user1.UserId, filters: filters);

            #endregion

            await apiClient.DeleteCurrentDatabaseAsync();

            await apiClient.LoginRootAsync(XMindsApiRootEmail, XMindsApiRootPassword);

            await apiClient.DeleteIndividualAccountAsync(XMindsApiIndividualAccountEmail);

            await apiClient.DeleteServiceAccountAsync(XMindsApiServiceAccountName);

            //await apiClient.DeleteCurrentAccountAsync();

            apiClient.Dispose();
        }
    }
}
