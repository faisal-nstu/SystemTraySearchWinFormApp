using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace SystemTraySearchWinFormApp
{
    public class SystemTrayContext: ApplicationContext
    {
        private NotifyIcon notifyIcon;
        private SearchForm searchForm;
        private bool _isFirstRun = true;

        private string _engine;


        public event PropertyChangedEventHandler PropertyChanged;
        
        public SystemTrayContext()
        {
            notifyIcon = new NotifyIcon();
            MenuItem exitMenuItem = new MenuItem("Exit", Exit);
            MenuItem googleSelectorMenu = new MenuItem("Google", EngineSelected);
            MenuItem bingSelectorMenu = new MenuItem("Bing", EngineSelected);
            MenuItem duckDuckGoSelectorMenu = new MenuItem("Duck Duck Go", EngineSelected);
            MenuItem wikipediaSelectorMenu = new MenuItem("Wikipedia", EngineSelected);
            MenuItem engineSelector = new MenuItem("Select Engines", new MenuItem[]{googleSelectorMenu, bingSelectorMenu, duckDuckGoSelectorMenu, wikipediaSelectorMenu});

            _engine = "Google";
            notifyIcon.Icon = SystemTraySearchWinFormApp.Properties.Resources.sysTraySearchIcon;
            notifyIcon.Text = "Click to Search";
            notifyIcon.Click += NotifyIconOnClick;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]{engineSelector, exitMenuItem});
            notifyIcon.Visible = true;
            //SearchForm._formSearchEngine = Engine;
            
        }

        private void NotifyIconOnClick(object sender, EventArgs eventArgs)
        {
            if (searchForm == null)
            {
                searchForm = new SearchForm();
                ShowSearchForm();
            }
            else
            {
                if (searchForm.WindowState == FormWindowState.Minimized)
                {
                    ShowSearchForm();
                }
                else
                {
                    searchForm.WindowState = FormWindowState.Minimized;
                }
            }
        }

        private void ShowSearchForm()
        {
            searchForm.Show();
            searchForm.WindowState = FormWindowState.Normal;
            var desktopWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            searchForm.Left = desktopWorkingArea.Right - searchForm.Width;
            searchForm.Top = desktopWorkingArea.Bottom - searchForm.Height;
            searchForm.Focus();
        }

        public void ShowContext(object sender, EventArgs eventArgs)
        {
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            Menu aMenu = new MainMenu(new[] { exitMenuItem });
        }

        private void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;

            Application.Exit();
        }

        private void EngineSelected(object sender, EventArgs e)
        {
            _engine = (sender as MenuItem).Text;
            SearchForm.FormSearchEngine = _engine;
        }
    }
}
