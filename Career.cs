using OpenQA.Selenium;
//using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using SeleniumExtras.PageObjects;
using System.ComponentModel.DataAnnotations;

namespace AlzaProj.PageObjects
{
    public class CareerPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        int timeout = 10000;

        public CareerPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            PageFactory.InitElements(driver, this);

        }

        [FindsBy(How = How.Id, Using = "position")]
        [CacheLookup]
        private IWebElement vyhledatPozici;

        [FindsBy(How = How.XPath, Using = "/html/body/app-root/alza-router-outlet/career-masterpage/div/div/div/career-landing-page/job-offer-list/div/div[1]/a")]
        [CacheLookup]
        private IList<IWebElement> pozice;


        /// <summary>
        /// Navigate to Career page
        /// </summary>
        /// <returns>The CareerPage class instance.</returns>
        public CareerPage GoToPage(string url)
        {
            driver.Navigate().GoToUrl(url);
            LoadComplete();
            return this;
        }

        /// <summary>
        /// Get Links to All Searched Position
        /// </summary>
        /// <returns>List of url links</returns>
        public List<string> GetAllPosition()
        {
            List<string> urls = new List<string>();
            foreach (var p in pozice)
            {
                urls.Add(p.GetAttribute("href"));
            }
            return urls;

        }


        /// <summary>
        /// Async wait to ensure that page load is complete
        /// </summary>
        /// <returns>The CareerPage class instance.</returns>
        public CareerPage LoadComplete()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            // Wait for the page to load
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            return this;
        }

        /// <summary>
        /// Async wait to ensure that page load is complete
        /// </summary>
        /// <returns>The CareerPage class instance.</returns>
        public CareerPage RefreshComplete()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            // Wait for the page to load
            wait.Until(d => (driver.Url).Equals("https://www.alza.cz/kariera?search=qa"));
            return this;
        }

        /// <summary>
        /// Set value to Vyhledat Pozici... Text field.
        /// </summary>
        /// <returns>The CareerPage class instance.</returns>
        public CareerPage SetVyhledatPoziciTextField(string vyhledatPoziciValue)
        {
            vyhledatPozici.SendKeys(vyhledatPoziciValue);
            return this;
        }

    }
}
