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

        static void Main(string[] args)
        {
            RunExample().GetAwaiter().GetResult();
        }

        private static async Task RunExample()
        {
            var apiClient = new CrossingMindsApiClient();

            var loginRootResult = await apiClient.LoginRootAsync(XMindsApiRootEmail, XMindsApiRootPassword);

            var listAllAccountsResult = await apiClient.ListAllAccountsAsync();

            //var loginResult = await apiClient.LoginIndividualAsync(XMindsApiRootEmail, XMindsApiRootPassword, );

            apiClient.Dispose();
        }
    }
}
