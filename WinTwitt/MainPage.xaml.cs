using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinTwitt.Utility;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace WinTwitt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
        private void btnAcquire_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("oauth_consumer_key", AuthConstants.ConsumerKey);
            parameters.Add("oauth_callback", "oob");
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", StringHelper.UNIXTimestamp);
            parameters.Add("oauth_nonce", Guid.NewGuid().ToString().Replace("-", ""));
            parameters.Add("oauth_version", "1.0");

            OAuthClient.PerformRequest(parameters, "http://api.twitter.com/oauth/request_token", AuthConstants.ConsumerSecret, string.Empty, OAuthClient.RequestType.InitialToken);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("oauth_consumer_key", AuthConstants.ConsumerKey);
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", StringHelper.UNIXTimestamp);
            parameters.Add("oauth_nonce", Guid.NewGuid().ToString().Replace("-", ""));
            parameters.Add("oauth_version", "1.0");
            parameters.Add("oauth_verifier", txtPIN.Text);
            parameters.Add("oauth_token", App.Token);

            OAuthClient.PerformRequest(parameters, "https://api.twitter.com/oauth/access_token", AuthConstants.ConsumerKey, App.Token, OAuthClient.RequestType.AccessToken);
        }

    }
}
