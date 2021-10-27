using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Webszolg_z9s0kk.Entities;
using Webszolg_z9s0kk.MnbServiceReference;

namespace Webszolg_z9s0kk
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();

        public Form1()
        {
            InitializeComponent();
            Webszolghivas();
            dataGridView1.DataSource = Rates;
            xmlFeldolgozas();
        }

        private string Webszolghivas()
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody()
            {
                currencyNames = "EUR",
                startDate = "2020-01-01",
                endDate = "2020-06-30"
            };

            var response = mnbService.GetExchangeRates(request);
            var result = response.GetExchangeRatesResult;
            return result;
        }

        private void xmlFeldolgozas()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Webszolghivas());

            foreach (XmlElement element in xml.DocumentElement)
            {
                RateData rd = new RateData();

                rd.Date = DateTime.Parse(element.GetAttribute("date"));

                var childElement = (XmlElement)element.ChildNodes[0];
                rd.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0) rd.Value = value / unit;

                Rates.Add(rd);
            }
        }
    }
}
