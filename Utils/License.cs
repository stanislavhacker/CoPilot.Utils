using Microsoft.Phone.Marketplace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#if DEBUG
using MockIAPLib;
using Store = MockIAPLib;
using System.Xml.Linq;
using System.Reflection;
#else
using Windows.ApplicationModel.Store;
#endif

namespace CoPilot.Utils
{
    public class License
    {
        //privates
        private static ListingInformation listings;
        private static Boolean populated = false;

        //app ids
        private const String REMOVE_ADS_ID = "remove_adds";

        /// <summary>
        /// Is Addvertismets
        /// </summary>
        private static bool isAddvertismets = true;
        public static bool IsAddvertismets
        {
            get
            {
                return isAddvertismets;
            }
        }

        /// <summary>
        /// Resolve license
        /// </summary>
        public static void ResolveLicense() 
        {
            //SETUP MOCK
            SetupMockIAP();
            //DO FULLFILMENT
            DoFulfillment();
        }


        /// <summary>
        /// Buy IsAddvertismets
        /// </summary>
        /// <returns></returns>
        public static async Task BuyIsAddvertismets()
        {
            //load data
            await loadListingInformation();
            //request purchase
            try
            {
                await CurrentApp.RequestProductPurchaseAsync(REMOVE_ADS_ID, false);
                DoFulfillment();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// DoFulfillment
        /// </summary>
        private static void DoFulfillment()
        {
            //licenses
            var productLicenses = CurrentApp.LicenseInformation.ProductLicenses;

            //isAddvertismets
            if (productLicenses.ContainsKey(REMOVE_ADS_ID))
            {
                isAddvertismets = !productLicenses[REMOVE_ADS_ID].IsActive;
            }
            else
            {
                isAddvertismets = true;
            }
        }

        /// <summary>
        /// Load data
        /// </summary>
        /// <returns></returns>
        private static async Task loadListingInformation()
        {
            if (listings == null) {
                List<String> products = new List<string>();
                products.Add(REMOVE_ADS_ID);
                listings = await CurrentApp.LoadListingInformationByProductIdsAsync(products.ToArray());
            }
        }







        /// <summary>
        /// Mock up api
        /// </summary>
        private static void SetupMockIAP()
        {
#if DEBUG
            if (populated == false)
            {
                populated = true;
                //init
                MockIAP.Init();
                MockIAP.ClearCache();
                MockIAP.RunInMockMode(true);
                MockIAP.SetListingInformation(1, "en-us", "Removing ads in the application and support developer.", "1", "Remove Ads");

                // Add some more items manually.
                var data = Application.GetResourceStream(new Uri("IAPMock/mock.xml", UriKind.Relative));
                XDocument d = XDocument.Load(data.Stream);
                MockIAP.PopulateIAPItemsFromXml(d.ToString());
            }
#endif
        }

    }
}
