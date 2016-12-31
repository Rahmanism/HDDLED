using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HDDLED
{
    /// <summary>
    /// This class pulls HDD activity and shows it with a notification tray icon
    /// You should call Start method after creating an instance of it.
    /// </summary>
    class HddActivityLed
    {
        #region Globals
        NotifyIcon hddNotifyIcon;
        Icon busyIcon, idleIcon;
        Thread hddActivityWorker;
        #endregion

        #region Initialisation
        public HddActivityLed()
        {
            // Load icons from file
            busyIcon = new Icon( "hddBusy.ico" );
            idleIcon = new Icon( "hddIdle.ico" );

            // Create notification tray icon
            hddNotifyIcon = new NotifyIcon();
            hddNotifyIcon.Icon = idleIcon;
            hddNotifyIcon.Visible = true;

            // Create context menu and assign it to notfication icon
            var progNameMenuItem = new MenuItem( "HDD Activity LED, v1.0 Just for test!! ;)" );
            var spacerMenuItem = new MenuItem( "-" );
            var quitMenuItem = new MenuItem( "Quit" );
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add( progNameMenuItem );
            contextMenu.MenuItems.Add( spacerMenuItem );
            contextMenu.MenuItems.Add( quitMenuItem );
            hddNotifyIcon.ContextMenu = contextMenu;

            // Wire up quit menu item to close app
            quitMenuItem.Click += QuitMenuItemClick;
        }
        #endregion

        #region Methods
        public void Start()
        {
            // Start the HDD acitvity thread
            hddActivityWorker = new Thread( new ThreadStart( HddActivityThread ) );
            hddActivityWorker.Start();
        }
        #endregion

        #region Context Menu Event Handlers
        private void QuitMenuItemClick( object sender, EventArgs e )
        {
            hddActivityWorker.Abort();
            hddNotifyIcon.Dispose();
            Application.Exit();
        }
        #endregion

        #region HDD Activity Thread
        /// <summary>
        /// Thread that pulls HDD activity and updates notification tray icon
        /// </summary>
        public void HddActivityThread()
        {
            ManagementClass driveDataClass =
                new ManagementClass( "Win32_PerfFormattedData_PerfDisk_PhysicalDisk" );
            try {
                while ( true ) {
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach ( ManagementObject obj in driveDataClassCollection ) {
                        if ( obj["Name"].ToString() == "_Total" ) {
                            hddNotifyIcon.Icon = Convert.ToUInt64( obj["DiskBytesPersec"] ) > 0 ?
                                busyIcon : idleIcon;
                        }
                    }
                }
            }
            catch ( ThreadAbortException tae ) {
                driveDataClass.Dispose();
            }
        }
        #endregion
    }
}
