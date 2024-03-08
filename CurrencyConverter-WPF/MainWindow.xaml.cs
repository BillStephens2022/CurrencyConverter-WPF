using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConverter_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;    

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }

        private void mycon()
        {
            string Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open();
        }

        private void BindCurrency()
        {

            // Used the below code before hooking  up database, hard coded values
            //DataTable dtCurrency = new DataTable();
            //dtCurrency.Columns.Add("Text");
            //dtCurrency.Columns.Add("Value");
            //// adding rows with the (text, value)
            //dtCurrency.Rows.Add("--SELECT--", 0);
            //dtCurrency.Rows.Add("AUD", 0.66);
            //dtCurrency.Rows.Add("CAD", 0.74);
            //dtCurrency.Rows.Add("CNY", 0.14);
            //dtCurrency.Rows.Add("EUR", 1.09);
            //dtCurrency.Rows.Add("GBP", 1.27);
            //dtCurrency.Rows.Add("INR", 0.012);
            //dtCurrency.Rows.Add("JPY", 0.0068);
            //dtCurrency.Rows.Add("USD", 1);

            mycon();
            DataTable dt = new DataTable(); 
            // Write query to get data from Currency_Master table
            cmd = new SqlCommand("SELECT Id, CurrencyName FROM Currency_Master", con);
            // CommandType define which type of command we use for writing a query
            cmd.CommandType = CommandType.Text;

            // It is accepting a parameter that contains the command text of the object's selectCommand property
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            // Create an object for DataRow
            DataRow newRow = dt.NewRow();
            // Assign value to Id column
            newRow["Id"] = 0;
            // Assign value to CurrencyName column
            newRow["CurrencyName"] = "--SELECT--";

            // Insert a new row in dt with the data at 0 position
            dt.Rows.InsertAt(newRow, 0);

            // dt is not null and rows count > 0
            if (dt != null && dt.Rows.Count > 0)
            {
                // Assign the datatable data to from currency combobox using ItemSource property
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                // Assign the datatable data to to currency combobox using ItemSource property
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }

            con.Close();
           
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;  // sets to the --Select-- text since at index zero of the datatable dtCurrency.

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            // else if currency from is not selected or the default value is selected (i.e. "--SELECT--")
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            // else if currency to is not selected or the default value is selected (i.e. "--SELECT--")
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String To Double.
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show in label converted currency and converted currency name.
                // and ToString("N3") is used to place 000 after after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {

                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }

        
        }

        private void Clear_Click(object sender, RoutedEventArgs e) 
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
      
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) 
        { 

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) 
        { 

        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) 
        { 
        
        }


    }
}
