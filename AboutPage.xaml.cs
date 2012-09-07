using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace GoodYeyGenerator
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

			var app = ((App)App.Current);

			this.softwareNameBlock.Text = app.SoftwareName;
			this.versionBlock.Text = app.SoftwareVersion;
			
        }

		private void urlLink_Click(object sender, RoutedEventArgs e)
		{
			WebBrowserTask task = new WebBrowserTask()
			{
				Uri = new Uri(this.urlLink.Content as string)
			};

			task.Show();
		}

		private void mailLink_Click(object sender, RoutedEventArgs e)
		{
			EmailComposeTask task = new EmailComposeTask()
			{
				To = this.mailLink.Content as string,
				Subject = "[ShortcutDialer] "
			};

			task.Show();
		}

		private void reviewButton_Click(object sender, RoutedEventArgs e)
		{
			MarketplaceReviewTask marketplace = new MarketplaceReviewTask();
			marketplace.Show();
		}
    }
}
