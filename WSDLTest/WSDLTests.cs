
using System;
using System.Configuration;
using System.IO;
using NUnit;
using NUnit.Framework;
using System.Linq;
using NUnit.Framework.Internal;
using WSDLTest.Holiday;

namespace WSDLTest
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    public class WsdlTests
    {
        public HolidayService2Soap Client;

        // Save info about each test
        public static string TestData = String.Empty;

        // Max time for each test
        public const int TestTimeout = 100000;

        //Path to log with results
        public static string FilePath = @"G:\Report";

        //Action that will be executed before each test
        [SetUp]
        public void SetUp()
        {
            Client = new HolidayService2SoapClient();

        }
        /// <summary>
        /// Get country using index and check description and coutry code
        /// </summary>
        /// <param name="countryCode"></param>
        /// <param name="expectedCode"></param>
        /// <param name="expectedDescription"></param>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.GetCountriesAvailableData)), Timeout(TestTimeout)]
        public void GetCountriesAvailable(int countryCode, string expectedCode, string expectedDescription)
        {
            var countries = Client.GetCountriesAvailable();
            var actualCode = countries[countryCode].Code;
            var actualDescription = countries[countryCode].Description;
            Assert.AreEqual(expectedCode, actualCode, "Country code doesnt match for index{0}", countryCode);
            Assert.AreEqual(expectedDescription, actualDescription, "Country description doesnt match for index{0}", countryCode);
        }
        /// <summary>
        /// Get holidays for country and check for count
        /// </summary>
        /// <param name="country"></param>
        /// <param name="expectedCount"></param>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.GetHolidaysAvailableData)), Timeout(TestTimeout)]
        public void GetHolidaysAvailable(Country country, int expectedCount)
        {
            var holidays = Client.GetHolidaysAvailable(country);
            Assert.AreEqual(holidays.Length, expectedCount);
        }
        /// <summary>
        /// Check if we can get date for every holiday in every country
        /// </summary>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.GetHolidayDateData)), Timeout(TestTimeout)]
        public void GetHolidayDate(Country country, string holidayCode, int year)
        {
            var date = new DateTime();
            try
            {
                date = Client.GetHolidayDate(country, holidayCode, year);
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("The holiday code provided was invalid"))
                    throw;
            }
            Assert.AreNotEqual(null, date);
        }
        /// <summary>
        /// Check GetHolidayDateE exceptions
        /// </summary>
        /// <param name="country"></param>
        /// <param name="holidayCode"></param>
        /// <param name="year"></param>
        /// <param name="exceptionMessage"></param>
        [TestCase(Country.Canada, "", 2017, "holiday code provided was invalid"), Timeout(TestTimeout)]
        [TestCase(Country.Canada, "FLAG-DAY", 1699, "The year provided was invalid")]
        [TestCase(null, "FLAG-DAY", 2017, "The year provided was invalid")]
        public void GetHolidayDateE(Country country, string holidayCode, int year, string exceptionMessage)
        {
            try
            {
                var date = Client.GetHolidayDate(country, holidayCode, year);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains(exceptionMessage));
            }
            Assert.Fail("Exception expected!");
        }

        /// <summary>
        /// Check GetHolidaysForDateRange exceptions
        /// </summary>
        /// <param name="country"></param>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.GetHolidaysForDateRangeE)), Timeout(TestTimeout)]
        public void GetHolidaysForDateRangeE(Country country, DateTime starTime, DateTime endTime)
        {
            try
            {
                var holidays = Client.GetHolidaysForDateRange(country, starTime, endTime);
            }
            catch (Exception)
            {
                return; }
            Assert.Fail("Exception expected");

        }

        /// <summary>
        /// Check GetHolidaysForDateRange comparing to GetHolidaysForDateRange
        /// </summary>
        /// <param name="country"></param>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.CountryList)), Timeout(TestTimeout)]
        public void GetHolidaysForDateRange(Country country)
        {
            var rangeHol=Client.GetHolidaysForDateRange(country, new DateTime(2017, 1, 1), new DateTime(2018, 1, 1));
            var yearHol=Client.GetHolidaysForYear(country, 2017);
            Assert.AreNotEqual(rangeHol,yearHol);
        }

        /// <summary>
        /// Check GetHolidaysForMonth comparing to GetHolidaysForYear
        /// </summary>
        /// <param name="country"></param>
        [TestCaseSource(typeof(DataSource), nameof(DataSource.CountryList)), Timeout(TestTimeout)]
        public void GetHolidaysForMonth(Country country)
        {
            var holidays = Client.GetHolidaysForMonth(country, 2017, 1).ToList();
            for (int i = 2; i <= 12; i++)
            {
                var holidays2=Client.GetHolidaysForMonth(country, 2017, i);
                foreach (var holiday in holidays2)
                {
                    holidays.Add(holiday);
                }
            }
            var year=Client.GetHolidaysForYear(country, 2017).ToList();
            Assert.IsTrue(holidays.Count.Equals(year.Count));
            int j = 0; // for better debuggin
            foreach (var holiday in holidays)
            {
                Assert.AreEqual(year[j].Descriptor, holidays[j].Descriptor);
                j++;
            }
        }

        //Action that will be executed after each test
        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.FailCount == 0)
            {
                TestData += TestContext.CurrentContext.Test.Name + " end with result Pass" + System.Environment.NewLine;
            }
            else
            {
                TestData += TestContext.CurrentContext.Test.Name + " end with result Fail"
                    + Environment.NewLine + "With Message " + TestContext.CurrentContext.Result.Message + Environment.NewLine + "Call stack: " + TestContext.CurrentContext.Result.StackTrace + System.Environment.NewLine;
            }
        }

        //Action that will be executed after all test
        [OneTimeTearDown]
        public void FinalTearDown()
        {
            var time = DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" +
                       DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second;
            time += ".txt";
            var filepath = Path.Combine(FilePath, time);
            var file = File.CreateText(filepath);
            file.WriteLine(TestData);
            file.Close();
        }
    }

}
