using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using SeleniumExtras.PageObjects;
using System.Drawing;
using System.Net;

namespace AlzaProj.PageObjects
{
    public class PositionPage
    {
        private Dictionary<string, string> data;
        private IWebDriver driver;
        private int timeout = 15000;

     
        public PositionPage(IWebDriver driver) { 
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }

        /// <summary>
        /// Edit image to comparable shape
        /// </summary>
        /// <returns>Bitmap in comparable shape.</returns>
        public List<bool> GetHash(Bitmap bmpSource)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }

        /// <summary>
        /// Download image per its url
        /// </summary>
        /// <returns>Hash of image ready for comparision</returns>
        public List<bool> parseImageURLAndDownload(string url, string imageName) {
            int index = url.IndexOf(")");
            var finalURL = url.Remove(index - 1, url.Length - index + 1).Remove(0, 23);
            WebClient webClient = new WebClient();
            webClient.DownloadFile(finalURL, @"~..\..\..\..\..\download\" + imageName+".jpg");
            return GetHash(new Bitmap(@"~..\..\..\..\..\download\" + imageName+".jpg"));
            
        }

        /// <summary>
        /// Get all people with desctiption and photo from position page
        /// </summary>
        /// <returns>Return List of people with names, description and photo</returns>
        public List<Dictionary<string, string>> getPeople() {
            var QAPeople = driver.FindElements(By.XPath("/html/body/app-root/alza-router-outlet/career-masterpage/div/div/div/career-position-detail-page/div/job-people/div/div/div"));
            List<Dictionary<string, string>> qaPeople = new List<Dictionary<string, string>>();
            foreach (var p in QAPeople) {
                Dictionary<string, string> people = new Dictionary<string, string>();
                people.Add("Name", p.FindElement(By.ClassName("subtitle")).GetAttribute("innerText"));
                people.Add("Description", p.FindElement(By.ClassName("description")).GetAttribute("innerText"));
                people.Add("Picture", p.FindElement(By.ClassName("rounded")).GetAttribute("style"));
                qaPeople.Add(people);
            }
            return qaPeople;
        }

        public string getPositionDescription() {
            return driver.FindElement(By.XPath("/html/body/app-root/alza-router-outlet/career-masterpage/div/div/div/career-position-detail-page/div/div[1]/div/job-detail-item[1]/div/div/alza-article-body/div")).GetAttribute("innerText");
        }

        /// <summary>
        /// Async wait to ensure that page load is complete
        /// </summary>
        /// <returns>The PositionPage class instance.</returns>
        public PositionPage LoadComplete()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            // Wait for the page to load
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            return this;
        }       
    }
}
