﻿using Squareface.Windows.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Standardseite" ist unter "http://go.microsoft.com/fwlink/?LinkID=390556" dokumentiert.

namespace Squareface.Windows
{
	/// <summary>
	/// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Frames navigiert werden kann.
	/// </summary>
	public sealed partial class AboutPage : Page
	{
		private NavigationHelper navigationHelper;
		private ObservableDictionary defaultViewModel = new ObservableDictionary();

		public AboutPage()
		{
			this.InitializeComponent();

			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
			this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
		}

		/// <summary>
		/// Ruft den <see cref="NavigationHelper"/> ab, der mit dieser <see cref="Page"/> verknüpft ist.
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		/// <summary>
		/// Ruft das Anzeigemodell für diese <see cref="Page"/> ab.
		/// Dies kann in ein stark typisiertes Anzeigemodell geändert werden.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get { return this.defaultViewModel; }
		}

		/// <summary>
		/// Füllt die Seite mit Inhalt auf, der bei der Navigation übergeben wird.  Gespeicherte Zustände werden ebenfalls
		/// bereitgestellt, wenn eine Seite aus einer vorherigen Sitzung neu erstellt wird.
		/// </summary>
		/// <param name="sender">
		/// Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Ereignisdaten, die die Navigationsparameter bereitstellen, die an
		/// <see cref="Frame.Navigate(Type, Object)"/> als diese Seite ursprünglich angefordert wurde und
		/// ein Wörterbuch des Zustands, der von dieser Seite während einer früheren
		/// beibehalten wurde.  Der Zustand ist beim ersten Aufrufen einer Seite NULL.</param>
		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
		}

		/// <summary>
		/// Behält den dieser Seite zugeordneten Zustand bei, wenn die Anwendung angehalten oder
		/// die Seite im Navigationscache verworfen wird.  Die Werte müssen den Serialisierungsanforderungen
		/// von <see cref="SuspensionManager.SessionState"/> entsprechen.
		/// </summary>
		/// <param name="sender">Die Quelle des Ereignisses, normalerweise <see cref="NavigationHelper"/></param>
		/// <param name="e">Ereignisdaten, die ein leeres Wörterbuch zum Auffüllen bereitstellen
		/// serialisierbarer Zustand.</param>
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		#region NavigationHelper-Registrierung

		/// <summary>
		/// Die in diesem Abschnitt bereitgestellten Methoden werden einfach verwendet, um
		/// damit NavigationHelper auf die Navigationsmethoden der Seite reagieren kann.
		/// <para>
		/// Platzieren Sie seitenspezifische Logik in Ereignishandlern für  
		/// <see cref="NavigationHelper.LoadState"/>
		/// und <see cref="NavigationHelper.SaveState"/>.
		/// Der Navigationsparameter ist in der LoadState-Methode verfügbar 
		/// zusätzlich zum Seitenzustand, der während einer früheren Sitzung beibehalten wurde.
		/// </para>
		/// </summary>
		/// <param name="e">Stellt Daten für Navigationsmethoden und -ereignisse bereit.
		/// Handler, bei denen die Navigationsanforderung nicht abgebrochen werden kann.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private async void SendFeedback_Click(object sender, RoutedEventArgs e)
		{
			//predefine Recipient
			EmailRecipient sendTo = new EmailRecipient()
			{
				Address = "hello@rootsoup.de"
			};

			//generate mail object
			EmailMessage mail = new EmailMessage();
			mail.Subject = "Feedback from Squareface App";
			//mail.Body = "this is the Body";

			//add recipients to the mail object
			mail.To.Add(sendTo);
			//mail.Bcc.Add(sendTo);
			//mail.CC.Add(sendTo);

			//open the share contract with Mail only:
			await EmailManager.ShowComposeNewEmailAsync(mail);
		}

		private async void RateApp_Click(object sender, RoutedEventArgs e)
		{
			//TODO
			//await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
		}
	}
}
