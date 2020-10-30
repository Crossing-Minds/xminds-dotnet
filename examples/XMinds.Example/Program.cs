using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMinds.Example
{
    class Program
    {
        const string RootAccountEmail = "your_root_email";
        const string RootAccountPassword = "your_root_password";

        static void Main(string[] args)
        {
            RunExample().GetAwaiter().GetResult();
        }

        private static async Task RunExample()
        {
            var apiClient = new CrossingMindsApiClient();

            await apiClient.LoginRootAsync(RootAccountEmail, RootAccountPassword);

            apiClient.Dispose();
        }
    }
}
