using System.Diagnostics;

namespace GenieClient.My
{
    internal partial class MyApplication
    {
        [DebuggerStepThrough()]
        public MyApplication() : base(Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
        {
            IsSingleInstance = false;
            EnableVisualStyles = true;
            SaveMySettingsOnExit = false;
            ShutdownStyle = Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
        }

        [DebuggerStepThrough()]
        protected override void OnCreateMainForm()
        {
            MainForm = MyProject.Forms.FormMain;
        }
    }
}
