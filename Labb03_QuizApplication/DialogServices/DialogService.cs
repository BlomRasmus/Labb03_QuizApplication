using Labb03_QuizApplication.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb03_QuizApplication.DialogServices
{
    class DialogService : IDialog
    {
        public void ShowPackOptionsDialog()
        {
            var packOptionsDialog = new PackOptionsDialog();

            packOptionsDialog.ShowDialog();
        }

        public void ShowCreatePackModelDialog()
        {
            var createPackOptionsDialog = new CreateNewPackDialog();

            createPackOptionsDialog.ShowDialog();
        }
    }
}
