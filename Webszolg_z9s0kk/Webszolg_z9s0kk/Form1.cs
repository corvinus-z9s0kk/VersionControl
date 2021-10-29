using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using Webszolg_z9s0kk.Entities;
using Webszolg_z9s0kk.MnbServiceReference;

namespace Webszolg_z9s0kk
{
    public partial class Form1 : Form
    {
        BindingList<RateData> Rates = new BindingList<RateData>();
        BindingList<string> Currencies = new BindingList<string>();

        public Form1()
        {
            InitializeComponent();
            GetCurrencies();
            RefreshData();
        }

        private void RefreshData()
        {
            Rates.Clear();
            Webszolghivas();
            dataGridView1.DataSource = Rates;
            xmlFeldolgozas();
            diagramKeszites();
        }

        public string Webszolghivas()
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody()
            {
                currencyNames = comboBox1.SelectedItem.ToString(),
                startDate = startDateTimePicker.Value.ToString(),
                endDate = endDateTimePicker.Value.ToString()
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
                if (childElement == null)
                    continue;
                rd.Currency = childElement.GetAttribute("curr");

                var unit = decimal.Parse(childElement.GetAttribute("unit"));
                var value = decimal.Parse(childElement.InnerText);
                if (unit != 0) rd.Value = value / unit;

                Rates.Add(rd);
            }
        }

        private void diagramKeszites()
        {
            chartRateData.DataSource = Rates;

            var series = chartRateData.Series[0];
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        public void GetCurrencies()
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetCurrenciesRequestBody request = new GetCurrenciesRequestBody();
            var response = mnbService.GetCurrencies(request);
            var result = response.GetCurrenciesResult;
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(result);
            foreach (XmlElement item in xml.DocumentElement.ChildNodes[0])
            {
                string elem = item.InnerText;
                Currencies.Add(elem);
            }

            comboBox1.DataSource = Currencies;
        }

        private void startDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void endDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
