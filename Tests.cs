using System;
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using AlzaProj.PageObjects;
using System.Drawing;
using System.IO;
using System.Text.Json;
using OpenQA.Selenium.Remote;

namespace AlzaProj
{
	public class Tests
	{		
		Dictionary<string, string> config = new Dictionary<string, string>();
		private StreamWriter LogFile;
		private IWebDriver driver;
		

		[SetUp]
		public void Init()
		{
			StreamReader r = new StreamReader(@"~..\..\..\..\..\config\config.json");
			string json = r.ReadToEnd();
			config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

			LogFile = new StreamWriter(@config["logPath"]);

			if (config["seleniumHubUrl"] != "")
			{
				var options = new ChromeOptions();
				options.PlatformName = "Windows 10";
				options.BrowserVersion = "84";
				driver = new RemoteWebDriver(new Uri(config["seleniumHubUrl"]), options);
			}
			else {
				driver = new ChromeDriver(@"C:\Users\mkrajcovicova\Downloads\chromedriver1");
			}
			

		}

		[Test]
		public void Test1()
		{
			try
			{
				LogFile.WriteLine("INFO: Career page initialization");
				CareerPage career = new CareerPage(driver);

				LogFile.WriteLine("INFO: Position page initialization");
				PositionPage position = new PositionPage(driver);

				LogFile.WriteLine("INFO: Expeted values definition");
				List<Dictionary<string, string>> expectedResults = new List<Dictionary<string, string>>();
				List<string> expectedResultsPositionDescription = new List<string>();
				Dictionary<string, string> expectedResult1 = new Dictionary<string, string>();
				Dictionary<string, string> expectedResult2 = new Dictionary<string, string>();

				expectedResult1.Add("Name", "Ciencialová Barbora");
				expectedResult1.Add("Description", "HR a Alza mě baví! A právě proto se vedle studia psychologie a managementu věnuji náboru pro naši IT větev. Volný čas ráda trávím aktivně, miluji pohyb všeho druhu, ale nejčastěji mně najdete běhat podél Vltavy nebo zašitou v posilovně se sluchátky na uších.");
				expectedResult1.Add("Picture", @"~..\..\..\..\..\src\pic1.jpg");
				expectedResults.Add(expectedResult1);

				expectedResult2.Add("Name", "Tomusko Ján");
				expectedResult2.Add("Description", "Celou moji profesní kariéru se věnuji problematice testování SW a HW a v šíření výhod včasné detekce chyb v produktu. Ve svém volném čase se věnuji turistice a lyžování.");
				expectedResult2.Add("Picture", @"~..\..\..\..\..\src\pic2.jpg");
				expectedResults.Add(expectedResult2);

				expectedResultsPositionDescription.Add("Už se v QA nějaký ten pátek pohybuješ a do toho máš za sebou i zkušenost manažerskou? Tak to jsi tady správně - hledáme nového teamleadera, který povede náš tým testerů starající se o náš web a mobilní aplikace. U náš v Alze se stále ještě setkáš s manuálním testováním, ale i automatizace si u náš získává stále více a více prostoru. Pokud Ti pojmy jako Selenium, Jira, Confluence nejsou neznámou, orientuješ se v C# a zaujali jsme Tě, pak pokračuj dále ve čtení!");
				expectedResultsPositionDescription.Add("Pokud se v QA už nějaký ten pátek pohybuješ a vzděláváš, určitě Ti neuniklo, že automatizované testování je velmi aktuálním tématem a Alza není žádnou výjimkou. Do našeho týmu hledáme další chytré hlavy, které ví, jak na to. Pokud máš již předešlé zkušenosti se Selenium Framework, orientuješ se v C#, Jira, Azure DevOps a nebo jsme zkrátka jen chytli Tvou pozornost, pak pokračuj dále ve čtení!");


				LogFile.WriteLine("INFO: Navigate to Career page and find all open QA position");
				career.GoToPage(config["sutUrl"]);
				career.SetVyhledatPoziciTextField("qa");
				career.RefreshComplete();

				LogFile.WriteLine("INFO: Verification that all QA positions contain same people with same description and same photo");
				List<string> QAposition = career.GetAllPosition();
				Assert.AreEqual(expectedResultsPositionDescription.Count, QAposition.Count);
				int counter = 0;

				foreach (var qa in QAposition)
				{
					driver.Navigate().GoToUrl(qa);
					position.LoadComplete();
					Assert.Contains(position.getPositionDescription(), expectedResultsPositionDescription);

					var people = position.getPeople();

					foreach (var p in people)
					{
						foreach (var er in expectedResults)
						{
							if (p["Name"] == er["Name"])
							{
								Assert.AreEqual(er["Description"], p["Description"]);
								Assert.AreEqual(position.GetHash(new Bitmap(er["Picture"])), position.parseImageURLAndDownload(p["Picture"], p["Name"] + counter));
							}
						}
					}
					driver.Navigate().Back();
					career.LoadComplete();
					counter++;

				}
			}
			catch (Exception e) {
					LogFile.WriteLine("ERROR: " + e);
				try
				{
					Screenshot image = ((ITakesScreenshot)driver).GetScreenshot();
					image.SaveAsFile(@"~..\..\..\..\..\Screenshot.png");
				}
				catch (Exception ex) {
					LogFile.WriteLine("ERROR: " + ex);
				}
			}
		}


			[TearDown]
			public void closeBrowser()
			{
				LogFile.Close();
				driver.Close();
			}
		}
	}

