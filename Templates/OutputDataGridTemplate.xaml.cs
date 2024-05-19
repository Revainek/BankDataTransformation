using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BankDataTransformation.Templates
{
    public partial class OutputDataGridTemplateRD
    {
        private void PreviewDataGrid_AutoGeneratingColEvent(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "entry":
                    e.Column.Visibility = Visibility.Collapsed;
                    break;
                default:
                    e.Column.Visibility = Visibility.Visible;
                    break;
            }
        }
      
    }
}
