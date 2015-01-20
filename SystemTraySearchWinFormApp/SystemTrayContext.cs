using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private MenuItem toggleStartUp, startup, googleSelectorMenu, bingSelectorMenu, duckDuckGoSelectorMenu, wikipediaSelectorMenu, engineSelector;
        private bool _isStartUpEnabled = false;
        private string _engine;


        public event PropertyChangedEventHandler PropertyChanged;
        
        public SystemTrayContext()
        {
            notifyIcon = new NotifyIcon();
            MenuItem exitMenuItem = new MenuItem("Exit", Exit);

            // Engine selector context menu
            googleSelectorMenu = new MenuItem("Google", EngineSelected);
            bingSelectorMenu = new MenuItem("Bing", EngineSelected);
            duckDuckGoSelectorMenu = new MenuItem("Duck Duck Go", EngineSelected);
            wikipediaSelectorMenu = new MenuItem("Wikipedia", EngineSelected);
            engineSelector = new MenuItem("Select Engines", new MenuItem[]{googleSelectorMenu, bingSelectorMenu, duckDuckGoSelectorMenu, wikipediaSelectorMenu});
            //****************************************************************************

            // Start up settings context menu
            toggleStartUp = new MenuItem(GetStartUpStatus(), ToggleStartUp);
            startup = new MenuItem("Start with Windows", new MenuItem[]{toggleStartUp});
            //****************************************************************************
            
            _engine = "Google";
            googleSelectorMenu.Checked = true;

            notifyIcon.Icon = SystemTraySearchWinFormApp.Properties.Resources.sysTraySearchIcon;
            notifyIcon.Text = "Click to Search";
            notifyIcon.Click += NotifyIconOnClick;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]{startup,engineSelector, exitMenuItem});
            notifyIcon.Visible = true;
        }


        private void ToggleStartUp(object sender, EventArgs e)
        {
            if (_isStartUpEnabled == false)
            {
                CreateShortcut();
                toggleStartUp.Text = "Disable";
                _isStartUpEnabled = true;
            }
            else
            {
                RemoveShortcut();
                toggleStartUp.Text = "Enable";
                _isStartUpEnabled = false;
            }
        }

        private void NotifyIconOnClick(object sender, EventArgs eventArgs)
        {
            if ((eventArgs as MouseEventArgs).Button == MouseButtons.Left)
            {
                if (searchForm == null)
                {
                    searchForm = new SearchForm();
                    ShowSearchForm();
                }
                else
                {
                    //if (searchForm.WindowState == FormWindowState.Minimized)
                    //{
                    //    ShowSearchForm();
                    //}
                    //else
                    //{
                    //    searchForm.WindowState = FormWindowState.Minimized;
                    //}
                    searchForm.Dispose();
                    searchForm = null;
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
            (sender as MenuItem).Checked = true;
            SearchEngineCheckChange(sender);
            _engine = (sender as MenuItem).Text;
            SearchForm.FormSearchEngine = _engine;
        }

        private void SearchEngineCheckChange(object sender)
        {
            if (sender == googleSelectorMenu)
            {
                googleSelectorMenu.Checked = true;
                bingSelectorMenu.Checked = false;
                duckDuckGoSelectorMenu.Checked = false;
                wikipediaSelectorMenu.Checked = false;
            }
            if (sender == bingSelectorMenu)
            {
                googleSelectorMenu.Checked = false;
                bingSelectorMenu.Checked = true;
                duckDuckGoSelectorMenu.Checked = false;
                wikipediaSelectorMenu.Checked = false;
            }
            if (sender == duckDuckGoSelectorMenu)
            {
                googleSelectorMenu.Checked = false;
                bingSelectorMenu.Checked = false;
                duckDuckGoSelectorMenu.Checked = true;
                wikipediaSelectorMenu.Checked = false;
            }
            if (sender == wikipediaSelectorMenu)
            {
                googleSelectorMenu.Checked = false;
                bingSelectorMenu.Checked = false;
                duckDuckGoSelectorMenu.Checked = false;
                wikipediaSelectorMenu.Checked = true;
            }
        }

        private void CreateShortcut()
        {
            Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); //Windows Script Host Shell Object
            dynamic shell = Activator.CreateInstance(t);
            try
            {
                var lnk = shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\QuickSearch.lnk");
                try
                {
                    lnk.TargetPath = Application.ExecutablePath;
                    lnk.IconLocation = "shell32.dll, 1";
                    lnk.Save();
                }
                finally
                {
                    Marshal.FinalReleaseComObject(lnk);
                }
            }
            finally
            {
                Marshal.FinalReleaseComObject(shell);
            }
        }

        private void RemoveShortcut()
        {
            string startUpPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (System.IO.File.Exists(Path.Combine(startUpPath, "QuickSearch.lnk")))
            {
                System.IO.File.Delete(Path.Combine(startUpPath, "QuickSearch.lnk"));
            }
        }

        private string GetStartUpStatus()
        {
            string startUpPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (System.IO.File.Exists(Path.Combine(startUpPath, "QuickSearch.lnk")))
            {
                return "Disable";
            }
            else
            {
                return "Enable";
            }
        }
    }
}
