namespace More.Globalization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public partial class FourFourFiveCalendarTest
    {
        static XDocument CreateTestData()
        {
            return XDocument.Parse(
                "<weeks>" +
                    "<week year='2008' month='1' start='06/30/2007' end='07/06/2007' />" +
                    "<week year='2008' month='1' start='07/07/2007' end='07/13/2007' />" +
                    "<week year='2008' month='1' start='07/14/2007' end='07/20/2007' />" +
                    "<week year='2008' month='1' start='07/21/2007' end='07/27/2007' />" +
                    "<week year='2008' month='2' start='07/28/2007' end='08/03/2007' />" +
                    "<week year='2008' month='2' start='08/04/2007' end='08/10/2007' />" +
                    "<week year='2008' month='2' start='08/11/2007' end='08/17/2007' />" +
                    "<week year='2008' month='2' start='08/18/2007' end='08/24/2007' />" +
                    "<week year='2008' month='3' start='08/25/2007' end='08/31/2007' />" +
                    "<week year='2008' month='3' start='09/01/2007' end='09/07/2007' />" +
                    "<week year='2008' month='3' start='09/08/2007' end='09/14/2007' />" +
                    "<week year='2008' month='3' start='09/15/2007' end='09/21/2007' />" +
                    "<week year='2008' month='3' start='09/22/2007' end='09/28/2007' />" +
                    "<week year='2008' month='4' start='09/29/2007' end='10/05/2007' />" +
                    "<week year='2008' month='4' start='10/06/2007' end='10/12/2007' />" +
                    "<week year='2008' month='4' start='10/13/2007' end='10/19/2007' />" +
                    "<week year='2008' month='4' start='10/20/2007' end='10/26/2007' />" +
                    "<week year='2008' month='5' start='10/27/2007' end='11/02/2007' />" +
                    "<week year='2008' month='5' start='11/03/2007' end='11/09/2007' />" +
                    "<week year='2008' month='5' start='11/10/2007' end='11/16/2007' />" +
                    "<week year='2008' month='5' start='11/17/2007' end='11/23/2007' />" +
                    "<week year='2008' month='6' start='11/24/2007' end='11/30/2007' />" +
                    "<week year='2008' month='6' start='12/01/2007' end='12/07/2007' />" +
                    "<week year='2008' month='6' start='12/08/2007' end='12/14/2007' />" +
                    "<week year='2008' month='6' start='12/15/2007' end='12/21/2007' />" +
                    "<week year='2008' month='6' start='12/22/2007' end='12/28/2007' />" +
                    "<week year='2008' month='7' start='12/29/2007' end='01/04/2008' />" +
                    "<week year='2008' month='7' start='01/05/2008' end='01/11/2008' />" +
                    "<week year='2008' month='7' start='01/12/2008' end='01/18/2008' />" +
                    "<week year='2008' month='7' start='01/19/2008' end='01/25/2008' />" +
                    "<week year='2008' month='8' start='01/26/2008' end='02/01/2008' />" +
                    "<week year='2008' month='8' start='02/02/2008' end='02/08/2008' />" +
                    "<week year='2008' month='8' start='02/09/2008' end='02/15/2008' />" +
                    "<week year='2008' month='8' start='02/16/2008' end='02/22/2008' />" +
                    "<week year='2008' month='9' start='02/23/2008' end='02/29/2008' />" +
                    "<week year='2008' month='9' start='03/01/2008' end='03/07/2008' />" +
                    "<week year='2008' month='9' start='03/08/2008' end='03/14/2008' />" +
                    "<week year='2008' month='9' start='03/15/2008' end='03/21/2008' />" +
                    "<week year='2008' month='9' start='03/22/2008' end='03/28/2008' />" +
                    "<week year='2008' month='10' start='03/29/2008' end='04/04/2008' />" +
                    "<week year='2008' month='10' start='04/05/2008' end='04/11/2008' />" +
                    "<week year='2008' month='10' start='04/12/2008' end='04/18/2008' />" +
                    "<week year='2008' month='10' start='04/19/2008' end='04/25/2008' />" +
                    "<week year='2008' month='11' start='04/26/2008' end='05/02/2008' />" +
                    "<week year='2008' month='11' start='05/03/2008' end='05/09/2008' />" +
                    "<week year='2008' month='11' start='05/10/2008' end='05/16/2008' />" +
                    "<week year='2008' month='11' start='05/17/2008' end='05/23/2008' />" +
                    "<week year='2008' month='12' start='05/24/2008' end='05/30/2008' />" +
                    "<week year='2008' month='12' start='05/31/2008' end='06/06/2008' />" +
                    "<week year='2008' month='12' start='06/07/2008' end='06/13/2008' />" +
                    "<week year='2008' month='12' start='06/14/2008' end='06/20/2008' />" +
                    "<week year='2008' month='12' start='06/21/2008' end='06/27/2008' />" +
                    "<week year='2009' month='1' start='06/28/2008' end='07/04/2008' />" +
                    "<week year='2009' month='1' start='07/05/2008' end='07/11/2008' />" +
                    "<week year='2009' month='1' start='07/12/2008' end='07/18/2008' />" +
                    "<week year='2009' month='1' start='07/19/2008' end='07/25/2008' />" +
                    "<week year='2009' month='1' start='07/26/2008' end='08/01/2008' />" +
                    "<week year='2009' month='2' start='08/02/2008' end='08/08/2008' />" +
                    "<week year='2009' month='2' start='08/09/2008' end='08/15/2008' />" +
                    "<week year='2009' month='2' start='08/16/2008' end='08/22/2008' />" +
                    "<week year='2009' month='2' start='08/23/2008' end='08/29/2008' />" +
                    "<week year='2009' month='3' start='08/30/2008' end='09/05/2008' />" +
                    "<week year='2009' month='3' start='09/06/2008' end='09/12/2008' />" +
                    "<week year='2009' month='3' start='09/13/2008' end='09/19/2008' />" +
                    "<week year='2009' month='3' start='09/20/2008' end='09/26/2008' />" +
                    "<week year='2009' month='4' start='09/27/2008' end='10/03/2008' />" +
                    "<week year='2009' month='4' start='10/04/2008' end='10/10/2008' />" +
                    "<week year='2009' month='4' start='10/11/2008' end='10/17/2008' />" +
                    "<week year='2009' month='4' start='10/18/2008' end='10/24/2008' />" +
                    "<week year='2009' month='4' start='10/25/2008' end='10/31/2008' />" +
                    "<week year='2009' month='5' start='11/01/2008' end='11/07/2008' />" +
                    "<week year='2009' month='5' start='11/08/2008' end='11/14/2008' />" +
                    "<week year='2009' month='5' start='11/15/2008' end='11/21/2008' />" +
                    "<week year='2009' month='5' start='11/22/2008' end='11/28/2008' />" +
                    "<week year='2009' month='6' start='11/29/2008' end='12/05/2008' />" +
                    "<week year='2009' month='6' start='12/06/2008' end='12/12/2008' />" +
                    "<week year='2009' month='6' start='12/13/2008' end='12/19/2008' />" +
                    "<week year='2009' month='6' start='12/20/2008' end='12/26/2008' />" +
                    "<week year='2009' month='7' start='12/27/2008' end='01/02/2009' />" +
                    "<week year='2009' month='7' start='01/03/2009' end='01/09/2009' />" +
                    "<week year='2009' month='7' start='01/10/2009' end='01/16/2009' />" +
                    "<week year='2009' month='7' start='01/17/2009' end='01/23/2009' />" +
                    "<week year='2009' month='7' start='01/24/2009' end='01/30/2009' />" +
                    "<week year='2009' month='8' start='01/31/2009' end='02/06/2009' />" +
                    "<week year='2009' month='8' start='02/07/2009' end='02/13/2009' />" +
                    "<week year='2009' month='8' start='02/14/2009' end='02/20/2009' />" +
                    "<week year='2009' month='8' start='02/21/2009' end='02/27/2009' />" +
                    "<week year='2009' month='9' start='02/28/2009' end='03/06/2009' />" +
                    "<week year='2009' month='9' start='03/07/2009' end='03/13/2009' />" +
                    "<week year='2009' month='9' start='03/14/2009' end='03/20/2009' />" +
                    "<week year='2009' month='9' start='03/21/2009' end='03/27/2009' />" +
                    "<week year='2009' month='10' start='03/28/2009' end='04/03/2009' />" +
                    "<week year='2009' month='10' start='04/04/2009' end='04/10/2009' />" +
                    "<week year='2009' month='10' start='04/11/2009' end='04/17/2009' />" +
                    "<week year='2009' month='10' start='04/18/2009' end='04/24/2009' />" +
                    "<week year='2009' month='10' start='04/25/2009' end='05/01/2009' />" +
                    "<week year='2009' month='11' start='05/02/2009' end='05/08/2009' />" +
                    "<week year='2009' month='11' start='05/09/2009' end='05/15/2009' />" +
                    "<week year='2009' month='11' start='05/16/2009' end='05/22/2009' />" +
                    "<week year='2009' month='11' start='05/23/2009' end='05/29/2009' />" +
                    "<week year='2009' month='12' start='05/30/2009' end='06/05/2009' />" +
                    "<week year='2009' month='12' start='06/06/2009' end='06/12/2009' />" +
                    "<week year='2009' month='12' start='06/13/2009' end='06/19/2009' />" +
                    "<week year='2009' month='12' start='06/20/2009' end='06/26/2009' />" +
                    "<week year='2009' month='12' start='06/27/2009' end='07/03/2009' />" +
                    "<week year='2010' month='1' start='07/04/2009' end='07/10/2009' />" +
                    "<week year='2010' month='1' start='07/11/2009' end='07/17/2009' />" +
                    "<week year='2010' month='1' start='07/18/2009' end='07/24/2009' />" +
                    "<week year='2010' month='1' start='07/25/2009' end='07/31/2009' />" +
                    "<week year='2010' month='2' start='08/01/2009' end='08/07/2009' />" +
                    "<week year='2010' month='2' start='08/08/2009' end='08/14/2009' />" +
                    "<week year='2010' month='2' start='08/15/2009' end='08/21/2009' />" +
                    "<week year='2010' month='2' start='08/22/2009' end='08/28/2009' />" +
                    "<week year='2010' month='3' start='08/29/2009' end='09/04/2009' />" +
                    "<week year='2010' month='3' start='09/05/2009' end='09/11/2009' />" +
                    "<week year='2010' month='3' start='09/12/2009' end='09/18/2009' />" +
                    "<week year='2010' month='3' start='09/19/2009' end='09/25/2009' />" +
                    "<week year='2010' month='3' start='09/26/2009' end='10/02/2009' />" +
                    "<week year='2010' month='4' start='10/03/2009' end='10/09/2009' />" +
                    "<week year='2010' month='4' start='10/10/2009' end='10/16/2009' />" +
                    "<week year='2010' month='4' start='10/17/2009' end='10/23/2009' />" +
                    "<week year='2010' month='4' start='10/24/2009' end='10/30/2009' />" +
                    "<week year='2010' month='5' start='10/31/2009' end='11/06/2009' />" +
                    "<week year='2010' month='5' start='11/07/2009' end='11/13/2009' />" +
                    "<week year='2010' month='5' start='11/14/2009' end='11/20/2009' />" +
                    "<week year='2010' month='5' start='11/21/2009' end='11/27/2009' />" +
                    "<week year='2010' month='6' start='11/28/2009' end='12/04/2009' />" +
                    "<week year='2010' month='6' start='12/05/2009' end='12/11/2009' />" +
                    "<week year='2010' month='6' start='12/12/2009' end='12/18/2009' />" +
                    "<week year='2010' month='6' start='12/19/2009' end='12/25/2009' />" +
                    "<week year='2010' month='6' start='12/26/2009' end='01/01/2010' />" +
                    "<week year='2010' month='7' start='01/02/2010' end='01/08/2010' />" +
                    "<week year='2010' month='7' start='01/09/2010' end='01/15/2010' />" +
                    "<week year='2010' month='7' start='01/16/2010' end='01/22/2010' />" +
                    "<week year='2010' month='7' start='01/23/2010' end='01/29/2010' />" +
                    "<week year='2010' month='8' start='01/30/2010' end='02/05/2010' />" +
                    "<week year='2010' month='8' start='02/06/2010' end='02/12/2010' />" +
                    "<week year='2010' month='8' start='02/13/2010' end='02/19/2010' />" +
                    "<week year='2010' month='8' start='02/20/2010' end='02/26/2010' />" +
                    "<week year='2010' month='9' start='02/27/2010' end='03/05/2010' />" +
                    "<week year='2010' month='9' start='03/06/2010' end='03/12/2010' />" +
                    "<week year='2010' month='9' start='03/13/2010' end='03/19/2010' />" +
                    "<week year='2010' month='9' start='03/20/2010' end='03/26/2010' />" +
                    "<week year='2010' month='9' start='03/27/2010' end='04/02/2010' />" +
                    "<week year='2010' month='10' start='04/03/2010' end='04/09/2010' />" +
                    "<week year='2010' month='10' start='04/10/2010' end='04/16/2010' />" +
                    "<week year='2010' month='10' start='04/17/2010' end='04/23/2010' />" +
                    "<week year='2010' month='10' start='04/24/2010' end='04/30/2010' />" +
                    "<week year='2010' month='11' start='05/01/2010' end='05/07/2010' />" +
                    "<week year='2010' month='11' start='05/08/2010' end='05/14/2010' />" +
                    "<week year='2010' month='11' start='05/15/2010' end='05/21/2010' />" +
                    "<week year='2010' month='11' start='05/22/2010' end='05/28/2010' />" +
                    "<week year='2010' month='12' start='05/29/2010' end='06/04/2010' />" +
                    "<week year='2010' month='12' start='06/05/2010' end='06/11/2010' />" +
                    "<week year='2010' month='12' start='06/12/2010' end='06/18/2010' />" +
                    "<week year='2010' month='12' start='06/19/2010' end='06/25/2010' />" +
                    "<week year='2010' month='12' start='06/26/2010' end='07/02/2010' />" +
                "</weeks>" );
        }

        public static FourFourFiveCalendar CreateCalendar()
        {
            var schedule = new List<FiscalYear>();
            var xml = CreateTestData();
            var dates = from year in xml.Descendants( "week" )
                        select new
                        {
                            Year = int.Parse( year.Attribute( "year" ).Value ),
                            Month = int.Parse( year.Attribute( "month" ).Value ),
                            FirstDay = DateTime.Parse( year.Attribute( "start" ).Value ).Date,
                            LastDay = DateTime.Parse( year.Attribute( "end" ).Value ).Date
                        };

            var years = from date in dates
                        orderby date.Year, date.Month, date.FirstDay
                        group date by date.Year into yrs
                        from yr in
                            ( from date in yrs
                              group date by date.Month )
                        group yr by yrs.Key;

            foreach ( var year in years )
            {
                var fiscalYear = new FiscalYear();
                schedule.Add( fiscalYear );

                foreach ( var month in year )
                {
                    var fiscalMonth = new FiscalMonth();
                    fiscalYear.Months.Add( month.Key, fiscalMonth );

                    foreach ( var week in month )
                    {
                        fiscalMonth.Weeks.Add( new FiscalWeek( week.FirstDay, week.LastDay ) );
                    }
                }
            }

            var target = new FourFourFiveCalendar( schedule );
            return target;
        }
    }
}
