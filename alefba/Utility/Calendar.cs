using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alefba.api.Utility
{
    public class Calendar
    {
        public string Miladi(string year, string month, string day)
        {
            //تبدیل تاریخ شمسی به میلادی
            try
            {
                PersianCalendar pc = new PersianCalendar();
                DateTime dt1 = pc.ToDateTime(int.Parse(year), int.Parse(month), int.Parse(day), 0, 0, 0, 0);
                return dt1.Year + "/" + dt1.Month + "/" + dt1.Day;
            }
            catch
            { return ""; }
        }

    }
}
