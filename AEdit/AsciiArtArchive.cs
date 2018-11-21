using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace AEdit
{
	////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>	Handler for the ASCII art archive. </summary>
	///
	/// <remarks>	This pulls in ascii drawings and categorized
	/// 			from the web.
	/// 			Darrell Plank, 11/21/2018. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public class AsciiArtArchive
	{
		#region Private variables
		private const string AaaPath = "https://www.asciiart.eu/";
		private List<Hlink> _topLevel;
		private bool LinkFailed { get; set; }
		#endregion

		#region Internal classes
		private class Hlink
		{
			public Hlink(string name, Uri link)
			{
				Name = name;
				Link = link;
				Subtree = null;
			}

			private string Name { get; }
			public Uri Link { get; }
			public List<Hlink> Subtree { get; private set; }

			public void SetSubtree(List<Hlink> subtree)
			{
				Subtree = subtree;
			}
		}
		#endregion

		#region Constructor
		public AsciiArtArchive()
		{
			GetTree();
		}
		#endregion

		#region Web Scraping
		private void GetTree()
		{
			var topLevelTask = new System.Net.WebClient().DownloadStringTaskAsync(new Uri(AaaPath));
			topLevelTask.ContinueWith(_ =>
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(topLevelTask.Result);
				_topLevel = new List<Hlink>();
				GetSubjectLinks(doc, _topLevel);
				foreach (var hlink in _topLevel)
				{
					hlink.SetSubtree(new List<Hlink>());
					GetSubTree(hlink.Link, hlink.Subtree);
				}
			});
		}

		private void GetSubTree(Uri uri, List<Hlink> output)
		{
			var task = new System.Net.WebClient().DownloadStringTaskAsync(uri);
			task.ContinueWith(_ =>
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(task.Result);
				GetSubjectLinks(doc, output);
			});
		}

		private void GetSubjectLinks(HtmlDocument doc, List<Hlink> output)
		{
			output.Clear();
			try
			{
				output.AddRange(
					doc.DocumentNode.
					Descendants("div").
					Where(x =>
					{
						var v = x.Attributes["class"];
						return v != null && v.Value == "directory-columns";
					}).
					First().
					Descendants("a").
					Select(n => new Hlink(
						n.InnerHtml,
						new Uri(AaaPath + n.Attributes["href"].Value))));
			}
			catch (Exception)
			{
				LinkFailed = true;
			}
		}

		public void ScrapeArt(int iTop, int iSecond, List<string> art, Action<List<string>> finished = null)
		{
			Uri uri = _topLevel[iTop].Subtree[iSecond].Link;
			ScrapeArt(uri, art, finished);
		}

		private void ScrapeArt(Uri uri, List<string> art, Action<List<string>> finished = null)
		{
			var task = new System.Net.WebClient().DownloadStringTaskAsync(uri);
			task.ContinueWith(_ =>
			{
				var doc = new HtmlDocument();
				doc.LoadHtml(task.Result);
				GetArt(doc, art);
				finished?.Invoke(art);
			});
		}

		private void GetArt(HtmlDocument doc, List<string> art)
		{
			art.Clear();
			try
			{
				art.AddRange(
					doc.DocumentNode.
						Descendants("div").
						Where(x =>
						{
							var v = x.Attributes["class"];
							return v != null && v.Value == "asciiarts mt-3";
						}).
						First().
						Descendants("pre").
						Select(n => n.InnerText));
			}
			catch (Exception)
			{
				LinkFailed = true;
			}
		}
		#endregion
	}
}
