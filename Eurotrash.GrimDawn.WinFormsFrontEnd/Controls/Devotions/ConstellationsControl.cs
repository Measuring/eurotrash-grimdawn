using System.Windows.Forms;

namespace Eurotrash.GrimDawn.WinFormsFrontEnd.Controls.Devotions
{
    public partial class ConstellationsControl : UserControl
    {
        public ConstellationsControl()
        {
            InitializeComponent();
        }

        private void _constellationSearchControl_AddToBuild(Eurotrash.GrimDawn.Core.Build.Devotions.DevotionSelectionAction obj)
        {
            _devotionBuildControl.AddDevotionSelectionAction(obj);
        }

        internal void SetDataSource(Eurotrash.GrimDawn.Core.Build.GrimDawnBuild build)
        {
            _devotionBuildControl.SetDataSource(build);
        }
    }
}
