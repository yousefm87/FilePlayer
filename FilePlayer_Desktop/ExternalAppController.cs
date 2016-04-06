using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlayer
{
    public class ExternalAppController
    {
        private Process appProc = null;

        private string appPath;
        private string filePath;
        private string args;
        private string caption;

        public ExternalAppController(string _appPath, string _filePath, string _args, string _caption)
        {
            appPath = _appPath;
            filePath = _filePath;
            args = _args;
            caption = _caption;
        }


        public void OpenSelectedItemInApp()
        {
            appProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = args + " \"" + filePath + "\"",
                    UseShellExecute = true,
                    CreateNoWindow = false
                }

            };
            

            appProc.Start();
            appProc.WaitForInputIdle();
        }

        public bool CanOpenSelectedItem()
        {
            return File.Exists(appPath) && File.Exists(filePath);
        }

        public void MaximizeCurrentApp()
        {
            WindowActions.MaximizeWindow(caption);
        }

        public void CloseCurrentApplication()
        {
            if (appProc != null)
            {
                if (!appProc.HasExited)
                {
                    appProc.Kill();
                }
            }
        }

    }
}
