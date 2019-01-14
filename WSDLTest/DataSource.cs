using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using WSDLTest.Holiday;
using System.Web.Services;

namespace WSDLTest
{
    public class DataSource
    {
        public static IEnumerable GetHolidaysAvailableData
        {

            get
            {
                yield return new TestCaseData(Country.Canada, 48);
                yield return new TestCaseData(Country.GreatBritain, 42);
                yield return new TestCaseData(Country.IrelandNorthern, 44);
                yield return new TestCaseData(Country.IrelandRepublicOf, 36);
                yield return new TestCaseData(Country.Scotland, 44);
                yield return new TestCaseData(Country.UnitedStates, 34);
            }
        }

        public static IEnumerable GetHolidayDateData
        {
            get
            {
                var Client = new HolidayService2SoapClient();
                foreach (var country in Enum.GetValues(typeof(Country)).Cast<Country>())
                {
                    var holidays = Client.GetHolidaysAvailable(country);
                    foreach (var holiday in holidays)
                        yield return new TestCaseData(country, holiday.Code, 2017);
                }
            }
        }

        public static object[] GetCountriesAvailableData =
        {
            new object[] {0, "Canada", "Canada" },
            new object[] {1, "GreatBritain",  "Great Britain and Wales"},
            new object[] {2, "IrelandNorthern", "Northern Ireland"},
            new object[] {3, "IrelandRepublicOf", "Republic of Ireland" },
            new object[] {4, "Scotland", "Scotland"},
            new object[] {5, "UnitedStates", "United States"  },
        };


        public static IEnumerable GetHolidaysForDateRangeE
        {
            get
            {
                yield return new TestCaseData(Country.Canada, new DateTime(1700, 1, 1), new DateTime(2100, 1, 1));
                yield return new TestCaseData(Country.Canada, new DateTime(2018, 1, 1), new DateTime(2017, 1, 1));
                yield return new TestCaseData(null, new DateTime(2016, 1, 1), new DateTime(2017, 1, 1));
            }
        }

        public static IEnumerable CountryList
        {
            get
            {
                yield return new TestCaseData(Country.Canada);
                yield return new TestCaseData(Country.GreatBritain);
                yield return new TestCaseData(Country.IrelandNorthern);
                yield return new TestCaseData(Country.IrelandRepublicOf);
                yield return new TestCaseData(Country.Scotland);
                yield return new TestCaseData(Country.UnitedStates);
            }
        }
    }
}
