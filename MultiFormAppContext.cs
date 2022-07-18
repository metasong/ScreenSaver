using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Metaseed.WebPageScreenSaver
{
    internal class MultiFormAppContext : ApplicationContext
    {
        public MultiFormAppContext(List<Form> forms)
        {
            if (forms == null)
            {
                throw new ArgumentNullException(nameof(forms));
            }

            foreach (var form in forms)
            {
                form.FormClosed += (s, args) => ExitThread();
                form.Show();
            }
        }
    }
}
