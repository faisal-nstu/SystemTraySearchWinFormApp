using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SystemTraySearchWinFormApp
{
    public partial class SearchForm : Form
    {
        public static string FormSearchEngine = "Google";
        public SearchForm()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            clearButton.Visible = false;
            searchTextbox.KeyPress += SearchTextboxOnKeyPress;
            searchTextbox.TextChanged += SearchTextboxOnTextChanged;
        }

        private void SearchTextboxOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (searchTextbox.Text == "")
            {
                clearButton.Visible = false;
            }
            else
            {
                clearButton.Visible = true;
            }
        }

        private void SearchTextboxOnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
                Search();
        }

        private void Search()
        {
            string searchTerm = searchTextbox.Text;
            if (FormSearchEngine == "Google")
            {
                try
                {
                    var res = "http://google.com/search?q=" + HttpUtility.UrlEncode(searchTerm);
                    Process.Start(res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else if (FormSearchEngine == "Bing")
            {
                try
                {
                    var res = "https://www.bing.com/search?q=" + HttpUtility.UrlEncode(searchTerm);
                    Process.Start(res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else if (FormSearchEngine == "Duck Duck Go")
            {
                try
                {
                    var res = "https://duckduckgo.com/?q=" + HttpUtility.UrlEncode(searchTerm);
                    Process.Start(res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else if (FormSearchEngine == "Wikipedia")
            {
                try
                {
                    var res = "https://en.wikipedia.org/wiki/" + HttpUtility.UrlEncode(searchTerm);
                    Process.Start(res);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            Search();
            
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            searchTextbox.Text = "";
        }
    }
}
