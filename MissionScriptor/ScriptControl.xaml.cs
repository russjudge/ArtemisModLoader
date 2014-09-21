using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using RussLibrary.Controls;


namespace MissionStudio
{
    /// <summary>
    /// Interaction logic for ScriptControl.xaml
    /// </summary>
    public partial class ScriptControl : XmlEditor
    {
        public ScriptControl()
        {
            InitializeComponent();
           
        }
        

        //static readonly ILog _log = LogManager.GetLogger(typeof(ScriptControl));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //protected override void OnTextEntered(System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    base.OnTextEntered(e);
        //}
        //protected override void OnTextEntering(System.Windows.Input.TextCompositionEventArgs e)
        //{
        //    base.OnTextEntering(e);
        //}
    }
  

}
