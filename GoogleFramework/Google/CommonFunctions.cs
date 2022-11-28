﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Linq;
using System.Diagnostics;
using System.Linq;
using Login;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace GoogleFramework
{
    public class CommonFunctions    {
        private static readonly log4net.ILog logger = log4net.LogManager.
            GetLogger(type: MethodBase.GetCurrentMethod()!.DeclaringType);
        internal static readonly CommonFunctions commonFunctions = new();
        public CommonFunctions common = commonFunctions;

        public TestContext? TestContext { get; set; }

        public static void Login(GoogleLogin.Sites site) => GoogleLogin.Go(site, Driver.Instance!);

        public static void LogInfo(string message) => logger.Info(string.Format(message));


        public static void LogWarn(string message) => logger.Warn(string.Format(message));


        public static void LogError(string message) => logger.Error(string.Format(message));

        public static void GoToTab(int tab) =>
            Driver.Instance?.SwitchTo().Window(Driver.Instance.WindowHandles[tab]);

        public static void CloseTab(int tab) =>
            Driver.Instance?.SwitchTo().Window(Driver.Instance.WindowHandles[tab]).Close();

        public static void GoToPage(string url)=> Driver.Instance!.Navigate().GoToUrl(url);

        public static void Delay(int miliSeconds)
        {
            Thread.Sleep(miliSeconds);
            logger.Info(string.Format("Delay miliseconds: " + miliSeconds.ToString()));
        }

        public static IWebElement FindElement(By by)
        {
            IWebElement? findElement = null;
            try
            {
                findElement = Driver.Instance!.FindElement(by);
                logger.Info(string.Format("Element found, Tag: " + findElement.TagName+ " Element: "+findElement.Text));

            }
            catch (Exception e)
            {
                logger.Error(string.Format("Error to find element: " + e.Message.ToString()));
            }
            Delay(500);

            return findElement!;
        }

        public static ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            ReadOnlyCollection<IWebElement>? findElements = null;
            try
            {
                findElements = Driver.Instance!.FindElements(by);
                logger.Info(string.Format("Elements found: " + findElements!.Count.ToString()));

            }
            catch (Exception e)
            {
                logger.Error(string.Format("Error: " + e.Message.ToString()));
            }
            Delay(500);

            return findElements!;
        }

        public static void TakeScreenshot(string testName)
        {
            ITakesScreenshot? instance = Driver.Instance as ITakesScreenshot;
            var screenshot = instance!.GetScreenshot();
            var fileName = $"{"sShot-"}{testName}{"_"}{DateTime.Now.Ticks}{".jpg"}";
            var path = "C:\\Temp\\" + fileName;
            screenshot.SaveAsFile(path, ScreenshotImageFormat.Jpeg);
            logger.Info(string.Format("ScreenShot taken: " + path));
        }

        public static void SendKey(By by, string text)
        {
            IWebElement element = WaitElementPresent(by);
            element.SendKeys(text);
            Delay(1000);
            logger.Info(string.Format("Sendkey: " + text + " to element: " + element.Text.ToString()));
        }
        public static void SendKeyAndEnter(By by, string text)
        {
            CommonFunctions.Delay(1000);
            IWebElement element = WaitElementPresent(by);
            element.SendKeys(text);
            element.SendKeys(Keys.Return);
            Delay(1000);
            logger.Info(string.Format("Sendkey: " + text + " to element: " + element.Text.ToString()));
        }

        public static IWebElement WaitElementPresent(By by, int iTimeout = 10)
        {
            WebDriverWait wait = new(Driver.Instance, TimeSpan.FromSeconds(iTimeout));
            IWebElement element = wait.Until(e => e.FindElement(by));

            return element;
        }

        public static void Click(By by)
        {
            try
            {
                int elem = 1;
                ReadOnlyCollection<IWebElement> findElements = Driver.Instance!.FindElements(by);
                foreach (IWebElement element in findElements)
                {
                    logger.Info(String.Format("Total element: "+ findElements.Count.ToString()+" of "+elem.ToString()));
                    if (element.Displayed)
                    {
                        logger.Info(string.Format("Element found, Tag: " + element.TagName + " Element: " + element.Text));
                        Actions builder = new(Driver.Instance);
                        logger.Info(string.Format("Click element: " + element.Text));
                        builder.MoveToElement(Driver.Instance!.FindElement(by)).Click().Perform();
                    }
                    elem++;
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Error: " + e.Message.ToString()));
            }
            Delay(700);
        }

        public static void Clear_TextElement(By by)
        {
            IWebElement element = WaitElementPresent(by);
            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
            Delay(1000);
        }
    }
}
