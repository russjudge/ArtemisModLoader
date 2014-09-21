using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;

namespace ArtemisModLoader.Xml
{
    [XmlConversionRoot("FileMap")]
    public class FileMap : ChangeDependencyObject
    {
        public FileMap()
        {
            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileMap(string source, string target)
        {
            Initialize(source, target, false);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FileMap(string source, string target, bool forSubMod)
        {
            Initialize(source, target, forSubMod);
        }
        void Initialize(string source, string target, bool forSubMod)
        {
            this.BeginInitialization();
            Source = source;
            Target = target;
            ForSubMod = forSubMod;
            AcceptChanges();
            this.EndInitialization();
            
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string),
            typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
         [XmlConversion("Source")]
        public string Source
        {
            get
            {
                return (string)this.UIThreadGetValue(SourceProperty);

            }
            set
            {
                this.UIThreadSetValue(SourceProperty, value);

            }
        }
        public static readonly DependencyProperty TargetProperty =
           DependencyProperty.Register("Target", typeof(string),
           typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("Target")]
        public string Target
        {
            get
            {
                return (string)this.UIThreadGetValue(TargetProperty);

            }
            set
            {
                this.UIThreadSetValue(TargetProperty, value);

            }
        }


        public static readonly DependencyProperty ForSubModProperty =
           DependencyProperty.Register("ForSubMod", typeof(bool),
           typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
          [XmlConversion("ForSubMod")]
        public bool ForSubMod
        {
            get
            {
                return (bool)this.UIThreadGetValue(ForSubModProperty);

            }
            private set
            {
                this.UIThreadSetValue(ForSubModProperty, value);

            }
        }


          protected override void ProcessValidation()
          {
              if (string.IsNullOrEmpty(this.Source))
              {
                  base.ValidationCollection.AddValidation(DataStrings.Source, ValidationValue.IsError, 
                          AMLResources.Properties.Resources.SourceValidation);
              }
              if (string.IsNullOrEmpty(this.Target))
              {
                  base.ValidationCollection.AddValidation(DataStrings.Target, ValidationValue.IsError,
                        AMLResources.Properties.Resources.TargetValidation);
              }
          }
    }
}
