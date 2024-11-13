using System.ComponentModel.DataAnnotations;

namespace GeneralValuationSubs.Models
{
    public class FPL9Table
    {
        private string financial_year;
        [Key]
        public int ID { get; set; }
        public DateTime? DocDate { get; set; }

        public string? Type { get; set; }

        public long? DocNo { get; set; }

        public string? Div { get; set; }

        public string? Description { get; set; }

        public double?  Amount { get; set; }

        public string? Financial_Year
        {
            get { return financial_year; }
            set
            {
                if (DocDate >= DateTime.Parse("2009/07/1") && DocDate <= DateTime.Parse("2010/06/30"))
                {
                    financial_year = "2009-2010";
                }
                else if (DocDate >= DateTime.Parse("2010/07/1") && DocDate <= DateTime.Parse("2011/06/30"))
                {
                    financial_year = "2010-2011";
                }
                else if (DocDate >= DateTime.Parse("2011/07/1") && DocDate <= DateTime.Parse("2012/06/30"))
                {
                    financial_year = "2011-2012";
                }
                else if (DocDate >= DateTime.Parse("2012/07/1") && DocDate <= DateTime.Parse("2013/06/30"))
                {
                    financial_year = "2012-2013";
                }
                else if (DocDate >= DateTime.Parse("2013/07/1") && DocDate <= DateTime.Parse("2014/06/30"))
                {
                    financial_year = "2013-2014";
                }
                else if (DocDate >= DateTime.Parse("2014/07/1") && DocDate <= DateTime.Parse("2015/06/30"))
                {
                    financial_year = "2014-2015";
                }
                else if (DocDate >= DateTime.Parse("2015/07/1") && DocDate <= DateTime.Parse("2016/06/30"))
                {
                    financial_year = "2015-2016";
                }
                else if (DocDate >= DateTime.Parse("2016/07/1") && DocDate <= DateTime.Parse("2017/06/30"))
                {
                    financial_year = "2016-2017";
                }
                else if (DocDate >= DateTime.Parse("2017/07/1") && DocDate <= DateTime.Parse("2018/06/30"))
                {
                    financial_year = "2017-2018";
                }
                else if (DocDate >= DateTime.Parse("2018/07/1") && DocDate <= DateTime.Parse("2019/06/30"))
                {
                    financial_year = "2018-2019";
                }
                else if (DocDate >= DateTime.Parse("2019/07/1") && DocDate <= DateTime.Parse("2020/06/30"))
                {
                    financial_year = "2019-2020";
                }
                else if (DocDate >= DateTime.Parse("2020/07/1") && DocDate <= DateTime.Parse("2021/06/30"))
                {
                    financial_year = "2020-2021";
                }
                else if (DocDate >= DateTime.Parse("2021/07/1") && DocDate <= DateTime.Parse("2022/06/30"))
                {
                    financial_year = "2021-2022";
                }
                else if (DocDate >= DateTime.Parse("2022/07/1") && DocDate <= DateTime.Parse("2023/06/30"))
                {
                    financial_year = "2022-2023";
                }
                else if (DocDate >= DateTime.Parse("2023/07/1") && DocDate <= DateTime.Parse("2024/06/30"))
                {
                    financial_year = "2023-2024";
                }
            }
        }

    }
}
